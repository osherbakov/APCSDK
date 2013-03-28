using System;

namespace Diacom
{
	/// <summary>
	/// Possible logon types.
	/// </summary>
	public enum SPLogonType
	{
		/// <summary>
		/// Logon as extension.
		/// </summary>
		EXTENSION,

		/// <summary>
		/// Logon as agent.
		/// </summary>
		AGENT,

		/// <summary>
		/// Logon as supervisor.
		/// </summary>
		SUPERVISOR,

		/// <summary>
		/// Logon as IP extension.
		/// </summary>
		IP_EXTENSION,

		/// <summary>
		/// Logon as administrator.
		/// </summary>
		ADMINISTRATOR
	}

	/// <summary>
	/// Posssible status values (result of any operation).
	/// </summary>
	public enum SPStatus
	{
		/// <summary>
		/// Execution in progress.
		/// </summary>
		DISCONNECTED,

		/// <summary>
		/// Something wrong with connection...
		/// </summary>
		ERROR_CONNECTION,

		/// <summary>
		/// Can't create socket.
		/// </summary>
		ERROR_CREATING_COMPONENTS,

		/// <summary>
		/// Can't create or start threads.
		/// </summary>
		ERROR_CREATING_THREADS,

		/// <summary>
		/// Invalid parameters (mean cast is not valid).
		/// </summary>
		ERROR_INVALID_PARAMETERS,

		/// <summary>
		/// Invalid operation.
		/// </summary>
		ERROR_INVALID_OPERATION,

		/// <summary>
		/// Can't logon to server (bad parameters in implementation constructor).
		/// </summary>
		ERROR_LOGON,

		/// <summary>
		/// Object is disposed before the call to it.
		/// </summary>
		ERROR_OBJECT_DISPOSED,

		/// <summary>
		/// Can't finalize threads.
		/// </summary>
		ERROR_SHUTTING_THREADS,

		/// <summary>
		/// Unhandled exception was raised.
		/// </summary>
		ERROR_UNHANDLED_EXCEPTION,

		/// <summary>
		/// All is OK.
		/// </summary>
		OK
	}

	/// <summary>
	/// Implements arguments for event.
	/// </summary>
	public class SPStatusEventArgs : EventArgs
	{
		/// <summary>
		/// Raises event with specified arguments if <see cref="ISP"/> status is not OK.
		/// </summary>
		/// <param name="status">Service Provider status.</param>
		/// <param name="info">Additional information.</param>
		public SPStatusEventArgs(SPStatus status, string info)
		{
			this.Status= status;
			this.Info = info;
			this.Time = DateTime.Now;
		}

		/// <summary>
		/// <see cref="ISP"/> status.
		/// </summary>
		public readonly SPStatus Status;
		
		/// <summary>
		/// Additional information for the event.
		/// </summary>
		public readonly string Info;

		/// <summary>
		/// The time event was raised.
		/// </summary>
		public readonly DateTime Time;
	}

	/// <summary>
	/// Delegate for <see cref="Diacom.ISP.SPStatusEvent"/> event.
	/// </summary>
	public delegate void SPStatusEventHandler(object source, SPStatusEventArgs e);

	/// <summary>
	/// Exposes methods for working with hardware part.
	/// </summary>
	public interface ISP : IDisposable
	{
		/// <summary>
		/// Event with information about <see cref="Diacom.ISP"/> status.
		/// </summary>
		/// <remarks>
		/// Raised when any thread have exception - i.e. when <see cref="Diacom.ISP.Send"/> or <see cref="Diacom.ISP.Receive"/> methods of <see cref="Diacom.ISP"/> interface are failed.
		/// </remarks>
		event SPStatusEventHandler SPStatusEvent;
		
		/// <summary>
		/// Indicates status of execution of any method.
		/// </summary>
		/// <remarks>
		/// Check this property every time you think there can be an error during execution.
		/// </remarks>
		SPStatus Status();

		/// <summary>
		/// Connects to the server with given parameters.
		/// </summary>
		/// <param name="serverIP">IP address of server.</param>
		/// <param name="serverPort">Port to connect.</param>
		/// <param name="account">Account.</param>
		/// <param name="password">Password.</param>
		/// <remarks>
		/// <para>
		/// Creates new TCP socket, connects to server with given IP address and port,
		/// tries to logon as administrator to "account" with given "password".
		/// </para>
		/// <para>
		/// Blocks the current thread until initialization is finished.
		/// Waits for connection indefinitely.
		/// </para>
		/// <para>
		/// To check execution results, use <see cref="Diacom.ISP.Status"/> property and <see cref="Diacom.SPStatus"/> enumeration.
		/// </para>
		/// </remarks>
		void Connect(string serverIP, int serverPort, string account, string password);

