using System;
using System.ComponentModel;
using System.Windows.Forms.Design;
using System.Drawing.Design;
using System.Runtime.Serialization;

namespace Diacom.APCService
{
	/// <summary>
	/// Class used to pass the information about current SP status.
	/// </summary>
	public class APCServiceProviderStatus : MarshalByRefObject
	{
		/// <summary>
		/// Represents the status of the Service Provider.
		/// </summary>
		public string Status = "OK";
		/// <summary>
		/// Gives an additional Information for the status.
		/// </summary>
		public string AdditionalInfo = "UNAVAILABLE"; 
		/// <summary>
		/// Obtains a lifetime service object to control the lifetime policy for this instance.
		/// </summary>
		/// <returns>An object of type <see cref="System.Runtime.Remoting.Lifetime.ILease"/> or <see href="ms-help://MS.VSCC.2003/MS.MSDNQTR.2003APR.1033/csref/html/vclrfnull.htm">null</see> to indicate an indefinite life.</returns>
		public override object InitializeLifetimeService()
		{
			return null; // return null to live forever
		}
	}

	/// <summary>
	/// TypeConverter for service provider type.
	/// </summary>
	public class APCServiceProviderTypeConverter : StringConverter
	{
		/// <summary>
		/// Returns whether this object supports a standard set of values that can be picked from a list, using the specified context.
		/// </summary>
		/// <param name="context">An <see cref="ITypeDescriptorContext"/> that provides a format context.</param>
		/// <returns>true if <see cref="GetStandardValues"/> should be called to find a common set of values the object supports; otherwise, false.</returns>
		public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
		{
			return true;
		}

		/// <summary>
		/// Returns a collection of standard values for the data type this type converter is designed for when provided with a format context.
		/// </summary>
		/// <param name="context">An <see cref="ITypeDescriptorContext"/> that provides a format context that can be used to extract additional information about the environment from which this converter is invoked. This parameter or properties of this parameter can be a null reference (Nothing in Visual Basic).</param>
		/// <returns>A <see cref="TypeConverter.StandardValuesCollection"/> that holds a standard set of valid values, or a null reference (Nothing in Visual Basic) if the data type does not support a standard set of values.</returns>
		public override System.ComponentModel.TypeConverter.StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
		{
			return new System.ComponentModel.TypeConverter.StandardValuesCollection(APCServiceOptions.GetSPTypes());
		}
	}

	/// <summary>
	/// Class for type editing for folders list.
	/// </summary>
	public class APCServiceStatesEditor : UITypeEditor
	{
		/// <summary>
		/// Style of editing.
		/// </summary>
		/// <param name="context">Context of the execution.</param>
		/// <returns>Style of editing.</returns>
		public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
		{
			return UITypeEditorEditStyle.Modal;
		}

		/// <summary>
		/// Edits current value.
		/// </summary>
		/// <param name="context">Context of the execution.</param>
		/// <param name="provider">Service provider.</param>
		/// <param name="value">Value to edit.</param>
		/// <returns>New edited value.</returns>
		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
		{
			System.Windows.Forms.FolderBrowserDialog bd = new System.Windows.Forms.FolderBrowserDialog();
			bd.Description = "Select new scripts folder";
			bd.SelectedPath = System.Windows.Forms.Application.StartupPath;
			bd.ShowNewFolderButton = false;
			if(bd.ShowDialog().Equals(System.Windows.Forms.DialogResult.OK))
			{
				string dirlist = ((string)(value));
				if(!dirlist.Equals(String.Empty)) dirlist += ";";
				return dirlist+bd.SelectedPath;
			}
			else return value;
		}
	}

	/// <summary>
	/// APCService options class.
	/// </summary>
	[Serializable]
	public class APCServiceOptions : MarshalByRefObject, ISerializable
	{
		/// <summary>
		/// Obtains a lifetime service object to control the lifetime policy for this instance.
		/// </summary>
		/// <returns>An object of type <see cref="System.Runtime.Remoting.Lifetime.ILease"/> or <see href="ms-help://MS.VSCC.2003/MS.MSDNQTR.2003APR.1033/csref/html/vclrfnull.htm">null</see> to indicate an indefinite life.</returns>
		public override object InitializeLifetimeService()
		{
			return null; // return null to live forever
		}

		/// <summary>
		/// Service Provider type.
		/// </summary>
		[Browsable(true)]
		[DefaultValue("")]
		[Description("Type of Service Provider.")]
		[TypeConverter(typeof(APCServiceProviderTypeConverter))]
		[Category("Service Provider Type")]
		public string ServiceProviderType
		{
			get { return _ServiceProviderType; }
			set { _ServiceProviderType = value; }
		}

		/// <summary>
		/// Service Provider IP.
		/// </summary>
		[Browsable(true)]
		[DefaultValue("127.0.0.1")]
		[Description("IP address for connection to Service Provider.")]
		[Category("Service Provider connection Settings")]
		public string ServerIPAddress
		{
			get { return _ServerIPAddress; }
			set { _ServerIPAddress = value; }
		}

