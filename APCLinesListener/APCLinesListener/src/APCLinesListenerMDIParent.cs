using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Threading;
using System.Text;
using System.IO;

namespace APCLinesListener
{
	/// <summary>
	/// Implements the main (MDI Parent) window class.
	/// </summary>
	public class APCLinesListenerMDIParent : System.Windows.Forms.Form
	{
		private System.Windows.Forms.MainMenu APCLinesListenerMainMenu;
		private System.Windows.Forms.MenuItem menuItemNewConnection;
		private System.Windows.Forms.MenuItem menuItemWindowList;
		private System.Windows.Forms.MenuItem menuItem2;
		private System.Windows.Forms.MenuItem menuItem3;
		private System.Windows.Forms.MenuItem menuItem4;
		private System.Windows.Forms.MenuItem menuItem6;
		private System.Windows.Forms.MenuItem menuItemSaveLogs;
		private System.Windows.Forms.MenuItem menuItemExit;
		private System.Windows.Forms.MenuItem menuItemTraceOutPut;
		private System.Windows.Forms.MenuItem menuItem1;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		private void APCLinesListenerMDIParent_Load(object sender, EventArgs e)
		{
			APCLinesListenerMDITrace mt = new APCLinesListenerMDITrace();
			mt.MdiParent = this;
			mt.Show();
			this.menuItemNewConnection_Click(sender, e);
		}

		/// <summary>
		/// Creates new instance of the class.
		/// </summary>
		public APCLinesListenerMDIParent()
		{
			InitializeComponent();
		}