		/// <summary>
		/// Connects to the server with given parameters.
		/// </summary>
		/// <param name="serverIP">IP address of server.</param>
		/// <param name="serverPort">Port to connect.</param>
		/// <param name="account">Account.</param>
		/// <param name="password">Password.</param>
		/// <param name="timeout">Timeout interval.</param>
		/// <remarks>
		/// <para>
		/// Creates new TCP socket, connects to server with given IP address and port,
		/// tries to logon as administrator to "account" with given "password".
		/// </para>
		/// <para>
		/// Blocks the current thread until initialization is finished.
		/// Waits for connection "timeout" miliseconds.
		/// </para>
		/// <para>
		/// To check execution results, use <see cref="Diacom.ISP.Status"/> property and <see cref="Diacom.SPStatus"/> enumeration.
		/// </para>
		/// </remarks>
		void Connect(string serverIP, int serverPort, string account, string password, int timeout);

		/// <summary>
		/// Connects to the server with given parameters.
		/// </summary>
		/// <param name="serverIP">IP address of server.</param>
		/// <param name="serverPort">Port to connect.</param>
		/// <param name="logonType">Type of logon.</param>
		/// <param name="account">Account.</param>
		/// <param name="password">Password.</param>
		/// <remarks>
		/// <para>
		/// Creates new TCP socket, connects to server with given IP address and port,
		/// tries to logon as "logonType" to "account" with given "password".
		/// </para>
		/// <para>
		/// Blocks the current thread until initialization is finished.
		/// Waits for connection infinitely.
		/// </para>
		/// <para>
		/// To check execution results, use <see cref="Diacom.ISP.Status"/> property and <see cref="Diacom.SPStatus"/> enumeration.
		/// </para>
		/// </remarks>
		void Connect(string serverIP, int serverPort, SPLogonType logonType, string account, string password);

		/// <summary>
		/// Connects to the server with given parameters.
		/// </summary>
		/// <param name="serverIP">IP address of server.</param>
		/// <param name="serverPort">Port to connect.</param>
		/// <param name="logonType">Type of logon.</param>
		/// <param name="account">Account.</param>
		/// <param name="password">Password.</param>
		/// <param name="timeout">Timeout interval.</param>
		/// <remarks>
		/// <para>
		/// Creates new TCP socket, connects to server with given IP address and port,
		/// tries to logon as "logonType" to "account" with given "password".
		/// </para>
		/// <para>
		/// Blocks the current thread until initialization is finished.
		/// Waits for connection "timeout" miliseconds.
		/// </para>
		/// <para>
		/// To check execution results, use <see cref="Diacom.ISP.Status"/> property and <see cref="Diacom.SPStatus"/> enumeration.
		/// </para>
		/// </remarks>
		void Connect(string serverIP, int serverPort, SPLogonType logonType, string account, string password, int timeout);

		/// <summary>
		/// Disconnects from the server.
		/// </summary>
		/// <remarks>
		/// <para>
		/// Stops threads, closes socket, etc.
		/// </para>
		///</remarks>
		void Disconnect();

		/// <summary>
		/// Retrieves next available event (one of <see cref="Diacom.Ev"/> namespace classes).
		/// </summary>
		/// <returns>Next available event of <see cref="Diacom.Ev"/> type.</returns>
		/// <remarks>Waits while queue is empty.</remarks>
		Diacom.Ev.EvBase Receive();

		/// <summary>
		/// Sends command (one of <see cref="Diacom.Cmd"/> neamespace classes).
		/// </summary>
		/// <param name="cmd">Command to send of <see cref="Diacom.Cmd"/> type.</param>
		/// <remarks>Returns immediately.</remarks>
		void Send(Diacom.Cmd.CmdBase cmd);

		/// <summary>
		/// Provides an access to all the lines available.
		/// </summary>
		/// <returns>Pointer to array of <see cref="Diacom.SPLine"/> [] type.</returns>
		SPLine [] GetLines();

