using System;
using System.Threading;
using Diacom;
using Diacom.AltiGen;

namespace Diacom
{
	/// <summary>
	/// Defines the module that communicates with Service Provider (SP) module.
	/// </summary>
	public class SPCommandModuleExe
	{
		private Diacom.APCStates.APCStateControl ww;
		private Diacom.ISP currentSP;
		/// <summary>
		/// Initializes a new instance of <see cref="SPCommandModuleExe"/> class with no parameters.
		/// </summary>
		public SPCommandModuleExe()
		{
		}

		/// <summary>
		/// Main entry point to the executable.
		/// </summary>
		public static void Main()
		{
			SPCommandModuleExe zzz = new SPCommandModuleExe();
			zzz.DoIt();
			System.Windows.Forms.Application.Run();
		}
		
		private void DoIt()
		{

//			currentUser = new Diacom.AltiGen.ExtensionManager();
//			currentUser.Connect("192.168.1.3",10025, "216", "2710");
//			currentUser.Ring +=new Diacom.ExtensionManager.CallEventHandler(currentUser_Ring);

//			currentSP = new Diacom.AltiGen.ASPHdw();
			currentSP = new Diacom.AltiSDK.AltiSDKSP();
//			currentSP.Connect("213.208.165.202",10025, "6666", "2222");
			currentSP.Connect("192.168.1.3",10025, Diacom.SPLogonType.ADMINISTRATOR, "222", "2222", 10000);
//			currentSP.Connect("10.33.2.21",10025, Diacom.SPLogonType.ADMINISTRATOR, "3333", "2222", 50);
//			currentSP.Connect("212.118.47.196",10025, Diacom.SPLogonType.ADMINISTRATOR, "5555", "2638374", 15000);
			if( currentSP.Status() == Diacom.SPStatus.OK)
			{
				ww = new Diacom.APCStates.APCStateControl(currentSP);
				ww.AddStates(@"C:\AINF\");
//				ww.AddScript(@"C:\AINF\TESTSCRIPT.DLL");
//				ww.AddScript(@"C:\class1.vb");
//              ww.AddScript(@"C:\APCSDK\APCExe\AEPSCRIPT\MainScript.vb");
                ww.AddScript(@"H:\APCSDK.NET2\APCExe\CS_TEST\CS_TEST\Class1.cs");
//              ww.AddScript(@"H:\APCSDK.NET2\APCExe\JS_TEST\JS_TEST\Class1.jsl");
//              ww.AddScript(@"H:\APCSDK.NET2\APCExe\JS_TEST\JS_TEST\Class1.jsc");
//				ww.AddScript(@"C:\APCSDK\APCExe\AEPSCRIPT\bin\AEPScript.dll");
			
//				ww.AddScript(@"C:\APCSDK\Scripts\TRIUMPH\script.vb");

//				ww.AddScript(@"C:\APCSDK\Scripts\TRIUMPH\bin\Triumph.dll");
			
//				ww.AddScript(@"C:\APCSDK\APCExe\TESTSCRIPT\bin\TESTSCRIPT.DLL");
//				ww.AddScript(@"C:\AINF.vb");
				foreach (SPLine _line in currentSP.GetLines())
				{
					ww.AddLine(_line);
				}
				ww.InitLines();
/*
				string res;
				res = ww.Convert("Test", "card", 566002);
				res = ww.Convert("Test", "%card", 32667323);
				res = ww.Convert("Test", "ord", 234334532);
				res = ww.Convert("Test", "card", 56600);
				res = ww.Convert("Test", "card", 6000);
				res = ww.Convert("Test", "card", 800);
				res = ww.Convert("Test", "Dozens", 190);
				res = ww.Convert("Test", "Checks1", 666928923.4765);
				res = ww.Convert("Test", "Checks2", 666928923.4765);
				res = ww.Convert("Test", "Hours", 3710);
				res = ww.Convert("Test", "Stockquotes", 34543.75);
				res = ww.Convert("Test", "Roman", 1925);
				res = ww.Convert("Test", "Hours", 33);
				res = ww.Convert("Test", "SSN", 614285427);

				res = ww.Convert("Test", "date", DateTime.Now);
				res = ww.Convert("Test", "time", DateTime.Now);
				res = ww.Convert("Test", "Duration", 61428);
	
				res = ww.Convert("en", "main", 566002);
				res = ww.Convert("en", "main", 32667323);
				res = ww.Convert("en", "main", 234334532);
				res = ww.Convert("en", "main", 190);
				res = ww.Convert("en", "main", 666928923.4765);
				res = ww.Convert("fr", "main", 666928923.4765);
				res = ww.Convert("fr", "main", 3710);
				res = ww.Convert("de", "main", 34543.75);
				res = ww.Convert("de", "main", 1925);
				res = ww.Convert("it", "main", 33003);
				res = ww.Convert("it", "main", 12211);
				res = ww.Convert("it", "main", 554312);

				res = ww.Convert("Test", "Hours", 33);
*/
			}
		}

		private void currentUser_Ring(object source, Diacom.ExtensionManager.CallInfoEventArgs oCallInfo)
		{
			SPLine _Line = (SPLine) source;
	
		}
	}
}
