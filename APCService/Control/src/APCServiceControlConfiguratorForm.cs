using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.IO;
using System.ServiceProcess;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters;
using System.Runtime.Serialization.Formatters.Soap;


namespace Diacom.APCService.Control
{
	/// <summary>
	/// Represents APCServiceControlConfiguratorForm class.
	/// </summary>
	internal class APCServiceControlConfiguratorForm : System.Windows.Forms.Form
	{
		private System.Windows.Forms.TextBox TargetMachineNameTextBox;
		private System.Windows.Forms.TextBox TargetMachinePortTextBox;
		private System.Windows.Forms.Button ApplyConfigurationButton;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		/// <summary>
		/// Variables for testing and configuring.
		/// </summary>
		private APCService.APCServiceOptions opt = null;
		private System.ServiceProcess.ServiceController APCServiceController = null;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.CheckBox AutoStartCheckBox;
		private System.Windows.Forms.Button TestConfigurationButton;
		private System.Windows.Forms.CheckBox StartCheckBox; 
		private System.ServiceProcess.ServiceControllerStatus PreviousState;
		private TcpChannel RemoteChannel;
		private readonly string iniFileName = Application.StartupPath+Path.DirectorySeparatorChar+"APCServiceControl.ini.xml";
		private readonly string autostartRegKey = "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run";
		private readonly string autostartValueKey = "APCService Control";
		private readonly string autostartValue = "\"" + Application.ExecutablePath + "\"" + " -systray";

		/// <summary>
		/// Creates new form for configuration.
		/// </summary>
		public APCServiceControlConfiguratorForm()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			this.Text = "APC Service Control configuration manager";
		}

		/// <summary>
		/// Executes when form is loaded.
		/// </summary>
		/// <param name="sender">Sender of the event.</param>
		/// <param name="e">Event parameters.</param>
		private void OnLoad(object sender, System.EventArgs e)
		{
			this.Text = "Loading configuration...";
			System.IO.FileStream fs = null;
			try
			{
				fs = new System.IO.FileStream(iniFileName, System.IO.FileMode.Open, System.IO.FileAccess.Read, System.IO.FileShare.Read);
				SoapFormatter optFormatter = 	new System.Runtime.Serialization.Formatters.Soap.SoapFormatter();
				this.opt = ((APCService.APCServiceOptions)(optFormatter.Deserialize(fs)));
			}
			catch(System.Exception _e)
			{
				System.Diagnostics.Debug.WriteLine(_e.ToString());
				this.opt = new APCService.APCServiceOptions();
			}
			finally
			{
				if(fs != null) fs.Close();
			}
			this.TargetMachineNameTextBox.Text = this.opt.MachineName;
			this.TargetMachinePortTextBox.Text = this.opt.MachinePort.ToString();
			this.Text = "APCServiceControl configuration manager";
		}

