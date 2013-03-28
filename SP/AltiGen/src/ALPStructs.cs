using System;
using System.Net;
using System.Collections;
using System.Collections.Specialized;

namespace Diacom
{
	namespace AltiGen
	{
		/// <summary>
		/// Implements the structure to hold the Line info.
		/// </summary>
		internal class LineInfo: AltiLinkPlus.ALPData
		{
			/// <summary>
			/// Line handle (line PAD).
			/// </summary>
			public int lineHandle;

			/// <summary>
			/// Type of the line.
			/// </summary>
			public int	lineType;

			/// <summary>
			/// Subtype of the line (unused).
			/// </summary>
			public byte lineSubType;

			/// <summary>
			/// Number of the line.
			/// </summary>
			public string lineNumber = "";

			/// <summary>
			/// First part of the line name.
			/// </summary>
			public string firstName = "";

			/// <summary>
			/// Second part of the line name.
			/// </summary>
			public string lastName = "";

			/// <summary>
			/// Phisical port for the line.
			/// </summary>
			public string physPort = "";

			/// <summary>
			/// Direction of the line.
			/// </summary>
			public byte direction;

			/// <summary>
			/// Access digit for the line.
			/// </summary>
			public byte accessDigit;

			/// <summary>
			/// Initializes a new instance of the class LineInfo with default paameters.
			/// </summary>
			public LineInfo()
			{
			}

			/// <summary>
			/// <para>Initializes a new instance of the class LineInfo from the ALPData storage.</para>
			/// <para>Reads the necessary number of bytes from the provided data storage and advances the current position.</para>
			/// </summary>
			/// <param name="data">Any object deriving from ALPData that holds the data.</param>
			/// <param name="versionID">ALPV2 version (differences in parameters for versions 2.7 and higher).</param>
			public LineInfo(AltiLinkPlus.ALPData data, int versionID)
			{
				// Length of the LineInfo stucture is based on the version of AltiLink:
				if(versionID > 0x26FF) this.Length = 111;
				else this.Length = 105;
				base.ToALPData(data.ReadBytes(Length));
				Position = 0; lineHandle = ReadInt32(); 
				Position = 8; lineType = ReadInt32(); 
				Position = 12; lineNumber = ReadString(12);
				Position = 33; firstName = ReadString(32);
				Position = 65; lastName = ReadString(32);
				physPort = ReadString(6); Position--; direction = ReadByte(); accessDigit = ReadByte(); lineSubType = ReadByte();
			}

			/// <summary>
			/// Gets all the data from Line Info structure.
			/// </summary>
			/// <returns>An array of bytes holding all data for Line Info structure.</returns>
			public override byte [] GetBytes()
			{
				Position = 0; Write(lineHandle); 
				Position = 8; Write(lineType); 
				Position = 12; Write(lineNumber,12);
				Position = 33; Write(firstName,32);
				Position = 65; Write(lastName,32);
				Write(physPort, 5); Write((byte)direction); Write((byte)accessDigit); Write((byte)lineSubType);
				return base.GetBytes();
			}

			/// <summary>
			/// Gets or sets the size of the structure Line info.
			/// </summary>
			public new int Length = 105;
		}

		/// <summary>
		/// Implements the structure to hold the Call info.
		/// </summary>
		internal class CallInfo: AltiLinkPlus.ALPData
		{
			/// <summary>
			/// Line ID.
			/// </summary>
			public int lineID;

			/// <summary>
			/// Caller line PAD.
			/// </summary>
			public int callerPAD;

			/// <summary>
			/// Caller IP address.
			/// </summary>
			public int callerIPAddress;

			/// <summary>
			/// Caller ID.
			/// </summary>
			public string callerID = "";

			/// <summary>
			/// Caller name.
			/// </summary>
			public string callerName = "";

			/// <summary>
			/// Callee line PAD.
			/// </summary>
			public int calleePAD;

			/// <summary>
			/// Access code.
			/// </summary>
			public string accessCode = "";

			/// <summary>
			/// Callee ID.
			/// </summary>
			public string calleeID = "";

			/// <summary>
			/// Callee name.
			/// </summary>
			public string calleeName = "";

			/// <summary>
			/// Workgroup PAD.
			/// </summary>
			public int workgroupPAD;

			/// <summary>
			/// Workgroup number.
			/// </summary>
			public string WorkGroupNumber = String.Empty;

			/// <summary>
			/// Outbound workgroup number.
			/// </summary>
			public string OutBoundWorkGroupNumber = String.Empty;

			/// <summary>
			/// DNIS ID.
			/// </summary>
			public string DNISID = "";

			/// <summary>
			/// DNIS name.
			/// </summary>
			public string DNISName = "";

