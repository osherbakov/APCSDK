using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace Diacom.APCService
{
	/// <summary>
	/// Summary description for ConfigureForm.
	/// </summary>
	internal class ConfigureForm : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Panel ButtonPanel;
		private System.Windows.Forms.Button button1_OK;
		private System.Windows.Forms.Button button2_Cancel;
		private System.Windows.Forms.Panel OptionsPanel;
		private System.Windows.Forms.PropertyGrid propertyGrid1;
		private System.Windows.Forms.CheckBox RestartCheckBox;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		/// <summary>
		/// Constructor for the form.
		/// </summary>
		public ConfigureForm(string aServiceName, string aMachineName, APCServiceOptions aAPCSrvOpt)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			//
			// TODO: Add any constructor code after InitializeComponent call
			//
			this.Text = String.Format("{0} on {1} configuration", aServiceName, aMachineName);
			this.propertyGrid1.SelectedObject = new APCServiceOptions();
		}

		/// <summary>
		/// Cleans up any resources being used.
		/// </summary>
		protected override void Dispose(bool disposing)
		{
			if((disposing)&&(components != null)) components.Dispose();
			base.Dispose( disposing );
		}

		/// <summary>
		/// Form is closing - check if checkbox is checked and closing is not cancelling operation.
		/// </summary>
		/// <param name="sender">Sender of the event.</param>
		/// <param name="e">Event parameters.</param>
		private void ConfigureForm_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			if((this.DialogResult.Equals(DialogResult.OK))&&(this.RestartCheckBox.Checked)) this.DialogResult = DialogResult.Yes;
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(ConfigureForm));
			this.ButtonPanel = new System.Windows.Forms.Panel();
			this.RestartCheckBox = new System.Windows.Forms.CheckBox();
			this.button2_Cancel = new System.Windows.Forms.Button();
			this.button1_OK = new System.Windows.Forms.Button();
			this.OptionsPanel = new System.Windows.Forms.Panel();
			this.propertyGrid1 = new System.Windows.Forms.PropertyGrid();
			this.ButtonPanel.SuspendLayout();
			this.OptionsPanel.SuspendLayout();
			this.SuspendLayout();
			// 
			// ButtonPanel
			// 
			this.ButtonPanel.Controls.Add(this.RestartCheckBox);
			this.ButtonPanel.Controls.Add(this.button2_Cancel);
			this.ButtonPanel.Controls.Add(this.button1_OK);
			this.ButtonPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.ButtonPanel.Location = new System.Drawing.Point(0, 299);
			this.ButtonPanel.Name = "ButtonPanel";
			this.ButtonPanel.Size = new System.Drawing.Size(342, 24);
			this.ButtonPanel.TabIndex = 2;
			// 
			// RestartCheckBox
			// 
			this.RestartCheckBox.Checked = true;
			this.RestartCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
			this.RestartCheckBox.Dock = System.Windows.Forms.DockStyle.Left;
			this.RestartCheckBox.Location = new System.Drawing.Point(0, 0);
			this.RestartCheckBox.Name = "RestartCheckBox";
			this.RestartCheckBox.Size = new System.Drawing.Size(128, 24);
			this.RestartCheckBox.TabIndex = 2;
			this.RestartCheckBox.Text = "restart service";
			// 
			// button2_Cancel
			// 
			this.button2_Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.button2_Cancel.Dock = System.Windows.Forms.DockStyle.Right;
			this.button2_Cancel.Location = new System.Drawing.Point(192, 0);
			this.button2_Cancel.Name = "button2_Cancel";
			this.button2_Cancel.Size = new System.Drawing.Size(75, 24);
			this.button2_Cancel.TabIndex = 1;
			this.button2_Cancel.Text = "Cancel";
			// 
			// button1_OK
			// 
			this.button1_OK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.button1_OK.Dock = System.Windows.Forms.DockStyle.Right;
			this.button1_OK.Location = new System.Drawing.Point(267, 0);
			this.button1_OK.Name = "button1_OK";
			this.button1_OK.Size = new System.Drawing.Size(75, 24);
			this.button1_OK.TabIndex = 0;
			this.button1_OK.Text = "OK";
			// 
			// OptionsPanel
			// 
			this.OptionsPanel.Controls.Add(this.propertyGrid1);
			this.OptionsPanel.Dock = System.Windows.Forms.DockStyle.Fill;
			this.OptionsPanel.Location = new System.Drawing.Point(0, 0);
			this.OptionsPanel.Name = "OptionsPanel";
			this.OptionsPanel.Size = new System.Drawing.Size(342, 299);
			this.OptionsPanel.TabIndex = 3;
			// 
			// propertyGrid1
			// 
			this.propertyGrid1.CommandsVisibleIfAvailable = true;
			this.propertyGrid1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.propertyGrid1.LargeButtons = false;
			this.propertyGrid1.LineColor = System.Drawing.SystemColors.ScrollBar;
			this.propertyGrid1.Location = new System.Drawing.Point(0, 0);
			this.propertyGrid1.Name = "propertyGrid1";
			this.propertyGrid1.Size = new System.Drawing.Size(342, 299);
			this.propertyGrid1.TabIndex = 0;
			this.propertyGrid1.Text = "propertyGrid1";
			this.propertyGrid1.ViewBackColor = System.Drawing.SystemColors.Window;
			this.propertyGrid1.ViewForeColor = System.Drawing.SystemColors.WindowText;
			// 
			// ConfigureForm
			// 
			this.AcceptButton = this.button1_OK;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.CancelButton = this.button2_Cancel;
			this.ClientSize = new System.Drawing.Size(342, 323);
			this.Controls.Add(this.OptionsPanel);
			this.Controls.Add(this.ButtonPanel);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MinimumSize = new System.Drawing.Size(350, 350);
			this.Name = "ConfigureForm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Closing += new System.ComponentModel.CancelEventHandler(this.ConfigureForm_Closing);
			this.ButtonPanel.ResumeLayout(false);
			this.OptionsPanel.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion
	}
}