		/// <summary>
		/// Cleans up any resources being used.
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
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(APCLinesListenerMDIParent));
			this.APCLinesListenerMainMenu = new System.Windows.Forms.MainMenu();
			this.menuItem3 = new System.Windows.Forms.MenuItem();
			this.menuItemNewConnection = new System.Windows.Forms.MenuItem();
			this.menuItemTraceOutPut = new System.Windows.Forms.MenuItem();
			this.menuItem1 = new System.Windows.Forms.MenuItem();
			this.menuItemSaveLogs = new System.Windows.Forms.MenuItem();
			this.menuItem6 = new System.Windows.Forms.MenuItem();
			this.menuItemExit = new System.Windows.Forms.MenuItem();
			this.menuItemWindowList = new System.Windows.Forms.MenuItem();
			this.menuItem4 = new System.Windows.Forms.MenuItem();
			this.menuItem2 = new System.Windows.Forms.MenuItem();
			// 
			// APCLinesListenerMainMenu
			// 
			this.APCLinesListenerMainMenu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																									 this.menuItem3,
																									 this.menuItemWindowList,
																									 this.menuItem4});
			// 
			// menuItem3
			// 
			this.menuItem3.Index = 0;
			this.menuItem3.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																					  this.menuItemNewConnection,
																					  this.menuItemTraceOutPut,
																					  this.menuItem1,
																					  this.menuItemSaveLogs,
																					  this.menuItem6,
																					  this.menuItemExit});
			this.menuItem3.Text = "&File";
			// 
			// menuItemNewConnection
			// 
			this.menuItemNewConnection.Index = 0;
			this.menuItemNewConnection.Text = "&New Connection";
			this.menuItemNewConnection.Click += new System.EventHandler(this.menuItemNewConnection_Click);
			// 
			// menuItemTraceOutPut
			// 
			this.menuItemTraceOutPut.Index = 1;
			this.menuItemTraceOutPut.Text = "&Trace Output";
			this.menuItemTraceOutPut.Click += new System.EventHandler(this.menuItemTraceOutPut_Click);
			// 
			// menuItem1
			// 
			this.menuItem1.Index = 2;
			this.menuItem1.Text = "-";
			// 
			// menuItemSaveLogs
			// 
			this.menuItemSaveLogs.Checked = true;
			this.menuItemSaveLogs.Index = 3;
			this.menuItemSaveLogs.Text = "&Save Logs On Exit";
			// 
			// menuItem6
			// 
			this.menuItem6.Index = 4;
			this.menuItem6.Text = "-";
			// 
			// menuItemExit
			// 
			this.menuItemExit.Index = 5;
			this.menuItemExit.Text = "E&xit";
			this.menuItemExit.Click += new System.EventHandler(this.menuItemExit_Click);
			// 
			// menuItemWindowList
			// 
			this.menuItemWindowList.Index = 1;
			this.menuItemWindowList.MdiList = true;
			this.menuItemWindowList.Text = "&Window";
			// 
			// menuItem4
			// 
			this.menuItem4.Index = 2;
			this.menuItem4.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																					  this.menuItem2});
			this.menuItem4.Text = "&Help";
			// 
			// menuItem2
			// 
			this.menuItem2.Index = 0;
			this.menuItem2.Text = "&About";
			this.menuItem2.Click += new System.EventHandler(this.About);
			// 
			// APCLinesListenerMDIParent
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(680, 509);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.IsMdiContainer = true;
			this.Menu = this.APCLinesListenerMainMenu;
			this.Name = "APCLinesListenerMDIParent";
			this.Text = "APC Lines Listener";
			this.Closing += new System.ComponentModel.CancelEventHandler(this.APCLinesListenerMDIParent_Closing);
			this.MdiChildActivate += new System.EventHandler(this.APCLinesListenerMDIParent_MdiChildActivate);
			this.Load += new System.EventHandler(this.APCLinesListenerMDIParent_Load);

		}
		#endregion

		private void APCLinesListenerMDIParent_MdiChildActivate(object sender, EventArgs e)
		{
//			this.Text = this.ActiveMdiChild.Text;
		}

		ConnectionPropertiesForm cpf = null;

		private void ConnectionPropertiesFormClosed(object sender, EventArgs e)
		{
			if(this.cpf != null)
			{
				if(this.cpf.DialogResult.Equals(DialogResult.OK))
				{
					APCLinesListenerMDIConnection mc = new APCLinesListenerMDIConnection(this.cpf.Properties);
					mc.MdiParent = this;
					mc.Show();
				}
			}
		}

		private void menuItemNewConnection_Click(object sender, System.EventArgs e)
		{
			this.cpf = new ConnectionPropertiesForm();
			this.cpf.TopMost = true;
			this.cpf.StartPosition = FormStartPosition.CenterScreen;
			this.cpf.Closed += new EventHandler(this.ConnectionPropertiesFormClosed);
			this.cpf.ShowDialog();
		}

		private void SaveLogs(object sender, System.EventArgs e)
		{
			string [] Outputs = new string [this.MdiChildren.Length];
			string [] OutputTitles = new string [this.MdiChildren.Length];
			for(int i = 0; i < this.MdiChildren.Length; i++)
			{
				if(this.MdiChildren[i] is APCLinesListenerMDIConnection)
				{
					APCLinesListenerMDIConnection form = ((APCLinesListenerMDIConnection)(this.MdiChildren[i]));
					OutputTitles[i] = form.Text;
					string text = "<font color=\"#00ff00\">Lines:</font>"+Environment.NewLine+"<hr>"+Environment.NewLine+"<font color=\"#ffff00\">"+Environment.NewLine;
					foreach(string line in form.ListBoxLines.Items) text += line+Environment.NewLine;
					text += Environment.NewLine+"</font>"+Environment.NewLine+"<hr>"+Environment.NewLine;
					text += "<font color=\"#00ff00\">Events:</font>"+Environment.NewLine+"<hr>"+Environment.NewLine+"<font color=\"#00ffff\">"+Environment.NewLine+form.richTextBoxEvents.Text+Environment.NewLine+"</font>";
					Outputs[i] = text;
				}
				else if(this.MdiChildren[i] is APCLinesListenerMDITrace)
				{
					OutputTitles[i] = "Trace Output:";
					Outputs[i] = ((APCLinesListenerMDITrace)(this.MdiChildren[i])).richTextBoxTraceOutPut.Text;
				}
			}
			Diacom.AltiGen.ConnectionData.Save(OutputTitles, Outputs);
		}

		private void menuItemExit_Click(object sender, System.EventArgs e)
		{
			this.Close();
		}

		/// <summary>
		/// Shows information about this app.
		/// </summary>
		/// <param name="sender">Sender of the click event.</param>
		/// <param name="e">Event parameters.</param>
		public void About(object sender, System.EventArgs e)
		{
			string tlt = "About APC Lines Listener";
			string msg = "APC Lines Listener"+Environment.NewLine+"Copyright® 2004 OOO \"Diacom\""+Environment.NewLine+"127434, Moscow, Russia,"+Environment.NewLine+"Dmitrovskoe Shosse, 2."+Environment.NewLine+"Tel: +7(095) 777-9698"+Environment.NewLine+"voip.dia.com.ru"+Environment.NewLine+"http://www.dia.com.ru";
			MessageBox.Show(msg, tlt, MessageBoxButtons.OK, MessageBoxIcon.Information);
		}

		private void menuItemTraceOutPut_Click(object sender, System.EventArgs e)
		{
			bool TheWindowExists = false;
			foreach(Form f in this.MdiChildren)
			{
				if(f.Name.Equals("APCLinesListenerMDITrace"))
				{
					TheWindowExists = true;
					f.Focus();
				}
			}
			if(!TheWindowExists)
			{
				APCLinesListenerMDITrace mt = new APCLinesListenerMDITrace();
				mt.MdiParent = this;
				mt.Show();
			}
		}

		private void APCLinesListenerMDIParent_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			if(this.menuItemSaveLogs.Checked) this.SaveLogs(sender, e);
		}
	}
}
