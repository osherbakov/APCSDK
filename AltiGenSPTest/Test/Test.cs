using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;

namespace Diacom
{
	/// <summary>
	/// Summary description for Form1.
	/// </summary>
	public class Form1 : System.Windows.Forms.Form
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public Form1()
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
				if (components != null) 
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
			this.listBox1 = new System.Windows.Forms.ListBox();
			this.textBox1 = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.button1 = new System.Windows.Forms.Button();
			this.textBox2 = new System.Windows.Forms.TextBox();
			this.textBox3 = new System.Windows.Forms.TextBox();
			this.button2 = new System.Windows.Forms.Button();
			this.button3 = new System.Windows.Forms.Button();
			this.comboBox1 = new System.Windows.Forms.ComboBox();
			this.button4 = new System.Windows.Forms.Button();
			this.textBox4 = new System.Windows.Forms.TextBox();
			this.textBox5 = new System.Windows.Forms.TextBox();
			this.button5 = new System.Windows.Forms.Button();
			this.textBox6 = new System.Windows.Forms.TextBox();
			this.button6 = new System.Windows.Forms.Button();
			this.button7 = new System.Windows.Forms.Button();
			this.button8 = new System.Windows.Forms.Button();
			this.button9 = new System.Windows.Forms.Button();
			this.button10 = new System.Windows.Forms.Button();
			this.textBox7 = new System.Windows.Forms.TextBox();
			this.button11 = new System.Windows.Forms.Button();
			this.button12 = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// listBox1
			// 
			this.listBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.listBox1.Font = new System.Drawing.Font("Lucida Console", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(204)));
			this.listBox1.HorizontalScrollbar = true;
			this.listBox1.ItemHeight = 11;
			this.listBox1.Location = new System.Drawing.Point(8, 32);
			this.listBox1.Name = "listBox1";
			this.listBox1.Size = new System.Drawing.Size(400, 356);
			this.listBox1.TabIndex = 0;
			// 
			// textBox1
			// 
			this.textBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.textBox1.Location = new System.Drawing.Point(416, 32);
			this.textBox1.Name = "textBox1";
			this.textBox1.Size = new System.Drawing.Size(224, 20);
			this.textBox1.TabIndex = 1;
			this.textBox1.Text = "textBox1";
			// 
			// label1
			// 
			this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label1.Location = new System.Drawing.Point(416, 8);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(100, 16);
			this.label1.TabIndex = 2;
			this.label1.Text = "Status:";
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(8, 8);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(100, 16);
			this.label2.TabIndex = 3;
			this.label2.Text = "Lines:";
			// 
			// button1
			// 
			this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.button1.Location = new System.Drawing.Point(656, 64);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(104, 23);
			this.button1.TabIndex = 4;
			this.button1.Text = "Get User Data";
			this.button1.Click += new System.EventHandler(this.button1_Click);
			// 
			// textBox2
			// 
			this.textBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.textBox2.Location = new System.Drawing.Point(416, 64);
			this.textBox2.Multiline = true;
			this.textBox2.Name = "textBox2";
			this.textBox2.Size = new System.Drawing.Size(224, 20);
			this.textBox2.TabIndex = 5;
			this.textBox2.Text = "textBox2";
			// 
			// textBox3
			// 
			this.textBox3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.textBox3.Location = new System.Drawing.Point(416, 128);
			this.textBox3.Name = "textBox3";
			this.textBox3.Size = new System.Drawing.Size(224, 20);
			this.textBox3.TabIndex = 6;
			this.textBox3.Text = "textBox3";
			// 
			// button2
			// 
			this.button2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.button2.Location = new System.Drawing.Point(656, 128);
			this.button2.Name = "button2";
			this.button2.Size = new System.Drawing.Size(104, 23);
			this.button2.TabIndex = 7;
			this.button2.Text = "Get Agent State";
			this.button2.Click += new System.EventHandler(this.button2_Click);
			// 
			// button3
			// 
			this.button3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.button3.Location = new System.Drawing.Point(656, 160);
			this.button3.Name = "button3";
			this.button3.Size = new System.Drawing.Size(104, 23);
			this.button3.TabIndex = 9;
			this.button3.Text = "Set Agent State";
			this.button3.Click += new System.EventHandler(this.button3_Click);
			// 
			// comboBox1
			// 
			this.comboBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.comboBox1.Items.AddRange(new object[] {
														   "READY",
														   "NOT_READY"});
			this.comboBox1.Location = new System.Drawing.Point(416, 160);
			this.comboBox1.Name = "comboBox1";
			this.comboBox1.Size = new System.Drawing.Size(224, 21);
			this.comboBox1.TabIndex = 10;
			this.comboBox1.Text = "NOT_READY";
			// 
			// button4
			// 
			this.button4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.button4.Location = new System.Drawing.Point(656, 96);
			this.button4.Name = "button4";
			this.button4.Size = new System.Drawing.Size(104, 23);
			this.button4.TabIndex = 12;
			this.button4.Text = "Set User Data";
			this.button4.Click += new System.EventHandler(this.button4_Click);
			// 
			// textBox4
			// 
			this.textBox4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.textBox4.Location = new System.Drawing.Point(416, 96);
			this.textBox4.Multiline = true;
			this.textBox4.Name = "textBox4";
			this.textBox4.Size = new System.Drawing.Size(224, 20);
			this.textBox4.TabIndex = 11;
			this.textBox4.Text = "textBox4";
			// 
			// textBox5
			// 
			this.textBox5.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.textBox5.Location = new System.Drawing.Point(416, 192);
			this.textBox5.Name = "textBox5";
			this.textBox5.Size = new System.Drawing.Size(224, 20);
			this.textBox5.TabIndex = 14;
			this.textBox5.Text = "textBox5";
			// 
			// button5
			// 
			this.button5.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.button5.Location = new System.Drawing.Point(656, 192);
			this.button5.Name = "button5";
			this.button5.Size = new System.Drawing.Size(104, 23);
			this.button5.TabIndex = 13;
			this.button5.Text = "Make Call";
			this.button5.Click += new System.EventHandler(this.button5_Click);
			// 
			// textBox6
			// 
			this.textBox6.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.textBox6.Location = new System.Drawing.Point(416, 256);
			this.textBox6.Multiline = true;
			this.textBox6.Name = "textBox6";
			this.textBox6.Size = new System.Drawing.Size(224, 20);
			this.textBox6.TabIndex = 16;
			this.textBox6.Text = "textBox6";
			// 
			// button6
			// 
			this.button6.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.button6.Location = new System.Drawing.Point(656, 256);
			this.button6.Name = "button6";
			this.button6.Size = new System.Drawing.Size(104, 23);
			this.button6.TabIndex = 15;
			this.button6.Text = "Get IVR data";
			this.button6.Click += new System.EventHandler(this.button6_Click);
			// 
			// button7
			// 
			this.button7.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.button7.Location = new System.Drawing.Point(416, 288);
			this.button7.Name = "button7";
			this.button7.Size = new System.Drawing.Size(168, 23);
			this.button7.TabIndex = 17;
			this.button7.Text = "Start Call Recording";
			this.button7.Click += new System.EventHandler(this.button7_Click);
			// 
			// button8
			// 
			this.button8.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.button8.Location = new System.Drawing.Point(592, 288);
			this.button8.Name = "button8";
			this.button8.Size = new System.Drawing.Size(168, 23);
			this.button8.TabIndex = 18;
			this.button8.Text = "Stop Call Recording";
			this.button8.Click += new System.EventHandler(this.button8_Click);
			// 
			// button9
			// 
			this.button9.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.button9.Location = new System.Drawing.Point(592, 320);
			this.button9.Name = "button9";
			this.button9.Size = new System.Drawing.Size(168, 23);
			this.button9.TabIndex = 20;
			this.button9.Text = "Drop Call";
			this.button9.Click += new System.EventHandler(this.button9_Click);
			// 
			// button10
			// 
			this.button10.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.button10.Location = new System.Drawing.Point(416, 320);
			this.button10.Name = "button10";
			this.button10.Size = new System.Drawing.Size(168, 23);
			this.button10.TabIndex = 19;
			this.button10.Text = "Answer Call";
			this.button10.Click += new System.EventHandler(this.button10_Click);
			// 
			// textBox7
			// 
			this.textBox7.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.textBox7.Location = new System.Drawing.Point(416, 224);
			this.textBox7.Name = "textBox7";
			this.textBox7.Size = new System.Drawing.Size(224, 20);
			this.textBox7.TabIndex = 22;
			this.textBox7.Text = "textBox7";
			// 
			// button11
			// 
			this.button11.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.button11.Location = new System.Drawing.Point(656, 224);
			this.button11.Name = "button11";
			this.button11.Size = new System.Drawing.Size(104, 23);
			this.button11.TabIndex = 21;
			this.button11.Text = "Redirect Call";
			this.button11.Click += new System.EventHandler(this.button11_Click);
			// 
			// button12
			// 
			this.button12.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.button12.Location = new System.Drawing.Point(672, 368);
			this.button12.Name = "button12";
			this.button12.Size = new System.Drawing.Size(88, 23);
			this.button12.TabIndex = 23;
			this.button12.Text = "&Quit";
			this.button12.Click += new System.EventHandler(this.button12_Click);
			// 
			// Form1
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(770, 397);
			this.Controls.Add(this.button12);
			this.Controls.Add(this.textBox7);
			this.Controls.Add(this.button11);
			this.Controls.Add(this.button9);
			this.Controls.Add(this.button10);
			this.Controls.Add(this.button8);
			this.Controls.Add(this.button7);
			this.Controls.Add(this.textBox6);
			this.Controls.Add(this.button6);
			this.Controls.Add(this.textBox5);
			this.Controls.Add(this.button5);
			this.Controls.Add(this.button4);
			this.Controls.Add(this.textBox4);
			this.Controls.Add(this.comboBox1);
			this.Controls.Add(this.button3);
			this.Controls.Add(this.button2);
			this.Controls.Add(this.textBox3);
			this.Controls.Add(this.textBox2);
			this.Controls.Add(this.button1);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.textBox1);
			this.Controls.Add(this.listBox1);
			this.Name = "Form1";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "AltiGen SP for extension test";
			this.Load += new System.EventHandler(this.Form1_Load);
			this.ResumeLayout(false);

		}
		#endregion

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main() 
		{
			Application.Run(new Form1());
		}

		private System.Windows.Forms.ListBox listBox1;
		private System.Windows.Forms.TextBox textBox1;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Button button1;
		private System.Windows.Forms.TextBox textBox2;
		private System.Windows.Forms.TextBox textBox3;
		private System.Windows.Forms.Button button2;
		private System.Windows.Forms.Button button3;
		private System.Windows.Forms.ComboBox comboBox1;
		private System.Windows.Forms.Button button4;
		private System.Windows.Forms.TextBox textBox4;
		private System.Windows.Forms.TextBox textBox5;
		private System.Windows.Forms.Button button5;
		private System.Windows.Forms.TextBox textBox6;
		private System.Windows.Forms.Button button6;
		private System.Windows.Forms.Button button7;
		private System.Windows.Forms.Button button8;
		private System.Windows.Forms.Button button9;
		private System.Windows.Forms.Button button10;
		private System.Windows.Forms.TextBox textBox7;
		private System.Windows.Forms.Button button11;
		private System.Windows.Forms.Button button12;

		private IExtension X = null;
		private void Form1_Load(object sender, System.EventArgs e)
		{
			this.X = new Diacom.AltiGen.ASPHdw();
			this.X.LogOn("192.168.1.3", 10025, "119", "2003");
			this.X.ExtensionStatusChanged += new ExtensionStatusChangedEventHandler(X_ExtensionStatusChanged);
			this.X.Call += new CallEventHandler(X_Call);
			foreach(SPLine line in this.X.GetLines())
			{
				this.listBox1.Items.Add(line.ToString());
			}
		}

		private void X_ExtensionStatusChanged(string State)
		{
			this.textBox1.Text = State;
		}
		private void button1_Click(object sender, System.EventArgs e)
		{
			this.textBox2.Text = this.X.GetUserData();
		}

		private void button2_Click(object sender, System.EventArgs e)
		{
			this.textBox3.Text = this.X.GetAgentState().ToString();
		}

		private void button3_Click(object sender, System.EventArgs e)
		{
			try
			{
				Diacom.Cmd.AgentState s = ((Diacom.Cmd.AgentState)(this.comboBox1.SelectedIndex));
				this.X.SetAgentState(s);
			}
			catch
			{
			}
		}

		private void button4_Click(object sender, System.EventArgs e)
		{
			this.X.SetUserData(this.textBox4.Text);
		}

		private void button5_Click(object sender, System.EventArgs e)
		{
			this.X.MakeCall(this.textBox5.Text, "", "");
		}

		private void button6_Click(object sender, System.EventArgs e)
		{
			this.textBox6.Text = this.X.GetIVR();
		}

		private void button7_Click(object sender, System.EventArgs e)
		{
			this.X.StartCallRecord();
		}

		private void button8_Click(object sender, System.EventArgs e)
		{
			this.X.StopCallRecord();
		}

		private void button10_Click(object sender, System.EventArgs e)
		{
			this.X.AnswerCall();
		}

		private void button9_Click(object sender, System.EventArgs e)
		{
			this.X.DropCall();
		}

		private void button11_Click(object sender, System.EventArgs e)
		{
			this.X.Redirect(this.textBox7.Text, "");
		}

		private void button12_Click(object sender, System.EventArgs e)
		{
			this.Close();
		}


	}
}
