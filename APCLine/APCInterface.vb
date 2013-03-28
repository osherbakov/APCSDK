Namespace Diacom
    Public Interface IAPCInterface
        ' Function to perform required function on the line
        Sub APCPlayDTMF(ByVal srcLine As APCLine, ByVal oLine As APCLine, ByVal dialString As String, ByVal commandTimeOut As Integer)
        Sub APCPlayFile(ByVal srcLine As APCLine, ByVal oLine As APCLine, ByVal FileString As String, ByVal CutoutString As String, ByVal commandTimeOut As Integer)
        Sub APCRecordFile(ByVal srcLine As APCLine, ByVal oLine As APCLine, ByVal FileString As String, ByVal CutoutString As String, ByVal commandTimeOut As Integer)
        Sub APCPlayNumber(ByVal srcLine As APCLine, ByVal oLine As APCLine, ByVal Number As Object, ByVal PlayFormat As String, ByVal CutoutString As String, ByVal commandTimeOut As Integer)
        Sub APCCollectNumber(ByVal oLine As APCLine, ByVal VarName As String, ByVal MaxDigits As Integer, ByVal CutoutString As String, ByVal InitialTimeout As Integer, ByVal InterDigitTimeout As Integer, ByVal commandTimeOut As Integer)
        Sub APCAnswer(ByVal srcLine As APCLine, ByVal oLine As APCLine, ByVal commandTimeOut As Integer)
        Sub APCReject(ByVal srcLine As APCLine, ByVal oLine As APCLine, ByVal rejectReason As Integer, ByVal commandTimeOut As Integer)
        Sub APCRelease(ByVal srcLine As APCLine, ByVal oLine As APCLine, ByVal commandTimeOut As Integer)
        Sub APCPass(ByVal srcLine As APCLine, ByVal oLine As APCLine, ByVal commandTimeOut As Integer)
        Sub APCReset(ByVal oLine As APCLine)
        Sub APCResetAll(ByVal oLine As APCLine)
        Sub APCConnectToServer(ByVal serverName As String, ByVal param1 As String, ByVal param2 As String, ByVal param3 As String)
        Sub APCWaitDigit(ByVal oLine As APCLine, ByVal commandTimeOut As Integer)
        Sub APCWaitComplete(ByVal oLine As APCLine, ByVal commandTimeOut As Integer)
        Sub APCWaitTimer(ByVal oLine As APCLine, ByVal commandTimeOut As Integer)
        Sub APCConnect(ByVal srcLine As APCLine, ByVal oLine As APCLine, ByVal oLinkLine As APCLine, ByVal commandTimeOut As Integer)
        Sub APCDisconnect(ByVal srcLine As APCLine, ByVal oLine As APCLine, ByVal commandTimeOut As Integer)
        '' SQL functions
        Sub APCSQLNonQuery(ByVal oLine As APCLine, ByVal VarName As String, ByVal SQLServerString As String, ByVal SQLCommand As String, ByVal commandTimeOut As Integer)
        Sub APCSQLGetRecord(ByVal oLine As APCLine, ByVal VarName As String, ByVal SQLServerString As String, ByVal SQLCommand As String, ByVal commandTimeOut As Integer)
        Sub APCSQLGetValue(ByVal oLine As APCLine, ByVal VarName As String, ByVal SQLServerString As String, ByVal SQLCommand As String, ByVal commandTimeOut As Integer)
        Sub APCSQLExecProcedure(ByVal oLine As APCLine, ByVal VarName As String, ByVal SQLServerString As String, ByVal SQLCommand As String, ByVal commandTimeOut As Integer)
        Sub APCSQLReset(ByVal oLine As APCLine)
        '' Async functions
        Sub APCAsyncExecute(ByVal oLine As APCLine, ByVal VarName As String, ByVal FunctionName As String, ByVal stateParam As Object())
        Sub APCAsyncReset(ByVal oLine As APCLine)
        '' MakeCall functions
        Sub APCDial(ByVal srcLine As APCLine, ByVal oLine As APCLine, ByVal dialNumber As String, ByVal sourceNumber As String, ByVal commandTimeOut As Integer)
        Sub APCTransfer(ByVal srcLine As APCLine, ByVal oLine As APCLine, ByVal dialString As String, ByVal sourceNumber As String, ByVal commandTimeOut As Integer)
        Sub APCDialReset(ByVal oLine As APCLine)
        '' Data conversion functions
        Function Convert(ByVal LanguageName As String, ByVal ConversionName As String, ByVal DataNumber As Object) As String
        '' Event catcher functions
        Sub APCOnEvent(ByVal oLine As APCLine, ByVal eventCode As String, ByVal eventFunction As String, ByVal eventHandler As Boolean, ByVal commandTimeOut As Integer)
        '' User Data storage and retrieval functions
        Property APCUserData(ByVal oLine As APCLine, ByVal Key As Object) As Object
    End Interface
End Namespace
