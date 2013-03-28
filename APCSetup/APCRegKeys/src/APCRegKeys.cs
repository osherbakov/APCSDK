//
// Usage: 
//
// APCRegkeys.exe [install option]
//
// where option is -install or -uninstall


using System;
using System.Windows.Forms;

namespace Diacom.APCRegKeys
{
	/// <summary>
	/// Creates or removes some registry values.
	/// </summary>
	internal class APCRegKeys
	{
		/// <summary>
		/// Creates registry values (APCService description and autorun for APCServiceControl).
		/// </summary>
		private static void CreateRegistryValues()
		{
			System.IO.FileInfo fi = null;
			// Autorun.
			fi = new System.IO.FileInfo(Application.StartupPath+System.IO.Path.DirectorySeparatorChar+"APCServiceControl.exe");
			// Searching for "APCServiceControl.exe".
			if(fi.Exists)
			{
				// Have one.
				try
				{
					Microsoft.Win32.RegistryKey AppStartUpKey = Microsoft.Win32.Registry.LocalMachine.CreateSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run");
					AppStartUpKey.SetValue("APCService Control", "\"" + Application.StartupPath + System.IO.Path.DirectorySeparatorChar +"APCServiceControl.exe\"" + " -systray");
				}
				catch
				{
				}
			}
			// Service description.
			fi = new System.IO.FileInfo(Application.StartupPath + System.IO.Path.DirectorySeparatorChar +"APCService.exe");
			// Searching for "APCService.exe".
			if(fi.Exists)
			{
				try
				{
					Microsoft.Win32.RegistryKey AppStartUpKey = Microsoft.Win32.Registry.LocalMachine.OpenSubKey("SYSTEM\\CurrentControlSet\\Services\\APCService", true);
					AppStartUpKey.SetValue("Description", "Provides software application programming control (APC) for telephony services");
				}
				catch
				{
				}
			}
		}

		/// <summary>
		/// Deletes registry values (APCService description and autorun for APCServiceControl).
		/// </summary>
		private static void DeleteRegistryValues()
		{
			// Autorun.
			try
			{
				Microsoft.Win32.RegistryKey AppStartUpKey = Microsoft.Win32.Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
				AppStartUpKey.DeleteValue("APCService Control");
			}
			catch
			{
			}
			// Service description.
			try
			{
				Microsoft.Win32.RegistryKey AppStartUpKey = Microsoft.Win32.Registry.LocalMachine.OpenSubKey("SYSTEM\\CurrentControlSet\\Services\\APCService", true);
				AppStartUpKey.DeleteValue("Description");
			}
			catch
			{
			}
		}

		private static void StartControlTool()
		{
			try
			{
				System.Diagnostics.Process.Start(System.Windows.Forms.Application.StartupPath+System.IO.Path.DirectorySeparatorChar+"APCServiceControl.exe", "-systray");
			}
			catch
			{
			}
		}

		private static void StopControlTool()
		{
			try
			{
				System.Diagnostics.Process.GetProcessesByName("APCServiceControl")[0].CloseMainWindow();
				if(!System.Diagnostics.Process.GetProcessesByName("APCServiceControl")[0].WaitForExit(1))
				{
					System.Diagnostics.Process.GetProcessesByName("APCServiceControl")[0].Kill();
				}
			}
			catch
			{
			}
		}

		/// <summary>
		/// Entry point for app.
		/// </summary>
		/// <param name="args">Commands application to perform.</param>
		/// <remarks>
		/// <para>
		/// Argument for command line should be following: "-install" or "-unistall" -- the action to perform.
		/// </para>
		/// </remarks>
		[STAThread]
		public static void Main(string [] args)
		{
			if(args.Length < 1) return;
			switch(args[0])
			{
				case "-install":
				{
					CreateRegistryValues();
					StartControlTool();
					break;
				}
				case "-uninstall":
				{
					DeleteRegistryValues();
					StopControlTool();
					break;
				}
			}
		}
	}
}
