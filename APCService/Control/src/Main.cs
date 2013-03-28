using System;
using System.Windows.Forms;

namespace Diacom.APCService.Control
{
	/// <summary>
	/// Represents entry point for APCServiceTrayController application class.
	/// </summary>
	public class CMain
	{
		/// <summary>
		/// Entry point for APCServiceTrayController application.
		/// </summary>
		[STAThread]
		public static void Main(string [] args)
		{
			string CurrentProcessName = System.Diagnostics.Process.GetCurrentProcess().ProcessName;
			// Check if where is other copy of application (remember one of processes we are).
			if(System.Diagnostics.Process.GetProcessesByName(CurrentProcessName).Length > 1)
			{
				// Have a process. Maybe we were started by this process? - when it should finish soon. Lets wait sometime..
				System.Threading.Thread.Sleep(5000);
				if(System.Diagnostics.Process.GetProcessesByName(CurrentProcessName).Length > 1)
				{
					int id = System.Diagnostics.Process.GetCurrentProcess().Id;
					// Killing running copies.
					foreach(System.Diagnostics.Process copy in System.Diagnostics.Process.GetProcessesByName(CurrentProcessName))
					{
						// Not this process?
						if(copy.Id != id) copy.Kill();
					}
				}
			}
			// Ruinning depending on command line arguments.
			if((args.Length == 1)&&(args[0].Equals("-systray")))
			{
				Application.Run(new APCServiceTrayForm());
			}
			else
			{
				Application.Run(new APCServiceControlConfiguratorForm());
			}
		}
	}
}