		/// <summary>
		/// Provides an access to specified line.
		/// </summary>
		/// <param name="lineID">Line ID.</param>
		/// <returns><see cref="Diacom.SPLine"/> instance if line exists, null otherwise.</returns>
		SPLine GetLine(object lineID);
	} // end of IAltiGenSp interface.


	/////////////////////////////////////////////////////////////
	//                                                         //
	// Exposes commands to send by IAltiGenSP.Send(...) method //
	//                                                         //
	/////////////////////////////////////////////////////////////
	namespace Cmd
	{
		/// <summary>
		/// Commands enumeration.
		/// </summary>
		public enum CommandID
		{
			/// <summary>
			/// Answer the call.
			/// </summary>
			ANSWER_CALL,

			/// <summary>
			/// Connect two lines each other.
			/// </summary>
			CONNECT,

			/// <summary>
			/// Dial specified number.
			/// </summary>
			DIAL,

			/// <summary>
			/// Disconnect appropriate line.
			/// </summary>
			DISCONNECT,

			/// <summary>
			/// Play DTMF code.
			/// </summary>
			PLAY_DTMF,

			/// <summary>
			/// Play file.
			/// </summary>
			PLAY_FILE,

			/// <summary>
			/// Record file.
			/// </summary>
			RECORD_RILE,

			/// <summary>
			/// Reject the call.
			/// </summary>
			REJECT_CALL,

			/// <summary>
			/// Reset line (terminate execution of current command, if possible).
			/// </summary>
			RESET,

			/// <summary>
			/// Ring specified extension.
			/// </summary>
			RING_EXTENSION,

			/// <summary>
			/// Snatch line.
			/// </summary>
			SNATCH_LINE,

			/// <summary>
			/// Switch music mode.
			/// </summary>
			SWITCH_MUSIC,

			/// <summary>
			/// Transfer call.
			/// </summary>
			TRANSFER
		}
		/// <summary>
		/// Base class for all commands.
		/// </summary>
		public class CmdBase
		{
			/// <summary>
			/// Command ID (see <see cref="Diacom.Cmd.CommandID">CommandID</see> enumeration).
			/// </summary>
			public CommandID ID { get {return this.cmdID; } }

			/// <summary>
			/// Line the command comes from.
			/// </summary>
			public object Sender { get { return this.cmdSender; } }

			/// <summary>
			/// Line the command should be executed on.
			/// </summary>
			public object Line { get { return this.targetLine; } }

			/// <summary>
			/// Storage for command ID.
			/// </summary>
			private readonly CommandID cmdID;

			/// <summary>
			/// Storage for command sender ID.
			/// </summary>
			private readonly object cmdSender;

			/// <summary>
			/// Storage for the line on which the command is executed.
			/// </summary>
			private readonly object targetLine;

			/// <summary>
			/// Implements a constructor for the command base class.
			/// </summary>
			/// <param name="commandID">Command ID (see <see cref="Diacom.Cmd.CommandID">CommandID</see> enumeration).</param>
			/// <param name="sender">Line the command comes from.</param>
			/// <param name="target">Target line - the line on which command should be executed.</param>
			public CmdBase(CommandID commandID, object sender, object target)
			{
				cmdID = commandID;
				cmdSender = sender;
				targetLine = target;
			}
		}

		/// <summary>
		/// Implements Answer Call command.
		/// </summary>
		public class AnswerCall : CmdBase
		{
			/// <summary>
			/// Answers call from specified line.
			/// </summary>
			/// <param name="sender">Line the command comes from.</param>
			/// <param name="line">Line to answer the call from.</param>
			public AnswerCall(object sender, object line) 
				: base(CommandID.ANSWER_CALL, sender, line) 
			{
			}
		}

		/// <summary>
		/// Implements Connect command.
		/// </summary>
		public class Connect : CmdBase
		{
			/// <summary>
			/// Connects two lines each other.
			/// </summary>
			/// <param name="sender">Line the command comes from.</param>
			/// <param name="line1">First line to connect.</param>
			/// <param name="line2">Second line to connect.</param>
			public Connect(object sender, object line1, object line2)
				 : base(CommandID.CONNECT, sender, line1) 
			{
				LineOne = line1;
				LineTwo = line2;
			}

			/// <summary>
			/// First line to connect.
			/// </summary>
			public readonly object LineOne;

			/// <summary>
			/// Second line to connect.
			/// </summary>
			public readonly object LineTwo;
		}

