using System;
using System.IO;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Diagnostics;
using System.Threading;

namespace APCLinesListener
{
	/// <summary>
	/// Implements main APC Lines Listener form class.
	/// </summary>
	public class APCLinesListenerMDITrace : System.Windows.Forms.Form
	{
		private System.ComponentModel.Container components = null;
		private Thread RunningStuffHandle = null;
		/// <summary>
		/// RichTextBox for trace output.
		/// </summary>
		public System.Windows.Forms.RichTextBox richTextBoxTraceOutPut;

		/// <summary>
		/// Creates new form.
		/// </summary>
		public APCLinesListenerMDITrace()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
			this.Text = "Trace output";
			this.richTextBoxTraceOutPut.AppendText("Creating and starting main trace listener...");
			this.RunningStuffHandle = new Thread(new ThreadStart(this.Run));
			this.RunningStuffHandle.IsBackground = true;
			this.RunningStuffHandle.Start();
			this.richTextBoxTraceOutPut.AppendText("  OK"+Environment.NewLine);
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if((disposing)&&(components != null)) components.Dispose();
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(APCLinesListenerMDITrace));
			this.richTextBoxTraceOutPut = new System.Windows.Forms.RichTextBox();
			this.SuspendLayout();
			// 
			// richTextBoxTraceOutPut
			// 
			this.richTextBoxTraceOutPut.Dock = System.Windows.Forms.DockStyle.Fill;
			this.richTextBoxTraceOutPut.Font = new System.Drawing.Font("Lucida Console", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(204)));
			this.richTextBoxTraceOutPut.Location = new System.Drawing.Point(0, 0);
			this.richTextBoxTraceOutPut.Name = "richTextBoxTraceOutPut";
			this.richTextBoxTraceOutPut.ReadOnly = true;
			this.richTextBoxTraceOutPut.Size = new System.Drawing.Size(792, 573);
			this.richTextBoxTraceOutPut.TabIndex = 1;
			this.richTextBoxTraceOutPut.Text = "";
			this.richTextBoxTraceOutPut.WordWrap = false;
			// 
			// APCLinesListenerMDITrace
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(792, 573);
			this.Controls.Add(this.richTextBoxTraceOutPut);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "APCLinesListenerMDITrace";
			this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
			this.Text = "Trace Output";
			this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
			this.ResumeLayout(false);

		}
		#endregion

		private string _TraceOutputString = String.Empty;
		private void _PutTraceLine()
		{
			this.richTextBoxTraceOutPut.AppendText(this._TraceOutputString);
			this.richTextBoxTraceOutPut.ScrollToCaret();
		}

		private void PutLineFromTraceOutPut(string aLine)
		{
			try
			{
				this._TraceOutputString = aLine;
				this.richTextBoxTraceOutPut.Invoke(new MethodInvoker(this._PutTraceLine));
			}
			catch
			{
			}
		}

		private CTraceStream _TraceStream = null;
		private TextWriterTraceListener _TextWriterTraceListener = null;
		private void Run()
		{
			this._TraceStream = new CTraceStream();
			this._TraceStream.DataReady += new CTraceStream.DataReadyEventHandler(this.PutLineFromTraceOutPut);
			this._TextWriterTraceListener = new TextWriterTraceListener(this._TraceStream);
			Trace.Listeners.Add(this._TextWriterTraceListener);

		}
	}
}
