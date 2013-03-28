Option Explicit On 
Option Strict On

Imports System.Threading
Imports System.Runtime.InteropServices

Namespace Diacom.APCStates
    '''<summary>
    ''' Class to initiate the Play DTMF on the line command.
    '''</summary>
    Friend Class PlayDTMFModule : Inherits EventProcessingModule
        Private m_DTMFString As String

        Public Sub New(ByVal srcLine As APCStateLine, ByVal oLine As APCStateLine, ByVal DTMFString As String, ByVal TimeoutVal As Integer)
            MyBase.New(srcLine, oLine, TimeoutVal)
            m_DTMFString = DTMFString.Trim.ToUpper
        End Sub

        Public Overrides Sub StartOperation()
            DBGLN(m_Line, "Module: PlayDTMF: Start: " & m_DTMFString)
            m_Line.spCommand.SendPlayDTMFCommand(m_SourceLine.LineReference, m_Line.LineReference, m_DTMFString)
            m_TimerError = m_TimeoutError
            m_Active = True
        End Sub
        Public Overrides Function ToString() As String
            Return "Module: PlayDTMF:" & " DTMF: " & m_DTMFString
        End Function
    End Class
End Namespace