		/// <summary>
		/// Implements Dial command.
		/// </summary>
		public class Dial : CmdBase
		{
			/// <summary>
			/// Dials specified line number.
			/// </summary>
			/// <param name="sender">Line the command comes from.</param>
			/// <param name="line">Line to dial on.</param>
			/// <param name="destination">Destination number (to dial).</param>
			/// <param name="source">Source number (who dials).</param>
			/// <param name="account">Account.</param>
			/// <param name="tone">Tone detect: 0 - disabled, 1 - enabled.</param>
			public Dial(object sender, object line, string destination, string source, string account, int tone)
				 : base(CommandID.DIAL, sender, line) 
			{
				this.Destination = destination;
				this.Source = source;
				this.Account = account;
				this.Tone = tone;
			}

			/// <summary>
			/// Destination number (to dial).
			/// </summary>
			public readonly string Destination;

			/// <summary>
			/// Source number (who dials).
			/// </summary>
			public readonly string Source;

			/// <summary>
			/// Account.
			/// </summary>
			public readonly string Account;

			/// <summary>
			/// Tone detect: 0 - disabled, 1 - enabled.
			/// </summary>
			public readonly int Tone;
		}

		/// <summary>
		/// Implements Disconnect command.
		/// </summary>
		public class Disconnect : CmdBase
		{
			/// <summary>
			/// Disconnects specified line.
			/// </summary>
			/// <param name="sender">Line the command comes from.</param>
			/// <param name="line">Line to disconnect.</param>
			public Disconnect(object sender, object line)
				 : base(CommandID.DISCONNECT, sender, line) 
			{
			}
		}

		/// <summary>
		/// Implements Play DTMF command.
		/// </summary>
		public class PlayDTMF : CmdBase
		{
			/// <summary>
			/// Plays specified DTMF code on appropriate line.
			/// </summary>
			/// <param name="sender">Line the command comes from.</param>
			/// <param name="line">Line to play DTMF.</param>
			/// <param name="DTMFCode">DTMF code.</param>
			public PlayDTMF(object sender, object line, string DTMFCode)
				: base(CommandID.PLAY_DTMF, sender, line) 			
			{
				this.DTMFCode = DTMFCode;
			}
			/// <summary>
			/// DTMF code.
			/// </summary>
			public readonly string DTMFCode;
		}

		/// <summary>
		/// Implements Play File command.
		/// </summary>
		public class PlayFile : CmdBase
		{
			/// <summary>
			/// Plays specified file on appropriate line.
			/// </summary>
			/// <param name="sender">Line the command comes from.</param>
			/// <param name="line">Line to play file.</param>
			/// <param name="filePath">Path to the file to play.</param>
			/// <param name="cutOffString">CutOff string.</param>
			public PlayFile(object sender, object line, string filePath, string cutOffString)
				 : base(CommandID.PLAY_FILE, sender, line) 
			{
				this.FilePath = filePath;
				this.CutOffString = cutOffString;
			}

			/// <summary>
			/// Path to the file to play.
			/// </summary>
			public readonly string FilePath;

			/// <summary>
			/// CutOff string.
			/// </summary>
			public readonly string CutOffString;
		}

		/// <summary>
		/// Implements Record File command.
		/// </summary>
		public class RecordFile : CmdBase
		{
			/// <summary>
			/// Records file with specified path on appropriate line.
			/// </summary>
			/// <param name="sender">Line the command comes from.</param>
			/// <param name="line">Line to record the file.</param>
			/// <param name="filePath">Path to record the file to.</param>
			/// <param name="cutOffString">CutOff string.</param>
			/// <param name="appendMode">Append mode: 0 - create new, 1 - append to existing.</param>
			public RecordFile(object sender, object line, string filePath, string cutOffString, int appendMode)
				: base(CommandID.RECORD_RILE, sender, line) 
			{
				this.FilePath = filePath;
				this.CutOffString = cutOffString;
				this.AppendMode = appendMode;
			}

			/// <summary>
			/// Path to record the file to.
			/// </summary>
			public readonly string FilePath;

			/// <summary>
			/// CutOff string.
			/// </summary>
			public readonly string CutOffString;

			/// <summary>
			/// Append mode: 0 - create new, 1 - append to existing.
			/// </summary>
			public readonly int AppendMode;
		}

