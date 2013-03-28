Option Explicit On 
Option Strict On


Imports System.Runtime.InteropServices
Imports System.Reflection
Imports System.Text.RegularExpressions


Namespace Diacom.APCStates

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Implements the <see cref="IAPCInterface"/> interface for work with a state machine.
    ''' </summary>
    ''' -----------------------------------------------------------------------------
    Public Class APCCommandModule : Implements IAPCInterface
        Private ReadOnly apcControl As APCStateControl
        Public FileTimeStamp As Integer
        Public DecodeKey1 As Integer

        Public Sub New(ByVal apcCtl As APCStateControl)
            Me.apcControl = apcCtl
        End Sub

        Sub APCDial(ByVal srcLine As APCLine, ByVal oLine As APCLine, ByVal dialString As String, ByVal callerNumber As String, ByVal commandTimeOut As Integer) Implements IAPCInterface.APCDial
            Try
                Dim _src As APCStateLine = CType(srcLine, APCStateLine)
                Dim _line As APCStateLine = CType(oLine, APCStateLine)

                Dim _continue As Boolean
                Dim _found As Boolean
                Dim _targetLine As APCStateLine
                _targetLine = apcControl.FindDialTargetLine(dialString, _found, _continue)
                If _continue Then
                    _line.FireStateEvent(_src, APCEvents.THENCONTINUE, Nothing, True)
                ElseIf _found Then
                    apcControl.DialFilter.AddModule(New MakeCallModule(_line, _targetLine, dialString, callerNumber, commandTimeOut))
                Else
                    _line.FireStateEvent(_src, APCEvents.ELSECONTINUE, Nothing, True)
                End If
            Catch _e As Exception
                DBGEX(_e)
            End Try
        End Sub

        Sub APCTransfer(ByVal srcLine As APCLine, ByVal oLine As APCLine, ByVal dialString As String, ByVal callerNumber As String, ByVal commandTimeOut As Integer) Implements IAPCInterface.APCTransfer
            Try
                Dim _src As APCStateLine = CType(srcLine, APCStateLine)
                Dim _line As APCStateLine = CType(oLine, APCStateLine)

                Dim _continue As Boolean
                Dim _found As Boolean
                Dim _targetLine As APCStateLine
                Dim _transferType As Diacom.Cmd.TransferCallType
                _targetLine = apcControl.FindTransferTargetLine(_line, dialString, _found, _continue, _transferType)
                If _found = True Then
                    _line.EventFilter.AddModule(New TransferCallModule(_src, _line, _targetLine, _transferType, dialString, callerNumber, commandTimeOut))
                ElseIf _continue = False Then
                    _line.FireStateEvent(_src, APCEvents.ELSECONTINUE, Nothing, True)
                Else
                    _line.FireStateEvent(_src, APCEvents.THENCONTINUE, Nothing, True)
                End If
            Catch _e As Exception
                DBGEX(_e)
            End Try
        End Sub

        Sub APCAnswer(ByVal srcLine As APCLine, ByVal oLine As APCLine, ByVal commandTimeOut As Integer) Implements IAPCInterface.APCAnswer
            Try
                Dim _src As APCStateLine = CType(srcLine, APCStateLine)
                Dim _line As APCStateLine = CType(oLine, APCStateLine)
                _line.EventFilter.AddModule(New AnswerModule(_src, _line, commandTimeOut))
            Catch _e As Exception
                DBGEX(_e)
            End Try
        End Sub

        Sub APCReject(ByVal srcLine As APCLine, ByVal oLine As APCLine, ByVal rejectReason As Integer, ByVal commandTimeOut As Integer) Implements IAPCInterface.APCReject
            Try
                Dim _src As APCStateLine = CType(srcLine, APCStateLine)
                Dim _line As APCStateLine = CType(oLine, APCStateLine)
                _line.EventFilter.AddModule(New RejectModule(_src, _line, rejectReason, commandTimeOut))
            Catch _e As Exception
                DBGEX(_e)
            End Try
        End Sub

        Sub APCRelease(ByVal srcLine As APCLine, ByVal oLine As APCLine, ByVal commandTimeOut As Integer) Implements IAPCInterface.APCRelease
            Try
                Dim _src As APCStateLine = CType(srcLine, APCStateLine)
                Dim _line As APCStateLine = CType(oLine, APCStateLine)
                _line.EventFilter.AddModule(New ReleaseModule(_src, _line, commandTimeOut))
            Catch _e As Exception
                DBGEX(_e)
            End Try
        End Sub

        Sub APCPass(ByVal srcLine As APCLine, ByVal oLine As APCLine, ByVal commandTimeOut As Integer) Implements IAPCInterface.APCPass
            Try
                Dim _src As APCStateLine = CType(srcLine, APCStateLine)
                Dim _line As APCStateLine = CType(oLine, APCStateLine)
                _line.EventFilter.AddModule(New PassModule(_src, _line, commandTimeOut))
            Catch _e As Exception
                DBGEX(_e)
            End Try
        End Sub

        Sub APCPlayDTMF(ByVal srcLine As APCLine, ByVal oLine As APCLine, ByVal dialString As String, ByVal commandTimeOut As Integer) Implements IAPCInterface.APCPlayDTMF
            Try
                Dim _src As APCStateLine = CType(srcLine, APCStateLine)
                Dim _line As APCStateLine = CType(oLine, APCStateLine)
                If (Not dialString.Equals(String.Empty)) Then
                    _line.EventFilter.AddModule(New PlayDTMFModule(_src, _line, dialString, commandTimeOut))
                End If
            Catch _e As Exception
                DBGEX(_e)
            End Try
        End Sub
        Sub APCPlayFile(ByVal srcLine As APCLine, ByVal oLine As APCLine, ByVal FileString As String, ByVal CutoutString As String, ByVal commandTimeOut As Integer) Implements IAPCInterface.APCPlayFile
            Try
                Dim _src As APCStateLine = CType(srcLine, APCStateLine)
                Dim _line As APCStateLine = CType(oLine, APCStateLine)
                ' Split the FileString into substrings using "," as separator
                Dim _phraseNum As Integer
                Dim FileNames() As String = FileString.Split(","c)
                For Each _fileName As String In FileNames
                    _fileName = _fileName.Trim.ToUpper
                    ' If the string can be converted to a number - use it
                    If Int32.TryParse(_fileName, _phraseNum) Then
                        _phraseNum += (Me.DecodeKey1 Xor &H20)
                        _fileName = apcControl.Convert(_line.CurrentLanguageSet, "phrase", _phraseNum)
                    End If
                    ' After conversion to the number there can be multiple other strings - split them
                    Dim Phrases() As String = _fileName.Split(","c)
                    For Each sPhrase As String In Phrases
                        sPhrase = sPhrase.Trim
                        If (sPhrase <> String.Empty) Then
                            _line.EventFilter.AddModule(New PlayFileModule(_src, _line, sPhrase, CutoutString.Trim.ToUpper, commandTimeOut))
                        End If
                    Next
                Next
            Catch _e As Exception
                DBGEX(_e)
            End Try
        End Sub

        Sub APCRecordFile(ByVal srcLine As APCLine, ByVal oLine As APCLine, ByVal FileString As String, ByVal CutoutString As String, ByVal commandTimeOut As Integer) Implements IAPCInterface.APCRecordFile
            Try
                Dim _src As APCStateLine = CType(srcLine, APCStateLine)
                Dim _line As APCStateLine = CType(oLine, APCStateLine)
                Dim _phraseNum As Integer
                Dim _fileName As String
                If (Not FileString.Equals(String.Empty)) Then
                    _fileName = FileString
                    If Int32.TryParse(_fileName, _phraseNum) Then
                        _fileName = apcControl.Convert(_line.CurrentLanguageSet, "phrase", _phraseNum)
                    End If
                    _line.EventFilter.AddModule(New RecordFileModule(_src, _line, _fileName, CutoutString.Trim.ToUpper, commandTimeOut))
                End If
            Catch _e As Exception
                DBGEX(_e)
            End Try
        End Sub
        Sub APCPlayNumber(ByVal srcLine As APCLine, ByVal oLine As APCLine, ByVal Number As Object, ByVal PlayFormat As String, ByVal CutoutString As String, ByVal commandTimeOut As Integer) Implements IAPCInterface.APCPlayNumber
            Try
                Dim _src As APCStateLine = CType(srcLine, APCStateLine)
                Dim _line As APCStateLine = CType(oLine, APCStateLine)
                Dim _fileName As String
                _fileName = apcControl.Convert(_line.CurrentLanguageSet, PlayFormat, Number)
                ' After conversion to the number there can be multiple other strings - split them
                Dim Phrases() As String = _fileName.Split(","c)
                For Each sPhrase As String In Phrases
                    sPhrase = sPhrase.Trim
                    If (sPhrase <> String.Empty) Then
                        _line.EventFilter.AddModule(New PlayFileModule(_src, _line, sPhrase, CutoutString.Trim.ToUpper, commandTimeOut))
                    End If
                Next
            Catch _e As Exception
                DBGEX(_e)
            End Try
        End Sub
        Sub APCCollectNumber(ByVal oLine As APCLine, ByVal VarName As String, ByVal MaxDigits As Integer, ByVal CutoutString As String, ByVal InitialTimeout As Integer, ByVal InterDigitTimeout As Integer, ByVal commandTimeOut As Integer) Implements IAPCInterface.APCCollectNumber
            Try
                Dim _line As APCStateLine = CType(oLine, APCStateLine)
                _line.EventFilter.AddModule(New CollectNumberModule(_line, _line, VarName, MaxDigits, CutoutString, InitialTimeout, InterDigitTimeout, commandTimeOut))
            Catch _e As Exception
                DBGEX(_e)
            End Try
        End Sub
        Sub APCReset(ByVal oLine As APCLine) Implements IAPCInterface.APCReset
            Try
                Dim _line As APCStateLine = CType(oLine, APCStateLine)
                _line.EventFilter.Reset()
            Catch _e As Exception
                DBGEX(_e)
            End Try
        End Sub
        Sub APCResetAll(ByVal oLine As APCLine) Implements IAPCInterface.APCResetAll
            Try
                Dim _line As APCStateLine = CType(oLine, APCStateLine)
                _line.EventFilter.Reset(True)
                _line.SQLFilter.Reset(True)
                apcControl.AsyncFilter.Reset(_line, True)
                apcControl.DialFilter.Reset(_line)
            Catch _e As Exception
                DBGEX(_e)
            End Try
        End Sub

        Sub APCConnectToServer(ByVal serverName As String, ByVal param1 As String, ByVal param2 As String, ByVal param3 As String) Implements IAPCInterface.APCConnectToServer
            apcControl.Connect(serverName, param1, param2, param3)
        End Sub
        Sub APCWaitDigit(ByVal oLine As APCLine, ByVal commandTimeOut As Integer) Implements IAPCInterface.APCWaitDigit
            Try
                Dim _line As APCStateLine = CType(oLine, APCStateLine)
                _line.EventFilter.AddModule(New WaitDigitModule(_line, _line, commandTimeOut))
            Catch _e As Exception
                DBGEX(_e)
            End Try
        End Sub
        Sub APCWaitComplete(ByVal oLine As APCLine, ByVal commandTimeOut As Integer) Implements IAPCInterface.APCWaitComplete
            Try
                Dim _line As APCStateLine = CType(oLine, APCStateLine)
                _line.EventFilter.AddModule(New WaitCompleteModule(_line, _line, commandTimeOut))
            Catch _e As Exception
                DBGEX(_e)
            End Try
        End Sub
        Sub APCWaitTimer(ByVal oLine As APCLine, ByVal commandTimeOut As Integer) Implements IAPCInterface.APCWaitTimer
            Try
                Dim _line As APCStateLine = CType(oLine, APCStateLine)
                _line.EventFilter.AddModule(New WaitTimerModule(_line, _line, commandTimeOut))
            Catch _e As Exception
                DBGEX(_e)
            End Try
        End Sub
        Sub APCConnect(ByVal srcLine As APCLine, ByVal oLine As APCLine, ByVal oLinkLine As APCLine, ByVal commandTimeOut As Integer) Implements IAPCInterface.APCConnect
            Try
                Dim _src As APCStateLine = CType(srcLine, APCStateLine)
                Dim _line1 As APCStateLine = CType(oLine, APCStateLine)
                Dim _line2 As APCStateLine = CType(oLinkLine, APCStateLine)
                _line1.EventFilter.AddModule(New ConnectModule(_src, _line1, _line2, commandTimeOut))
            Catch _e As Exception
                DBGEX(_e)
            End Try
        End Sub
        Sub APCDisconnect(ByVal srcLine As APCLine, ByVal oLine As APCLine, ByVal commandTimeOut As Integer) Implements IAPCInterface.APCDisconnect
            Try
                Dim _src As APCStateLine = CType(srcLine, APCStateLine)
                Dim _line1 As APCStateLine = CType(oLine, APCStateLine)
                _line1.EventFilter.AddModule(New DisconnectModule(_src, _line1, commandTimeOut))
            Catch _e As Exception
                DBGEX(_e)
            End Try
        End Sub
        '' SQL functions
        Sub APCSQLNonQuery(ByVal oLine As APCLine, ByVal VarName As String, ByVal SQLServerString As String, ByVal SQLCommand As String, ByVal commandTimeOut As Integer) Implements IAPCInterface.APCSQLNonQuery
            Try
                Dim _line As APCStateLine = CType(oLine, APCStateLine)
                _line.SQLFilter.AddModule(New SQLNonQueryModule(_line, VarName, SQLServerString, SQLCommand, commandTimeOut))
            Catch _e As Exception
                DBGEX(_e)
            End Try
        End Sub
        Sub APCSQLGetRecord(ByVal oLine As APCLine, ByVal VarName As String, ByVal SQLServerString As String, ByVal SQLCommand As String, ByVal commandTimeOut As Integer) Implements IAPCInterface.APCSQLGetRecord
            Try
                Dim _line As APCStateLine = CType(oLine, APCStateLine)
                _line.SQLFilter.AddModule(New SQLGetRecordModule(_line, VarName, SQLServerString, SQLCommand, commandTimeOut))
            Catch _e As Exception
                DBGEX(_e)
            End Try
        End Sub
        Sub APCSQLGetValue(ByVal oLine As APCLine, ByVal VarName As String, ByVal SQLServerString As String, ByVal SQLCommand As String, ByVal commandTimeOut As Integer) Implements IAPCInterface.APCSQLGetValue
            Try
                Dim _line As APCStateLine = CType(oLine, APCStateLine)
                _line.SQLFilter.AddModule(New SQLGetValueModule(_line, VarName, SQLServerString, SQLCommand, commandTimeOut))
            Catch _e As Exception
                DBGEX(_e)
            End Try
        End Sub
        Sub APCSQLExecProcedure(ByVal oLine As APCLine, ByVal VarName As String, ByVal SQLServerString As String, ByVal SQLCommand As String, ByVal commandTimeOut As Integer) Implements IAPCInterface.APCSQLExecProcedure
            Try
                Dim _line As APCStateLine = CType(oLine, APCStateLine)
                _line.SQLFilter.AddModule(New SQLExecProcedureModule(_line, VarName, SQLServerString, SQLCommand, commandTimeOut))
            Catch _e As Exception
                DBGEX(_e)
            End Try
        End Sub
        Sub APCSQLReset(ByVal oLine As APCLine) Implements IAPCInterface.APCSQLReset
            Try
                Dim _line As APCStateLine = CType(oLine, APCStateLine)
                _line.SQLFilter.Reset()
            Catch _e As Exception
                DBGEX(_e)
            End Try
        End Sub
        '' Async functions
        Sub APCAsyncExecute(ByVal oLine As APCLine, ByVal VarName As String, ByVal AsyncCommand As String, ByVal stateParam As Object()) Implements IAPCInterface.APCAsyncExecute
            Try
                Dim _line As APCStateLine = CType(oLine, APCStateLine)
                Dim FullProcName As String = oLine.FullStateName(AsyncCommand.Trim.ToUpper)
                Dim _callInfo As ScriptCallInfo = CType(apcControl.APCFunctions(FullProcName), ScriptCallInfo)
                If Not (_callInfo Is Nothing) Then
                    DBG("Function found : " & FullProcName)
                    apcControl.AsyncFilter.AddModule(_line, New AsyncGetValueModule(VarName, _callInfo.CallInstance, _callInfo.CallMethod, stateParam))
                Else
                    DBG("Function not found : " & FullProcName)
                End If
            Catch _e As Exception
                DBGEX(_e)
            End Try
        End Sub
        Sub APCAsyncReset(ByVal oLine As APCLine) Implements IAPCInterface.APCAsyncReset
            Try
                Dim _line As APCStateLine = CType(oLine, APCStateLine)
                apcControl.AsyncFilter.Reset(_line)
            Catch _e As Exception
                DBGEX(_e)
            End Try
        End Sub
        '' MakeCall reset
        Sub APCDialReset(ByVal oLine As APCLine) Implements IAPCInterface.APCDialReset
            Try
                Dim _line As APCStateLine = CType(oLine, APCStateLine)
                apcControl.DialFilter.Reset(_line)
            Catch _e As Exception
                DBGEX(_e)
            End Try
        End Sub
        '' Data conversion functions
        Function Convert(ByVal LanguageName As String, ByVal ConversionName As String, ByVal DataNumber As Object) As String Implements IAPCInterface.Convert
            Try
                Return apcControl.Convert(LanguageName, ConversionName, DataNumber)
            Catch _e As Exception
                DBGEX(_e)
                Return Nothing
            End Try
        End Function

        '' User-specific Data
        Property APCUserData(ByVal oLine As APCLine, ByVal key As Object) As Object Implements IAPCInterface.APCUserData
            Get
                Try
                    Return apcControl.UserData(oLine.LineID, key)
                Catch _e As Exception
                    DBGEX(_e)
                    Return Nothing
                End Try
            End Get
            Set(ByVal Value As Object)
                apcControl.UserData(oLine.LineID, key) = Value
            End Set
        End Property

        Sub APCOnEvent(ByVal oLine As APCLine, ByVal eventCodes As String, ByVal eventFunction As String, ByVal eventHandler As Boolean, ByVal commandTimeOut As Integer) Implements IAPCInterface.APCOnEvent
            Try
                Dim _line As APCStateLine = CType(oLine, APCStateLine)
                Dim _ev As New Collections.ArrayList
                Dim _digits As New System.Text.StringBuilder
                Dim _callInfo As ScriptCallInfo

                For Each _char As Char In CStr(eventCodes).ToUpper
                    Select Case _char

                        Case "Z"c
                            _ev.Add(APCEvents.THENCONTINUE)

                        Case "S"c
                            _ev.Add(APCEvents.ELSECONTINUE)

                        Case "I"c
                            _ev.Add(APCEvents.CONNECT)

                        Case "X"c
                            _ev.Add(APCEvents.DISCONNECT)

                        Case "Y"c
                            _ev.Add(APCEvents.DISCONNECTLINK)

                        Case "Q"c
                            _ev.Add(APCEvents.QUERYREADY)

                        Case "W"c
                            _ev.Add(APCEvents.WAITERROR)

                        Case "T"c
                            _ev.Add(APCEvents.TIMEOUT)

                        Case "J"c
                            _ev.Add(APCEvents.RING)

                        Case "V"c
                            _ev.Add(APCEvents.ALERTED)

                        Case "L"c
                            _ev.Add(APCEvents.LINKREQUEST)

                        Case "G"c
                            _ev.Add(APCEvents.GRANTREQUEST)

                        Case "U"c
                            _ev.Add(APCEvents.LINKRELEASE)

                        Case "P"c
                            If (_digits.Length = 0) Then _ev.Add(APCEvents.DIGIT)
                            _digits.Append(_char)
                            _digits.Append("#")

                        Case "R"c
                            If (_digits.Length = 0) Then _ev.Add(APCEvents.DIGIT)
                            _digits.Append(_char)
                            _digits.Append("*")

                        Case "0"c To "9"c, "#"c, "*"c
                            If (_digits.Length = 0) Then _ev.Add(APCEvents.DIGIT)
                            _digits.Append(_char)
                            If (_char = "#"c) Then _digits.Append("P")
                            If (_char = "*"c) Then _digits.Append("R")

                    End Select
                Next
                _callInfo = CType(Me.apcControl.APCStates(_line.FullStateName(CStr(eventFunction).Trim.ToUpper)), _
                                    ScriptCallInfo)
                If Not (_callInfo Is Nothing) Then
                    _line.EventFilter.AddEventHandler(New OnEventModule(_line, _line, _callInfo, _ev, eventCodes, _digits.ToString, eventHandler, commandTimeOut))
                Else
                    DBG("Event function is not found :" & eventFunction)
                End If
            Catch _e As Exception
                DBGEX(_e)
            End Try
        End Sub
    End Class

    ''' <summary>
    ''' Defines the class that communicates with Service Provider (SP) module.
    ''' </summary>
    Friend Class SPCommandModule : Implements IDisposable
        Private ReadOnly iSP As Diacom.ISP
        Private ReadOnly APCCmd As APCStateControl
        Private ReadOnly readThread As System.Threading.Thread
        Private TerminateThread As Boolean = False
        Private disposed As Boolean = False
        Const Const1 As Integer = 8272

        ''' <summary>
        ''' Initializes a new instance of <see cref="SPCommandModule"/> class .
        ''' </summary>
        ''' <param name="interfaceSP">Interface instance to communicate with SP.</param>
        ''' <param name="apcModule">The APC module that will be used.</param>
        Public Sub New(ByVal interfaceSP As Diacom.ISP, ByVal apcModule As APCStateControl)
            iSP = interfaceSP
            APCCmd = apcModule
            readThread = New Threading.Thread(AddressOf Me.ProcessReadQueue)
            readThread.IsBackground = True
            readThread.Name = "APCSM Thread"
            readThread.Start()
        End Sub

        Sub Dispose() Implements IDisposable.Dispose
            If (Not disposed) Then
                disposed = True
                GC.SuppressFinalize(Me)
                TerminateThread = True
                iSP.Dispose()
                APCCmd.Dispose()
            End If
        End Sub

        Public Sub SendDialInternalCommand(ByVal srcLine As Object, ByVal targetLine As Object, ByVal extensionNumber As String)
            iSP.Send(New Diacom.Cmd.RingExtension(srcLine, targetLine, extensionNumber, 1))
        End Sub

        Public Sub SendDialExternalCommand(ByVal srcLine As Object, ByVal targetLine As Object, ByVal dialNumber As String, ByVal callerID As String, ByVal accountCode As String)
            iSP.Send(New Diacom.Cmd.Dial(srcLine, targetLine, dialNumber, callerID, accountCode, 1))
        End Sub

        Public Sub SendPlayDTMFCommand(ByVal srcLine As Object, ByVal targetLine As Object, ByVal dtmfString As String)
            iSP.Send(New Diacom.Cmd.PlayDTMF(srcLine, targetLine, dtmfString))
        End Sub

        Public Sub SendReleaseCommand(ByVal srcLine As Object, ByVal targetLine As Object)
            iSP.Send(New Diacom.Cmd.DropCall(srcLine, targetLine))
        End Sub

        Public Sub SendAnswerCommand(ByVal srcLine As Object, ByVal targetLine As Object)
            iSP.Send(New Diacom.Cmd.AnswerCall(srcLine, targetLine))
        End Sub

        Public Sub SendRejectCommand(ByVal srcLine As Object, ByVal targetLine As Object, ByVal rejectReason As Integer)
            iSP.Send(New Diacom.Cmd.RejectCall(srcLine, targetLine, rejectReason))
        End Sub

        Public Sub SendPassCommand(ByVal srcLine As Object, ByVal targetLine As Object)
            iSP.Send(New Diacom.Cmd.PassCall(srcLine, targetLine))
        End Sub

        Public Sub SendPlayFileCommand(ByVal srcLine As Object, ByVal targetLine As Object, ByVal fileName As String, ByVal cutoffString As String)
            iSP.Send(New Diacom.Cmd.PlayFile(srcLine, targetLine, fileName, cutoffString))
        End Sub

        Public Sub SendRecordFileCommand(ByVal srcLine As Object, ByVal targetLine As Object, ByVal fileName As String, ByVal cutoffString As String)
            iSP.Send(New Diacom.Cmd.RecordFile(srcLine, targetLine, fileName, cutoffString, 0))
        End Sub

        Public Sub SendConnectCommand(ByVal srcLine As Object, ByVal targetLine As Object, ByVal connectingLine As Object)
            iSP.Send(New Diacom.Cmd.Connect(srcLine, targetLine, connectingLine))
        End Sub

        Public Sub SendDisconnectCommand(ByVal srcLine As Object, ByVal targetLine As Object)
            iSP.Send(New Diacom.Cmd.Disconnect(srcLine, targetLine))
        End Sub

        Public Sub SendResetCommand(ByVal srcLine As Object, ByVal targetLine As Object)
            iSP.Send(New Diacom.Cmd.Reset(srcLine, targetLine))
        End Sub

        Public Sub SendTransferToVMCommand(ByVal srcLine As Object, ByVal targetLine As Object, ByVal destLine As Object, ByVal extensionNumber As String)
            iSP.Send(New Diacom.Cmd.TransferCall(srcLine, targetLine, destLine, extensionNumber, Diacom.Cmd.TransferCallType.EXTENSION_VOICE_MESSAGE))
        End Sub

        Public Sub SendTransferToOpCommand(ByVal srcLine As Object, ByVal targetLine As Object, ByVal destLine As Object, ByVal operatorNumber As String)
            iSP.Send(New Diacom.Cmd.TransferCall(srcLine, targetLine, destLine, operatorNumber, Diacom.Cmd.TransferCallType.[OPERATOR]))
        End Sub

        Public Sub SendTransferToInternalCommand(ByVal srcLine As Object, ByVal targetLine As Object, ByVal destLine As Object, ByVal extensionNumber As String)
            iSP.Send(New Diacom.Cmd.TransferCall(srcLine, targetLine, destLine, extensionNumber, Diacom.Cmd.TransferCallType.EXTENSION))
        End Sub

        Public Sub SendTransferToExternalCommand(ByVal srcLine As Object, ByVal targetLine As Object, ByVal destLine As Object, ByVal outdialNumber As String)
            iSP.Send(New Diacom.Cmd.TransferCall(srcLine, targetLine, destLine, outdialNumber, Diacom.Cmd.TransferCallType.TRUNK))
        End Sub

        Public Sub SendTransferToAACommand(ByVal srcLine As Object, ByVal targetLine As Object, ByVal destLine As Object, ByVal aaNumber As String)
            iSP.Send(New Diacom.Cmd.TransferCall(srcLine, targetLine, destLine, aaNumber, Diacom.Cmd.TransferCallType.AUTOATEDDANT))
        End Sub

        ''' <summary>
        ''' Processes all incoming responses and line events from SP queue.
        ''' </summary>
        Private Sub ProcessReadQueue()
            Dim spEvent As Object
            Dim _line As APCStateLine
            Dim _eventType As Ev.EventID

            While (Not TerminateThread)

                Try
                    ' Receive the message from the queue
                    spEvent = iSP.Receive()
                    If TerminateThread = True Then Exit While

                    ' Process the message
                    _eventType = CType(spEvent, Ev.EvBase).ID
                    _line = Me.APCCmd(CType(spEvent, Ev.EvBase).Line)
                    If Not _line Is Nothing Then
                        Select Case _eventType
                            Case Ev.EventID.COMMAND_STATUS
                                Dim _ev As Ev.CommandStatus = CType(spEvent, Ev.CommandStatus)
                                DBGLN(_line, "Event from SP:" & [Enum].GetName(GetType(Ev.EventID), _eventType) & " : Status:" & _ev.Status.ToString())
                                If (_ev.Status = Ev.CmdStatus.OK) Then
                                    _line.FireStateEvent(_line, APCEvents.COMMANDOK, Nothing, True)
                                Else
                                    _line.FireStateEvent(_line, APCEvents.COMMANDFAIL, Nothing, True)
                                End If

                            Case Ev.EventID.CONNECT
                                DBGLN(_line, "Event from SP:" & [Enum].GetName(GetType(Ev.EventID), _eventType))
                                _line.LastEvent = "I"
                                _line.LastEventParameter = ""
                                If (APCStateControl.ConfigData Xor Const1) = &H2012 Then
                                    _line.FireStateEvent(_line.EventLine, APCEvents.CONNECT, Nothing, True)
                                End If

                            Case Ev.EventID.DIGIT
                                Dim _ed As Ev.Digit = CType(spEvent, Ev.Digit)
                                DBGLN(_line, "Event from SP:" & [Enum].GetName(GetType(Ev.EventID), _eventType) & " : Digit:" & _ed.Code.ToString())
                                _line.LastEventParameter = _ed.Code.ToString()
                                Select Case _line.LastEventParameter
                                    Case "*"
                                        _line.LastEvent = "R"
                                    Case "#"
                                        _line.LastEvent = "P"
                                    Case Else
                                        _line.LastEvent = _line.LastEventParameter
                                End Select
                                _line.LastDigit = _line.LastEventParameter
                                _line.FireStateEvent(_line.EventLine, APCEvents.DIGIT, _line.LastEventParameter, True)

                            Case Ev.EventID.DISCONNECT
                                DBGLN(_line, "Event from SP:" & [Enum].GetName(GetType(Ev.EventID), _eventType))
                                ' If the line on which event came is a slave line - send to the master special 
                                '  disconnect event - APCEvents.DISCONNECTLINK.
                                ' If after that the slave line become non-slave - send to it regular APCEvents.DISCONNECT
                                _line.LastEventParameter = ""
                                If (Not (_line.EventLine Is _line)) Then
                                    _line.LastEvent = "Y"
                                    _line.FireStateEvent(_line.EventLine, APCEvents.DISCONNECTLINK, Nothing, True)
                                Else
                                    _line.LastEvent = "X"
                                    _line.FireStateEvent(_line.EventLine, APCEvents.DISCONNECT, Nothing, True)
                                End If

                            Case Ev.EventID.LINE_STATE_CHANGED
                                Dim _ee As Ev.LineStateChanged = CType(spEvent, Ev.LineStateChanged)
                                DBGLN(_line, "Event from SP:" & [Enum].GetName(GetType(Ev.EventID), _eventType) & " : State:" & _ee.State.ToString())
                                If (_ee.State = SPLineState.LINE_REMOVE) Then
                                    Me.APCCmd.RemoveLine(_ee.Line)
                                ElseIf (_ee.State = SPLineState.LINE_ADD) Then
                                    Dim _addedLine As SPLine = Me.iSP.GetLine(_ee.Line)
                                    Me.APCCmd.RemoveLine(_ee.Line)
                                    Me.APCCmd.AddLine(_addedLine)
                                Else
                                    _line.LineStatus = CType(CInt(_ee.State), APCLineState)
                                End If

                            Case Ev.EventID.RING
                                Dim _ej As Ev.Ring = CType(spEvent, Ev.Ring)
                                Dim _info As SPLine = _ej.ringInfo
                                With _line
                                    .CallName = _info.CalledName
                                    .CallNumber = _info.CalledNumber
                                    .CIDName = _info.CIDName
                                    .CIDNumber = _info.CIDNumber
                                    .DIDName = _info.DIDName
                                    .DIDNumber = _info.DIDNumber
                                    .DNISName = _info.DNISName
                                    .DNISNumber = _info.DNISNumber
                                End With
                                DBGLN(_line, "Event from SP:" & [Enum].GetName(GetType(Ev.EventID), _eventType))
                                _line.LastEvent = "J"
                                _line.LastEventParameter = ""
                                _line.FireStateEvent(_line.EventLine, APCEvents.RING, Nothing, True)

                            Case Ev.EventID.RING_BACK
                                Dim _eb As Ev.RingBack = CType(spEvent, Ev.RingBack)
                                Dim _info As SPLine = _eb.ringInfo
                                With _line
                                    .CallName = _info.CalledName
                                    .CallNumber = _info.CalledNumber
                                    .CIDName = _info.CIDName
                                    .CIDNumber = _info.CIDNumber
                                    .DIDName = _info.DIDName
                                    .DIDNumber = _info.DIDNumber
                                    .DNISName = _info.DNISName
                                    .DNISNumber = _info.DNISNumber
                                End With
                                DBGLN(_line, "Event from SP:" & [Enum].GetName(GetType(Ev.EventID), _eventType))
                                _line.LastEvent = "V"
                                _line.LastEventParameter = ""
                                _line.FireStateEvent(_line.EventLine, APCEvents.RINGBACK, Nothing, False)

                            Case Ev.EventID.TONE
                                Dim _et As Ev.Tone = CType(spEvent, Ev.Tone)
                                DBGLN(_line, "Event from SP:" & [Enum].GetName(GetType(Ev.EventID), _eventType) & " : Tone:" & _et.Type.ToString())
                                _line.LastEvent = "V"
                                _line.LastEventParameter = _et.Type.ToString
                                _line.FireStateEvent(_line.EventLine, APCEvents.ALERTED, _et.Type, True)

                            Case Ev.EventID.LINK_LINE
                                Dim _ll As Ev.LineLinked = CType(spEvent, Ev.LineLinked)
                                Dim _slaveLine As APCStateLine = Me.APCCmd(_ll.SlaveLine)
                                If _ll.Line.Equals(_ll.SlaveLine) Then
                                    _line.LinkLine = Nothing
                                Else
                                    _line.LinkLine = _slaveLine
                                End If
                                _slaveLine.EventLine = _line

                        End Select
                    Else
                        If (_eventType = Ev.EventID.LINE_STATE_CHANGED) Then
                            Dim _ee As Ev.LineStateChanged = CType(spEvent, Ev.LineStateChanged)
                            Dim _addedLine As SPLine = Me.iSP.GetLine(_ee.Line)
                            If (_ee.State = SPLineState.LINE_ADD) Then
                                Me.APCCmd.AddLine(_addedLine)
                                DBG("New Line Added: " & _addedLine.ToString())
                            End If
                        End If
                    End If
                Catch ex As Exception
                    DBGEX(ex)
                End Try
            End While
            DBG("ProcessReadQueue: Exiting and terminating a thread ....")
        End Sub
    End Class

End Namespace