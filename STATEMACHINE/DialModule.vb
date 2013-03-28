Option Explicit On 
Option Strict On

Namespace Diacom.APCStates

    Friend MustInherit Class DialModule : Inherits EventProcessingModule
        Protected m_DialNumber As String
        Protected m_TargetNumber As String
        Protected m_OrigNumber As String

        Public Sub New(ByVal srcLine As APCStateLine, ByVal oLine As APCStateLine, ByVal NumberToDial As String, ByVal CallerNumber As String, ByVal TimeoutVal As Integer)
            MyBase.New(srcLine, oLine, TimeoutVal)
            m_DialNumber = NumberToDial.Trim.ToUpper
            If CallerNumber Is Nothing Then CallerNumber = String.Empty
            m_OrigNumber = CallerNumber
        End Sub

        Public Overloads Function ProcessEvent(ByVal oLine As APCLine) As Boolean
            ProcessEvent = False
            If ((oLine.CIDNumber Is Nothing OrElse oLine.CIDNumber.Equals(m_OrigNumber)) AndAlso _
                    oLine.CallNumber.Equals(m_TargetNumber)) Then
                DBGLN(m_Line, "Found Dialed call :" & m_TargetNumber & " Src: " & m_SourceLine.LineID.ToString())
                MyBase.FinishOperation()
                ProcessEvent = True
            End If
        End Function

        Public Shadows Sub FinishOperation(ByVal oLine As APCStateLine)
            If (oLine Is m_SourceLine) Then
                MyBase.FinishOperation()
            End If
        End Sub

        Public ReadOnly Property TargetLine() As APCStateLine
            Get
                Return Me.m_Line
            End Get
        End Property
        Public ReadOnly Property OriginalLine() As APCStateLine
            Get
                Return Me.m_SourceLine
            End Get
        End Property

        Public Overrides Function ToString() As String
            Return "Module: Dial: " + m_Line.LineID.ToString() & " DialNumber: " & m_DialNumber & " TargetNumber: " & m_TargetNumber & " OrigNumber: " & m_OrigNumber
        End Function

    End Class

    Friend Class MakeCallModule : Inherits DialModule
        Public Sub New(ByVal srcLine As APCStateLine, ByVal oLine As APCStateLine, ByVal NumberToDial As String, ByVal CallerNumber As String, ByVal TimeoutVal As Integer)
            MyBase.New(srcLine, oLine, NumberToDial, CallerNumber, TimeoutVal)
            If (m_Line.LineType = "T") Then
                m_TargetNumber = oLine.LineAccessCode & m_DialNumber
            Else
                m_TargetNumber = oLine.LineAccessCode
            End If
        End Sub

        Public Overrides Sub StartOperation()
            If (m_Line.LineType = "T") Then
                m_Line.spCommand.SendDialExternalCommand(m_Line.LineReference, m_Line.LineReference, m_DialNumber, m_OrigNumber, "")
            Else
                m_Line.spCommand.SendDialInternalCommand(m_Line.LineReference, m_Line.LineReference, m_DialNumber)
            End If
            m_TimerError = m_TimeoutError
            m_Active = True
        End Sub
    End Class

    Friend Class TransferCallModule : Inherits DialModule
        Private m_TransferTargetType As Diacom.Cmd.TransferCallType
        Private m_TransferTargetLine As APCStateLine

        Public Sub New(ByVal srcLine As APCStateLine, ByVal oLine As APCStateLine, ByVal transferTargetLine As APCStateLine, ByVal transferType As Diacom.Cmd.TransferCallType, _
                            ByVal NumberToTransfer As String, ByVal CallerNumber As String, ByVal TimeoutVal As Integer)
            MyBase.New(srcLine, oLine, NumberToTransfer, CallerNumber, TimeoutVal)
            m_TransferTargetType = transferType
            m_TransferTargetLine = transferTargetLine
            If (m_TransferTargetLine.LineType = "T") Then
                m_TargetNumber = m_TransferTargetLine.LineAccessCode & m_DialNumber
            Else
                m_TargetNumber = m_TransferTargetLine.LineAccessCode
            End If
        End Sub

        Public Overrides Sub StartOperation()
            Select Case Me.m_TransferTargetType
                Case Cmd.TransferCallType.AUTOATEDDANT
                    m_Line.spCommand.SendTransferToAACommand(m_SourceLine.LineReference, m_Line.LineReference, m_TransferTargetLine.LineReference, m_DialNumber)

                Case Cmd.TransferCallType.EXTENSION_VOICE_MESSAGE
                    m_Line.spCommand.SendTransferToVMCommand(m_SourceLine.LineReference, m_Line.LineReference, m_TransferTargetLine.LineReference, m_DialNumber)

                Case Cmd.TransferCallType.[OPERATOR]
                    m_Line.spCommand.SendTransferToOpCommand(m_SourceLine.LineReference, m_Line.LineReference, m_TransferTargetLine.LineReference, m_DialNumber)

                Case Cmd.TransferCallType.EXTENSION
                    m_Line.spCommand.SendTransferToInternalCommand(m_SourceLine.LineReference, m_Line.LineReference, m_TransferTargetLine.LineReference, m_DialNumber)

                Case Cmd.TransferCallType.TRUNK
                    m_Line.spCommand.SendTransferToExternalCommand(m_SourceLine.LineReference, m_Line.LineReference, m_TransferTargetLine.LineReference, m_DialNumber)
            End Select
            m_TimerError = m_TimeoutError
            m_Active = True
        End Sub
    End Class

    Friend Class DialEventsFilter
        Private m_Modules As New Collections.ArrayList

        Public Sub AddModule(ByVal oCallModule As DialModule)
            DBGLN(oCallModule.TargetLine, "DialModule Filter: Add Module # " & m_Modules.Count() & " " & oCallModule.ToString())
            oCallModule.TargetLine.EventFilter.AddModule(oCallModule)
            m_Modules.Add(oCallModule)
        End Sub

        Public Sub ProcessEvent(ByVal oLine As APCLine)
            Dim otObj As DialModule
            Dim index As Integer
            Dim oOrigLine As APCLine

            index = 0
            oOrigLine = oLine
            Do While (index < m_Modules.Count())
                otObj = CType(m_Modules(index), DialModule)

                If (otObj.ProcessEvent(oLine) = True) Then
                    oOrigLine = otObj.OriginalLine
                    DBGLN(oLine, "DialModule Filter: Process: Unload Module: # " & index & " " & otObj.ToString)
                    m_Modules.Remove(otObj) ' Remove the module
                    Exit Do
                Else
                    index += 1
                End If
            Loop
            oOrigLine.FireStateEvent(oLine.EventLine, APCEvents.ALERTED, Nothing, True)
        End Sub

        Public Sub Reset(ByVal oLine As APCStateLine)
            DBGLN(oLine, "Link Events Filter: Reset ")
            Dim otObj As DialModule
            Dim index As Integer
            ' Call FinishOperation function for each module in the queue
            index = 0
            Do While (index < m_Modules.Count())
                otObj = CType(m_Modules(index), DialModule)
                otObj.FinishOperation(oLine)
                If (otObj.Unload = True) Then
                    DBGLN(oLine, "DialModule Filter: Reset: Unload Module: # " & index & " " & otObj.ToString())
                    m_Modules.Remove(otObj) ' Remove the module first
                Else
                    index += 1
                End If
            Loop
        End Sub
    End Class

End Namespace
