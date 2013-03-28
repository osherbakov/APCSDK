Option Explicit On 
Option Strict On

Namespace Diacom

    Public Delegate Sub StateFunction()
    Public Delegate Function AsyncFunction(ByVal Line As Diacom.APCLine) As Object

    ''' <summary>
    ''' Represents the control from which all other controlling classes are derived.
    ''' </summary>
    Public Class APCLineControl
        Public Sub New()
        End Sub

        Public Sub New(ByVal ctlIf As IAPCInterface)
            APCIf = ctlIf
        End Sub

        Private Shared APCIf As IAPCInterface

        ''' <summary>
        ''' The line from which the event came (Originating Line).
        ''' </summary>
        Private Shared oSourceLine As APCLine

        ''' <summary>
        ''' The line on which state changes are made.
        ''' </summary>
        Private Shared oStateLine As APCLine

        ''' <summary>
        ''' Gets the sync object of the main module.
        ''' </summary>
        ''' <returns>The sync object of the Main module.</returns>
        Public Function GetSyncRoot() As Object
            Return Me
        End Function

        ' Test functions for testing Methods and static functions and subs
        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Test function for testing methods, static functions and subs. Puts its parameter to a 
        ''' standard error stram.
        ''' </summary>
        ''' <param name="testParam">String to put to the standard error stream of the application.</param>
        ''' <returns><c>Nothing</c>.</returns>
        Public Shared Function SharedFunctionTest(ByVal testParam As String) As Object
            TraceOut.Put(testParam)
            Return Nothing
        End Function

        ''' <summary>
        ''' Test function for testing methods, static functions and subs. Puts its parameter to a 
        ''' standard error stram.
        ''' </summary>
        ''' <param name="testParam">String to put to the standard error stream of the application.</param>
        Public Shared Sub SharedSubTest(ByVal testParam As String)
            TraceOut.Put(testParam)
        End Sub

        ''' <summary>
        ''' Test function for testing methods, static functions and subs. Puts its parameter to a 
        ''' standard error stram.
        ''' </summary>
        ''' <param name="testParam">String to put to the standard error stream of the application.</param>
        Public Function FunctionTest(ByVal testParam As String) As Object
            TraceOut.Put(testParam)
            Return Nothing
        End Function

        ''' <summary>
        ''' Test function for testing methods, static functions and subs. Puts its parameter to a 
        ''' standard error stram.
        ''' </summary>
        ''' <param name="testParam">String to put to the standard error stream of the application.</param>
        Public Sub SubTest(ByVal testParam As String)
            TraceOut.Put(testParam)
        End Sub

        ''' <summary>
        ''' Connects to server with given parameters.
        ''' </summary>
        ''' <param name="ServerName">Name of the server.</param>
        ''' <param name="Param1">First parameter.</param>
        ''' <param name="Param2">Second parameter.</param>
        ''' <param name="Param3">Third parameter.</param>
        ''' <remarks><seealso cref="IAPCInterface.APCConnectToServer"/></remarks>
        Public Sub ServerConnect(ByVal ServerName As String, ByVal Param1 As String, ByVal Param2 As String, ByVal Param3 As String)
            APCIf.APCConnectToServer(ServerName, Param1, Param2, Param3)
        End Sub

        ''' <summary>
        ''' Connects to server with given parameters.
        ''' </summary>
        ''' <param name="ServerName">Name of the server.</param>
        ''' <param name="Param1">First parameter.</param>
        ''' <param name="Param2">Second parameter.</param>
        ''' <param name="Param3">Third parameter.</param>
        ''' <remarks><seealso cref="IAPCInterface.APCConnectToServer"/></remarks>
        Public Sub ConnectToServer(ByVal ServerName As String, ByVal Param1 As String, ByVal Param2 As String, ByVal Param3 As String)
            APCIf.APCConnectToServer(ServerName, Param1, Param2, Param3)
        End Sub

        ''' <summary>
        ''' Gets new empty data record.
        ''' </summary>
        ''' <returns>New empty data record.</returns>
        ''' -----------------------------------------------------------------------------
        Public Function EmptyRecord() As DataRecord
            Return New DataRecord
        End Function

        'Voice/Tone/DTMF Methods of the class LineState
        ''' <summary>
        ''' Plays given DTMF code for the line.
        ''' </summary>
        ''' <param name="DTMFString">DTMF code to play.</param>
        Public Sub PlayDTMF(ByVal DTMFString As String)
            APCIf.APCPlayDTMF(oStateLine, oStateLine, DTMFString, 0)
        End Sub

        ''' <summary>
        ''' Plays DTMF code with given parameters for the line.
        ''' </summary>
        ''' <param name="DTMFString">DTMF code to play.</param>
        ''' <param name="TimeoutVal">Timeout for action.</param>
        Public Sub PlayDTMF(ByVal DTMFString As String, ByVal TimeoutVal As Integer)
            APCIf.APCPlayDTMF(oStateLine, oStateLine, DTMFString, TimeoutVal)
        End Sub

        ''' <summary>
        ''' Plays given file for the line.
        ''' </summary>
        ''' <param name="FileString">File to play path.</param>
        ''' -----------------------------------------------------------------------------
        Public Sub PlayFile(ByVal FileString As String)
            APCIf.APCPlayFile(oStateLine, oStateLine, FileString, "ALL", 0)
        End Sub

        ''' <summary>
        ''' Plays given file for the line with given cutoff digits.
        ''' </summary>
        ''' <param name="FileString">File to play path.</param>
        ''' <param name="CutoutString">Cutoff digits.</param>
        Public Sub PlayFile(ByVal FileString As String, ByVal CutoutString As String)
            APCIf.APCPlayFile(oStateLine, oStateLine, FileString, CutoutString, 0)
        End Sub

        ''' <summary>
        ''' Plays given file for the line with given cutoff digits and action timeout.
        ''' </summary>
        ''' <param name="FileString">File to play path.</param>
        ''' <param name="CutoutString">Cutoff digits.</param>
        ''' <param name="TimeoutVal">Timeout for action.</param>
        Public Sub PlayFile(ByVal FileString As String, ByVal CutoutString As String, ByVal TimeoutVal As Integer)
            APCIf.APCPlayFile(oStateLine, oStateLine, FileString, CutoutString, TimeoutVal)
        End Sub

        ''' <summary>
        ''' Records a file for the line.
        ''' </summary>
        ''' <param name="FileString">Path to store the file to (with file name).</param>
        Public Sub RecordFile(ByVal FileString As String)
            APCIf.APCRecordFile(oStateLine, oStateLine, FileString, "ALL", 0)
        End Sub

        ''' <summary>
        ''' Records a file for the line.
        ''' </summary>
        ''' <param name="FileString">Path to store the file to (with file name).</param>
        ''' <param name="CutoutString">Cutoff digits.</param>
        Public Sub RecordFile(ByVal FileString As String, ByVal CutoutString As String)
            APCIf.APCRecordFile(oStateLine, oStateLine, FileString, CutoutString, 0)
        End Sub

        ''' <summary>
        ''' Records a file for the line.
        ''' </summary>
        ''' <param name="FileString">Path to store the file to (with file name).</param>
        ''' <param name="CutoutString">Cutoff digits.</param>
        ''' <param name="TimeoutVal">Timeout for action.</param>
        Public Sub RecordFile(ByVal FileString As String, ByVal CutoutString As String, ByVal TimeoutVal As Integer)
            APCIf.APCRecordFile(oStateLine, oStateLine, FileString, CutoutString, TimeoutVal)
        End Sub

        ''' <summary>
        ''' Stops file playing for the line.
        ''' </summary>
        Public Sub StopPlay()
            APCIf.APCReset(oStateLine)
        End Sub

        ''' <summary>
        ''' Stops file recording for the line.
        ''' </summary>
        Public Sub StopRecord()
            APCIf.APCReset(oStateLine)
        End Sub

        ''' <summary>
        ''' Resets all commands of the line.
        ''' </summary>
        Public Sub Reset()
            APCIf.APCReset(oStateLine)
        End Sub

        ''' <summary>
        ''' Clears the line.
        ''' </summary>
        ''' <remarks>
        ''' <seealso cref="Reset"/></remarks>
        Public Sub Clear()
            Reset()
        End Sub

        ''' <summary>
        ''' Collects number from the line.
        ''' </summary>
        ''' <param name="NumberVar">Variable name to collect number to.</param>
        ''' <param name="MaxDigits">Maximum number of digits to collect.</param>
        Public Sub CollectNumber(ByVal NumberVar As String, ByVal MaxDigits As Integer)
            APCIf.APCCollectNumber(oStateLine, NumberVar, MaxDigits, "#*", 7, 5, 0)
        End Sub

        ''' <summary>
        ''' Collects number from the line.
        ''' </summary>
        ''' <param name="NumberVar">Variable name to collect number to.</param>
        ''' <param name="MaxDigits">Maximum number of digits to collect.</param>
        ''' <param name="CutoutString">Cutoff digits.</param>
        Public Sub CollectNumber(ByVal NumberVar As String, ByVal MaxDigits As Integer, ByVal CutoutString As String)
            APCIf.APCCollectNumber(oStateLine, NumberVar, MaxDigits, CutoutString, 7, 5, 0)
        End Sub

        ''' <summary>
        ''' Collects number from the line.
        ''' </summary>
        ''' <param name="NumberVar">Variable name to collect number to.</param>
        ''' <param name="MaxDigits">Maximum number of digits to collect.</param>
        ''' <param name="CutoutString">Cutoff digits.</param>
        ''' <param name="InitTimeoutVal">Timeout for the action.</param>
        ''' <param name="DigitTimeoutVal">Timeout between neighboring digits.</param>
        Public Sub CollectNumber(ByVal NumberVar As String, ByVal MaxDigits As Integer, ByVal CutoutString As String, ByVal InitTimeoutVal As Integer, ByVal DigitTimeoutVal As Integer)
            APCIf.APCCollectNumber(oStateLine, NumberVar, MaxDigits, CutoutString, InitTimeoutVal, DigitTimeoutVal, 0)
        End Sub

        ''' <summary>
        ''' Collects number from the line.
        ''' </summary>
        ''' <param name="NumberVar">Variable name to collect number to.</param>
        ''' <param name="MaxDigits">Maximum number of digits to collect.</param>
        ''' <param name="CutoutString">Cutoff digits.</param>
        ''' <param name="InitTimeoutVal">Timeout for the action.</param>
        ''' <param name="DigitTimeoutVal">Timeout between neighboring digits.</param>
        ''' <param name="TimeoutVal">Timeout for the whole action.</param>
        Public Sub CollectNumber(ByVal NumberVar As String, ByVal MaxDigits As Integer, ByVal CutoutString As String, ByVal InitTimeoutVal As Integer, ByVal DigitTimeoutVal As Integer, ByVal TimeoutVal As Integer)
            APCIf.APCCollectNumber(oStateLine, NumberVar, MaxDigits, CutoutString, InitTimeoutVal, DigitTimeoutVal, TimeoutVal)
        End Sub

        ''' <summary>
        ''' Plays number for the line.
        ''' </summary>
        ''' <param name="DataNumber">The number to play.</param>
        Public Sub PlayNumber(ByVal DataNumber As Object)
            APCIf.APCPlayNumber(oStateLine, oStateLine, DataNumber, "number", "ALL", 0)
        End Sub

        ''' <summary>
        ''' Plays number for the line.
        ''' </summary>
        ''' <param name="DataNumber">The number to play.</param>
        ''' <param name="RuleName">Name of the rule to use for the number.</param>
        Public Sub PlayNumber(ByVal DataNumber As Object, ByVal RuleName As String)
            APCIf.APCPlayNumber(oStateLine, oStateLine, DataNumber, RuleName, "ALL", 0)
        End Sub

        ''' <summary>
        ''' Plays number for the line.
        ''' </summary>
        ''' <param name="DataNumber">The number to play.</param>
        ''' <param name="RuleName">Name of the rule to use for the number.</param>
        ''' <param name="CutoutString">Cutoff digits.</param>
        Public Sub PlayNumber(ByVal DataNumber As Object, ByVal RuleName As String, ByVal CutoutString As String)
            APCIf.APCPlayNumber(oStateLine, oStateLine, DataNumber, RuleName, CutoutString, 0)
        End Sub

        ''' <summary>
        ''' Plays number for the line.
        ''' </summary>
        ''' <param name="DataNumber">The number to play.</param>
        ''' <param name="RuleName">Name of the rule to use for the number.</param>
        ''' <param name="CutoutString">Cutoff digits.</param>
        ''' <param name="TimeoutVal">Timeout for the action.</param>
        Public Sub PlayNumber(ByVal DataNumber As Object, ByVal RuleName As String, ByVal CutoutString As String, ByVal TimeoutVal As Integer)
            APCIf.APCPlayNumber(oStateLine, oStateLine, DataNumber, RuleName, CutoutString, TimeoutVal)
        End Sub

        ' Line Control Methods of the class LineState
        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Releases the line from APC control.
        ''' </summary>
        ''' <remarks><seealso cref="IAPCInterface.APCRelease"/></remarks>
        Public Sub Release()
            APCIf.APCRelease(oStateLine, oStateLine, 0)
        End Sub

        ''' <summary>
        ''' Releases the line from APC control.
        ''' </summary>
        ''' <param name="TimeoutVal">Timeout for the action.</param>
        ''' <remarks><seealso cref="IAPCInterface.APCRelease"/></remarks>
        Public Sub Release(ByVal TimeoutVal As Integer)
            APCIf.APCRelease(oStateLine, oStateLine, TimeoutVal)
        End Sub

        ''' <summary>
        ''' Answers call and puts line under APC control.
        ''' </summary>
        ''' <remarks><seealso cref="IAPCInterface.APCAnswer"/></remarks>
        Public Sub AnswerCall()
            APCIf.APCAnswer(oStateLine, oStateLine, 0)
        End Sub

        ''' <summary>
        ''' Answers call and puts line under APC control.
        ''' </summary>
        ''' <param name="TimeoutVal">Timeout for the action.</param>
        ''' <remarks><seealso cref="IAPCInterface.APCAnswer"/></remarks>
        Public Sub AnswerCall(ByVal TimeoutVal As Integer)
            APCIf.APCAnswer(oStateLine, oStateLine, TimeoutVal)
        End Sub


        ''' <summary>
        ''' Rejects the incoming call.
        ''' </summary>
        ''' <remarks><seealso cref="IAPCInterface.APCReject"/></remarks>
        Public Sub RejectCall()
            APCIf.APCReject(oStateLine, oStateLine, 0, 0)
        End Sub

        ''' <summary>
        ''' Rejects the incoming call.
        ''' </summary>
        ''' <param name="RejectReason">Reason for the call rejection.</param>
        ''' <remarks><seealso cref="IAPCInterface.APCReject"/></remarks>
        Public Sub RejectCall(ByVal RejectReason As Integer)
            APCIf.APCReject(oStateLine, oStateLine, RejectReason, 0)
        End Sub

        ''' <summary>
        ''' Rejects the incoming call.
        ''' </summary>
        ''' <param name="RejectReason">Reason for the call rejection.</param>
        ''' <param name="TimeoutVal">Timeout for the action.</param>
        ''' <remarks><seealso cref="IAPCInterface.APCReject"/></remarks>
        Public Sub RejectCall(ByVal RejectReason As Integer, ByVal TimeoutVal As Integer)
            APCIf.APCReject(oStateLine, oStateLine, RejectReason, TimeoutVal)
        End Sub


        ''' <summary>
        ''' Releases the line from APC control.
        ''' </summary>
        ''' <remarks><seealso cref="IAPCInterface.APCRelease"/></remarks>
        Public Sub DropCall()
            APCIf.APCRelease(oStateLine, oStateLine, 0)
        End Sub

        ''' <summary>
        ''' Releases the line from APC control.
        ''' </summary>
        ''' <param name="TimeoutVal">Timeout for the action.</param>
        ''' <remarks><seealso cref="IAPCInterface.APCRelease"/></remarks>
        Public Sub DropCall(ByVal TimeoutVal As Integer)
            APCIf.APCRelease(oStateLine, oStateLine, TimeoutVal)
        End Sub

        ''' <summary>
        ''' Pass the call to the regular processing from APC control.
        ''' </summary>
        ''' <remarks><seealso cref="IAPCInterface.APCPass"/></remarks>
        Public Sub PassCall()
            APCIf.APCPass(oStateLine, oStateLine, 0)
        End Sub

        ''' <summary>
        ''' Pass the call to the regular processing from APC control.
        ''' </summary>
        ''' <param name="TimeoutVal">Timeout for the action.</param>
        ''' <remarks><seealso cref="IAPCInterface.APCPass"/></remarks>
        Public Sub PassCall(ByVal TimeoutVal As Integer)
            APCIf.APCPass(oStateLine, oStateLine, TimeoutVal)
        End Sub

        ''' <summary>
        ''' Dials specified number.
        ''' </summary>
        ''' <param name="NumberString">Number to dial.</param>
        ''' <remarks><seealso cref="IAPCInterface.APCDial"/></remarks>
        Public Sub Dial(ByVal NumberString As String)
            APCIf.APCDial(oStateLine, oStateLine, NumberString, Nothing, 0)
        End Sub

        ''' <summary>
        ''' Dials specified number.
        ''' </summary>
        ''' <param name="NumberString">Number to dial.</param>
        ''' <param name="SourceNumber">Number of caller.</param>
        ''' <remarks><seealso cref="IAPCInterface.APCDial"/></remarks>
        Public Sub Dial(ByVal NumberString As String, ByVal SourceNumber As String)
            APCIf.APCDial(oStateLine, oStateLine, NumberString, SourceNumber, 0)
        End Sub

        ''' <summary>
        ''' Dials specified number.
        ''' </summary>
        ''' <param name="NumberString">Number to dial.</param>
        ''' <param name="SourceNumber">Number of caller.</param>
        ''' <param name="TimeoutVal">Timeout for the action.</param>
        ''' <remarks><seealso cref="IAPCInterface.APCDial"/></remarks>
        Public Sub Dial(ByVal NumberString As String, ByVal SourceNumber As String, ByVal TimeoutVal As Integer)
            APCIf.APCDial(oStateLine, oStateLine, NumberString, SourceNumber, TimeoutVal)
        End Sub

        ''' <summary>
        ''' Trasfers incoming call to the specified number.
        ''' </summary>
        ''' <param name="NumberString">Number to transfer the call to.</param>
        ''' <remarks><seealso cref="IAPCInterface.APCTransfer"/></remarks>
        Public Sub Transfer(ByVal NumberString As String)
            APCIf.APCTransfer(oStateLine, oStateLine, NumberString, Nothing, 0)
        End Sub

        ''' <summary>
        ''' Trasfers incoming call to the specified number.
        ''' </summary>
        ''' <param name="NumberString">Number to transfer the call to.</param>
        ''' <param name="SourceNumber">Number of the line which transfers the call.</param>
        ''' <remarks><seealso cref="IAPCInterface.APCTransfer"/></remarks>
        Public Sub Transfer(ByVal NumberString As String, ByVal SourceNumber As String)
            APCIf.APCTransfer(oStateLine, oStateLine, NumberString, SourceNumber, 0)
        End Sub
        ''' <summary>
        ''' Trasfers incoming call to the specified number.
        ''' </summary>
        ''' <param name="NumberString">Number to transfer the call to.</param>
        ''' <param name="TimeoutVal">Timeout for the action.</param>
        ''' <remarks><seealso cref="IAPCInterface.APCTransfer"/></remarks>
        Public Sub Transfer(ByVal NumberString As String, ByVal TimeoutVal As Integer)
            APCIf.APCTransfer(oStateLine, oStateLine, NumberString, Nothing, TimeoutVal)
        End Sub

        ''' <summary>
        ''' Trasfers incoming call to the specified number.
        ''' </summary>
        ''' <param name="NumberString">Number to transfer the call to.</param>
        ''' <param name="SourceNumber">Number of the line which transfers the call.</param>
        ''' <param name="TimeoutVal">Timeout for the action.</param>
        ''' <remarks><seealso cref="IAPCInterface.APCTransfer"/></remarks>
        Public Sub Transfer(ByVal NumberString As String, ByVal SourceNumber As String, ByVal TimeoutVal As Integer)
            APCIf.APCTransfer(oStateLine, oStateLine, NumberString, SourceNumber, TimeoutVal)
        End Sub

        ''' <summary>
        ''' Waits for a digit on the line.
        ''' </summary>
        ''' <remarks><seealso cref="IAPCInterface.APCWaitDigit"/></remarks>
        Public Sub WaitDigit()
            APCIf.APCWaitDigit(oStateLine, 0)
        End Sub

        ''' <summary>
        ''' Waits for a digit on the line.
        ''' </summary>
        ''' <param name="TimeoutVal">Timeout for action.</param>
        ''' <remarks><seealso cref="IAPCInterface.APCWaitDigit"/></remarks>
        Public Sub WaitDigit(ByVal TimeoutVal As Integer)
            APCIf.APCWaitDigit(oStateLine, TimeoutVal)
        End Sub

        ''' <summary>
        ''' Waits for a query of commands to be completed.
        ''' </summary>
        ''' <remarks><seealso cref="IAPCInterface.APCWaitComplete"/></remarks>
        Public Sub WaitComplete()
            APCIf.APCWaitComplete(oStateLine, 0)
        End Sub

        ''' <summary>
        ''' Waits for a query of commands to be completed.
        ''' </summary>
        ''' <param name="TimeoutVal">Timeout for action.</param>
        ''' <remarks><seealso cref="IAPCInterface.APCWaitComplete"/></remarks>
        Public Sub WaitComplete(ByVal TimeoutVal As Integer)
            APCIf.APCWaitComplete(oStateLine, TimeoutVal)
        End Sub

        ''' <summary>
        ''' Sets a delay for the line depending on parameter.
        ''' </summary>
        ''' <param name="TimeoutVal">Timeout for action.</param>
        ''' <remarks><seealso cref="IAPCInterface.APCWaitTimer"/></remarks>
        Public Sub Pause(ByVal TimeoutVal As Integer)
            APCIf.APCWaitTimer(oStateLine, TimeoutVal)
        End Sub

        ''' <summary>
        ''' Sets a delay for the line depending on parameter.
        ''' </summary>
        ''' <param name="TimeoutVal">Timeout for action.</param>
        ''' <remarks><seealso cref="IAPCInterface.APCWaitTimer"/></remarks>
        Public Sub WaitTimer(ByVal TimeoutVal As Integer)
            APCIf.APCWaitTimer(oStateLine, TimeoutVal)
        End Sub

        ''' <summary>
        ''' Raises <see cref="APCEvents.LINKREQUEST"/> event for a linked line.
        ''' </summary>
        ''' <remarks><seealso cref="APCLine.FireStateEvent"/></remarks>
        Public Sub RequestLink()
            oStateLine.FireStateEvent(oSourceLine, APCEvents.LINKREQUEST, Nothing, True)
        End Sub

        ''' <summary>
        ''' Raises <see cref="APCEvents.LINKREQUEST"/> event for a specified line.
        ''' </summary>
        ''' <param name="targetLine">Line to raise the event for.</param>
        ''' <remarks><seealso cref="APCLine.FireStateEvent"/></remarks>
        Public Sub RequestLink(ByVal targetLine As APCLine)
            oStateLine.FireStateEvent(targetLine, APCEvents.LINKREQUEST, Nothing, True)
        End Sub

        ''' <summary>
        ''' Raises <see cref="APCEvents.GRANTREQUEST"/> event for a linked line.
        ''' </summary>
        ''' <remarks><seealso cref="APCLine.FireStateEvent"/></remarks>
        Public Sub RequestGrant()
            oStateLine.FireStateEvent(oSourceLine, APCEvents.GRANTREQUEST, Nothing, True)
        End Sub

        ''' <summary>
        ''' Raises <see cref="APCEvents.LINKREQUEST"/> event for a specified line.
        ''' </summary>
        ''' <param name="targetLine">Line to raise the event for.</param>
        ''' <remarks><seealso cref="APCLine.FireStateEvent"/></remarks>
        Public Sub RequestGrant(ByVal targetLine As APCLine)
            oStateLine.FireStateEvent(targetLine, APCEvents.GRANTREQUEST, Nothing, True)
        End Sub

        ''' <summary>
        ''' Breakes the link between Master and Slave line.
        ''' </summary>
        ''' <remarks><seealso cref="APCLine.FireStateEvent"/></remarks>
        Public Sub ReleaseLinkSlave()      ' Called by the MasterLine to break the Link 
            Dim otLinkLine As APCLine = oStateLine.LinkLine
            If IsLinkLine() Then
                oStateLine.LinkLine = Nothing ' Remove Link to the Line we controlled
                otLinkLine.LinkLine = Nothing
                otLinkLine.EventLine = otLinkLine ' Now this line can handle all it's events
                oStateLine.FireStateEvent(otLinkLine, APCEvents.LINKRELEASE, Nothing, True)
            End If
        End Sub

        ''' <summary>
        ''' Breakes the link between Master and specified Slave line.
        ''' </summary>
        ''' <param name="linkedLine">Slave line to break the link with.</param>
        ''' <remarks><seealso cref="APCLine.FireStateEvent"/></remarks>
        Public Sub ReleaseLinkSlave(ByVal linkedLine As Object)  ' Called by the MasterLine to break the Link 
            Dim otLinkLine As APCLine = CType(linkedLine, APCLine)
            If IsLinkLine(otLinkLine) Then
                otLinkLine.LinkLine = Nothing
                otLinkLine.EventLine = otLinkLine ' Now this line can handle all it's events
                oStateLine.FireStateEvent(otLinkLine, APCEvents.LINKRELEASE, Nothing, True)
            End If
        End Sub

        ''' <summary>
        ''' Accepts request to link as Slave line.
        ''' </summary>
        ''' <remarks><seealso cref="APCLine.FireStateEvent"/></remarks>
        Public Sub AcceptLinkAsSlave()
            oStateLine.FireStateEvent(oSourceLine, APCEvents.GRANTREQUEST, Nothing, True)
        End Sub

        ''' <summary>
        ''' Accepts request to link as Master line.
        ''' </summary>
        ''' <remarks><seealso cref="APCLine.FireStateEvent"/></remarks>
        Public Sub AcceptGrantAsMaster()
            oStateLine.LinkLine = oSourceLine
            oSourceLine.EventLine = oStateLine
            oSourceLine.LinkLine = oStateLine
        End Sub

        ''' <summary>
        ''' Rejects request to link.
        ''' </summary>
        ''' <remarks><seealso cref="APCLine.FireStateEvent"/></remarks>
        Public Sub RejectLinkOrGrant()
            oStateLine.FireStateEvent(oSourceLine, APCEvents.ELSECONTINUE, Nothing, True)
        End Sub

        ''' <summary>
        ''' Shows do we have any Link'ed lines or not.
        ''' </summary>
        ''' <returns>True if we do have.</returns>
        Private Function IsLinkLine() As Boolean
            Return ((Not oStateLine.LinkLine Is Nothing) AndAlso _
                            (oStateLine.LinkLine.EventLine Is oStateLine))
        End Function

        ''' <summary>
        ''' Shows do we have specified line as Link'ed line or not.
        ''' </summary>
        ''' <param name="linkedLine">Line to check if it is Link'ed line.</param>
        ''' <returns>True if we do have.</returns>
        Private Function IsLinkLine(ByVal linkedLine As APCLine) As Boolean
            Return (Not linkedLine Is Nothing) AndAlso _
                            (linkedLine.EventLine Is oStateLine)
        End Function

        ''' <summary>
        ''' Shows does the event came from Linked Line or not.
        ''' </summary>
        ''' <returns>True if the event came from a Link'ed line.</returns>
        Public Function IsLinkLineEvent() As Boolean
            Return (oStateLine.LinkLine Is oSourceLine)
        End Function

        ''' <summary>
        ''' Shows does the event came from Current Line.
        ''' </summary>
        ''' <returns>True if the event came from the current line.</returns>
        Public Function IsCurrentLineEvent() As Boolean
            Return (oStateLine Is oSourceLine)
        End Function

        ''' <summary>
        ''' Drops the call on the Link'ed line.
        ''' </summary>
        Public Sub LinkDropCall()
            If IsLinkLine() Then
                APCIf.APCRelease(oStateLine, oStateLine.LinkLine, 0)
            End If
        End Sub

        ''' <summary>
        ''' Drops the call on the Link'ed line.
        ''' </summary>
        ''' <param name="TimeoutVal">Timeout for action.</param>
        Public Sub LinkDropCall(ByVal TimeoutVal As Integer)
            If IsLinkLine() Then
                APCIf.APCRelease(oStateLine, oStateLine.LinkLine, TimeoutVal)
            End If
        End Sub

        ''' <summary>
        ''' Releases the Link'ed line.
        ''' </summary>
        Public Sub LinkRelease()
            If IsLinkLine() Then
                APCIf.APCRelease(oStateLine, oStateLine.LinkLine, 0)
            End If
        End Sub

        ''' <summary>
        ''' Releases the Link'ed line.
        ''' </summary>
        ''' <param name="TimeoutVal">Timeout for action.</param>
        Public Sub LinkRelease(ByVal TimeoutVal As Integer)
            If IsLinkLine() Then
                APCIf.APCRelease(oStateLine, oStateLine.LinkLine, TimeoutVal)
            End If
        End Sub

        ''' <summary>
        ''' Transfers the call to the Link'ed line.
        ''' </summary>
        ''' <param name="NumberString">Number to transfer the call to.</param>
        Public Sub LinkTransfer(ByVal NumberString As String)
            If IsLinkLine() Then
                APCIf.APCTransfer(oStateLine, oStateLine.LinkLine, NumberString, Nothing, 0)
            End If
        End Sub

        ''' <summary>
        ''' Transfers the call to the Link'ed line.
        ''' </summary>
        ''' <param name="NumberString">Number to transfer the call to.</param>
        ''' <param name="SourceNumber">Number of the line which transfers the call.</param>
        Public Sub LinkTransfer(ByVal NumberString As String, ByVal SourceNumber As String)
            If IsLinkLine() Then
                APCIf.APCTransfer(oStateLine, oStateLine.LinkLine, NumberString, SourceNumber, 0)
            End If
        End Sub

        ''' <summary>
        ''' Transfers the call to the Link'ed line.
        ''' </summary>
        ''' <param name="NumberString">Number to transfer the call to.</param>
        ''' <param name="SourceNumber">Number of the line which transfers the call.</param>
        ''' <param name="TimeoutVal">Timeout for action.</param>
        Public Sub LinkTransfer(ByVal NumberString As String, ByVal SourceNumber As String, ByVal TimeoutVal As Integer)
            If IsLinkLine() Then
                APCIf.APCTransfer(oStateLine, oStateLine.LinkLine, NumberString, SourceNumber, TimeoutVal)
            End If
        End Sub

        ''' <summary>
        ''' Plays DTMF code on the Link'ed line.
        ''' </summary>
        ''' <param name="DTMFString">DTMF code to play.</param>
        Public Sub LinkPlayDTMF(ByVal DTMFString As String)
            If IsLinkLine() Then
                APCIf.APCPlayDTMF(oStateLine, oStateLine.LinkLine, DTMFString, 0)
            End If
        End Sub

        ''' <summary>
        ''' Plays DTMF code on the Link'ed line.
        ''' </summary>
        ''' <param name="DTMFString">DTMF code to play.</param>
        ''' <param name="TimeoutVal">Timeout for action.</param>
        Public Sub LinkPlayDTMF(ByVal DTMFString As String, ByVal TimeoutVal As Integer)
            If IsLinkLine() Then
                APCIf.APCPlayDTMF(oStateLine, oStateLine.LinkLine, DTMFString, TimeoutVal)
            End If
        End Sub

        ''' <summary>
        ''' Plays file on the Link'ed line.
        ''' </summary>
        ''' <param name="FileString">Path to the file to play.</param>
        Public Sub LinkPlayFile(ByVal FileString As String)
            If IsLinkLine() Then
                APCIf.APCPlayFile(oStateLine, oStateLine.LinkLine, FileString, "ALL", 0)
            End If
        End Sub

        ''' <summary>
        ''' Plays file on the Link'ed line.
        ''' </summary>
        ''' <param name="FileString">Path to the file to play.</param>
        ''' <param name="CutoutString">Cutoff digits.</param>
        Public Sub LinkPlayFile(ByVal FileString As String, ByVal CutoutString As String)
            If IsLinkLine() Then
                APCIf.APCPlayFile(oStateLine, oStateLine.LinkLine, FileString, CutoutString, 0)
            End If
        End Sub

        ''' <summary>
        ''' Plays file on the Link'ed line.
        ''' </summary>
        ''' <param name="FileString">Path to the file to play.</param>
        ''' <param name="CutoutString">Cutoff digits.</param>
        ''' <param name="TimeoutVal">Timeout for action.</param>
        Public Sub LinkPlayFile(ByVal FileString As String, ByVal CutoutString As String, ByVal TimeoutVal As Integer)
            If IsLinkLine() Then
                APCIf.APCPlayFile(oStateLine, oStateLine.LinkLine, FileString, CutoutString, TimeoutVal)
            End If
        End Sub

        ''' <summary>
        ''' Records the file on the Link'ed line.
        ''' </summary>
        ''' <param name="FileString">Path to store the file.</param>
        Public Sub LinkRecordFile(ByVal FileString As String)
            If IsLinkLine() Then
                APCIf.APCRecordFile(oStateLine, oStateLine.LinkLine, FileString, "ALL", 0)
            End If
        End Sub

        ''' <summary>
        ''' Records the file on the Link'ed line.
        ''' </summary>
        ''' <param name="FileString">Path to store the file.</param>
        ''' <param name="CutoutString">Cutoff digits.</param>
        Public Sub LinkRecordFile(ByVal FileString As String, ByVal CutoutString As String)
            If IsLinkLine() Then
                APCIf.APCRecordFile(oStateLine, oStateLine.LinkLine, FileString, CutoutString, 0)
            End If
        End Sub

        ''' <summary>
        ''' Records the file on the Link'ed line.
        ''' </summary>
        ''' <param name="FileString">Path to store the file.</param>
        ''' <param name="CutoutString">Cutoff digits.</param>
        ''' <param name="TimeoutVal">Timeout for action.</param>
        Public Sub LinkRecordFile(ByVal FileString As String, ByVal CutoutString As String, ByVal TimeoutVal As Integer)
            If IsLinkLine() Then
                APCIf.APCRecordFile(oStateLine, oStateLine.LinkLine, FileString, CutoutString, TimeoutVal)
            End If
        End Sub

        ''' <summary>
        ''' Stops playing file on the Link'ed line.
        ''' </summary>
        Public Sub LinkStopPlay()
            LinkReset()
        End Sub

        ''' <summary>
        ''' Stops file recording on the Link'ed line.
        ''' </summary>
        Public Sub LinkStopRecord()
            LinkReset()
        End Sub

        ''' <summary>
        ''' Resets the Link'ed line.
        ''' </summary>
        Public Sub LinkReset()
            If IsLinkLine() Then
                APCIf.APCReset(oStateLine.LinkLine)
            End If
        End Sub

        ''' <summary>
        ''' Clears the Link'ed line.
        ''' </summary>
        Public Sub LinkClear()
            LinkReset()
        End Sub

        ''' <summary>
        ''' Tells the Link'ed line to wait for a digit.
        ''' </summary>
        Public Sub LinkWaitDigit()
            If IsLinkLine() Then
                APCIf.APCWaitDigit(oStateLine.LinkLine, 0)
            End If
        End Sub

        ''' <summary>
        ''' Tells the Link'ed line to wait a digit.
        ''' </summary>
        ''' <param name="TimeoutVal">Timeout for action.</param>
        Public Sub LinkWaitDigit(ByVal TimeoutVal As Integer)
            If IsLinkLine() Then
                APCIf.APCWaitDigit(oStateLine.LinkLine, TimeoutVal)
            End If
        End Sub

        ''' <summary>
        ''' Tells the Link'ed line to wait while all commands will complete.
        ''' </summary>
        Public Sub LinkWaitComplete()
            If IsLinkLine() Then
                APCIf.APCWaitComplete(oStateLine.LinkLine, 0)
            End If
        End Sub

        ''' <summary>
        ''' Tells the Link'ed line to wait while all commands will complete at the specified time.
        ''' </summary>
        ''' <param name="TimeoutVal">Timeout for action.</param>
        Public Sub LinkWaitComplete(ByVal TimeoutVal As Integer)
            If IsLinkLine() Then
                APCIf.APCWaitComplete(oStateLine.LinkLine, TimeoutVal)
            End If
        End Sub

        ''' <summary>
        ''' Pauses the Link'ed line.
        ''' </summary>
        ''' <param name="TimeoutVal">Time to pause for.</param>
        Public Sub LinkPause(ByVal TimeoutVal As Integer)
            If IsLinkLine() Then
                APCIf.APCWaitTimer(oStateLine.LinkLine, TimeoutVal)
            End If
        End Sub

        ''' <summary>
        ''' Pauses the Link'ed line.
        ''' </summary>
        ''' <param name="TimeoutVal">Time to pause for.</param>
        Public Sub LinkWaitTimer(ByVal TimeoutVal As Integer)
            LinkPause(TimeoutVal)
        End Sub

        ' Variables control Properties and Methods
        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets access to the named variable.
        ''' </summary>
        ''' <param name="NameVar">Name to the variable to take access to.</param>
        ''' <value>The named variable.</value>
        Public Property Var(ByVal NameVar As String) As Object
            Get
                Return oStateLine.Var(NameVar)
            End Get
            Set(ByVal Value As Object)
                oStateLine.Var(NameVar) = Value
            End Set
        End Property

        ''' <summary>
        ''' Gets access to the named variable.
        ''' </summary>
        ''' <param name="NameVar">Name to the variable to take access to.</param>
        ''' <value>The named variable.</value>
        Default Public Property Item(ByVal NameVar As String) As Object
            Get
                Return oStateLine.Var(NameVar)
            End Get
            Set(ByVal Value As Object)
                oStateLine.Var(NameVar) = Value
            End Set
        End Property

        ' Variables control Properties and Methods
        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets access to the named variable of the Link'ed line.
        ''' </summary>
        ''' <param name="NameVar">Name to the variable to take access to.</param>
        ''' <value>The named variable.</value>
        Public Property LinkVar(ByVal NameVar As String) As Object
            Get
                If IsLinkLine() Then
                    Return oStateLine.LinkLine.Var(NameVar)
                Else
                    Return Nothing
                End If
            End Get
            Set(ByVal Value As Object)
                If IsLinkLine() Then
                    oStateLine.LinkLine.Var(NameVar) = Value
                End If
            End Set
        End Property

        ''' <summary>
        ''' Executes non-queried SQL command.
        ''' </summary>
        ''' <param name="SQLServerString">Name of the SQL server to execute command on.</param>
        ''' <param name="SQLCommand">The command to execute.</param>
        Public Sub SQLNonQuery(ByVal SQLServerString As String, ByVal SQLCommand As String)
            APCIf.APCSQLNonQuery(oStateLine, Nothing, SQLServerString, SQLCommand, 0)
        End Sub

        ''' <summary>
        ''' Executes non-queried SQL command.
        ''' </summary>
        ''' <param name="NameVar">Variable to store the result of the command.</param>
        ''' <param name="SQLServerString">Name of the SQL server to execute command on.</param>
        ''' <param name="SQLCommand">The command to execute.</param>
        Public Sub SQLNonQuery(ByVal NameVar As String, ByVal SQLServerString As String, ByVal SQLCommand As String)
            APCIf.APCSQLNonQuery(oStateLine, NameVar, SQLServerString, SQLCommand, 0)
        End Sub

        ''' <summary>
        ''' Executes non-queried SQL command.
        ''' </summary>
        ''' <param name="NameVar">Variable to store the result of the command.</param>
        ''' <param name="SQLServerString">Name of the SQL server to execute command on.</param>
        ''' <param name="SQLCommand">The command to execute.</param>
        ''' <param name="TimeoutVal">Timeout for action.</param>
        Public Sub SQLNonQuery(ByVal NameVar As String, ByVal SQLServerString As String, ByVal SQLCommand As String, ByVal TimeoutVal As Integer)
            APCIf.APCSQLNonQuery(oStateLine, NameVar, SQLServerString, SQLCommand, TimeoutVal)
        End Sub

        ''' <summary>
        ''' Retrieves scalar value as a result of specified query.
        ''' </summary>
        ''' <param name="SQLServerString">Name of the SQL server to execute command on.</param>
        ''' <param name="SQLCommand">The command to execute.</param>
        Public Sub SQLGetValue(ByVal SQLServerString As String, ByVal SQLCommand As String)
            APCIf.APCSQLGetValue(oStateLine, Nothing, SQLServerString, SQLCommand, 0)
        End Sub

        ''' <summary>
        ''' Retrieves scalar value as a result of specified query.
        ''' </summary>
        ''' <param name="NameVar">Variable to store the result of the command.</param>
        ''' <param name="SQLServerString">Name of the SQL server to execute command on.</param>
        ''' <param name="SQLCommand">The command to execute.</param>
        Public Sub SQLGetValue(ByVal NameVar As String, ByVal SQLServerString As String, ByVal SQLCommand As String)
            APCIf.APCSQLGetValue(oStateLine, NameVar, SQLServerString, SQLCommand, 0)
        End Sub

        ''' <summary>
        ''' Retrieves scalar value as a result of specified query.
        ''' </summary>
        ''' <param name="NameVar">Variable to store the result of the command.</param>
        ''' <param name="SQLServerString">Name of the SQL server to execute command on.</param>
        ''' <param name="SQLCommand">The command to execute.</param>
        ''' <param name="TimeoutVal">Timeout for action.</param>
        Public Sub SQLGetValue(ByVal NameVar As String, ByVal SQLServerString As String, ByVal SQLCommand As String, ByVal TimeoutVal As Integer)
            APCIf.APCSQLGetValue(oStateLine, NameVar, SQLServerString, SQLCommand, TimeoutVal)
        End Sub

        ''' <summary>
        ''' Retrieves data as a result of specified query.
        ''' </summary>
        ''' <param name="SQLServerString">Name of the SQL server to execute command on.</param>
        ''' <param name="SQLCommand">The command to execute.</param>
        Public Sub SQLGetRecord(ByVal SQLServerString As String, ByVal SQLCommand As String)
            APCIf.APCSQLGetRecord(oStateLine, Nothing, SQLServerString, SQLCommand, 0)
        End Sub

        ''' <summary>
        ''' Retrieves data as a result of specified query.
        ''' </summary>
        ''' <param name="NameRecord">Variable to store the result of the command.</param>
        ''' <param name="SQLServerString">Name of the SQL server to execute command on.</param>
        ''' <param name="SQLCommand">The command to execute.</param>
        Public Sub SQLGetRecord(ByVal NameRecord As String, ByVal SQLServerString As String, ByVal SQLCommand As String)
            APCIf.APCSQLGetRecord(oStateLine, NameRecord, SQLServerString, SQLCommand, 0)
        End Sub

        ''' <summary>
        ''' Retrieves data as a result of specified query.
        ''' </summary>
        ''' <param name="NameRecord">Variable to store the result of the command.</param>
        ''' <param name="SQLServerString">Name of the SQL server to execute command on.</param>
        ''' <param name="SQLCommand">The command to execute.</param>
        ''' <param name="TimeoutVal">Timeout for action.</param>
        Public Sub SQLGetRecord(ByVal NameRecord As String, ByVal SQLServerString As String, ByVal SQLCommand As String, ByVal TimeoutVal As Integer)
            APCIf.APCSQLGetRecord(oStateLine, NameRecord, SQLServerString, SQLCommand, TimeoutVal)
        End Sub

        ''' <summary>
        ''' Executes stored procedure.
        ''' </summary>
        ''' <param name="SQLServerString">Name of the SQL server to execute command on.</param>
        ''' <param name="SQLCommand">The stored procedure to execute.</param>
        Public Sub SQLExecProcedure(ByVal SQLServerString As String, ByVal SQLCommand As String)
            APCIf.APCSQLExecProcedure(oStateLine, Nothing, SQLServerString, SQLCommand, 0)
        End Sub

        ''' <summary>
        ''' Executes stored procedure.
        ''' </summary>
        ''' <param name="NameRecord">Variable to store the result of the command.</param>
        ''' <param name="SQLServerString">Name of the SQL server to execute command on.</param>
        ''' <param name="SQLCommand">The stored procedure to execute.</param>
        Public Sub SQLExecProcedure(ByVal NameRecord As String, ByVal SQLServerString As String, ByVal SQLCommand As String)
            APCIf.APCSQLExecProcedure(oStateLine, NameRecord, SQLServerString, SQLCommand, 0)
        End Sub

        ''' <summary>
        ''' Executes stored procedure.
        ''' </summary>
        ''' <param name="NameRecord">Variable to store the result of the command.</param>
        ''' <param name="SQLServerString">Name of the SQL server to execute command on.</param>
        ''' <param name="SQLCommand">The stored procedure to execute.</param>
        ''' <param name="TimeoutVal">Timeout for action.</param>
        Public Sub SQLExecProcedure(ByVal NameRecord As String, ByVal SQLServerString As String, ByVal SQLCommand As String, ByVal TimeoutVal As Integer)
            APCIf.APCSQLExecProcedure(oStateLine, NameRecord, SQLServerString, SQLCommand, TimeoutVal)
        End Sub

        ''' <summary>
        ''' Executes stored procedure.
        ''' </summary>
        ''' <param name="SQLServerString">Name of the SQL server to execute command on.</param>
        ''' <param name="SQLCommand">The stored procedure to execute.</param>
        ''' <param name="TimeoutVal">Timeout for action.</param>
        Public Sub SQLExecProcedure(ByVal SQLServerString As String, ByVal SQLCommand As String, ByVal TimeoutVal As Integer)
            APCIf.APCSQLExecProcedure(oStateLine, Nothing, SQLServerString, SQLCommand, TimeoutVal)
        End Sub

        ''' <summary>
        ''' Resets all SQL queries for the line.
        ''' </summary>
        Public Sub SQLReset()
            APCIf.APCSQLReset(oStateLine)
        End Sub

        'Properties of the class LineState
        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets number of the line.
        ''' </summary>
        ''' <value>Number of the line.</value>
        Public ReadOnly Property LineNumber() As String
            Get
                Return oStateLine.LineNumber
            End Get
        End Property

        ''' <summary>
        ''' Gets name of the line.
        ''' </summary>
        ''' <value>Name of the line.</value>
        Public ReadOnly Property LineName() As String
            Get
                Return oStateLine.LineName
            End Get
        End Property

        ''' <summary>
        ''' Gets port of the line.
        ''' </summary>
        ''' <value>Port of the line.</value>
        Public ReadOnly Property PortName() As String
            Get
                Return oStateLine.LinePort
            End Get
        End Property

        ''' <summary>
        ''' Gets type of the line.
        ''' </summary>
        ''' <value>Type of the line.</value>
        Public ReadOnly Property LineType() As String
            Get
                Return oStateLine.LineType
            End Get
        End Property

        ''' <summary>
        ''' Gets access code for the line.
        ''' </summary>
        ''' <value>Access code for the line.</value>
        Public ReadOnly Property AccessCode() As String
            Get
                Return oStateLine.LineAccessCode
            End Get
        End Property

        ''' <summary>
        ''' Gets or sets DID number of the line.
        ''' </summary>
        ''' <value>DID number of the line.</value>
        Public Property DIDNumber() As String
            Get
                Return oStateLine.DIDNumber
            End Get
            Set(ByVal Value As String)
                oStateLine.DIDNumber = Value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets CID number of the line.
        ''' </summary>
        ''' <value>CID number of the line.</value>
        Public Property CIDNumber() As String
            Get
                Return oStateLine.CIDNumber
            End Get
            Set(ByVal Value As String)
                oStateLine.CIDNumber = Value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets user number of the line.
        ''' </summary>
        ''' <value>User number of the line.</value>
        Public Property UserNumber() As String
            Get
                Return oStateLine.UserNumber
            End Get
            Set(ByVal Value As String)
                oStateLine.UserNumber = Value
            End Set
        End Property

        ''' <summary>
        ''' Gets or Sets User-specific Data.
        ''' </summary>
        ''' <param name="Key">Key to reference to stored Data.</param>
        ''' <returns>Any type of stored Data.</returns>
        Public Property UserData(ByVal Key As Object) As Object
            Get
                Return APCIf.APCUserData(oStateLine, Key)
            End Get
            Set(ByVal Value As Object)
                APCIf.APCUserData(oStateLine, Key) = Value
            End Set
        End Property

        ''' <summary>
        ''' Gets or Sets User-specific Data.
        ''' </summary>
        ''' <param name="TargetLine">Line to store Data on.</param>
        ''' <param name="Key">Key to reference to stored Data.</param>
        ''' <returns>Any type of stored Data.</returns>
        Public Property UserData(ByVal TargetLine As APCLine, ByVal Key As Object) As Object
            Get
                Return APCIf.APCUserData(TargetLine, Key)
            End Get
            Set(ByVal Value As Object)
                APCIf.APCUserData(TargetLine, Key) = Value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets last event on the line.
        ''' </summary>
        ''' <value>Last event on the line.</value>
        Public Property LastEvent() As String
            Get
                Return oStateLine.LastEvent
            End Get
            Set(ByVal Value As String)
                oStateLine.LastEvent = Value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets parameter of the last event.
        ''' </summary>
        ''' <value>Parameter of the last event.</value>
        Public Property LastEventParameter() As String
            Get
                Return oStateLine.LastEventParameter
            End Get
            Set(ByVal Value As String)
                oStateLine.LastEventParameter = Value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets last digit on the line.
        ''' </summary>
        ''' <value>Last digit on the line.</value>
        Public Property LastDigit() As String
            Get
                Return oStateLine.LastDigit
            End Get
            Set(ByVal Value As String)
                oStateLine.LastDigit = Value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets all digits on the line.
        ''' </summary>
        ''' <value>All digits on the line.</value>
        Public Property AllDigits() As String
            Get
                Return oStateLine.AllDigits
            End Get
            Set(ByVal Value As String)
                oStateLine.AllDigits = Value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets timeout for the line (delay the line will receive the <see cref="APCEvents.TIMEOUT"/> event.
        ''' </summary>
        ''' <value>Timeout for the line (delay the line will receive the <see cref="APCEvents.TIMEOUT"/> event.</value>
        Public Property Timeout() As Integer
            Get
                Return oStateLine.Timeout
            End Get
            Set(ByVal Value As Integer)
                oStateLine.Timeout = Value
            End Set
        End Property

        ''' <summary>
        ''' Gets the state of the line.
        ''' </summary>
        ''' <value>State of the line.</value>
        Public ReadOnly Property State() As String
            Get
                Return oStateLine.State
            End Get
        End Property

        ''' <summary>
        ''' Gets the pure line (<see cref="APCLine"/>).
        ''' </summary>
        ''' <value>The pure line (<see cref="APCLine"/>).</value>
        Public ReadOnly Property Line() As APCLine
            Get
                Return oStateLine
            End Get
        End Property

        ''' <summary>
        ''' Gets or sets the event source line.
        ''' </summary>
        ''' <value>Event source line.</value>
        Public Property SourceLine() As APCLine
            Get
                Return oSourceLine
            End Get
            Set(ByVal Value As APCLine)
                oSourceLine = Value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the line all events will came to.
        ''' </summary>
        ''' <value>Line all events will came to.</value>
        Public Property StateLine() As APCLine
            Get
                Return oStateLine
            End Get
            Set(ByVal Value As APCLine)
                oStateLine = Value
            End Set
        End Property

        ''' <summary>
        ''' Gets the pure line (<see cref="APCLine"/>).
        ''' </summary>
        ''' <value>The pure line (<see cref="APCLine"/>).</value>
        Public ReadOnly Property CurrentLine() As APCLine
            Get
                Return oStateLine
            End Get
        End Property

        ''' <summary>
        ''' Gets the pure line (<see cref="APCLine"/>) the event comes from.
        ''' </summary>
        ''' <value>The pure line (<see cref="APCLine"/>) the event comes from.</value>
        Public ReadOnly Property EventLine() As APCLine
            Get
                Return oSourceLine
            End Get
        End Property

        ''' <summary>
        ''' Gets the pure (<see cref="APCLine"/>) Link'ed line.
        ''' </summary>
        ''' <value>The pure (<see cref="APCLine"/>) Link'ed line.</value>
        Public ReadOnly Property LinkLine() As APCLine
            Get
                Return oStateLine.LinkLine
            End Get
        End Property

        ''' <summary>
        ''' Tells the line to go to the specified state.
        ''' </summary>
        ''' <param name="vNewState">The state line should go to.</param>
        Public Sub NewState(ByVal vNewState As String)
            If Not IsNothing(vNewState) Then
                oStateLine.FireStateEvent(oStateLine, APCEvents.NEWSTATE, vNewState, False)
            End If
        End Sub

        ''' <summary>
        ''' Tells the line to go to the specified state.
        ''' </summary>
        ''' <param name="vNewState">The state line should go to.</param>
        Public Sub GotoState(ByVal vNewState As String)
            NewState(vNewState)
        End Sub

        ''' <summary>
        ''' Tells the line to go to the specified state.
        ''' </summary>
        ''' <param name="vNewState">The state line should go to.</param>
        Public Sub GotoState(ByVal vNewState As StateFunction)
            NewState(vNewState.Method.Name)
        End Sub
        ''' <summary>
        ''' Tells the line to go to the previous state.
        ''' </summary>
        Public Sub ReturnState()
            oStateLine.PopState()
        End Sub

        ''' <summary>
        ''' Tells the line to go to the previous state.
        ''' </summary>
        Public Sub StateReturn()
            oStateLine.PopState()
        End Sub

        ''' <summary>
        ''' Calls the state function with return parameter.
        ''' </summary>
        ''' <param name="vNewState">New state for the line.</param>
        ''' <param name="vReturnState">The state for the line after ReturnStateFunction is executed.</param>
        Public Sub CallStateFunction(ByVal vNewState As String, ByVal vReturnState As String)
            If Not IsNothing(vNewState) AndAlso Not IsNothing(vReturnState) Then
                oStateLine.PushFunctionState(vReturnState)
                oStateLine.FireStateEvent(oStateLine, APCEvents.NEWSTATE, vNewState, False)
            End If
        End Sub
        ''' <summary>
        ''' Calls the state function with return parameter.
        ''' </summary>
        ''' <param name="vNewState">New state for the line.</param>
        ''' <param name="vReturnState">The state for the line after ReturnStateFunction is executed.</param>
        Public Sub CallStateFunction(ByVal vNewState As StateFunction, ByVal vReturnState As StateFunction)
            If Not IsNothing(vNewState) AndAlso Not IsNothing(vReturnState) Then
                oStateLine.PushFunctionState(vReturnState.Method.Name)
                oStateLine.FireStateEvent(oStateLine, APCEvents.NEWSTATE, vNewState.Method.Name, False)
            End If
        End Sub

        Public Sub ReturnStateFunction()
            oStateLine.FireStateEvent(oStateLine, APCEvents.NEWSTATE, oStateLine.PopFunctionState(), False)
        End Sub

        ''' <summary>
        ''' Tells the Link'ed line to go to the specified state.
        ''' </summary>
        ''' <param name="vNewState">The state line should go to.</param>
        Public Sub LinkNewState(ByVal vNewState As String)
            Dim otLinkLine As APCLine = oStateLine.LinkLine

            If (Not IsNothing(vNewState)) And IsLinkLine() Then
                ' Remove links from both lines
                otLinkLine.EventLine = otLinkLine
                otLinkLine.LinkLine = Nothing
                oStateLine.LinkLine = Nothing
                oStateLine.FireStateEvent(otLinkLine, APCEvents.NEWSTATE, vNewState, False)
            End If
        End Sub
        ''' <summary>
        ''' Tells the Link'ed line to go to the specified state.
        ''' </summary>
        ''' <param name="vNewState">The state line should go to.</param>
        Public Sub LinkNewState(ByVal vNewState As StateFunction)
            Dim otLinkLine As APCLine = oStateLine.LinkLine

            If (Not IsNothing(vNewState)) And IsLinkLine() Then
                ' Remove links from both lines
                otLinkLine.EventLine = otLinkLine
                otLinkLine.LinkLine = Nothing
                oStateLine.LinkLine = Nothing
                oStateLine.FireStateEvent(otLinkLine, APCEvents.NEWSTATE, vNewState.Method.Name, False)
            End If
        End Sub

        ''' <summary>
        ''' Tells the Link'ed line to go to the specified state.
        ''' </summary>
        ''' <param name="vNewState">The state line should go to.</param>
        Public Sub LinkGotoState(ByVal vNewState As String)
            LinkNewState(vNewState)
        End Sub
        ''' <summary>
        ''' Tells the Link'ed line to go to the specified state.
        ''' </summary>
        ''' <param name="vNewState">The state line should go to.</param>
        Public Sub LinkGotoState(ByVal vNewState As StateFunction)
            LinkNewState(vNewState.Method.Name)
        End Sub

        ''' <summary>
        ''' Tells the Link'ed line to go to the previous state.
        ''' </summary>
        Public Sub LinkReturnState()
            Dim otLinkLine As APCLine = oStateLine.LinkLine

            If IsLinkLine() Then
                otLinkLine.EventLine = otLinkLine
                otLinkLine.LinkLine = Nothing
                oStateLine.LinkLine = Nothing
            End If
        End Sub

        ''' <summary>
        ''' Sends the <see cref="APCEvents.ELSECONTINUE"/> (reject) event.
        ''' </summary>
        Public Sub SendRejectEvent()
            oStateLine.FireStateEvent(oSourceLine, APCEvents.ELSECONTINUE, Nothing, True)
        End Sub

        ''' <summary>
        ''' Initializes the line.
        ''' </summary>
        Public Sub Init()
            Initialize(oStateLine)
        End Sub

        ''' <summary>
        ''' Initializes specified line.
        ''' </summary>
        Private Sub Initialize(ByVal TargetLine As APCLine)
            oStateLine = TargetLine
            oSourceLine = TargetLine
            ReleaseLinkSlave()
            oStateLine.Init()
            APCIf.APCResetAll(oStateLine)
            If (Not oStateLine.State.Equals(oStateLine.InitialState)) Then
                oStateLine.FireStateEvent(oStateLine, APCEvents.NEWSTATE, oStateLine.InitialState, False)
            End If
        End Sub

        ''' <summary>
        ''' Resets both commands and SQL queues of the line.
        ''' </summary>
        Public Sub ResetAll()
            APCIf.APCResetAll(oStateLine)
        End Sub

        ''' <summary>
        ''' Connects current line with it's Link'ed line.
        ''' </summary>
        Public Sub Connect()
            If IsLinkLine() Then
                APCIf.APCConnect(oStateLine, oStateLine, oStateLine.LinkLine, 0)
            End If
        End Sub

        ''' <summary>
        ''' Connects current line with it's Link'ed line.
        ''' </summary>
        ''' <param name="TimeoutVal">Timeout for the opaeration.</param>
        Public Sub Connect(ByVal TimeoutVal As Integer)
            If IsLinkLine() Then
                APCIf.APCConnect(oStateLine, oStateLine, oStateLine.LinkLine, TimeoutVal)
            End If
        End Sub

        ''' <summary>
        ''' Disconnects the line from the line it was connected to and snatches it under APC control.
        ''' </summary>
        Public Sub Disconnect()
            APCIf.APCDisconnect(oStateLine, oStateLine, 0)
        End Sub

        ''' <summary>
        ''' Disconnects specified line from the line it was connected to and snatches it under APC control.
        ''' </summary>
        ''' <param name="TargetLine">Line to snatch.</param>
        Public Sub Disconnect(ByVal TargetLine As APCLine)
            APCIf.APCDisconnect(oStateLine, TargetLine, 0)
        End Sub

        ''' <summary>
        ''' Disconnects specified line from the line it was connected to and snatches it under APC control.
        ''' </summary>
        ''' <param name="TargetLine">Line to snatch.</param>
        ''' <param name="TimeoutVal">Timeout for action.</param>
        Public Sub Disconnect(ByVal TargetLine As APCLine, ByVal TimeoutVal As Integer)
            APCIf.APCDisconnect(oStateLine, TargetLine, TimeoutVal)
        End Sub

        ''' <summary>
        ''' Gets total duration of the call for the line.
        ''' </summary>
        ''' <value>Duration of the call for the line.</value>
        Public ReadOnly Property CallDuration() As Integer
            Get
                Return oStateLine.CallDuration
            End Get
        End Property

        ''' <summary>
        ''' Gets the time ring was detected on the line.
        ''' </summary>
        ''' <value>The time ring was detected on the line.</value>
        Public ReadOnly Property CallRingTime() As DateTime
            Get
                Return oStateLine.CallRingTime
            End Get
        End Property

        ''' <summary>
        ''' Gets time the line was connected to another.
        ''' </summary>
        ''' <value>Time the line was connected to another.</value>
        Public ReadOnly Property CallConnectTime() As DateTime
            Get
                Return oStateLine.CallConnectTime
            End Get
        End Property

        ''' <summary>
        ''' Gets current duration of the call for the line.
        ''' </summary>
        ''' <value>Current duration of the call for the line.</value>
        Public ReadOnly Property InCallTime() As Integer
            Get
                Return oStateLine.InCallTime
            End Get
        End Property

        ''' <summary>
        ''' Gets the time the line was disconnected from another.
        ''' </summary>
        ''' <value>The time the line was disconnected from another.</value>
        Public ReadOnly Property CallDisconnectTime() As DateTime
            Get
                Return oStateLine.CallDisconnectTime
            End Get
        End Property

        ''' <summary>
        ''' Gets total duration of the call for the Link'ed line.
        ''' </summary>
        ''' <value>Duration of the call for the Link'ed line.</value>
        Public ReadOnly Property LinkCallDuration() As Integer
            Get
                If IsLinkLine() Then
                    Return oStateLine.LinkLine.CallDuration
                Else
                    Return 0
                End If
            End Get
        End Property

        ''' <summary>
        ''' Gets the time ring was detected on the Link'ed line.
        ''' </summary>
        ''' <value>The time ring was detected on the Link'ed line.</value>
        Public ReadOnly Property LinkCallRingTime() As DateTime
            Get
                If IsLinkLine() Then
                    Return oStateLine.LinkLine.CallRingTime
                Else
                    Return Nothing
                End If
            End Get
        End Property

        ''' <summary>
        ''' Gets time the Link'ed line was connected to another.
        ''' </summary>
        ''' <value>Time the Link'ed line was connected to another.</value>
        Public ReadOnly Property LinkCallConnectTime() As DateTime
            Get
                If IsLinkLine() Then
                    Return oStateLine.LinkLine.CallConnectTime
                Else
                    Return Nothing
                End If
            End Get
        End Property

        ''' <summary>
        ''' Gets current duration of the call for the Link'ed line.
        ''' </summary>
        ''' <value>Current duration of the call for the Link'ed line.</value>
        Public ReadOnly Property LinkInCallTime() As Integer
            Get
                If IsLinkLine() Then
                    Return oStateLine.LinkLine.InCallTime
                Else
                    Return 0
                End If
            End Get
        End Property

        ''' <summary>
        ''' Gets the time the Link'ed line was disconnected from another.
        ''' </summary>
        ''' <value>The time the Link'ed line was disconnected from another.</value>
        Public ReadOnly Property LinkCallDisconnectTime() As DateTime
            Get
                If IsLinkLine() Then
                    Return oStateLine.LinkLine.CallDisconnectTime
                Else
                    Return Nothing
                End If
            End Get
        End Property

        ''' <summary>
        ''' Sets the language the line uses.
        ''' </summary>
        ''' <param name="LangName">The language the line will use.</param>
        Public Sub UseLanguage(ByVal LangName As String)
            If (Not LangName Is Nothing) Then
                oStateLine.CurrentLanguageSet = LangName
            End If
        End Sub

        ''' <summary>
        ''' Converts data using specified rule.
        ''' </summary>
        ''' <param name="ConversionName">The rule to use.</param>
        ''' <param name="DataNumber">Data to convert.</param>
        ''' <returns>Converted data.</returns>
        Public Function Convert(ByVal ConversionName As String, ByVal DataNumber As Object) As String
            Return APCIf.Convert(oStateLine.CurrentLanguageSet, ConversionName, DataNumber)
        End Function

        ''' <summary>
        ''' Converts data using specified rule.
        ''' </summary>
        ''' <param name="LangName">Language to use for conversion.</param>
        ''' <param name="ConversionName">The rule to use.</param>
        ''' <param name="DataNumber">Data to convert.</param>
        ''' <returns>Converted data.</returns>
        Public Function Convert(ByVal LangName As String, ByVal ConversionName As String, ByVal DataNumber As Object) As String
            Return APCIf.Convert(LangName, ConversionName, DataNumber)
        End Function

        ''' <summary>
        ''' Creates date from the specified arguments.
        ''' </summary>
        ''' <param name="Year">Year.</param>
        ''' <param name="Month">Month.</param>
        ''' <param name="Day">Day.</param>
        ''' <returns>Date in <see cref="DateTime"/> format.</returns>
        Public Function CreateDate(ByVal Year As Integer, ByVal Month As Integer, ByVal Day As Integer) As DateTime
            Return New DateTime(Year, Month, Day)
        End Function

        ''' <summary>
        ''' Creates date from the specified arguments.
        ''' </summary>
        ''' <param name="Hours">Hours.</param>
        ''' <param name="Minutes">Minutes.</param>
        ''' <param name="Seconds">Seconds.</param>
        ''' <returns>Date in <see cref="DateTime"/> format.</returns>
        Public Function CreateTime(ByVal Hours As Integer, ByVal Minutes As Integer, ByVal Seconds As Integer) As TimeSpan
            Return New TimeSpan(Hours, Minutes, Seconds)
        End Function

        ''' <summary>
        ''' Creates date from the specified arguments.
        ''' </summary>
        ''' <param name="Year">Year.</param>
        ''' <param name="Month">Month.</param>
        ''' <param name="Day">Day.</param>
        ''' <param name="Hours">Hours.</param>
        ''' <param name="Minutes">Minutes.</param>
        ''' <param name="Seconds">Seconds.</param>
        ''' <returns>Date in <see cref="DateTime"/> format.</returns>
        Public Function CreateDateTime(ByVal Year As Integer, ByVal Month As Integer, ByVal Day As Integer, _
                ByVal Hours As Integer, ByVal Minutes As Integer, ByVal Seconds As Integer) As DateTime
            Return New DateTime(Year, Month, Day, Hours, Minutes, Seconds)
        End Function

        ''' <summary>
        ''' Resets all asyncronious operations for the line.
        ''' </summary>
        Public Sub AsyncReset()
            APCIf.APCAsyncReset(oStateLine)
        End Sub

        ''' <summary>
        ''' Asyncroniously executes specified procedure.
        ''' </summary>
        ''' <param name="FunctionName">Procedure to execute.</param>
        ''' <param name="stateParam">Procedure parameters.</param>
        Public Sub AsyncExecute(ByVal FunctionName As String, ByVal ParamArray stateParam As Object())
            APCIf.APCAsyncExecute(oStateLine, Nothing, FunctionName, stateParam)
        End Sub

        ''' <summary>
        ''' Asyncroniously executes specified procedure.
        ''' </summary>
        ''' <param name="FunctionName">Procedure to execute.</param>
        Public Sub AsyncExecute(ByVal FunctionName As AsyncFunction)
            APCIf.APCAsyncExecute(oStateLine, Nothing, FunctionName.Method.Name, Nothing)
        End Sub

        ''' <summary>
        ''' Asyncroniously executes specified function.
        ''' </summary>
        ''' <param name="VariableName">Storage for the result of function.</param>
        ''' <param name="FunctionName">Function to execute.</param>
        Public Sub AsyncFunction(ByVal VariableName As String, ByVal FunctionName As String)
            APCIf.APCAsyncExecute(oStateLine, VariableName, FunctionName, Nothing)
        End Sub

        ''' <summary>
        ''' Asyncroniously executes specified function.
        ''' </summary>
        ''' <param name="VariableName">Storage for the result of function.</param>
        ''' <param name="FunctionName">Function to execute.</param>
        Public Sub AsyncFunction(ByVal VariableName As String, ByVal FunctionName As AsyncFunction)
            APCIf.APCAsyncExecute(oStateLine, VariableName, FunctionName.Method.Name, Nothing)
        End Sub

        ''' <summary>
        ''' Asyncroniously executes specified function.
        ''' </summary>
        ''' <param name="VariableName">Storage for the result of function.</param>
        ''' <param name="FunctionName">Function to execute.</param>
        ''' <param name="stateParam">Function parameters.</param>
        Public Sub AsyncFunction(ByVal VariableName As String, ByVal FunctionName As String, ByVal ParamArray stateParam As Object())
            APCIf.APCAsyncExecute(oStateLine, VariableName, FunctionName, stateParam)
        End Sub

        ''' <summary>
        ''' Single event handler (handles only first event of specified type).
        ''' </summary>
        ''' <param name="eventCode">Type of event to handle.</param>
        ''' <param name="eventSub">Procedure to execute when event occurs.</param>
        Public Sub OnEvent(ByVal eventCode As String, ByVal eventSub As String)
            APCIf.APCOnEvent(oStateLine, eventCode, eventSub, False, 0)
        End Sub

        ''' <summary>
        ''' Single event handler (handles only first event of specified type).
        ''' </summary>
        ''' <param name="eventCode">Type of event to handle.</param>
        ''' <param name="eventSub">Procedure to execute when event occurs.</param>
        Public Sub OnEvent(ByVal eventCode As String, ByVal eventSub As StateFunction)
            APCIf.APCOnEvent(oStateLine, eventCode, eventSub.Method.Name, False, 0)
        End Sub

        ''' <summary>
        ''' Sets single event handler (handles only first event of specified type).
        ''' </summary>
        ''' <param name="eventCode">Type of event to handle.</param>
        ''' <param name="eventSub">Procedure to execute when event occurs.</param>
        ''' <param name="TimeoutVal">Timeout for action.</param>
        Public Sub OnEvent(ByVal eventCode As String, ByVal eventSub As String, ByVal TimeoutVal As Integer)
            APCIf.APCOnEvent(oStateLine, eventCode, eventSub, False, TimeoutVal)
        End Sub

        ''' <summary>
        ''' Sets single event handler (handles only first event of specified type).
        ''' </summary>
        ''' <param name="eventCode">Type of event to handle.</param>
        ''' <param name="eventSub">Procedure to execute when event occurs.</param>
        ''' <param name="TimeoutVal">Timeout for action.</param>
        Public Sub OnEvent(ByVal eventCode As String, ByVal eventSub As StateFunction, ByVal TimeoutVal As Integer)
            APCIf.APCOnEvent(oStateLine, eventCode, eventSub.Method.Name, False, TimeoutVal)
        End Sub

        ''' <summary>
        ''' Sets event handler (handles every event of specified type).
        ''' </summary>
        ''' <param name="eventCode">Type of event to handle.</param>
        ''' <param name="eventSub">Procedure to execute when event occurs.</param>
        Public Sub OnEventHandler(ByVal eventCode As String, ByVal eventSub As String)
            APCIf.APCOnEvent(oStateLine, eventCode, eventSub, True, 0)
        End Sub
        ''' <summary>
        ''' Sets event handler (handles every event of specified type).
        ''' </summary>
        ''' <param name="eventCode">Type of event to handle.</param>
        ''' <param name="eventSub">Procedure to execute when event occurs.</param>
        Public Sub OnEventHandler(ByVal eventCode As String, ByVal eventSub As StateFunction)
            APCIf.APCOnEvent(oStateLine, eventCode, eventSub.Method.Name, True, 0)
        End Sub

        ''' <summary>
        ''' Sets event handler (handles every event of specified type).
        ''' </summary>
        ''' <param name="eventCode">Type of event to handle.</param>
        ''' <param name="eventSub">Procedure to execute when event occurs.</param>
        ''' <param name="TimeoutVal">Timeout for action.</param>
        Public Sub OnEventHandler(ByVal eventCode As String, ByVal eventSub As String, ByVal TimeoutVal As Integer)
            APCIf.APCOnEvent(oStateLine, eventCode, eventSub, True, TimeoutVal)
        End Sub
        ''' <summary>
        ''' Sets event handler (handles every event of specified type).
        ''' </summary>
        ''' <param name="eventCode">Type of event to handle.</param>
        ''' <param name="eventSub">Procedure to execute when event occurs.</param>
        ''' <param name="TimeoutVal">Timeout for action.</param>
        Public Sub OnEventHandler(ByVal eventCode As String, ByVal eventSub As StateFunction, ByVal TimeoutVal As Integer)
            APCIf.APCOnEvent(oStateLine, eventCode, eventSub.Method.Name, True, TimeoutVal)
        End Sub
    End Class

End Namespace
