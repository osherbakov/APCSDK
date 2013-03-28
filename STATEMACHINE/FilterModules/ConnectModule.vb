Option Explicit On 
Option Strict On

Imports System.Threading
Imports System.Runtime.InteropServices

Namespace Diacom.APCStates
    '''<summary>
    ''' Class to initiate the connect lines command.
    '''</summary>
    Friend Class ConnectModule : Inherits EventProcessingModule
        Private m_DestLinePad As APCStateLine

        Public Sub New(ByVal srcLine As APCStateLine, ByVal oLine As APCStateLine, ByVal LineConnectTo As APCStateLine, ByVal TimeoutVal As Integer)
            MyBase.New(srcLine, oLine, TimeoutVal)
            m_DestLinePad = LineConnectTo
        End Sub

        Public Overrides Sub StartOperation()
            DBGLN(m_Line, "Module: Connect: Start")
            m_Line.spCommand.SendConnectCommand(m_SourceLine.LineReference, m_Line.LineReference, m_DestLinePad.LineReference)
            m_TimerError = m_TimeoutError
            m_Active = True
        End Sub
        Public Overrides Function ToString() As String
            Return "Module: Connect:" & " To: " & m_DestLinePad.LineID.ToString()
        End Function
    End Class
    '''<summary>
    ''' Class to initiate disconnect lines command.
    '''</summary>
    Friend Class DisconnectModule : Inherits EventProcessingModule

        Public Sub New(ByVal srcLine As APCStateLine, ByVal oLine As APCStateLine, ByVal TimeoutVal As Integer)
            MyBase.New(srcLine, oLine, TimeoutVal)
        End Sub

        Public Overrides Sub StartOperation()
            DBGLN(m_Line, "Module: Disconnect: Start")
            m_Line.spCommand.SendDisconnectCommand(m_SourceLine.LineReference, m_Line.LineReference)
            m_Active = True
        End Sub
        Public Overrides Function ToString() As String
            Return "Module: Disconnect:"
        End Function
    End Class

End Namespace
