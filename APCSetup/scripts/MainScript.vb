class testcall : inherits Diacom.APCLineControl

	dim SQLTest as string = "data source=SERVER\Diacom;initial catalog=Pubs;integrated security=True;persist security info=True;workstation id=Hopper;packet size=4096"
	dim SQLQueryAuthorNumber as string = "SELECT COUNT(*) FROM Authors"
	dim SQLQueryAuthors as string = "SELECT * FROM Authors"


    function TestAsync(l as Diacom.APCLine) as string
'		SubTest("dfsdsdfsdfsdfsdfsdfsdfsdfdsfsdfsdfs " & l.LineAccessCode)
        return "dfsdsdfsdfsdfsdfsdfsdfsdfdsfsdfsdfs " & l.LineAccessCode
    end function

	sub Z()
		SharedSubTest("AAAA")
	end sub

	sub ZJ()
        OnEvent("Q", "PrintAccessCode")
        AsyncExecute("zzz", "TestAsync", CurrentLine)
		ReturnState()
	end sub

    sub PrintAccessCode()
		if(Var("ZZZ") <> Nothing) then SubTest(Var("ZZZ"))
		ReturnState()
    end sub

	sub ZX()
		SubTest("BBBBBBBBB")
		Init()
	end sub

	sub InitLine()
		SubTest("BBBBBBBBB")
		Init()
	end sub

    sub DataReady()
		if(Var("Number") <> Nothing) then SubTest(Var("Number"))
		ReturnState()
    end sub

	sub CallOleg()
		Dial("95185628")
		ReturnState()
	end sub

    sub LineReadyForAPC
        AcceptGrantAsMaster()
        Reset()
        OnEvent("I", "LineReadyForConnect")
    end sub

    sub LineReadyForConnect
        Reset()
        Connect()
        OnEvent("X", "DropLinkNow")
        OnEvent("Y", "DropMainNow")
    end sub

    sub DropLinkNow
        LinkReset()
        LinkRelease()
        LinkReturnState()
        Init()
    end sub

    sub DropMainNow
        Reset()
        Release()
        LinkReturnState()
		GotoState("Z")
    end sub

	sub zi()
        PlayNumber(Now(), "%date")
		PlayFile("C:\qwe")
		PlayFile("C:\q")
		SQLGetValue("Number", SQLTest, SQLQueryAuthorNumber)
		SQLGetRecord("Authors", SQLTest, SQLQueryAuthors)
        OnEventHandler("Q", "DataReady")
        OnEvent("5", "CallOleg")
        OnEvent("G", "LineReadyForAPC")
        OnEvent("X", "InitLine")
		WaitComplete()
	end sub

	sub ZIz()
		GotoState("ZI")
	end sub

	sub ZIP()
		Reset()
		GotoState("ZI")
        OnEvent("890", "zi1")
	end sub

	sub ZIR()
		GotoState("ZI")
	end sub

	sub ZIq()
		if(Var("Number") <> Nothing) then SubTest(Var("Number"))
		ReturnState()
	end sub

	sub ZI1()
		Reset()
		DropCall()
		ReturnState()
	end sub

	sub ZI2()
		Reset()
		RecordFile("C:\zxcv")
		ReturnState()
	end sub

	sub ZI3()
		Reset()
		PlayFile("C:\zxcv")
		ReturnState()
	end sub

	sub ZI4()
		Reset()
		Dial("215")
		ReturnState()
	end sub

	sub ZI5()
		Reset()
		Dial("95185628")
	end sub

	sub ZI6()
		Reset()
		Transfer("215")
		ReturnState()
	end sub

	sub ZI7()
		Reset()
		Transfer("VM215")
		ReturnState()
	end sub

	sub ZI8()
		Reset()
		Transfer("VM")
		ReturnState()
	end sub

	sub ZI9()
		Reset()
		Transfer("95185628")
		ReturnState()
	end sub

	sub ZI0()
		Reset()
		Transfer("OP")
		ReturnState()
	end sub

	sub ZV()
        AcceptLinkAsSlave()
	end sub

	sub ZVS()
        Reset()
        DropCall()
		ReturnState()
	end sub

	sub ZVU()
        Reset()
        DropCall()
		ReturnState()
	end sub
    
end class