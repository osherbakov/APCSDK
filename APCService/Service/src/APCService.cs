using System;
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.ServiceProcess;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Runtime.Serialization.Formatters;
using System.Runtime.Serialization.Formatters.Soap;


namespace Diacom.APCService
{
	/// <summary>
	/// Main class for windows service.
	/// </summary>
	internal class APCService : System.ServiceProcess.ServiceBase
	{
		private System.Threading.Thread mainThreadHandle = null;
		private TcpChannel TCPChannel = null;
		private readonly APCServiceOptions opt = new APCServiceOptions();
		private readonly APCServiceProviderStatus spStatus = new APCServiceProviderStatus();
		private System.ComponentModel.IContainer components = null;
		private SoapFormatter optFormatter = null;
		private int	TCPPort = 0;
		private readonly string TraceMessagesSeparator = Environment.NewLine+Environment.NewLine+Environment.NewLine+"********************************************************************************"+Environment.NewLine;
		private readonly ArrayList ActiveScripts = new ArrayList();
		private bool disposed = false;
		private readonly string iniConfigFileName = System.Windows.Forms.Application.StartupPath+System.IO.Path.DirectorySeparatorChar+"APCService.ini.xml";

		/// <summary>
		/// Initializes all class components.
		/// </summary>
		public APCService()
		{
			TraceOut.Put("Service Constructor called.");
			this.AutoLog = false;
			this.ServiceName = "APCService";
			ReadConfig();
			RegisterTCPChannel();
			RegisterRemoteTypes();
		}
		~APCService()
		{
			this.Dispose(false);
		}

		/// <summary>
		/// Cleans up any resources being used.
		/// </summary>
		protected override void Dispose(bool disposing)
		{
			TraceOut.Put("Service Dispose called. Disposing = " + disposing);
			if (disposed) return;
			disposed = true;
			GC.SuppressFinalize(this);
			if(disposing)
			{
				if(components != null) this.components.Dispose();
				if (this.RestartTimer != null)
				{
					this.RestartTimer.Dispose();
					this.RestartTimer = null;
				}
				UnRegisterRemoteTypes();
				UnRegisterTCPChannel();
				TraceOut.Put("Disposing all script handlers...");
				foreach(ScriptHandler sc in this.ActiveScripts)
				{
					try 
					{ 
						sc.SPStatusChanged -= new Diacom.APCService.OnSPStatusChange(SPStatusChangedHandler);
						sc.Dispose(); 
					}
					catch (Exception _e)
					{
						TraceOut.Put(_e);
					}
				}
			}
			base.Dispose(disposing);
		}

		/// <summary>
		/// Set things in motion so your service can do its work.
		/// </summary>
		protected override void OnStart(string[] args)
		{
			try
			{
				TraceOut.Put("[APCService] Starting.. Creating and starting main thread...");
				// Creating and starting main thread.
				this.mainThreadHandle = new System.Threading.Thread(new System.Threading.ThreadStart(this.StartHandlers));
				this.mainThreadHandle.SetApartmentState(System.Threading.ApartmentState.MTA);
				this.mainThreadHandle.Priority = System.Threading.ThreadPriority.Normal;
				this.mainThreadHandle.IsBackground = false;
				this.mainThreadHandle.Name = "APCSMT";
				this.mainThreadHandle.Start();
			}
			catch (Exception _e)
			{
				TraceOut.Put(_e);
			}
			finally
			{
				base.OnStart(args);
			}
		}
 
		/// <summary>
		/// Stop this service.
		/// </summary>
		protected override void OnStop()
		{
			try
			{
				TraceOut.Put("[APCService] Stopping ... Calling Dispose method...");
				WriteConfig();
				TraceOut.Put("Disposing all script handlers...");
				foreach(ScriptHandler sc in this.ActiveScripts)
				{
					try 
					{ 
						sc.SPStatusChanged -= new Diacom.APCService.OnSPStatusChange(SPStatusChangedHandler);
						sc.Dispose(); 
					}
					catch (Exception _e)
					{
						TraceOut.Put(_e);
					}
				}
				ActiveScripts.Clear();
			}
			catch (Exception _e)
			{
				TraceOut.Put(_e);
			}
			finally
			{
				base.OnStop();
			}
		}

