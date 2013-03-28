Imports Diacom
Imports AltiComLib

Namespace Diacom.AltiSDK

    ''' <summary>
    ''' AltiLink Plus v.2 protocol responses on commands codes.
    ''' </summary>
    Friend Enum ALPRespID
        ''' <summary>
        ''' All is OK.
        ''' </summary>
        OK = 0

        ''' <summary>
        ''' ID is invalid.
        ''' </summary>
        INVALID_ID = 3001

        ''' <summary>
        ''' Command is invalid.
        ''' </summary>
        INVALID_CMD = 3002

        ''' <summary>
        ''' Parameter is invalid.
        ''' </summary>
        INVALID_PARAM = 3003

        ''' <summary>
        ''' PWD is invalid.
        ''' </summary>
        INVALID_PWD = 3004

        ''' <summary>
        ''' State is invalid.
        ''' </summary>
        INVALID_STATE = 3005

        ''' <summary>
        ''' Application ID is invalid.
        ''' </summary>
        INVALID_APPID = 3006

        ''' <summary>
        ''' Data is invalid.
        ''' </summary>
        INVALID_DATA = 3007

        ''' <summary>
        ''' Serial number is invalid.
        ''' </summary>
        INVALID_SERIAL = 3008

        ''' <summary>
        ''' System error.
        ''' </summary>
        SYSTEM_ERROR = 3009

        ''' <summary>
        ''' Protocol error.
        ''' </summary>
        PROTOCOL_ERROR = 3010

        ''' <summary>
        ''' Port is out of reach.
        ''' </summary>
        PORT_OUTOFREACH = 3011

        ''' <summary>
        ''' Application has full licence.
        ''' </summary>
        APPID_LICENSE_FULL = 3012

        ''' <summary>
        ''' User is not logged in system.
        ''' </summary>
        NOT_LOGON = 3013

        ''' <summary>
        ''' User already logged in system.
        ''' </summary>
        LOGON_ALREADY = 3014

        ''' <summary>
        ''' Web call rejected.
        ''' </summary>
        WEBCALL_REJECT = 3015

        ''' <summary>
        ''' Echo.
        ''' </summary>
        ECHO = 3016

        ''' <summary>
        ''' Serial exists in system.
        ''' </summary>
        SERIAL_EXIST = 3017

        ''' <summary>
        ''' Insert is duplicated.
        ''' </summary>
        INSERT_DUPLICATE = 3018

        ''' <summary>
        ''' Calls database is overfulled.
        ''' </summary>
        CALLDB_FULL = 3019

        ''' <summary>
        ''' Error to link to database.
        ''' </summary>
        DBLINK_ERROR = 3020

        ''' <summary>
        ''' No record was selected.
        ''' </summary>
        NO_RECORD_SELECTED = 3021

        ''' <summary>
        ''' CDR query data.
        ''' </summary>
        CDRQUERY_DATA = 3022

        ''' <summary>
        ''' Application with this ID is already registered.
        ''' </summary>
        APPID_REGISTERED = 3023

        ''' <summary>
        ''' End of list.
        ''' </summary>
        LIST_END = 3024

        ''' <summary>
        ''' logon failed.
        ''' </summary>
        LOGON_FAILED = 3025

        ''' <summary>
        ''' Call abandoned.
        ''' </summary>
        CALL_ABANDONED = 3026

        ''' <summary>
        ''' Maximum number of connections to database reached.
        ''' </summary>
        MAXDBCON_REACHED = 3027

        ''' <summary>
        ''' Buckup is in processing.
        ''' </summary>
        BACKUP_INPROCESS = 3028

        ''' <summary>
        ''' SQL temporary database is overfulled.
        ''' </summary>
        SQLTEMPDB_FULL = 3029

        ''' <summary>
        ''' No free session.
        ''' </summary>
        NO_MORE_SESSION = 3030

        ''' <summary>
        ''' Option pack is not installed.
        ''' </summary>
        OPTIONPACK_NOT_INSTALLED = 3031

        ''' <summary>
        ''' IP extension not enabled.
        ''' </summary>
        IPEXT_NOTENABLED = 3032

        ''' <summary>
        ''' IP extension is fixed.
        ''' </summary>
        IPEXT_IPFIXED = 3033

        ''' <summary>
        ''' Line is busy.
        ''' </summary>
        LINEBUSY = 3034

        ''' <summary>
        ''' Invalid access control code.
        ''' </summary>
        INVALID_ACCODE = 3035

        ''' <summary>
        ''' Invalid SP controller.
        ''' </summary>
        APC_INVALID_CONTROLLER = 3036

        ''' <summary>
        ''' Extension under SP control is busy.
        ''' </summary>
        APC_EXTENSION_BUSY = 3037

        ''' <summary>
        ''' Access denied.
        ''' </summary>
        ACCESSDENY = 3038

        ''' <summary>
        ''' No basic license.
        ''' </summary>
        NO_BAS_LICENSE = 3039

        ''' <summary>
        ''' No VRD license.
        ''' </summary>
        NO_VRD_LICENSE = 3040

        ''' <summary>
        ''' No SP license.
        ''' </summary>
        NO_APC_LICENSE = 3041

        ''' <summary>
        ''' No ECDR license.
        ''' </summary>
        NO_ECDR_LICENSE = 3042

        ''' <summary>
        ''' No AltiCRT license.
        ''' </summary>
        NO_ACRT_LICENSE = 3043

        ''' <summary>
        ''' No CSLE license.
        ''' </summary>
        NO_CSLE_LICENSE = 3044

        ''' <summary>
        ''' No AltiWeb license.
        ''' </summary>
        NO_AWEB_LICENSE = 3045

        ''' <summary>
        ''' No AltiView license.
        ''' </summary>
        NO_AVIEW_LICENSE = 3046

        ''' <summary>
        ''' No AltiWorkgroup license.
        ''' </summary>
        NO_AWRG_LICENSE = 3047

        ''' <summary>
        ''' Application is not registered.
        ''' </summary>
        NOT_REGISTERED = 3048

        ''' <summary>
        ''' Application is registered alredy.
        ''' </summary>
        ALREADY_REGISTERED = 3049

        ''' <summary>
        ''' There are no more console.
        ''' </summary>
        NO_MORE_CONSOLE = 3050

        ''' <summary>
        ''' Login is blocked.
        ''' </summary>
        LOGIN_ISBLOCKED = 3051

        ''' <summary>
        ''' Login is OK (unsecure password).
        ''' </summary>
        LOGIN_OK_UNSECURE_PWD = 3052

        ''' <summary>
        ''' No IP phone license.
        ''' </summary>
        NO_IPPHONE_LICENSE = 3053

        ''' <summary>
        ''' No priviledge.
        ''' </summary>
        NO_PRIVILEGE = 3056

        ''' <summary>
        ''' Version mismatch warning.
        ''' </summary>
        WARN_VERSIONMISMATCH = 3057

        ''' <summary>
        ''' Version mismatch error.
        ''' </summary>
        ERR_VERSIONMISMATCH = 3058

        ''' <summary>
        ''' System disabled voice recording.
        ''' </summary>
        VR_SYSTEM_DISABLE = 3100

        ''' <summary>
        ''' Voice recording in progress.
        ''' </summary>
        VR_RECORDING_IN_PROGRESS = 3101

        ''' <summary>
        ''' System can't open file for voice recording.
        ''' </summary>
        VR_FILE_CANNOT_OPEN = 3102

        ''' <summary>
        ''' Disk is full for voice recording.
        ''' </summary>
        VR_DISK_FULL = 3103

        ''' <summary>
        ''' Voice recording sevice is not installed.
        ''' </summary>
        VR_VRSERVICE_NOT_INSTALLED = 3104

        ''' <summary>
        ''' Mail box for voice recording is overfulled.
        ''' </summary>
        VR_MAILBOX_IS_FULL = 3105

        ''' <summary>
        ''' Invalid voice recording type.
        ''' </summary>
        VR_INVALID_VRTYPE = 3106

        ''' <summary>
        ''' Voice recording for this extension is disabled.
        ''' </summary>
        VR_EXT_DISABLE = 3107

        ''' <summary>
        ''' Voice recording is not supported.
        ''' </summary>
        VR_NOT_SUPPORTED = 3108

        ''' <summary>
        ''' Voice recording not started.
        ''' </summary>
        VR_NOT_START = 3109

        ''' <summary>
        ''' Voice recording processes maximum riched.
        ''' </summary>
        VR_MAX = 3120

        ''' <summary>
        ''' Error (reason is undefined).
        ''' </summary>
        [ERROR] = 3998

        ''' <summary>
        ''' Unknown error.
        ''' </summary>
        UNKNOWN = 3999
    End Enum


    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Represents SDK-specific Line.
    ''' </summary>
    ''' -----------------------------------------------------------------------------
    Friend Class AltiSDKLine : Inherits SPLine
        Private UserData As New System.Collections.Hashtable
        Public APCHandle As AltiComLib.IAAPC
        Public APCControlled As Boolean = False
        Public UseStopCommand As Boolean = False
        Public LastCommand As Diacom.Cmd.CommandID
        Public ConnectedLine As AltiSDKLine = Nothing
        Public Digits As New System.Collections.Specialized.StringCollection

        Public Sub New(ByVal altiLine As AltiComLib.IALine, _
                            ByVal altiAPCInterface As AltiComLib.IAAPC)
            APCHandle = altiAPCInterface
            MyBase.CalledName = ""
            MyBase.CalledNumber = ""
            MyBase.CIDName = ""
            MyBase.CIDNumber = ""
            MyBase.DIDName = ""
            MyBase.DIDNumber = ""
            MyBase.DNISName = ""
            MyBase.DNISNumber = ""
            MyBase.Name = ""
            If altiLine.FirstName IsNot Nothing Then
                MyBase.Name += altiLine.FirstName & " "
            End If
            If altiLine.LastName IsNot Nothing Then
                MyBase.Name += altiLine.LastName
            End If
            If altiLine.Number Is Nothing Then
                MyBase.Number = ""
            Else
                MyBase.Number = altiLine.Number
            End If
            MyBase.ID = altiLine.Pad
            MyBase.PAD = altiLine.Pad
            If altiLine.PhysicalPort Is Nothing Then
                MyBase.Port = ""
            Else
                MyBase.Port = altiLine.PhysicalPort
            End If
            MyBase.State = SPLineState.IDLE
            MyBase.UserName = ""
            MyBase.UserNumber = ""
            Select Case altiLine.Type
                Case A_LINE_TYPE.A_LINE_APP
                    MyBase.Type = "C"
                    MyBase.AccessCode = altiLine.Number

                Case A_LINE_TYPE.A_LINE_PHYSICALEXT
                    MyBase.Type = "E"
                    MyBase.AccessCode = altiLine.Number

                Case A_LINE_TYPE.A_LINE_VIRTUALEXT
                    MyBase.Type = "V"
                    MyBase.AccessCode = altiLine.Number

                Case A_LINE_TYPE.A_LINE_WORKGROUP
                    MyBase.Type = "W"
                    MyBase.AccessCode = altiLine.Number

                Case A_LINE_TYPE.A_LINE_TRUNK
                    If altiLine.TrunkDirection = A_TRUNK_DIRECTION.A_TRUNK_DIRECTION_PAGING Then
                        MyBase.Type = "P"
                        MyBase.AccessCode = ""
                    ElseIf altiLine.TrunkDirection = A_TRUNK_DIRECTION.A_TRUNK_DIRECTION_IN Then
                        MyBase.Type = "T"
                        MyBase.AccessCode = ""
                    Else
                        MyBase.Type = "T"
                        If altiLine.TrunkAccessCode <= 9 AndAlso _
                            altiLine.TrunkAccessCode >= 0 Then
                            MyBase.AccessCode = altiLine.TrunkAccessCode.ToString("D")
                        Else
                            MyBase.AccessCode = ""
                        End If
                    End If
                Case Else
                    MyBase.Type = ""
                    MyBase.AccessCode = ""
            End Select
        End Sub
        Default Public Property Item(ByVal key As Object) As Object
            Get
                SyncLock (UserData.SyncRoot)
                    Return UserData(key)
                End SyncLock
            End Get
            Set(ByVal Value As Object)
                SyncLock (UserData.SyncRoot)
                    UserData(key) = Value
                End SyncLock
            End Set
        End Property
    End Class


    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Implements the SP class that directly communicates with AltiGen SDK
    ''' </summary>
    ''' -----------------------------------------------------------------------------
    Public Class AltiSDKSP : Implements ISP

        Public Event spEvent As Diacom.SPStatusEventHandler Implements ISP.SPStatusEvent
        Friend spCommands As New System.Collections.ArrayList

#Region "Private fields"
        Private spLines As New System.Collections.Generic.Dictionary(Of Object, Diacom.SPLine)
        Private spStatus As Diacom.SPStatus = spStatus.DISCONNECTED
        Private altiSDKAccess As AltiComLib.AAccessClass
        Private altiSDKSession As AltiComLib.ASession
        Private m_disposed As Boolean = False

        ''' <summary>
        '''Queue for events (incoming).
        ''' </summary>
        Private eventsQueue As New WaitingQueue(Of Diacom.Ev.EvBase)

        ''' <summary>
        ''' Queue for commands (outcoming).
        ''' </summary>
        Private commandsQueue As New WaitingQueue(Of Diacom.Cmd.CmdBase)

        ''' <summary>
        ''' Thread handle for outcoming messages.
        ''' </summary>
        Private outThreadHandle As System.Threading.Thread

        ''' <summary>
        ''' Flag for thread for outcoming messages to end his job.
        ''' </summary>
        Private outThreadTerminate As Boolean = False

        Private ConnectionChecker As System.Threading.Timer
#End Region

        Private Const USER_DATA As String = "UserData"

        Private Class LoginServerClass
            Private m_serverIP As String
            Private m_serverPort As Integer
            Private m_logonType As Diacom.SPLogonType
            Private m_account As String
            Private m_password As String
            Private m_timeout As Integer
            Private m_thread As Threading.Thread

            Public spStatus As Diacom.SPStatus = spStatus.DISCONNECTED
            Public spErrorString As String = ""
            Public altiSDKAccess As AltiComLib.AAccessClass
            Public altiSDKSession As AltiComLib.ASession


            Public Sub New(ByVal serverIP As String, ByVal serverPort As Integer, _
                        ByVal logonType As Diacom.SPLogonType, ByVal account As String, _
                            ByVal password As String, ByVal timeout As Integer)
                Me.m_serverIP = serverIP
                Me.m_serverPort = serverPort
                Me.m_logonType = logonType
                Me.m_account = account
                Me.m_password = password
                Me.m_timeout = timeout
            End Sub

            Public Sub Login()
                m_thread = New Threading.Thread(AddressOf Me.BeginLogin)
                m_thread.SetApartmentState(Threading.ApartmentState.MTA)
                m_thread.Name = "Login Thread"
                m_thread.IsBackground = True
                m_thread.Start()
                If m_timeout = 0 Then
                    m_timeout = System.Threading.Timeout.Infinite
                ElseIf m_timeout < 1000 Then
                    m_timeout = m_timeout * 1000
                End If

                If (m_thread.Join(m_timeout) = False) Then
                    Diacom.TraceOut.Put("Waiting for logon Time exceeded")
                    m_thread.Abort()
                    spErrorString = "Logon Timeout"
                    Me.spStatus = spStatus.ERROR_CONNECTION
                End If
            End Sub

            <MTAThreadAttribute()> _
            Private Sub BeginLogin()
                Dim logonTypeSDK As AltiComLib.A_LOGON_TYPE
                Try
                    Select Case m_logonType
                        Case SPLogonType.AGENT
                            logonTypeSDK = A_LOGON_TYPE.A_LOGON_TYPE_AGENT
                        Case SPLogonType.EXTENSION
                            logonTypeSDK = A_LOGON_TYPE.A_LOGON_TYPE_EXT
                        Case SPLogonType.ADMINISTRATOR
                            logonTypeSDK = A_LOGON_TYPE.A_LOGON_TYPE_ADMIN
                        Case SPLogonType.IP_EXTENSION
                            logonTypeSDK = A_LOGON_TYPE.A_LOGON_TYPE_IPEXT
                        Case SPLogonType.SUPERVISOR
                            logonTypeSDK = A_LOGON_TYPE.A_LOGON_TYPE_SUPERVISOR
                        Case Else
                            spStatus = spStatus.ERROR_LOGON
                            Exit Sub
                    End Select

                    altiSDKAccess = New AltiComLib.AAccessClass
                    altiSDKAccess.Port = m_serverPort
                    altiSDKAccess.Connect(m_serverIP)
                    altiSDKSession = altiSDKAccess.Logon(m_account, m_password, logonTypeSDK)
                    If (altiSDKAccess.IsConnected = False) Then
                        Me.spStatus = spStatus.ERROR_LOGON
                        spErrorString = "Logon Failed"
                        Exit Sub
                    End If

                    ' Tell AlicomLib which events we are interested in
                    altiSDKSession.EventFilter1 = altiSDKSession.EventFilter1 Or _
                                AltiComLib.A_EVENT_TYPE.A_TONEDETECT_EVENT Or _
                                    AltiComLib.A_EVENT_TYPE.A_APC_CMD_RESULT_EVENT Or _
                                        AltiComLib.A_EVENT_TYPE.A_APC_DIGIT_EVENT Or _
                                            AltiComLib.A_EVENT_TYPE.A_APC_STATE_CHANGED Or _
                                                AltiComLib.A_EVENT_TYPE.A_LINE_STATE_CHANGED Or _
                                                    AltiComLib.A_EVENT_TYPE.A_TRUNK_DIGIT_EVENT Or _
                                                        AltiComLib.A_EVENT_TYPE.A_TRUNK_RING_EVENT2

                    altiSDKSession.EventFilter2 = altiSDKSession.EventFilter2 Or AltiComLib.A_EVENT_TYPE.A_TRUNK_RING_EVENT2
                    Me.spStatus = spStatus.OK
                Catch ex As System.Runtime.InteropServices.COMException
                    Diacom.TraceOut.Put(ex)
                    Me.spStatus = spStatus.ERROR_LOGON
                    spErrorString = ex.Message
                Catch ex As Exception
                    Diacom.TraceOut.Put(ex)
                    Me.spStatus = spStatus.ERROR_LOGON
                    spErrorString = "Logon Error"
                End Try
            End Sub
        End Class

        Private Sub Initialize(ByVal serverIP As String, ByVal serverPort As Integer, _
                    ByVal logonType As Diacom.SPLogonType, ByVal account As String, _
                        ByVal password As String, ByVal timeout As Integer)
            Try
                If altiSDKAccess IsNot Nothing Then
                    Me.Disconnect()
                End If
                spStatus = spStatus.DISCONNECTED

                Dim loginObj As New LoginServerClass(serverIP, serverPort, _
                                                logonType, account, password, timeout)
                loginObj.Login()
                spStatus = loginObj.spStatus
                If (spStatus <> spStatus.OK) Then
                    RaiseEvent spEvent(Me, New SPStatusEventArgs(spStatus.ERROR_LOGON, loginObj.spErrorString))
                    Exit Sub
                End If

                altiSDKAccess = loginObj.altiSDKAccess
                altiSDKSession = loginObj.altiSDKSession
                ' Attach event handler to COM object
                AddHandler altiSDKSession.Event, AddressOf Me.SDKEventHandler

                Dim _spLine As AltiSDKLine
                Dim _altiInterface As AltiComLib.IAAPC
                Dim _ar As System.Collections.IEnumerable = CType(altiSDKSession.Lines(), IEnumerable)
                For Each _sdkLine As AltiComLib.IALine In _ar
                    _altiInterface = Me.GetAPCControl()
                    _spLine = New AltiSDKLine(_sdkLine, _altiInterface)
                    Me.Item(_sdkLine.Pad) = _spLine
                Next
                Me.outThreadHandle = New System.Threading.Thread(AddressOf Me.CommandsHandler)
                Me.outThreadHandle.Name = "SDK Commands thread"
                Me.outThreadHandle.SetApartmentState(Threading.ApartmentState.MTA)
                Me.outThreadTerminate = False
                Me.outThreadHandle.Start()

                ' Start the connection checking timer
                Me.ConnectionChecker = New System.Threading.Timer( _
                    New System.Threading.TimerCallback(AddressOf Me.CheckConnection), Nothing, 30000, 30000)
                spStatus = spStatus.OK
                RaiseEvent spEvent(Me, New SPStatusEventArgs(spStatus.OK, "Logon Success"))
            Catch ex As System.Runtime.InteropServices.COMException
                Diacom.TraceOut.Put(ex)
                spStatus = spStatus.ERROR_LOGON
                RaiseEvent spEvent(Me, New SPStatusEventArgs(spStatus.ERROR_LOGON, ex.Message))
            Catch ex As Exception
                Diacom.TraceOut.Put(ex)
                spStatus = spStatus.ERROR_LOGON
                RaiseEvent spEvent(Me, New SPStatusEventArgs(spStatus.ERROR_LOGON, "Logon Failure"))
            End Try
        End Sub


        Public Sub Connect(ByVal serverIP As String, ByVal serverPort As Integer, _
                    ByVal logonType As Diacom.SPLogonType, ByVal account As String, _
                        ByVal password As String, ByVal timeout As Integer) Implements Diacom.ISP.Connect
            Initialize(serverIP, serverPort, logonType, account, password, timeout)
        End Sub

        Public Sub Disconnect() Implements ISP.Disconnect
            If (altiSDKSession IsNot Nothing) Then
                Try
                    ' RemoveHandler altiSDKSession.Event, AddressOf Me.SDKEventHandler
                    altiSDKSession.Dispose()
                    altiSDKSession = Nothing
                Catch _e As Exception
                    Diacom.TraceOut.Put(_e)
                End Try
            End If

            If (altiSDKAccess IsNot Nothing) Then
                Try
                    altiSDKAccess.Disconnect()
                    altiSDKAccess = Nothing
                Catch ex As Exception
                    Diacom.TraceOut.Put(ex)
                End Try
            End If
            spStatus = spStatus.DISCONNECTED

            ' Dim pUnk As System.IntPtr
            ' pUnk = System.Runtime.InteropServices.Marshal.GetIUnknownForObject(altiSDKSession)
            'System.Runtime.InteropServices.Marshal.ReleaseComObject(altiSDKSession)
            ' While System.Runtime.InteropServices.Marshal.Release(pUnk) <> 0
            ' End While
            ' While System.Runtime.InteropServices.Marshal.ReleaseComObject(altiSDKSession) <> 0
            ' End While
            'System.Runtime.InteropServices.Marshal.ReleaseComObject(altiSDKAccess)
            ' pUnk = System.Runtime.InteropServices.Marshal.GetIUnknownForObject(altiSDKAccess)
            ' While System.Runtime.InteropServices.Marshal.Release(pUnk) <> 0
            ' End While
            ' While System.Runtime.InteropServices.Marshal.ReleaseComObject(altiSDKAccess) <> 0
            ' End While
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Implements <see cref="Diacom.ISP.Receive"/> interface to receive events from SP.
        ''' </summary>
        ''' <returns>An event from SP of type derived from <see cref="Diacom.Ev.EvBase"/> class.</returns>
        ''' -----------------------------------------------------------------------------
        Public Function Receive() As Diacom.Ev.EvBase Implements ISP.Receive
            Return CType(Me.eventsQueue.Dequeue(), Diacom.Ev.EvBase)
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Implements <see cref="Diacom.ISP.Send"/> interface to send commands to SP.
        ''' </summary>
        ''' <param name="command"> Command derived from <see cref="Diacom.Cmd.CmdBase"/> class.</param>
        ''' -----------------------------------------------------------------------------
        Public Sub Send(ByVal command As Diacom.Cmd.CmdBase) Implements ISP.Send
            Me.commandsQueue.Enqueue(command)
        End Sub

        Public Function Status() As Diacom.SPStatus Implements ISP.Status
            Return Me.spStatus
        End Function
        Default Public Property Item(ByVal lineID As Object, ByVal key As Object) As Object Implements Diacom.ISP.Item
            Get
                Return Me(lineID)(key)
            End Get
            Set(ByVal Value As Object)
                Dim _line As AltiSDKLine = Me(lineID)
                _line(key) = Value
                If (TypeOf key Is String) AndAlso CStr(key).Equals(USER_DATA) Then
                    Dim ac As AsyncSetUserInfoCommand = _
                        New AsyncSetUserInfoCommand(_line, Me, CStr(Value))
                End If
            End Set
        End Property

#Region "IDisposable implementation"
        Public Sub Dispose() Implements IDisposable.Dispose
            Me.Dispose(True)
        End Sub
        Protected Overridable Sub Dispose(ByVal disposing As Boolean)
            If m_disposed Then Exit Sub
            m_disposed = True
            GC.SuppressFinalize(Me)
            If disposing Then
                spLines.Clear()
                Me.ConnectionChecker.Dispose()
                Me.outThreadTerminate = True
            End If
            Me.Disconnect()
        End Sub
        Protected Overrides Sub Finalize()
            Me.Dispose(False)
        End Sub
#End Region

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the line based on unique reference object.
        ''' </summary>
        ''' <param name="lineID">Unique reference.</param>
        ''' <returns>A Line of type <see cref="Diacom.SPLine"/></returns>
        ''' -----------------------------------------------------------------------------
        Public Function GetLine(ByVal lineID As Object) As Diacom.SPLine Implements ISP.GetLine
            Dim _result As SPLine = Nothing
            If spLines Is Nothing Then Return Nothing
            SyncLock spLines
                _result = CType(spLines(lineID).Clone, SPLine)
            End SyncLock
            Return _result
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Returns an array of all lines registered with APC
        ''' </summary>
        ''' <returns>An array of type <see cref="Diacom.SPLine"/></returns>
        ''' -----------------------------------------------------------------------------
        Public Function GetLines() As Diacom.SPLine() Implements ISP.GetLines
            Dim _result() As SPLine = Nothing
            If spLines Is Nothing Then Return Nothing
            SyncLock spLines
                If (spLines.Count > 0) Then
                    ReDim _result(spLines.Count - 1)
                    Dim _index As Integer = 0
                    For Each _Line As Diacom.SPLine In spLines.Values
                        _result(_index) = CType(_Line.Clone(), Diacom.SPLine)
                        _index = _index + 1
                    Next _Line
                    Array.Reverse(_result)
                End If
            End SyncLock
            Return _result
        End Function

        Default Friend Property Item(ByVal lineID As Object) As AltiSDK.AltiSDKLine
            Get
                If spLines Is Nothing Then Return Nothing
                SyncLock spLines
                    Return CType(spLines(lineID), AltiSDK.AltiSDKLine)
                End SyncLock
            End Get
            Set(ByVal Value As AltiSDK.AltiSDKLine)
                If spLines Is Nothing Then Exit Property
                SyncLock spLines
                    spLines(lineID) = Value
                End SyncLock
            End Set
        End Property

        Friend Function GetAPCControl() As AltiComLib.IAAPC
            Return CType(altiSDKSession.GetServiceObj(A_SERVICE_TYPE.A_SERVICE_APC), AltiComLib.IAAPC)
        End Function

        Friend Function GetAPCLine(ByVal lineHandle As Integer) As AltiComLib.IALine
            Return altiSDKSession.FindLineFromPad(lineHandle)
        End Function

        Public Sub ReportError(ByVal source As Object, ByVal commandID As Diacom.Cmd.CommandID)
            Me.eventsQueue.Enqueue(New Diacom.Ev.CommandStatus(source, commandID, Ev.CmdStatus.ERROR))
        End Sub

        Private Sub SDKEventHandler(ByVal iType As AltiComLib.A_EVENT_TYPE, ByVal vData As Object)
            Select Case iType
                Case A_EVENT_TYPE.A_APC_CMD_RESULT_EVENT
                    Dim _ev As AltiComLib.AAPCCmdResult = CType(vData, AAPCCmdResult)
                    Dim _line As Diacom.AltiSDK.AltiSDKLine = Me.Item(_ev.CallHandle)

                    Diacom.TraceOut.Put("APC Cmd result on :" + _ev.CallHandle.ToString + " for Cmd : " + _ev.CommandType.ToString + " Result : " + _ev.CommandResult.ToString + " Return : " + _ev.ReturnValue.ToString)
                    _Line.UseStopCommand = False
                    Select Case _ev.CommandResult
                        Case A_APC_CMD_RESULT.A_APC_CMD_RESULT_FAILED
                            If (_ev.CommandType <> A_APC_CMD_TYPE.A_APC_CMD_STOPPLAYVOICE AndAlso _
                                        _ev.CommandType <> A_APC_CMD_TYPE.A_APC_CMD_STOPRECORDVOICE) Then
                                Me.eventsQueue.Enqueue(New Diacom.Ev.CommandStatus(_ev.CallHandle, _line.LastCommand, Ev.CmdStatus.ERROR))
                            End If

                        Case A_APC_CMD_RESULT.A_APC_CMD_RESULT_STARTED
                            _Line.UseStopCommand = True

                        Case A_APC_CMD_RESULT.A_APC_CMD_RESULT_FINISHED
                            Me.eventsQueue.Enqueue(New Diacom.Ev.CommandStatus(_ev.CallHandle, _line.LastCommand, Ev.CmdStatus.OK))

                        Case A_APC_CMD_RESULT.A_APC_CMD_RESULT_SUCCEED
                            If (_ev.CommandType <> A_APC_CMD_TYPE.A_APC_CMD_STOPPLAYVOICE AndAlso _
                                        _ev.CommandType <> A_APC_CMD_TYPE.A_APC_CMD_STOPRECORDVOICE) Then
                                Me.eventsQueue.Enqueue(New Diacom.Ev.CommandStatus(_ev.CallHandle, _line.LastCommand, Ev.CmdStatus.OK))
                            End If
                    End Select

                Case A_EVENT_TYPE.A_APC_DIGIT_EVENT
                    Dim _ev As AltiComLib.AAPCDigit = CType(vData, AAPCDigit)
                    Diacom.TraceOut.Put("APC Digit on :" + _ev.CallHandle.ToString + " Digit : " + _ev.Digit.ToString)
                    Dim _line As Diacom.AltiSDK.AltiSDKLine = Me.Item(_ev.CallHandle)
                    If _line.APCControlled Then
                        Me.eventsQueue.Enqueue(New Diacom.Ev.Digit(_ev.CallHandle, _ev.Digit.Chars(0)))
                    Else
                        _line.Digits.Add(_ev.Digit.Chars(0))
                    End If

                Case A_EVENT_TYPE.A_APC_STATE_CHANGED
                    Dim _ev As AltiComLib.AAPCState = CType(vData, AAPCState)
                    Dim _line As Diacom.AltiSDK.AltiSDKLine = Me.Item(_ev.CallHandle)
                    Diacom.TraceOut.Put("APC State on :" + _ev.CallHandle.ToString + " State : " + _ev.CurState.ToString)

                    Select Case _ev.CurState
                        Case A_APC_STATE.A_APC_STATE_CALLPRESENT
                            Try
                                _line.APCHandle = GetAPCControl()
                                _line.APCHandle.Attach(_ev.CallHandle)
                            Catch ex As Exception
                                Diacom.TraceOut.Put(ex)
                                Diacom.TraceOut.Put("Cannot attach APC handle on Call present")
                                Exit Sub
                            End Try

                            If Not _line.APCControlled Then
                                Dim _callInfo As AltiComLib.IAAPCCallPropt
                                Dim _spLine As New Diacom.SPLine
                                _spLine.ID = _ev.CallHandle
                                Try
                                    _line.Digits.Clear()
                                    _callInfo = _line.APCHandle.CallProperty()
                                    If Not _callInfo.CallNumber Is Nothing Then _spLine.CIDNumber = _callInfo.CallNumber
                                    If Not _callInfo.DNISNum Is Nothing Then _spLine.DNISNumber = _callInfo.DNISNum
                                    If Not _callInfo.CallNumber Is Nothing Then _spLine.DIDNumber = _callInfo.CallNumber
                                Catch ex As Exception
                                    Diacom.TraceOut.Put(ex)
                                    Diacom.TraceOut.Put("Cannot get CallProperty")
                                End Try
                                Diacom.TraceOut.Put("Call present from : " + _spLine.CIDNumber)
                                Me.eventsQueue.Enqueue(New Diacom.Ev.Ring(_ev.CallHandle, _spLine))
                            End If

                        Case A_APC_STATE.A_APC_STATE_CONNECT
                            Try
                                _line.APCHandle.Attach(_ev.CallHandle)
                            Catch ex As Exception
                                Diacom.TraceOut.Put(ex)
                                Diacom.TraceOut.Put("Cannot attach APC handle on APC_State_Connect")
                                Exit Sub
                            End Try
                            If Not _line.APCControlled Then
                                Diacom.TraceOut.Put("Line is APC Controlled")
                                _line.APCControlled = True
                                Me.eventsQueue.Enqueue(New Diacom.Ev.Connect(_ev.CallHandle))
                                For Each _digit As String In _line.Digits
                                    Me.eventsQueue.Enqueue(New Diacom.Ev.Digit(_ev.CallHandle, _digit.Chars(0)))
                                Next
                            ElseIf Not _line.ConnectedLine Is Nothing Then
                                Me.eventsQueue.Enqueue(New Diacom.Ev.CommandStatus(_line.ConnectedLine.ID, Cmd.CommandID.DISCONNECT_LINE, Ev.CmdStatus.OK))
                                _line.ConnectedLine = Nothing
                            End If

                        Case A_APC_STATE.A_APC_STATE_DROP
                            Diacom.TraceOut.Put("A_APC_STATE.A_APC_STATE_DROP")

                        Case A_APC_STATE.A_APC_STATE_RINGBACK
                            Try
                                _line.APCHandle.Attach(_ev.CallHandle)
                            Catch ex As Exception
                                Diacom.TraceOut.Put(ex)
                                Diacom.TraceOut.Put("Cannot attach APC handle on Ringback")
                                Exit Sub
                            End Try
                            Dim _callInfo As AltiComLib.IAAPCCallPropt = _line.APCHandle.CallProperty()
                            Dim _spLine As New Diacom.SPLine
                            _spLine.ID = _ev.CallHandle
                            If Not _callInfo.CallNumber Is Nothing Then _spLine.CalledNumber = _callInfo.CallNumber
                            If _line.Type.Equals("T") Then
                                _spLine.CalledNumber = _line.AccessCode & _spLine.CalledNumber
                            End If
                            If Not _callInfo.DNISNum Is Nothing Then _spLine.DNISNumber = _callInfo.DNISNum
                            If Not _callInfo.ANINum Is Nothing Then _spLine.DIDNumber = _callInfo.ANINum
                            Me.eventsQueue.Enqueue(New Diacom.Ev.RingBack(_ev.CallHandle, _spLine))

                        Case A_APC_STATE.A_APC_STATE_FAILED
                            Diacom.TraceOut.Put("!!!!!!APC Failed on :" + _ev.CallHandle.ToString + "!!!!")

                    End Select

                Case A_EVENT_TYPE.A_TONEDETECT_EVENT
                    Dim _ev As AltiComLib.ToneEvent = CType(vData, ToneEvent)
                    Diacom.TraceOut.Put("ToneDetect on :" + _ev.Pad.ToString + " Tone : " + _ev.Tone.ToString)
                    Me.eventsQueue.Enqueue(New Diacom.Ev.Tone(_ev.Pad, CType(_ev.Tone, Diacom.Ev.ToneType)))

                Case A_EVENT_TYPE.A_COMMUNICATION_FAIL
                    Diacom.TraceOut.Put("!!!!Communications failure!!!!")
                    RaiseEvent spEvent(Me, New SPStatusEventArgs(spStatus.ERROR_CONNECTION, "Communications Failure"))

                Case A_EVENT_TYPE.A_TRUNK_DIGIT_EVENT
                    Dim _ev As AltiComLib.ATrunkDigitEvent = CType(vData, ATrunkDigitEvent)
                    Dim _line As Diacom.AltiSDK.AltiSDKLine = Me.Item(_ev.TrunkPad)
                    Diacom.TraceOut.Put("Trunk Digit on :" + _ev.TrunkPad.ToString + " Digit : " + _ev.TrunkDigit.ToString)

                Case A_EVENT_TYPE.A_TRUNK_RING_EVENT2
                    Dim _ev As AltiComLib.ATrunkRingEvent = CType(vData, ATrunkRingEvent)
                    Diacom.TraceOut.Put("!!!!!!Trunk Ring on :" + _ev.CallHandle.ToString + "!!!!")

                Case A_EVENT_TYPE.A_LINE_STATE_CHANGED
                    Dim _ev As AltiComLib.LineEvent = CType(vData, LineEvent)
                    Dim _line As Diacom.AltiSDK.AltiSDKLine = Me.Item(_ev.LinePad)
                    Diacom.TraceOut.Put("LineState change on :" + _ev.LinePad.ToString + " State : " + _ev.CurState.ToString)
                    If (_line.APCControlled AndAlso (_ev.CurState = A_LINE_STATE.A_LINE_STATE_IDLE)) Then
                        Diacom.TraceOut.Put("A_LINE_STATE_CHANGED : Line is not APC Controlled")
                        _line.APCControlled = False
                        _line.APCHandle.Detach()
                        _line.APCHandle = Nothing
                        Me.eventsQueue.Enqueue(New Diacom.Ev.Disconnect(_ev.LinePad))
                        'ElseIf (_ev.CurState = A_LINE_STATE.A_LINE_STATE_CONNECTED AndAlso _
                        '       _Line.LastCommand = Cmd.CommandID.TRANSFER) Then
                        'Me.eventsQueue.Enqueue(New Diacom.Ev.CommandStatus(_ev.LinePad, Cmd.CommandID.TRANSFER, Ev.CmdStatus.OK))
                    End If
                    Me.eventsQueue.Enqueue(New Diacom.Ev.LineStateChanged(_ev.LinePad, CType(_ev.CurState, Diacom.SPLineState)))

                    ' Case A_EVENT_TYPE.A_CALL_STATE_CHANGED
                    ' Dim _ev As AltiComLib.CallEvent = CType(vData, CallEvent)
                    ' Diacom.TraceOut.Put("CallState change on :" + _ev.LinePad.ToString + " State : " + _ev.CurState.ToString)

                Case A_EVENT_TYPE.A_SYS_CONFIG_CHANGED
                    ' Line was either added or deleted
                    Dim _ev As AltiComLib.ASysConfigEvent = CType(vData, ASysConfigEvent)
                    Select Case _ev.ConfigChangedType
                        Case A_SYSCFG_CHG_TYPE.A_SYSCFG_CHG_TYPE_LINEADD
                            Dim aglic As AsyncGetLineInfoCommand = New AsyncGetLineInfoCommand(_ev.Pad, Me)

                        Case A_SYSCFG_CHG_TYPE.A_SYSCFG_CHG_TYPE_LINEREMOVE
                            Me.eventsQueue.Enqueue(New Diacom.Ev.LineStateChanged(_ev.Pad, SPLineState.LINE_REMOVE))
                    End Select
            End Select
        End Sub

        Private Sub CommandsHandler()
            Dim _cmd As Diacom.Cmd.CmdBase
            While Not Me.outThreadTerminate
                Try
                    _cmd = CType(Me.commandsQueue.Dequeue(), Diacom.Cmd.CmdBase)
                    Dim _asyncCmd As New AsyncSPCommand(_cmd, Me)
                    GC.KeepAlive(_asyncCmd)
                Catch ex As Exception
                    Diacom.TraceOut.Put(ex)
                End Try
            End While
        End Sub

        Private Sub CheckConnection(ByVal state As Object)
            Try
                Dim Timestamp As Integer = Me.altiSDKSession.TimeStamp()
                Diacom.TraceOut.Put("TimeStamp Command issued")
            Catch ex As Exception
                Diacom.TraceOut.Put(ex)
                RaiseEvent spEvent(Me, New SPStatusEventArgs(spStatus.ERROR_CONNECTION, "Timestamp Timeout"))
            End Try
        End Sub

        ' Class to set a user info 
        Private Class AsyncSetUserInfoCommand
            Private ReadOnly m_Line As AltiSDKLine
            Private ReadOnly m_AltiSP As AltiSDKSP
            Private ReadOnly m_Data As String
            Public Sub New(ByVal sdkLine As AltiSDKLine, ByVal altiSP As AltiSDKSP, ByVal Data As String)
                m_Line = sdkLine
                m_AltiSP = altiSP
                m_Data = Data
                m_AltiSP.spCommands.Add(Me)
                System.Threading.ThreadPool.QueueUserWorkItem(AddressOf Me.AsyncInvoke)
            End Sub

            Private Sub AsyncInvoke(ByVal stateInfo As Object)
                Try
                    Dim _callInfo As AltiComLib.IAAPCCallPropt = m_Line.APCHandle.CallProperty()
                    _callInfo.UserData = m_Data
                Catch ex As Exception
                    Diacom.TraceOut.Put(ex)
                Finally
                    m_AltiSP.spCommands.Remove(Me)
                End Try
            End Sub
        End Class


        ' Class whose only purpose in life is to get a line info and to add it to the list of lines
        Private Class AsyncGetLineInfoCommand
            Private ReadOnly m_LineID As Integer
            Private ReadOnly m_AltiSP As AltiSDKSP
            Public Sub New(ByVal LineID As Integer, ByVal altiSP As AltiSDKSP)
                m_LineID = LineID
                m_AltiSP = altiSP
                m_AltiSP.spCommands.Add(Me)
                System.Threading.ThreadPool.QueueUserWorkItem(AddressOf Me.AsyncInvoke)
            End Sub

            Private Sub AsyncInvoke(ByVal stateInfo As Object)
                Dim _spLine As Diacom.AltiSDK.AltiSDKLine
                Dim _apcControl As AltiComLib.IAAPC
                Try
                    _apcControl = m_AltiSP.GetAPCControl()
                    _spLine = New AltiSDKLine(m_AltiSP.GetAPCLine(m_LineID), _apcControl)
                    m_AltiSP.Item(m_LineID) = _spLine
                    m_AltiSP.eventsQueue.Enqueue(New Diacom.Ev.LineStateChanged(m_LineID, SPLineState.LINE_ADD))
                Catch ex As Exception
                    Diacom.TraceOut.Put(ex)
                Finally
                    m_AltiSP.spCommands.Remove(Me)
                End Try
            End Sub
        End Class

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Implements the command to be executed asynchronously by the thread from the thread pool.
        ''' </summary>
        ''' -----------------------------------------------------------------------------
        Private Class AsyncSPCommand
            Private ReadOnly m_Cmd As Diacom.Cmd.CmdBase
            Private ReadOnly m_AltiSP As AltiSDKSP

            Public Sub New(ByVal command As Diacom.Cmd.CmdBase, ByVal altiSP As AltiSDKSP)
                m_Cmd = command
                m_AltiSP = altiSP
                m_AltiSP.spCommands.Add(Me)
                System.Threading.ThreadPool.QueueUserWorkItem(AddressOf Me.AsyncInvoke)
            End Sub

            Private Sub AsyncInvoke(ByVal stateInfo As Object)
                Dim _handle As Integer
                Dim _line As Diacom.AltiSDK.AltiSDKLine = Nothing
                Dim _apcControl As AltiComLib.IAAPC

                Try
                    _line = m_AltiSP.Item(m_Cmd.Line)
                    If _line Is Nothing Then Exit Try

                    _handle = CInt(_line.ID)

                    _apcControl = _line.APCHandle
                    If _apcControl Is Nothing Then
                        Me.m_AltiSP.ReportError(_line.ID, m_Cmd.ID)
                        Exit Try
                    End If

                    Select Case m_Cmd.ID

                        Case Cmd.CommandID.ANSWER_CALL
                            Diacom.TraceOut.Put("Line : " & _handle.ToString & " : Answer Command issued")
                            _apcControl.AnswerCall(_handle)
                            Me.m_AltiSP.eventsQueue.Enqueue(New Diacom.Ev.CommandStatus(_line.ID, m_Cmd.ID, Ev.CmdStatus.OK))

                        Case Cmd.CommandID.REJECT_CALL
                            Dim _rcCmd As Diacom.Cmd.RejectCall = CType(m_Cmd, Diacom.Cmd.RejectCall)
                            Diacom.TraceOut.Put("Line : " & _handle.ToString & " : Reject Command issued")
                            _apcControl.RejectCall(_handle, CType(_rcCmd.rejectReasonCode, AltiComLib.A_REJECT_CAUSE))
                            Me.m_AltiSP.eventsQueue.Enqueue(New Diacom.Ev.CommandStatus(_line.ID, m_Cmd.ID, Ev.CmdStatus.OK))

                        Case Cmd.CommandID.PASS_CALL
                            Diacom.TraceOut.Put("Line : " & _handle.ToString & " : Pass Command issued")
                            _apcControl.PassCall(_handle)
                            Me.m_AltiSP.eventsQueue.Enqueue(New Diacom.Ev.CommandStatus(_line.ID, m_Cmd.ID, Ev.CmdStatus.OK))

                        Case Cmd.CommandID.DROP_CALL
                            Diacom.TraceOut.Put("Line : " & _handle.ToString & " : Drop Call Command issued")
                            _apcControl.DropCall()

                        Case Cmd.CommandID.CONNECT_LINES
                            Dim _ccCmd As Diacom.Cmd.Connect = CType(m_Cmd, Diacom.Cmd.Connect)
                            Dim Line2 As Diacom.AltiSDK.AltiSDKLine = m_AltiSP.Item(_ccCmd.LineTwo)
                            If (_line.APCControlled = True AndAlso Line2.APCControlled = True) Then
                                Line2.LastCommand = Cmd.CommandID.CONNECT_LINES
                                Diacom.TraceOut.Put("Line : " & _handle.ToString & " : and Line : " & CInt(Line2.ID) & " : Connect Command issued")
                                _apcControl.ConnectTo(CInt(Line2.ID))
                            Else
                                Me.m_AltiSP.eventsQueue.Enqueue(New Diacom.Ev.CommandStatus(_line.ID, m_Cmd.ID, Ev.CmdStatus.ERROR))
                            End If

                        Case Cmd.CommandID.DIAL
                            Dim _mkCmd As Diacom.Cmd.Dial = CType(m_Cmd, Diacom.Cmd.Dial)
                            Dim _dialDest As String = _line.AccessCode + _mkCmd.Destination
                            Diacom.TraceOut.Put("Line : " & _handle.ToString & " : Dial Command issued")
                            If (_mkCmd.Tone = 0) Then
                                _apcControl.MakeCallEx(_dialDest, _mkCmd.Account, False, _mkCmd.Source)
                            Else
                                _apcControl.MakeCallEx(_dialDest, _mkCmd.Account, True, _mkCmd.Source)
                            End If
                            Me.m_AltiSP.eventsQueue.Enqueue(New Diacom.Ev.CommandStatus(_line.ID, m_Cmd.ID, Ev.CmdStatus.OK))

                        Case Cmd.CommandID.PLAY_DTMF
                            If Not _line.APCControlled Then Exit Select
                            Dim _pdCmd As Diacom.Cmd.PlayDTMF = CType(m_Cmd, Diacom.Cmd.PlayDTMF)
                            Diacom.TraceOut.Put("Line : " & _handle.ToString & " : Play DTMF Command issued")
                            _apcControl.PlayDTMFTone(_pdCmd.DTMFCode)

                        Case Cmd.CommandID.PLAY_FILE
                            If Not _line.APCControlled Then Exit Select
                            Dim _pfCmd As Diacom.Cmd.PlayFile = CType(m_Cmd, Diacom.Cmd.PlayFile)
                            Diacom.TraceOut.Put("Line : " & _handle.ToString & " : Play File Command issued")
                            _apcControl.PlayVoice(_pfCmd.CutOffString, _pfCmd.FilePath, 0)

                        Case Cmd.CommandID.RECORD_FILE
                            If Not _line.APCControlled Then Exit Select
                            Dim _rfCmd As Diacom.Cmd.RecordFile = CType(m_Cmd, Diacom.Cmd.RecordFile)
                            Diacom.TraceOut.Put("Line : " & _handle.ToString & " : Record File Command issued")
                            If _rfCmd.AppendMode = 0 Then
                                _apcControl.RecordVoice(_rfCmd.CutOffString, _rfCmd.FilePath, 0, A_APC_RECORD_MODE.A_APC_RECORD_CREATE)
                            Else
                                _apcControl.RecordVoice(_rfCmd.CutOffString, _rfCmd.FilePath, 0, A_APC_RECORD_MODE.A_APC_RECORD_APPEND)
                            End If

                        Case Cmd.CommandID.RESET
                            If _line.UseStopCommand Then
                                If _line.LastCommand = Cmd.CommandID.PLAY_FILE Then
                                    Diacom.TraceOut.Put("Line : " & _handle.ToString & " : StopPlayVoice Command issued")
                                    _apcControl.StopPlayVoice(A_APC_STOP_PLAY_TYPE.A_APC_STOP_PLAY_ALL)
                                ElseIf _line.LastCommand = Cmd.CommandID.RECORD_FILE Then
                                    Diacom.TraceOut.Put("Line : " & _handle.ToString & " : StopRecordVoice Command issued")
                                    _apcControl.StopRecordVoice(A_APC_STOP_RECORD_TYPE.A_APC_STOP_RECORD_ALL)
                                End If
                            End If

                        Case Cmd.CommandID.RING_EXTENSION
                            Dim _reCmd As Diacom.Cmd.RingExtension = CType(m_Cmd, Diacom.Cmd.RingExtension)
                            Dim _dialExt As String = _line.AccessCode + _reCmd.Extension
                            Diacom.TraceOut.Put("Line : " & _handle.ToString & " : Ring Extension Command issued")
                            If _reCmd.RingType = 2 Then ' Intercom Ring
                                _apcControl.RingExtension(A_APC_RING_TYPE.A_APC_RING_TYPE_INTERCOM, _dialExt)
                            Else
                                _apcControl.RingExtension(A_APC_RING_TYPE.A_APC_RING_TYPE_NORMAL, _dialExt)
                            End If
                            Me.m_AltiSP.eventsQueue.Enqueue(New Diacom.Ev.CommandStatus(_line.ID, m_Cmd.ID, Ev.CmdStatus.OK))

                        Case Cmd.CommandID.DISCONNECT_LINE
                            Diacom.TraceOut.Put("Line : " & _handle.ToString & " : SnatchLine Command issued")
                            Dim OtherLineID As Integer = _apcControl.SnatchLine(_handle)
                            Dim OtherLine As AltiSDKLine = m_AltiSP.Item(OtherLineID)

                            If OtherLine Is Nothing Then
                                Diacom.TraceOut.Put("Snatched only Line1=" & _handle.ToString)
                                Me.m_AltiSP.eventsQueue.Enqueue(New Diacom.Ev.CommandStatus(_line.ID, m_Cmd.ID, Ev.CmdStatus.OK))
                            Else
                                Diacom.TraceOut.Put("Snatched Line1=" & _handle.ToString & " and Line2=" & OtherLineID.ToString)
                                OtherLine.APCControlled = True
                                OtherLine.ConnectedLine = _line
                                Me.m_AltiSP.eventsQueue.Enqueue(New Diacom.Ev.LineLinked(_line.ID, OtherLineID))
                            End If

                        Case Cmd.CommandID.SWITCH_MUSIC
                            If Not _line.APCControlled Then Exit Select
                            Dim _smCmd As Diacom.Cmd.SwitchMusic = CType(m_Cmd, Diacom.Cmd.SwitchMusic)
                            Diacom.TraceOut.Put("Line : " & _handle.ToString & " : Switch music Command issued")
                            If _smCmd.MusicMode = 0 Then
                                _apcControl.SwitchMusic(A_APC_SWITCHMUSIC_MODE.A_APC_SWITCHMUSIC_ENTER)
                            Else
                                _apcControl.SwitchMusic(A_APC_SWITCHMUSIC_MODE.A_APC_SWITCHMUSIC_LEAVE)
                            End If

                        Case Cmd.CommandID.TRANSFER
                            Dim _tCmd As Diacom.Cmd.TransferCall = CType(m_Cmd, Diacom.Cmd.TransferCall)
                            Dim _targetLine As AltiSDKLine = m_AltiSP.Item(_tCmd.Target)

                            Dim _transferType As AltiComLib.A_APC_TRANSFER_TYPE
                            Dim _numberToTransfer As String = _targetLine.AccessCode & _tCmd.Destination
                            Dim _numericTarget As Integer = 0
                            Select Case _tCmd.Type
                                Case Cmd.TransferCallType.AUTOATEDDANT
                                    _transferType = A_APC_TRANSFER_TYPE.A_APC_TRANSFER_TO_AA
                                    _numericTarget = Convert.ToInt32(_tCmd.Destination)
                                    _numberToTransfer = Nothing
                                Case Cmd.TransferCallType.EXTENSION
                                    _transferType = A_APC_TRANSFER_TYPE.A_APC_TRANSFER_TO_EXT
                                    _numericTarget = Convert.ToInt32(_numberToTransfer)
                                    _numberToTransfer = Nothing
                                Case Cmd.TransferCallType.EXTENSION_VOICE_MESSAGE
                                    _transferType = A_APC_TRANSFER_TYPE.A_APC_TRANSFER_TO_EXTVM
                                    _numericTarget = Convert.ToInt32(_numberToTransfer)
                                    _numberToTransfer = Nothing
                                Case Cmd.TransferCallType.[OPERATOR]
                                    _transferType = A_APC_TRANSFER_TYPE.A_APC_TRANSFER_TO_OPERATOR
                                    _numericTarget = 0
                                    _numberToTransfer = Nothing
                                Case Cmd.TransferCallType.TRUNK
                                    _transferType = A_APC_TRANSFER_TYPE.A_APC_TRANSFER_TO_TRUNK
                                    _numericTarget = 0
                            End Select
                            Diacom.TraceOut.Put("Line : " & _handle.ToString & " : Transfer Command issued")
                            _apcControl.TransferTo(_transferType, _numericTarget, _numberToTransfer, 45)

                        Case Else
                            Diacom.TraceOut.Put("!!!Unknown Command !!!!")
                    End Select
                    _line.LastCommand = m_Cmd.ID

                Catch ex As Exception
                    Diacom.TraceOut.Put(ex)
                    If _line.APCControlled Then Me.m_AltiSP.ReportError(_line.ID, m_Cmd.ID)
                Finally
                    m_AltiSP.spCommands.Remove(Me)
                End Try
            End Sub
        End Class
    End Class

End Namespace