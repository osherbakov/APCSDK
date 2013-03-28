Public Class TriumphScript : Inherits Diacom.APCLineControl
    '       DATBASE Connection string and Procedures
    ' Database Connection string - change accordingly to you server name, userID and password
    Const SQLServerConnection As String = "Integrated Security=true;Persist Security Info=False;Initial Catalog=TRIUMPH;Data Source=SERVER\DIACOM;Workstation ID=HOPPER"

    '       CallerID
    ' Stored SQL Procedure to verify CallerID
    Const VerifyCallerID As String = "VerifyCallerID(@@DealerID, @CID, @@DisplayName)"

    '       SUCCESS
    ' Play that phrase before transferring to the AutoAttendant if everything is OK
    Const VerifiedPhrase As String = "100"
    ' Transfer to that AutoAttendant if caller was successfully verified
    Const VerifiedTransfer As String = "333"

    '       NON VERIFIED
    ' Play that phrase before transferring to the AutoAttendant if caller cannot be verified
    Const NonVerifiedPhrase As String = "101"
    ' Transfer to that AutoAttendant if caller cannot be verified
    Const NonVerifiedTransfer As String = "333"

    '       SYSTEM FAILURE
    ' Play that phrase before transferring to the AutoAttendant if Database or any other failure
    Const FailurePhrase As String = "102"
    ' Transfer to that AutoAttendant if during processing the Database or any other error was encountered
    Const FailureTransfer As String = "333"

    '***************************************************************************************************
    Sub Z()     ' Initial State for the Line
        Init()
    End Sub

    Sub ZJ()    ' Event RING "J" came from the line
        AnswerCall()            ' Answer the call and accept connection
    End Sub

    Sub ZJI()   ' Event CONNECT "I" came - now we are connected
        OnEvent("X", "Z")           ' On disconnect "X" event initialize the line
        OnEvent("WS", "SQLError")   ' On Timeout "W" and Error "S" event goto SQLError procedure
        OnEvent("Q", "SQLDataReceived") ' On receiving data from SQL server "Q" - process it
        ' Assign parameters for the SQL procedure call
        If (CIDNumber.Length = 10) Then
            ' Format the US number in a standard form - XXX-YYY-YYYYY
            Var("CID") = CIDNumber.Substring(0, 3) + "-" + CIDNumber.Substring(3, 3) + "-" + CIDNumber.Substring(6, 4)
        Else
            Var("CID") = CIDNumber
        End If
        Var("DealerID") = 0     ' Initially the caller is not verified
        Var("DisplayName") = "" ' Initially no name to display
        If (CIDNumber.Length <> 0) Then
            ' Call SQL procedure to verify Caller ID with 5 sec timeout
            SQLExecProcedure("Return", SQLServerConnection, VerifyCallerID, 5)
        Else
            GotoState("SQLDataReceived")    ' If no CallerID arrived - no need to send SQL request
        End If
    End Sub

    Sub SQLError()  ' SQL Server did not answer within timeout period - "W" or any other error - "S"
        Reset()
        OnEvent("X", "Z")           ' On disconnect "X" event initialize the line
        PlayFile(FailurePhrase)     ' Play Failure phrase
        Transfer(FailureTransfer)  ' Go to AutoAttendant
    End Sub

    Sub SQLDataReceived()   ' Response from SQL server is received - process it
        Reset()
        OnEvent("X", "Z")           ' On disconnect "X" event initialize the line
        ' Non-zero DealerID means OK
        If (CInt(Var("DealerID")) <> 0) Then
            UserData("UserData") = Var("DisplayName")   ' Text to show in AltiAgent
            PlayFile(VerifiedPhrase)    ' Play Success phrase
            Transfer(VerifiedTransfer) ' Go to AutoAttendant for verified callers
        Else
            CallStateFunction("DealerID.Check", "TriumphScript.VerifiedUser")
        End If
    End Sub

    Sub VerifiedUser()   ' Return from procudure of user verification
        Reset()
        OnEvent("X", "Z")           ' On disconnect "X" event initialize the line
        ' Non-zero DealerID means OK
        If CInt(Var("DealerID")) <> 0 Then
            UserData("UserData") = Var("DisplayName")   ' Text to show in AltiAgent
            PlayFile(VerifiedPhrase)    ' Play Success phrase
            Transfer(VerifiedTransfer) ' Go to AutoAttendant for verified callers
        Else
            PlayFile(NonVerifiedPhrase)     ' Play NonVerified phrase
            Transfer(NonVerifiedTransfer)  ' Go to AutoAttendant for non-verified callers
        End If
    End Sub

