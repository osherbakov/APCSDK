Option Explicit On 
Option Strict On

Imports System.Threading
Imports System.Runtime.InteropServices

Namespace Diacom.APCStates

    '''<summary>
    ''' Class to initiate the Play File on the line command.
    '''</summary>
    Friend Class PlayFileModule : Inherits EventProcessingModule
        Private m_FileName As String
        Private m_Cutoff As String
        Private m_StopSent As Boolean

        Public Sub New(ByVal srcLine As APCStateLine, ByVal oLine As APCStateLine, ByVal FileName As String, ByVal CutOff As String, ByVal TimeoutVal As Integer)
            MyBase.New(srcLine, oLine, TimeoutVal)
            m_Cutoff = CutOff.Trim.ToUpper
            m_FileName = FileName.Trim.ToUpper
            m_StopSent = False
        End Sub

        Public Overrides Sub StartOperation()
            DBGLN(m_Line, "Module: PlayFile: Start: " & m_FileName)
            m_Line.spCommand.SendPlayFileCommand(m_SourceLine.LineReference, m_Line.LineReference, m_FileName, m_Cutoff)
            m_TimerError = m_TimeoutError
            m_Active = True
        End Sub

        Public Overrides Sub FinishOperation()
            DBGLN(m_Line, "Module: PlayFile: Finish: " & m_FileName)
            If m_Active = False Then
                MyBase.FinishOperation()
            ElseIf m_StopSent = False Then
                DBGLN(m_Line, "Module: PlayFile: Finish: Send Reset")
                m_Line.spCommand.SendResetCommand(m_SourceLine.LineReference, m_Line.LineReference)
                m_StopSent = True
            End If
        End Sub
        Public Overrides Function ToString() As String
            Return "Module: PlayFile:" & " File: " & m_FileName
        End Function
    End Class
End Namespace
