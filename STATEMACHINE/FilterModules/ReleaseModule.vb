Option Explicit On 
Option Strict On

Imports System.Threading
Imports System.Runtime.InteropServices

Namespace Diacom.APCStates

    '''<summary>
    ''' Class to initiate the disconnect (drop call)line command.
    '''</summary>
    Friend Class ReleaseModule : Inherits EventProcessingModule

        Public Sub New(ByVal srcLine As APCStateLine, ByVal oLine As APCStateLine, ByVal TimeoutVal As Integer)
            MyBase.New(srcLine, oLine, TimeoutVal)
        End Sub

        Public Overrides Sub StartOperation()
            DBGLN(m_Line, "Module: Release: Start")
            m_Line.spCommand.SendReleaseCommand(m_SourceLine.LineReference, m_Line.LineReference)
            m_TimerError = m_TimeoutError
            m_Active = True
        End Sub
        Public Overrides Function ToString() As String
            Return "Module: Release:"
        End Function
    End Class

    '''<summary>
    ''' Class to initiate the answer call command.
    '''</summary>
    Friend Class AnswerModule : Inherits EventProcessingModule

        Public Sub New(ByVal srcLine As APCStateLine, ByVal oLine As APCStateLine, ByVal TimeoutVal As Integer)
            MyBase.New(srcLine, oLine, TimeoutVal)
        End Sub

        Public Overrides Sub StartOperation()
            DBGLN(m_Line, "Module: Answer: Start")
            m_Line.spCommand.SendAnswerCommand(m_SourceLine.LineReference, m_Line.LineReference)
            m_TimerError = m_TimeoutError
            m_Active = True
        End Sub
        Public Overrides Function ToString() As String
            Return "Module: Answer:"
        End Function
    End Class

    '''<summary>
    ''' Class to initiate the reject call command.
    '''</summary>
    Friend Class RejectModule : Inherits EventProcessingModule
        Private m_ReasonCode As Integer

        Public Sub New(ByVal srcLine As APCStateLine, ByVal oLine As APCStateLine, ByVal rejectReason As Integer, ByVal TimeoutVal As Integer)
            MyBase.New(srcLine, oLine, TimeoutVal)
            m_ReasonCode = rejectReason
        End Sub

        Public Overrides Sub StartOperation()
            DBGLN(m_Line, "Module: Reject: Start")
            m_Line.spCommand.SendRejectCommand(m_SourceLine.LineReference, m_Line.LineReference, m_ReasonCode)
            m_TimerError = m_TimeoutError
            m_Active = True
        End Sub
        Public Overrides Function ToString() As String
            Return "Module: Reject:"
        End Function
    End Class

    '''<summary>
    ''' Class to initiate the pass call command.
    '''</summary>
    Friend Class PassModule : Inherits EventProcessingModule

        Public Sub New(ByVal srcLine As APCStateLine, ByVal oLine As APCStateLine, ByVal TimeoutVal As Integer)
            MyBase.New(srcLine, oLine, TimeoutVal)
        End Sub

        Public Overrides Sub StartOperation()
            DBGLN(m_Line, "Module: Pass: Start")
            m_Line.spCommand.SendPassCommand(m_SourceLine.LineReference, m_Line.LineReference)
            m_TimerError = m_TimeoutError
            m_Active = True
        End Sub
        Public Overrides Function ToString() As String
            Return "Module: Pass:"
        End Function
    End Class

End Namespace