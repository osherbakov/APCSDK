Option Explicit On 
Option Strict On

Namespace Diacom

    ''' <summary>
    ''' Represents the internal system-independent events that state machine processes.
    ''' </summary>
    <Serializable()> _
    Public Enum APCEvents

        ''' <summary>
        ''' Go to new state.
        ''' </summary>
        NEWSTATE = 0    ' New state

        ''' <summary>
        ''' Execute the code thru the method.
        ''' </summary>
        EXECUTE

        ''' <summary>
        ''' The last command was executed and returned status is OK.
        ''' </summary>
        ''' 
        COMMANDOK       ' 

        ''' <summary>
        ''' The last command was executed and returned FAIL status.
        ''' </summary>
        COMMANDFAIL

        ''' <summary>
        ''' The DIAL or RING command had executed and the given line is ringing now.
        ''' </summary>
        RINGBACK        '

        ''' <summary>
        ''' The block of previously issued commands was successfully executed.
        ''' </summary>
        THENCONTINUE    ' Z

        ''' <summary>
        ''' The block of previously issued commands had failed.
        ''' </summary>
        ELSECONTINUE    ' S

        ''' <summary>
        ''' DTMF Digit was issued on the selected line.
        ''' </summary>
        DIGIT           ' 0 - F, P for Pound, R - for staR

        ''' <summary>
        ''' There is an incoming ring on the line.
        ''' </summary>
        RING            ' J

        ''' <summary>
        ''' The line is either under APC control or connect signal came from the line.
        ''' </summary>
        CONNECT         ' I

        ''' <summary>
        ''' Disconnect / Hang up signal came from the line.
        ''' </summary>
        DISCONNECT      ' X

        ''' <summary>
        ''' Voice activity or other signal came from the line.
        ''' </summary>
        ALERTED         ' V

        ''' <summary>
        ''' The disconnect signal came from the linked line.
        ''' </summary>
        DISCONNECTLINK  ' Y

        ''' <summary>
        ''' The command that was issued with timeout parameter was not executed withing a specified time.
        ''' </summary>
        WAITERROR       ' W

        ''' <summary>
        ''' Global timeout for the line was exceeded.
        ''' </summary>
        TIMEOUT         ' T

        ''' <summary>
        ''' The async query from SQL server returned result or async function call was executed.
        ''' </summary>
        QUERYREADY      ' Q

        ''' <summary>
        ''' The line is requesting to be placed under the control of another line.
        ''' </summary>
        LINKREQUEST     ' L

        ''' <summary>
        ''' The line is returning from the control of another line .
        ''' </summary>
        LINKRELEASE     ' U

        ''' <summary>
        ''' The line is informing that it is ready to be under control of another line.
        ''' </summary>
        GRANTREQUEST    ' G
    End Enum

    ''' <summary>
    ''' Holds parameters passed with the APCEvents.
    ''' </summary>
    <Serializable()> _
    Public Class APCEventArgs : Inherits System.EventArgs

        ''' <summary>
        ''' The line for which that event is assigned.
        ''' </summary>
        Public DestinationLine As APCLine

        ''' <summary>
        ''' Event type (member of <see cref="APCEvents"/> enumeration).
        ''' </summary>
        Public EventType As APCEvents

        ''' <summary>
        ''' Parameters for one of the <see cref="APCEvents"/> enumeration member.
        ''' </summary>
        Public EventParam As Object

        ''' <summary>
        ''' Specifies if the event should go through the line filter or not.
        ''' </summary>
        Public CallLineFilter As Boolean
    End Class

    ''' <summary>
    ''' Represents the line under the APC control.
    ''' </summary>
    <Serializable()> _
    Public Class APCLine
        Inherits LineClass

        ''' <summary>
        ''' Total time the call continued.
        ''' </summary>
        Public CallRingTime As System.DateTime

        ''' <summary>
        ''' Time the connection started.
        ''' </summary>
        Public CallConnectTime As System.DateTime

        ''' <summary>
        ''' Time the connection ended.
        ''' </summary>
        Public CallDisconnectTime As System.DateTime

        ''' <summary>
        ''' Timeout for the line.
        ''' </summary>
        Public Timeout As Integer

        ''' <summary>
        ''' Current language for the line.
        ''' </summary>
        Public CurrentLanguageSet As String

        ''' <summary>
        ''' Line to link to.
        ''' </summary>
        Public LinkLine As APCLine

        ''' <summary>
        ''' The line all events will came to.
        ''' </summary>
        Public EventLine As APCLine

        Private oLastEvent As String = String.Empty
        Private oLastDigit As String = String.Empty
        Private oAllDigits As String = String.Empty
        Private oVars As New Collections.Hashtable
        Private oStatesStack As New Collections.Stack
        Private ReadOnly oInitialState As String
        Private prevState As String
        Private prevModule As String
        Private oModuleName As String
        Private oStateName As String
        Private oLastEventParameter As String = String.Empty

        ''' <summary>
        ''' Occurs when line status has been chanded.
        ''' </summary>
        Public Event APCStateEvent(ByVal sender As Object, ByVal e As APCEventArgs)

        ''' <summary>
        ''' Initializes new instance of the class with default parameters.
        ''' </summary>
        Public Sub New(ByVal oInitState As String)
            MyBase.New()
            oModuleName = Nothing
            oInitialState = oInitState
            prevModule = Nothing
            prevState = oInitState
            Init()
        End Sub

        ''' <summary>
        ''' Implements access to a named variables.
        ''' </summary>
        ''' <param name="NameString">Name of the variable.</param>
        ''' <value>Variable value or <c>Nothing</c> if no variable with this name available.</value>
        Default Public Property Item(ByVal NameString As String) As Object
            Get
                Return Var(NameString)
            End Get
            Set(ByVal Value As Object)
                Var(NameString) = Value
            End Set
        End Property

        ' Variables control Methods

        ''' <summary>
        ''' Implements access to a named variables.
        ''' </summary>
        ''' <param name="NameString">Name of the variable.</param>
        ''' <value>Variable value or <c>Nothing</c> if no variable with this name available.</value>
        Public Property Var(ByVal NameString As String) As Object
            Get
                Dim _Result As Object = Nothing
                If Not NameString Is Nothing Then
                    NameString = NameString.Trim.ToUpper
                    SyncLock (oVars.SyncRoot)
                        _Result = oVars(NameString)
                        If _Result Is Nothing Then
                            TraceOut.Put("Line : " & CStr(Me.LineID) & " " & NameString & " >> null")
                        Else
                            TraceOut.Put("Line : " & CStr(Me.LineID) & " " & NameString & " >> " & _Result.ToString())
                        End If
                    End SyncLock
                End If
                Return _Result
            End Get
            Set(ByVal Value As Object)
                If Not NameString Is Nothing Then
                    NameString = NameString.Trim.ToUpper
                    SyncLock (oVars.SyncRoot)
                        oVars(NameString) = Value
                        If Value Is Nothing Then
                            TraceOut.Put("Line : " & CStr(Me.LineID) & " " & NameString & " << null")
                        Else
                            TraceOut.Put("Line : " & CStr(Me.LineID) & " " & NameString & " << " & Value.ToString())
                        End If
                    End SyncLock
                End If
            End Set
        End Property

        ''' <summary>
        ''' Gets a <see cref="Collections.Hashtable"/> of all named variables.
        ''' </summary>
        ''' <value><see cref="Collections.Hashtable"/> of all named variables.</value>
        Public ReadOnly Property Vars() As Collections.Hashtable
            Get
                Dim _ret As Collections.Hashtable
                SyncLock (oVars.SyncRoot)
                    _ret = CType(oVars.Clone(), Collections.Hashtable)
                End SyncLock
                Return _ret
            End Get
        End Property


        ''' <summary>
        ''' Function that returns a full state name.
        ''' </summary>
        ''' <param name="StateName">Name of the state.</param>
        ''' <returns>Full state name.</returns>
        ''' <remarks>
        ''' <para>Full state name for the state is something like:</para>
        ''' <para>ZZZZ with no module -> ZZZZ</para>
        ''' <para>ZZZZ with module MMMM -> MMMM.ZZZZ</para>
        ''' <para>.ZZZZ with any module -> ZZZZ</para>
        ''' </remarks>
        Public Function FullStateName(ByVal StateName As String) As String
            Dim _split(), _ret As String
            _split = StateName.Split("."c)
            If ((_split.Length < 2) AndAlso (Not oModuleName Is Nothing)) Then
                _ret = oModuleName & "." & StateName
            ElseIf ((_split.Length > 1) AndAlso _split(0).Equals(String.Empty)) Then
                _ret = _split(1)
            Else
                _ret = StateName
            End If
            Return _ret
        End Function

        ''' <summary>
        ''' Gets current line state.
        ''' </summary>
        ''' <value>Current line state.</value>
        Public Property State() As String
            Get
                Return oStateName
            End Get
            Set(ByVal Value As String)
                prevState = oStateName
                prevModule = oModuleName
                Dim _split() As String = Value.Split("."c)
                If (_split.Length < 2) Then
                    oModuleName = Nothing
                    oStateName = Value
                Else
                    oModuleName = _split(0)
                    oStateName = _split(1)
                End If
            End Set
        End Property

        ''' <summary>
        ''' Returns line to the previous state.
        ''' </summary>
        Public Sub PopState()
            oStateName = prevState
            oModuleName = prevModule
        End Sub

        Public Sub PushFunctionState(ByVal ReturnState As String)
            Me.oStatesStack.Push(FullStateName(ReturnState))
        End Sub

        Public Function PopFunctionState() As String
            Return CStr(Me.oStatesStack.Pop())
        End Function

        ''' <summary>
        ''' Initializes the line.
        ''' </summary>
        ''' <remarks>
        ''' All the values will be set by default.
        ''' </remarks>
        Public Sub Init()
            EventLine = Me
            LinkLine = Nothing
            oVars.Clear()
            oStatesStack.Clear()
            Timeout = 0
            oModuleName = Nothing
            oStateName = InitialState
            LastDigit = ""
            AllDigits = ""
            CallRingTime = System.DateTime.Now()
            CallConnectTime = CallRingTime
            CallDisconnectTime = CallRingTime
            CurrentLanguageSet = "default"
        End Sub

        ''' <summary>
        ''' Gets initial state of the line.
        ''' </summary>
        ''' <value>Initial state of the line.</value>
        Public ReadOnly Property InitialState() As String
            Get
                Return Me.oInitialState
            End Get
        End Property

        ''' <summary>
        ''' Gets last event occurred on the line.
        ''' </summary>
        ''' <value>Last event occurred on the line.</value>
        Public Property LastEvent() As String
            Get
                Return oLastEvent
            End Get
            Set(ByVal Value As String)
                oLastEvent = Value
            End Set
        End Property

        ''' <summary>
        ''' Gets  parameter of the last event occurred on the line.
        ''' </summary>
        ''' <value>Parameter of the last event occurred on the line.</value>
        Public Property LastEventParameter() As String
            Get
                Return oLastEventParameter
            End Get
            Set(ByVal Value As String)
                oLastEventParameter = Value
            End Set
        End Property

        ''' <summary>
        ''' Gets last digit on the line if available.
        ''' </summary>
        ''' <value>Last digit on the line if available.</value>
        Public Property LastDigit() As String
            Get
                Return oLastDigit
            End Get
            Set(ByVal Value As String)
                oLastDigit = Value
                oAllDigits &= Value
            End Set
        End Property

        ''' <summary>
        ''' Gets all digits for the line.
        ''' </summary>
        ''' <value>All digits for the line.</value>
        Public Property AllDigits() As String
            Get
                Return oAllDigits
            End Get
            Set(ByVal Value As String)
                oAllDigits = Value
            End Set
        End Property

        ''' <summary>
        ''' Gets current duration of the call.
        ''' </summary>
        ''' <value>Current duration of the call.</value>
        Public ReadOnly Property InCallTime() As Integer
            Get
                Dim _Duration As System.TimeSpan
                _Duration = System.DateTime.Now.Subtract(CallConnectTime)
                Return CInt(_Duration.TotalSeconds)
            End Get
        End Property

        ''' <summary>
        ''' Gets total duration of the call.
        ''' </summary>
        ''' <value>Total duration of the call.</value>
        Public ReadOnly Property CallDuration() As Integer
            Get
                Dim _Duration As System.TimeSpan
                _Duration = CallDisconnectTime.Subtract(CallConnectTime)
                Return CInt(_Duration.TotalSeconds)
            End Get
        End Property

        ''' <summary>
        ''' Raizes event of given type for the given line with given event data and data filter.
        ''' </summary>
        ''' <param name="destLine">Destination line.</param>
        ''' <param name="eventType">Type of event will raized.</param>
        ''' <param name="eventData">Data for the event.</param>
        ''' <param name="useLineFilter">Filter used for this event.</param>
        Public Sub FireStateEvent(ByVal destLine As APCLine, ByVal eventType As APCEvents, ByVal eventData As Object, ByVal useLineFilter As Boolean)
            Dim _arg As APCEventArgs = New APCEventArgs
            _arg.DestinationLine = destLine
            _arg.EventType = eventType
            _arg.EventParam = eventData
            _arg.CallLineFilter = useLineFilter
            RaiseEvent APCStateEvent(Me, _arg)
        End Sub

        ''' <summary>
        ''' Format string for <see cref="ToString"/>() method.
        ''' </summary>
        ''' <remarks>
        ''' Actually is the following string:
        ''' <pre>  ID | Number     | Name                           | Port     | Prefix   | Status               | Type | User Name                      | User Number | Call Name                      | Call Number | CID Name                       | CID NUmber  | DID Name                       | DID Number  | DNIS Name                      | DNIS Number</pre>
        ''' </remarks>
        Public Const ToStringFormat As String = "  ID | Number     | Name                           | Port     | Prefix   | Status               | Type | User Name                      | User Number | Call Name                      | Call Number | CID Name                       | CID NUmber  | DID Name                       | DID Number  | DNIS Name                      | DNIS Number"

        ''' <summary>
        ''' Separator string for <see cref="ToString"/>() method.
        ''' </summary>
        ''' <remarks>
        ''' Actually is the following string:
        ''' <pre>----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------</pre>
        ''' </remarks>
        Public Const ToStringSeparator As String = "----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------"

        ''' <summary>
        ''' Converts content of class to string.
        ''' </summary>
        ''' <returns>String presentation of class properties.</returns>

        Public Overrides Function ToString() As String
            Dim Str As New System.Text.StringBuilder(String.Empty)
            Str.Append(Me.LineID.ToString().PadLeft(4) + " | ")
            Str.Append(Me.LineNumber.PadRight(10) + " | ")
            Str.Append(Me.LineName.PadRight(30) + " | ")
            Str.Append(Me.LinePort.PadRight(8) + " | ")
            Str.Append(Me.LineAccessCode.PadRight(8) + " | ")
            Str.Append(Me.LineStatus.ToString().PadRight(20) + " | ")
            Str.Append(Me.LineType.PadRight(4) + " | ")
            Str.Append(Me.UserName.PadRight(30) + " | ")
            Str.Append(Me.UserNumber.PadRight(11) + " | ")
            Str.Append(Me.CallName.PadRight(30) + " | ")
            Str.Append(Me.CallNumber.PadRight(11) + " | ")
            Str.Append(Me.CIDName.PadRight(30) + " | ")
            Str.Append(Me.CIDNumber.PadRight(11) + " | ")
            Str.Append(Me.DIDName.PadRight(30) + " | ")
            Str.Append(Me.DIDNumber.PadRight(11) + " | ")
            Str.Append(Me.DNISName.PadRight(30) + " | ")
            Str.Append(Me.DNISNumber.PadRight(11))
            Return Str.ToString()
        End Function
    End Class
End Namespace