		/// <summary>
		/// Service Provider port.
		/// </summary>
		[Browsable(true)]
		[DefaultValue(10025)]
		[Description("Port for connection to Service Provider.")]
		[Category("Service Provider connection Settings")]
		public int ServerPort
		{
			get { return _ServerPort; }
			set { _ServerPort = value; }
		}

		/// <summary>
		/// Service Provider control line ID.
		/// </summary>
		[Browsable(true)]
		[DefaultValue("222")]
		[Description("Service Provider control account (line) ID.")]
		[Category("Service Provider connection Settings")]
		public string ControlLineID
		{
			get { return _ControlLineID; }
			set { _ControlLineID = value; }
		}


		/// <summary>
		/// Service Provider control account password.
		/// </summary>
		[Browsable(true)]
		[DefaultValue("2222")]
		[Description("Service Provider control account password.")]
		[Category("Service Provider connection Settings")]
		public string ControlLinePassword
		{
			get { return _ControlLinePassword; }
			set { _ControlLinePassword = value; }
		}

		/// <summary>
		/// Logon type.
		/// </summary>
		[Browsable(true)]
		[DefaultValue(SPLogonType.ADMINISTRATOR)]
		[Description("Type of logon to Service Provider.")]
		[Category("Service Provider connection Settings")]
		public SPLogonType ControlLineLogonType
		{
			get { return _ControlLineLogonType; }
			set { _ControlLineLogonType = value; }
		}

		/// <summary>
		/// Timeout.
		/// </summary>
		[Browsable(true)]
		[DefaultValue(15000)]
		[Description("Timeout for connection to Service Provider (milliseconds).")]
		[Category("Service Provider connection Settings")]
		public int LogonTimeout
		{
			get { return _LogonTimeout; }
			set { _LogonTimeout = value; }
		}

		/// <summary>
		/// Possible states files folders.
		/// </summary>
		[Browsable(true)]
		[Category("Service Provider Settings")]
		[DefaultValue(".\\Scripts\\")]
		[Editor(typeof(APCServiceStatesEditor), typeof(UITypeEditor))]
		[Description("Folders list for APC service states.")]
		public string APCServiceStatesFolders
		{
			get { return _APCServiceStatesFolders; }
			set { _APCServiceStatesFolders = value; }
		}

		/// <summary>
		/// Main script file for execution.
		/// </summary>
		[Browsable(true)]
		[Category("Service Provider Settings")]
		[DefaultValue("MainScript.vb")]
		[Editor(typeof(FileNameEditor), typeof(UITypeEditor))]
		[Description("Main script file for execution.")]
		public string APCScriptFileName
		{
			get { return _APCScriptFileName; }
			set { _APCScriptFileName = value; }
		}
	
		/// <summary>
		/// Name of the computer the service is running on.
		/// </summary>
		[Browsable(false)]
		public string MachineName
		{
			get { return _MachineName; }
			set { _MachineName = value; }
		}
		
		/// <summary>
		/// Port to connect to service.
		/// </summary>
		[Browsable(false)]
		public int MachinePort
		{
			get { return _MachinePort; }
			set { _MachinePort = value; }
		}

		#region Storages for parameters
		/// <summary>
		/// Service provide type.
		/// </summary>
		private static string _ServiceProviderType = "AltiGen Service Provider";
		
		/// <summary>
		/// AltiGen server IP.
		/// </summary>
		private static string _ServerIPAddress = "192.168.1.3";

		/// <summary>
		/// AltiGen server port.
		/// </summary>
		private static int _ServerPort = 10025;

		/// <summary>
		/// AltiGen server control line ID.
		/// </summary>
		private static string _ControlLineID = "222";

		/// <summary>
		/// AltiGen server control password.
		/// </summary>
		private static string _ControlLinePassword = "2222";

		/// <summary>
		/// Admin logon type.
		/// </summary>
		private static SPLogonType _ControlLineLogonType = SPLogonType.ADMINISTRATOR;

		/// <summary>
		/// Timeout.
		/// </summary>
		private static int _LogonTimeout = 60000;

		/// <summary>
		/// Directories with scripts.
		/// </summary>
		private static string _APCServiceStatesFolders = "";

		/// <summary>
		/// Main script file for execution.
		/// </summary>
		private static string _APCScriptFileName = "";

		/// <summary>
		/// Storage for service provider's types.
		/// </summary>
		private static string [] _SPTypes = null;

		/// <summary>
		/// Name of the computer the service is running on.
		/// </summary>
		private static string _MachineName = "127.0.0.1";

		/// <summary>
		/// Port to connect to service.
		/// </summary>
		private static int _MachinePort = 8088;
		#endregion

		/// <summary>
		/// Creates class instance with default parameters.
		/// </summary>
		public APCServiceOptions()
		{
		}

		/// <summary>
		/// Gets types of service providers which could be used.
		/// </summary>
		/// <returns>Types of service providers which could be used.</returns>
		public static string [] GetSPTypes()
		{
			return _SPTypes;
		}

