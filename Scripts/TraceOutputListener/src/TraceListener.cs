using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Diagnostics;
using System.Windows.Forms;

namespace Diacom
{
	/// <summary>
	/// Represents new trace listener for whole application standard trace output.
	/// </summary>
	public class TraceOutputListener : TextWriterTraceListener
	{
		private TraceStream MyStream = null;
		private string NewLine = Environment.NewLine;
		private System.HTML.HTMLDocument Log = null;

		/// <summary>
		/// Creates new trace output listener, that logs into new file, located in system temporary folder.
		/// </summary>
		public TraceOutputListener()
		{
			Trace.WriteLine("Starting to listen the trace output...");
			try
			{
				this.MyStream = new TraceStream();
				base.Writer = this.MyStream;
				base.Name = "APCService trace listener";
				Trace.Listeners.Add(this);
			}
			catch(Exception x)
			{
				string message = "Could not create trace listener..."+this.NewLine+"For more information please check the the exception followed."+this.NewLine+"Exception:"+this.NewLine+x.ToString()+this.NewLine+"Inner exception:"+this.NewLine+x.InnerException.ToString();
				System.Diagnostics.EventLog.WriteEntry("APCService: trace listener", message, System.Diagnostics.EventLogEntryType.Error);
			}
			catch
			{
				System.Diagnostics.EventLog.WriteEntry("APCService: trace listener", "Unhandled exception.", System.Diagnostics.EventLogEntryType.Error);
			}
		}

		/// <summary>
		/// Closes the file and removes the listener from the list of trace listeners.
		/// </summary>
		~TraceOutputListener()
		{
			Trace.WriteLine("Saving logs...");
			try
			{
				Trace.Listeners.Remove("APCService trace TraceOutputListenerlistener");
				DirectoryInfo LogFileDirectory = new DirectoryInfo(Path.GetTempPath()+Path.DirectorySeparatorChar+"APCService");
				if(!(LogFileDirectory.Exists)) LogFileDirectory.Create();
				string LogFilePath = String.Format("{0}{1}APCService.{2}.log.html", LogFileDirectory.FullName, Path.DirectorySeparatorChar, DateTime.Now.ToString("yyyy.MM.dd-HH.mm.ss"));
				string prefix = "<pre><font color=\"#00ff00\">";
				string postfix = "</font></pre>";
				this.Log = new System.HTML.HTMLDocument(String.Format("APCService log on {0}", DateTime.Now.ToString("yyyy.MM.dd, HH:mm:ss:fff")), String.Empty, "<font color=\"#808080\">Created "+DateTime.Now.ToString("yyyy.MM.dd, HH:mm:ss:fff")+" by Diaom Ltd.<br>"+"127434, Moscow, Russia, Dmitrovskoe Shosse, 2. Tel: +7(095) 777-9698, voip.dia.com.ru, <a href=\"http://www.dia.com.ru\">www.dia.com.ru</a></font>", prefix+this.NewLine+this.MyStream.Contents+this.NewLine+postfix);
				this.Log.ToFile(LogFilePath);
			}
			catch(Exception x)
			{
				string message = "Could not dispose trace listener..."+this.NewLine+"For more information please check the the exception followed."+this.NewLine+"Exception:"+this.NewLine+x.ToString()+this.NewLine+"Inner exception:"+this.NewLine+x.InnerException.ToString();
				System.Diagnostics.EventLog.WriteEntry("APCService: trace listener", message, System.Diagnostics.EventLogEntryType.Error);
			}
			catch
			{
				System.Diagnostics.EventLog.WriteEntry("APCService: trace listener", "Unhandled exception.", System.Diagnostics.EventLogEntryType.Error);
			}
		}
	}

	public class TLSE : Diacom.APCLineControl
	{
		public TLSE()
		{
			string msg = Environment.NewLine+
				"********************************************************************************"+Environment.NewLine+
				"********************************************************************************"+Environment.NewLine+
				"********************************************************************************"+Environment.NewLine+
				"**********************                                   ***********************"+Environment.NewLine+
				"**********************                                   ***********************"+Environment.NewLine+
				"**********************                TLSE               ***********************"+Environment.NewLine+
				"**********************                                   ***********************"+Environment.NewLine+
				"**********************                                   ***********************"+Environment.NewLine+
				"********************************************************************************"+Environment.NewLine+
				"********************************************************************************"+Environment.NewLine+
				"********************************************************************************";
				Trace.WriteLine(msg);
		}

		public void Start()
		{
			string msg = Environment.NewLine+
				"********************************************************************************"+Environment.NewLine+
				"********************************************************************************"+Environment.NewLine+
				"********************************************************************************"+Environment.NewLine+
				"**********************                                   ***********************"+Environment.NewLine+
				"**********************                                   ***********************"+Environment.NewLine+
				"**********************             TLSE.Start()          ***********************"+Environment.NewLine+
				"**********************                                   ***********************"+Environment.NewLine+
				"**********************                                   ***********************"+Environment.NewLine+
				"********************************************************************************"+Environment.NewLine+
				"********************************************************************************"+Environment.NewLine+
				"********************************************************************************";
			Trace.WriteLine(msg);
		}
	}
}
