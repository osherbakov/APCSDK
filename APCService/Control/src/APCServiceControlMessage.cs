using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;

namespace Diacom.APCService.Control
{
	/// <summary>
	/// Summary description for APCServiceControlMessage.
	/// </summary>
	internal class MessageBox : System.Windows.Forms.Form
	{
		public enum Type
		{
			Error,
			Warning,
			Information,
		}

		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		private System.Windows.Forms.Button ButtonOK;
		private System.Windows.Forms.TextBox textBox1;
		private System.Windows.Forms.Button ButtonCancel;
		private System.Resources.ResourceManager resources = null;

		public MessageBox()
		{
			this.resources = new System.Resources.ResourceManager(GetType());
			this.ButtonOK = new System.Windows.Forms.Button();
			this.textBox1 = new System.Windows.Forms.TextBox();
			this.ButtonCancel = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// ButtonOK
			// 
			this.ButtonOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.ButtonOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.ButtonOK.Location = new System.Drawing.Point(10, 135);
			this.ButtonOK.Name = "ButtonOK";
			this.ButtonOK.TabIndex = 0;
			this.ButtonOK.Text = "OK";
			// 
			// textBox1
			// 
			this.textBox1.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.textBox1.Location = new System.Drawing.Point(144, 0);
			this.textBox1.Multiline = true;
			this.textBox1.Name = "textBox1";
			this.textBox1.ReadOnly = true;
			this.textBox1.Size = new System.Drawing.Size(200, 135);
			this.textBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right | System.Windows.Forms.AnchorStyles.Bottom)));
			this.textBox1.TabIndex = 3;
			this.textBox1.Text = "";
			// 
			// ButtonCancel
			// 
			this.ButtonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.ButtonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.ButtonCancel.Location = new System.Drawing.Point(86, 135);
			this.ButtonCancel.Name = "ButtonCancel";
			this.ButtonCancel.TabIndex = 4;
			this.ButtonCancel.Text = "Cancel";
			this.ButtonCancel.Visible = false;
			// 
			// MessageBox
			// 
			this.AcceptButton = this.ButtonOK;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(350, 153);
			this.Controls.Add(this.textBox1);
			this.Controls.Add(this.ButtonOK);
			this.Controls.Add(this.ButtonCancel);
			this.MinimumSize = new System.Drawing.Size(360, 190);
			this.Name = "MessageBox";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.TopMost = true;
			this.ShowInTaskbar = false;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Icon = ((System.Drawing.Icon)(this.resources.GetObject("Icon")));
			this.ResumeLayout(false);
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

		private static System.Windows.Forms.DialogResult ShowMessage(string aText, string aTitle, Type aType, bool aCancellationButton)
		{
			MessageBox mb = new MessageBox();
			mb.Text = aTitle;
			mb.textBox1.Text = aText;
			mb.ButtonCancel.Visible = aCancellationButton;
			mb.ShowDialog();
			return mb.DialogResult;
		}

		public static System.Windows.Forms.DialogResult Show(string aText, string aTitle, Type aType)
		{
			return ShowMessage(aText, aTitle, aType, false);
		}

		private void InitializeComponent()
		{
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(MessageBox));
			// 
			// MessageBox
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(292, 273);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "MessageBox";

		}

		public static System.Windows.Forms.DialogResult Show(string aText, string aTitle, Type aType, bool aCancellationButton)
		{
			return ShowMessage(aText, aTitle, aType, aCancellationButton);
		}
	}
}