		/// <summary>
		/// Reads options for APCService.
		/// </summary>
		private void ReadConfig()
		{
			System.IO.FileStream fs = null;
			try
			{
				TraceOut.Put("Reading configuration file...");
				// File exists - read from it.
				fs = new System.IO.FileStream(iniConfigFileName, System.IO.FileMode.Open, System.IO.FileAccess.Read, System.IO.FileShare.ReadWrite);
				optFormatter = new System.Runtime.Serialization.Formatters.Soap.SoapFormatter();
				APCServiceOptions _opt = ((APCServiceOptions)(optFormatter.Deserialize(fs)));
				_opt.CopyTo( this.opt);
				TraceOut.Put("Acting with following options: "+Environment.NewLine+this.opt.ToString());
			}
			catch(Exception ix)
			{
				// No file - setting default options.
				TraceOut.Put(ix);
				TraceOut.Put("Service Provider will be set by default...");
				APCServiceOptions _opt = new APCServiceOptions();
				_opt.CopyTo(this.opt);
			}
			finally
			{
				if (fs != null) fs.Close();
			}
		}

		private void WriteConfig()
		{
			System.IO.FileStream fs = null;
			try
			{
				// Saving settings.
				fs = new System.IO.FileStream(iniConfigFileName, System.IO.FileMode.Create, System.IO.FileAccess.Write, System.IO.FileShare.ReadWrite);
				optFormatter = new System.Runtime.Serialization.Formatters.Soap.SoapFormatter();
				optFormatter.Serialize(fs, opt);
				TraceOut.Put("Saving " + this.opt.ToString());
			}
			catch (Exception _e)
			{
				TraceOut.Put(_e);
				TraceOut.Put("Cannot save service configuration...");
			}
			finally
			{
				if (fs != null) fs.Close();
			}
		}

		/// <summary>
		/// Creats TCP channel for sharing APCServiceCore::GetOptions() 
		/// and APCServiceCore::SetOptions(...) functions.
		/// </summary>
		private void RegisterTCPChannel()
		{
			try
			{
				BinaryClientFormatterSinkProvider clientProvider = null;
				BinaryServerFormatterSinkProvider serverProvider = 	new BinaryServerFormatterSinkProvider();
				serverProvider.TypeFilterLevel = System.Runtime.Serialization.Formatters.TypeFilterLevel.Full;
				IDictionary props = new Hashtable();
				props["port"] = opt.MachinePort;
				props["exclusiveAddressUse"] = false;
				props["typeFilterLevel"] = System.Runtime.Serialization.Formatters.TypeFilterLevel.Full;

				this.TCPChannel = new TcpChannel( props, clientProvider, serverProvider );
				ChannelServices.RegisterChannel(this.TCPChannel, false);

				this.TCPPort = opt.MachinePort;
				TraceOut.Put("TCP channel registered...");
			}
			catch(Exception ix)
			{
				TraceOut.Put(ix);
			}
		}

		private void UnRegisterTCPChannel()
		{
			try
			{
				ChannelServices.UnregisterChannel(this.TCPChannel);
				this.TCPChannel = null;
				TraceOut.Put("TCP channel unregistered...");
			}
			catch(Exception ix)
			{
				TraceOut.Put(ix);
			}
		}

		/// <summary>
		/// Registers remote types.
		/// </summary>
		private void RegisterRemoteTypes()
		{
			try
			{
				RemotingConfiguration.RegisterWellKnownServiceType(typeof(APCServiceProviderStatus), 
											"APCServiceProviderStatus.rem", WellKnownObjectMode.Singleton);
				RemotingServices.Marshal(this.spStatus,"APCServiceProviderStatus.rem");

				RemotingConfiguration.RegisterWellKnownServiceType(typeof(APCServiceOptions), 
											"APCService.rem", WellKnownObjectMode.Singleton);
				RemotingServices.Marshal(this.opt, "APCService.rem");
				TraceOut.Put("Remote types registered...");
			}
			catch(Exception ix)
			{
				TraceOut.Put(ix);
			}
		}

