using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.ServiceProcess;
using System.Windows.Forms;

namespace Diacom.APCService
{
	/// <summary>
	/// Actions for service.
	/// </summary>
	internal enum ServiceAction
	{
		Start,
		Stop,
		Restart,
	}

	/// <summary>
	/// Represents ServicePendingForm class for acting with service.
	/// </summary>
	internal class ServicePendingForm : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Label ServiceActionLabel;
		private System.Windows.Forms.ProgressBar ServiceActionProgressBar;
		private ServiceController APCServiceController = null;
		private ServiceAction APCServiceAction;
		private ServiceControllerStatus FinalState;
		private DateTime DueTime;
		private readonly int SecondsToWait = 5;
		private Timer ProgressTimer = new Timer();

		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		/// <summary>
		/// Initializes new instance of class.
		/// </summary>
		/// <param name="aSC">Service controller to deal with.</param>
		/// <param name="act">Action for service to perform.</param>
		public ServicePendingForm(System.ServiceProcess.ServiceController aSC, ServiceAction act)
		{
			this.InitializeComponent();
			this.APCServiceController = aSC;
			this.APCServiceAction = act;
			this.BringToFront();
			this.TopMost = true;
		}

		/// <summary>
		/// Cleans up any resources being used.
		/// </summary>
		protected override void Dispose(bool disposing)
		{
			if(disposing)
			{
				if(components != null) components.Dispose();
				if(ProgressTimer != null) ProgressTimer.Dispose();
			}
			base.Dispose( disposing );
		}
		
		/// <summary>
		/// Stops the service.
		/// </summary>
		private void StopService()
		{
			this.APCServiceController.Refresh();
			if( (this.APCServiceController.Status == ServiceControllerStatus.Running )||
				(this.APCServiceController.Status == ServiceControllerStatus.Paused ))
			{
				this.ServiceActionLabel.Text = "Stopping \""+this.APCServiceController.ServiceName+"\" service on \""+this.APCServiceController.MachineName+"\" computer...";
				FinalState = ServiceControllerStatus.Stopped;
				APCServiceController.Stop();
			}
		}

		/// <summary>
		/// Starts the service.
		/// </summary>
		private void StartService()
		{
			this.APCServiceController.Refresh();
			if(this.APCServiceController.Status == ServiceControllerStatus.Stopped )
			{
				this.ServiceActionLabel.Text = "Starting \""+this.APCServiceController.ServiceName+"\" service on \""+this.APCServiceController.MachineName+"\" computer...";
				this.APCServiceController.Start();
				FinalState = ServiceControllerStatus.Running;
			}
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.ServiceActionLabel = new System.Windows.Forms.Label();
			this.ServiceActionProgressBar = new System.Windows.Forms.ProgressBar();
			this.SuspendLayout();
			// 
			// ServiceActionLabel
			// 
			this.ServiceActionLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(204)));
			this.ServiceActionLabel.Location = new System.Drawing.Point(0, 2);
			this.ServiceActionLabel.Name = "ServiceActionLabel";
			this.ServiceActionLabel.Size = new System.Drawing.Size(290, 24);
			this.ServiceActionLabel.TabIndex = 1;
			// 
			// ServiceActionProgressBar
			// 
			this.ServiceActionProgressBar.Location = new System.Drawing.Point(0, 17);
			this.ServiceActionProgressBar.Name = "ServiceActionProgressBar";
			this.ServiceActionProgressBar.Size = new System.Drawing.Size(288, 22);
			this.ServiceActionProgressBar.TabIndex = 0;
			// 
			// ServicePendingForm
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(288, 39);
			this.ControlBox = false;
			this.Controls.Add(this.ServiceActionProgressBar);
			this.Controls.Add(this.ServiceActionLabel);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "ServicePendingForm";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Performing action...";
			this.Load += new System.EventHandler(this.ServicePendingForm_Load);
			this.ResumeLayout(false);

		}
		#endregion

		/// <summary>
		/// Executes when form is loaded.
		/// </summary>
		/// <param name="sender">Sender of the event.</param>
		/// <param name="e">Event arguments.</param>
		private void ServicePendingForm_Load(object sender, System.EventArgs e)
		{
			this.ServiceActionProgressBar.Minimum = 0;
			this.ServiceActionProgressBar.Value = 0;
			this.ServiceActionProgressBar.Maximum = SecondsToWait;
			this.DueTime = DateTime.Now.AddSeconds(SecondsToWait);

			this.ServiceActionLabel.Update();
			this.ServiceActionProgressBar.Update();
			this.Update();

			ProgressTimer.Interval = 500;
			ProgressTimer.Tick +=new EventHandler(ProgressTimer_Tick);
			ProgressTimer.Enabled = true;
			switch(this.APCServiceAction)
			{
				case ServiceAction.Start:
					this.StartService();
					break;
				case ServiceAction.Restart:
				case ServiceAction.Stop:
					this.StopService();
					break;
			}
		}

		private void ProgressTimer_Tick(object sender, EventArgs e)
		{
			this.APCServiceController.Refresh();
			if( this.APCServiceController.Status != FinalState && (DateTime.Now > DueTime) )
			{
				DialogResult = DialogResult.Cancel;
				return;
			}

			if (this.APCServiceController.Status == FinalState)
			{
				switch(this.APCServiceAction)
				{
					case ServiceAction.Restart:
						if(FinalState == ServiceControllerStatus.Stopped)
							this.StartService();
						else
							DialogResult = DialogResult.OK;
						return;
					case ServiceAction.Start:
						DialogResult = DialogResult.OK;
						return;
					case ServiceAction.Stop:
						DialogResult = DialogResult.OK;
						return;
				}
			}
			ServiceActionProgressBar.Value = this.DueTime.Subtract(DateTime.Now).Seconds;
			ServiceActionProgressBar.Update();
			this.APCServiceController.Refresh();
		}
	}
}
