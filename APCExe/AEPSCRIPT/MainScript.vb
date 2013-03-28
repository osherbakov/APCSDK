Public Class AEPScript : Inherits Diacom.APCLineControl
    Private Structure Destination
        Public ReadOnly Pattern As String   ' Allowed Destination pattern (Location ID)
        Public ReadOnly Length As Integer   ' Destination length
        Public ReadOnly PrefixDigits As String  ' Digits to add to the beginning of the number

        Sub New(ByVal stringPattern As String, ByVal stringLength As Integer)
            Me.Pattern = stringPattern
            Me.Length = stringLength
            Me.PrefixDigits = ""
        End Sub
        Sub New(ByVal stringPattern As String, ByVal stringLength As Integer, ByVal prefix As String)
            Me.Pattern = stringPattern
            Me.Length = stringLength
            Me.PrefixDigits = prefix
        End Sub
    End Structure

    Private ReadOnly PBXPrefix As String = "9"
    Private ReadOnly MGTSPrefix As String = "7"
    Private ReadOnly IPPrefix As String = "8"

    Private ReadOnly LocalLength As Integer = 4
    Private ReadOnly PBXLength As Integer = 4
    Private ReadOnly IDLocLength As Integer = 1

    Private Const AllowedTimeToUseSeconds As Integer = 45

    Private ReadOnly MaxNumberOfAttempts As Integer = 3
    Private ReadOnly LocalNumbersStartWith As String = "0"
    Private ReadOnly PBXNumbersStartWith As String = "9"
    Private ReadOnly ExternalNumbersStartWith As String = "12345678"
    Private ReadOnly InvalidNumbersStartWith As String = "*#ABCD"
    Private ReadOnly PBXAllowedNumbersStartWith As String = "012345678"

    Private ReadOnly MGTSAccessStartsWith As String = "9"

    ' Lines that are allowed to connect as MGTS Lines
    Private ReadOnly AllowedMGTSNumbers() As String = {"6322483", "6321223", "1110", "1111", "1112", "216", "0612", "416"}

    Private ReadOnly AllowedIPToConnect() As String = {"192.168"}
    Private ReadOnly AllowedIPToAccessPBX() As String = {"192.168"}
    Private ReadOnly AllowedIPToAccessMGTS() As String = {"192.168", "212.118.47", "213.208", "210.212.242.250", "203.197.75.10", "82.116.42.126"}

    Private ReadOnly IPAllowedDestinations As New System.Collections.ArrayList
    Private ReadOnly MGTSAllowedDestinations As New System.Collections.ArrayList

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Checks if the specified address is in the supplied control access list.
    ''' </summary>
    ''' <param name="arrayOfAddresses">Array or collection of addresses.</param>
    ''' <param name="Address">Address to check.</param>
    ''' <returns>True - if found match, False - if not</returns>
    ''' -----------------------------------------------------------------------------
    Private Function IsAddressOK(ByVal arrayOfAddresses As System.Collections.IEnumerable, ByVal Address As String) As Boolean
        For Each _s As String In arrayOfAddresses
            If Address.StartsWith(_s) Then
                Return True
            End If
        Next
        Return False
    End Function

    Private Function IsMGTSNumberOK(ByVal Number As String, ByRef ContinueToCollect As Boolean) As Boolean
        Dim Found As Boolean = False
        ContinueToCollect = False
        For Each _s As Destination In MGTSAllowedDestinations
            If _s.Pattern.StartsWith(Number) Then
                Found = True
                If (_s.Length - Number.Length) > 0 Then
                    ContinueToCollect = True ' Get next new digit(s)
                    Exit For
                End If
            End If
            If Number.StartsWith(_s.Pattern) Then
                Found = True
                If (_s.Length - Number.Length) > 0 Then
                    ContinueToCollect = True ' Get next new digit
                Else
                    ContinueToCollect = False ' No more matches - all digits are collected
                    Exit For
                End If
            End If
        Next
        Return Found
    End Function

    ' Initialize IP destination array
    Sub InitializeIPDestinations()
        IPAllowedDestinations.Add(New Destination("2", 3, "2"))
        IPAllowedDestinations.Add(New Destination("3", 4, "38"))
        IPAllowedDestinations.Add(New Destination("4", 3, "4"))
        IPAllowedDestinations.Add(New Destination("5", 3, "5"))
        IPAllowedDestinations.Add(New Destination("6", 3, "69"))
        IPAllowedDestinations.Add(New Destination("7", 3, "7#,,,"))
    End Sub

    ' Initialize MGTS destination array
    Sub InitializeMGTSDestinations()
        MGTSAllowedDestinations.Add(New Destination("1", 7))
        MGTSAllowedDestinations.Add(New Destination("100", 3))
        MGTSAllowedDestinations.Add(New Destination("2", 7))
        MGTSAllowedDestinations.Add(New Destination("3", 7))
        MGTSAllowedDestinations.Add(New Destination("4", 7))
        MGTSAllowedDestinations.Add(New Destination("5", 7))
        MGTSAllowedDestinations.Add(New Destination("6", 7))
        MGTSAllowedDestinations.Add(New Destination("7", 7))
        MGTSAllowedDestinations.Add(New Destination("9", 7))
        MGTSAllowedDestinations.Add(New Destination("8499", 11))
        MGTSAllowedDestinations.Add(New Destination("8901", 11))
        MGTSAllowedDestinations.Add(New Destination("8903", 11))
        MGTSAllowedDestinations.Add(New Destination("8905", 11))
        MGTSAllowedDestinations.Add(New Destination("8916", 11))
        MGTSAllowedDestinations.Add(New Destination("8917", 11))
        MGTSAllowedDestinations.Add(New Destination("8926", 11))
        MGTSAllowedDestinations.Add(New Destination("8800", 11))
        MGTSAllowedDestinations.Add(New Destination("8095", 11))
        MGTSAllowedDestinations.Add(New Destination("8495", 11))
        MGTSAllowedDestinations.Add(New Destination("8254", 9))
    End Sub

    Sub Main()
        InitializeIPDestinations()
        InitializeMGTSDestinations()
    End Sub

    Sub Z()     ' Initial State for the Line
        Init()
    End Sub

    Sub ZS()     ' On any error - init the line
        Init()
    End Sub

    Sub ZX()     ' On any disconnect - init the line
        Init()
    End Sub

    Sub ZW()     ' On any timeout - init the line
        Init()
    End Sub


    Sub FreeLine()
        ResetAll()
        OnEvent("XYSTWZ", "Z")
        DropCall(2)
        WaitComplete()
    End Sub

    Sub ZJ()    ' Event RING "J" came from the line
        Timeout = AllowedTimeToUseSeconds
        OnEvent("XYSTW", "FreeLine")  ' On any error or disconnect event initialize the line        
        Var("Attempts") = 1     ' Initialize retries counter
        Var("ValidPassword") = False
    End Sub

    Sub ZJI()   ' Event CONNECT "I" came - now we are connected
        Timeout = AllowedTimeToUseSeconds
        Select Case AccessCode
            Case PBXPrefix      ' Call came from Local PBX line
                GotoState("FromTrunkPBX")
            Case MGTSPrefix     ' Call came from Moscow City POTS Line
                GotoState("FromTrunkMGTS")
            Case IPPrefix       ' Call came from Internet
                GotoState("FromTrunkIP")
            Case Else           ' Call came from IP or Local phone
                If IsAddressOK(AllowedMGTSNumbers, LineNumber) Then
                    GotoState("FromTrunkMGTS")
                Else
                    GotoState("InvalidTrunk")  ' No allowed IP address found - drop the call
                End If
        End Select
    End Sub

    Sub InvalidTrunk()      ' 
        Reset()
        OnEvent("XYSTW", "FreeLine")  ' On any error or disconnect event init the line
        PlayFile("202")     ' Play "Sorry that you are having problems" phrase
        DropCall(2)
    End Sub

    Sub LineIsBusy()      ' 
        Reset()
        If CInt(Var("Attempts")) < MaxNumberOfAttempts Then
            OnEvent("XYSTW", "FreeLine")  ' On any error or disconnect event init the line
            Var("Attempts") = CInt(Var("Attempts")) + 1
            PlayFile("210")     ' Play "Number you are calling is busy" phrase
            GotoState("ZJI")
        Else
            GotoState("InvalidTrunk")
        End If
    End Sub

    Sub InvalidNumber()     ' The number is incorrect - give user another chance to enter the number
        Reset()
        If CInt(Var("Attempts")) < MaxNumberOfAttempts Then
            OnEvent("XYSTW", "FreeLine")  ' On any error or disconnect event initialize the line
            Var("Attempts") = CInt(Var("Attempts")) + 1
            PlayFile("201") ' Play "Error in a number - please retry" phrase
            GotoState("ZJI")
        Else
            GotoState("InvalidTrunk")
        End If
    End Sub

    '-------------------------------------------------------------------------------------
    ' Call came from MGTS trunk - users can enter either local or IP number
    Sub FromTrunkMGTS()
        OnEvent("XYSTW", "FreeLine")  ' On any error or disconnect event initialize the line
        OnEvent(LocalNumbersStartWith, "LocalNumber")       ' If first digit is 0 - local call
        OnEvent(ExternalNumbersStartWith, "IPNumber")     ' If 1-8 - dial external number
        OnEvent(PBXNumbersStartWith, "TrunkMGTSExternalNumber")   ' If 9 - dial PBX number
        OnEvent(InvalidNumbersStartWith, "InvalidNumber")   ' On *# and any other - error
        PlayFile("207")   ' Play "Please enter Location number and then extension" phrase
    End Sub

    Sub TrunkMGTSExternalNumber()    ' If user pressed external access or 9 - check if password was OK
        Reset()
        If CBool(Var("ValidPassword")) = False Then
            CallStateFunction("Password.Check", "TrunkMGTSCheckPassword")
        Else
            GotoState("TrunkMGTSCheckPassword")
        End If
    End Sub

    Sub TrunkMGTSCheckPassword()    ' We get here after we received password
        Reset()
        If CBool(Var("ValidPassword")) = True Then
            OnEvent("XYSTW", "FreeLine")  ' On disconnect event init the line
            OnEvent(LocalNumbersStartWith, "LocalNumber")       ' If first digit is 0 - local call
            OnEvent(ExternalNumbersStartWith, "IPNumber")     ' If 1-8 - dial external number
            OnEvent(PBXNumbersStartWith, "TrunkMGTSGetPBX")
            OnEvent(InvalidNumbersStartWith, "InvalidNumber")   ' On *# and any other - error
            PlayFile("207")   ' Play "Please enter Location number and then extension" phrase
        Else
            GotoState("InvalidTrunk")
        End If
    End Sub

    Sub TrunkMGTSGetPBX()
        Reset()
        OnEvent("XYSTW", "FreeLine")  ' On disconnect event init the line
        OnEvent(PBXAllowedNumbersStartWith, "PBXNumber")
        OnEvent(MGTSAccessStartsWith, "InvalidNumber")    ' If 9 - dial MGTS  number
        OnEvent(InvalidNumbersStartWith, "InvalidNumber")   ' On 6-8 and any other - error
        PlayFile("205")   ' Play "Please enter PBX Number or 9 for Moscow number" phrase
    End Sub

    '-------------------------------------------------------------------------------------
    ' Call came from PBX - users can call IP trunks or local numbers
    Sub FromTrunkPBX()
        OnEvent("XYSTW", "FreeLine")  ' On any error or disconnect event initialize the line
        OnEvent(LocalNumbersStartWith, "LocalNumber")       ' If first digits 0 - local call
        OnEvent(ExternalNumbersStartWith, "IPNumber")       ' If 1-8 - dial external IP number
        OnEvent(PBXNumbersStartWith, "InvalidNumber")       ' If 9 - PBX
        OnEvent(InvalidNumbersStartWith, "InvalidNumber")   ' On *# and any other - error
        PlayFile("209")   ' Play "Please enter Location number and then extension" phrase
    End Sub

    '-------------------------------------------------------------------------------------
    ' Call came from IP - users can call Local, PBX or MGTS numbers
    Sub FromTrunkIP()
        ' We will allow to connect all users in "AllowedIPToConnect" and in "AllowedIPToAccessMGTS" + "AllowedIPToAccessPBX"
        Dim AllowedIP As New System.Collections.ArrayList
        AllowedIP.AddRange(AllowedIPToConnect)
        AllowedIP.AddRange(AllowedIPToAccessMGTS)
        AllowedIP.AddRange(AllowedIPToAccessPBX)
        AllowedIP.AddRange(AllowedMGTSNumbers)

        If IsAddressOK(AllowedIP, CIDNumber) Then
            OnEvent("XYSTW", "FreeLine")  ' On any error or disconnect event initialize the line
            OnEvent(LocalNumbersStartWith, "LocalNumber")       ' If first digit 0 - local call
            OnEvent(ExternalNumbersStartWith, "IPNumber")       ' If 1-8 - dial external IP number
            OnEvent(PBXNumbersStartWith, "PBXOrMGTSNumber")     ' If 9 - PBX or MGTS
            OnEvent(InvalidNumbersStartWith, "InvalidNumber")   ' On *# and any other - error
            PlayFile("207")   ' Play "Please enter Location number and then extension" phrase
        Else
            GotoState("InvalidTrunk")  ' No allowed IP address found - drop the call
        End If
    End Sub

    Sub PBXOrMGTSNumber()
        Reset()
        ' We will allow to connect all users in "AllowedIPToAccessMGTS" + "AllowedIPToAccessPBX"
        Dim AllowedIP As New System.Collections.ArrayList
        AllowedIP.AddRange(AllowedIPToAccessMGTS)
        AllowedIP.AddRange(AllowedIPToAccessPBX)
        AllowedIP.AddRange(AllowedMGTSNumbers)

        If IsAddressOK(AllowedIP, CIDNumber) Then
            OnEvent("XYSTW", "FreeLine")  ' On disconnect event init the line
            OnEvent(MGTSAccessStartsWith, "IPMGTSNumber")    ' If 9 - dial MGTS  number
            OnEvent(PBXAllowedNumbersStartWith, "PBXNumber")
            OnEvent(InvalidNumbersStartWith, "InvalidNumber")   ' On 6-8 and any other - error
            PlayFile("205")   ' Play "Please enter PBX Number or 9 for Moscow number" phrase
        Else
            GotoState("InvalidTrunk")  ' No allowed IP address found - drop the call
        End If
    End Sub

    Sub IPMGTSNumber()
        Dim AllowedIP As New System.Collections.ArrayList
        AllowedIP.AddRange(AllowedIPToAccessMGTS)
        AllowedIP.AddRange(AllowedMGTSNumbers)
        If IsAddressOK(AllowedIP, CIDNumber) Then
            GotoState("MGTSNumber")
        Else
            GotoState("InvalidTrunk")  ' No allowed IP address found - drop the call
        End If
    End Sub

    '------------------------------------------------------------------------------------
    ' Here we collect all digits of PBX Number
    Sub PBXNumber()
        Reset()     ' Clear all event handlers
        OnEvent("XYSTW", "FreeLine")  ' On disconnect event init the line
        Var("FirstDigit") = LastDigit
        CollectNumber("Num", PBXLength - 1) ' Get  more digits
    End Sub

    Sub PBXNumberZ()
        Reset()
        Dim DialNum As String = CStr(Var("FirstDigit")) + CStr(Var("Num"))
        If DialNum.Length = PBXLength Then
            Timeout = 0
            OnEvent("XYT", "FreeLine")  ' On disconnect event init the line
            OnEvent("SW", "LineIsBusy")  ' On error event 
            Transfer(PBXPrefix + DialNum)
            WaitComplete()
            If AccessCode = MGTSPrefix Then
                OnEvent("Z", "ConnectedWithTimeLimit")
            Else
                OnEvent("Z", "Connected")
            End If
        Else
            GotoState("InvalidNumber")
        End If
    End Sub


    '------------------------------------------------------------------------------------
    ' Here we collect all digits of Local Number
    Sub LocalNumber()
        Reset()     ' Clear all event handlers
        OnEvent("XYSTW", "FreeLine")  ' On disconnect event init the line
        CollectNumber("Num", LocalLength) ' Get  more digits
    End Sub

    Sub LocalNumberZ()
        Reset()
        Dim LocalDialNum As String = CStr(Var("Num"))
        If LocalDialNum.Length = LocalLength Then
            ' This part will take care of the AltiGen number call bug
            ' If number starts with 0 - that number should be dialled thru PBX trunk
            If LocalDialNum.StartsWith("0") Then
                LocalDialNum = PBXPrefix + LocalDialNum
            End If
            Timeout = 0
            OnEvent("XYT", "FreeLine")  ' On disconnect event init the line
            OnEvent("SW", "LineIsBusy")  ' On error event 
            Transfer(LocalDialNum)
            WaitComplete()
            If AccessCode = MGTSPrefix Then
                OnEvent("Z", "ConnectedWithTimeLimit")
            Else
                OnEvent("Z", "Connected")
            End If
        Else
            GotoState("InvalidNumber")
        End If
    End Sub

    '------------------------------------------------------------------------------------
    ' Here we collect Location ID and digits of IP destination number
    Sub IPNumber()
        Reset()
        Var("FirstDigit") = LastDigit
        OnEvent("XYSTW", "FreeLine")  ' On disconnect event init the line
        GotoState("DialIDLoc")
    End Sub

    Sub DialIDLoc()
        If (IDLocLength > 1) Then
            CollectNumber("LocID", IDLocLength - 1) ' Get digits
        Else
            Var("LocID") = ""
            GotoState("DialIDLocZ")
        End If
    End Sub

    Sub DialIDLocZ()
        Reset()
        Dim IPLocID As String = CStr(Var("FirstDigit")) + CStr(Var("LocID"))
        If IPLocID.Length = IDLocLength Then
            For Each _s As Destination In IPAllowedDestinations
                If IPLocID.StartsWith(_s.Pattern) Then
                    Var("LocID") = _s.PrefixDigits ' Use prefix digits
                    Var("NumDigits") = _s.Length
                    GotoState("DialIPNumber")
                    Exit Sub
                End If
            Next
        End If
        GotoState("InvalidNumber")
    End Sub

    Sub DialIPNumber()
        Dim nDigits As Integer = CInt(Var("NumDigits"))
        OnEvent("XYT", "FreeLine")  ' On disconnect event init the line
        OnEvent("SW", "InvalidNumber")  ' On error event 
        If nDigits = 0 Then
            Var("Num") = ""
            GotoState("DialIPNumberZ")
        Else
            CollectNumber("Num", nDigits)  ' Get digits
        End If
    End Sub

    Sub DialIPNumberZ()
        Reset()
        Dim IPDialNum As String = CStr(Var("LocID")) + CStr(Var("Num"))
        If CStr(Var("Num")).Length = Var("NumDigits") Then
            ' Here we have to process a special case - the "#" and ","
            ' All digits, that are located AFTER "#" should be sent in DTMF mode
            Dim poundPosition As Integer = IPDialNum.IndexOf("#")
            If poundPosition <> -1 Then
                Var("InBandDial") = IPDialNum.Substring(poundPosition + 1)
                IPDialNum = IPDialNum.Substring(0, poundPosition)
            Else
                Var("InBandDial") = ""
            End If
            ' In IPDialNum there is no need for ","
            IPDialNum = IPDialNum.Replace(",", String.Empty)
            OnEvent("XYT", "FreeLine")  ' On disconnect event init the line
            OnEvent("SW", "LineIsBusy")  ' On error event 
            Timeout = 0
            Transfer(IPPrefix + IPDialNum)
            WaitComplete()
            If AccessCode = MGTSPrefix Then
                OnEvent("Z", "ConnectedWithTimeLimit")
            Else
                OnEvent("Z", "Connected")
            End If
            Exit Sub
        Else
            GotoState("InvalidNumber")
        End If
    End Sub

    '------------------------------------------------------------------------------------
    ' Here we collect all digits of MGTS number and dial it out
    Sub MGTSNumber()
        Reset()
        OnEvent("XYT", "FreeLine")  ' On disconnect event init the line
        OnEvent("SW", "InvalidNumber")  ' On error event 
        GotoState("DialMGTSNumber")
    End Sub

    Sub DialMGTSNumber()
        Var("MGTSDialNum") = ""
        PlayFile("208")   ' Play "Please enter Outside number" phrase
        CollectNumber("Num", 1)  ' Get digits
    End Sub

    Sub DialMGTSNumberZ()
        Dim MGTSDialNum As String = CStr(Var("MGTSDialNum")) + CStr(Var("Num"))
        Dim extraDigitsToReceive As Boolean = False

        If IsMGTSNumberOK(MGTSDialNum, extraDigitsToReceive) Then
            Var("MGTSDialNum") = MGTSDialNum
            If extraDigitsToReceive = True Then    ' We need to collect more digits
                CollectNumber("Num", 1)  ' Get digits
                ReturnState()
            Else
                GotoState("DialMGTSNumberReady")
            End If
        Else
            GotoState("InvalidNumber")
        End If
    End Sub

    Sub DialMGTSNumberReady()
        Dim MGTSDialNum As String = CStr(Var("MGTSDialNum"))
        Reset()
        OnEvent("XYT", "FreeLine")  ' On disconnect event init the line
        OnEvent("SW", "LineIsBusy")  ' On error event 
        Timeout = 0
        Transfer(MGTSPrefix + MGTSDialNum)
        WaitComplete()
        OnEvent("Z", "ConnectedWithTimeLimit")
    End Sub

    '
    ' The state we are in if we have timeout limit
    '
    Const MGTSTalkLimitInMinutes As Integer = 10

    Sub Connected()
        Reset()
        OnEvent("SXYW", "FreeLine")  ' On disconnect or error event init the line
        CallStateFunction("ConnectAndPlayDTMF.Start", "ConnectedZ")
    End Sub

    Sub ConnectedZ()
        Reset()
        OnEvent("SXYW", "FreeLine")  ' On disconnect event init the line
    End Sub

    Sub ConnectedWithTimeLimit()
        Reset()
        OnEvent("SXYW", "FreeLine")  ' On disconnect or error event init the line
        CallStateFunction("ConnectAndPlayDTMF.Start", "ConnectedWithTimeLimitZ")
    End Sub

    Sub ConnectedWithTimeLimitZ()
        Reset()
        OnEvent("SXYW", "FreeLine")  ' On disconnect event init the line
        Timeout = (MGTSTalkLimitInMinutes * 60) - 60    ' Warn people 1 minute before disconnection
        OnEvent("T", "ConnectedWithTimeLimitTimeOut")
    End Sub

    Sub ConnectedWithTimeLimitTimeOut()
        Reset()
        OnEvent("SXYW", "FreeLine")  ' On disconnect event init the line
        Disconnect()
        WaitComplete(2)
    End Sub

    Sub ConnectedWithTimeLimitTimeOutZ()
        Timeout = 60        ' Give people 1 more minute (60 seconds) to finish conversation
        OnEvent("SXYW", "FreeLine")  ' On disconnect event init the line
        OnEvent("T", "DropLine.Start") ' On Timeout - drop line completely
        LinkPlayFile("211")   ' Play "Sorry, you have 1 minute to finish " phrase
        PlayFile("211")   ' Play "Sorry, you have 1 minute to finish" phrase
        Connect()
    End Sub
End Class

''' <summary>
''' Class to play DTMF digits from Var("InBandDial") after the connection.
''' </summary>
Public Class ConnectAndPlayDTMF : Inherits Diacom.APCLineControl
    Sub Start()
        Dim InBandDial As String = Var("InBandDial")
        If InBandDial Is Nothing OrElse InBandDial = "" Then
            ReturnStateFunction()
        Else
            Disconnect()
            WaitComplete(5)
        End If
    End Sub

    Sub StartZ()
        Dim DialDigits As String = Var("InBandDial")
        For Each OneDigit As String In DialDigits
            LinkPlayDTMF(OneDigit)
            PlayDTMF(OneDigit)
        Next
        Connect()
        WaitComplete(5)
    End Sub

    Sub StartZZ()
        ReturnStateFunction()
    End Sub
