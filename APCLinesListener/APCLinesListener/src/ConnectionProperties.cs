using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace APCLinesListener
{
	/// <summary>
	/// Summary description for ConnectionProperties.
	/// </summary>
	public class ConnectionPropertiesForm : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.Panel panel2;
		private System.Windows.Forms.Button ButtonConnect;
		private System.Windows.Forms.PropertyGrid propertyGrid1;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		private ConnectionProperties _ConnectionProperties = null;

		/// <summary>
		/// Gets properties for connection.
		/// </summary>
		public ConnectionProperties Properties
		{
			get
			{
				return this._ConnectionProperties;
			}
		}

		/// <summary>
		/// Creates new properties form.
		/// </summary>
		public ConnectionPropertiesForm()
		{
			InitializeComponent();
			this._ConnectionProperties = new ConnectionProperties();
			this.propertyGrid1.SelectedObject = this._ConnectionProperties;
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
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(ConnectionPropertiesForm));
			this.ButtonConnect = new System.Windows.Forms.Button();
			this.propertyGrid1 = new System.Windows.Forms.PropertyGrid();
			this.panel1 = new System.Windows.Forms.Panel();
			this.panel2 = new System.Windows.Forms.Panel();
			this.panel1.SuspendLayout();
			this.panel2.SuspendLayout();
			this.SuspendLayout();
			// 
			// ButtonConnect
			// 
			this.ButtonConnect.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.ButtonConnect.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.ButtonConnect.Location = new System.Drawing.Point(217, 3);
			this.ButtonConnect.Name = "ButtonConnect";
			this.ButtonConnect.TabIndex = 0;
			this.ButtonConnect.Text = "Connect";
			this.ButtonConnect.Click += new System.EventHandler(this.ButtonConnect_Click);
			// 
			// propertyGrid1
			// 
			this.propertyGrid1.CommandsVisibleIfAvailable = true;
			this.propertyGrid1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.propertyGrid1.LargeButtons = false;
			this.propertyGrid1.LineColor = System.Drawing.SystemColors.ScrollBar;
			this.propertyGrid1.Location = new System.Drawing.Point(0, 0);
			this.propertyGrid1.Name = "propertyGrid1";
			this.propertyGrid1.Size = new System.Drawing.Size(292, 247);
			this.propertyGrid1.TabIndex = 2;
			this.propertyGrid1.Text = "propertyGrid1";
			this.propertyGrid1.ViewBackColor = System.Drawing.SystemColors.Window;
			this.propertyGrid1.ViewForeColor = System.Drawing.SystemColors.WindowText;
			// 
			// panel1
			// 
			this.panel1.Controls.Add(this.ButtonConnect);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.panel1.Location = new System.Drawing.Point(0, 247);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(292, 26);
			this.panel1.TabIndex = 12;
			// 
			// panel2
			// 
			this.panel2.Controls.Add(this.propertyGrid1);
			this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel2.Location = new System.Drawing.Point(0, 0);
			this.panel2.Name = "panel2";
			this.panel2.Size = new System.Drawing.Size(292, 247);
			this.panel2.TabIndex = 13;
			// 
			// ConnectionPropertiesForm
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(292, 273);
			this.Controls.Add(this.panel2);
			this.Controls.Add(this.panel1);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "ConnectionPropertiesForm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Connection Properties";
			this.panel1.ResumeLayout(false);
			this.panel2.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		private void ButtonConnect_Click(object sender, System.EventArgs e)
		{
			this.DialogResult = DialogResult.OK;
			this.Visible = false;
			this.Close();
		}
	}
}
