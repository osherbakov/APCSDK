Imports System
Imports System.IO
Imports System.Text
Imports System.Diagnostics
Imports System.Windows.Forms

Public Class TraceStream : Inherits TextWriter
    Private _Stream As StreamWriter
    Private _TraceListener As TextWriterTraceListener

    Public Sub New()
        Trace.WriteLine("TraceStream::TraceStram() call...")
    End Sub

    Private Sub AddToTraceListaners()
        Me._Stream.WriteLine("Start logging at " + DateTime.Now.ToString())
        Me._TraceListener = New TextWriterTraceListener(Me)
        Trace.Listeners.Add(Me._TraceListener)
    End Sub

    Public Sub New(ByVal Output As Stream)
        Trace.WriteLine("TraceStream::TraceStram(Stream) call...")
        Try
            Me._Stream = New StreamWriter(Output)
        Catch ex As Exception
            Trace.WriteLine("TraceStream: can't handle output stream: exception:" + Environment.NewLine + ex.ToString())
        End Try
        Me.AddToTraceListaners()
    End Sub

    Public Sub New(ByVal aFilePath As String)
        Trace.WriteLine("TraceStream::TraceStram(String) call...")
        Try
            Me._Stream = New StreamWriter(aFilePath, True)
        Catch ex As Exception
            Trace.WriteLine("TraceStream: can't handle output stream: exception:" + Environment.NewLine + ex.ToString())
        End Try
        Me.AddToTraceListaners()
    End Sub

    Public Overloads Sub Write(ByVal aString As String)
        Me._Stream.Write(aString)
        Me._Stream.Flush()
        MessageBox.Show(aString, "writing to trace", MessageBoxButtons.OK, MessageBoxIcon.Information)
    End Sub

    Public Overloads Sub WriteLine(ByVal aString As String)
        Me._Stream.WriteLine(aString)
        Me._Stream.Flush()
        MessageBox.Show(aString, "writing to trace", MessageBoxButtons.OK, MessageBoxIcon.Information)
    End Sub

    Public Overrides ReadOnly Property Encoding() As System.Text.Encoding
        Get
            Return System.Text.Encoding.Default
        End Get
    End Property

    Public Overloads Sub Dispose()
        Trace.WriteLine("TraceStream.Dispose() call...")
        Me._Stream.Close()
    End Sub

    Public Overloads Sub Dispose(ByVal disposing As Boolean)
        Trace.WriteLine("TraceStream.Dispose(boolean) call...")
        Me._Stream.Close()
    End Sub
End Class

Public Class MainScript : Inherits Diacom.APCLineControl
    Dim TL As TraceStream

    Sub New()
    End Sub

    Sub Main()
        SharedSubTest("Main script initialization...")
        SharedSubTest("Creating trace listener...")
        Dim aLogDir As New DirectoryInfo(Application.StartupPath + Path.DirectorySeparatorChar + "logs")
        If Not aLogDir.Exists Then
            aLogDir.Create()
        End If
        Me.TL = New TraceStream(aLogDir.FullName + Path.DirectorySeparatorChar + DateTime.Now.ToFileTime.ToString() + ".log")
    End Sub

    Sub Dispose()
        Me.TL.Dispose()
    End Sub

    Sub Z()
        SharedSubTest("[MainScript] State: Z")
    End Sub

    Sub ZI()
        SharedSubTest("[MainScript] State: ZI")
        PlayNumber(Now(), "%date")
        WaitComplete()
    End Sub

    Sub ZJ()
        SharedSubTest("[MainScript] State: ZJ")
        ReturnState()
    End Sub

    Sub ZX()
        SharedSubTest("[MainScript] State: ZX")
    End Sub

    Sub ZIZ()
        SharedSubTest("[MainScript] State: ZIZ")
        ReturnState()
    End Sub

    Sub ZIP()
        SharedSubTest("[MainScript] State: ZIP")
        PlayFile(4015, "%phrase")
        ReturnState()
    End Sub

    Sub ZIR()
        SharedSubTest("[MainScript] State: ZIR")
        PlayNumber(Now(), "%date")
        PlayNumber(Now(), "%time")
        WaitComplete()
        ReturnState()
    End Sub

    Sub ZIF()
        SharedSubTest("[MainScript] State: ZIF")
        PlayFile(4800, "%phrase")
        WaitComplete()
        Release()
        ReturnState()
    End Sub

    Sub ZIQ()
        SharedSubTest("[MainScript] State: ZIQ")
    End Sub

    Sub ZI1()
        PlayNumber(1)
        SharedSubTest("[MainScript] State: ZI1")
        ReturnState()
    End Sub

    Sub ZI2()
        PlayNumber(2)
        SharedSubTest("[MainScript] State: ZI2")
        ReturnState()
    End Sub

    Sub ZI3()
        PlayNumber(3)
        SharedSubTest("[MainScript] State: ZI3")
        ReturnState()
    End Sub

    Sub ZI4()
        PlayNumber(4)
        SharedSubTest("[MainScript] State: ZI4")
        ReturnState()
    End Sub

    Sub ZI5()
        PlayNumber(5)
        SharedSubTest("[MainScript] State: ZI5")
        ReturnState()
    End Sub

    Sub ZI6()
        PlayNumber(6)
        SharedSubTest("[MainScript] State: ZI6")
        ReturnState()
    End Sub

    Sub ZI7()
        PlayNumber(7)
        SharedSubTest("[MainScript] State: ZI7")
        ReturnState()
    End Sub

    Sub ZI8()
        PlayNumber(8)
        SharedSubTest("[MainScript] State: ZI8")
        ReturnState()
    End Sub

    Sub ZI9()
        PlayNumber(9)
        SharedSubTest("[MainScript] State: ZI9")
        ReturnState()
    End Sub

    Sub ZI0()
        PlayNumber(0)
        SharedSubTest("[MainScript] State: ZI0")
        ReturnState()
    End Sub

    Sub ZV()
        SharedSubTest("[MainScript] State: ZV")
        ReturnState()
    End Sub

    Sub ZVS()
        SharedSubTest("[MainScript] State: ZVS")
        ReturnState()
    End Sub

    Sub ZVU()
        SharedSubTest("[MainScript] State: ZVU")
        ReturnState()
    End Sub
End Class
