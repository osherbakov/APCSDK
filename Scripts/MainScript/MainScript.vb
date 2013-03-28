Imports System
Imports System.IO
Imports System.Text
Imports System.Diagnostics

Public Class MainScript : Inherits Diacom.APCLineControl
    Private Class TraceStream : Inherits TextWriter
        Private Title As String
        Private NL As String
        Private Storage As StringBuilder

        Public Sub New()
            Me.Title = "APCService - log on " & DateTime.Now.ToString("dd MM yyyy, HH:mm:ss")
            Me.NL = Environment.NewLine
            Me.Storage = New StringBuilder
            Me.Storage.AppendFormat("<html>{0}<head>{0}<meta http-equiv=""Content-Type"" content=""text/html; charset=unicode"">{0}<title>{1}</title>{0}</head>{0}", Me.NL, Me.Title)
            Me.Storage.AppendFormat("<body bgcolor=""#000000""{0}", Me.NL)
            Me.Storage.AppendFormat("<h3 align=""center"">{0}</h3><hr>{1}", Me.Title, Me.NL)
        End Sub

        Public Overloads Sub Write(ByVal value As Char)
            Me.Storage.Append(value)
        End Sub

        Public Overloads Sub Write(ByVal value As Char())
            Me.Storage.Append(value)
        End Sub

        Public Overloads Sub Write(ByVal value As Char(), ByVal index As Integer, ByVal count As Integer)
            Me.Storage.Append(value, index, count)
        End Sub

        Public Overloads Sub Write(ByVal value As Boolean)
            Me.Storage.Append(value)
        End Sub

        Public Overloads Sub Write(ByVal value As Integer)
            Me.Storage.Append("<span style=""color: darkRed"">")
            Me.Storage.Append(value)
            Me.Storage.Append("</span>")
        End Sub

        Public Overloads Sub Write(ByVal value As UInt32)
            Me.Storage.Append("<span style=""color: darkRed"">")
            Me.Storage.Append(value)
            Me.Storage.Append("</span>")
        End Sub

        Public Overloads Sub Write(ByVal value As Long)
            Me.Storage.Append("<span style=""color: darkRed"">")
            Me.Storage.Append(value)
            Me.Storage.Append("</span>")
        End Sub

        Public Overloads Sub Write(ByVal value As UInt64)
            Me.Storage.Append("<span style=""color: darkRed"">")
            Me.Storage.Append(value)
            Me.Storage.Append("</span>")
        End Sub

        Public Overloads Sub Write(ByVal value As Single)
            Me.Storage.Append("<span style=""color: darkRed"">")
            Me.Storage.Append(value)
            Me.Storage.Append("</span>")
        End Sub

        Public Overloads Sub Write(ByVal value As Double)
            Me.Storage.Append("<span style=""color: darkRed"">")
            Me.Storage.Append(value)
            Me.Storage.Append("</span>")
        End Sub

        Public Overloads Sub Write(ByVal value As Decimal)
            Me.Storage.Append("<span style=""color: darkRed"">")
            Me.Storage.Append(value)
            Me.Storage.Append("</span>")
        End Sub

        Public Overloads Sub Write(ByVal value As String)
            Me.Storage.Append(value)
        End Sub

        Public Overloads Sub Write(ByVal value As Object)
            Me.Storage.Append(value)
        End Sub

        Public Overloads Sub Write(ByVal format As String, ByVal arg0 As Object)
            Me.Storage.AppendFormat(format, arg0)
        End Sub

        Public Overloads Sub Write(ByVal format As String, ByVal arg0 As Object, ByVal arg1 As Object)
            Me.Storage.AppendFormat(format, arg0, arg1)
        End Sub

        Public Overloads Sub Write(ByVal format As String, ByVal arg0 As Object, ByVal arg1 As Object, ByVal arg2 As Object)
            Me.Storage.AppendFormat(format, arg0, arg1, arg2)
        End Sub

        Public Overloads Sub Write(ByVal format As String, ByVal ParamArray arg() As Object)
            Me.Storage.AppendFormat(format, arg)
        End Sub

        Public Overloads Sub WriteLine()
            Me.Storage.Append("<br>")
            Me.Storage.Append(Me.NL)
        End Sub

        Public Overloads Sub WriteLine(ByVal value As Char)
            Me.Storage.Append(value)
            Me.Storage.Append("<br>")
            Me.Storage.Append(Me.NL)
        End Sub

        Public Overloads Sub WriteLine(ByVal value As Char())
            Me.Storage.Append(value)
            Me.Storage.Append("<br>")
            Me.Storage.Append(Me.NL)
        End Sub

        Public Overloads Sub WriteLine(ByVal value As Char(), ByVal index As Integer, ByVal count As Integer)
            Me.Storage.Append(value, index, count)
            Me.Storage.Append("<br>")
            Me.Storage.Append(Me.NL)
        End Sub

        Public Overloads Sub WriteLine(ByVal value As Boolean)
            Me.Storage.Append(value)
            Me.Storage.Append("<br>")
            Me.Storage.Append(Me.NL)
        End Sub

        Public Overloads Sub WriteLine(ByVal value As Integer)
            Me.Storage.Append(value)
            Me.Storage.Append("<br>")
            Me.Storage.Append(Me.NL)
        End Sub

        Public Overloads Sub WriteLine(ByVal value As UInt32)
            Me.Storage.Append(value)
            Me.Storage.Append("<br>")
            Me.Storage.Append(Me.NL)
        End Sub

        Public Overloads Sub WriteLine(ByVal value As Long)
            Me.Storage.Append(value)
            Me.Storage.Append("<br>")
            Me.Storage.Append(Me.NL)
        End Sub

        Public Overloads Sub WriteLine(ByVal value As UInt64)
            Me.Storage.Append(value)
            Me.Storage.Append("<br>")
            Me.Storage.Append(Me.NL)
        End Sub

        Public Overloads Sub WriteLine(ByVal value As Single)
            Me.Storage.Append(value)
            Me.Storage.Append("<br>")
            Me.Storage.Append(Me.NL)
        End Sub

        Public Overloads Sub WriteLine(ByVal value As Double)
            Me.Storage.Append(value)
            Me.Storage.Append("<br>")
            Me.Storage.Append(Me.NL)
        End Sub

        Public Overloads Sub WriteLine(ByVal value As Decimal)
            Me.Storage.Append(value)
            Me.Storage.Append("<br>")
            Me.Storage.Append(Me.NL)
        End Sub

        Public Overloads Sub WriteLine(ByVal value As String)
            Me.Storage.Append(value)
            Me.Storage.Append("<br>")
            Me.Storage.Append(Me.NL)
        End Sub

        Public Overloads Sub WriteLine(ByVal value As Object)
            Me.Storage.Append(value)
            Me.Storage.Append("<br>")
            Me.Storage.Append(Me.NL)
        End Sub

        Public Overloads Sub WriteLine(ByVal format As String, ByVal arg0 As Object)
            Me.Storage.AppendFormat(format, arg0)
            Me.Storage.Append("<br>")
            Me.Storage.Append(Me.NL)
        End Sub

        Public Overloads Sub WriteLine(ByVal format As String, ByVal arg0 As Object, ByVal arg1 As Object)
            Me.Storage.AppendFormat(format, arg0, arg1)
            Me.Storage.Append("<br>")
            Me.Storage.Append(Me.NL)
        End Sub

        Public Overloads Sub WriteLine(ByVal format As String, ByVal arg0 As Object, ByVal arg1 As Object, ByVal arg2 As Object)
            Me.Storage.AppendFormat(format, arg0, arg1, arg2)
            Me.Storage.Append("<br>")
            Me.Storage.Append(Me.NL)
        End Sub

        Public Overloads Sub WriteLine(ByVal format As String, ByVal ParamArray arg() As Object)
            Me.Storage.AppendFormat(format, arg)
            Me.Storage.Append("<br>")
            Me.Storage.Append(Me.NL)
        End Sub

        Public Overrides ReadOnly Property Encoding() As Encoding
            Get
                Return Encoding.Unicode
            End Get
        End Property

        Public Overrides Property NewLine() As String
            Get
                Return Me.NL
            End Get
            Set(ByVal value As String)
                Me.NL = value
            End Set
        End Property

        Public Overloads Function ToString() As String
            Return Me.Storage.ToString()
        End Function

        Public Sub ToFile(ByVal FilePath As String)
            Dim SW As New StreamWriter(FilePath, False, Me.Encoding)
            SW.WriteLine(Me.Storage.ToString())
            SW.Close()
        End Sub

        Public Sub ToFile()
            Dim FilePath As String = Path.GetTempPath() & "APCService." & DateTime.Now.ToString("yyyy.MM.dd.HH.mm.ss") & ".log.html"
            Me.ToFile(FilePath)
        End Sub
    End Class

    Private Class TraceListener : Inherits TextWriterTraceListener
        Private TraceListenerStream As TraceStream

        Public Sub New()
            Trace.WriteLine("Creating listener...")
            Try
                Me.TraceListenerStream = New TraceStream
                MyBase.Writer = Me.TraceListenerStream
                MyBase.Name = "APCService Trace Output Listener"
                Trace.Listeners.Add(Me)
            Catch ex As Exception
                Trace.WriteLine("Couldn't create the listener...")
                Return
            End Try
            Trace.WriteLine("Listener created")
        End Sub

        Protected Overrides Sub Finalize()
            If Not Me.TraceListenerStream Is Nothing Then
                Me.Close()
            End If
        End Sub

        Public Overloads Sub Close()
            If Not Me.TraceListenerStream Is Nothing Then
                Trace.WriteLine("Shutting down listener...")
                Trace.Listeners.Remove(Me)
                Me.TraceListenerStream.ToFile()
                Me.TraceListenerStream = Nothing
            End If
        End Sub
    End Class

    Private Class SQLCmdFrm
        Public Shared ConnectionString As String = "Integrated Security=SSPI; server=SCOTCH; database=APCService; Connect Timeout=30"
        Public Shared CheckTheBaseExistsConnectionString As String = "Integrated Security=SSPI; server=SCOTCH; Connect Timeout=30"
        Public Shared CheckTheBaseExists As String = _
            "IF NOT EXISTS (SELECT name FROM master.dbo.sysdatabases WHERE name = N'APCService')" + Environment.NewLine + _
            "	CREATE DATABASE [APCService]"
        Public Shared CheckTheLineDataTableExists As String = _
         "IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE ID = object_id(N'[Line{0}Data]') AND OBJECTPROPERTY(ID, N'IsUserTable') = 1)" + Environment.NewLine + _
         "CREATE TABLE [Line{0}Data] ( " + Environment.NewLine + _
         "	[start]			[datetime]		NOT NULL DEFAULT (getdate()), " + Environment.NewLine + _
         "	[end]			[datetime]		NULL, " + Environment.NewLine + _
         "	[direction]		[bit]			NOT NULL DEFAULT (0), " + Environment.NewLine + _
         "	[connection]	[char] (512)	NULL, " + Environment.NewLine + _
         ") ON [PRIMARY] "
        Public Shared SetCallStart As String = "INSERT INTO [dbo].[Line{0}Data] VALUES ('{1}', NULL, {2}, '{3}')"
        Public Shared SetCallEnd As String = "UPDATE [dbo].[Line{0}Data] SET [end] = '{2}' WHERE [start] = '{1}'"
        Public Shared GetAllCalls As String = "SELECT * FROM [dbo].[Line{0}Data]"
        Public Shared GetLimitedCountOfLastCalls As String = "SELECT TOP {1} * FROM [dbo].[Line{0}Data] ORDER BY [end] DESC"
    End Class

    Private DataBaseExistsCheckComplete As Boolean = False

    Private Sub Check()
        If Not Me.DataBaseExistsCheckComplete Then
            Me.DataBaseExistsCheckComplete = True
            SQLNonQuery(SQLCmdFrm.CheckTheBaseExistsConnectionString, SQLCmdFrm.CheckTheBaseExists)
        End If
        SQLNonQuery(SQLCmdFrm.ConnectionString, String.Format(SQLCmdFrm.CheckTheLineDataTableExists, Me.Line.LineID.ToString()))
        Me.Var("DBEntryIsAlreadyPlaced") = False
    End Sub

    Private Sub PlaceDBEntry(ByVal Direction As Integer)
        If Not Me.Var("DBEntryIsAlreadyPlaced") Then
            Me.Var("DBEntryIsAlreadyPlaced") = True
            SQLNonQuery(SQLCmdFrm.ConnectionString, String.Format(SQLCmdFrm.SetCallStart, Me.Line.LineID.ToString(), Me.CallConnectTime.ToString("MM.dd.yyyy HH:mm:ss:fff"), Direction, Me.LineName))
        End If
    End Sub

    Private TL As TraceListener

    Sub New()
        Me.TL = New TraceListener
        Trace.WriteLine("[MainScript] Initialization")
    End Sub

    Protected Overrides Sub Finalize()
        Me.TL.Close()
        Trace.WriteLine("[MainScript] Shutting down")
    End Sub

    Sub Dispose()
        Me.TL.Close()
    End Sub

    Sub Main()
        Trace.WriteLine("[MainScript] Main procedure")
    End Sub

    Sub Z()
        Trace.WriteLine("[MainScript] State: Z")
        Check()
    End Sub

    Sub ZI()
        Trace.WriteLine("[MainScript] State: ZI")
        PlaceDBEntry(0)
        PlayNumber(Now(), "%date")
        WaitComplete()
    End Sub

    Sub ZJ()
        Trace.WriteLine("[MainScript] State: ZJ")
    End Sub

    Sub ZX()
        SQLNonQuery(SQLCmdFrm.ConnectionString, String.Format(SQLCmdFrm.SetCallEnd, Me.Line.LineID.ToString(), Me.CallConnectTime.ToString("MM.dd.yyyy HH:mm:ss:fff"), Me.CallDisconnectTime.ToString("MM.dd.yyyy HH:mm:ss:fff")))
        Trace.WriteLine("[MainScript] State: ZX")

    End Sub

    Sub ZIZ()
        Trace.WriteLine("[MainScript] State: ZIZ")
        ReturnState()
    End Sub

    Sub ZIP()
        Trace.WriteLine("[MainScript] State: ZIP")
        PlayFile(4015, "%phrase")
        ReturnState()
    End Sub

    Sub ZIR()
        Trace.WriteLine("[MainScript] State: ZIR")
        PlayNumber(Now(), "%date")
        PlayNumber(Now(), "%time")
        WaitComplete()
        ReturnState()
    End Sub

    Sub ZIF()
        Trace.WriteLine("[MainScript] State: ZIF")
        PlayFile(4800, "%phrase")
        WaitComplete()
        Release()
        ReturnState()
    End Sub

    Sub ZIQ()
        Trace.WriteLine("[MainScript] State: ZIQ")
    End Sub

    Sub ZI0()
        PlayNumber(0)
        Trace.WriteLine("[MainScript] State: ZI0")
        ReturnState()
    End Sub

    Sub ZI1()
        PlayNumber(1)
        Trace.WriteLine("[MainScript] State: ZI1")
        ReturnState()
    End Sub

    Sub ZI2()
        PlayNumber(2)
        Trace.WriteLine("[MainScript] State: ZI2")
        ReturnState()
    End Sub

    Sub ZI3()
        PlayNumber(3)
        Trace.WriteLine("[MainScript] State: ZI3")
        ReturnState()
    End Sub

    Sub ZI4()
        PlayNumber(4)
        Trace.WriteLine("[MainScript] State: ZI4")
        ReturnState()
    End Sub

    Sub ZI5()
        PlayNumber(5)
        Trace.WriteLine("[MainScript] State: ZI5")
        ReturnState()
    End Sub

    Sub ZI6()
        PlayNumber(6)
        Trace.WriteLine("[MainScript] State: ZI6")
        ReturnState()
    End Sub

    Sub ZI7()
        PlayNumber(7)
        Trace.WriteLine("[MainScript] State: ZI7")
        ReturnState()
    End Sub

    Sub ZI8()
        PlayNumber(8)
        Trace.WriteLine("[MainScript] State: ZI8")
        ReturnState()
    End Sub

    Sub ZI9()
        PlayNumber(9)
        Trace.WriteLine("[MainScript] State: ZI9")
        ReturnState()
    End Sub

    Sub ZV()
        Trace.WriteLine("[MainScript] State: ZV")
        ReturnState()
    End Sub

    Sub ZVS()
        Trace.WriteLine("[MainScript] State: ZVS")
        ReturnState()
    End Sub

    Sub ZVU()
        Trace.WriteLine("[MainScript] State: ZVU")
        ReturnState()
    End Sub
End Class
