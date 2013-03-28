Option Explicit On 
Option Strict On

Imports System.Data.Odbc
Imports System.Data.SqlClient
Imports System.Data.OleDb
Imports System.Text.RegularExpressions
Imports System.Threading
Namespace Diacom.APCStates

    Friend MustInherit Class SQLModule
        Private m_Command As String
        Protected m_StartCommand As WaitCallback
        Protected m_Line As APCStateLine
        Protected m_Active As Boolean
        Protected m_Unload As Boolean

        Protected m_TimeoutError As Integer
        Private m_TimerError As Integer

        Structure SQLParamData
            Dim Direction As ParameterDirection
            Dim Value As Object
        End Structure

        Protected SQL_Parameters As Collections.ArrayList
        Protected SQL_OutputParameters As Collections.ArrayList

        Private Shared rxParam As New Regex("@+(?<Par>\w+)")
        Private Shared rxParamList As New Regex("\s*\(*\s*@+\w+\s*,*\s*\)*")
        Private Shared rxReturn As New Regex("^\s*(EXEC\w*|)\s*@+(?<Par>\w+)\s*=\s*", RegexOptions.IgnoreCase)
        Private Shared rxOutParam As New Regex("[^@]\@{1,2}(?<Par>\w+)\s+OUT", RegexOptions.IgnoreCase)
        Private m_SQLServer As String
        Protected m_SQLConnection As IDbConnection
        Protected m_SQLCommand As IDbCommand
        Protected m_SQLCommandType As CommandType
        Protected m_SQLReader As IDataReader

        Public Sub New(ByVal oLine As APCStateLine, ByVal SQLServerString As String, ByVal SQLCommandString As String, ByVal TimeoutVal As Integer)
            DBGLN(oLine, "SQL Queries Filter: New Module: SQL: " & SQLServerString & " CMD: " & SQLCommandString)

            m_Line = oLine
            m_SQLServer = SQLServerString
            m_Command = SQLCommandString
            m_TimeoutError = TimeoutVal
            m_TimerError = 0
            m_Active = False
            m_Unload = False
            m_StartCommand = New WaitCallback(AddressOf Me.StartOperation)
            m_SQLCommandType = CommandType.Text
        End Sub

        Public Overridable Sub InitOperation()
            Dim _Var As DictionaryEntry
            Dim _WrkString As String
            Dim _ParamName As String
            Dim _ReplaceArgs As Boolean
            Dim _m As Match
            Dim _VarName As String
            Dim _StoreName As String
            Dim _ReturnParamIndex As Integer
            Dim _Data As SQLParamData
            Dim LineVars As Collections.Hashtable

            Try
                ' We have to select which out of three SQL Providers we should use:
                '   - MS SQL ADO.NET provider
                '   - OLE DB (regular ADO) provider
                '   - ODBC for .NET provider
                ' We differentiate by the phrase they use to open DB.
                '   DRIVER={.....}; or DSN=  - this is a tell-tale sign of ODBC provider
                '   Provider=.....; - this is no doubt OLE DB provider
                '    .... anything else - that's MS SQL .NET
                _WrkString = m_SQLServer.ToUpper
                If ((_WrkString.IndexOf("DRIVER=") <> -1) OrElse _
                        (_WrkString.IndexOf("DSN=") <> -1)) Then
                    m_SQLConnection = New OdbcConnection(m_SQLServer)
                    m_SQLCommand = New OdbcCommand
                    _ReplaceArgs = True
                ElseIf (_WrkString.IndexOf("PROVIDER=") <> -1) Then
                    m_SQLConnection = New OleDbConnection(m_SQLServer)
                    m_SQLCommand = New OleDbCommand
                    _ReplaceArgs = True
                Else
                    m_SQLConnection = New SqlConnection(m_SQLServer)
                    m_SQLCommand = New SqlCommand
                    _ReplaceArgs = False
                End If

                SQL_Parameters = New Collections.ArrayList
                SQL_OutputParameters = New Collections.ArrayList
                ' Search for parameters in command string and add the parameters in this order.
                ' For OleDb and ODBC providers replace the parameters with "?"

                ' Make the command string all uppercase - much easier to search
                _WrkString = m_Command.ToUpper()

                ' Check if the expression has the return value
                _m = rxReturn.Match(_WrkString)
                If _m.Success Then
                    _ReturnParamIndex = _m.Groups("Par").Index
                Else
                    _ReturnParamIndex = -1
                End If

                ' Get all parameters defined for that Line
                LineVars = m_Line.Vars

                ' Find all parameters for the DB Command
                For Each _m In rxParam.Matches(_WrkString)
                    ' If the name starts with single @ - it is input parameter
                    '   with @@ - it is output
                    '       and with @@@ - it is input/output
                    If _m.Value.StartsWith("@@@") Then
                        _Data.Direction = ParameterDirection.InputOutput
                    ElseIf _m.Value.StartsWith("@@") Then
                        _Data.Direction = ParameterDirection.Output
                    Else
                        _Data.Direction = ParameterDirection.Input
                    End If

                    ' Check for the Return parameter as " [EXEC] @Par = ...."
                    If (_ReturnParamIndex = _m.Groups("Par").Index) Then
                        _Data.Direction = ParameterDirection.ReturnValue
                    End If

                    ' Check for the Output parameter as " @Par OUT"
                    Dim _outMatch As Match = rxOutParam.Match(_WrkString, _m.Index)
                    If (_outMatch.Success AndAlso (_outMatch.Index = _m.Index)) Then
                        _Data.Direction = ParameterDirection.Output
                    End If

                    _ParamName = "@" & _m.Groups("Par").Value
                    'Check if the parameter exists
                    _StoreName = _ParamName
                    _Data.Value = DBNull.Value
                    For Each _Var In LineVars
                        _VarName = _Var.Key.ToString
                        ' Prepend each name with @
                        If (Not _VarName.StartsWith("@")) Then _VarName = "@" & _VarName

                        If (_ParamName.Equals(_VarName)) Then
                            _StoreName = _Var.Key.ToString
                            _Data.Value = _Var.Value
                            Exit For
                        End If
                    Next

                    SQL_Parameters.Add(New DictionaryEntry(_ParamName, _Data))
                    If (_Data.Direction <> ParameterDirection.Input) Then
                        SQL_OutputParameters.Add(New DictionaryEntry(_StoreName, _Data))
                    End If
                Next
                ' Now replace all @@@Param1 @@Param2 and so on either with @Param1 or ?
                If (m_SQLCommandType = CommandType.StoredProcedure) Then
                    m_Command = rxReturn.Replace(m_Command, "")
                    m_Command = rxOutParam.Replace(m_Command, "@${Par}")
                    m_Command = rxParamList.Replace(m_Command, "").Trim
                ElseIf (_ReplaceArgs = True) Then
                    m_Command = rxOutParam.Replace(m_Command, "? OUT")
                    m_Command = rxParam.Replace(m_Command, "?")
                Else
                    m_Command = rxReturn.Replace(m_Command, "EXEC ")
                    m_Command = rxOutParam.Replace(m_Command, "@${Par} OUT")
                    m_Command = rxParam.Replace(m_Command, "@${Par}")
                End If

                ' Place the StartOperation Method into the WorkingThread Queue
                ThreadPool.QueueUserWorkItem(m_StartCommand)
            Catch _e As Exception
                m_Active = False
                DBGEX(_e)
                Throw _e
            End Try
        End Sub

        Public Overridable Sub StartOperation(ByVal oObject As Object)
            Dim _Var As DictionaryEntry
            Dim _Param As IDbDataParameter
            Dim _Data As SQLParamData

            Try
                ' Open SQL connection and create all the objects needed for retrieving the data
                ' It's OK to open and close the connection for every single request - the Pooling
                ' implementation will take care of maintainig some of the connections open
                DBGLN(m_Line, "SQL Queries: Start:" & " Command:" & m_Command)
                m_SQLCommand.Connection = m_SQLConnection
                m_SQLCommand.CommandType = m_SQLCommandType
                m_SQLCommand.CommandText = m_Command

                ' Add all parameters to the Command
                For Each _Var In SQL_Parameters
                    _Data = CType(_Var.Value, SQLParamData)
                    _Param = m_SQLCommand.CreateParameter()
                    _Param.ParameterName = _Var.Key.ToString
                    _Param.Direction = _Data.Direction
                    If (_Data.Value Is Nothing) Then
                        _Param.Value = DBNull.Value
                    Else
                        _Param.Value = _Data.Value
                        If TypeOf _Data.Value Is String Then
                            _Param.DbType = DbType.String
                            _Param.Size = 4000
                        ElseIf TypeOf _Data.Value Is Integer Then
                            _Param.DbType = DbType.Int32
                        ElseIf TypeOf _Data.Value Is Long Then
                            _Param.DbType = DbType.Int64
                        ElseIf TypeOf _Data.Value Is DateTime Then
                            _Param.DbType = DbType.DateTime
                        ElseIf TypeOf _Data.Value Is Decimal Then
                            _Param.DbType = DbType.Decimal
                        ElseIf TypeOf _Data.Value Is Double Then
                            _Param.DbType = DbType.Double
                        End If
                    End If
                    m_SQLCommand.Parameters.Add(_Param)
                Next _Var

                m_TimerError = m_TimeoutError
                m_Active = True
                m_SQLConnection.Open()
            Catch _e As Exception
                DBGEX(_e)
                Try
                    m_SQLConnection.Close()
                Catch _ee As Exception
                    DBGEX(_ee)
                End Try
                Throw _e
            End Try
        End Sub

        Public Overridable Sub FinishOperation()
            Try
                If ((m_Active = True) AndAlso Not (m_SQLCommand Is Nothing) AndAlso _
                    (m_SQLCommand.Connection.State <> ConnectionState.Closed)) Then m_SQLCommand.Cancel()
            Catch _e As Exception
                DBGEX(_e)
            Finally
                m_Active = False
                m_Unload = True
            End Try
        End Sub

        Public Overridable Sub TimerEvent()
            If (m_Active = False) Then Exit Sub
            ' Check for the Error Timeout
            If (m_TimerError > 0) Then
                m_TimerError -= 1
                If (m_TimerError = 0) Then
                    ' Timeout happened - report this
                    DBGLN(m_Line, "SQL Queries Filter: Timeout: ")
                    FinishOperation()
                    m_Line.FireStateEvent(m_Line.EventLine, APCEvents.WAITERROR, Nothing, True)
                End If
            End If
        End Sub

        Public Sub ReportError()
            If (m_Active = True) AndAlso (m_Unload = False) Then m_Line.FireStateEvent(m_Line.EventLine, APCEvents.ELSECONTINUE, Nothing, True)
        End Sub

        Public Sub SaveVariables()
            Dim _Var As DictionaryEntry
            Dim _Param As IDbDataParameter
            Dim _ParName As String
            Dim _val As Object

            If (m_Active = True) Then
                'Get all Return and Output Parameters
                For Each _Var In SQL_OutputParameters
                    _ParName = _Var.Key.ToString
                    If Not _ParName.StartsWith("@") Then _ParName = "@" & _ParName
                    _Param = CType(m_SQLCommand.Parameters(_ParName), IDbDataParameter)
                    _val = _Param.Value
                    m_Line.Var(_Var.Key.ToString) = _val
                Next _Var
            End If
        End Sub

        Public Sub SaveResult(ByVal VarName As String, ByVal VarValue As Object)
            If (m_Active = True) Then
                m_Line.Var(VarName) = VarValue
                m_Line.FireStateEvent(m_Line.EventLine, APCEvents.QUERYREADY, VarName, True)
            End If
        End Sub

        Public ReadOnly Property Unload() As Boolean
            Get
                Return m_Unload
            End Get
        End Property

    End Class

    Friend Class SQLNonQueryModule : Inherits SQLModule
        Private m_VarName As String
        Public Sub New(ByVal oLine As APCStateLine, ByVal VarName As String, ByVal SQLServerString As String, ByVal SQLCommandString As String, ByVal TimeoutVal As Integer)
            MyBase.New(oLine, SQLServerString, SQLCommandString, TimeoutVal)
            m_VarName = VarName
            If (m_VarName Is Nothing) Then m_Unload = True
        End Sub

        Public Overrides Sub StartOperation(ByVal oObject As Object)
            Dim _Result As Integer
            Try ' Execute the command and prepare to wait for a long, long, long time ....
                MyBase.StartOperation(oObject)
                _Result = m_SQLCommand.ExecuteNonQuery()
                If (m_Unload = False) Then
                    SaveResult(m_VarName, _Result)
                End If
            Catch _e As Exception
                DBGEX(_e)
                ReportError()
            Finally
                m_Active = False
                m_Unload = True
                Try
                    m_SQLConnection.Close()
                Catch _ee As Exception
                    DBGEX(_ee)
                End Try
            End Try
        End Sub
    End Class


    Friend Class SQLGetValueModule : Inherits SQLModule
        Private m_VarName As String

        Public Sub New(ByVal oLine As APCStateLine, ByVal VarName As String, ByVal SQLServerString As String, ByVal SQLCommandString As String, ByVal TimeoutVal As Integer)
            MyBase.New(oLine, SQLServerString, SQLCommandString, TimeoutVal)
            m_VarName = VarName
            If (m_VarName Is Nothing) Then m_Unload = True
        End Sub

        Public Overrides Sub StartOperation(ByVal oObject As Object)
            Dim _Result As Object
            Try
                MyBase.StartOperation(oObject)
                ' Execute the command and prepare to wait for a long, long, long time ....
                _Result = m_SQLCommand.ExecuteScalar()
                If (m_Unload = False) Then
                    SaveResult(m_VarName, _Result)
                End If
            Catch _e As Exception
                DBGEX(_e)
                ReportError()
            Finally
                m_Active = False
                m_Unload = True
                Try
                    m_SQLConnection.Close()
                Catch _ee As Exception
                    DBGEX(_ee)
                End Try
            End Try
        End Sub
    End Class

    Friend Class SQLGetRecordModule : Inherits SQLModule
        Private m_VarName As String

        Public Sub New(ByVal oLine As APCStateLine, ByVal RecordName As String, ByVal SQLServerString As String, ByVal SQLCommandString As String, ByVal TimeoutVal As Integer)
            MyBase.New(oLine, SQLServerString, SQLCommandString, TimeoutVal)
            m_VarName = RecordName
            If (m_VarName Is Nothing) Then m_Unload = True
        End Sub

        Public Overrides Sub StartOperation(ByVal oObject As Object)
            Dim _oneRow As Collections.ArrayList
            Dim _Record As DataRecord
            Dim _index As Integer
            Dim _val As Object

            Try
                MyBase.StartOperation(oObject)
                _Record = New DataRecord
                m_SQLReader = m_SQLCommand.ExecuteReader()

                If (m_SQLReader.FieldCount > 0) Then
                    'Get names of all fields 
                    Dim _names(m_SQLReader.FieldCount - 1) As String
                    For _index = 0 To m_SQLReader.FieldCount - 1
                        _names(_index) = (m_SQLReader.GetName(_index))
                    Next
                    _Record = New DataRecord(_names)

                    While (m_SQLReader.Read())
                        _oneRow = New Collections.ArrayList
                        For _index = 0 To m_SQLReader.FieldCount - 1
                            If m_SQLReader.IsDBNull(_index) Then
                                _val = Nothing
                            Else
                                _val = m_SQLReader.GetValue(_index)
                            End If
                            _oneRow.Add(_val)
                        Next
                        _Record.Add(_oneRow)
                    End While
                End If
                m_SQLReader.Close()
                If (m_Unload = False) Then
                    'Get all Return and Output Parameters
                    SaveVariables()
                    ' Finally store the variable and generate Q-event
                    SaveResult(m_VarName, _Record)
                End If
            Catch _e As Exception
                DBGEX(_e)
                ReportError()
            Finally
                m_Active = False
                m_Unload = True
                Try
                    If (Not (m_SQLReader Is Nothing) AndAlso (Not m_SQLReader.IsClosed)) Then m_SQLReader.Close()
                    m_SQLConnection.Close()
                Catch _ee As Exception
                    DBGEX(_ee)
                End Try
            End Try
        End Sub
    End Class



    Friend Class SQLExecProcedureModule : Inherits SQLModule
        Private m_VarName As String

        Public Sub New(ByVal oLine As APCStateLine, ByVal RecordName As String, ByVal SQLServerString As String, ByVal SQLCommandString As String, ByVal TimeoutVal As Integer)
            MyBase.New(oLine, SQLServerString, SQLCommandString, TimeoutVal)
            m_VarName = RecordName
            m_SQLCommandType = CommandType.StoredProcedure
            If (m_VarName Is Nothing) Then m_Unload = True
        End Sub

        Public Overrides Sub StartOperation(ByVal oObject As Object)
            Dim _oneRow As Collections.ArrayList
            Dim _Record As DataRecord
            Dim _index As Integer
            Dim _val As Object

            Try
                MyBase.StartOperation(oObject)
                _Record = New DataRecord
                m_SQLReader = m_SQLCommand.ExecuteReader()

                ' Get all the results from the execution
                If (m_SQLReader.FieldCount > 0) Then
                    'Get names of all fields 
                    Dim _names(m_SQLReader.FieldCount - 1) As String
                    For _index = 0 To m_SQLReader.FieldCount - 1
                        _names(_index) = (m_SQLReader.GetName(_index))
                    Next
                    _Record = New DataRecord(_names)

                    While (m_SQLReader.Read())
                        _oneRow = New Collections.ArrayList
                        For _index = 0 To m_SQLReader.FieldCount - 1
                            If m_SQLReader.IsDBNull(_index) Then
                                _val = Nothing
                            Else
                                _val = m_SQLReader.GetValue(_index)
                            End If
                            _oneRow.Add(_val)
                        Next
                        _Record.Add(_oneRow)
                    End While
                End If
                m_SQLReader.Close()
                If (m_Unload = False) Then
                    'Get all Return and Output Parameters
                    SaveVariables()
                    ' Finally store the variable and generate Q-event
                    SaveResult(m_VarName, _Record)
                End If
            Catch _e As Exception
                DBGEX(_e)
                ReportError()
            Finally
                m_Active = False
                m_Unload = True
                Try
                    If (Not m_SQLReader.IsClosed) Then m_SQLReader.Close()
                    m_SQLConnection.Close()
                Catch _ee As Exception
                    DBGEX(_ee)
                End Try
            End Try
        End Sub
    End Class



    Friend Class SQLQueriesFilter
        Private m_Modules As Collections.ArrayList
        Private m_NoResultModules As Collections.ArrayList
        Public m_Line As APCStateLine

        Public Sub New(ByVal oLine As APCStateLine)
            m_Line = oLine
            m_Modules = New Collections.ArrayList
            m_NoResultModules = New Collections.ArrayList
        End Sub

        Public Sub AddModule(ByVal oSQLModule As SQLModule)
            DBGLN(m_Line, "SQL Queries Filter: Add Module # " & m_Modules.Count())
            m_Modules.Add(oSQLModule)
            oSQLModule.InitOperation()
            If (oSQLModule.Unload = True) Then
                DBGLN(m_Line, "SQL Queries Filter: Remove Module # " & m_Modules.Count())
                m_Modules.Remove(oSQLModule) ' Remove the module 
                m_NoResultModules.Add(oSQLModule) ' Move into Separate collection
            End If
        End Sub

        Public Sub ProcessTimer()
            Dim otObj As SQLModule
            Dim index As Integer
            index = 0
            Do While (index < m_Modules.Count)
                otObj = CType(m_Modules(index), SQLModule)
                otObj.TimerEvent()
                If (otObj.Unload = True) Then
                    DBGLN(m_Line, "SQL Queries Filter: Timer: Unload Module: # " & index & " " & otObj.ToString)
                    m_Modules.Remove(otObj) ' Remove the module 
                Else
                    index += 1
                End If
            Loop
            ' Now go thru all NoResult (non-returning modules)
            index = 0
            Do While (index < m_NoResultModules.Count)
                otObj = CType(m_NoResultModules(index), SQLModule)
                If (otObj.Unload = True) Then
                    DBGLN(m_Line, "SQL Queries Filter: Timer: Unload AsyncModule: # " & index & " " & otObj.ToString)
                    m_NoResultModules.Remove(otObj) ' Remove the module 
                Else
                    index += 1
                End If
            Loop
        End Sub


        Public Sub Reset(Optional ByVal RemoveAll As Boolean = False)
            DBGLN(m_Line, "SQL Queries Filter: Reset ")
            Dim otObj As SQLModule
            Dim index As Integer
            ' Call FinishOperation function for each module in the queue
            index = 0
            Do While (index < m_Modules.Count)
                otObj = CType(m_Modules(index), SQLModule)
                otObj.FinishOperation()
                If ((otObj.Unload = True) Or RemoveAll) Then
                    DBGLN(m_Line, "SQL Queries Filter: Reset: Unload Module: # " & index & " " & otObj.ToString)
                    m_Modules.Remove(otObj) ' Remove the module first
                Else
                    index += 1
                End If
            Loop
        End Sub
    End Class
End Namespace
