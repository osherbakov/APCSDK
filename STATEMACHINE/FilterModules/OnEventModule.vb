Option Explicit On 
Option Strict On

Namespace Diacom.APCStates

    Friend Class OnEventModule : Inherits EventProcessingModule

        Private m_ArrayOfEvents As Collections.ArrayList
        Private m_eventHandler As Boolean
        Private m_origString As String
        Private m_DTMFDigits As String
        Private m_ExecuteData As ScriptCallInfo

        Public Sub New(ByVal srcLine As APCStateLine, ByVal oLine As APCStateLine, ByVal eventCallFunction As ScriptCallInfo, _
                        ByVal eventArray As Collections.ArrayList, ByVal originalString As String, ByVal eventDigits As String, ByVal eventHandler As Boolean, ByVal TimeoutVal As Integer)
            MyBase.New(srcLine, oLine, TimeoutVal)
            m_ExecuteData = eventCallFunction
            m_origString = originalString
            m_DTMFDigits = eventDigits
            m_ArrayOfEvents = eventArray
            m_eventHandler = eventHandler
            m_TimerError = m_TimeoutError
            m_Active = True
        End Sub

        Public Overrides Sub StartOperation()
        End Sub

        Public Overloads Function ProcessEvent(ByVal sourceLine As APCStateLine, ByVal EventType As APCEvents, ByVal EventData As Object) As Boolean
            DBGLN(m_Line, "Events Handler: OnEvent: Process: " + m_origString)
            ProcessEvent = False ' By default we don't process this event, so other modules can do it
            If (m_Active = False) Then Exit Function
            If m_ArrayOfEvents.Contains(EventType) Then
                If (EventType = APCEvents.DIGIT AndAlso m_DTMFDigits.IndexOf(CStr(EventData)) = -1) Then
                    Exit Function
                End If
                ProcessEvent = True
                DBGLN(m_Line, "Events Handler: OnEvent: Found - Execute:" & m_ExecuteData.CallName)
                sourceLine.FireStateEvent(m_Line.EventLine, APCEvents.EXECUTE, m_ExecuteData, False)
                If Not m_eventHandler Then FinishOperation()
            End If
        End Function
        Public Overrides Function ToString() As String
            Return "Events Handler:" & " Events: " & m_origString & " Func: " & m_ExecuteData.CallName
        End Function
    End Class
End Namespace
