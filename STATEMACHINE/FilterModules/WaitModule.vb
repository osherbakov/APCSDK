Option Explicit On 
Option Strict On

Imports System.Threading

Namespace Diacom.APCStates
    Friend Class WaitDigitModule : Inherits EventProcessingModule

        Public Sub New(ByVal srcLine As APCStateLine, ByVal oLine As APCStateLine, ByVal TimeoutVal As Integer)
            MyBase.New(srcLine, oLine, TimeoutVal)
            m_Active = True
            m_TimerError = m_TimeoutError
        End Sub

        Public Overrides Sub StartOperation()
            DBGLN(m_Line, "Module: WaitDigit: Start")
        End Sub

        Public Overrides Function ProcessEvent(ByVal EventType As APCEvents, ByVal EventData As Object) As Boolean
            DBGLN(m_Line, "Module: WaitDigit: Process")
            If EventType = APCEvents.DIGIT Then
                ProcessEvent = True
                FinishOperation()
            End If
        End Function
        Public Overrides Function ToString() As String
            Return "Module: WaitDigit:"
        End Function
    End Class


    Friend Class WaitCompleteModule : Inherits EventProcessingModule

        Public Sub New(ByVal srcLine As APCStateLine, ByVal oLine As APCStateLine, ByVal TimeoutVal As Integer)
            MyBase.New(srcLine, oLine, TimeoutVal)
            m_TimerError = m_TimeoutError
            m_Active = True
        End Sub

        Public Overrides Sub StartOperation()
            DBGLN(m_Line, "Module: WaitComplete: Start")
            FinishOperation()
            m_Line.FireStateEvent(m_Line.EventLine, APCEvents.THENCONTINUE, Nothing, True)
        End Sub
        Public Overrides Function ToString() As String
            Return "Module: WaitComplete:"
        End Function

    End Class



    Friend Class WaitTimerModule : Inherits EventProcessingModule
        Private m_Timer As Integer

        Public Sub New(ByVal srcLine As APCStateLine, ByVal oLine As APCStateLine, ByVal TimeoutVal As Integer)
            MyBase.New(srcLine, oLine, 0)    ' No Error Timeout
            m_Timer = TimeoutVal
        End Sub

        Public Overrides Sub StartOperation()
            DBGLN(m_Line, "Module: WaitTime: Start")
            m_Active = True
        End Sub

        Public Overrides Sub TimerEvent()
            If (m_Active = False) Then Exit Sub
            ' Check for the Timeout
            If (m_Timer > 0) Then
                m_Timer -= 1
            End If
            If (m_Timer = 0) Then
                ' Timeout - unload
                FinishOperation()
            End If
        End Sub
        Public Overrides Function ToString() As String
            Return "Module: WaitTimer:"
        End Function

    End Class
End Namespace
