using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Threading;

namespace APCLinesListener
{
	/// <summary>
	/// Implements new connection to AltiGen.
	/// </summary>
	public class APCLinesListenerMDIConnection : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.Splitter splitter1;
		private System.Windows.Forms.Panel panel2;
		private System.Windows.Forms.ContextMenu ConnectionContextMenu;
		private System.Windows.Forms.MenuItem menuItem_Connect;
		private System.Windows.Forms.MenuItem menuItem_Dial;
		private System.Windows.Forms.MenuItem menuItem_Disconnect;
		private System.Windows.Forms.MenuItem menuItem_PlayDTMF;
		private System.Windows.Forms.MenuItem menuItem_PlayFile;
		private System.Windows.Forms.MenuItem menuItem_RecordFile;
		private System.Windows.Forms.MenuItem menuItem_Reset;
		private System.Windows.Forms.MenuItem menuItem_RingExt;
		private System.Windows.Forms.MenuItem menuItem_SwitchMusic;
		private System.Windows.Forms.MenuItem menuItem_Transfer;
		private System.Windows.Forms.MenuItem menuItem_About;
		private System.Windows.Forms.MenuItem menuItem_Sep1;
		private System.Windows.Forms.MenuItem menuItem13;
		private System.Windows.Forms.MenuItem menuItem_CloseConnection;
		private System.Windows.Forms.MenuItem menuItem_Snatch;
		/// <summary>
		/// RichTextBox for events logging.
		/// </summary>
		public System.Windows.Forms.RichTextBox richTextBoxEvents;
		/// <summary>
		/// ListBox for lines information.
		/// </summary>
		public System.Windows.Forms.ListBox ListBoxLines;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		private ConnectionProperties _CP = null;

		/// <summary>
		/// Creates new instance of class.
		/// </summary>
		public APCLinesListenerMDIConnection(ConnectionProperties aCP)
		{
			InitializeComponent();
			this._CP = aCP;
		}

