using System;

namespace Diacom
{
	/// <summary>
	/// Service Provider (SP) line state.
	/// </summary>
	public enum SPLineState
	{
		/// <summary>
		/// Line is idle.
		/// </summary>
		IDLE,

		/// <summary>
		/// EAP line state.
		/// </summary>
		EAP,

		/// <summary>
		/// Line is connected.
		/// </summary>
		CONNECTED,

		/// <summary>
		/// Line is in use.
		/// </summary>
		INUSE,

		/// <summary>
		/// Offhook.
		/// </summary>
		OFFHOOK,

		/// <summary>
		/// Pending call is on the line.
		/// </summary>
		CALLPENDING,

		/// <summary>
		/// Ring backs.
		/// </summary>
		RINGBACK,

		/// <summary>
		/// Conference on the line.
		/// </summary>
		CONFERENCING,

		/// <summary>
		/// Autoateddant on line.
		/// </summary>
		AA,

		/// <summary>
		/// Voice message on line.
		/// </summary>
		VM,

		/// <summary>
		/// Line is hold.
		/// </summary>
		HOLD,

		/// <summary>
		/// Pending hold is on the line.
		/// </summary>
		HOLDPENDING,

		/// <summary>
		/// Line is porceeding something.
		/// </summary>
		PROCEEDING,

		/// <summary>
		/// Line disconnects.
		/// </summary>
		DISCONNECT,

		/// <summary>
		/// Error on the line.
		/// </summary>
		ERROR,

		/// <summary>
		/// Ring on the line.
		/// </summary>
		RING,

		/// <summary>
		/// Line is parked.
		/// </summary>
		PARK,

		/// <summary>
		/// Softoffhook.
		/// </summary>
		SOFTOFFHOOK,

		/// <summary>
		/// Line holds music on.
		/// </summary>
		MUSICONHOLD,

		/// <summary>
		/// Line is waiting for onhook for XFER.
		/// </summary>
		XFER_WAITFORONHOOK,

		/// <summary>
		/// Ring is back on line for XFER.
		/// </summary>
		XFER_RINGBACK,

		/// <summary>
		/// Line is busy.
		/// </summary>
		BUSY,

		/// <summary>
		/// Something plays on the line.
		/// </summary>
		PLAY,

		/// <summary>
		/// Something records on the line.
		/// </summary>
		RECORD,

		/// <summary>
		/// Line is under APC control.
		/// </summary>
		APC,

		/// <summary>
		/// Voice recording started on the line.
		/// </summary>
		VOICE_RECORD_START,

		/// <summary>
		/// Voice recording ended on the line.
		/// </summary>
		VOICE_RECORD_STOP,

		/// <summary>
		/// Silent monitor on the line.
		/// </summary>
		SILENTMONITOR,

		/// <summary>
		/// Line is barged in.
		/// </summary>
		BARGEIN,

		/// <summary>
		/// Conference holded on the line (for conference mode).
		/// </summary>
		HOLD_CONFERENCE,

		/// <summary>
		/// Line is idle for offhook when hand free / dialtone disabled.
		/// </summary>
		HFDTD_IDLE,

		/// <summary>
		/// Playing call is finished on the line.
		/// </summary>
		PLAY_CALL_STOP,

		/// <summary>
		/// Line is playing call.
		/// </summary>
		PLAY_CALL,

		/// <summary>
		/// New line added.
		/// </summary>
		LINE_ADD,

		/// <summary>
		/// Line not accessible any more.
		/// </summary>
		LINE_REMOVE
	}

	/// <summary>
	/// Implements information about a line.
	/// </summary>
	public class SPLine : System.ICloneable
	{
		/// <summary>
		/// Access code for a line.
		/// </summary>
		public string AccessCode;

		/// <summary>
		/// Line identificator (ID).
		/// </summary>
		public object ID;

		/// <summary>
		/// Name of the line.
		/// </summary>
		public string Name;

		/// <summary>
		/// Line number.
		/// </summary>
		public string Number;

		/// <summary>
		/// Line PAD.
		/// </summary>
		public int PAD;

		/// <summary>
		/// Line port.
		/// </summary>
		public string Port;

		/// <summary>
		/// Line prefix.
		/// </summary>
		public string Prefix;

		/// <summary>
		/// Current line status (see <see cref="SPLineState"/> enumeration).
		/// </summary>
		public SPLineState State;

		/// <summary>
		/// Line type.
		/// </summary>
		public string Type;


		/// <summary>
		/// Call name.
		/// </summary>
		public string CalledName;

		/// <summary>
		/// Call number.
		/// </summary>
		public string CalledNumber;

		/// <summary>
		/// CID name.
		/// </summary>
		public string CIDName;

		/// <summary>
		/// CID number.
		/// </summary>
		public string CIDNumber;

		/// <summary>
		/// DID name.
		/// </summary>
		public string DIDName;

		/// <summary>
		/// DID number.
		/// </summary>
		public string DIDNumber;

		/// <summary>
		/// DNIS name.
		/// </summary>
		public string DNISName;

		/// <summary>
		/// DNIS number.
		/// </summary>
		public string DNISNumber;

		/// <summary>
		/// User name.
		/// </summary>
		public string UserName;

		/// <summary>
		/// User number.
		/// </summary>
		public string UserNumber;

		/// <summary>
		/// Creates a shallow copy of the current object.
		/// </summary>
		/// <returns>Copy of SPLine class.</returns>
		public object Clone()
		{
			return this.MemberwiseClone();
		}

		/// <summary>
		/// Format string for <see cref="ToString"/>() method.
		/// </summary>
		/// <remarks>
		/// Actually is the following string:
		/// <pre>  ID | Number     | Name                           | Port     | Prefix   | Status               | Type | User Name                      | User Number | Call Name                      | Call Number | CID Name                       | CID NUmber  | DID Name                       | DID Number  | DNIS Name                      | DNIS Number</pre>
		/// </remarks>
		public const string ToStringFormat = "  ID | Number     | Name                           | Port     | Prefix   | Status               | Type | User Name                      | User Number | Call Name                      | Call Number | CID Name                       | CID NUmber  | DID Name                       | DID Number  | DNIS Name                      | DNIS Number";

		/// <summary>
		/// Separator string for <see cref="ToString"/>() method.
		/// </summary>
		/// <remarks>
		/// Actually is the following string:
		/// <pre>----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------</pre>
		/// </remarks>
		public const string ToStringSeparator = "----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------";
		/// <summary>
		/// Converts content of class to string.
		/// </summary>
		/// <returns>String presentation of class properties.</returns>
		public override string ToString()
		{
			string str = this.ID.ToString().PadLeft(4) + " | ";
			str += this.Number.ToString().PadRight(10) + " | ";
			str += this.Name.ToString().PadRight(30) + " | ";
			str += this.Port.PadRight(8) + " | ";
			str += this.AccessCode.PadRight(8) + " | ";
			str += this.State.ToString().PadRight(20) + " | ";
			str += this.Type.PadRight(4) + " | ";
			str += this.UserName.PadRight(30) + " | ";
			str += this.UserNumber.PadRight(11) + " | ";
			str += this.CalledName.PadRight(30) + " | ";
			str += this.CalledNumber.PadRight(11) + " | ";
			str += this.CIDName.PadRight(30) + " | ";
			str += this.CIDNumber.PadRight(11) + " | ";
			str += this.DIDName.PadRight(30) + " | ";
			str += this.DIDNumber.PadRight(11) + " | ";
			str += this.DNISName.PadRight(30) + " | ";
			str += this.DNISNumber.PadRight(11);
			return str;
		}
	}
}
