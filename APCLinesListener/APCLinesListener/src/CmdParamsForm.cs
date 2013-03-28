using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace APCLinesListener
{
	/// <summary>
	/// Form for user to input command parameters.
	/// </summary>
	internal class CommandParameters : System.Windows.Forms.Form
	{
		/// <summary>
		/// Represents form control for parameter value (Label+TextBox).
		/// </summary>
		private class ParamControl : System.Windows.Forms.Control
		{
			/// <summary>
			/// TextBox for parameter value.
			/// </summary>
			private TextBox Edit = null;

			/// <summary>
			/// Label with parameter description.
			/// </summary>
			private Label Hint = null;

			/// <summary>
			/// Gets the value of the parameter.
			/// </summary>
			public string Value { get { return this.Edit.Text; } }

			/// <summary>
			/// Height of the control.
			/// </summary>
			public new const int Height = 45;

			/// <summary>
			/// Creates new instance of the control depending on specified parameters.
			/// </summary>
			/// <param name="aDefaultValue">Default parameter value.</param>
			/// <param name="aHint">Parameter description.</param>
			/// <param name="aTabIndex">TabIndex of the control.</param>
			/// <param name="aWidth">Width of the control.</param>
			public ParamControl(string aDefaultValue, string aHint, int aTabIndex, int aWidth)
			{
				this.TabIndex = aTabIndex;
				this.Size = new Size(aWidth, Height);
				this.Location = new Point(0, aTabIndex*Height);
				this.Hint = new Label();
				this.Hint.Location = new Point(0, 0);
				this.Hint.Name = "Hint";
				this.Hint.Size = new Size(this.Size.Width, 20);
				this.Hint.Text = aHint;
				this.Controls.Add(this.Hint);
				this.Edit = new TextBox();
				this.Edit.Location = new Point(0, 20);
				this.Edit.Name = "Edit";
				this.Edit.Size = new Size(this.Size.Width, 20);
				this.Edit.TabIndex = 0;
				this.Edit.Text = aDefaultValue;
				this.Edit.Anchor = AnchorStyles.Left|AnchorStyles.Top|AnchorStyles.Right;
				this.Controls.Add(this.Edit);
				this.Anchor = AnchorStyles.Left|AnchorStyles.Top|AnchorStyles.Right;
			}

			/// <summary>
			/// Executes when control gets focus - sets the focus to a text field.
			/// </summary>
			/// <param name="e">Event parameters.</param>
			protected override void OnGotFocus(EventArgs e)
			{
				this.Edit.Focus();
				base.OnGotFocus (e);
			}
		}

		/// <summary>
		/// Array of parameters controls
		/// </summary>
		private ParamControl [] Params = null;

		/// <summary>
		/// Button with "OK" value.
		/// </summary>
		private Button ActionButton = null;

		/// <summary>
		/// Button with "Cancel" value.
		/// </summary>
		private Button CloseButton = null;

		/// <summary>
		/// Resulting parameters as string[].
		/// </summary>
		private string [] Results = null;

		/// <summary>
		/// Creates new instance of the class and initialises form components.
		/// </summary>
		public CommandParameters(int aNumberOfParameters, string aTitle, string [] aHints, string [] aDefaults)
		{
			this.Results = new string[5];
			// Suspending layout.
			this.SuspendLayout();
			// "Cancel" button.
			this.CloseButton = new Button();
			this.CloseButton.Location = new System.Drawing.Point(0, (aNumberOfParameters+1)*ParamControl.Height+2);
			this.CloseButton.Name = "CancelButton";
			this.CloseButton.Size = new System.Drawing.Size(72, 23);
			this.CloseButton.TabIndex = aNumberOfParameters+3;
			this.CloseButton.DialogResult = DialogResult.Cancel;
			this.CloseButton.Text = "&Cancel";
			this.CloseButton.Anchor = AnchorStyles.Left|AnchorStyles.Bottom;
			// "OK" button.
			this.ActionButton = new Button();
			this.ActionButton.Location = new System.Drawing.Point(73, (aNumberOfParameters+1)*ParamControl.Height+2);
			this.ActionButton.Name = "ActionButton";
			this.ActionButton.Size = new System.Drawing.Size(72, 23);
			this.ActionButton.TabIndex = aNumberOfParameters+2;
			this.ActionButton.DialogResult = DialogResult.OK;
			this.ActionButton.Text = "&OK";
			this.ActionButton.Anchor = AnchorStyles.Left|AnchorStyles.Bottom;
			// Parameters controls.
			this.Params = new ParamControl[aNumberOfParameters+1];
			this.Params[0] = new ParamControl(aDefaults[0], "Sender of the command:", 0, this.Width);
			this.Params[1] = new ParamControl(aDefaults[1], aHints[0], 1, this.Width);
			for(int i = 1; i < aNumberOfParameters; i++) this.Params[i+1] = new ParamControl(aDefaults[i+1], aHints[i], i+1, this.Width);
			// The form itself.
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.CancelButton = this.CloseButton;
			this.AcceptButton = this.ActionButton;
			this.ClientSize = new System.Drawing.Size(300, (aNumberOfParameters+1)*ParamControl.Height+25);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Sizable;
			this.MaximizeBox = this.MinimizeBox = false;
			this.Name = "CmdParamsForm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.TopMost = true;
			this.MinimumSize = this.Size;
			this.Text = aTitle;
			this.Icon = ((System.Drawing.Icon)((new System.Resources.ResourceManager(typeof(CommandParameters))).GetObject("$this.Icon")));
			// Adding controls.
			this.Controls.Add(this.ActionButton);
			this.Controls.Add(this.CloseButton);
			for(int i = 0; i <= aNumberOfParameters; i++) this.Controls.Add(this.Params[i]);
			// Resuming layout.
			this.ResumeLayout(false);
		}

		/// <summary>
		/// Gets resulting parameters user has input.
		/// </summary>
		/// <returns>Resulting parameters as string[].</returns>
		public string [] Get()
		{
			if(this.DialogResult.Equals(DialogResult.OK))
			{
				for(int i = 1; i < this.Params.Length; i++) this.Results[i-1] = this.Params[i].Value;
				this.Results[4] = this.Params[0].Value;
				return this.Results;
			}
			else return null;
		}
	}
}