		private void OnFormLoad(object sender, EventArgs e)
		{
			this._StatusStringPrefix = "[ Thread "+AppDomain.GetCurrentThreadId().ToString()+"][APCLinesListenerMDIConnection initialization]";
			this.ListBoxLines.Items.Add(Diacom.SPLine.ToStringFormat);
			this.ListBoxLines.Items.Add(Diacom.SPLine.ToStringSeparator);
			Thread thr = new Thread(new ThreadStart(this.Run));
			thr.IsBackground = true;
			thr.Name = this._StatusStringPrefix+" [Initialization thread]";
			thr.Start();
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
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(APCLinesListenerMDIConnection));
			this.panel1 = new System.Windows.Forms.Panel();
			this.richTextBoxEvents = new System.Windows.Forms.RichTextBox();
			this.ConnectionContextMenu = new System.Windows.Forms.ContextMenu();
			this.menuItem_About = new System.Windows.Forms.MenuItem();
			this.menuItem_Sep1 = new System.Windows.Forms.MenuItem();
			this.menuItem_Connect = new System.Windows.Forms.MenuItem();
			this.menuItem_Dial = new System.Windows.Forms.MenuItem();
			this.menuItem_Disconnect = new System.Windows.Forms.MenuItem();
			this.menuItem_PlayDTMF = new System.Windows.Forms.MenuItem();
			this.menuItem_PlayFile = new System.Windows.Forms.MenuItem();
			this.menuItem_RecordFile = new System.Windows.Forms.MenuItem();
			this.menuItem_Reset = new System.Windows.Forms.MenuItem();
			this.menuItem_RingExt = new System.Windows.Forms.MenuItem();
			this.menuItem_Snatch = new System.Windows.Forms.MenuItem();
			this.menuItem_SwitchMusic = new System.Windows.Forms.MenuItem();
			this.menuItem_Transfer = new System.Windows.Forms.MenuItem();
			this.menuItem13 = new System.Windows.Forms.MenuItem();
			this.menuItem_CloseConnection = new System.Windows.Forms.MenuItem();
			this.splitter1 = new System.Windows.Forms.Splitter();
			this.panel2 = new System.Windows.Forms.Panel();
			this.ListBoxLines = new System.Windows.Forms.ListBox();
			this.panel1.SuspendLayout();
			this.panel2.SuspendLayout();
			this.SuspendLayout();
			// 
			// panel1
			// 
			this.panel1.Controls.Add(this.richTextBoxEvents);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.panel1.Location = new System.Drawing.Point(0, 309);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(600, 160);
			this.panel1.TabIndex = 0;
			// 
			// richTextBoxEvents
			// 
			this.richTextBoxEvents.ContextMenu = this.ConnectionContextMenu;
			this.richTextBoxEvents.Dock = System.Windows.Forms.DockStyle.Fill;
			this.richTextBoxEvents.Location = new System.Drawing.Point(0, 0);
			this.richTextBoxEvents.Name = "richTextBoxEvents";
			this.richTextBoxEvents.Size = new System.Drawing.Size(600, 160);
			this.richTextBoxEvents.TabIndex = 0;
			this.richTextBoxEvents.Text = "";
			// 
			// ConnectionContextMenu
			// 
			this.ConnectionContextMenu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																								  this.menuItem_About,
																								  this.menuItem_Sep1,
																								  this.menuItem_Connect,
																								  this.menuItem_Dial,
																								  this.menuItem_Disconnect,
																								  this.menuItem_PlayDTMF,
																								  this.menuItem_PlayFile,
																								  this.menuItem_RecordFile,
																								  this.menuItem_Reset,
																								  this.menuItem_RingExt,
																								  this.menuItem_Snatch,
																								  this.menuItem_SwitchMusic,
																								  this.menuItem_Transfer,
																								  this.menuItem13,
																								  this.menuItem_CloseConnection});
			// 
			// menuItem_About
			// 
			this.menuItem_About.Index = 0;
			this.menuItem_About.Text = "About";
			this.menuItem_About.Click += new System.EventHandler(this.menuItem_About_Click);
			// 
			// menuItem_Sep1
			// 
			this.menuItem_Sep1.Index = 1;
			this.menuItem_Sep1.Text = "-";
			// 
			// menuItem_Connect
			// 
			this.menuItem_Connect.Index = 2;
			this.menuItem_Connect.Text = "Connect";
			this.menuItem_Connect.Click += new System.EventHandler(this.menuItem_Connect_Click);
			// 
			// menuItem_Dial
			// 
			this.menuItem_Dial.Index = 3;
			this.menuItem_Dial.Text = "Dial";
			this.menuItem_Dial.Click += new System.EventHandler(this.menuItem_Dial_Click);
			// 
			// menuItem_Disconnect
			// 
			this.menuItem_Disconnect.Index = 4;
			this.menuItem_Disconnect.Text = "Disconnect";
			this.menuItem_Disconnect.Click += new System.EventHandler(this.menuItem_Disconnect_Click);
			// 
			// menuItem_PlayDTMF
			// 
			this.menuItem_PlayDTMF.Index = 5;
			this.menuItem_PlayDTMF.Text = "Play DTMF";
			this.menuItem_PlayDTMF.Click += new System.EventHandler(this.menuItem_PlayDTMF_Click);
			// 
			// menuItem_PlayFile
			// 
			this.menuItem_PlayFile.Index = 6;
			this.menuItem_PlayFile.Text = "Play File";
			this.menuItem_PlayFile.Click += new System.EventHandler(this.menuItem_PlayFile_Click);
			// 
			// menuItem_RecordFile
			// 
			this.menuItem_RecordFile.Index = 7;
			this.menuItem_RecordFile.Text = "Record File";
			this.menuItem_RecordFile.Click += new System.EventHandler(this.menuItem_RecordFile_Click);
			// 
			// menuItem_Reset
			// 
			this.menuItem_Reset.Index = 8;
			this.menuItem_Reset.Text = "Reset";
			this.menuItem_Reset.Click += new System.EventHandler(this.menuItem_Reset_Click);
			// 
			// menuItem_RingExt
			// 
			this.menuItem_RingExt.Index = 9;
			this.menuItem_RingExt.Text = "Ring Extension";
			this.menuItem_RingExt.Click += new System.EventHandler(this.menuItem_RingExt_Click);
			// 
			// menuItem_Snatch
			// 
			this.menuItem_Snatch.Index = 10;
			this.menuItem_Snatch.Text = "Snatch";
			this.menuItem_Snatch.Click += new System.EventHandler(this.menuItem_Snatch_Click);
			// 
			// menuItem_SwitchMusic
			// 
			this.menuItem_SwitchMusic.Index = 11;
			this.menuItem_SwitchMusic.Text = "Switch Music";
			this.menuItem_SwitchMusic.Click += new System.EventHandler(this.menuItem_SwitchMusic_Click);
			// 
			// menuItem_Transfer
			// 
			this.menuItem_Transfer.Index = 12;
			this.menuItem_Transfer.Text = "Transfer";
			this.menuItem_Transfer.Click += new System.EventHandler(this.menuItem_Transfer_Click);
			// 
			// menuItem13
			// 
			this.menuItem13.Index = 13;
			this.menuItem13.Text = "-";
			// 
			// menuItem_CloseConnection
			// 
			this.menuItem_CloseConnection.Index = 14;
			this.menuItem_CloseConnection.Text = "Close Connection";
			this.menuItem_CloseConnection.Click += new System.EventHandler(this.menuItem_CloseConnection_Click);
			// 
			// splitter1
			// 
			this.splitter1.ContextMenu = this.ConnectionContextMenu;
			this.splitter1.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.splitter1.Location = new System.Drawing.Point(0, 306);
			this.splitter1.Name = "splitter1";
			this.splitter1.Size = new System.Drawing.Size(600, 3);
			this.splitter1.TabIndex = 1;
			this.splitter1.TabStop = false;
			// 
			// panel2
			// 
			this.panel2.Controls.Add(this.ListBoxLines);
			this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel2.Location = new System.Drawing.Point(0, 0);
			this.panel2.Name = "panel2";
			this.panel2.Size = new System.Drawing.Size(600, 306);
			this.panel2.TabIndex = 2;
			// 
			// ListBoxLines
			// 
			this.ListBoxLines.ContextMenu = this.ConnectionContextMenu;
			this.ListBoxLines.Dock = System.Windows.Forms.DockStyle.Fill;
			this.ListBoxLines.Font = new System.Drawing.Font("Lucida Console", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(204)));
			this.ListBoxLines.HorizontalScrollbar = true;
			this.ListBoxLines.IntegralHeight = false;
			this.ListBoxLines.ItemHeight = 11;
			this.ListBoxLines.Location = new System.Drawing.Point(0, 0);
			this.ListBoxLines.Name = "ListBoxLines";
			this.ListBoxLines.Size = new System.Drawing.Size(600, 306);
			this.ListBoxLines.TabIndex = 0;
			// 
			// APCLinesListenerMDIConnection
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(600, 469);
			this.ContextMenu = this.ConnectionContextMenu;
			this.Controls.Add(this.panel2);
			this.Controls.Add(this.splitter1);
			this.Controls.Add(this.panel1);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "APCLinesListenerMDIConnection";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "APCLinesListenerMDIConnection";
			this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
			this.Load += new System.EventHandler(this.OnFormLoad);
			this.panel1.ResumeLayout(false);
			this.panel2.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		/// <summary>
		/// Storage for a text to set as form title (required by <see cref="_SetStatusString"/>() method).
		/// </summary>
		private string _Status = String.Empty;
		
		private string _StatusStringPrefix = "Initialization";

		/// <summary>
		/// Sets title of the form, depending on <see cref="_Status"/>, which was set by the <see cref="SetStatusString"/>() method.
		/// </summary>
		private void _SetStatusString()
		{
			this.Text = this._StatusStringPrefix+" - "+this._Status;
		}
		/// <summary>
		/// Changes title of the form depending on its argument.
		/// </summary>
		/// <param name="aStatus">The string to insert after "Lines State Listener - " in a title of the form.</param>
		private void SetStatusString(string aStatus)
		{
			try
			{
				this._Status = aStatus;
				this.Invoke(new MethodInvoker(this._SetStatusString));
			}
			catch {}
		}

		private string _EventToAdd = String.Empty;

		private void _AddToEventsLog()
		{
			this.richTextBoxEvents.AppendText(this._EventToAdd+Environment.NewLine);
			this.richTextBoxEvents.ScrollToCaret();
		}

		private void AddLineToEventLog(DateTime time, string aLine)
		{
			try
			{
				this._EventToAdd = time.ToString("[HH:mm:ss:fff]")+" "+aLine;
				this.richTextBoxEvents.Invoke(new MethodInvoker(this._AddToEventsLog));
			}
			catch {}
		}

		private Diacom.ISP _ISP = null;

		private Diacom.SPLine _Line2Update = null;
		private Hashtable _LinesStates = null;
		private Hashtable _Lines2ListBoxIndex = null;
		private int _Lines2ListBoxIndexCounter = 2;
		private void _UpdateLineInfo()
		{
			if(!(this._Lines2ListBoxIndex.ContainsKey(this._Line2Update.ID)))
			{
				this._Lines2ListBoxIndex.Add(this._Line2Update.ID, this._Lines2ListBoxIndexCounter);
				this._LinesStates.Add(this._Line2Update.ID, this._Line2Update.State);
				this.ListBoxLines.Items.Add(this._Line2Update.ToString());
				this._Lines2ListBoxIndexCounter++;
			}
			if(!(this._LinesStates[this._Line2Update.ID].Equals(this._Line2Update.State)))
			{
				this.ListBoxLines.Items[(int)(this._Lines2ListBoxIndex[this._Line2Update.ID])] = this._Line2Update.ToString();
				this._LinesStates[this._Line2Update.ID] = this._Line2Update.State;
			}
		}

		private void UpdateLineInfo(Diacom.SPLine aLine)
		{
			this._Line2Update = aLine;
			this.ListBoxLines.Invoke(new MethodInvoker(this._UpdateLineInfo));
		}

		/// <summary>
		/// Implements GetLines() command and displays lines information on a form.
		/// </summary>
		private void GetLines()
		{
			this.ListBoxLines.Items.Clear();
			this.ListBoxLines.Items.Add(Diacom.SPLine.ToStringFormat);
			this.ListBoxLines.Items.Add(Diacom.SPLine.ToStringSeparator);
			this._Lines2ListBoxIndexCounter = 2;
			foreach(Diacom.SPLine _line in this._ISP.GetLines())
			{
				this.UpdateLineInfo(_line);
			}
		}

		/// <summary>
		/// Handles all incoming from AltiGen events - just prints them.
		/// </summary>
		private void _InputEventsHandler()
		{
			object ev;
			while(true)
			{	
				ev = this._ISP.Receive();
				Diacom.Ev.EventID id = ((Diacom.Ev.EvBase)ev).ID;
				switch(id)
				{
					case Diacom.Ev.EventID.RING:
					{
						AddLineToEventLog(DateTime.Now, String.Concat("Event: ", id, " -> line = ", Convert.ToInt32(((Diacom.Ev.Ring)ev).ringInfo.ID), ", name = ", ((Diacom.Ev.Ring)ev).ringInfo.Name, ", number = ", ((Diacom.Ev.Ring)ev).ringInfo.Number));
						break;
					}
					case Diacom.Ev.EventID.RING_BACK:
					{
						AddLineToEventLog(DateTime.Now, String.Concat("Event: ", id, " -> line = ", Convert.ToInt32(((Diacom.Ev.RingBack)ev).ringInfo.ID), ", name = ", ((Diacom.Ev.RingBack)ev).ringInfo.Name, ", number = ", ((Diacom.Ev.RingBack)ev).ringInfo.Number));
						break;
					}
					case Diacom.Ev.EventID.CONNECT:
					{
						AddLineToEventLog(DateTime.Now, String.Concat("Event: ", id, " -> line = ", Convert.ToInt32(((Diacom.Ev.Connect)ev).Line)));
						break;
					}
					case Diacom.Ev.EventID.DIGIT:
					{
						AddLineToEventLog(DateTime.Now, String.Concat("Event: ", id, " -> line = ", Convert.ToInt32(((Diacom.Ev.Digit)ev).Line), ", digit = ",  ((Diacom.Ev.Digit)ev).Code));
						break;
					}
					case Diacom.Ev.EventID.DISCONNECT:
					{
						AddLineToEventLog(DateTime.Now, String.Concat("Event: ", id, " -> line = ", Convert.ToInt32(((Diacom.Ev.Disconnect)ev).Line)));
						break;
					}
					case Diacom.Ev.EventID.COMMAND_STATUS:
					{
						AddLineToEventLog(DateTime.Now, String.Concat("Event: ", id, " -> command = ", ((Diacom.Ev.CommandStatus)ev).CommandID, ", sender = ", Convert.ToInt32(((Diacom.Ev.CommandStatus)ev).Line), ", addressee = ", ((Diacom.Ev.CommandStatus)ev).Addressee, ", status = ", ((Diacom.Ev.CommandStatus)ev).Status));
						break;
					}
					case Diacom.Ev.EventID.TONE:
					{
						AddLineToEventLog(DateTime.Now, String.Concat("Event: ", id, " -> line = ", ((Diacom.Ev.Tone)ev).Line, ", type = ", ((Diacom.Ev.Tone)ev).Type));
						break;
					}
					case Diacom.Ev.EventID.LINE_STATE_CHANGED:
					{
						Diacom.SPLineState state = ((Diacom.Ev.LineStateChanged)ev).State;
						object lineID = ((Diacom.Ev.LineStateChanged)ev).Line;
						this.AddLineToEventLog(DateTime.Now, String.Concat("Event: ", id, " -> line = ", lineID, ", state = ", state));
						if((state == Diacom.SPLineState.LINE_ADD)||(state == Diacom.SPLineState.LINE_REMOVE))
						{
						}
						else
						{
							Diacom.SPLine _line = this._ISP.GetLine(lineID);
							if(_line == null) break;
							this.UpdateLineInfo(_line);
						}
						break;
					}
				}
			}
		}

		private void Run()
		{
			while(!(this.IsHandleCreated)) Thread.Sleep(59);
			this.AddLineToEventLog(DateTime.Now, "Initializing...");
			this._Lines2ListBoxIndex = new System.Collections.Hashtable();
			this._LinesStates = new System.Collections.Hashtable();
			this.AddLineToEventLog(DateTime.Now, "Creating SP...");
			this._ISP = new Diacom.AltiGen.ASPHdw();
			this.AddLineToEventLog(DateTime.Now, "Connecting to "+this._CP.ToString());
			this._ISP.Connect(this._CP.ServerIPAddress, this._CP.ServerPortNumber, this._CP.LogonType, this._CP.Extension, this._CP.Password);
			if(!(this._ISP.Status().Equals(Diacom.SPStatus.OK)))
			{
				this.SetStatusString("Error connection");
				this.AddLineToEventLog(DateTime.Now, "Error connection"+Environment.NewLine+"SP status is "+this._ISP.Status().ToString());
				this.Close();
				return;
			}
			else
			{
				this._StatusStringPrefix = "["+this._CP.Extension+"]["+this._CP.LogonType.ToString()+"]";
				this.AddLineToEventLog(DateTime.Now, "Connected OK");
			}
			this.GetLines();
			this._ISP.SPStatusEvent += new Diacom.SPStatusEventHandler(this._SPStatusEventHandler);
			this.AddLineToEventLog(DateTime.Now, "Creating and starting events handler...");
			Thread thr = new Thread(new ThreadStart(this._InputEventsHandler));
			thr.IsBackground = true;
			thr.Name = this._StatusStringPrefix+" [Input events handler]";
			thr.Start();
			this.AddLineToEventLog(DateTime.Now, "All done.");
			this.Text = this._StatusStringPrefix;
		}

		private void _SPStatusEventHandler(object source, Diacom.SPStatusEventArgs e)
		{
			this.AddLineToEventLog(DateTime.Now, String.Format("Have an SPStatus event: raised {0}:{1:D3}, status {2}, additional information {3}  - {4}", e.Time.ToLongTimeString(), e.Time.Millisecond, e.Status, e.Info, this._CP.LogonType.ToString()));
		}

		private void menuItem_Connect_Click(object sender, System.EventArgs e)
		{
			int line0 = 0;
			int line1 = 0;
			int cmdsender = 0;
			try
			{
				// Creating dialog form for user to input command parameters.
				string [] cmdHints = new string[2];
				cmdHints[0] = "First line number to connect (integer)";
				cmdHints[1] = "Second line number to connect (integer)";
				int line = 0;
				if(this.ListBoxLines.Focused) line = Convert.ToInt32(this.ListBoxLines.SelectedItem.ToString().Substring(1, 3));
				CommandParameters cmdParams = new CommandParameters(2, "Please, input parameters for Connect command", cmdHints, new string [4] {this._CP.DefaultCommandSender, line.ToString(), this._CP.DefaultCommandSender, this._CP.DefaultCommandSender});
				// Showing it
				cmdParams.ShowDialog();
				// "Cancel" button was clicked or the form was closed - nothing to do.
				if(cmdParams.Get() == null)
				{
					this.AddLineToEventLog(DateTime.Now, String.Concat("Sending Connect command: FAILED. Cause: user abort"));
					return;
				}
				// "OK" button was clicked - trying to convert arguments to appropriate type.
				line0 = Convert.ToInt32((cmdParams.Get())[0]);
				line1 = Convert.ToInt32((cmdParams.Get())[1]);
				cmdsender = Convert.ToInt32((cmdParams.Get())[4]);
			}
				// Conversion exception.
			catch(FormatException fx)
			{
				this.AddLineToEventLog(DateTime.Now, String.Concat("Sending Connect command: FAILED. Cause: invalid parameters: ", fx.Message));
				return;
			}
				// Any other exception.
			catch(Exception ox)
			{
				this.AddLineToEventLog(DateTime.Now, String.Concat("Sending Connect command: FAILED. Cause: ", ox.Message));
				return;
			}
			try
			{
				// Creating new command class with user-defined parameters.
				Diacom.Cmd.Connect cmd = new Diacom.Cmd.Connect(cmdsender, line0, line1);
				// Sending this command to AltiGen.
				this._ISP.Send(cmd);
			}
				// Any exception.
			catch(Exception cx)
			{
				this.AddLineToEventLog(DateTime.Now, String.Concat("Sending Connect command: FAILED. Cause: ", cx.Message));
				return;
			}
			// Logging all is OK.
			this.AddLineToEventLog(DateTime.Now, String.Concat("Sending Connect command: line 1 ", line0, ", line 2 ", line1, ", sender ", cmdsender));
		}

		private void menuItem_Dial_Click(object sender, System.EventArgs e)
		{
			string destination = "";
			string source = "";
			string account = "";
			int cmdsender = 0;
			try
			{
				// Creating dialog form for user to input command parameters.
				string [] cmdHints = new string[3];
				cmdHints[0] = "Destination number to connect (string)";
				cmdHints[1] = "Source number to connect (string)";
				cmdHints[2] = "Account";
				int line = 0;
				CommandParameters cmdParams = new CommandParameters(3, "Please, input parameters for Dial command", cmdHints, new string [5] {this._CP.DefaultCommandSender, line.ToString(), this._CP.Extension, String.Empty, String.Empty});
				// Showing it
				cmdParams.ShowDialog();
				// "Cancel" button was clicked or the form was closed - nothing to do.
				if(cmdParams.Get() == null)
				{
					this.AddLineToEventLog(DateTime.Now, String.Concat("Sending Dial command: FAILED. Cause: user abort"));
					return;
				}
				// "OK" button was clicked - trying to convert arguments to appropriate type.
				destination = (cmdParams.Get())[0];
				source = (cmdParams.Get())[1];
				account = (cmdParams.Get())[2];
				cmdsender = Convert.ToInt32((cmdParams.Get())[4]);
			}
				// Conversion exception.
			catch(FormatException fx)
			{
				this.AddLineToEventLog(DateTime.Now, String.Concat("Sending Dial command: FAILED. Cause: invalid parameters: ", fx.Message));
				return;
			}
				// Any other exception.
			catch(Exception ox)
			{
				this.AddLineToEventLog(DateTime.Now, String.Concat("Sending Dial command: FAILED. Cause: ", ox.Message));
				return;
			}
			try
			{
				// Creating new command class with user-defined parameters.
				Diacom.Cmd.Dial cmd = new Diacom.Cmd.Dial(cmdsender, cmdsender, destination, source, account, 1);
				// Sending this command to AltiGen.
				this._ISP.Send(cmd);
			}
				// Any exception.
			catch(Exception cx)
			{
				this.AddLineToEventLog(DateTime.Now, String.Concat("Sending Dial command: FAILED. Cause: ", cx.Message));
				return;
			}
			// Logging all is OK.
			this.AddLineToEventLog(DateTime.Now, String.Concat("Sending Dial command: destination ", destination, ", source ", source, ", sender ", cmdsender));
		}

		private void menuItem_Disconnect_Click(object sender, System.EventArgs e)
		{
			int line = 0;
			int cmdsender = 0;
			try
			{
				// Creating dialog form for user to input command parameters.
				string [] cmdHints = new string[1];
				cmdHints[0] = "Line number to disconnect (integer)";
				if(this.ListBoxLines.Focused) line = Convert.ToInt32(this.ListBoxLines.SelectedItem.ToString().Substring(1, 3));
				CommandParameters cmdParams = new CommandParameters(1, "Please, input parameter for Disconnect command", cmdHints, new string [2] {this._CP.DefaultCommandSender, line.ToString()});
				// Showing it
				cmdParams.ShowDialog();
				// "Cancel" button was clicked or the form was closed - nothing to do.
				if(cmdParams.Get() == null)
				{
					this.AddLineToEventLog(DateTime.Now, String.Concat("Sending Disconnect command: FAILED. Cause: user abort"));
					return;
				}
				// "OK" button was clicked - trying to convert arguments to appropriate type.
				line = Convert.ToInt32((cmdParams.Get())[0]);
				cmdsender = Convert.ToInt32((cmdParams.Get())[4]);
			}
				// Conversion exception.
			catch(FormatException fx)
			{
				this.AddLineToEventLog(DateTime.Now, String.Concat("Sending Disconnect command: FAILED. Cause: invalid parameters: ", fx.Message));
				return;
			}
				// Any other exception.
			catch(Exception ox)
			{
				this.AddLineToEventLog(DateTime.Now, String.Concat("Sending Disconnect command: FAILED. Cause: ", ox.Message));
				return;
			}
			try
			{
				// Creating new command class with user-defined parameters.
				Diacom.Cmd.Disconnect cmd = new Diacom.Cmd.Disconnect(cmdsender, line);
				// Sending this command to AltiGen.
				this._ISP.Send(cmd);
			}
				// Any exception.
			catch(Exception cx)
			{
				this.AddLineToEventLog(DateTime.Now, String.Concat("Sending Disconnect command: FAILED. Cause: ", cx.Message));
				return;
			}
			// Logging all is OK.
			this.AddLineToEventLog(DateTime.Now, String.Concat("Sending Disconnect command: line ", line, ", sender ", cmdsender));
		}

		private void menuItem_PlayDTMF_Click(object sender, System.EventArgs e)
		{
			int line = 0;
			string DTMFCode = "";
			int cmdsender = 0;
			try
			{
				// Creating dialog form for user to input command parameters.
				string [] cmdHints = new string[2];
				cmdHints[0] = "Line number to play DTMF (integer)";
				cmdHints[1] = "DTMF code to play (string)";
				if(this.ListBoxLines.Focused) line = Convert.ToInt32(this.ListBoxLines.SelectedItem.ToString().Substring(1, 3));
				CommandParameters cmdParams = new CommandParameters(2, "Please, input parameters for Play DTMF command", cmdHints, new string [3] {this._CP.DefaultCommandSender, line.ToString(), "0123456789"});
				// Showing it.
				cmdParams.ShowDialog();
				// "Cancel" button was clicked or the form was closed - nothing to do.
				if(cmdParams.Get() == null)
				{
					this.AddLineToEventLog(DateTime.Now, String.Concat("Sending Play DTMF command: FAILED. Cause: user abort"));
					return;
				}
				// "OK" button was clicked - trying to convert arguments to appropriate type.
				line = Convert.ToInt32((cmdParams.Get())[0]);
				DTMFCode = (cmdParams.Get())[1];
				cmdsender = Convert.ToInt32((cmdParams.Get())[4]);
			}
				// Conversion exception.
			catch(FormatException fx)
			{
				this.AddLineToEventLog(DateTime.Now, String.Concat("Sending Play DTMF command: FAILED. Cause: invalid parameters: ", fx.Message));
				return;
			}
				// Any other exception.
			catch(Exception ox)
			{
				this.AddLineToEventLog(DateTime.Now, String.Concat("Sending Play DTMF command: FAILED. Cause: ", ox.Message));
				return;
			}
			try
			{
				// Creating new command class with user-defined parameters.
				Diacom.Cmd.PlayDTMF cmd = new Diacom.Cmd.PlayDTMF(cmdsender, line, DTMFCode);
				// Sending this command to AltiGen.
				this._ISP.Send(cmd);
			}
				// Any exception.
			catch(Exception cx)
			{
				this.AddLineToEventLog(DateTime.Now, String.Concat("Sending Play DTMF command: FAILED. Cause: ", cx.Message));
				return;
			}
			// Logging all is OK.
			this.AddLineToEventLog(DateTime.Now, String.Concat("Sending Play DTMF command: line ", line, ", DTMF ", DTMFCode, ", sender", cmdsender));
		}

		private void menuItem_PlayFile_Click(object sender, System.EventArgs e)
		{
			int line = 0;
			string filePath = "";
			string cutoffString = "";
			int cmdsender = 0;
			try
			{
				// Creating dialog form for user to input command parameters.
				string [] cmdHints = new string[3];
				cmdHints[0] = "Line number to play file for (integer)";
				cmdHints[1] = "Path to the file to play (string)";
				cmdHints[2] = "Cutoff string";
				if(this.ListBoxLines.Focused) line = Convert.ToInt32(this.ListBoxLines.SelectedItem.ToString().Substring(1, 3));
				CommandParameters cmdParams = new CommandParameters(3, "Please, input parameters for Play File command", cmdHints, new string [4] {this._CP.DefaultCommandSender, line.ToString(), "c:\\q", "0123456789"});
				// Showing it.
				cmdParams.ShowDialog();
				// "Cancel" button was clicked or the form was closed - nothing to do.
				if(cmdParams.Get() == null)
				{
					this.AddLineToEventLog(DateTime.Now, String.Concat("Sending Play File command: FAILED. Cause: user abort"));
					return;
				}
				// "OK" button was clicked - trying to convert arguments to appropriate type.
				line = Convert.ToInt32((cmdParams.Get())[0]);
				filePath = (cmdParams.Get())[1];
				cutoffString = (cmdParams.Get())[2];
				cmdsender = Convert.ToInt32((cmdParams.Get())[4]);
			}
				// Conversion exception.
			catch(FormatException fx)
			{
				this.AddLineToEventLog(DateTime.Now, String.Concat("Sending Play File command: FAILED. Cause: invalid parameters: ", fx.Message));
				return;
			}
				// Any other exception.
			catch(Exception ox)
			{
				this.AddLineToEventLog(DateTime.Now, String.Concat("Sending Play File command: FAILED. Cause: ", ox.Message));
				return;
			}
			try
			{
				// Creating new command class with user-defined parameters.
				Diacom.Cmd.PlayFile cmd = new Diacom.Cmd.PlayFile(cmdsender, line, filePath, cutoffString);
				// Sending this command to AltiGen.
				this._ISP.Send(cmd);
			}
				// Any exception.
			catch(Exception cx)
			{
				this.AddLineToEventLog(DateTime.Now, String.Concat("Sending Play File command: FAILED. Cause: ", cx.Message));
				return;
			}
			// Logging all is OK.
			this.AddLineToEventLog(DateTime.Now, String.Concat("Sending Play File command: line ", line, ", file path ", filePath, ", sender ", cmdsender));
		}

		private void menuItem_RecordFile_Click(object sender, System.EventArgs e)
		{
			int line = 0;
			string filePath = "";
			string cutoffString = "";
			int cmdsender = 0;
			try
			{
				// Creating dialog form for user to input command parameters.
				string [] cmdHints = new string[3];
				cmdHints[0] = "Line number to record file for (integer)";
				cmdHints[1] = "Path to the file to which it should be recorded (string)";
				cmdHints[2] = "Cutoff string";
				if(this.ListBoxLines.Focused) line = Convert.ToInt32(this.ListBoxLines.SelectedItem.ToString().Substring(1, 3));
				CommandParameters cmdParams = new CommandParameters(3, "Please, input parameters for Record File command", cmdHints, new string [4] {this._CP.DefaultCommandSender, line.ToString(), "c:\\q", "0123456789"});
				// Showing it.
				cmdParams.ShowDialog();
				// "Cancel" button was clicked or the form was closed - nothing to do.
				if(cmdParams.Get() == null)
				{
					this.AddLineToEventLog(DateTime.Now, String.Concat("Sending Record File command: FAILED. Cause: user abort"));
					return;
				}
				// "OK" button was clicked - trying to convert arguments to appropriate type.
				line = Convert.ToInt32((cmdParams.Get())[0]);
				filePath = (cmdParams.Get())[1];
				cutoffString = (cmdParams.Get())[2];
				cmdsender = Convert.ToInt32((cmdParams.Get())[4]);
			}
				// Conversion exception.
			catch(FormatException fx)
			{
				this.AddLineToEventLog(DateTime.Now, String.Concat("Sending Record File command: FAILED. Cause: invalid parameters: ", fx.Message));
				return;
			}
				// Any other exception.
			catch(Exception ox)
			{
				this.AddLineToEventLog(DateTime.Now, String.Concat("Sending Record File command: FAILED. Cause: ", ox.Message));
				return;
			}
			try
			{
				// Creating new command class with user-defined parameters.
				Diacom.Cmd.RecordFile cmd = new Diacom.Cmd.RecordFile(cmdsender, line, filePath, cutoffString, 0);
				// Sending this command to AltiGen.
				this._ISP.Send(cmd);
			}
				// Any exception.
			catch(Exception cx)
			{
				this.AddLineToEventLog(DateTime.Now, String.Concat("Sending Record File command: FAILED. Cause: ", cx.Message));
				return;
			}
			// Logging all is OK.
			this.AddLineToEventLog(DateTime.Now, String.Concat("Sending Record File command: line ", line, ", file path ", filePath, ", cutoff string ", cutoffString, ", sender ", cmdsender));
		}

		private void menuItem_Reset_Click(object sender, System.EventArgs e)
		{
			int line = 0;
			int cmdsender = 0;
			try
			{
				// Creating dialog form for user to input command parameters.
				string [] cmdHints = new string[1];
				cmdHints[0] = "Line number to reset (integer)";
				if(this.ListBoxLines.Focused) line = Convert.ToInt32(this.ListBoxLines.SelectedItem.ToString().Substring(1, 3));
				CommandParameters cmdParams = new CommandParameters(1, "Please, input parameter for Reset command", cmdHints ,new string [2] {this._CP.DefaultCommandSender, line.ToString()});
				// Showing it.
				cmdParams.ShowDialog();
				// "Cancel" button was clicked or the form was closed - nothing to do.
				if(cmdParams.Get() == null)
				{
					this.AddLineToEventLog(DateTime.Now, String.Concat("Sending Reset command: FAILED. Cause: user abort"));
					return;
				}
				// "OK" button was clicked - trying to convert arguments to appropriate type.
				line = Convert.ToInt32((cmdParams.Get())[0]);
				cmdsender = Convert.ToInt32((cmdParams.Get())[4]);
			}
				// Conversion exception.
			catch(FormatException fx)
			{
				this.AddLineToEventLog(DateTime.Now, String.Concat("Sending Reset command: FAILED. Cause: invalid parameters: ", fx.Message));
				return;
			}
				// Any other exception.
			catch(Exception ox)
			{
				this.AddLineToEventLog(DateTime.Now, String.Concat("Sending Reset command: FAILED. Cause: ", ox.Message));
				return;
			}
			try
			{
				// Creating new command class with user-defined parameters.
				Diacom.Cmd.Reset cmd = new Diacom.Cmd.Reset(cmdsender, line);
				// Sending this command to AltiGen.
				this._ISP.Send(cmd);
			}
				// Any exception.
			catch(Exception cx)
			{
				this.AddLineToEventLog(DateTime.Now, String.Concat("Sending Reset command: FAILED. Cause: ", cx.Message));
				return;
			}
			// Logging all is OK.
			this.AddLineToEventLog(DateTime.Now, String.Concat("Sending Reset command: line ", line, ", sender ", cmdsender));
		}

		private void menuItem_RingExt_Click(object sender, System.EventArgs e)
		{
			string line = "";
			int cmdsender = 0;
			try
			{
				// Creating dialog form for user to input command parameters.
				string [] cmdHints = new string[1];
				cmdHints[0] = "Extension line to ring (integer)";
				if(this.ListBoxLines.Focused) line = this.ListBoxLines.SelectedItem.ToString().Substring(0, 4);
				CommandParameters cmdParams = new CommandParameters(1, "Please, input parameter for Ring Extension command", cmdHints, new string [2] {this._CP.DefaultCommandSender, line.ToString()});
				// Showing it.
				cmdParams.ShowDialog();
				// "Cancel" button was clicked or the form was closed - nothing to do.
				if(cmdParams.Get() == null)
				{
					this.AddLineToEventLog(DateTime.Now, String.Concat("Sending Ring Extension command: FAILED. Cause: user abort"));
					return;
				}
				// "OK" button was clicked - trying to convert arguments to appropriate type.
				line =(cmdParams.Get())[0];
				cmdsender = Convert.ToInt32((cmdParams.Get())[4]);
			}
				// Conversion exception.
			catch(FormatException fx)
			{
				this.AddLineToEventLog(DateTime.Now, String.Concat("Sending Ring Extension command: FAILED. Cause: invalid parameters: ", fx.Message));
				return;
			}
				// Any other exception.
			catch(Exception ox)
			{
				this.AddLineToEventLog(DateTime.Now, String.Concat("Sending Ring Extension command: FAILED. Cause: ", ox.Message));
				return;
			}
			try
			{
				// Creating new command class with user-defined parameters.
				Diacom.Cmd.RingExtension cmd = new Diacom.Cmd.RingExtension(cmdsender,  Convert.ToInt32(line), "", 0);
				// Sending this command to AltiGen.
				this._ISP.Send(cmd);
			}
				// Any exception.
			catch(Exception cx)
			{
				this.AddLineToEventLog(DateTime.Now, String.Concat("Sending Ring Extension command: FAILED. Cause: ", cx.Message));
				return;
			}
			// Logging all is OK.
			this.AddLineToEventLog(DateTime.Now, String.Concat("Sending Ring Extension command: ext ", line, ", sender ", cmdsender));
		}

		private void menuItem_Snatch_Click(object sender, System.EventArgs e)
		{
			int line = 0;
			int cmdsender = 0;
			try
			{
				// Creating dialog form for user to input command parameters.
				string [] cmdHints = new string[1];
				cmdHints[0] = "Line to snatch (integer)";
				if(this.ListBoxLines.Focused) line = Convert.ToInt32(this.ListBoxLines.SelectedItem.ToString().Substring(1, 3));
				CommandParameters cmdParams = new CommandParameters(1, "Please, input parameter for Snatch Line command", cmdHints, new string [2] {this._CP.DefaultCommandSender, line.ToString()});
				// Showing it.
				cmdParams.ShowDialog();
				// "Cancel" button was clicked or the form was closed - nothing to do.
				if(cmdParams.Get() == null)
				{
					this.AddLineToEventLog(DateTime.Now, String.Concat("Sending Snatch Line command: FAILED. Cause: user abort"));
					return;
				}
				// "OK" button was clicked - trying to convert arguments to appropriate type.
				line = Convert.ToInt32((cmdParams.Get())[0]);
				cmdsender = Convert.ToInt32((cmdParams.Get())[4]);

			}
				// Conversion exception.
			catch(FormatException fx)
			{
				this.AddLineToEventLog(DateTime.Now, String.Concat("Sending Snatch Line command: FAILED. Cause: invalid parameters: ", fx.Message));
				return;
			}
				// Any other exception.
			catch(Exception ox)
			{
				this.AddLineToEventLog(DateTime.Now, String.Concat("Sending Snatch Line command: FAILED. Cause: ", ox.Message));
				return;
			}
			try
			{
				// Creating new command class with user-defined parameters.
				Diacom.Cmd.SnatchLine cmd = new Diacom.Cmd.SnatchLine(cmdsender, line);
				// Sending this command to AltiGen.
				this._ISP.Send(cmd);
			}
				// Any exception.
			catch(Exception cx)
			{
				this.AddLineToEventLog(DateTime.Now, String.Concat("Sending Snatch Line command: FAILED. Cause: ", cx.Message));
				return;
			}
			// Logging all is OK.
			this.AddLineToEventLog(DateTime.Now, String.Concat("Sending Snatch Line command: ext ", line, ", sender ", cmdsender));
		}

		private void menuItem_SwitchMusic_Click(object sender, System.EventArgs e)
		{
			int line = 0;
			int mode = 0;
			int cmdsender = 0;
			try
			{
				// Creating dialog form for user to input command parameters.
				string [] cmdHints = new string[2];
				cmdHints[0] = "Line number to switch music (integer)";
				cmdHints[1] = "Mode (0 - music on, 1 - off) (integer)";
				if(this.ListBoxLines.Focused) line = Convert.ToInt32(this.ListBoxLines.SelectedItem.ToString().Substring(1, 3));
				CommandParameters cmdParams = new CommandParameters(2, "Please, input parameters for Switch Music command", cmdHints, new string [3] {this._CP.DefaultCommandSender, line.ToString(), "0"});
				// Showing it.
				cmdParams.ShowDialog();
				// "Cancel" button was clicked or the form was closed - nothing to do.
				if(cmdParams.Get() == null)
				{
					this.AddLineToEventLog(DateTime.Now, String.Concat("Sending Switch Music command: FAILED. Cause: user abort"));
					return;
				}
				// "OK" button was clicked - trying to convert arguments to appropriate type.
				line = Convert.ToInt32((cmdParams.Get())[0]);
				mode = Convert.ToInt32((cmdParams.Get())[1]);
				cmdsender = Convert.ToInt32((cmdParams.Get())[4]);
			}
				// Conversion exception.
			catch(FormatException fx)
			{
				this.AddLineToEventLog(DateTime.Now, String.Concat("Sending Switch Music command: FAILED. Cause: invalid parameters: ", fx.Message));
				return;
			}
				// Any other exception.
			catch(Exception ox)
			{
				this.AddLineToEventLog(DateTime.Now, String.Concat("Sending Switch Music command: FAILED. Cause: ", ox.Message));
				return;
			}
			try
			{
				// Creating new command class with user-defined parameters.
				Diacom.Cmd.SwitchMusic cmd = new Diacom.Cmd.SwitchMusic(cmdsender, line, mode);
				// Sending this command to AltiGen.
				this._ISP.Send(cmd);
			}
				// Any exception.
			catch(Exception cx)
			{
				this.AddLineToEventLog(DateTime.Now, String.Concat("Sending Switch Music command: FAILED. Cause: ", cx.Message));
				return;
			}
			// Logging all is OK.
			this.AddLineToEventLog(DateTime.Now, String.Concat("Sending Switch Music command: line ", line, ", mode ", mode, ", sender ", cmdsender));
		}

		private void menuItem_Transfer_Click(object sender, System.EventArgs e)
		{
			int line = 0;
			string destination = "";
			int cmdsender = 0;
			Diacom.Cmd.TransferCallType type;
			try
			{
				// Creating dialog form for user to input command parameters.
				string [] cmdHints = new string[3];
				cmdHints[0] = "Line number to transfer (integer)";
				cmdHints[1] = "Destination (string or integer, if extension)";
				cmdHints[2] = "Type of transfer (0 - AA, 1 and 2 - ext., 3 - op., 4 - trunk)";
				if(this.ListBoxLines.Focused) line = Convert.ToInt32(this.ListBoxLines.SelectedItem.ToString().Substring(1, 3));
				CommandParameters cmdParams = new CommandParameters(3, "Please, input parameters for Transfer Call command", cmdHints, new string [4] {this._CP.DefaultCommandSender, line.ToString(), this._CP.DefaultCommandSender, "1"});
				// Showing it.
				cmdParams.ShowDialog();
				// "Cancel" button was clicked or the form was closed - nothing to do.
				if(cmdParams.Get() == null)
				{
					this.AddLineToEventLog(DateTime.Now, String.Concat("Sending Switch Music command: FAILED. Cause: user abort"));
					return;
				}
				// "OK" button was clicked - trying to convert arguments to appropriate type.
				line = Convert.ToInt32((cmdParams.Get())[0]);
				destination = (string)((cmdParams.Get())[1]);
				type = (Diacom.Cmd.TransferCallType)(Convert.ToInt32((cmdParams.Get())[2]));
				cmdsender = Convert.ToInt32((cmdParams.Get())[4]);
			}
				// Conversion exception.
			catch(FormatException fx)
			{
				this.AddLineToEventLog(DateTime.Now, String.Concat("Sending Transfer Call command: FAILED. Cause: invalid parameters: ", fx.Message));
				return;
			}
				// Any other exception.
			catch(Exception ox)
			{
				this.AddLineToEventLog(DateTime.Now, String.Concat("Sending Transfer Call command: FAILED. Cause: ", ox.Message));
				return;
			}
			try
			{
				// Creating new command class with user-defined parameters.
				//				Diacom.Cmd.TransferCall cmd = new Diacom.Cmd.TransferCall(cmdsender, line, destination, type);
				// Sending this command to AltiGen.
				//				this._ISP.Send(cmd);
			}
				// Any exception.
			catch(Exception cx)
			{
				this.AddLineToEventLog(DateTime.Now, String.Concat("Sending Transfer Call command: FAILED. Cause: ", cx.Message));
				return;
			}
			// Logging all is OK.
			this.AddLineToEventLog(DateTime.Now, String.Concat("Sending Transfer Call command: line ", line, ", destination ", destination, ", type ", type, ", sender ", cmdsender));
		}

		private void menuItem_CloseConnection_Click(object sender, System.EventArgs e)
		{
			this._ISP.Disconnect();
			this.Close();
		}

		private void menuItem_About_Click(object sender, System.EventArgs e)
		{
			((APCLinesListenerMDIParent)(this.MdiParent)).About(sender, e);
		}
	}
}