		/// <summary>
		/// Tests configuration is valid (connects to service and gets its options).
		/// </summary>
		/// <param name="sender">Sender of the event.</param>
		/// <param name="e">Event parameters.</param>
		private void TestOptions(object sender, System.EventArgs e)
		{
			this.Cursor = System.Windows.Forms.Cursors.WaitCursor;
			try
			{
				// Close all previously allocated Service Controllers
				if(this.APCServiceController != null)
				{
					this.APCServiceController.Close();
					this.APCServiceController = null;
				}

				// Trying to fing IP address of target machine...
				this.opt.MachineName = System.Net.Dns.GetHostEntry(this.TargetMachineNameTextBox.Text).AddressList[0].ToString();
				this.TargetMachineNameTextBox.Text = this.opt.MachineName;
				this.TargetMachineNameTextBox.Update();
				// Getting list of services for specified machine.
				System.ServiceProcess.ServiceController [] services = System.ServiceProcess.ServiceController.GetServices(this.opt.MachineName);
				// Looking for "APCService" service.
				foreach(System.ServiceProcess.ServiceController service in services)
				{
					if(service.ServiceName.Equals("APCService"))
					{
						this.APCServiceController = service;
						break;
					}
				}
				if(this.APCServiceController == null)
				{
					MessageBox.Show("APCService is not found...", "APCService Error", MessageBox.Type.Error);
					return;
				}
				this.Text = "Service found, waiting for reply...";

				System.DateTime DueTime = DateTime.Now.AddSeconds(10);
				this.APCServiceController.Refresh();
				ServiceControllerStatus _stat = this.APCServiceController.Status;
				while( (_stat != ServiceControllerStatus.Stopped) &&
							(_stat != ServiceControllerStatus.Running) &&
								(_stat != ServiceControllerStatus.Paused))
				{
					// It does something... Lets wait action to be done.
					if(DateTime.Now > DueTime)
					{
						MessageBox.Show("APCService is not responding...", "APCService Error", MessageBox.Type.Error);
						return;
					}
					System.Threading.Thread.Sleep(1000);
					this.APCServiceController.Refresh();
					_stat = this.APCServiceController.Status;
				}

				// Service now is able to execute the appropriate command.
				this.PreviousState = _stat;
				switch(_stat)
				{
					case System.ServiceProcess.ServiceControllerStatus.Paused:
						this.APCServiceController.Continue();
						break;
					case System.ServiceProcess.ServiceControllerStatus.Stopped:
						this.APCServiceController.Start();
						break;
				}

				// Waiting for service to run...
				DueTime = DateTime.Now.AddSeconds(10);
				this.Text = "Waiting for service to run...";
				this.APCServiceController.Refresh();
				while(this.APCServiceController.Status != ServiceControllerStatus.Running)
				{
					// It is running... Lets wait...
					if(DateTime.Now > DueTime)
					{
						MessageBox.Show("APCService is not responding to start command ...", "APCService Error", MessageBox.Type.Error);
						return;
					}
					System.Threading.Thread.Sleep(500);
					this.APCServiceController.Refresh();
				}
				this.Text = "Getting service options...";
				try
				{
					// Use this convoluted mechanism due to new security restrictions in .NET 1.1
					BinaryClientFormatterSinkProvider clientProvider = 	new BinaryClientFormatterSinkProvider();
					BinaryServerFormatterSinkProvider serverProvider = 	new BinaryServerFormatterSinkProvider();
					serverProvider.TypeFilterLevel = TypeFilterLevel.Full;

					IDictionary props = new Hashtable();
					props["port"] = 0;	// Dynamically assigned port
					props["name"] = System.Guid.NewGuid().ToString();	// Some random name
					props["typeFilterLevel"] = TypeFilterLevel.Full;
					props["exclusiveAddressUse"] = false;
					RemoteChannel = new TcpChannel(props,clientProvider,serverProvider);
					ChannelServices.RegisterChannel(RemoteChannel,false);

					APCService.APCServiceOptions aAPCServOpt = ((APCService.APCServiceOptions)(System.Activator.GetObject(typeof(APCService.APCServiceOptions), String.Format("tcp://{0}:{1}/APCService.rem", this.APCServiceController.MachineName, this.opt.MachinePort))));
					// Copy all the values to a local class.
					aAPCServOpt.CopyTo(this.opt);
					// Let user to know all is OK, configuration is valid.
					string msg = "APCServiceControl configuration has been tested."+Environment.NewLine+Environment.NewLine+"APCService found on \""+this.opt.MachineName+"\"."+Environment.NewLine+"APCService is running now."+Environment.NewLine+Environment.NewLine+this.opt.ToString();
					this.Text = "See service options in message box window";
					MessageBox.Show(msg, "Congratulations!!!", MessageBox.Type.Information);
					// Test is successfully finished, configuration now can be applied.
					this.TargetMachineNameTextBox.Text = this.opt.MachineName;
				}
				catch(Exception x)
				{
					System.Diagnostics.Debug.WriteLine(x.ToString());
					MessageBox.Show("APCService is running, but configuration cannot be accessed ..."+Environment.NewLine+"Please, check the port for connection or use following information:"+Environment.NewLine+x.ToString(), "APCService Error", MessageBox.Type.Error);
					return;
				}
				// Everything worked out perfectly well - enable configuration buttons
				this.Text = "APCServiceControl configuration manager";
				this.ApplyConfigurationButton.Enabled = true;
				this.AutoStartCheckBox.Enabled = true;
				this.StartCheckBox.Enabled = true;
			}
			catch(Exception x)
			{
				System.Diagnostics.Debug.WriteLine(x.ToString());
				MessageBox.Show("APCService is running, but connection couldn't be established..."+Environment.NewLine+"Please, check the port for connection or use following information:"+Environment.NewLine+x.ToString(), "APCService Error", MessageBox.Type.Error);
			}
			finally
			{
				this.Cursor = System.Windows.Forms.Cursors.Arrow;
			}
		}