		/// <summary>
		/// Unegisters remote types.
		/// </summary>
		private void UnRegisterRemoteTypes()
		{
			try
			{
				// Disconnect existing remote objects
				RemotingServices.Disconnect(this.opt);
				RemotingServices.Disconnect(this.spStatus);
				TraceOut.Put("Remote types unregistered...");
			}
			catch(Exception ix)
			{
				TraceOut.Put(ix);
			}
		}

		private System.Type SPType = null;
		
		private void SPStatusChangedHandler(object sender, SPStatusChangedEventArgs data)
		{
			TraceOut.Put("SPStatusChanged Event arrived. Status = " + data.spStatus.Status + ". Info = " + data.spStatus.AdditionalInfo);
			this.spStatus.Status = data.spStatus.Status;
			this.spStatus.AdditionalInfo = data.spStatus.AdditionalInfo;

			if(!spStatus.Status.ToUpper().Equals("OK"))
			{
				ScriptHandler currentSH = (ScriptHandler)sender;
				TraceOut.Put("Restarting connection: disposing objects, creating timer...");
				currentSH.SPStatusChanged -= new Diacom.APCService.OnSPStatusChange(SPStatusChangedHandler);
				this.ActiveScripts.Remove(currentSH);
				currentSH.Dispose();
				this.RestartTimer = new System.Threading.Timer(new System.Threading.TimerCallback(this.RestartTimerCallbackEntry), currentSH.FileName, 20000L, System.Threading.Timeout.Infinite);
			}
		}

		private System.Threading.Timer RestartTimer = null;

		/// <summary>
		/// Executes when timer ticks.
		/// </summary>
		/// <param name="state">Parameter, which is the name of the script file.</param>
		private void RestartTimerCallbackEntry(object state)
		{
			TraceOut.Put("RestartTimerCallbackEntry: restarting timer activated...");
			this.RestartTimer.Dispose();
			this.RestartTimer = null;
			ReadConfig();
			CreateScriptHandler((string) state);
		}

		private void CreateScriptHandler(string MainScript)
		{
			// Selecting service provider...
			string SPProviderFileName;
			AppDomain ad = AppDomain.CreateDomain("SP Files Search");
			{
				System.Type spsType = typeof(ServiceProviders);
				
				ServiceProviders sps = (ServiceProviders) ad.CreateInstanceAndUnwrap(spsType.Assembly.FullName, 
					spsType.Namespace + "." + spsType.Name);
				sps.FindProviders(System.Windows.Forms.Application.StartupPath);
				this.opt.SetSPTypesDinamically(sps.GetProviders());

				SPProviderFileName = sps.GetProviderFileName(this.opt.ServiceProviderType);
			}
			AppDomain.Unload(ad);

			if(SPProviderFileName == null)
			{
				TraceOut.Put("No Service provider is found - Exiting!!");
				return;
			}
			this.SPType = ServiceProviders.GetProviderType(SPProviderFileName);

			TraceOut.Put("Creating " + "\"" + MainScript + "\"" + " script handler...");
			ScriptHandler sh = new ScriptHandler(MainScript, this.ServiceName, this.SPType, this.opt);
			this.ActiveScripts.Add(sh);
			this.spStatus.Status = "OK";
			this.spStatus.AdditionalInfo = "Initializing";
			sh.SPStatusChanged += new Diacom.APCService.OnSPStatusChange(SPStatusChangedHandler);
			TraceOut.Put("Script handler for " + "\"" + MainScript + "\"" + " created.");
			sh.Start();
		}

		/// <summary>
		/// Main thread for APCService application.
		/// </summary>
		private void StartHandlers()
		{
			TraceOut.Put(this.TraceMessagesSeparator + " Starting execution...");
			// Reading configuration...
			ReadConfig();
			// Creating new instances for each main script file...
			foreach(string Script in this.opt.APCScriptFileName.Split(';'))
			{
				CreateScriptHandler(Script);
			}
			// All done.
			TraceOut.Put("All done.");
		}

		/// <summary>
		/// The main entry point for the process. Starts the service.
		/// </summary>
 		static void Main()
		{
			TraceOut.Put("Service Main called.");
			ServiceBase.Run(new APCService() );
		}
	}
	/// <summary>
	/// Represents the class to scan the directories for service providers.
	/// </summary>
	public class ServiceProviders : MarshalByRefObject
	{
		private StringCollection files = new StringCollection();
		private StringCollection types = new StringCollection();
		private Hashtable providers = new Hashtable();

