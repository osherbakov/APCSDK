Option Explicit On 
Option Strict On

Imports System.Reflection

Namespace Diacom.APCStates

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' The class that keeps all command and SQL request filters and queues.
    ''' </summary>
    ''' -----------------------------------------------------------------------------
    Friend Class APCStateLine : Inherits APCLine
        Friend ReadOnly EventFilter As New ChannelEventsFilter(Me)
        Friend ReadOnly SQLFilter As New SQLQueriesFilter(Me)
        Friend ReadOnly spCommand As SPCommandModule

        Friend Sub New(ByVal lineReference As Object, ByVal spCmd As SPCommandModule, ByVal oInitState As String)
            MyBase.New(oInitState)
            MyBase.LineID = lineReference
            spCommand = spCmd
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and Sets the unique line identifier.
        ''' </summary>
        ''' <returns>Unique reference to the line that SP will recognize.</returns>
        ''' -----------------------------------------------------------------------------
        Public ReadOnly Property LineReference() As Object
            Get
                Return MyBase.LineID
            End Get
        End Property
    End Class


    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Internal structure to keep information about scripting calls - 
    ''' Procedure name, on which host, instance of the class and method itself.
    ''' </summary>
    ''' -----------------------------------------------------------------------------
    Friend Class ScriptCallInfo
        Public CallName As String
        Public CallHost As ScriptHost
        Public CallInstance As Object
        Public CallMethod As MethodInfo
    End Class

    '''<summary> 
    ''' The class to hold the method and the call instance object
    ''' for state subroutines calls.
    '''</summary>
    Public Class APCStateControl : Implements IDisposable
        <CLSCompliant(True)> _
        Public Event APCControlEvent As Diacom.SPStatusEventHandler

#Region "Private members"
        Private IsDisposed As Boolean = False
        Private ReadOnly APCLines As New Collections.Hashtable
        Private ReadOnly LangSets As New LanguageSets
        Private ReadOnly StateControlObject As APCLineControl
        Private ReadOnly oneSecondTimer As New System.Threading.Timer(New System.Threading.TimerCallback(AddressOf Me.ProcessGlobalTimer), Me, 1000, 1000)
        Private ReadOnly LoginSP As Diacom.ISP
        Private ReadOnly ApcCmdMod As APCCommandModule
        Private ReadOnly spCmdMod As Diacom.APCStates.SPCommandModule
        Private LinesInitialized As Boolean = False