		/// <summary>
		/// Rejects call.
		/// </summary>
		public class RejectCall : CmdBase
		{
			/// <summary>
			/// Rejects the call from specified line.
			/// </summary>
			/// <param name="sender">Line the command comes from.</param>
			/// <param name="line">Line to reject the call from.</param>
			public RejectCall(object sender, object line)
				: base(CommandID.REJECT_CALL, sender, line)
			{
			}
		}

		/// <summary>
		/// Resets line (stops current command).
		/// </summary>
		public class Reset : CmdBase
		{
			/// <summary>
			/// Resets specified line.
			/// </summary>
			/// <param name="sender">Line the command comes from.</param>
			/// <param name="line">Line to reset.</param>
			public Reset(object sender, object line)
				: base(CommandID.RESET, sender, line) 
			{
			}
		}

		/// <summary>
		/// Implements Ring Extension command.
		/// </summary>
		public class RingExtension : CmdBase
		{
			/// <summary>
			/// Rings specified extension.
			/// </summary>
			/// <param name="sender">Line the command comes from.</param>
			/// <param name="line">Line to Ring extension on.</param>
			/// <param name="extension">Extension to ring to.</param>
			/// <param name="ringType">Type of ring: 0 - default, 1 - normal, 2 - intercom.</param>
			public RingExtension(object sender, object line, string extension, int ringType)
				: base(CommandID.RING_EXTENSION, sender, line) 
			{
				this.Extension = extension;
				this.RingType = ringType;
			}

			/// <summary>
			/// Extension to ring to.
			/// </summary>
			public readonly string Extension;

			/// <summary>
			/// Type of ring: 0 - default, 1 - normal, 2 - intercom.
			/// </summary>
			public readonly int RingType;
		}

		/// <summary>
		/// Implements Snatch Line command.
		/// </summary>
		public class SnatchLine : CmdBase
		{
			/// <summary>
			/// Snatches specified line (and also line it connects to, if exist), if line is trunk.
			/// Snatches specified line and line it connects to, if line is extension.
			/// </summary>
			/// <param name="sender">Line the command comes from.</param>
			/// <param name="line">Line to snatch.</param>
			public SnatchLine(object sender, object line)
				: base(CommandID.SNATCH_LINE, sender, line) 
			{
			}
		}

		/// <summary>
		/// Implements Switch Music command.
		/// </summary>
		public class SwitchMusic : CmdBase
		{
			/// <summary>
			/// Switches play music or not.
			/// </summary>
			/// <param name="sender">Line the command comes from.</param>
			/// <param name="line">Line to switch music.</param>
			/// <param name="musicMode">Music mode: 0 - music on, 1 - music off.</param>
			public SwitchMusic(object sender, object line, int musicMode)
				: base(CommandID.SWITCH_MUSIC, sender, line) 
			{
				this.MusicMode = musicMode;
			}

			/// <summary>
			/// Music mode: 0 - music on, 1 - music off.
			/// </summary>
			public readonly int MusicMode;
		}

		/// <summary>
		/// Possible trasfer types.
		/// </summary>
		public enum TransferCallType
		{
			/// <summary>
			/// Transfer to autoattedant.
			/// </summary>
			AUTOATEDDANT,

			/// <summary>
			/// Transfer to extension.
			/// </summary>
			EXTENSION,

			/// <summary>
			/// Transfer to extension (to voice message).
			/// </summary>
			EXTENSION_VOICE_MESSAGE,

			/// <summary>
			/// Transfer to operator.
			/// </summary>
			OPERATOR,

			/// <summary>
			/// Transfer to trunk.
			/// </summary>
			TRUNK
		}

		/// <summary>
		/// Implements Transfer Call command.
		/// </summary>
		public class TransferCall : CmdBase
		{
			/// <summary>
			/// Transfers call.
			/// </summary>
			/// <param name="sender">Line the command comes from.</param>
			/// <param name="line">Line which is used to transfer call from.</param>
			/// <param name="targetLine">Line which is used to transfer call to.</param>
			/// <param name="destination">Destination number.</param>
			/// <param name="type">Type of transfer (see <see cref="Diacom.Cmd.TransferCallType"/> enumeration).</param>
			public TransferCall(object sender, object line, object targetLine, string destination, TransferCallType type)
				: base(CommandID.TRANSFER, sender, line) 
			{
				this.Target = targetLine;
				this.Destination = destination;
				this.Type = type;
			}

			/// <summary>
			/// Destination number.
			/// </summary>
			public readonly object Target;

			/// <summary>
			/// Destination number.
			/// </summary>
			public readonly string Destination;

