using System;
using System.Net;
using System.Runtime.Serialization;

[assembly: CLSCompliant(true)]

#pragma warning disable 1591

namespace Diacom.ExtensionManager
{
	[Serializable]
    public class Identity
	{
		private string _Name;
		private string _Number;

		public string Name { get { return this._Name; } }

		public string Number { get { return this._Number; } }

		public Identity(string name, string number)
		{
			this._Name = name;
			this._Number = number;
		}

		public override string ToString()
		{
			string str = "name: ";
			if(this._Name.Equals(String.Empty)) str += "N/A";
			else str += this._Name;
			str += ", number: ";
			if(this._Number.Equals(String.Empty)) str += "N/A";
			else str += this._Number;
			return str;
		}
	}

	[Serializable]
	public class CalleeIdentity : Identity
	{
		private object _LineID;

		public object LineID { get { return this._LineID; } }

		public CalleeIdentity(string name, string number, object lineID) : base(name, number)
		{
			this._LineID = lineID;
		}

		public override string ToString()
		{
			string str = "Callee-> name: ";
			if(base.Name.Equals(String.Empty)) str += "N/A";
			else str += base.Name;
			str += ", number: ";
			if(base.Number.Equals(String.Empty)) str += "N/A";
			else str += base.Number;
			str += ", line ID: "+this._LineID.ToString();
			return str;
		}
	}

	[Serializable]
	public class CallerIdentity : Identity
	{
		private object _LineID;
		private string _IPAddress;

		public IPAddress IP { get { return IPAddress.Parse(this._IPAddress); } }

		public CallerIdentity(string name, string number, object lineID, int sIPAddress) : base(name, number)
		{
			this._LineID = lineID;
			this._IPAddress = sIPAddress.ToString();
		}

		public override string ToString()
		{
			string str = "Caller-> name: ";
			if(base.Name.Equals(String.Empty)) str += "N/A";
			else str += base.Name;
			str += ", number: ";
			if(base.Number.Equals(String.Empty)) str += "N/A";
			else str += base.Number;
			str += ", line ID: "+this._LineID.ToString()+", IP address: "+this._IPAddress;
			return str;
		}

	}

	[Serializable]
	public class CallInfoEventArgs : EventArgs
	{
		private int _SessionHandle;
		private int _CallHandle;
		private string _WorkGroupNumber;
		private string _OutBoundWorkGroupNumber;
		private ExtraCallInfo _ExtraCallInfo;

		public Identity ANI;
		public Identity DNIS;
		public CalleeIdentity Callee;
		public CallerIdentity Caller;
		public DateTime StartTime;
		public DateTime HoldTime;
		public DateTime CreationTime;
		public string WorkGroupNumber { get { return this._WorkGroupNumber; } }
		public string OutBoundWorkGroupNumber { get { return this._OutBoundWorkGroupNumber; } }
		public int SessionID { get { return this._SessionHandle; } }
		public int CallID { get { return this._CallHandle; } }
		
		public CallInfoEventArgs(
			string callerName, string callerNumber, object callerLineID, int callerIPAddress, // Caller.
			string calleeName, string calleeNumber, object calleeLineID, //Callee.
			string ANIName, string ANINumber, // ANI.
			string DNISName, string DNISNumber, // DNIS.
			DateTime callStartTime, DateTime callHoldTime, DateTime callInfoCreationTime, // Times.
			string callWorkGroupNumber, string callOutBoundWorkGroupNumber, // WorkGroup.
			int sessionID, int ID) // Call.
		{
			this.Caller = new CallerIdentity(callerName, callerNumber, callerLineID, callerIPAddress);
			this.Callee = new CalleeIdentity(calleeName, calleeNumber, calleeLineID);
			this.ANI = new Identity(ANIName, ANINumber);
			this.DNIS = new Identity(DNISName, DNISNumber);
			this.StartTime = callStartTime;
			this.HoldTime = callHoldTime;
			this.CreationTime = callInfoCreationTime;
			this._WorkGroupNumber = callWorkGroupNumber;
			this._OutBoundWorkGroupNumber = callOutBoundWorkGroupNumber;
			this._SessionHandle = sessionID;
			this._CallHandle = ID;
		}
		public ExtraCallInfo ExtraInfo {get {return this._ExtraCallInfo;} set {this._ExtraCallInfo = value;} }

		public override string ToString()
		{
			string str = "Call information:"+Environment.NewLine;
			str += this.Caller.ToString()+Environment.NewLine;
			str += this.Callee.ToString()+Environment.NewLine;
			str += "ANI: "+this.ANI.ToString()+Environment.NewLine;
			str += "DNIS: "+this.DNIS.ToString()+Environment.NewLine;
			str += "Workgroup number: ";
			if(this._WorkGroupNumber.Equals(String.Empty)) str += "N/A";
			else str += this._WorkGroupNumber;
			str += ", outbound workgroup number: ";
			if(this._OutBoundWorkGroupNumber.Equals(String.Empty)) str += "N/A";
			else str += this._OutBoundWorkGroupNumber;
			str += Environment.NewLine;
			str += "Start: "+this.StartTime.ToString()+", hold: "+this.HoldTime.ToString()+", inf. created: "+this.CreationTime.ToString()+Environment.NewLine;
			str += "Call ID: "+this._CallHandle.ToString()+", session ID: "+this._SessionHandle.ToString();
			return str;
		}
	}

	[Serializable]
	public class ExtraCallInfo
	{
		public readonly string SourceURL;
		public readonly string SourceURLHistory;
		public readonly string FormData;
		public readonly string IVRData;
		public readonly string UserData;
		public readonly string HistoryData;

		public ExtraCallInfo(	
			string callSourceURL, string callSourceURLHistory,
			string callFormData, string callIVRData,
			string callUserData, string callHistoryData)
		{
			this.SourceURL = callSourceURL;
			this.SourceURLHistory = callSourceURLHistory;
			this.FormData = callFormData;
			this.HistoryData = callHistoryData;
			this.IVRData = callIVRData;
			this.UserData = callUserData;
		}
	}

	[Serializable]
	public class LineStateInfoEventArgs : EventArgs
	{
		public readonly SPLineState state;
		public LineStateInfoEventArgs(SPLineState state)
		{
			this.state = state;
		}
	}

	public delegate void CallEventHandler(object source, CallInfoEventArgs oCallInfo);
	public delegate void StateChangedEventHandler(object source, LineStateInfoEventArgs stateInfo);

    public interface IXM
	{
		event CallEventHandler Ring;
		event StateChangedEventHandler StateChanged;
        event Diacom.SPStatusEventHandler SPStatusEvent;

		void Logon(string serverIP, int serverPort, string account, string password);
		void Logout();
		SPLine GetLine(object lineID);
		object GetLineID(string lineNumber);
		SPStatus Status();
		void Call(object lineID, string number );
		void Redirect(object lineID, string number, int callID);
		void Drop(object lineID);
	}
}
