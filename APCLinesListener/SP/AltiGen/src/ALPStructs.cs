using System;

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
			public LineInfo(AltiLinkPlus.ALPData data)
			{
				base.ToALPData(data.ReadBytes(Length));
				Position = 0; lineHandle = ReadInt32(); 
				Position = 8; lineType = ReadInt32(); 
				Position = 12; lineNumber = ReadString(12);
				Position = 33; firstName = ReadString(32);
				Position = 65; lastName = ReadString(32);
				physPort = ReadString(5); direction = ReadByte(); accessDigit = ReadByte(); lineSubType = ReadByte();
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
			/// Gets the size of the structure Line info.
			/// </summary>
			public override int Length { get { return 111; } }
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
			/// Caller ID.
			/// </summary>
			public string callerID = "";

			/// <summary>
			/// Caller name.
			/// </summary>
			public string callerName = "";

			/// <summary>
			/// Callee ID.
			/// </summary>
			public string calleeID = "";

			/// <summary>
			/// Callee name.
			/// </summary>
			public string calleeName = "";

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
			/// Access code.
			/// </summary>
			public string accessCode = "";

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
				Position = 0; lineID = ReadInt32();
				Position = 16; callerID = ReadString(41); 
				callerName = ReadString(65); 
				Position += 4; accessCode = ReadString(1); calleeID = ReadString(41); 
				calleeName = ReadString(65); 
				Position += 25; DNISName = ReadString(65); 
				DNISID = ReadString(41); 
				ANIName = ReadString(65); 
				ANIID = ReadString(41);
			}

			/// <summary>
			/// Gets all the data from Call Info structure.
			/// </summary>
			/// <returns>An array of bytes holding all data for Call Info structure.</returns>
			public override byte [] GetBytes()
			{
				base.Length = this.Length;
				Position = 0; Write(lineID);
				Position = 16; Write(callerID, 41); 
				Write(callerName, 65); 
				Position += 4; Write(accessCode,1); Write(calleeID, 41); 
				Write(calleeName, 65); 
				Position += 25; Write(DNISName, 65); 
				Write(DNISID, 41); 
				Write(ANIName, 65); 
				Write(ANIID, 41); 
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
		internal class StateInfo: AltiLinkPlus.ALPData
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
			public StateInfo()
			{
			}

			/// <summary>
			/// <para>Initializes a new instance of the class StateInfo from the ALPData storage.</para>
			/// <para>Reads the necessary number of bytes from the provided data storage and advances the current position.</para>
			/// </summary>
			/// <param name="data">Any object deriving from ALPData that holds the data.</param>
			public StateInfo(AltiLinkPlus.ALPData data)
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
			/// <param name="status">Command status.</param>
			public void Set(int seq, ALPCmdID id, object sender, ALPCmdStatus status)
			{
				ID = id;
				SenderLine = sender;
				SequenceID = seq;
				Status = status;
			}
		}

		/// <summary>
		/// Class for storing information about line.
		/// </summary>
		internal class Line
		{
			/// <summary>
			/// Shows if the line is uder APC.
			/// </summary>
			public bool IsUnderAPCControl = false;

			/// <summary>
			/// SPLine property for storing information about line.
			/// </summary>
			public SPLine Info = null;

			/// <summary>
			/// LastCommandStruct for storing information about last command ON THIS line. 
			/// </summary>
			public LastCommandStruct LastCommand = null;

			/// <summary>
			/// Based on line information constructor.
			/// </summary>
			/// <param name="lineInfo">Line information.</param>
			public Line(LineInfo lineInfo)
			{
				// Creating SPLine class.
				Info = new SPLine();
				// Creating LastCommand class.
				LastCommand = new LastCommandStruct();
				// Fill SPLine properties.
				Info.CalledName = "";
				Info.CalledNumber = "";
				Info.CIDName = "";
				Info.CIDNumber = "";
				Info.DIDName = "";
				Info.DIDNumber = "";
				Info.DNISName = "";
				Info.DNISNumber = "";
				Info.Name = String.Concat(((lineInfo.firstName == null) ? "" : lineInfo.firstName), " ", ((lineInfo.lastName == null) ? "" : lineInfo.lastName));
				Info.Number = ((lineInfo.lineNumber == null) ? "" : lineInfo.lineNumber);
				Info.ID = lineInfo.lineHandle;
				Info.State = SPLineState.IDLE;
				Info.Port = ((lineInfo.physPort == null) ? "" : lineInfo.physPort);
				Info.UserName = "";
				Info.UserNumber = "";
				switch(lineInfo.lineType)
				{
					case 6:		// Application control
						Info.Type = "C";
						Info.AccessCode = lineInfo.lineNumber;
						break;

					case 4:		// Virtual
						Info.Type = "V";
						Info.AccessCode = lineInfo.lineNumber;
						break;

					case 3:		// Workgroup or hunt group
						Info.Type = (lineInfo.lineSubType & 0x10) == 0x10 ? "W" : "H";
						Info.AccessCode = lineInfo.lineNumber;
						break;

					case 2:		// Trunk
						if(lineInfo.direction == 0x03)	// Paging trunk
						{
							Info.Type = "P";
							Info.AccessCode = "";
						}else if(lineInfo.direction == 0x00)	// Incoming only trunk
						{
							Info.Type = "T";
							Info.AccessCode = "";
						}else
						{
							Info.Type = "T";
							if( (lineInfo.accessDigit <= 0x09) && (lineInfo.accessDigit >= 0x00)) 
							{
								Info.AccessCode = lineInfo.accessDigit.ToString("D");
							}else
							{
								Info.AccessCode = "";
							}
						}
						break;

					case 1:		// Extension
						Info.Type = (lineInfo.lineSubType & 0x04) == 0x04 ? "I" : "E";
						Info.AccessCode = lineInfo.lineNumber;
						break;

					default:
						Info.AccessCode = "";
						Info.Type = "";
						break;
				}
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
	} // end of AltiGen namespace.
}