			/// <summary>
			/// Type of transfer (see <see cref="Diacom.Cmd.TransferCallType"/> enumeration).
			/// </summary>
			public readonly TransferCallType Type;
		}
	} // end of Cmd namespace.


	//////////////////////////////////////////////////////////
	//                                                      //
	// Exposes returned by IAltiGen.Receive() method events //
	//                                                      //
	//////////////////////////////////////////////////////////
	namespace Ev
	{
		/// <summary>
		/// Events enumeration.
		/// </summary>
		public enum EventID
		{
			/// <summary>
			/// Status of the command execution (see CmdStatus enumeration).
			/// </summary>
			COMMAND_STATUS,

			/// <summary>
			/// Connection established.
			/// </summary>
			CONNECT,

			/// <summary>
			/// Digit was pressed.
			/// </summary>
			DIGIT,

			/// <summary>
			/// End of connection.
			/// </summary>
			DISCONNECT,

			/// <summary>
			/// Changing of line state.
			/// </summary>
			LINE_STATE_CHANGED,

			/// <summary>
			/// Have a call.
			/// </summary>
			RING,

			/// <summary>
			/// Have a response on dialing the number.
			/// </summary>
			RING_BACK,

			/// <summary>
			/// Have a tone.
			/// </summary>
			TONE
		}
		/// <summary>
		/// Base class for all events.
		/// </summary>
		public class EvBase
		{
			/// <summary>
			/// Initializes new instance of class with specified parameters.
			/// </summary>
			/// <param name="eventLine">Object that uniquely identifies the line.</param>
			/// <param name="eventType">Type of the event (see <see cref="Diacom.Ev.EventID"/> enumeration).</param>
			public EvBase(object eventLine, Ev.EventID eventType)
			{
				evLine = eventLine;
				evID = eventType;
			}
			/// <summary>
			/// Event ID (see <see cref="Diacom.Ev.EventID">EventID</see> enumeration).
			/// </summary>
			public EventID ID { get { return this.evID; } }

			/// <summary>
			/// Line number.
			/// </summary>
			public object Line { get { return this.evLine; } }

			/// <summary>
			/// Storage for event ID.
			/// </summary>
			protected readonly EventID evID;

			/// <summary>
			/// Storage for line where event was raised.
			/// </summary>
			protected readonly object evLine;
		}

		/// <summary>
		/// Implements Ring event (J).
		/// </summary>
		public class Ring : EvBase
		{
			/// <summary>
			/// Initializes new instance of class with specified parameters.
			/// </summary>
			/// <param name="line">Object that identifies the line on which ring came.</param>
			/// <param name="lineInfo">Parameter of type <see cref="Diacom.SPLine"/> that contains all info about the call.</param>
			public Ring(object line, SPLine lineInfo) : base(line, Ev.EventID.RING)
			{
				ringInfo = (SPLine)lineInfo.Clone();
			}

			/// <summary>
			/// Information about the incoming ring.
			/// </summary>
			public readonly SPLine ringInfo;
		}

		/// <summary>
		/// Implements Ring Back event (V).
		/// </summary>
		public class RingBack : EvBase
		{
			/// <summary>
			/// Initializes new instance of class with specified parameters.
			/// </summary>
			/// <param name="line">Object that identifies the line on which ringback came.</param>
			/// <param name="lineInfo">Parameter of type <see cref="Diacom.SPLine"/> that contains all info about the call.</param>
			public RingBack(object line, SPLine lineInfo) : base(line, Ev.EventID.RING_BACK)
			{
				ringInfo = (SPLine) lineInfo.Clone();
			}

			/// <summary>
			/// Information about the incoming ring.
			/// </summary>
			public readonly SPLine ringInfo;
		}

		/// <summary>
		/// Implements Connect event (I).
		/// </summary>
		public class Connect : EvBase
		{
			/// <summary>
			/// Initializes new instance of class with specified line number.
			/// </summary>
			/// <param name="line">Connected line number.</param>
			public Connect(object line) : base(line, Ev.EventID.CONNECT)
			{
			}
		}

		/// <summary>
		/// Implements Digit event (0-F, P, R).
		/// </summary>
		public class Digit : EvBase
		{
			/// <summary>
			/// Initializes new instance of class with specified line number and digit code.
			/// </summary>
			/// <param name="line">Number of line on which Digit event was detected.</param>
			/// <param name="digit">Digit code.</param>
			public Digit(object line, char digit) : base(line, Ev.EventID.DIGIT)
			{
				this.Code = digit;
			}

