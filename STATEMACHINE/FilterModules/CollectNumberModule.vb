Option Explicit On 
Option Strict On

Namespace Diacom.APCStates

    Friend Class CollectNumberModule : Inherits EventProcessingModule

        Private m_Variable As String
        Private m_MaxDigits As Integer
        Private m_Number As String
        Private m_Cutoff As String

        Private m_TimeoutInitial As Integer
        Private m_TimeoutInterDigit As Integer

        Private m_Timer As Integer

        Public Sub New(ByVal sourceLine As APCStateLine, ByVal targetLine As APCStateLine, ByVal NumberVar As String, ByVal MaxDigits As Integer, ByVal CutoffString As String, ByVal InitTimeoutVal As Integer, ByVal DigitTimeoutVal As Integer, ByVal TimeoutVal As Integer)
            MyBase.New(sourceLine, targetLine, TimeoutVal)
            m_Variable = NumberVar
            m_MaxDigits = MaxDigits
            m_Number = ""
            m_Cutoff = CutoffString.Trim.ToUpper
            m_TimeoutInitial = InitTimeoutVal
            m_TimeoutInterDigit = DigitTimeoutVal
            m_Timer = 0
            m_Active = True
        End Sub

        Public Overrides Sub StartOperation()
            DBGLN(m_Line, "Module: CollectNumber: Start: " & m_Variable)
            m_Timer = m_TimeoutInitial
            m_TimerError = m_TimeoutError
        End Sub

        Public Overrides Function ProcessEvent(ByVal EventType As APCEvents, ByVal EventData As Object) As Boolean
            DBGLN(m_Line, "Module: CollectNumber: ProcessEvent: Var: " & m_Variable & " Num: " & m_Number)
            ProcessEvent = False ' By default we don't process this event, so other modules can do it

            If (m_Active = False) Then Exit Function
            Select Case EventType
                Case APCEvents.DIGIT
                    If Not EventData Is Nothing Then
                        Dim _digit As String = CStr(EventData).Substring(0, 1)

                        ' Check if digit that just arrived with this event is a cutoff digit
                        If (m_Cutoff.Equals("ALL") OrElse m_Cutoff.Equals("ANY") OrElse _
                            (m_Cutoff.IndexOf(_digit) <> -1)) Then
                            ' Cutoff digit - save the number and report that we are done
                            StoreVariable(m_Variable, m_Number)
                            FinishOperation()
                            m_Line.FireStateEvent(m_Line.EventLine, APCEvents.THENCONTINUE, Nothing, True)
                        Else
                            ' not cutoff digit - add to the collected string and reset the timer
                            m_Number = m_Number & _digit
                            m_Timer = m_TimeoutInterDigit
                            ProcessEvent = True ' Tell the channel filter that we consumed the digit

                            ' Is maximum number of digits reached ?
                            If (Len(m_Number) >= m_MaxDigits) Then
                                StoreVariable(m_Variable, m_Number)
                                FinishOperation()
                                m_Line.FireStateEvent(m_Line.EventLine, APCEvents.THENCONTINUE, Nothing, True)
                            End If
                        End If
                    End If
            End Select
        End Function

        Public Overrides Sub TimerEvent()
            If (m_Timer > 0) Then
                m_Timer -= 1
                If (m_Timer = 0) Then
                    StoreVariable(m_Variable, m_Number)
                    FinishOperation()
                    m_Line.FireStateEvent(m_Line.EventLine, APCEvents.THENCONTINUE, Nothing, True)
                End If
            End If

            MyBase.TimerEvent()
        End Sub

        Public Overrides Function ToString() As String
            Return "Module: CollectNumber:" & " Var: " & m_Variable & " Num: " & m_Number
        End Function

    End Class

End Namespace
