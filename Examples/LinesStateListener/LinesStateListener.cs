using System;
using System.IO;
using System.Drawing;
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows.Forms;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Diacom
{
	/// <summary>
	/// Summary description for LinesStateListener.
	/// </summary>
	public class CLinesStateListener : System.Windows.Forms.Form
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		/// <summary>
		/// 
		/// </summary>
		public CLinesStateListener()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.richTextBox1 = new System.Windows.Forms.RichTextBox();
			this.SuspendLayout();
			// 
			// richTextBox1
			// 
			this.richTextBox1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.richTextBox1.Font = new System.Drawing.Font("Lucida Console", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(204)));
			this.richTextBox1.Location = new System.Drawing.Point(0, 0);
			this.richTextBox1.Name = "richTextBox1";
			this.richTextBox1.ReadOnly = true;
			this.richTextBox1.Size = new System.Drawing.Size(568, 445);
			this.richTextBox1.TabIndex = 0;
			this.richTextBox1.Text = "";
			this.richTextBox1.WordWrap = false;
			// 
			// CLinesStateListener
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(568, 445);
			this.Controls.Add(this.richTextBox1);
			this.Name = "CLinesStateListener";
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
			this.Text = "Lines States Listener";
			this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
			this.ResumeLayout(false);

		}
		#endregion

		private System.Windows.Forms.RichTextBox richTextBox1;

		private class LinesStateChangedEventsListener
		{
			private TcpClient TheClient = null;
			private NetworkStream TheStream = null;
			private BinaryReader Reader = null;
			private BinaryWriter Writer = null;
			private System.Threading.Thread RunningThreadHandle = null;
			private bool Quit = false;

			public LinesStateChangedEventsListener(string aMachineName, int aPortNumber)
			{
				this.RunningThreadHandle = new System.Threading.Thread(new System.Threading.ThreadStart(this.RunningThread));
				this.RunningThreadHandle.Name = "Lines State Listener";
				this.RunningThreadHandle.IsBackground = true;
				this.RunningThreadHandle.Start();
			}

			public void Dispose(bool disposing)
			{
				this.Quit = true;
			}

			public delegate void DataReadyEventHandler(string aData);
			public event DataReadyEventHandler DataReady;
			public void Write(object aMessage)
			{
				this.Writer.Write(aMessage.ToString());
			}

			private void RunningThread()
			{
				this.TheClient = new TcpClient();
				this.TheClient.Connect(IPAddress.Parse("127.0.0.1"), 8089);
				this.TheStream = this.TheClient.GetStream();
				this.Reader = new BinaryReader(this.TheStream);
				this.Writer = new BinaryWriter(this.TheStream);
				while(!this.Quit)
				{
					string Message = this.Reader.ReadString();
					if((this.DataReady != null)&&(Message.Length > 0))
					{
						this.DataReady(Message);
					}
					else Thread.Sleep(1);
				}
			}
		}

		private LinesStateChangedEventsListener listener = null;

		/// <summary>
		/// Starts listening process.
		/// </summary>
		/// <param name="aMachineName">Name of the machine the APCService is running on.</param>
		/// <param name="aPortNumber">Port to connect.</param>
		public void Start(string aMachineName, int aPortNumber)
		{
			this.listener = new LinesStateChangedEventsListener(aMachineName, aPortNumber);
			this.listener.DataReady += new Diacom.CLinesStateListener.LinesStateChangedEventsListener.DataReadyEventHandler(listener_DataReady);
			Application.Run(this);
		}

		/// <summary>
		/// The main entry point.
		/// </summary>
		/// <param name="args">Arguments to run with.</param>
		[STAThread]
		public static void Main(string [] args)
		{
			CLinesStateListener lsl = new CLinesStateListener();
			lsl.Start("SCOTCH", 8089);
		}

		private void listener_DataReady(string aData)
		{
			this.richTextBox1.AppendText(aData+Environment.NewLine);
		}
	}
}
