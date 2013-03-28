using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.ServiceProcess;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters;
using System.Runtime.Serialization.Formatters.Soap;

namespace Diacom.APCService.Control
{
	/// <summary>
	/// Summary description for APCServiceTrayForm.
	/// </summary>
	public class APCServiceTrayForm : System.Windows.Forms.Form
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		/// <summary>
		/// Creates an instance of the class without parameters.
		/// </summary>
		public APCServiceTrayForm()
		{
			InitializeComponent();
			InitializeTray();
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(APCServiceTrayForm));
			// 
			// APCServiceTrayForm
			// 
			this.AutoScaleMode = AutoScaleMode.None;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.CausesValidation = false;
			this.ClientSize = new System.Drawing.Size(88, 88);
			this.ControlBox = false;
			this.Enabled = false;
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "APCServiceTrayForm";
			this.ShowInTaskbar = false;
			this.WindowState = System.Windows.Forms.FormWindowState.Minimized;

		}
		#endregion

		/// <summary>
		/// Context menu for an icon.
		/// </summary>
		private ContextMenu TrayMenu = new ContextMenu();
		private MenuItem CMIExit = new MenuItem();
		private MenuItem CMIStopService = new MenuItem();
		private MenuItem CMIStartService = new MenuItem();
		private MenuItem CMIRestartService = new MenuItem();
		private MenuItem CMIConfigureAPCService = new MenuItem();
		private MenuItem CMIConfigureControlTool = new MenuItem();
		private MenuItem CMIAbout = new MenuItem();
		private MenuItem CMISeparator0 = new MenuItem();
		private MenuItem CMISeparator1 = new MenuItem();
		private MenuItem CMISeparator2 = new MenuItem();

		private SystemTrayControl Tray;
		private ConfigureForm ConfigForm;

		/// <summary>
		/// Icon, which appears in taskbar when service is not controlled.
		/// </summary>
		private System.Drawing.Icon ServiceNotControlledIcon;

		/// <summary>
		/// Icon, which appears in taskbar when service is pending action.
		/// </summary>
		private System.Drawing.Icon ServiceIsPendingIcon;

		/// <summary>
		/// Icon, which appears in taskbar when service is stopped.
		/// </summary>
		private System.Drawing.Icon ServiceIsStoppedIcon;

		/// <summary>
		/// Icon, which appears in taskbar when service is running and all is OK.
		/// </summary>
		private System.Drawing.Icon ServiceIsRunningIcon;

		/// <summary>
		/// Icon, which appears in taskbar when service is running but where is an error on service provider side.
		/// </summary>
		private System.Drawing.Icon ServiceProviderIsNotOKIcon;

		/// <summary>
		/// Icon, which appears in taskbar when service is configured.
		/// </summary>
		private System.Drawing.Icon ServiceConfiguredIcon;

		/// <summary>
		/// Timer for refreshing information on service behavior.
		/// </summary>
		private System.Windows.Forms.Timer Timer = null;

		/// <summary>
		/// information on class storage.
		/// </summary>
		private string Info;

		/// <summary>
		/// APCService options.
		/// </summary>
		private APCServiceOptions APCSrvOpt = null;

		/// <summary>
		/// Service provider status.
		/// </summary>
		private APCServiceProviderStatus SPst;

		/// <summary>
		/// APCService service controller.
		/// </summary>
		private ServiceController APCServiceController = null;

		/// <summary>
		/// TCP channel for remote connections with APCService.
		/// </summary>
		private TcpChannel RemoteChannel = null;

		private bool disposed = false;

		private readonly string iniFileName = Application.StartupPath+System.IO.Path.DirectorySeparatorChar+"APCServiceControl.ini.xml";

		/// <summary>
		/// Flag which indicates what APCService Control Tool should be configured after exit.
		/// </summary>
		private bool RunAPCServiceControlConfiguration = false;

		/// <summary>
		/// Flag which indicates if APCService is configuring now.
		/// </summary>
		private bool IsConfiguring = false;

		/// <summary>
		/// Flag which indicates if APCService is Updating now.
		/// </summary>
		private bool IsUpdating = false;

		/// <summary>
		/// Last known service provider status.
		/// </summary>
		private string LastKnownSPStatus = "UNAVAILABLE";

		/// <summary>
		/// Last known Additional Information.
		/// </summary>
		private string LastAdditionalInfo = "";

		/// <summary>
		/// Last known Message shown.
		/// </summary>
		private MessageType LastMessageShown = MessageType.None;

		private bool RunningStatusShown = false;

		/// <summary>
		/// Timeout for balloon to be shown (milliseconds).
		/// </summary>
		private const int BalloonTimeout = 10000;

		/// <summary>
		/// Messages types.
		/// </summary>
		private enum MessageType
		{
			/// <summary>
			/// Unable to connect to specified machine.
			/// </summary>
			ErrorConnectionToMachine,

			/// <summary>
			/// Where is no service on machine specified.
			/// </summary>
			ErrorNoServiceFound,

			/// <summary>
			/// TCP channels error.
			/// </summary>
			ErrorTCPConnection,

			/// <summary>
			/// Error what occurs during APCService configuration.
			/// </summary>
			ErrorConfiguring,

			/// <summary>
			/// APCService not controlled.
			/// </summary>
			ErrorNoServiceControl,

			/// <summary>
			/// Command to stop, start or restart APCService timeout expired.
			/// </summary>
			ErrorServiceActionTimeoutExpired,

			/// <summary>
			/// Service is running.
			/// </summary>
			MessageServiceIsRunning,

			/// <summary>
			/// Service provider status should be shown to user.
			/// </summary>
			MessageSPStatus,

			/// <summary>
			/// No APCServiceControl.ini.xml file found.
			/// </summary>
			WarningNoOptionsFile,

			/// <summary>
			/// Service is stopped.
			/// </summary>
			WarningServiceIsStopped,

			/// <summary>
			/// No messages (empty entry).
			/// </summary>
			None,
		}

		/// <summary>
		/// Shows message to user.
		/// </summary>
		/// <param name="aType">Type of message <see cref="MessageType"/> enumeration.</param>
		/// <param name="aSPStatus">Status of service provider.</param>
		/// <param name="aAddInf">Additional information.</param>
		private void ShowMessage(MessageType aType, string aSPStatus, string aAddInf)
		{
			if (aSPStatus == null) aSPStatus = "";
			if (aAddInf == null) aAddInf = "";

			if( (this.LastMessageShown == aType) && 
				 (this.LastKnownSPStatus == aSPStatus) && (this.LastAdditionalInfo == aAddInf) ) return;

			string bTitle = "";
			string bText = "";
			SystemTrayControl.InformationFlags bIcon = SystemTrayControl.InformationFlags.None;

			switch(aType)
			{
				case MessageType.WarningServiceIsStopped:
					this.Tray.Icon = this.ServiceIsStoppedIcon;
					this.LastKnownSPStatus = "UNAVAILABLE";
					bTitle = "APCService Status";
					bText = "Current APCService status: STOPPED";
					bIcon = SystemTrayControl.InformationFlags.Warning;
					break;

				case MessageType.MessageServiceIsRunning:
					this.Tray.Icon = this.ServiceIsPendingIcon;
					this.LastKnownSPStatus = "UNAVAILABLE";
					bTitle = "APCService Status";
					bText = "Current APCService status: RUNNING";
					bIcon = SystemTrayControl.InformationFlags.Info;
					break;

				case MessageType.WarningNoOptionsFile:
					bTitle = "Error Configuration";
					bIcon = SystemTrayControl.InformationFlags.Warning;
					bText = "Can't find configuartion file";
					break;

				case MessageType.MessageSPStatus:
					if(aSPStatus.Equals(String.Empty)) return;
					if( aSPStatus.Equals("OK") ) 
					{
						this.Tray.Icon = this.ServiceIsRunningIcon;
						bIcon = SystemTrayControl.InformationFlags.Info;
					}
					else 
					{
						this.Tray.Icon = this.ServiceProviderIsNotOKIcon;
						bIcon = SystemTrayControl.InformationFlags.Error;
					}
					bTitle = "SP Status";
					bText = aSPStatus + " - " + aAddInf;
					break;

				case MessageType.ErrorConfiguring:
					bTitle = "Configuration Error";
					bText = "APCService Control Tool can't configure APCService";
					bIcon = SystemTrayControl.InformationFlags.Error;
					break;

				case MessageType.ErrorConnectionToMachine:
					this.Tray.Icon = this.ServiceProviderIsNotOKIcon;
					bTitle = "Connection Error";
					bText = "APCService Control Tool can't connect to "+this.APCSrvOpt.MachineName;
					bIcon = SystemTrayControl.InformationFlags.Error;
					break;

				case MessageType.ErrorNoServiceControl:
					this.Tray.Icon = this.ServiceProviderIsNotOKIcon;
					bTitle = "Control Error";
					bText = "APCService not controlled";
					bIcon = SystemTrayControl.InformationFlags.Error;
					break;

				case MessageType.ErrorNoServiceFound:
					this.Tray.Icon = this.ServiceProviderIsNotOKIcon;
					bTitle = "Service Error";
					bText = "APCService Control Tool can't find APCService on " + this.APCSrvOpt.MachineName;
					bIcon = SystemTrayControl.InformationFlags.Error;
					break;

				case MessageType.ErrorServiceActionTimeoutExpired:
					this.Tray.Icon = this.ServiceProviderIsNotOKIcon;
					bTitle = "Timeout Error";
					bText = "APCService timeout for "+aAddInf+" expired";
					bIcon = SystemTrayControl.InformationFlags.Error;
					break;

				case MessageType.ErrorTCPConnection:
					this.Tray.Icon = this.ServiceProviderIsNotOKIcon;
					bTitle = "TCP Error";
					bText = "APCService Control Tool can't connect to APCService";
					bIcon = SystemTrayControl.InformationFlags.Error;
					break;
			}
			this.Tray.ShowBalloon(bTitle, bText, bIcon, BalloonTimeout);
			this.LastMessageShown = aType;
			this.LastKnownSPStatus = aSPStatus;
			this.LastAdditionalInfo = aAddInf;
		}

		/// <summary>
		/// Initializes tray icon components.
		/// </summary>
		private void InitializeTray()
		{
			this.components = new System.ComponentModel.Container();
			// Icons.
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(GetType());
			this.ServiceIsPendingIcon = ((System.Drawing.Icon)(resources.GetObject("Icon_INI.ico")));
			this.ServiceIsRunningIcon = ((System.Drawing.Icon)(resources.GetObject("Icon_ST.ico")));
			this.ServiceIsStoppedIcon = ((System.Drawing.Icon)(resources.GetObject("Icon_OK.ico")));
			this.ServiceNotControlledIcon = ((System.Drawing.Icon)(resources.GetObject("Icon_NON.ico")));
			this.ServiceProviderIsNotOKIcon = ((System.Drawing.Icon)(resources.GetObject("Icon_ERR.ico")));
			this.ServiceConfiguredIcon = ((System.Drawing.Icon)(resources.GetObject("Icon_NON.ico")));
			// Context Menu.
			// About.
			this.CMIAbout.Index = 0;
			this.CMIAbout.Text = "About";
			this.CMIAbout.Click += new EventHandler(CMIAbout_Click);
			// Separator #0.
			this.CMISeparator0.Index = 1;
			this.CMISeparator0.Text = "-";
			// Configure APCService.
			this.CMIConfigureAPCService.Index = 2;
			this.CMIConfigureAPCService.DefaultItem = true;
			this.CMIConfigureAPCService.Text = "Configure APCService";
			this.CMIConfigureAPCService.Click += new EventHandler(CMIConfigureAPCService_Click);
			// Configure Control Tool.
			this.CMIConfigureControlTool.Index = 3;
			this.CMIConfigureControlTool.Text = "Configure Control Tool";
			this.CMIConfigureControlTool.Click += new EventHandler(CMIConfigureControlTool_Click);
			// Separator #1.
			this.CMISeparator1.Index = 4;
			this.CMISeparator1.Text = "-";
			// Start APCService.
			this.CMIStartService.Index = 5;
			this.CMIStartService.Text = "Start APCService";
			this.CMIStartService.Click += new EventHandler(CMIStartService_Click);
			// Restart APCService.
			this.CMIRestartService.Index = 6;
			this.CMIRestartService.Text = "Restart APCService";
			this.CMIRestartService.Click += new EventHandler(CMIRestartService_Click);
			// Stop APCService.
			this.CMIStopService.Index = 7;
			this.CMIStopService.Text = "Stop APCService";
			this.CMIStopService.Click += new EventHandler(CMIStopService_Click);
			// Separator #2.
			this.CMISeparator2.Index = 8;
			this.CMISeparator2.Text = "-";
			// Exit.
			this.CMIExit.Index = 9;
			this.CMIExit.Text = "Exit";
			this.CMIExit.Click += new EventHandler(CMIExit_Click);
			// Adding items.
			this.TrayMenu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {this.CMIAbout, this.CMISeparator0, this.CMIConfigureAPCService, this.CMIConfigureControlTool, this.CMISeparator1, this.CMIStartService, this.CMIRestartService, this.CMIStopService, this.CMISeparator2, this.CMIExit} );
			this.TrayMenu.Popup += new EventHandler(ContextMenu_Popup);
			// Connect it to the tray
			this.Tray = new SystemTrayControl(ServiceNotControlledIcon);
			this.Tray.Menu = this.TrayMenu;
			this.Tray.Visible = false;
			this.Tray.LeftMouseButtonDoubleClick += new EventHandler(this.OnDoubleClick);
			//
			// Timer (disabled yet).
			//
			this.Timer = new System.Windows.Forms.Timer(this.components);
			this.Timer.Tick += new EventHandler(this.OnTimerTick);
			this.Timer.Interval = 1000;
			this.Timer.Enabled = false;
			//
			// Load options from the serialized file.
			System.IO.FileStream fs = null;
			try
			{
				SoapFormatter optFormatter = new SoapFormatter();
				fs = new System.IO.FileStream(iniFileName, System.IO.FileMode.Open, System.IO.FileAccess.Read, System.IO.FileShare.ReadWrite);
				this.APCSrvOpt = ((APCServiceOptions)(optFormatter.Deserialize(fs)));
			}
			catch(Exception _x)
			{
				System.Diagnostics.Debug.WriteLine(_x.ToString());
				this.APCSrvOpt = new APCServiceOptions();
				ShowMessage(MessageType.WarningNoOptionsFile, null, "APCService Control Tool will run with default options");
			}
			finally
			{
				if (fs != null) fs.Close();
			}
			// Seting own properties.
			this.Info = String.Format("Controlling \\\\{0}", this.APCSrvOpt.MachineName);
			// Service controller.
			if(this.GetServiceController()) 
				this.Tray.Text = "Connection established.";
			else 
				this.Tray.Text = "Connection refused.";
			this.Tray.Visible = true;
			// Enabling timer.
			this.Timer.Enabled = true;
		}


		/// <summary>
		/// Gets service controller for APCService.
		/// </summary>
		/// <returns>True if service controller created and service is working properly.</returns>
		private bool GetServiceController()
		{
			// Getting list of services.
			try
			{
				if (this.APCServiceController != null)
				{
					this.APCServiceController.Close();
					this.APCServiceController = null;
				}
				foreach(ServiceController service in ServiceController.GetServices(this.APCSrvOpt.MachineName))
				{
					if(service.ServiceName.Equals("APCService"))
					{
						this.APCServiceController = service;
						break;
					}
				}
				if(this.APCServiceController == null)
				{
					this.ShowMessage(MessageType.ErrorNoServiceFound, null, "Connection to "+this.APCSrvOpt.MachineName+" established, but there is no APCService");
					return false;
				}
				// Use this convoluted mechanism due to new security restrictions in .NET 1.1
				if(RemoteChannel != null)
				{
					ChannelServices.UnregisterChannel(RemoteChannel);
					RemoteChannel = null;
				}
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
			}
			catch(Exception _x)
			{
				System.Diagnostics.Debug.WriteLine(_x.ToString());
				this.ShowMessage(MessageType.ErrorTCPConnection, null, "Connection to "+this.APCSrvOpt.MachineName+" established, but service options and service provider status cannot be received");
				return false;
			}
			return true;
		}

		/// <summary>
		/// Cleans up any resources being used.
		/// </summary>
		protected override void Dispose(bool disposing)
		{
			if(disposed) return;
			disposed = true;
			GC.SuppressFinalize(this);
			if(disposing)
			{
				if(components != null)
				{
					components.Dispose();
				}
				this.TrayMenu.Dispose();
				this.Tray.Dispose();
				if(this.RemoteChannel != null)
				{
					ChannelServices.UnregisterChannel(this.RemoteChannel);
					this.RemoteChannel = null;
				}
				if(this.APCServiceController != null)
				{
					APCServiceController.Close();
					APCServiceController = null;
				}
			}
			base.Dispose(disposing);
		}


		/// <summary>
		/// Shows service options dialog and restarts service if needed.
		/// </summary>
		private void ConfigureService()
		{
			// Check we are already configuring service.
			if(this.IsConfiguring)	return;
			try
			{
				this.Timer.Stop();
				this.Tray.Icon = this.ServiceConfiguredIcon;
				this.IsConfiguring = true;
				// Getting service options.
				APCServiceOptions remoteOptions = ((APCServiceOptions)(System.Activator.GetObject(typeof(APCServiceOptions), String.Format("tcp://{0}:{1}/APCService.rem", this.APCServiceController.MachineName, this.APCSrvOpt.MachinePort))));
				// Copy all the values to a local class.
				APCServiceOptions localOptions = new APCServiceOptions();
				remoteOptions.CopyTo(localOptions);
				// Creating window.
				this.ConfigForm = new ConfigureForm(this.APCServiceController.ServiceName, this.APCServiceController.MachineName, localOptions);
				ConfigForm.BringToFront();
				ConfigForm.TopMost = true;
				ConfigForm.TopMost = false;
				DialogResult cfr = ConfigForm.ShowDialog();
				// User wants settings to be applied.
				if( (cfr == DialogResult.OK)|| (cfr == DialogResult.Yes) )
				{
					// Copy all values to remote class.
					localOptions.CopyTo(remoteOptions);
					localOptions.CopyTo(this.APCSrvOpt);
					// Restarting service.
					if(cfr.Equals(System.Windows.Forms.DialogResult.Yes)) this.CMIRestartService_Click(null, null);
				}
			}
			catch(System.Runtime.Remoting.RemotingException _x)
			{
				System.Diagnostics.Debug.WriteLine(_x.ToString());
				ShowMessage(MessageType.ErrorTCPConnection, null, "Connection to "+this.APCSrvOpt.MachineName+" is established, but the service options and service provider status cannot be received");
			}
			catch(System.Exception _x)
			{
				System.Diagnostics.Debug.WriteLine(_x.ToString());
				ShowMessage(MessageType.ErrorConfiguring, null, "Unable to configure APCService now");
			}
			finally
			{
				this.LastMessageShown = MessageType.None;
				this.LastKnownSPStatus = "UNKNOWN";
				this.IsConfiguring = false;
				this.Timer.Start();
			}
		}

		/// <summary>
		/// Refreshes APCService and service provider status.
		/// </summary>
		private void RefreshAPCServiceStatus()
		{
			// Check if we are configuring service.
			if(this.IsConfiguring) return;
			if(this.IsUpdating) return;
			try
			{
				IsUpdating = true;

				// Refreshing service status.
				this.APCServiceController.Refresh();
				ServiceControllerStatus st = this.APCServiceController.Status;
				switch(st)
				{
						// Stopped.
					case System.ServiceProcess.ServiceControllerStatus.Stopped:
						this.Tray.Text = String.Format("{0}{1}APCService: stopped", this.ToString(), Environment.NewLine);
						ShowMessage(MessageType.WarningServiceIsStopped, null, null);
						this.RunningStatusShown = false;
						break;

						// Running.
					case System.ServiceProcess.ServiceControllerStatus.Running:
						if ( this.RunningStatusShown )
						{
							string SPCurrentStatus;
							string SPInfo;
							try
							{
								// Trying to reget class from the service.
								if(this.SPst == null)
								{
									this.SPst = ((APCServiceProviderStatus)(System.Activator.GetObject(typeof(APCServiceProviderStatus), String.Format("tcp://{0}:{1}/APCServiceProviderStatus.rem", this.APCServiceController.MachineName, this.APCSrvOpt.MachinePort))));
								}
								SPCurrentStatus = this.SPst.Status;
								SPInfo = this.SPst.AdditionalInfo;
							}
							catch
							{
								SPCurrentStatus = "UNKNOWN";
								SPInfo = "UNKNOWN";
								this.SPst = null;
								this.Tray.Icon = this.ServiceNotControlledIcon;
							}
							this.Tray.Text = String.Format("{0}{1}APCService: running [{2}-{3}]", this.ToString(), Environment.NewLine, SPCurrentStatus, SPInfo);
							ShowMessage(MessageType.MessageSPStatus, SPCurrentStatus, SPInfo);
						}
						else
						{
							this.Tray.Text = String.Format("{0}{1}APCService: started", this.ToString(), Environment.NewLine);
							ShowMessage(MessageType.MessageServiceIsRunning, null, null);
							this.RunningStatusShown = true;
						}
						break;

						// Service is in state we can't do anything about it.
					default:
						// Change icon.
						this.Tray.Text = String.Format("{0}{1}APCService: pending action", this.ToString(), Environment.NewLine);
						this.Tray.Icon = this.ServiceIsPendingIcon;
						this.RunningStatusShown = false;
						break;
				}
			}
			catch(Exception _x)
			{
				System.Diagnostics.Debug.WriteLine(_x.ToString());
				ShowMessage(MessageType.ErrorConfiguring, null, "Unable to configure APCService now");
			}
			finally
			{
				IsUpdating = false;
			}
		}

		/// <summary>
		/// Class information as string (which machine is controlled).
		/// </summary>
		/// <returns>Information about controlled machine.</returns>
		public override string ToString()
		{
			return this.Info;
		}

		/// <summary>
		/// Occurs when user clicks twice in icon.
		/// </summary>
		/// <param name="sender">Sender of event.</param>
		/// <param name="id">ID of message target.</param>
		private void OnDoubleClick(object sender, EventArgs id)
		{
			this.ValidateContextMenuItems();
			if(this.CMIConfigureAPCService.Visible) 
				this.ConfigureService();
			else if (this.IsConfiguring == true) 
			{
				ConfigForm.BringToFront();
				ConfigForm.TopMost = true;
				ConfigForm.TopMost = false;
			}
		}

		/// <summary>
		/// Occurs on timer activity (one per second).
		/// </summary>
		/// <param name="sender">Sender of event.</param>
		/// <param name="e">Event arguments.</param>
		private void OnTimerTick(object sender, System.EventArgs e)
		{
			// Refreshing service status.
			this.RefreshAPCServiceStatus();
		}

		/// <summary>
		/// Validates context menu items depending the status of service or on argument.
		/// </summary>
		private void ValidateContextMenuItems()
		{
			System.ServiceProcess.ServiceControllerStatus st;
			this.CMIConfigureAPCService.Visible = false;
			this.CMIConfigureControlTool.Visible = false;
			this.CMISeparator1.Visible = false;
			this.CMIStartService.Visible = false;
			this.CMIStopService.Visible = false;
			this.CMIRestartService.Visible = false;
			this.CMISeparator2.Visible = false;
			if(this.IsConfiguring)
			{
				return;
			}
			// Service is not configuring now. Trying to get it status, so on...
			try
			{
				this.APCServiceController.Refresh();
				st = this.APCServiceController.Status;
			}
			catch(Exception _x)
			{
				System.Diagnostics.Debug.WriteLine(_x.ToString());
				// Not controlled at all.
				this.CMIConfigureControlTool.Visible = true;
				this.CMISeparator1.Visible = true;
				return;
			}
			switch(st)
			{
				case System.ServiceProcess.ServiceControllerStatus.Running:
				{
					// Running - can be stopped, restarted and configured.
					this.CMIConfigureAPCService.Visible = true;
					this.CMIConfigureControlTool.Visible = true;
					this.CMISeparator1.Visible = true;
					this.CMIStopService.Visible = true;
					this.CMIRestartService.Visible = true;
					this.CMISeparator2.Visible = true;
					break;
				}
				case System.ServiceProcess.ServiceControllerStatus.Stopped:
				{
					// Stopped - can only be started.
					this.CMIConfigureControlTool.Visible = true;
					this.CMISeparator1.Visible = true;
					this.CMIStartService.Visible = true;
					this.CMISeparator2.Visible = true;
					break;
				}
				default:
				{
					this.CMIConfigureControlTool.Visible = true;
					this.CMISeparator1.Visible = true;
					break;
				}
			}
		}

		/// <summary>
		/// About context menu item clicked.
		/// </summary>
		/// <param name="sender">Sender of the event.</param>
		/// <param name="e">Event parameters.</param>
		private void CMIAbout_Click(object sender, EventArgs e)
		{
			AboutForm af = new AboutForm(this.APCSrvOpt.MachineName);
			af.Show();
		}

		/// <summary>
		/// Exit context menu item clicked.
		/// </summary>
		/// <param name="sender">Sender of the event.</param>
		/// <param name="e">Event parameters.</param>
		private void CMIExit_Click(object sender, EventArgs e)
		{
			System.IO.FileStream fs = null;
			try
			{
				// Saving settings.
				fs = new System.IO.FileStream(iniFileName, System.IO.FileMode.Create, System.IO.FileAccess.Write, System.IO.FileShare.ReadWrite);
				SoapFormatter optFormatter = new SoapFormatter();
				optFormatter.Serialize(fs, this.APCSrvOpt);
			}
			catch (Exception _x)
			{
				System.Diagnostics.Debug.WriteLine(_x.ToString());
			}
			finally
			{
				if (fs != null) fs.Close();
				// Exit was for running configurating tool.
				if(this.RunAPCServiceControlConfiguration) System.Diagnostics.Process.Start(Application.ExecutablePath);
				// Exiting.
				System.Windows.Forms.Application.Exit();
			}
		}

		/// <summary>
		/// Configure Service context menu item clicked.
		/// </summary>
		/// <param name="sender">Sender of the event.</param>
		/// <param name="e">Event parameters.</param>
		private void CMIConfigureAPCService_Click(object sender, EventArgs e)
		{
			this.ConfigureService();
		}

		private void CMIConfigureControlTool_Click(object sender, EventArgs e)
		{
			// Setting flag to run configurator and exiting.
			this.RunAPCServiceControlConfiguration = true;
			this.CMIExit_Click(sender, e);
		}

		/// <summary>
		/// Start Service context menu item clicked.
		/// </summary>
		/// <param name="sender">Sender of the event.</param>
		/// <param name="e">Event parameters.</param>
		private void CMIStartService_Click(object sender, EventArgs e)
		{
			// Creating and showing starting form.
			this.Timer.Stop();
			ServicePendingForm spf = new ServicePendingForm(this.APCServiceController, ServiceAction.Start);
			if(spf.ShowDialog() == DialogResult.Cancel ) ShowMessage(MessageType.ErrorServiceActionTimeoutExpired, null, "starting");
			spf.Dispose();
			this.LastMessageShown = MessageType.None;
			this.LastKnownSPStatus = "UNKNOWN";
			this.Timer.Start();
		}

		/// <summary>
		/// Restart Service context menu item clicked.
		/// </summary>
		/// <param name="sender">Sender of the event.</param>
		/// <param name="e">Event parameters.</param>
		private void CMIRestartService_Click(object sender, EventArgs e)
		{
			this.Timer.Stop();
			// Creating and showing restarting form.
			ServicePendingForm spf = new ServicePendingForm(this.APCServiceController, ServiceAction.Restart);
			if(spf.ShowDialog() == DialogResult.Cancel ) ShowMessage(MessageType.ErrorServiceActionTimeoutExpired, null, "restarting");
			spf.Dispose();
			this.LastMessageShown = MessageType.None;
			this.LastKnownSPStatus = "UNKNOWN";
			this.Timer.Start();
		}

		/// <summary>
		/// Stop Service context menu item clicked.
		/// </summary>
		/// <param name="sender">Sender of the event.</param>
		/// <param name="e">Event parameters.</param>
		private void CMIStopService_Click(object sender, EventArgs e)
		{
			this.Timer.Stop();
			// Creating and showing stopping form.
			ServicePendingForm spf = new ServicePendingForm(this.APCServiceController, ServiceAction.Stop);
			if(spf.ShowDialog() == DialogResult.Cancel ) ShowMessage(MessageType.ErrorServiceActionTimeoutExpired, null, "stopping");
			spf.Dispose();
			this.LastMessageShown = MessageType.None;
			this.LastKnownSPStatus = "UNKNOWN";
			this.Timer.Start();
		}

		/// <summary>
		/// Occurs before context menu pops up. Validates menu items.
		/// </summary>
		/// <param name="sender">Sender of event.</param>
		/// <param name="e">Event parameters.</param>
		private void ContextMenu_Popup(object sender, EventArgs e)
		{
			this.ValidateContextMenuItems();
		}
	}

	}
