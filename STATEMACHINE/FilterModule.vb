Option Explicit On 
Option Strict On

Imports System.Threading

Namespace Diacom.APCStates

    Friend MustInherit Class EventProcessingModule
        Protected ReadOnly m_SourceLine As APCStateLine
        Protected ReadOnly m_Line As APCStateLine
        Protected ReadOnly m_TimeoutError As Integer
        Protected m_Active As Boolean
        Protected m_TimerError As Integer
        Protected m_Unload As Boolean

        Public Sub New(ByVal sourceLine As APCStateLine, ByVal targetLine As APCStateLine, ByVal TimeoutVal As Integer)
            m_SourceLine = sourceLine
            m_Line = targetLine
            m_TimeoutError = TimeoutVal
            m_TimerError = 0
            m_Active = False
            m_Unload = False
        End Sub

        Public MustOverride Sub StartOperation()

        Public Overridable Function ProcessEvent(ByVal EventType As Diacom.APCEvents, ByVal EventData As Object) As Boolean
            DBGLN(m_Line, "Events Base: " & Me.ToString & ": Process")
            ProcessEvent = False ' By default we don't process this event, so other modules can do it
            If (m_Active = False) Then Exit Function
            Select Case EventType
                Case APCEvents.COMMANDOK
                    ProcessEvent = True
                    MyClass.FinishOperation()

                Case APCEvents.COMMANDFAIL
                    ProcessEvent = True
                    MyClass.FinishOperation()
                    m_Line.FireStateEvent(m_Line.EventLine, APCEvents.ELSECONTINUE, Nothing, True)
            End Select
        End Function
        Public Overridable Sub FinishOperation()
            DBGLN(m_Line, "Events Base: " & Me.ToString & ": Finish")
            m_Active = False
            m_Unload = True
        End Sub

        Public Overridable Sub TimerEvent()
            If (m_Active = False) Then Exit Sub
            ' Check for the Error Timeout
            If (m_TimerError > 0) Then
                m_TimerError -= 1
                If (m_TimerError = 0) Then
                    ' Timeout - report this and finish the operation
                    FinishOperation()
                    m_Line.FireStateEvent(m_Line.EventLine, APCEvents.WAITERROR, Nothing, True)
                End If
            End If
        End Sub

        Public ReadOnly Property Unload() As Boolean
            Get
                Return m_Unload
            End Get
        End Property

        Public Sub StoreVariable(ByVal VarName As String, ByVal VarValue As Object)
            Dim Value As String = "<Nothing>"
            If Not VarValue Is Nothing Then Value = VarValue.ToString()
            DBGLN(m_Line, "Store Variable: Var :" & VarName & " Value: " & Value)
            m_Line.Var(VarName) = VarValue
        End Sub
    End Class

    Friend Class ChannelEventsFilter

        Private m_Modules As New Collections.ArrayList
        Private m_Handlers As New Collections.ArrayList
        Public m_Line As APCStateLine

        Public Sub New(ByVal oLine As APCStateLine)
            m_Line = oLine
        End Sub

        Public Sub AddEventHandler(ByVal oProcessingModule As EventProcessingModule)
            DBGLN(m_Line, "Events Filter: Add EventHandler # " & m_Handlers.Count() & " " & oProcessingModule.ToString())
            m_Handlers.Insert(m_Handlers.Count, oProcessingModule)
        End Sub

        Public Sub AddModule(ByVal oProcessingModule As EventProcessingModule)
            DBGLN(m_Line, "Events Filter: Add Module # " & m_Modules.Count() & " " & oProcessingModule.ToString)
            m_Modules.Add(oProcessingModule)
            ' If this module is the first in queue - start actual I/O operation
            If (m_Modules.Count() = 1) Then
                oProcessingModule.StartOperation()
                If (oProcessingModule.Unload = True) Then
                    DBGLN(m_Line, "Events Filter: Add Mod: Unload Module: # 0 " & oProcessingModule.ToString)
                    m_Modules.Remove(oProcessingModule)
                End If
            End If
        End Sub

        Public Function RemoveModule(ByVal oModule As EventProcessingModule) As Integer
            DBGLN(m_Line, "Events Filter: Remove Module " & oModule.ToString())
            Dim position As Integer = m_Modules.IndexOf(oModule)
            If position = (-1) Then Return m_Modules.Count
            m_Modules.RemoveAt(position) ' Remove the module first
            ' After we removed top module - Start actual I/O operation on the next one
            If position < m_Modules.Count Then
                Dim otObj As EventProcessingModule = CType(m_Modules(position), EventProcessingModule)
                otObj.StartOperation()
                If (otObj.Unload = True) Then position = RemoveModule(otObj)
            End If
            Return position
        End Function

        Public Function ProcessEvent(ByVal sourceLine As APCStateLine, ByVal EventType As APCEvents, ByVal EventData As Object) As Boolean
            DBGLN(Me.m_Line, "Events Filter: Event : " & [Enum].GetName(GetType(APCEvents), EventType))
            Dim index As Integer
            Dim UsedEvent As Boolean = False

            If EventType = APCEvents.NEWSTATE OrElse EventType = APCEvents.EXECUTE OrElse _
                    (m_Modules.Count() = 0 AndAlso m_Handlers.Count = 0) Then
                ' When there are no modules installed or special events - give it back to the state machine
                sourceLine.FireStateEvent(m_Line.EventLine, EventType, EventData, False)
            Else
                ' There are filter modules installed - walk thru all of them and call ProcessEvent function.
                ' If module doesn't know what to do with that event - it will return false, so we have to
                ' continue calling this ProcessEvent on other modules. If module returns true - that means
                ' that it consumed that message.
                If EventType = APCEvents.COMMANDOK OrElse EventType = APCEvents.COMMANDFAIL OrElse EventType = APCEvents.DIGIT Then
                    index = 0
                    Do While (UsedEvent = False) And (index < m_Modules.Count())
                        Dim otObj As EventProcessingModule = CType(m_Modules(index), EventProcessingModule)
                        UsedEvent = otObj.ProcessEvent(EventType, EventData)
                        DBGLN(m_Line, "Events Filter[" & index.ToString & "]: Used= " & UsedEvent & " Unload= " & otObj.Unload)
                        If (otObj.Unload = True) Then
                            index = RemoveModule(otObj)
                        Else
                            index += 1
                        End If
                    Loop
                End If

                ' Now loop thru event handlers and find any of them will catch the event
                If EventType <> APCEvents.COMMANDOK AndAlso EventType <> APCEvents.COMMANDFAIL Then
                    index = 0
                    Do While (UsedEvent = False) AndAlso (index < m_Handlers.Count())
                        Dim evObj As OnEventModule = CType(m_Handlers(index), OnEventModule)
                        UsedEvent = evObj.ProcessEvent(sourceLine, EventType, EventData)
                        DBGLN(m_Line, "Events Handler [" & index.ToString & "]: Used = " & UsedEvent & " Unload = " & evObj.Unload)
                        If (evObj.Unload = True) Then
                            m_Handlers.Remove(evObj) ' Remove the handler first
                        Else
                            index += 1
                        End If
                    Loop
                End If
                If (UsedEvent = False) Then
                    ' There is no receiver for that event - give it back to the state machine
                    DBGLN(m_Line, "Events Filter: Not processed by modules and handlers")
                    sourceLine.FireStateEvent(m_Line.EventLine, EventType, EventData, False)
                End If
            End If
        End Function

        Public Sub ProcessTimer()
            Dim index As Integer

            index = 0
            Do While (index < m_Handlers.Count())
                Dim evObj As OnEventModule = CType(m_Handlers(index), OnEventModule)
                evObj.TimerEvent()
                If (evObj.Unload = True) Then
                    DBGLN(Me.m_Line, "Events Handler: Timeout: Unload[" & index & "] " & evObj.ToString)
                    m_Handlers.Remove(evObj) ' Remove the module
                Else
                    index += 1
                End If
            Loop

            index = 0
            Do While (index < m_Modules.Count())
                Dim otObj As EventProcessingModule = CType(m_Modules(index), EventProcessingModule)
                otObj.TimerEvent()
                If (otObj.Unload = True) Then
                    DBGLN(Me.m_Line, "Timeout: Unload[" & index & "] " & otObj.ToString)
                    index = RemoveModule(otObj)
                Else
                    index += 1
                End If
            Loop

            ' The global Timeout processing should be done after regular timeout
            If (m_Line.Timeout > 0) Then
                m_Line.Timeout -= 1
                If (m_Line.Timeout = 0) Then
                    m_Line.FireStateEvent(m_Line.EventLine, APCEvents.TIMEOUT, Nothing, True)
                End If
            End If
        End Sub

        Public Sub Reset(Optional ByVal RemoveAll As Boolean = False)
            DBGLN(Me.m_Line, "Events Filter: Reset ")
            Dim otObj As EventProcessingModule
            Dim index As Integer

            m_Handlers.Clear()
            ' Call FinishOperation function for each module in the queue
            index = 0
            Do While (index < m_Modules.Count())
                otObj = CType(m_Modules(index), EventProcessingModule)
                otObj.FinishOperation()
                If ((otObj.Unload = True) Or RemoveAll) Then
                    DBGLN(m_Line, "Events Filter: Reset: Unload Module: # " & index & " " & otObj.ToString)
                    m_Modules.Remove(otObj) ' Remove the module first
                Else
                    index += 1
                End If
            Loop
        End Sub
    End Class
End Namespace
