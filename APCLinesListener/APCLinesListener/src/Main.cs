using System;
using System.ComponentModel;

namespace APCLinesListener
{
	/// <summary>
	/// Implements all needfull options for SP connection.
	/// </summary>
	[Serializable]
	[DefaultProperty("ServerIPAddress")]
	public class ConnectionProperties
	{
		private string _IPAddress = "192.168.1.3";
		private int _Port = 10025;
		private string _Ext = "222";
		private string _Pswd = "2222";
		private Diacom.SPLogonType _Type = Diacom.SPLogonType.ADMINISTRATOR;
		private string _DefExt = "20";

		/// <summary>
		/// IP address of server.
		/// </summary>
		[Browsable(true)]
		[DefaultValue("192.168.1.3")]
		[Description("Server IP address.")]
		[Category("Service Provider connection options")]
		public string ServerIPAddress
		{
			get
			{
				return this._IPAddress;
			}
			set
			{
				this._IPAddress = value;
			}
		}

		/// <summary>
		/// Server port.
		/// </summary>
		[Browsable(true)]
		[DefaultValue(10025)]
		[Description("Server port number.")]
		[Category("Service Provider connection options")]
		public int ServerPortNumber
		{
			get
			{
				return this._Port;
			}
			set
			{
				this._Port = value;
			}
		}

		/// <summary>
		/// Control extension.
		/// </summary>
		[Browsable(true)]
		[DefaultValue("222")]
		[Description("Control extension (number)")]
		[Category("Logon options")]
		public string Extension
		{
			get
			{
				return this._Ext;
			}
			set
			{
				this._Ext = value;
			}
		}

		/// <summary>
		/// Password for control extension.
		/// </summary>
		[Browsable(true)]
		[DefaultValue("2222")]
		[Description("Control Extension Password")]
		[Category("Logon options")]
		public string Password
		{
			get
			{
				return this._Pswd;
			}
			set
			{
				this._Pswd = value;
			}
		}

		/// <summary>
		/// Type of logon.
		/// </summary>
		[Browsable(true)]
		[DefaultValue(Diacom.SPLogonType.ADMINISTRATOR)]
		[Description("Control Extension Logon Type")]
		[Category("Logon options")]
		public Diacom.SPLogonType LogonType
		{
			get
			{
				return this._Type;
			}
			set
			{
				this._Type = value;
			}
		}

		/// <summary>
		/// Default sender of all the commands.
		/// </summary>
		[Browsable(true)]
		[DefaultValue("20")]
		[Description("Default sender of commands")]
		[Category("Miscellaneous")]
		public string DefaultCommandSender
		{
			get
			{
				return this._DefExt;
			}
			set
			{
				this._DefExt = value;
			}
		}

		/// <summary>
		/// Returns a <see cref="System.String"/> that represents the current class instance.
		/// </summary>
		/// <returns>A <see cref="System.String"/> that represents the current class instance.</returns>
		public override string ToString()
		{
			return "IP: "+this._IPAddress+", port: "+this._Port.ToString()+", CE: "+this._Ext+", pswd: "+this._Pswd+", type: "+this._Type.ToString();
		}

	}

	/// <summary>
	/// Implements the main entry point for the application.
	/// </summary>
	public class CMain
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main() 
		{
			System.Windows.Forms.Application.Run(new APCLinesListenerMDIParent());
		}
	}
}