#End Region

        Dim IV As String = "oB7RrBHs1jAeXfWahD7C6w=="
        Dim UserDataKey As String = "ZWIvFVOr9cR1iy4PuRXr8gF648JKS13SXPhWHZSiyCE="
        Const Const2 As Integer = 3383

        Friend ReadOnly APCStates As New Collections.Hashtable
        Friend ReadOnly APCFunctions As New Collections.Hashtable
        Friend ReadOnly DialFilter As New DialEventsFilter
        Friend ReadOnly AsyncFilter As New AsyncFunctionsFilter

        Public Sub New(ByVal currentSP As Diacom.ISP)
            Me.LoginSP = currentSP
            Me.ApcCmdMod = New Diacom.APCStates.APCCommandModule(Me)
            Me.spCmdMod = New Diacom.APCStates.SPCommandModule(currentSP, Me)
            Me.StateControlObject = New APCLineControl(Me.ApcCmdMod)
        End Sub

        Public Sub Dispose() Implements IDisposable.Dispose
            Dispose(True)
        End Sub

        Private Sub Dispose(ByVal disposing As Boolean)
            If IsDisposed Then Exit Sub
            IsDisposed = True
            GC.SuppressFinalize(Me)
            If (disposing) Then
                oneSecondTimer.Dispose()
                Me.LoginSP.Disconnect()
                Me.LoginSP.Dispose()
            End If
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Connects to the server using SP, provided in a constructor/
        ''' </summary>
        ''' <param name="serverName">Server name or IP address to connect to.</param>
        ''' <param name="param1">Login parameter 1.</param>
        ''' <param name="param2">Login parameter 2.</param>
        ''' <param name="param3">Login parameter 3.</param>
        ''' -----------------------------------------------------------------------------
        Public Sub Connect(ByVal serverName As String, ByVal param1 As String, ByVal param2 As String, ByVal param3 As String)
            Me.LoginSP.Connect(serverName, CInt(param1), Diacom.SPLogonType.ADMINISTRATOR, param2, param3, 15000)
        End Sub

        ''' <summary>
        ''' Adds the script or compiled DLL file to the state machine from the supplied file.
        ''' </summary>
        ''' <param name="fileName">File name for the script.</param>
        Public Sub AddScript(ByVal fileName As String)
            Dim _scriptHost As ScriptHost
            Dim _callinfo As ScriptCallInfo
            Dim _program As String = String.Empty
            Dim _EngineType As String

            ' Check if the filename is OK
            If String.IsNullOrEmpty(fileName) Then
                RaiseEvent APCControlEvent(Me, New Diacom.SPStatusEventArgs(SPStatus.ERROR_INVALID_PARAMETERS, "No Script defined"))
            End If


            ' Select engine type based on the file extension, create appropriate engine
            Select Case System.IO.Path.GetExtension(fileName).Trim.ToUpper
                Case ".JS"
                    _EngineType = "JScript"
                Case ".VBS"
                    _EngineType = "VBScript"
                Case ".VB"
                    _EngineType = "VB.net"
                Case ".JSL"
                    _EngineType = "J#.net"
                Case ".JSC"
                    _EngineType = "JScript.NET"
                Case ".CS"
                    _EngineType = "C#.net"
                Case Else
                    _EngineType = "DLL"
            End Select

            Try
                _scriptHost = ScriptHost.CreateInstance(_EngineType)
            Catch _e As Exception
                DBG("Cannot create Script Engine: " & _e.ToString)
                DBGEX(_e)
                RaiseEvent APCControlEvent(Me, New Diacom.SPStatusEventArgs(SPStatus.ERROR_INVALID_PARAMETERS, "Script Engine Error"))
                Exit Sub
            End Try

            If (_scriptHost.EngineType <> ScriptHost.EngineTypes.COMPILED) Then
                ' Try to open the script file text
                Dim _sr As System.IO.StreamReader
                Try
                    DBG("Opening file: " & fileName)
                    ' Open the script file and read it into the string
                    _sr = New System.IO.StreamReader(GetFullFilePath(fileName), System.Text.Encoding.ASCII)
                    _program = _sr.ReadToEnd
                    _sr.Close()
                Catch _e As Exception
                    DBG("Cannot open the control file: " & fileName)
                    DBGEX(_e)
                    RaiseEvent APCControlEvent(Me, New Diacom.SPStatusEventArgs(SPStatus.ERROR_INVALID_PARAMETERS, "Script Not Found"))
                End Try
            Else
                _program = fileName
            End If

            ' Print the file name and the script source
            DBG("File opened : " & fileName)
            Try
                ' Add GlobalObject, Reference and Program Code to the Scripting Control
                _scriptHost.AddGlobalObject("gAPCLineControl", "Diacom.APCLineControl", StateControlObject)
                _scriptHost.AddReference(StateControlObject.GetType.[Assembly].Location)

                ' Add script to the script engine
                _scriptHost.AddClassCode(_program)

                ' Turn on scripting interpreter
                _scriptHost.Compile()
                _scriptHost.Run()

                ' Iterate thru all modules and procedures, and add the procedure names
                ' to the collection of the names for faster lookup
                Dim _procedure As String
                Dim _split() As String

                For Each _procedure In _scriptHost.Procedures()
                    If Not String.IsNullOrEmpty(_procedure) Then
                        Dim _procCanonicalName As String = _procedure.Trim.ToUpper
                        _callinfo = New ScriptCallInfo
                        _callinfo.CallName = _procCanonicalName
                        _callinfo.CallHost = _scriptHost
                        _callinfo.CallMethod = _scriptHost.GetMethodInfo(_procedure)
                        _callinfo.CallInstance = _scriptHost.GetInstanceInfo(_procedure)
                        DBG("Procedure: " & _procedure)
                        APCStates(_procCanonicalName) = _callinfo
                        ' For main module - also add the Procedure names but without class name
                        _split = _procCanonicalName.Split("."c)
                        If (_split.Length > 1) Then
                            DBG("Procedure: " & _split(1))
                            APCStates(_split(1)) = _callinfo
                        End If
                    End If
                Next

                For Each _function As String In _scriptHost.Functions()
                    If Not String.IsNullOrEmpty(_function) Then
                        Dim _funcCanonicalName As String = _function.Trim.ToUpper
                        _callinfo = New ScriptCallInfo
                        _callinfo.CallName = _funcCanonicalName
                        _callinfo.CallHost = _scriptHost
                        _callinfo.CallMethod = _scriptHost.GetMethodInfo(_function)
                        _callinfo.CallInstance = _scriptHost.GetInstanceInfo(_function)
                        DBG("Function: " & _function)
                        APCFunctions(_funcCanonicalName) = _callinfo
                        ' For main module - also add the Function names but without class name
                        _split = _funcCanonicalName.Split("."c)
                        If (_split.Length > 1) Then
                            DBG("Function: " & _split(1))
                            APCFunctions(_split(1)) = _callinfo
                        End If
                    End If
                Next _function

                ' We should call Main explicitly
                Try
                    Dim _callMethod As MethodInfo
                    Dim _obj As Object
                    _callinfo = CType(APCStates("MAIN"), ScriptCallInfo)
                    If (Not (_callinfo Is Nothing)) Then
                        SyncLock StateControlObject.GetSyncRoot
                            _scriptHost = _callinfo.CallHost
                            _callMethod = _callinfo.CallMethod
                            _obj = _callinfo.CallInstance
                            If (_callMethod Is Nothing) Then
                                _scriptHost.Invoke("Main")
                            Else
                                _callMethod.Invoke(_obj, Nothing)
                            End If
                        End SyncLock
                    Else
                        DBG("No Main() found")
                    End If
                Catch _e As Exception
                    DBGEX(_e)
                    RaiseEvent APCControlEvent(Me, New Diacom.SPStatusEventArgs(SPStatus.ERROR_INVALID_PARAMETERS, "Script Main Error"))
                End Try
            Catch _e As Exception
                DBGEX(_e)
                DBG("Desc: " & _scriptHost.Error.Description)
                DBG("Line :" & _scriptHost.Error.Line & " Column: " & _scriptHost.Error.Column & " Source: " & _scriptHost.Error.Source)
                DBG("Text: " & _scriptHost.Error.Text)
                RaiseEvent APCControlEvent(Me, New Diacom.SPStatusEventArgs(SPStatus.ERROR_INVALID_PARAMETERS, "Script Compilation Error"))
            End Try
        End Sub


        ''' <summary>
        ''' Gets full path for directory entry.
        ''' </summary>
        ''' <param name="FileEntry">Directory entry (full path or not).</param>
        ''' <returns>Full path to directory based on startup path.</returns>
        ''' <remarks>
        ''' Thinking all entries are of the following type: ".\dir" or "dir", and can start and end with "\".
        ''' </remarks>
        Private Function GetFullFilePath(ByVal FileEntry As String) As String
            If String.IsNullOrEmpty(FileEntry) Then Return System.String.Empty
            If System.IO.Path.IsPathRooted(FileEntry) Then
                '' Check if path is full.
                Return FileEntry
            Else
                '' Path is not full.
                Dim SB As System.Text.StringBuilder = New System.Text.StringBuilder(FileEntry)
                If (SB.Chars(0).Equals("."c)) Then
                    '' Starts with ".\".
                    SB.Remove(0, 2)
                End If
                Return System.Windows.Forms.Application.StartupPath + System.IO.Path.DirectorySeparatorChar + SB.ToString()
            End If
        End Function


        ''' <summary>
        ''' Adds the states by parsing all *.DLL files from the supplied directory.
        ''' </summary>
        ''' <param name="dllDirectories">Directories, separated by ";" where state functions are located.</param>
        Public Sub AddStates(ByVal dllDirectories As String)

            Dim fileName As String
            Dim _scriptHost As ScriptHost
            Dim _callinfo As ScriptCallInfo
            Dim _files As String() = {}

            ' Check if the filename is OK
            If String.IsNullOrEmpty(dllDirectories) Then Exit Sub
            ' Create and load the Language rulesets from en-US.config, es-EC.config and so on files
            For Each dllDirectory As String In dllDirectories.Split(";"c)
                Try
                    dllDirectory = FileSearch.GetFullDirPath(dllDirectory)
                    LangSets.Add(dllDirectory)
                Catch ex As Exception
                    DBG("Problem with rulesets directory: " & dllDirectory)
                    DBGEX(ex)
                End Try
            Next

            ' We have to go and check for all procedures and functions in a separate domain,
            '  because if we load any DLL into our AppDomain - we cannot Unload it.
            Try
                Dim ad As AppDomain = AppDomain.CreateDomain("DLL Files Search")
                Dim fst As Type = GetType(FileSearch)
                Dim remclass As FileSearch = CType(ad.CreateInstanceAndUnwrap(fst.[Assembly].FullName, fst.Namespace & "." & fst.Name), FileSearch)
                _files = FileSearch.GetDllFileNames(dllDirectories)
                AppDomain.Unload(ad)
            Catch ex As Exception
                DBG("Cannot obtain filenames remotely")
                DBGEX(ex)
            End Try
            For Each fileName In _files
                Try
                    _scriptHost = ScriptHost.CreateInstance("DLL")
                    _scriptHost.AddGlobalObject("APCLineControl", "Diacom.APCLineControl", StateControlObject)
                    _scriptHost.AddReference(StateControlObject.GetType.[Assembly].Location)
                    _scriptHost.AddModuleCode(fileName)

                    _scriptHost.Compile()
                    _scriptHost.Run()

                    ' Iterate thru all modules and procedures, and add the procedure names
                    ' to the collection of the names for faster lookup
                    For Each _procedure As String In _scriptHost.Procedures()
                        If Not String.IsNullOrEmpty(_procedure) Then
                            Dim _procCanonicalName As String = _procedure.Trim.ToUpper
                            _callinfo = New ScriptCallInfo
                            _callinfo.CallName = _procCanonicalName
                            _callinfo.CallHost = _scriptHost
                            _callinfo.CallMethod = _scriptHost.GetMethodInfo(_procedure)
                            _callinfo.CallInstance = _scriptHost.GetInstanceInfo(_procedure)
                            DBG("Procedure: " & _procedure)
                            APCStates(_procCanonicalName) = _callinfo
                        End If
                    Next _procedure
                    For Each _function As String In _scriptHost.Functions()
                        If Not String.IsNullOrEmpty(_function) Then
                            Dim _funcCanonicalName As String = _function.Trim.ToUpper
                            _callinfo = New ScriptCallInfo
                            _callinfo.CallName = _funcCanonicalName
                            _callinfo.CallHost = _scriptHost
                            _callinfo.CallMethod = _scriptHost.GetMethodInfo(_function)
                            _callinfo.CallInstance = _scriptHost.GetInstanceInfo(_function)
                            DBG("Function: " & _function)
                            APCFunctions(_funcCanonicalName) = _callinfo
                        End If
                    Next _function
                Catch ex As Exception
                    DBG("Cannot retrieve procedures and functions")
                    DBGEX(ex)
                End Try
            Next
        End Sub
        ''' <summary>
        ''' Adds the supplied line to the line events handler.
        ''' </summary>
        ''' <param name="newLine">Line to be added to the collection of controlled lines.</param>
        Public Sub AddLine(ByVal newLine As Diacom.SPLine)
            Dim _addedLine As APCStateLine = New APCStateLine(newLine.ID, spCmdMod, "Z")
            SyncLock StateControlObject.GetSyncRoot
                With _addedLine
                    .LineID = newLine.ID
                    .LineName = newLine.Name
                    .LineNumber = newLine.Number
                    .LinePort = newLine.Port
                    .LineAccessCode = newLine.AccessCode
                    .LineType = newLine.Type
                End With
                ' If there is an existing line - disconnect from events and remove it
                Dim _existingLine As APCStateLine = CType(APCLines(newLine.ID), APCStateLine)
                If Not _existingLine Is Nothing Then
                    RemoveHandler _existingLine.APCStateEvent, AddressOf OnAPCStateEvent
                    APCLines.Remove(newLine.ID)
                End If

                DBG("Added Line : " & newLine.ToString())
                APCLines(newLine.ID) = _addedLine
                AddHandler _addedLine.APCStateEvent, AddressOf OnAPCStateEvent
                If LinesInitialized Then
                    _addedLine.FireStateEvent(_addedLine, APCEvents.NEWSTATE, _addedLine.InitialState, False)
                End If
            End SyncLock
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Initialises all registered lines into initial state
        ''' </summary>
        ''' -----------------------------------------------------------------------------
        Public Sub InitLines()
            SyncLock StateControlObject.GetSyncRoot
                For Each _line As APCStateLine In Me.APCLines.Values
                    _line.FireStateEvent(_line, APCEvents.NEWSTATE, _line.InitialState, False)
                Next
                LinesInitialized = True
            End SyncLock
        End Sub
        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Removes the line from the collection of the lines and unregisters from the event handler.
        ''' </summary>
        ''' <param name="lineRef">Unique ID that identifies the line.</param>
        ''' -----------------------------------------------------------------------------
        Public Sub RemoveLine(ByVal lineRef As Object)
            SyncLock StateControlObject.GetSyncRoot
                Dim _removedLine As APCStateLine = CType(APCLines(lineRef), APCStateLine)
                DBG("Removing : " & _removedLine.ToString())
                APCLines.Remove(lineRef)
                RemoveHandler _removedLine.APCStateEvent, AddressOf OnAPCStateEvent
            End SyncLock
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Set and gets user-specific data in service Provider.
        ''' </summary>
        ''' <param name="lineRef">Unique ID that identifies the line.</param>
        ''' <param name="key">The data reference key.</param>
        ''' <returns>Any type of data previously stored.</returns>
        ''' -----------------------------------------------------------------------------
        Public Property UserData(ByVal lineRef As Object, ByVal key As Object) As Object
            Get
                Return Me.LoginSP(lineRef, key)
            End Get
            Set(ByVal Value As Object)
                Me.LoginSP(lineRef, key) = Value
            End Set
        End Property

        ''' <summary>
        ''' Function to check if the procedure exists in the script module.
        ''' If it does exist - execute it 
        ''' </summary>
        ''' <returns>TRUE - if exists and was successfully executed, FALSE - if doesn't exist or error was encountered during execution.</returns>
        Private Function FindAndExecute(ByVal stateLine As APCLine, ByVal stateName As String) As Boolean
            Dim FullProcName As String = stateLine.FullStateName(stateName)

            Dim _callInfo As ScriptCallInfo = CType(APCStates(FullProcName), ScriptCallInfo)
            If Not (_callInfo Is Nothing) Then
                Dim _callMethod As MethodInfo
                DBG("Procedure found : " & FullProcName)
                stateLine.State = FullProcName
                _callMethod = _callInfo.CallMethod
                If (_callMethod Is Nothing) Then
                    _callInfo.CallHost.Invoke(FullProcName)
                Else
                    _callMethod.Invoke(_callInfo.CallInstance, Nothing)
                End If
                Return True
            Else
                DBG("Procedure is not found : " & FullProcName)
                Return False
            End If
        End Function
        '
        ' Main event processing function
        '
        Private Sub ProcessState(ByVal stateLine As APCLine, ByVal stateSuffix As String, _
                        ByVal recursive As Boolean, ByVal reportError As Boolean)
            Dim _savedStateLine As APCLine = Nothing
            Try
                Dim sName As String = stateLine.State
                _savedStateLine = CType(StateControlObject.StateLine, APCLine)

                StateControlObject.StateLine = stateLine

                StateControlObject.SourceLine.LastEvent = stateSuffix
                StateControlObject.StateLine.LastEvent = stateSuffix

                Do
                    If FindAndExecute(stateLine, sName & stateSuffix) Then Exit Try
                    sName = sName.Substring(0, sName.Length - 1)
                Loop While recursive AndAlso (sName <> String.Empty)
                If reportError Then
                    stateLine.FireStateEvent(StateControlObject.SourceLine, APCEvents.ELSECONTINUE, Nothing, True)
                End If
            Catch _e As Exception
                DBGEX(_e)
                Try
                    stateLine.FireStateEvent(StateControlObject.SourceLine, APCEvents.ELSECONTINUE, Nothing, True)
                Catch ex As Exception
                    DBGEX(ex)
                End Try
            Finally
                StateControlObject.StateLine = _savedStateLine
            End Try
        End Sub

        Friend Sub OnAPCStateEvent(ByVal sender As Object, ByVal e As APCEventArgs)
            SyncLock StateControlObject.GetSyncRoot
                Dim srcLine As APCStateLine = CType(sender, APCStateLine)
                Dim destLine As APCStateLine = CType(e.DestinationLine, APCStateLine)

                DBGLN(destLine, "OnAPCStateEvent [" & e.CallLineFilter.ToString & "] Sender:" & srcLine.LineID.ToString & " State:" & srcLine.State & _
                        " Dest:" & destLine.LineID.ToString & " State:" & destLine.State & " Event: " & _
                                 [Enum].GetName(GetType(APCEvents), e.EventType))
                ' Save the Event origination line and set it to the sender line
                Dim _savedSrcLine As APCLine = CType(StateControlObject.SourceLine, APCLine)
                StateControlObject.SourceLine = srcLine

                If e.CallLineFilter Then
                    destLine.EventFilter.ProcessEvent(srcLine, e.EventType, e.EventParam)
                Else
                    Select Case (e.EventType)
                        Case APCEvents.NEWSTATE
                            Dim newState As String = CStr(e.EventParam).Trim.ToUpper
                            Dim _savedStateLine As APCLine = CType(StateControlObject.StateLine, APCLine)
                            StateControlObject.StateLine = destLine
                            Try
                                FindAndExecute(destLine, newState)
                            Catch ex As Exception
                                DBGEX(ex)
                                Try
                                    destLine.FireStateEvent(srcLine, APCEvents.ELSECONTINUE, Nothing, True)
                                Catch _ex As Exception
                                    DBGEX(_ex)
                                End Try
                            Finally
                                StateControlObject.StateLine = _savedStateLine
                            End Try

                        Case APCEvents.EXECUTE
                            Dim _savedStateLine As APCLine = CType(StateControlObject.StateLine, APCLine)
                            Try
                                Dim _callInfo As ScriptCallInfo = CType(e.EventParam, ScriptCallInfo)
                                Dim _callMethod As MethodInfo = _callInfo.CallMethod

                                StateControlObject.StateLine = destLine
                                destLine.State = _callInfo.CallName
                                If (_callMethod Is Nothing) Then
                                    _callInfo.CallHost.Invoke(_callInfo.CallName)
                                Else
                                    _callMethod.Invoke(_callInfo.CallInstance, Nothing)
                                End If
                            Catch ex As Exception
                                DBGEX(ex)
                                Try
                                    destLine.FireStateEvent(srcLine, APCEvents.ELSECONTINUE, Nothing, True)
                                Catch _ex As Exception
                                    DBGEX(_ex)
                                End Try
                            Finally
                                StateControlObject.StateLine = _savedStateLine
                            End Try

                        Case APCEvents.COMMANDFAIL
                            ProcessState(destLine, "S", True, False)

                        Case APCEvents.RINGBACK
                            DialFilter.ProcessEvent(destLine)

                        Case APCEvents.THENCONTINUE
                            ProcessState(destLine, "Z", False, False)

                        Case APCEvents.ELSECONTINUE
                            ProcessState(destLine, "S", True, False)

                        Case APCEvents.DIGIT
                            Dim _digit As String = CStr(e.EventParam)
                            Dim _suffix As String
                            If (Not _digit Is Nothing) AndAlso (_digit.Length > 0) Then
                                _digit = _digit.Substring(0, 1)
                                Select Case _digit
                                    Case "*"
                                        _suffix = "R"
                                    Case "#"
                                        _suffix = "P"
                                    Case Else
                                        _suffix = _digit
                                End Select
                                ProcessState(destLine, _suffix, False, False)
                            End If

                            ' Commands that are recursive on current state name
                        Case APCEvents.ALERTED
                            If e.EventParam Is Nothing Then
                                ProcessState(destLine, "V", True, False)
                            Else
                                ProcessState(destLine, "V", False, False)
                            End If

                        Case APCEvents.CONNECT
                            ProcessState(destLine, "I", True, False)

                        Case APCEvents.DISCONNECT
                            ProcessState(destLine, "X", True, False)

                        Case APCEvents.DISCONNECTLINK
                            ProcessState(destLine, "Y", True, False)

                        Case APCEvents.QUERYREADY
                            ProcessState(destLine, "Q", True, False)

                        Case APCEvents.RING
                            ProcessState(destLine, "J", True, False)

                        Case APCEvents.WAITERROR
                            ProcessState(destLine, "W", True, False)

                        Case APCEvents.TIMEOUT
                            ProcessState(destLine, "T", True, False)

                            ' Commands that report un-successful search
                        Case APCEvents.LINKREQUEST
                            ProcessState(destLine, "L", True, True)

                        Case APCEvents.GRANTREQUEST
                            ProcessState(destLine, "G", True, True)

                        Case APCEvents.LINKRELEASE
                            ProcessState(destLine, "U", True, False)
                    End Select
                End If
                StateControlObject.SourceLine = _savedSrcLine
            End SyncLock
        End Sub


        Private Sub ProcessGlobalTimer(ByVal source As Object)
            SyncLock StateControlObject.GetSyncRoot
                For Each _line As APCStateLine In APCLines.Values
                    _line.SQLFilter.ProcessTimer()
                    _line.EventFilter.ProcessTimer()
                Next
            End SyncLock
        End Sub

        Default Friend ReadOnly Property Item(ByVal lineRef As Object) As APCStateLine
            Get
                Return CType(APCLines(lineRef), APCStateLine)
            End Get
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Finds the Target line to place the call on.
        ''' </summary>
        ''' <param name="dialNumber">A string that has the number to be dialed.</param>
        ''' <param name="targetFound">A Boolean indicating if target line is found.</param>
        ''' <param name="continueSearch">A Boolean indicating if any more searches are needed.</param>
        ''' <returns>First line that best matches <paramref name="dialNumber"/> string or Null reference.</returns>
        ''' -----------------------------------------------------------------------------
        Friend Function FindDialTargetLine(ByRef dialNumber As String, ByRef targetFound As Boolean, ByRef continueSearch As Boolean) As APCStateLine
            targetFound = False
            continueSearch = False
            Dim _numDigits As Integer = 0
            Dim _numMatch As Integer = 0
            Dim _result As APCStateLine = Nothing
            Dim _dial As String = dialNumber

            ' If dialNumber is empty - return
            If String.IsNullOrEmpty(dialNumber) Then
                dialNumber = String.Empty
                continueSearch = True
                DBG("FindDialTargetLine : Dial:" & dialNumber & " Not Found:" & targetFound & " Cont:" & continueSearch)
                Return Nothing
            End If

            For Each _line As APCStateLine In APCLines.Values
                Dim _currLen As Integer = _line.LineAccessCode.Length
                If (_numDigits < _currLen) AndAlso _line.LineAccessCode.StartsWith(dialNumber) Then
                    _numDigits = _currLen
                End If
                If (_numMatch < _currLen) AndAlso dialNumber.StartsWith(_line.LineAccessCode) Then
                    targetFound = True
                    _numMatch = _currLen
                    _result = _line
                End If
            Next _line

            If _numDigits > dialNumber.Length Then
                continueSearch = True
            End If
            _dial = dialNumber.Substring(_numMatch)

            If (_result Is Nothing) Then
                DBG("FindDialTargetLine : Dial:" & dialNumber & " Not Found : Cont: " & continueSearch)
            Else
                DBGLN(_result, "FindDialTargetLine : Dial: " & dialNumber & " Num:" & _dial & " Found : Cont: " & continueSearch)
            End If

            dialNumber = _dial
            Return _result
        End Function

        Private Shared rxVM As New System.Text.RegularExpressions.Regex("^\s*?(?<Code>V.*M\D*)(?<Num>\d*)", Text.RegularExpressions.RegexOptions.IgnoreCase)
        Private Shared rxAA As New System.Text.RegularExpressions.Regex("^\s*?(?<Code>A.*A\D*)(?<Num>\d*)", Text.RegularExpressions.RegexOptions.IgnoreCase)
        Private Shared rxOP As New System.Text.RegularExpressions.Regex("^\s*?(?<Code>O.*P\D*)(?<Num>\d*)", Text.RegularExpressions.RegexOptions.IgnoreCase)
        Private Shared rxNumber As New System.Text.RegularExpressions.Regex("^\s*?(?<Num>\d+)", Text.RegularExpressions.RegexOptions.IgnoreCase)

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Finds the target for the call transfer
        ''' </summary>
        ''' <param name="dialNumber">A string that has the number to be dialed.</param>
        ''' <param name="targetFound">A Boolean indicating if target line is found.</param>
        ''' <param name="continueSearch">A Boolean indicating if any more searches are needed.</param>
        ''' <param name="TargetType">Output parameter indicating the type of the transfer (see <see cref="Diacom.Cmd.TransferCallType"/> enumeration).</param>
        ''' <returns>First line that best matches <paramref name="dialNumber"/> string or Null reference.</returns>
        ''' -----------------------------------------------------------------------------
        Friend Function FindTransferTargetLine(ByVal oLine As APCStateLine, ByRef dialNumber As String, ByRef targetFound As Boolean, ByRef continueSearch As Boolean, ByRef TargetType As Diacom.Cmd.TransferCallType) As APCStateLine
            targetFound = False
            continueSearch = False
            Dim _match As System.Text.RegularExpressions.Match

            _match = rxVM.Match(dialNumber)
            If _match.Success Then
                TargetType = Cmd.TransferCallType.EXTENSION_VOICE_MESSAGE
                dialNumber = _match.Groups("Num").ToString
                If dialNumber = String.Empty Then
                    targetFound = True
                    continueSearch = False
                    Return oLine
                Else
                    Return FindDialTargetLine(dialNumber, targetFound, continueSearch)
                End If
            End If

            _match = rxAA.Match(dialNumber)
            If _match.Success Then
                dialNumber = _match.Groups("Num").ToString
                TargetType = Cmd.TransferCallType.AUTOATEDDANT
                targetFound = True
                continueSearch = False
                Return oLine
            End If

            _match = rxOP.Match(dialNumber)
            If _match.Success Then
                dialNumber = _match.Groups("Num").ToString
                TargetType = Cmd.TransferCallType.[OPERATOR]
                targetFound = True
                continueSearch = False
                Return oLine
            End If

            _match = rxNumber.Match(dialNumber)
            If _match.Success Then
                dialNumber = _match.Groups("Num").ToString
                Dim _targetLine As APCStateLine = FindDialTargetLine(dialNumber, targetFound, continueSearch)
                If (targetFound) Then
                    If (_targetLine.LineType = "T") Then
                        TargetType = Cmd.TransferCallType.TRUNK
                    Else
                        TargetType = Cmd.TransferCallType.EXTENSION
                    End If
                End If
                Return _targetLine
            End If
            Return Nothing
        End Function

        Public Function Convert(ByVal LanguageName As String, ByVal ConversionName As String, ByVal DataNumber As Object) As String
            Dim ruleSet As LanguageRuleSet = LangSets(LanguageName)
            If Not ruleSet Is Nothing Then
                Return ruleSet.Convert(ConversionName, DataNumber)
            Else
                Return String.Empty
            End If
        End Function
    End Class

    Public Class FileSearch : Inherits MarshalByRefObject
        ''' <summary>
        ''' Returns the states, that contain states and functions by parsing all *.DLL files from the supplied directory.
        ''' </summary>
        ''' <param name="dllDirectories">Directories, separated by ";" where state functions are located.</param>
        Public Shared Function GetDllFileNames(ByVal dllDirectories As String) As String()

            ' Iterate thru all *.DLL files in current script directory and add them with separate script engines
            Dim fileName As String
            Dim _scriptHost As ScriptHost
            Dim FilesList As New Collections.ArrayList

            ' Check if the filename is OK
            If String.IsNullOrEmpty(dllDirectories) Then Return Nothing
            DBG("Dll directories: " + dllDirectories)
            For Each dllDirectory As String In dllDirectories.Split(";"c)
                dllDirectory = GetFullDirPath(dllDirectory)
                If Not String.IsNullOrEmpty(dllDirectory) Then
                    For Each fileName In System.IO.Directory.GetFiles(dllDirectory, "*.DLL")
                        Try
                            Dim FileFound As Boolean = False
                            If Not (fileName.Trim.ToUpper.Equals(dllDirectory.Trim.ToUpper)) Then
                                'Add Code to the script engine
                                _scriptHost = ScriptHost.CreateInstance("DLL")
                                _scriptHost.AddModuleCode(fileName)

                                For Each _procedure As String In _scriptHost.Procedures()
                                    If Not String.IsNullOrEmpty(_procedure) Then
                                        FileFound = True
                                        Exit For
                                    End If
                                Next _procedure
                                For Each _function As String In _scriptHost.Functions()
                                    If Not String.IsNullOrEmpty(_function) Then
                                        FileFound = True
                                        Exit For
                                    End If
                                Next _function
                            End If
                            If FileFound = True Then
                                FilesList.Add(fileName)
                            End If
                        Catch ex As Exception
                            DBG("Cannot load File : " & fileName)
                            DBGEX(ex)
                        End Try
                    Next
                End If
            Next
            Return CType(FilesList.ToArray(GetType(String)), String())
        End Function

        ''' <summary>
        ''' Gets full path for directory entry.
        ''' </summary>
        ''' <param name="DirEntry">Directory entry (full path or not).</param>
        ''' <returns>Full path to directory based on startup path.</returns>
        ''' <remarks>
        ''' Thinking all entries are of the following type: ".\dir" or "dir", and can start and end with "\".
        ''' </remarks>
        Public Shared Function GetFullDirPath(ByVal DirEntry As String) As String
            If String.IsNullOrEmpty(DirEntry) Then
                Return System.String.Empty
            End If
            If System.IO.Path.IsPathRooted(DirEntry) Then
                '' Check if path is full.
                Return DirEntry
            Else
                '' Path is not full.
                Dim SB As System.Text.StringBuilder = New System.Text.StringBuilder(DirEntry)
                If (SB.Chars(0).Equals("."c)) Then
                    '' Starts with ".\".
                    SB.Remove(0, 2)
                End If
                If (SB.Chars(SB.Length - 1).Equals(System.IO.Path.DirectorySeparatorChar)) Then
                    '' Ends with "\" - deleting symbol.
                    SB.Remove(SB.Length - 1, 1)
                End If
                Return System.Windows.Forms.Application.StartupPath + System.IO.Path.DirectorySeparatorChar + SB.ToString()
            End If
        End Function

    End Class
End Namespace