using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Net.Sockets;
using System.Collections;

namespace Diacom.AltiGen
{
	/// <summary>
	/// Summary description for ExtensionManager.
	/// </summary>
	internal class AltiGenSPExtensionManager : AltiGenSPCore, Diacom.ExtensionManager.IXM
	{
		private System.Collections.Hashtable RingInfo = new Hashtable();

		protected override void ProcessEvent(Diacom.AltiGen.AltiLinkPlus.ALPEvent altiEvent)
		{
			switch((ALPEvID)(altiEvent.CommandId))
			{
				case ALPEvID.RING:
					CallInfo rInfo = new CallInfo(altiEvent[0]);
					AltiLinkPlus.ALPCommand ac = new AltiLinkPlus.ALPCommand(rInfo.lineID, (int)(ALPCmdID.GET_EXTRACALLINFO));
					ac[0] = new AltiLinkPlus.ALPParameter(rInfo.SessionHandle);
					RingInfo.Add(ac.SequenceId, rInfo);
					base.SendALPCommand(ac);
					break;

				case ALPEvID.STCHG:
					Diacom.ExtensionManager.StateChangedEventHandler ehsc = this.StateChanged;
					if(ehsc != null) 
					{
						SPLineState state = ((SPLineState)(altiEvent[0].ReadInt32()));
						Diacom.ExtensionManager.LineStateInfoEventArgs si = new Diacom.ExtensionManager.LineStateInfoEventArgs(state);
						ehsc(GetLine(altiEvent.LocationId), si);
					}
					break;
			}
			base.ProcessEvent(altiEvent);
		}

		protected override void ProcessResponse(Diacom.AltiGen.AltiLinkPlus.ALPResponse altiResponse)
		{
			switch((ALPCmdID)(altiResponse.CommandId))
			{
				case ALPCmdID.GET_EXTRACALLINFO:
					CallInfo rInfo = (CallInfo) RingInfo[altiResponse.SequenceId];
					RingInfo.Remove(altiResponse.SequenceId);
					Diacom.ExtensionManager.CallInfoEventArgs RingData = 
						new Diacom.ExtensionManager.CallInfoEventArgs(
							rInfo.callerName, rInfo.callerID, rInfo.callerPAD, rInfo.callerIPAddress,
							rInfo.calleeName, rInfo.calleeID, rInfo.calleePAD,
							rInfo.ANIName, rInfo.ANIID, rInfo.DNISName, rInfo.DNISID,
							rInfo.StartTime, rInfo.HoldTime, rInfo.CurrentTime,
							rInfo.WorkGroupNumber, rInfo.OutBoundWorkGroupNumber, rInfo.SessionHandle, rInfo.CallHandle);

					if(altiResponse.ResponseCode == 0)
					{
						int SessionID = altiResponse[0].ReadInt32();
						Diacom.ExtensionManager.ExtraCallInfo xci = 
							new Diacom.ExtensionManager.ExtraCallInfo(altiResponse[1].ReadString(),
							altiResponse[2].ReadString(), altiResponse[3].ReadString(),
							altiResponse[4].ReadString(), altiResponse[5].ReadString(),
							altiResponse[6].ReadString());

						RingData.ExtraInfo = xci;
					}

					Diacom.ExtensionManager.CallEventHandler ehr = this.Ring;
					if(ehr != null)
					{
						SPLine _line = GetLine(rInfo.lineID);
						_line.CalledNumber = rInfo.callerID;
						_line.CalledName = rInfo.callerName;
						_line.DNISNumber = rInfo.DNISID;
						_line.DNISName = rInfo.DNISName;
						_line.CIDNumber = rInfo.ANIID;
						_line.CIDName = rInfo.ANIName;
						_line.DIDNumber = rInfo.calleeID;
						_line.DIDName = rInfo.calleeName;

						ehr(_line, RingData);
					}
					break;
			}
			base.ProcessResponse(altiResponse);
		}

		public event Diacom.ExtensionManager.CallEventHandler Ring;

		public event Diacom.ExtensionManager.StateChangedEventHandler StateChanged;

		public void Logon(string serverIP, int serverPort, string account, string password)
		{
			base.Connect(serverIP, serverPort, SPLogonType.EXTENSION, account, password, 15000);
		}
		public void Logout()
		{
			base.Disconnect();
		}

		public void Call(object lineID, string number)
		{
			AltiLinkPlus.ALPCommand ac = new AltiLinkPlus.ALPCommand((int)(lineID), (int)(ALPCmdID.MAKECALL));
			ac[0] = new AltiLinkPlus.ALPParameter(number);
			ac[1] = new AltiLinkPlus.ALPParameter(1);
			ac[2] = new AltiLinkPlus.ALPParameter(String.Empty);
			ac[3] = new AltiLinkPlus.ALPParameter(String.Empty);
			ac[4] = new AltiLinkPlus.ALPParameter(1);
			base.SendALPCommandAndWait(ac, 0);
		}

		public void Redirect(object lineID, string number, int callID)
		{
			AltiLinkPlus.ALPCommand ac = new AltiLinkPlus.ALPCommand((int)(lineID), (int)(ALPCmdID.REDIRECT));
			ac[0] = new AltiLinkPlus.ALPParameter(callID);
			ac[1] = new AltiLinkPlus.ALPParameter(number);
			ac[2] = new AltiLinkPlus.ALPParameter(1);
			ac[3] = new AltiLinkPlus.ALPParameter(String.Empty);
			base.SendALPCommandAndWait(ac,0);
		}

		public void Drop(object lineID)
		{
			AltiLinkPlus.ALPCommand ac = new AltiLinkPlus.ALPCommand((int)(lineID), (int)(ALPCmdID.DROPCALL));
			ac[0] = new AltiLinkPlus.ALPParameter(1);
			base.SendALPCommandAndWait(ac,0);
		}

		public object GetLineID(string lineNumber)
		{
			return base.GetLineId(lineNumber);
		}
	}

	public class ExtensionManager : Diacom.ExtensionManager.IXM, IDisposable
	{
		private AltiGenSPExtensionManager XM = null;

		public ExtensionManager()
		{
			this.XM = new AltiGenSPExtensionManager();
		}

		public event Diacom.ExtensionManager.CallEventHandler Ring
		{
			add { this.XM.Ring += value; }
			remove { this.XM.Ring -= value; }
		}

		public event Diacom.ExtensionManager.StateChangedEventHandler StateChanged
		{
			add { this.XM.StateChanged += value; }
			remove { this.XM.StateChanged -= value; }
		}

		public event Diacom.SPStatusEventHandler SPStatusEvent
		{
			add { this.XM.SPStatusEvent += value; }
			remove { this.XM.SPStatusEvent -= value; }
		}

		public void Redirect(object lineID, string number, int callID)
		{
			this.XM.Redirect(lineID, number, callID);
		}

		public void Logout()
		{
			this.XM.Logout();
		}

		public void Logon(string serverIP, int serverPort, string account, string password)
		{
			this.XM.Logon(serverIP, serverPort, account, password);
		}

		public void Call(object lineID, string number)
		{
			this.XM.Call(lineID, number);
		}

		public void Drop(object lineID)
		{
			this.XM.Drop(lineID);
		}

		public SPLine GetLine(object lineID)
		{
			return this.XM.GetLine(lineID);
		}

		public Diacom.SPStatus Status()
		{
			return this.XM.Status();
		}

		public void Dispose()
		{
			this.XM.Dispose();
		}

		public object GetLineID(string lineNumber)
		{
			return this.XM.GetLineID(lineNumber);
		}
	}
}