		/// <summary>
		/// Saves all options.
		/// </summary>
		/// <param name="sender">Sender of the event.</param>
		/// <param name="e">Event parameters.</param>
		private void SaveOptions(object sender, System.EventArgs e)
		{
			if(this.ApplyConfigurationButton.Enabled)
			{
				System.IO.FileStream fs = null;
				try
				{
					// Saving settings.
					fs = new System.IO.FileStream(iniFileName, System.IO.FileMode.Create, System.IO.FileAccess.Write, System.IO.FileShare.ReadWrite);
					SoapFormatter optFormatter = 
							new System.Runtime.Serialization.Formatters.Soap.SoapFormatter();
					optFormatter.Serialize(fs, this.opt);
					// Setting application to autostart or remove it from.
					Microsoft.Win32.RegistryKey AppStartUpKey = 
						Microsoft.Win32.Registry.LocalMachine.OpenSubKey(autostartRegKey, true);
					if(this.AutoStartCheckBox.Checked)
					{
						// Setting.
						AppStartUpKey.SetValue(autostartValueKey, autostartValue);
					}
					else
					{
						// Removing.
						AppStartUpKey.DeleteValue(autostartValueKey);
					}
				}
				catch(System.Exception x)
				{
					MessageBox.Show("Exception while applying new settings:"+Environment.NewLine+x.ToString()+Environment.NewLine+"Settings for your application could be set through changing file \""+Application.ExecutablePath+".ini\" manually."+Environment.NewLine+"Application can't continue execution... quit now.", "Unrecoverable error", MessageBox.Type.Error);
					this.DialogResult = DialogResult.Cancel;
					this.Close();
				}
				finally
				{
					if( fs != null) fs.Close();
				}
			}
			if(sender == this.ApplyConfigurationButton) 
				this.DialogResult = DialogResult.OK;
			else
				this.DialogResult = DialogResult.Cancel;
			this.Close();
		}

		/// <summary>
		/// Where are some changes... User should test them before save.
		/// </summary>
		/// <param name="sender">Sender of the event.</param>
		/// <param name="e">Event parameters.</param>
		private void TargetMachineNameTextBox_TextChanged(object sender, System.EventArgs e)
		{
			this.ApplyConfigurationButton.Enabled = false;
			this.AutoStartCheckBox.Enabled = false;
			this.StartCheckBox.Enabled = false;
		}

		/// <summary>
		/// Where are some changes... User should test them before save.
		/// </summary>
		/// <param name="sender">Sender of the event.</param>
		/// <param name="e">Event parameters.</param>
		private void TargetMachinePortTextBox_TextChanged(object sender, System.EventArgs e)
		{
			this.ApplyConfigurationButton.Enabled = false;
			this.AutoStartCheckBox.Enabled = false;
			this.StartCheckBox.Enabled = false;
			this.opt.MachinePort = Convert.ToInt32(this.TargetMachinePortTextBox.Text);
		}