			/// <summary>
			/// Digit code.
			/// </summary>
			public readonly char Code;
		}

		/// <summary>
		/// Implements Disconnect event (X).
		/// </summary>
		public class Disconnect : EvBase
		{
			/// <summary>
			/// Initializes new instance of class with specified line number.
			/// </summary>
			/// <param name="line">Disconnected line number.</param>
			public Disconnect(object line) : base(line, Ev.EventID.DISCONNECT)
			{
			}
		}

		/// <summary>
		/// Type of tone.
		/// </summary>
		public enum ToneType
		{
			/// <summary>
			/// Unknown tone type.
			/// </summary>
			UNKNOWN,

			/// <summary>
			/// Busy tone type.
			/// </summary>
			BUSY,

			/// <summary>
			/// Ring back tone type.
			/// </summary>
			RINGBACK,

			/// <summary>
			/// CED tone type.
			/// </summary>
			CED,

			/// <summary>
			/// CNG tone type.
			/// </summary>
			CNG,

			/// <summary>
			/// User tone type.
			/// </summary>
			USER,

			/// <summary>
			/// Voice tone type.
			/// </summary>
			VOICE
		}

		/// <summary>
		/// Implements Tone event.
		/// </summary>
		public class Tone : EvBase
		{
			/// <summary>
			/// Initializes new instance of class with specified line number.
			/// </summary>
			/// <param name="line">Line number.</param>
			/// <param name="tone">Tone type: 0 - unknown, 1 - busy, 2 - ring back, 3 - CED, 4 - CNG, 5 - user, 6 - voice (see <see cref="Diacom.Ev.ToneType">ToneType</see> enumeration).</param>
			public Tone(object line, ToneType tone) : base(line, Ev.EventID.TONE)
			{
				this.Type = tone;
			}

			/// <summary>
			/// Tone type: 0 - unknown, 1 - busy, 2 - ring back, 3 - CED, 4 - CNG, 5 - user, 6 - voice (see <see cref="Diacom.Ev.ToneType">ToneType</see> enumeration).
			/// </summary>
			public readonly ToneType Type;
		}

		/// <summary>
		/// Implements LineStateChange event.
		/// </summary>
		public class LineStateChanged : EvBase
		{
			/// <summary>
			/// Initializes new instance of class with specified line number.
			/// </summary>
			/// <param name="line">Line number.</param>
			/// <param name="state">State of the line.</param>
			public LineStateChanged(object line, SPLineState state) : base(line, Ev.EventID.LINE_STATE_CHANGED)
			{
				this.State = state;
			}

			/// <summary>
			/// State of the line.
			/// </summary>
			public readonly SPLineState State;
		}

		/// <summary>
		/// Possible command execution statuses.
		/// </summary>
		public enum CmdStatus
		{
			/// <summary>
			/// Command successfully executed.
			/// </summary>
			OK,

			/// <summary>
			/// Command execution was not successfull.
			/// </summary>
			ERROR
		}

		/// <summary>
		/// Implements Command Status event (S).
		/// </summary>
		public class CommandStatus : EvBase
		{
			/// <summary>
			/// Initializes new instance of class with specified parameters.
			/// </summary>
			/// <param name="addressee">Line which issued the command and where response goes to.</param>
			/// <param name="line">Line on which the command was executed.</param>
			/// <param name="commandID">Command ID (see <see cref="Diacom.Cmd.CommandID">CommandID</see> enumeration).</param>
			/// <param name="status">Command execution status (see <see cref="Diacom.Ev.CmdStatus">CmdStatus</see> enumeration).</param>
			public CommandStatus(object addressee, object line, Cmd.CommandID commandID, CmdStatus status)
				: base(line, Ev.EventID.COMMAND_STATUS)
			{
				this.Addressee = addressee;
				this.CommandID = commandID;
				this.Status = status;
			}

			/// <summary>
			/// Command ID (see <see cref="Diacom.Cmd.CommandID">CommandID</see> enumeration).
			/// </summary>
			public readonly object Addressee;

			/// <summary>
			/// Command ID (see <see cref="Diacom.Cmd.CommandID">CommandID</see> enumeration).
			/// </summary>
			public readonly Cmd.CommandID CommandID;