			/// <summary>
			/// ANI ID.
			/// </summary>
			public string ANIID = "";

			/// <summary>
			/// ANI name.
			/// </summary>
			public string ANIName = "";

			/// <summary>
			/// The time the information was got.
			/// </summary>
			public DateTime CurrentTime;

			/// <summary>
			/// Call start time.
			/// </summary>
			public DateTime StartTime;

			/// <summary>
			/// The call was holded.
			/// </summary>
			public DateTime HoldTime;

			/// <summary>
			/// Call handle.
			/// </summary>
			public int CallHandle;

			/// <summary>
			/// Call session handle.
			/// </summary>
			public int SessionHandle;

			/// <summary>
			/// Initializes a new instance of the class CallInfo with default paameters.
			/// </summary>
			public CallInfo()
			{
			}

			/// <summary>
			/// <para>Initializes a new instance of the class CallInfo from the ALPData storage.</para>
			/// <para>Reads the necessary number of bytes from the provided data storage and advances the current position.</para>
			/// </summary>
			/// <param name="data">Any object deriving from ALPData that holds the data.</param>
			public CallInfo(AltiLinkPlus.ALPData data)
			{
				base.ToALPData(data.ReadBytes(Length));
				Position = 0;
				lineID = CallHandle = ReadInt32();
				SessionHandle = ReadInt32();
				callerPAD = ReadInt32();
				callerIPAddress = ReadInt32();
				callerID = ReadString(41); 
				callerName = ReadString(65); 
				calleePAD = ReadInt32();
				accessCode = ReadString(1);
				calleeID = ReadString(41); 
				calleeName = ReadString(65); 
				workgroupPAD = ReadInt32();
				WorkGroupNumber = ReadString(9);
				OutBoundWorkGroupNumber = ReadString(8);
				DNISName = ReadString(65); 
				DNISID = ReadString(41); 
				ANIName = ReadString(65); 
				ANIID = ReadString(41);
				this.CurrentTime =  ConvertTime.ToDateTime(ReadInt32());
				this.StartTime = ConvertTime.ToDateTime(ReadInt32());
				this.HoldTime = ConvertTime.ToDateTime(ReadInt32());
			}

			/// <summary>
			/// Gets all the data from Call Info structure.
			/// </summary>
			/// <returns>An array of bytes holding all data for Call Info structure.</returns>
			public override byte [] GetBytes()
			{
				base.Length = this.Length;
				Position = 0; 
				Write(CallHandle);
				Write(SessionHandle);
				Write(callerIPAddress);
				Write(callerID, 41); 
				Write(callerName, 65); 
				Write(calleePAD);
				Write(accessCode,1); 
				Write(calleeID, 41); 
				Write(calleeName, 65); 
				Write(workgroupPAD);
				Write(WorkGroupNumber, 9);
				Write(OutBoundWorkGroupNumber, 8);
				Write(DNISName, 65); 
				Write(DNISID, 41); 
				Write(ANIName, 65); 
				Write(ANIID, 41); 
				Write(BitConverter.GetBytes(ConvertTime.To_time_t(CurrentTime)));
				Write(BitConverter.GetBytes(ConvertTime.To_time_t(StartTime)));
				Write(BitConverter.GetBytes(ConvertTime.To_time_t(HoldTime)));
				return base.GetBytes();
			}

			/// <summary>
			/// Gets the size of the structure Callinfo.
			/// </summary>
			public override int Length { get { return 483; } }
		}


		/// <summary>
		/// Possible command statuses.
		/// </summary>
		internal enum ALPCmdStatus : int
		{
			/// <summary>
			/// Command failed.
			/// </summary>
			FAILED = -1,

			/// <summary>
			/// Command execution started.
			/// </summary>
			STARTED = 0,

			/// <summary>
			/// Command execution finished.
			/// </summary>
			FINISHED = 1,

			/// <summary>
			/// Command successfully executed.
			/// </summary>
			SUCCEED = 2,

			/// <summary>
			/// In voice message.
			/// </summary>
			INVM = 3,

			/// <summary>
			/// Out voice message.
			/// </summary>
			OUTVM = 4,
		}

		/// <summary>
		/// Implements the structure to hold the Status info.
		/// </summary>
		internal class LineStateInfo: AltiLinkPlus.ALPData
		{
			/// <summary>
			/// ID of the line.
			/// </summary>
			public int lineID;

			/// <summary>
			/// ID of the command.
			/// </summary>
			public int commandID;

			/// <summary>
			/// Status of the command.
			/// </summary>
			public int cmdStatus;

			/// <summary>
			/// Additional data.
			/// </summary>
			public int cmdData;

			/// <summary>
			/// Initializes a new instance of the class StateInfo with default paameters.
			/// </summary>
			public LineStateInfo()
			{
			}