		/// <summary>
		/// Cleans up any resources being used.
		/// </summary>
		protected override void Dispose(bool disposing)
		{
			if(disposing)
			{
				if(this.APCServiceController != null)
				{
					this.APCServiceController.Close();
					this.APCServiceController = null;
				}

				if(RemoteChannel != null)
				{
					ChannelServices.UnregisterChannel(RemoteChannel);
					RemoteChannel = null;
				}
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(APCServiceControlConfiguratorForm));
			this.TargetMachineNameTextBox = new System.Windows.Forms.TextBox();
			this.TargetMachinePortTextBox = new System.Windows.Forms.TextBox();
			this.ApplyConfigurationButton = new System.Windows.Forms.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.AutoStartCheckBox = new System.Windows.Forms.CheckBox();
			this.TestConfigurationButton = new System.Windows.Forms.Button();
			this.StartCheckBox = new System.Windows.Forms.CheckBox();
			this.SuspendLayout();
			// 
			// TargetMachineNameTextBox
			// 
			this.TargetMachineNameTextBox.Location = new System.Drawing.Point(0, 24);
			this.TargetMachineNameTextBox.Name = "TargetMachineNameTextBox";
			this.TargetMachineNameTextBox.Size = new System.Drawing.Size(280, 20);
			this.TargetMachineNameTextBox.TabIndex = 4;
			this.TargetMachineNameTextBox.Text = "";
			this.TargetMachineNameTextBox.TextChanged += new System.EventHandler(this.TargetMachineNameTextBox_TextChanged);
			// 
			// TargetMachinePortTextBox
			// 
			this.TargetMachinePortTextBox.Location = new System.Drawing.Point(0, 64);
			this.TargetMachinePortTextBox.Name = "TargetMachinePortTextBox";
			this.TargetMachinePortTextBox.Size = new System.Drawing.Size(280, 20);
			this.TargetMachinePortTextBox.TabIndex = 5;
			this.TargetMachinePortTextBox.Text = "";
			this.TargetMachinePortTextBox.TextChanged += new System.EventHandler(this.TargetMachinePortTextBox_TextChanged);
			// 
			// ApplyConfigurationButton
			// 
			this.ApplyConfigurationButton.Enabled = false;
			this.ApplyConfigurationButton.Location = new System.Drawing.Point(208, 120);
			this.ApplyConfigurationButton.Name = "ApplyConfigurationButton";
			this.ApplyConfigurationButton.Size = new System.Drawing.Size(72, 24);
			this.ApplyConfigurationButton.TabIndex = 3;
			this.ApplyConfigurationButton.Text = "&OK";
			this.ApplyConfigurationButton.Click += new System.EventHandler(this.SaveOptions);
			// 
			// label1
			// 
			this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(204)));
			this.label1.Location = new System.Drawing.Point(0, 8);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(280, 16);
			this.label1.TabIndex = 8;
			this.label1.Text = "Machine name (IP address) APCService is running on:";
			// 
			// label2
			// 
			this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(204)));
			this.label2.Location = new System.Drawing.Point(0, 48);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(280, 16);
			this.label2.TabIndex = 9;
			this.label2.Text = "Machine port to connect to APCService:";
			// 
			// AutoStartCheckBox
			// 
			this.AutoStartCheckBox.Checked = true;
			this.AutoStartCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
			this.AutoStartCheckBox.Enabled = false;
			this.AutoStartCheckBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(204)));
			this.AutoStartCheckBox.Location = new System.Drawing.Point(0, 88);
			this.AutoStartCheckBox.Name = "AutoStartCheckBox";
			this.AutoStartCheckBox.Size = new System.Drawing.Size(280, 16);
			this.AutoStartCheckBox.TabIndex = 2;
			this.AutoStartCheckBox.Text = "APCServiceControl should start automatically";
			// 
			// TestConfigurationButton
			// 
			this.TestConfigurationButton.Location = new System.Drawing.Point(136, 120);
			this.TestConfigurationButton.Name = "TestConfigurationButton";
			this.TestConfigurationButton.Size = new System.Drawing.Size(72, 24);
			this.TestConfigurationButton.TabIndex = 0;
			this.TestConfigurationButton.Text = "&Test";
			this.TestConfigurationButton.Click += new System.EventHandler(this.TestOptions);
			// 
			// StartCheckBox
			// 
			this.StartCheckBox.Checked = true;
			this.StartCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
			this.StartCheckBox.Enabled = false;
			this.StartCheckBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(204)));
			this.StartCheckBox.Location = new System.Drawing.Point(-1, 104);
			this.StartCheckBox.Name = "StartCheckBox";
			this.StartCheckBox.Size = new System.Drawing.Size(280, 16);
			this.StartCheckBox.TabIndex = 10;
			this.StartCheckBox.Text = "Start APCServiceControl when done";
			// 
			// APCServiceControlConfiguratorForm
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(279, 143);
			this.Controls.Add(this.StartCheckBox);
			this.Controls.Add(this.TestConfigurationButton);
			this.Controls.Add(this.AutoStartCheckBox);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.ApplyConfigurationButton);
			this.Controls.Add(this.TargetMachinePortTextBox);
			this.Controls.Add(this.TargetMachineNameTextBox);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "APCServiceControlConfiguratorForm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "APCServiceControl configuration manager";
			this.Load += new System.EventHandler(this.OnLoad);
			this.Closed += new System.EventHandler(this.OnClose);
			this.ResumeLayout(false);

		}
		#endregion

		/// <summary>
		/// Checking if we should run control tool after closing.
		/// </summary>
		/// <param name="sender">Sender of the event.</param>
		/// <param name="e">Event parameters.</param>
		private void OnClose(object sender, System.EventArgs e)
		{
			if(!this.StartCheckBox.Checked)
			{
				// Return service to the state it was before we kicked it.
				this.Text = "Setting service into state it was before...";
				switch(this.PreviousState)
				{
					case System.ServiceProcess.ServiceControllerStatus.Paused:
					{
						this.APCServiceController.Pause();
						break;
					}
					case System.ServiceProcess.ServiceControllerStatus.Stopped:
					{
						this.APCServiceController.Stop();
						break;
					}
				}
			}
			else
			{
				// Starting APCServiceControl executable - with new options.
				if(this.ApplyConfigurationButton.Enabled) System.Diagnostics.Process.Start(Application.ExecutablePath, "-systray");
			}
		}
	}
}