		private void ExploreDirs(System.IO.DirectoryInfo startupDir)
		{
			foreach(System.IO.FileInfo file in startupDir.GetFiles("*.DLL"))
			{
				files.Add(file.FullName);
			}
			foreach(System.IO.DirectoryInfo subDir in startupDir.GetDirectories())
			{
				ExploreDirs(subDir);
			}
		}

		/// <summary>
		/// Gets the array of found service providers.
		/// </summary>
		/// <returns>A string array of Service Providers names.</returns>
		public string [] GetProviders()
		{
			string [] typesArray = new string[types.Count];
			types.CopyTo(typesArray, 0);
			return typesArray;
		}
		
		/// <summary>
		/// Initiates a search for all nested service providers starting from a specified directory.
		/// </summary>
		/// <param name="ProvidersDirectory">The Directory to start search in.</param>
		public void FindProviders(string ProvidersDirectory)
		{
			try
			{
				// Creating list of all assemblies in current directory.
				System.IO.DirectoryInfo startupDir = new System.IO.DirectoryInfo(ProvidersDirectory);
				// Generating possible service provider types list.
				ExploreDirs(startupDir);
				ScanDirs();
			}
			catch(Exception x)
			{
				TraceOut.Put(x);
			}
		}

		private void ScanDirs()
		{
			// Check if ISP interface is somewhere implemented.
			TraceOut.Put("Getting service providers...");
			foreach(string asmblPath in files)
			{
				TraceOut.Put("Loading Assembly :" + asmblPath + " ....");
				// Checking each assembly.
				try
				{
					System.Reflection.Assembly asmbl = System.Reflection.Assembly.LoadFrom(asmblPath);
					foreach(System.Reflection.Module module in asmbl.GetModules())
					{
						// Checking each module in assembly.
						foreach(System.Type type in module.GetTypes())
						{
							if (type.IsNotPublic) continue;
							// Checking each interface in module.
							foreach(System.Type interf in type.GetInterfaces())
							{
								// Checking each implemented interface in class.
								if(interf.Equals(typeof(ISP)))
								{
									TraceOut.Put("Class that implements interface: " + type.ToString());
									// ISP interface imlpemented, adding the class.
									string SPTitle = ((System.Reflection.AssemblyTitleAttribute)(asmbl.GetCustomAttributes(typeof(System.Reflection.AssemblyTitleAttribute), true)[0])).Title;
									types.Add(SPTitle);
									providers[SPTitle] = asmblPath;
									TraceOut.Put("Collecting service providers: \""+SPTitle+"\" added...");
								}
							}
						}
					}
				}
				catch(Exception x)
				{
					TraceOut.Put(x);
				}
			}
		}

		/// <summary>
		/// Returns service provider filename based on the Provider Name.
		/// </summary>
		/// <param name="ServiceProviderName">Name of the service provider.</param>
		/// <returns>Full filename to access a specified Service Provider.</returns>
		public string GetProviderFileName(string ServiceProviderName)
		{
			return (string) providers[ServiceProviderName];
		}

		/// <summary>
		/// Gets the type of the service provider given it's name.
		/// </summary>
		/// <param name="ProviderFileName">A type of the Service provider to instantiate.</param>
		/// <returns></returns>
		public static System.Type GetProviderType(string ProviderFileName)
		{
			try
			{
				System.Reflection.Assembly asmbl = System.Reflection.Assembly.LoadFrom(ProviderFileName);
				foreach(System.Reflection.Module module in asmbl.GetModules())
				{
					// Checking each module in assembly.
					foreach(System.Type type in module.GetTypes())
					{
						if (type.IsNotPublic) continue;
						// Checking each interface in module.
						foreach(System.Type interf in type.GetInterfaces())
						{
							// Checking each implemented interface in class.
							if(interf.Equals(typeof(ISP)))
							{
								TraceOut.Put("Type that implements interface: " + type.ToString());
								return type;
							}
						}
					}
				}
			}
			catch(Exception x)
			{
				TraceOut.Put(x);
			}
			return null;
		}
	}
}