			/// <summary>
			/// <para>Initializes a new instance of the class StateInfo from the ALPData storage.</para>
			/// <para>Reads the necessary number of bytes from the provided data storage and advances the current position.</para>
			/// </summary>
			/// <param name="data">Any object deriving from ALPData that holds the data.</param>
			public LineStateInfo(AltiLinkPlus.ALPData data)
			{
				base.ToALPData(data.ReadBytes(Length));
				Position = 0; lineID = ReadInt32();
				commandID = ReadInt32(); cmdStatus = ReadInt32(); cmdData = ReadInt32();
			}

			/// <summary>
			/// Gets all the data from Line Info structure.
			/// </summary>
			/// <returns>An array of bytes holding all data for Line Info structure.</returns>
			public override byte [] GetBytes()
			{
				base.Length = this.Length;
				Position = 0; Write(lineID);
				Write(commandID); Write(cmdStatus);	Write(cmdData);
				return base.GetBytes();
			}

			/// <summary>
			/// Gets the size of the structure Line info.
			/// </summary>
			public override int Length { get { return 16; } }
		}

		/// <summary>
		/// Implements command parameters for a line (mean command is FOR this line).
		/// </summary>
		internal class LastCommandStruct
		{
			/// <summary>
			/// Command ID.
			/// </summary>
			public ALPCmdID ID = ALPCmdID.NOT_A_COMMAND;

			/// <summary>
			/// Command sender line PAD.
			/// </summary>
			public object SenderLine = null;

			/// <summary>
			/// Command sequence ID.
			/// </summary>
			public int SequenceID = 0;

			/// <summary>
			/// Command status.
			/// </summary>
			public ALPCmdStatus Status = ALPCmdStatus.STARTED;

			/// <summary>
			/// Sets all the properties.
			/// </summary>
			/// <param name="seq">Command sequence ID.</param>
			/// <param name="id">Command ID (see <see cref="Diacom.AltiGen.ALPCmdID">ALPCmdID</see> enumeration).</param>
			/// <param name="sender">Command sender line PAD.</param>
			public void Set(int seq, ALPCmdID id, object sender)
			{
				ID = id;
				SenderLine = sender;
				SequenceID = seq;
			}
		}

		/// <summary>
		/// Class for storing information about line.
		/// </summary>
		internal class ALPLine
		{
			private Hashtable m_UserInfo =  new Hashtable();

			/// <summary>
			/// Shows if the line is under APC or not.
			/// </summary>
			public bool IsUnderAPCControl = false;

			public StringCollection Digits = new StringCollection();

			public ALPLine ConnectedLine = null;

			public enum CallInfoState
			{
				INITIAL,
				CALL_INFO_REQ_SENT,
				CALL_INFO_RECEIVED,
				CONNECT_DELAYED,
				SNATCHED_SLAVE_LINE
			}
			public CallInfoState InfoState = ALPLine.CallInfoState.INITIAL;

			/// <summary>
			/// SPLine property for storing information about line.
			/// </summary>
			public SPLine SPLineInfo = null;

			/// <summary>
			/// LastCommandStruct for storing information about last command ON THIS line. 
			/// </summary>
			public LastCommandStruct LastCommand = null;