End Class

'''---------------------------------------------------------------------------------------
''' Class	 : DealerID
''' <summary>
''' Represents the class that asks for and verifies the DealerID from the caller.
''' </summary>
''' -----------------------------------------------------------------------------
public Class DealerID : Inherits Diacom.APCLineControl

    '       DATBASE Connection string and Procedures
    ' Database Connection string - change accordingly to you server name, userID and password
    Const SQLServerConnection As String = "Integrated Security=true;Persist Security Info=False;Initial Catalog=TRIUMPH;Data Source=SERVER\DIACOM;Workstation ID=HOPPER"

    '       DealerID
    ' Stored SQL Procedure to verify DealerID
    Const VerifyDealerID As String = "VerifyDealerID(@@@DealerID, @@DisplayName)"

    '       UpdateDB
    ' Stored SQL Procedure to add CallerID to the Database
    Const AddCallerID As String = "AddCallerID(@DealerID, @CID)"

    ' Phrase to request DealerID number
    Const RequestIDPhrase As String = "28"
    ' How many digits in DealerID to collect from caller 
    Const DealerIDLength As Integer = 6

    ' Number of retries for the caller to enter the correct DealerID
    Const NumberOfAttempts As Integer = 2

    ' Phrase to inform caller that entered Dealer number is incorrect
    Const IncorrectDealerIDPhrase As String = "29"

    Public Sub Check()
        Reset()
        Var("DealerID") = 0     ' NonVerified ID
        Var("Attempts") = 1     ' This is the first attempt
        GotoState("EnterDealerID")
    End Sub

    Sub EnterDealerID()
        PlayFile(RequestIDPhrase)   ' Play "Please enter the DealerID" phrase
        CollectNumber("EnteredDID", DealerIDLength)
    End Sub

    Sub EnterDealerIDZ()    ' Successfully received the number - "Z" Event
        Reset()
        Var("DealerID") = CInt(Var("EnteredDID"))   ' Convert entered string to Number
        SQLExecProcedure("Return", SQLServerConnection, VerifyDealerID, 5)   ' Call SQL Procedure to verify the number with 5 sec timeout
    End Sub

    Sub EnterDealerIDZQ()    ' We received the response from the SQL server
        If CInt(Var("DealerID")) <> 0 Then
            If CStr(Var("CID")) <> "" Then
                ' The DealerID exists, so we have to add the CallerID to the table
                SQLExecProcedure(SQLServerConnection, AddCallerID)   ' Call SQL Procedure to add CallerID
            End If
            ReturnStateFunction()
        ElseIf CInt(Var("Attempts")) < NumberOfAttempts Then
            Var("Attempts") = CInt(Var("Attempts")) + 1
            PlayFile(IncorrectDealerIDPhrase)    ' Play "Incorrect DealerID" phrase
            GotoState("EnterDealerID")
        Else
            ReturnStateFunction()
        End If
    End Sub

    Sub EnterDealerIDZW()   ' SQL request timeout - "W" Event
        Var("DealerID") = 0     ' NonVerified ID
        ReturnStateFunction()
    End Sub

    Sub EnterDealerIDZS()   ' Any Error occured - "S" Event
        Var("DealerID") = 0     ' NonVerified ID
        ReturnStateFunction()
    End Sub

    Sub EnterDealerIDX()    ' Caller hangs up - "X" Event
        Init()
    End Sub
End Class
