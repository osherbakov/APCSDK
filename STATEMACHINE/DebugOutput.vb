Imports System.Threading

Namespace Diacom
    Module DebugOutput
        Sub DBG(ByVal debugText As String)
            TraceOut.Put(debugText)
        End Sub
        Sub DBGEX(ByVal _e As Exception)
            TraceOut.Put(_e)
        End Sub
        Sub DBGLN(ByVal oLine As APCLine, ByVal sDbg As String)
            TraceOut.Put("Line : " & oLine.LineID.ToString() & " : " & sDbg)
        End Sub
    End Module

End Namespace