End Class

''' <summary>
''' Class to fully disconnect the line (including connected lines).
''' </summary>
Public Class DropLine : Inherits Diacom.APCLineControl
    Sub Start()
        Reset()
        Timeout = 0
        GotoState("DropLines")
    End Sub
    Sub DropLines()
        Disconnect()
        WaitComplete(2)
        Timeout = 5
    End Sub

    Sub DropLinesZ()
        LinkDropCall(2)
        DropCall(2)
    End Sub

    Sub DropLinesY()
        DropCall(2)
    End Sub

    Sub DropLinesX()
        LinkDropCall(2)
    End Sub

    Sub DropLinesS()
        Init()
    End Sub

    Sub DropLinesW()
        Init()
    End Sub

    Sub DropLinesT()
        Init()
    End Sub
End Class
'''---------------------------------------------------------------------------------------
''' Class	 : Password
''' <summary>
''' Represents the class that asks for the password from the user.
''' </summary>
''' -----------------------------------------------------------------------------
Public Class Password : Inherits Diacom.APCLineControl
    ReadOnly AccessPassword As String = "142143"
    ReadOnly MaxNumberOfAttempts As Integer = 3

    Sub Check()
        Reset()
        Var("ValidPassword") = False
        Var("PassAttempts") = 1
        GotoState("EnterPass")
    End Sub

    Sub EnterPass()
        PlayFile("203")   ' Play "Please enter the password" phrase
        CollectNumber("Pass", AccessPassword.Length)
    End Sub

    Sub EnterPassZ()
        Reset()
        If CStr(Var("Pass")).Equals(AccessPassword) Then
            Var("ValidPassword") = True
            ReturnStateFunction()
        ElseIf CInt(Var("PassAttempts")) < MaxNumberOfAttempts Then
            Var("PassAttempts") = CInt(Var("PassAttempts")) + 1
            PlayFile("204")   ' Play "Invalid password" phrase
            GotoState("EnterPass")
        Else
            ReturnStateFunction()
        End If
    End Sub

    Sub EnterPassX()
        Init()
    End Sub

    Sub EnterPassS()
        Init()
    End Sub
End Class