			/// <summary>
			/// Command execution status (see <see cref="Diacom.Ev.CmdStatus">CmdStatus</see> enumeration).
			/// </summary>
			public readonly CmdStatus Status;
		}
	} // end of Ev namespace.

	/// <summary>
	/// Implements output to a standard application error stream.
	/// </summary>
	public class TraceOut
	{
		private static System.Threading.Thread TraceWriterHandle = null;
		private static WaitingQueue MessagesQueue = null;

		/// <summary>
		/// Gets messages from queue and puts them to a trace output.
		/// </summary>
		private static void TraceWriter()
		{
			while(true)
			{
				try
				{
					System.Diagnostics.Trace.WriteLine(Convert.ToString(MessagesQueue.Dequeue()));
				}
				catch(System.Exception x)
				{
					System.Diagnostics.Trace.WriteLine("[TraceWriter]" + System.DateTime.Now.ToString("HH:mm:ss:fff") + Environment.NewLine + "The message to trace output couldn't be converted to a string... reason:"+Environment.NewLine+x.ToString());
				}
			}
		}

		/// <summary>
		/// Creates messages queue and starts messages-processing thread.
		/// </summary>
		static TraceOut()
		{
			MessagesQueue = new WaitingQueue();
			TraceWriterHandle = new System.Threading.Thread(new System.Threading.ThreadStart(TraceWriter));
			TraceWriterHandle.IsBackground = true;
			TraceWriterHandle.Start();
		}

		/// <summary>
		/// Puts given string to trace output, adding the time it happends.
		/// </summary>
		/// <param name="aMessage">String to put.</param>
		public static void Put(string aMessage)
		{
			MessagesQueue.Enqueue( "[" + System.Threading.Thread.CurrentThread.Name + "][" + System.AppDomain.GetCurrentThreadId().ToString("X") + "][" + System.DateTime.Now.ToString("HH:mm:ss:fff") + "]: " + aMessage );
		}

		/// <summary>
		/// Puts given exception to trace output, adding the time it happends.
		/// </summary>
		/// <param name="ex">Exception to handle.</param>
		public static void Put(System.Exception ex)
		{
			System.Text.StringBuilder msg = new System.Text.StringBuilder();
			msg.Append( "Exception: " + Environment.NewLine + ex.ToString() + Environment.NewLine );
			msg.Append( "Source: " + Environment.NewLine + ex.Source + Environment.NewLine );
			if (ex.InnerException != null)
			{
				msg.Append( "Inner exception: " + Environment.NewLine + ex.InnerException.ToString() + Environment.NewLine );
				msg.Append( "Source: " + Environment.NewLine + ex.InnerException.Source );
			}
			Put(msg.ToString());
		}
	}

	/// <summary>
	/// Implements thread-safe queue.
	/// </summary>
	public class  WaitingQueue
	{
		/// <summary>
		/// Event that signales about queue is not empty - data already stored in queue.
		/// </summary>
		private System.Threading.ManualResetEvent queueHasData = null;

		/// <summary>
		/// The queue itself.
		/// </summary>
		private System.Collections.Queue dataQueue = null;

		/// <summary>
		/// Initialized fiels - nothing else.
		/// </summary>
		public WaitingQueue()
		{
			queueHasData = new System.Threading.ManualResetEvent(false);
			dataQueue = new System.Collections.Queue();
		}

		/// <summary>
		/// Puts element into queue.
		/// </summary>
		/// <param name="data">Element to put in.</param>
		public void Enqueue(object data)
		{
			lock(this)
			{
				dataQueue.Enqueue(data);
				queueHasData.Set();
			}
		}

		/// <summary>
		/// Extracts element from queue.
		/// </summary>
		/// <returns>Extracted element.</returns>
		public object Dequeue()
		{
			object _return = null;
			do
			{
				queueHasData.WaitOne();
				lock(this)
				{
					if(dataQueue.Count != 0)
					{
						_return = dataQueue.Dequeue();
					}
					else
					{
						queueHasData.Reset();
					}
				}
			}while(_return == null);
			return _return;
		}

		/// <summary>
		/// Clears queue.
		/// </summary>
		public void Clear()
		{
			lock(this)
			{
				dataQueue.Clear();
				queueHasData.Reset();
			}
		}

		/// <summary>
		/// Gets quantity of elements in queue.
		/// </summary>
		public int Count
		{
			get
			{
				int _return = 0;
				lock(this)
				{
					_return = dataQueue.Count;
				}
				return _return;
			}
		}
	}
}
