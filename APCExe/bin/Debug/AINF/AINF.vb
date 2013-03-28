    Class AINF : Inherits Diacom.APCLineControl
        Sub Z()
            Init()
        End Sub

        Sub ZX()
            Init()
        End Sub

        Sub ZI()
            PlayFile("2")
            CollectNumber("Num", 4)
            WaitComplete()
        End Sub

        Sub ZIZ()
            Dim Num As String = CStr(Var("Num"))
            If ((Num.Length <> 4) OrElse (Num.Chars(0) > "6")) Then
                GotoState("CallOperator")
            Else
                GotoState("CallNumber")
            End If
        End Sub

        Sub CallOperator()
            OnEvent("SX", "ZIS")
            PlayDTMF("BA5501")
            DropCall()
        End Sub

        Sub CallNumber()
            OnEvent("SX", "ZIS")
            PlayDTMF("BA" + CStr(Var("Num")))
            DropCall()
        End Sub

        Sub ZIS()
	    Clear()
            DropCall()
        End Sub

        Sub ZISS()
            Init()
        End Sub
    End Class