		/// <summary>
		/// Sets types of service providers which could be used.
		/// </summary>
		/// <param name="aSPTypes">Types of service providers which could be used.</param>
		public static void SetSPTypes(string [] aSPTypes)
		{
			_SPTypes = aSPTypes;
		}

		/// <summary>
		/// Gets types of service providers which could be used.
		/// </summary>
		/// <returns>Types of service providers which could be used.</returns>
		public string [] GetSPTypesDinamically()
		{
			return _SPTypes;
		}

		/// <summary>
		/// Sets types of service providers which could be used.
		/// </summary>
		/// <param name="aSPTypes">Types of service providers which could be used.</param>
		public void SetSPTypesDinamically(string [] aSPTypes)
		{
			_SPTypes = aSPTypes;
		}

		/// <summary>
		/// Represents class information as text.
		/// </summary>
		/// <returns>Class information.</returns>
		public override string ToString()
		{
			string result = "APCService configuration:"+Environment.NewLine;
			result += "Control line ID: "+this.ControlLineID+Environment.NewLine;
			result += "Control line logon type: "+this.ControlLineLogonType+Environment.NewLine;
			result += "Control line password: "+this.ControlLinePassword+Environment.NewLine;
			result += "Logon timeout: "+this.LogonTimeout+Environment.NewLine;
			result += "Server IP address: "+this.ServerIPAddress+Environment.NewLine;
			result += "Server port: "+this.ServerPort+Environment.NewLine;
			result += "Service Provider type: "+this.ServiceProviderType+Environment.NewLine;
			result += "Main script file path: "+this.APCScriptFileName+Environment.NewLine;
			result += "Additional scripts folders: "+this.APCServiceStatesFolders+Environment.NewLine;
			return result;
		}

		/// <summary>
		/// Copies data from specified class.
		/// </summary>
		/// <param name="aDestination">Destination class.</param>
		public  void CopyTo( APCServiceOptions aDestination)
		{
			aDestination.APCScriptFileName = this.APCScriptFileName;
			aDestination.APCServiceStatesFolders = this.APCServiceStatesFolders;
			aDestination.ControlLineID = this.ControlLineID;
			aDestination.ControlLineLogonType = this.ControlLineLogonType;
			aDestination.ControlLinePassword = this.ControlLinePassword;
			aDestination.LogonTimeout = this.LogonTimeout;
			aDestination.ServerIPAddress = this.ServerIPAddress;
			aDestination.ServerPort = this.ServerPort;
			aDestination.ServiceProviderType = this.ServiceProviderType;
			aDestination.SetSPTypesDinamically(this.GetSPTypesDinamically());
			aDestination.MachineName = this.MachineName;
			aDestination.MachinePort = this.MachinePort;
		}

		#region ISerializable Members
		/// <summary>
		/// Creates class instance based on serialization information.
		/// </summary>
		/// <param name="info">Serialization information.</param>
		/// <param name="context">Serialization context.</param>
		protected APCServiceOptions(SerializationInfo info, StreamingContext context)
		{
			this.APCScriptFileName = info.GetString("APCScriptFileName");
			this.APCServiceStatesFolders = info.GetString("APCServiceStatesFolders");
			this.ControlLineID = info.GetString("ControlLineID");
			this.ControlLineLogonType = ((SPLogonType)(info.GetValue("ControlLineLogonType", typeof(SPLogonType))));
			this.ControlLinePassword = info.GetString("ControlLinePassword");
			this.LogonTimeout = info.GetInt32("LogonTimeout");
			this.ServerIPAddress = info.GetString("ServerIPAddress");
			this.ServerPort = info.GetInt32("ServerPort");
			this.ServiceProviderType = info.GetString("ServiceProviderType");
			this.MachineName = info.GetString("MachineName");
			this.MachinePort = info.GetInt32("MachinePort");
		}

		/// <summary>
		/// Populates a SerializationInfo with the data needed to serialize the target object.
		/// </summary>
		/// <param name="info">The <see cref="SerializationInfo"/> to populate with data.</param>
		/// <param name="context">The destination (see <see cref="StreamingContext"/>) for this serialization.</param>
		[System.Security.Permissions.SecurityPermissionAttribute(System.Security.Permissions.SecurityAction.Demand, SerializationFormatter = true)]
		public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue("APCScriptFileName", this.APCScriptFileName);
			info.AddValue("APCServiceStatesFolders", this.APCServiceStatesFolders);
			info.AddValue("ControlLineID", this.ControlLineID);
			info.AddValue("ControlLineLogonType", this.ControlLineLogonType);
			info.AddValue("ControlLinePassword", this.ControlLinePassword);
			info.AddValue("LogonTimeout", this.LogonTimeout);
			info.AddValue("ServerIPAddress", this.ServerIPAddress);
			info.AddValue("ServerPort", this.ServerPort);
			info.AddValue("ServiceProviderType", this.ServiceProviderType);
			info.AddValue("MachineName", this.MachineName);
			info.AddValue("MachinePort", this.MachinePort);
		}
		#endregion
	}
}

