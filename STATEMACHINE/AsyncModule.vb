Option Explicit On 
Option Strict On

Imports System.Threading
Imports System.Reflection

Namespace Diacom.APCStates


    Friend MustInherit Class AsyncModule
        Protected Delegate Function AsyncFunction() As Object
        Protected m_Line As APCStateLine
        Protected m_Active As Boolean
        Protected m_Unload As Boolean
        Protected m_Instance As Object
        Protected m_Function As MethodInfo
        Protected m_Parameters As Object()
        Protected m_AsyncDelegate As AsyncFunction
        Protected m_VariableName As String

        Public Sub New(ByVal classInstance As Object, ByVal AsyncFunc As MethodInfo, ByVal funcParameters As Object())
            m_AsyncDelegate = New AsyncFunction(AddressOf Me.StartOperation)
            m_Instance = classInstance
            m_Function = AsyncFunc
            If (funcParameters Is Nothing) Then
                Dim _emptyArray() As Object = {}
                funcParameters = _emptyArray
            End If
            m_Parameters = funcParameters
            m_Active = False
            m_Unload = False
            m_VariableName = Nothing
        End Sub

        Public Overridable Sub InitOperation(ByVal oLine As APCStateLine)
            DBGLN(oLine, "Async Module: " & "Init")
            m_Line = oLine
            Try
                m_Active = True
                m_AsyncDelegate.BeginInvoke(New AsyncCallback(AddressOf Me.EndOperation), m_AsyncDelegate)
            Catch _e As Exception
                DBGEX(_e)
                ReportError()
                Me.FinishOperation(m_Line)
            End Try
        End Sub

        ' This function is called asyncronously
        Public Overridable Function StartOperation() As Object
            DBGLN(m_Line, "Async Module: StartOperation")
            Try
                ' Get the parameters information array
                Dim piArray As ParameterInfo() = m_Function.GetParameters()
                If (piArray.Length = 0) Then
                    Throw New System.Exception("Function must have non-zero parameters")
                End If
                ' Create the array of parameters to be passed
                Dim Parameters(piArray.Length - 1) As Object

                If (piArray.Length <= m_Parameters.Length) Then
                    System.Array.Copy(m_Parameters, Parameters, piArray.Length)
                ElseIf (piArray.Length = (m_Parameters.Length + 1)) Then
                    Parameters(0) = m_Line
                    m_Parameters.CopyTo(Parameters, 1)
                End If
                Return m_Function.Invoke(m_Instance, Parameters)
            Catch _e As Exception
                DBGEX(_e)
                ReportError()
                Me.FinishOperation(m_Line)
                Return Nothing
            End Try
        End Function

        Public Overridable Sub EndOperation(ByVal ar As IAsyncResult)
            DBGLN(m_Line, "Async Module: EndOperation")
            Try
                If ((m_Active = True) AndAlso (m_Unload = False) AndAlso (Not m_VariableName Is Nothing)) Then
                    SaveResult(CType(ar.AsyncState, AsyncFunction).EndInvoke(ar))
                End If
            Catch _e As Exception
                DBGEX(_e)
                ReportError()
            Finally
                Me.FinishOperation(m_Line)
            End Try
        End Sub

        Public Overridable Sub FinishOperation(ByVal oLine As APCStateLine)
            If (oLine Is m_Line) Then
                DBGLN(m_Line, "Async Module: FinishOperation")
                m_Active = False
                m_Unload = True
            End If
        End Sub

        Public Sub SaveResult(ByVal VarValue As Object)
            m_Line.Var(m_VariableName) = VarValue
            m_Line.FireStateEvent(m_Line.EventLine, APCEvents.QUERYREADY, m_VariableName, True)
        End Sub

        Public ReadOnly Property Unload() As Boolean
            Get
                Return m_Unload
            End Get
        End Property

        Public Sub ReportError()
            If (m_Active = True) AndAlso (m_Unload = False) Then
                m_Line.FireStateEvent(m_Line.EventLine, APCEvents.ELSECONTINUE, Nothing, True)
            End If
        End Sub
    End Class

    Class AsyncGetValueModule : Inherits AsyncModule
        Public Sub New(ByVal VariableName As String, ByVal classInstance As Object, ByVal AsyncFunc As MethodInfo, _
                                ByVal ParamArray paramList As Object())
            MyBase.New(classInstance, AsyncFunc, paramList)
            m_VariableName = VariableName
            If (m_VariableName Is Nothing) Then
                m_Unload = True
            End If
        End Sub

    End Class

    Friend Class AsyncFunctionsFilter
        Private m_Modules As Collections.ArrayList
        Private m_NoResultModules As Collections.ArrayList


        Public Sub New()
            m_Modules = New Collections.ArrayList
            m_NoResultModules = New Collections.ArrayList
        End Sub

        Public Sub AddModule(ByVal oLine As APCStateLine, ByVal oAsyncModule As AsyncModule)
            DBGLN(oLine, "AsyncFunctions Events Filter: Add Module # " & m_Modules.Count())
            m_Modules.Add(oAsyncModule)
            ' For every module added to the collection we should start the operation
            oAsyncModule.InitOperation(oLine)
            If (oAsyncModule.Unload = True) Then
                m_Modules.Remove(oAsyncModule)
                m_NoResultModules.Add(oAsyncModule)
            End If
        End Sub

        Public Sub Reset(ByVal oLine As APCStateLine, Optional ByVal RemoveAll As Boolean = False)
            DBGLN(oLine, "AsyncFunctions Events Filter: Reset ")
            Dim otObj As AsyncModule
            Dim index As Integer

            ' Call FinishOperation function for each module in the queue
            index = 0
            Do While (index < m_Modules.Count())
                otObj = CType(m_Modules(index), AsyncModule)
                otObj.FinishOperation(oLine)
                If ((otObj.Unload = True) Or RemoveAll) Then
                    DBGLN(oLine, "AsyncFunctions Event Filter: Reset: Unload Module: # " & index & " " & otObj.ToString)
                    m_Modules.Remove(otObj) ' Remove the module first
                Else
                    index += 1
                End If
            Loop
            ' Clear NoResult queue
            index = 0
            Do While (index < m_NoResultModules.Count())
                otObj = CType(m_NoResultModules(index), AsyncModule)
                If (otObj.Unload = True) Then
                    DBGLN(oLine, "AsyncFunctions Event Filter: Reset: Unload NoResultModule: # " & index & " " & otObj.ToString)
                    m_NoResultModules.Remove(otObj) ' Remove the module first
                Else
                    index += 1
                End If
            Loop
        End Sub
    End Class
End Namespace
