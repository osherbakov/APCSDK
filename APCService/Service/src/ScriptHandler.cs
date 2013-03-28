using System;
using System.Threading;

namespace Diacom.APCService
{
	internal class SPStatusChangedEventArgs : EventArgs
	{
		public APCServiceProviderStatus spStatus; 
		public SPStatusChangedEventArgs(string StatusText, string InfoText)
		{
			spStatus = new APCServiceProviderStatus();
			spStatus.Status = StatusText;
			spStatus.AdditionalInfo = InfoText;
		}
	}
	internal delegate void OnSPStatusChange(object sender, SPStatusChangedEventArgs eventData );

	/// <summary>
	/// Represents all needfull actions for script to run.
	/// </summary>
	internal class ScriptHandler : MarshalByRefObject
	{
		private ISP SPHandle = null;
		private APCStates.APCStateControl APCStCtrl = null;
		private System.Type SPType = null;
		private string ScriptFileName = String.Empty;
		private string ServiceName = String.Empty;
		private string StatusText = String.Empty;
		private string StatusInfo = String.Empty;

		private Thread StartThreadHandle = null;
		private APCServiceOptions opt = null;

		public event OnSPStatusChange SPStatusChanged;

		private void ReportStatus()
		{
			OnSPStatusChange eventCall = this.SPStatusChanged;
			if(eventCall != null) eventCall(this, new SPStatusChangedEventArgs(StatusText, StatusInfo));
		}

		/// <summary>
		/// Login to Server.
		/// </summary>
		/// <returns>True if success, otherwise false.</returns>
		private bool LoginToServer()
		{
			try
			{
				TraceOut.Put("Starting login...");
				// Connecting to server.
				SPHandle.Connect(opt.ServerIPAddress, opt.ServerPort, opt.ControlLineLogonType, opt.ControlLineID, opt.ControlLinePassword, opt.LogonTimeout);
				// Connection established.
				TraceOut.Put("Connection to server established... checking for connection status...");
				StatusText = this.SPHandle.Status().ToString();
				TraceOut.Put("Status of Service Provider is " + StatusText);
				if(StatusText != Diacom.SPStatus.OK.ToString())
				{
					StatusInfo = this.ServiceName + 
						" Error " + this.opt.ServiceProviderType.ToString()+" with : " + 
								Environment.NewLine + this.opt.ToString() + Environment.NewLine + 
									"Service provider status: " + this.SPHandle.Status();
				}else
				{
					StatusInfo = "Success";
				}
				ReportStatus();
				TraceOut.Put("Connection complete.");
			}
			catch(Exception x)
			{
				TraceOut.Put(x);
				StatusText = Diacom.SPStatus.ERROR_CONNECTION.ToString();
				StatusInfo = this.ServiceName+": Login Exception :" + Environment.NewLine + this.opt.ToString()+Environment.NewLine+"Service provider status: "+this.SPHandle.Status();
				ReportStatus();
				return false;
			}
			return true;
		}

		private void ServiceProviderStatusEventHandler(object source, Diacom.SPStatusEventArgs e)
		{
			if(e == null) return;
			StatusText = e.Status.ToString();
			StatusInfo = e.Info;
			TraceOut.Put("SPStatusEvent: Event: " + StatusText + Environment.NewLine +  "Info: " + StatusInfo);
			ReportStatus();
		}


		/// <summary>
		/// Cleans up any resources being used.
		/// </summary>
		public void Dispose()
		{
			TraceOut.Put("Shutting down script "+this.ScriptFileName);
			try 
			{ 
				TraceOut.Put("Calling this.APCStCtrl.Dispose");
				if(this.APCStCtrl != null) this.APCStCtrl.Dispose(); 
			} 
			catch(Exception ix)
			{
				TraceOut.Put(ix);
			}

			try 
			{ 
				TraceOut.Put("Calling this.SPCoreHandle.Disconnect");
				if(this.SPHandle != null)
				{
					this.SPHandle.SPStatusEvent -= new Diacom.SPStatusEventHandler(this.ServiceProviderStatusEventHandler);
					this.SPHandle.Disconnect(); 
					this.SPHandle.Dispose();
				}
			}
			catch(Exception ix)
			{
				TraceOut.Put(ix);
			}
		}

		public string FileName { get { return this.ScriptFileName; } }

		private void Run()
		{
			try
			{
				// Creating new instance of SP.
				TraceOut.Put("Creating new ISP instance...");
				this.SPHandle = ((ISP)(Activator.CreateInstance(this.SPType)));
				this.SPHandle.SPStatusEvent += new SPStatusEventHandler(this.ServiceProviderStatusEventHandler);
				TraceOut.Put("Event handler added...");
				// Creating control class.
				TraceOut.Put("Creating APCControl...");
				this.APCStCtrl = new APCStates.APCStateControl(this.SPHandle);
				this.APCStCtrl.APCControlEvent += new SPStatusEventHandler(this.ServiceProviderStatusEventHandler);
				TraceOut.Put("APCControl created...");
				// Adding states.
				TraceOut.Put("Added APC service states folders: "+this.opt.APCServiceStatesFolders+".");
				this.APCStCtrl.AddStates(this.opt.APCServiceStatesFolders);
				// Adding script.
				this.APCStCtrl.AddScript(this.ScriptFileName);
				TraceOut.Put("Main script file defined...");
				// Check if where is no connection.
				if(this.SPHandle.Status() == Diacom.SPStatus.DISCONNECTED)
				{
					TraceOut.Put("Main did not connect to the Server - Connecting ...");
					// No connection means the script uses default service provider.
					if(!LoginToServer())
					{
						TraceOut.Put("Connection failed.");
						return;
					}
					TraceOut.Put("Connected.");
				}
				// Adding lines.
				TraceOut.Put("Adding lines...");
				foreach(SPLine line in this.SPHandle.GetLines()) 
					this.APCStCtrl.AddLine(line);
				TraceOut.Put("Lines added.");
				// Initializating lines.
				this.APCStCtrl.InitLines();
				TraceOut.Put("Lines initialization complete.");
			}
			catch(Exception x)
			{
				TraceOut.Put("Exception during creating class");
				TraceOut.Put(x);
			}
		}

        /// <summary>
        /// Creates the instance of the class with specified parameters.
        /// </summary>
        /// <param name="aScriptFileName">The name of the main script file.</param>
        /// <param name="aServiceName">The name of current Service.</param>
        /// <param name="aSPType">The type of Service Provider for that script.</param>
        /// <param name="aAPCServiceOptions">The options (like Logon info and so on) of type <see cref="Diacom.APCService.APCServiceOptions"/>.</param>
        public ScriptHandler(string aScriptFileName, string aServiceName, System.Type aSPType, APCServiceOptions aAPCServiceOptions)
		{
			this.ScriptFileName = aScriptFileName;
			this.ServiceName = aServiceName;
			this.SPType = aSPType;
			this.opt = aAPCServiceOptions;

			this.StartThreadHandle = new Thread(new ThreadStart(this.Run));
			this.StartThreadHandle.IsBackground = true;
			this.StartThreadHandle.Name = "APCSSH:"+this.ScriptFileName.Substring(this.ScriptFileName.LastIndexOf(System.IO.Path.DirectorySeparatorChar)+1);
		}

		public void Start()
		{
			this.StartThreadHandle.Start();
			TraceOut.Put("[Script Handler]: thread started...");
		}
	}
}