			/// <summary>
			/// Based on line information constructor.
			/// </summary>
			/// <param name="lineInfo">Line information.</param>
			public ALPLine(LineInfo lineInfo)
			{
				// Creating SPLine class.
				SPLineInfo = new SPLine();
				// Creating LastCommand class.
				LastCommand = new LastCommandStruct();
				// Fill SPLine properties.
				SPLineInfo.CalledName = "";
				SPLineInfo.CalledNumber = "";
				SPLineInfo.CIDName = "";
				SPLineInfo.CIDNumber = "";
				SPLineInfo.DIDName = "";
				SPLineInfo.DIDNumber = "";
				SPLineInfo.DNISName = "";
				SPLineInfo.DNISNumber = "";
				SPLineInfo.Name = String.Concat(((lineInfo.firstName == null) ? "" : lineInfo.firstName), " ", ((lineInfo.lastName == null) ? "" : lineInfo.lastName));
				SPLineInfo.Number = ((lineInfo.lineNumber == null) ? "" : lineInfo.lineNumber);
				SPLineInfo.ID = lineInfo.lineHandle;
				SPLineInfo.State = SPLineState.IDLE;
				SPLineInfo.Port = ((lineInfo.physPort == null) ? "" : lineInfo.physPort);
				SPLineInfo.UserName = "";
				SPLineInfo.UserNumber = "";
				switch(lineInfo.lineType)
				{
					case 6:		// Application control
						SPLineInfo.Type = "C";
						SPLineInfo.AccessCode = lineInfo.lineNumber;
						break;

					case 4:		// Virtual
						SPLineInfo.Type = "V";
						SPLineInfo.AccessCode = lineInfo.lineNumber;
						break;

					case 3:		// Workgroup or hunt group
						SPLineInfo.Type = (lineInfo.lineSubType & 0x10) == 0x10 ? "W" : "H";
						SPLineInfo.AccessCode = lineInfo.lineNumber;
						break;

					case 2:		// Trunk
						if(lineInfo.direction == 0x03)	// Paging trunk
						{
							SPLineInfo.Type = "P";
							SPLineInfo.AccessCode = "";
						}else if(lineInfo.direction == 0x00)	// Incoming only trunk
						{
							SPLineInfo.Type = "T";
							SPLineInfo.AccessCode = "";
						}else
						{
							SPLineInfo.Type = "T";
							if( (lineInfo.accessDigit <= 0x09) && (lineInfo.accessDigit >= 0x00)) 
							{
								SPLineInfo.AccessCode = lineInfo.accessDigit.ToString("D");
							}else
							{
								SPLineInfo.AccessCode = "";
							}
						}
						break;

					case 1:		// Extension
						SPLineInfo.Type = (lineInfo.lineSubType & 0x04) == 0x04 ? "I" : "E";
						SPLineInfo.AccessCode = lineInfo.lineNumber;
						break;

					default:
						SPLineInfo.AccessCode = "";
						SPLineInfo.Type = "";
						break;
				}
			}
			public object this[object key]
			{
				get{ lock(m_UserInfo.SyncRoot){ return m_UserInfo[key]; } }
				set{ lock(m_UserInfo.SyncRoot){ m_UserInfo[key] = value; } }
			}
		}

		/// <summary>
		/// Structure for saving information about Dial and Ring Extension commands.
		/// </summary>
		internal class OutcallCommandStruct
		{
			/// <summary>
			/// Usable constructor. Sets all the fields as needed.
			/// </summary>
			/// <param name="sender">Line that sent that command and will receive the response.</param>
			/// <param name="targetLine">Line on which the command was sent.</param>
			/// <param name="commandID">Command ID.</param>
			public OutcallCommandStruct(object sender, object targetLine, Cmd.CommandID commandID)
			{
				Sender = sender;
				ID = commandID;
				Line = targetLine;
			}

			/// <summary>
			/// Sender ID - the ID of the line that sent the command.
			/// </summary>
			public readonly object Sender;

			/// <summary>
			/// The line on which that command was sent.
			/// </summary>
			public readonly object Line;

			/// <summary>
			/// Command ID.
			/// </summary>
			public readonly Cmd.CommandID ID;
		}

		
		/// <summary>
		/// Provides static function for time_t and DateTime conversion.
		/// </summary>
		public sealed class ConvertTime 
		{ 
			/// <summary> 
			/// Private constructor to prevent the class from being instantiated. 
			/// </summary> 
			private ConvertTime() {} 

			private static DateTime origin = System.TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1, 0, 0, 0)); 

			/// <summary> 
			/// time_t is an int representing the number of seconds since Midnight UTC 1 Jan 1970 on the Gregorian Calendar. 
			/// </summary> 
			/// <param name="time_t"></param> 
			/// <returns></returns> 
			public static DateTime ToDateTime(int time_t) 
			{ 
				DateTime convertedValue = origin + new TimeSpan(time_t * TimeSpan.TicksPerSecond); 
				if (System.TimeZone.CurrentTimeZone.IsDaylightSavingTime(convertedValue) == true) 
				{ 
					System.Globalization.DaylightTime daylightTime = System.TimeZone.CurrentTimeZone.GetDaylightChanges(convertedValue.Year); 
					convertedValue = convertedValue + daylightTime.Delta; 
				} 
				return convertedValue; 
			} 

			/// <summary> 
			/// time_t is an int representing the number of seconds since Midnight UTC 1 Jan 1970 on the Gregorian Calendar. 
			/// </summary> 
			/// <param name="time"></param> 
			/// <returns></returns> 
			public static int To_time_t(DateTime time) 
			{ 
				DateTime convertedValue = time; 
				if (System.TimeZone.CurrentTimeZone.IsDaylightSavingTime(convertedValue) == true) 
				{ 
					System.Globalization.DaylightTime daylightTime = System.TimeZone.CurrentTimeZone.GetDaylightChanges(convertedValue.Year); 
					convertedValue = convertedValue - daylightTime.Delta; 
				} 
				long diff = convertedValue.Ticks - origin.Ticks; 
				return (int)(diff / TimeSpan.TicksPerSecond); 
			} 
		} 

	} // end of AltiGen namespace.
}