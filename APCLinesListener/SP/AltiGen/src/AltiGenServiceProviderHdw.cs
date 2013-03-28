using System;
using System.IO;
using System.Threading;
using System.Net.Sockets;
using System.Collections;

namespace Diacom.AltiGen
{
	/// <summary>
	/// Implements <see cref="Diacom.ISP"/> interface for AltiGen servers.
	/// </summary>
	public class ASPHdw : ISP, IDisposable
	{
		#region -------------------------------------------------------------------------- Private properties ---
		/// <summary>
		/// TCP client for connection to AltiGen server.
		/// </summary>
		private TcpClient tcpc = null;

		/// <summary>
		/// Stream to access the TCP client.
		/// </summary>
		private Stream st = null;

		/// <summary>
		/// Binary reader for the stream.
		/// </summary>
		private BinaryWriter bw = null;

		/// <summary>
		/// Binary writer for the stream.
		/// </summary>
		private BinaryReader br = null;

		/// <summary>
		/// Queue for events (incoming).
		/// </summary>
		private WaitingQueue eventsQueue = null;

		/// <summary>
		/// Thread handle for incoming messages.
		/// </summary>
		private Thread inThreadHandle = null;

		/// <summary>
		/// Flag for thread for incoming messages to end his job.
		/// </summary>
		private bool inThreadLivingStatus = false;

		/// <summary>
		/// Queue for commands (outcoming).
		/// </summary>
		private WaitingQueue commandsQueue = null;

		/// <summary>
		/// Thread handle for outcoming messages.
		/// </summary>
		private Thread outThreadHandle = null;

		/// <summary>
		/// Flag for thread for outcoming messages to end his job.
		/// </summary>
		private bool outThreadLivingStatus = false;

		/// <summary>
		/// Event that signales what connection with AltiGen server is established.
		/// </summary>
		private ManualResetEvent connectionEstablished = null;

		/// <summary>
		/// SP account ID (control line PAD).
		/// </summary>
		private int locationID = 0;

		/// <summary>
		/// Hashtable of all lines in system.
		/// </summary>
		private Hashtable lines = null;

		/// <summary>
		/// Hashtable for storing information on commands DIAL somebody.
		/// </summary>
		private Hashtable outcallNumbersDialingCommands = null;

		/// <summary>
		/// Status of SP.
		/// </summary>
		private SPStatus spStatus = SPStatus.DISCONNECTED;

		/// <summary>
		/// Disposing flag.
		/// </summary>
		private bool isDisposed = false;

		/// <summary>
		/// Checks if connection with SP was interrupted and restarts the connection in this case.
		/// </summary>
		private System.Threading.Timer ConnectionChecker = null;

		/// <summary>
		/// The flag representing connection status.
		/// </summary>
		private bool ConnectionIsAlive = true;
		#endregion

		#region ------------------------------------------------------ Private methods for ISP implementation ---
		/// <summary>
		/// Raises event with information about IAltiGenSP status.
		/// </summary>
		/// <remarks>
		/// <para>Must be raised only when any thread have exception - i.e. when
		/// <see cref="Diacom.AltiGen.ASPHdw.Send"/> or <see cref="Diacom.AltiGen.ASPHdw.Receive"/> methods
		/// of <see cref="Diacom.AltiGen.ASPHdw"/> interface are failed.</para>
		/// <seealso cref="Diacom.ISP.SPStatusEvent"/>
		/// </remarks>
		private void RaiseEvent(SPStatus _status, string _info)
		{
			SPStatusEventArgs e = new SPStatusEventArgs(_status, _info);
			if(this.SPStatusEvent != null) this.SPStatusEvent(this, e);
		}

		/// <summary>
		/// Initializes main SP components.
		/// </summary>
		/// <remarks>
		/// <para>Sets SP status to <see cref="Diacom.SPStatus.DISCONNECTED"/> as a current status,
		/// creates <see cref="Diacom.AltiGen.ASPHdw.commandsQueue">commands</see> and
		/// <see cref="Diacom.AltiGen.ASPHdw.eventsQueue">events</see> queues and 
		/// <see cref="Diacom.AltiGen.ASPHdw.inThread">thr</see>
		/// <see cref="Diacom.AltiGen.ASPHdw.outThread">eads</see>.</para>
		/// <seealso cref="Diacom.ISP"/>
		/// </remarks>
		public ASPHdw()
		{
			// Creating Status object;
			spStatus = SPStatus.DISCONNECTED;
			// Creating non-signaled ManualResetEvent.
			connectionEstablished = new ManualResetEvent(false);
			// Creating new TCPClient to communicate using AltiLink Plus.
			tcpc = new TcpClient();
			// Creating 2 queues for commands, events & responses.
			commandsQueue = new WaitingQueue();
			eventsQueue = new WaitingQueue();
			// Hashtable outcallNumbersDialingCommands is not created yet.
			outcallNumbersDialingCommands = new Hashtable();
			// Thread to get messages from SP.
			inThreadHandle = new Thread(new ThreadStart(inThread));
			inThreadHandle.IsBackground = inThreadLivingStatus = true;
			// Thread to send messages to SP.
			outThreadHandle = new Thread(new ThreadStart(outThread));
			outThreadHandle.IsBackground = outThreadLivingStatus = true;
		}

		/// <summary>
		/// Connects to server with given parameters.
		/// </summary>
		/// <param name="serverIP">IP address of server.</param>
		/// <param name="serverPort">Port to connect.</param>
		/// <param name="logonType">Type of logon.</param>
		/// <param name="account">Account.</param>
		/// <param name="password">Password.</param>
		/// <param name="timeout">Timeout interval.</param>
		/// <remarks>
		/// <para>Creates new TCP socket, connects to server with given IP address and port,
		/// tries to logon as "logonType" to "account" with given "password".</para>
		/// <para>Blocks the current thread until initialization is finished.
		/// Waits for connection "timeout" miliseconds.</para>
		/// <para>Use <see cref="Diacom.AltiGen.ASPHdw.Status"/> property and <see cref="Diacom.SPStatus"/> 
		/// enumeration to check execution results.</para>
		/// <seealso cref="Diacom.AltiGen.ASPHdw.Status"/>
		/// <seealso cref="Diacom.SPStatus"/>
		/// </remarks>
		private void Initialize(string serverIP, int serverPort, SPLogonType logonType, string account, string password, int timeout)
		{
			this.ConnectionChecker = new System.Threading.Timer(new System.Threading.TimerCallback(this.CheckConnection), null, 30000, 15000);
			int _timeout = timeout;
			AltiLinkPlus.ALPCommand ac = null;
			try
			{
				// Connect to the AltiWare server.
				tcpc.Connect(serverIP, serverPort);
				this.ConnectionIsAlive = true;
				// Assign network stream.
				st = tcpc.GetStream();
				// To Binaryreader.
				bw = new BinaryWriter(st);
				// To BinaryWriter.
				br = new BinaryReader(st);
				// Command to Register Application.
				ac = new AltiLinkPlus.ALPCommand(0, Convert.ToInt32(ALPCmdID.REGISTER_APPID));
				// AltiGen SDK application.
				ac[0] = new AltiLinkPlus.ALPParameter("GATORS11");
				// Sending command.
				commandsQueue.Enqueue(ac);
				// Command to Logon to SP.
				ac = new AltiLinkPlus.ALPCommand(0, Convert.ToInt32(ALPCmdID.LOGON));
				// Type of logon - ADMIN.
				ac[0] = new AltiLinkPlus.ALPParameter((int)(logonType));
				// Extension number.
				ac[1] = new AltiLinkPlus.ALPParameter(account);
				// Extension password.
				ac[2] = new AltiLinkPlus.ALPParameter(password);
				ac[3] = new AltiLinkPlus.ALPParameter(0);
				ac[4] = new AltiLinkPlus.ALPParameter(0);
				ac[5] = new AltiLinkPlus.ALPParameter(0);
				ac[6] = new AltiLinkPlus.ALPParameter(0);
				// Sending command.
				commandsQueue.Enqueue(ac);



				ac = new Diacom.AltiGen.AltiLinkPlus.ALPCommand(0, Convert.ToInt32(ALPCmdID.GET_VERSION));
				commandsQueue.Enqueue(ac);

				// Command to Get Lines Information.
				ac = new Diacom.AltiGen.AltiLinkPlus.ALPCommand(0, Convert.ToInt32(ALPCmdID.GET_LINEINFO));
				// Sending command.
				commandsQueue.Enqueue(ac);
				// Geeting names.
				this.inThreadHandle.Name = "AltiGen Service Provider: InThread";
				this.outThreadHandle.Name = "AltiGen Service Provider: OutThread";
				// Starting threads.
				inThreadHandle.Start();
				outThreadHandle.Start();
				// Waiting while all responces will be received.
				if(_timeout == 0) _timeout = Timeout.Infinite;
				// Specified timeout elapses.
				if(connectionEstablished.WaitOne(_timeout, false) == false)
				{
					spStatus = SPStatus.ERROR_LOGON;
					return;
				}
				// Have a bad response on LOGON or GET_LINES command.
				if((lines == null)||(locationID == 0))
				{
					spStatus = SPStatus.ERROR_LOGON;
					return;
				}
				spStatus = SPStatus.OK;
			}
				// Something is wrong - setting appropriate status.
				// From new operator.
			catch(OutOfMemoryException _e)
			{
				TraceOut.Put(_e);
				RaiseEvent(SPStatus.ERROR_CREATING_COMPONENTS, _e.ToString());
			}
				// From TcpClient, etc.
			catch(ArgumentNullException _e)
			{
				TraceOut.Put(_e);
				RaiseEvent(SPStatus.ERROR_INVALID_PARAMETERS, _e.ToString());
			}
				// From ...
			catch(ArgumentOutOfRangeException _e)
			{
				TraceOut.Put(_e);
				RaiseEvent(SPStatus.ERROR_INVALID_PARAMETERS, _e.ToString());
			}
				// From Convert class.
			catch(InvalidCastException _e)
			{
				TraceOut.Put(_e);
				RaiseEvent(SPStatus.ERROR_INVALID_PARAMETERS, _e.ToString());
			}
				// From TcpClient.
			catch(SocketException _e)
			{
				TraceOut.Put(_e);
				RaiseEvent(SPStatus.ERROR_CONNECTION, _e.ToString());
			}
				// From TcpClient.
			catch(ObjectDisposedException _e)
			{
				TraceOut.Put(_e);
				RaiseEvent(SPStatus.ERROR_CONNECTION, _e.ToString());
			}
				// From TcpClient.
			catch(InvalidOperationException _e)
			{
				TraceOut.Put(_e);
				RaiseEvent(SPStatus.ERROR_CONNECTION, _e.ToString());
			}
				// Any exception.
			catch
			{
				TraceOut.Put("Unknown Exception");
				RaiseEvent(SPStatus.ERROR_UNHANDLED_EXCEPTION, String.Empty);
			}
		}

		/// <summary>
		/// Destructor.
		/// </summary>
		~ASPHdw()
		{
			Dispose(false);
		}

		/// <summary>
		/// Checks the connection status.
		/// </summary>
		/// <param name="data">Data for check (not used).</param>
		private void CheckConnection(object data)
		{
			if(!this.ConnectionIsAlive)
			{
				RaiseEvent(SPStatus.ERROR_CONNECTION, "");
			}
			this.ConnectionIsAlive = false;
			AltiLinkPlus.ALPCommand ac = new AltiLinkPlus.ALPCommand(this.locationID, Convert.ToInt32(ALPCmdID.PING));
			ac[0] = new AltiLinkPlus.ALPParameter(1);
			commandsQueue.Enqueue(ac);
			TraceOut.Put("[Timer] Checking connection("+this.ConnectionIsAlive.ToString()+")...");
		}
		#endregion

		#region ------------------------------------------------------------------ Implementing ISP interface ---
		/// <summary>
		/// Implements <see cref="System.IDisposable.Dispose"/> method of 
		/// <see cref="System.IDisposable"/> interface.
		/// </summary>
		/// <remarks>
		/// <para>Stops threads, closes socket, etc.</para>
		/// <seealso cref="System.IDisposable"/>
		///</remarks>
		private void Dispose(bool disposing)
		{
			if(isDisposed) return;
			lock(this)
			{
				isDisposed = true;
				if(disposing)
				{
					// Command to Logoff from SP.
					AltiLinkPlus.ALPCommand ac = new AltiLinkPlus.ALPCommand(0, Convert.ToInt32(ALPCmdID.LOGOFF));
					commandsQueue.Enqueue(ac);
					// Setting flags for threads to finish the work.
					inThreadLivingStatus = false;
					outThreadLivingStatus = false;
					commandsQueue.Enqueue(null);
					eventsQueue.Enqueue(null);
					this.tcpc.Close();
					this.ConnectionChecker.Dispose();
					this.bw.Close();
					this.br.Close();
					this.st.Close();
				}					
			}
		}

		/// <summary>
		/// Implements <see cref="Diacom.ISP.SPStatusEvent"/> event of <see cref="Diacom.ISP"/> interface
		/// with information about SP status.
		/// </summary>
		/// <remarks>
		/// <para>Raised when any thread have exception - i.e. when <see cref="Diacom.AltiGen.ASPHdw.Send"/>
		/// or <see cref="Diacom.AltiGen.ASPHdw.Receive"/> methods of 
		/// <see cref="Diacom.AltiGen.ASPHdw"/> class are failed.</para>
		/// <seealso cref="Diacom.ISP.SPStatusEvent"/>
		/// </remarks>
		public event SPStatusEventHandler SPStatusEvent;

		/// <summary>
		/// Implements <see cref="Diacom.ISP.Status"/> of <see cref="Diacom.ISP"/> interface - 
		/// method which indicates status of execution of any method.
		/// </summary>
		/// <remarks>
		/// <para>Check this property every time you think there can be an error during execution 
		/// (see documentation on <see cref="Diacom.AltiGen.ASPHdw"/> methods and constructor).</para>
		/// <seealso cref="Diacom.ISP.Status"/>
		/// </remarks>
		public SPStatus Status()
		{
			return spStatus;
		}

		/// <summary>
		/// Implements <see cref="Diacom.ISP.Connect"/> method of <see cref="Diacom.ISP"/> interface.
		/// Connects to server with given parameters.
		/// </summary>
		/// <param name="serverIP">IP address of server.</param>
		/// <param name="serverPort">Port to connect.</param>
		/// <param name="account">Account.</param>
		/// <param name="password">Password.</param>
		/// <remarks>
		/// <para>Creates new TCP socket, connects to server with given IP address and port,
		/// tries to logon as administrator to "account" with given "password".</para>
		/// <para>Blocks the current thread until initialization is finished.
		/// Waits for connection indefinitely.</para>
		/// <para>Use <see cref="Diacom.AltiGen.ASPHdw.Status"/> property and <see cref="Diacom.SPStatus"/> 
		/// enumeration to check execution results.</para>
		/// <seealso cref="Diacom.AltiGen.ASPHdw.Status"/>
		/// <seealso cref="Diacom.SPStatus"/>
		/// <seealso cref="Diacom.ISP"/>
		/// </remarks>
		public void Connect(string serverIP, int serverPort, string account, string password)
		{
			Initialize(serverIP, serverPort, SPLogonType.ADMINISTRATOR, account, password, 0);
		}

		/// <summary>
		/// Implements <see cref="Diacom.ISP.Connect"/> method of <see cref="Diacom.ISP"/> interface.
		/// Connects to server with given parameters.
		/// </summary>
		/// <param name="serverIP">IP address of server.</param>
		/// <param name="serverPort">Port to connect.</param>
		/// <param name="account">Account.</param>
		/// <param name="password">Password.</param>
		/// <param name="timeout">Timeout interval.</param>
		/// <remarks>
		/// <para>Creates new TCP socket, connects to server with given IP address and port,
		/// tries to logon as administrator to "account" with given "password".</para>
		/// <para>Blocks the current thread until initialization is finished.
		/// Waits for connection "timeout" miliseconds.</para>
		/// <para>Use <see cref="Diacom.AltiGen.ASPHdw.Status"/> property and <see cref="Diacom.SPStatus"/> 
		/// enumeration to check execution results.</para>
		/// <seealso cref="Diacom.AltiGen.ASPHdw.Status"/>
		/// <seealso cref="Diacom.SPStatus"/>
		/// <seealso cref="Diacom.ISP"/>
		/// </remarks>
		public void Connect(string serverIP, int serverPort, string account, string password, int timeout)
		{
			Initialize(serverIP, serverPort, SPLogonType.ADMINISTRATOR, account, password, timeout);
		}

		/// <summary>
		/// Implements <see cref="Diacom.ISP.Connect"/> method of <see cref="Diacom.ISP"/> interface.
		/// Connects to server with given parameters.
		/// </summary>
		/// <param name="serverIP">IP address of server.</param>
		/// <param name="serverPort">Port to connect.</param>
		/// <param name="logonType">Type of logon.</param>
		/// <param name="account">Account.</param>
		/// <param name="password">Password.</param>
		/// <remarks>
		/// <para>Creates new TCP socket, connects to server with given IP address and port,
		/// tries to logon as "logonType" to "account" with given "password".</para>
		/// <para>Blocks the current thread until initialization is finished.
		/// Waits for connection infinitely.</para>
		/// <para>Use <see cref="Diacom.AltiGen.ASPHdw.Status"/> property and <see cref="Diacom.SPStatus"/> 
		/// enumeration to check execution results.</para>
		/// <seealso cref="Diacom.AltiGen.ASPHdw.Status"/>
		/// <seealso cref="Diacom.SPStatus"/>
		/// <seealso cref="Diacom.ISP"/>
		/// </remarks>
		public void Connect(string serverIP, int serverPort, SPLogonType logonType, string account, string password)
		{
			Initialize(serverIP, serverPort, logonType, account, password, 0);
		}

		/// <summary>
		/// Implements <see cref="Diacom.ISP.Connect"/> method of <see cref="Diacom.ISP"/> interface.
		/// Connects to server with given parameters.
		/// </summary>
		/// <param name="serverIP">IP address of server.</param>
		/// <param name="serverPort">Port to connect.</param>
		/// <param name="logonType">Type of logon.</param>
		/// <param name="account">Account.</param>
		/// <param name="password">Password.</param>
		/// <param name="timeout">Timeout interval.</param>
		/// <remarks>
		/// <para>Creates new TCP socket, connects to server with given IP address and port,
		/// tries to logon as "logonType" to "account" with given "password".</para>
		/// <para>Blocks the current thread until initialization is finished.
		/// Waits for connection "timeout" miliseconds.</para>
		/// <para>Use <see cref="Diacom.AltiGen.ASPHdw.Status"/> property and <see cref="Diacom.SPStatus"/> 
		/// enumeration to check execution results.</para>
		/// <seealso cref="Diacom.AltiGen.ASPHdw.Status"/>
		/// <seealso cref="Diacom.SPStatus"/>
		/// <seealso cref="Diacom.ISP"/>
		/// </remarks>
		public void Connect(string serverIP, int serverPort, SPLogonType logonType, string account, string password, int timeout)
		{
			Initialize(serverIP, serverPort, logonType, account, password, timeout);
		}

		/// <summary>
		/// Implements <see cref="System.IDisposable.Dispose"/> method  of <see cref="Diacom.ISP"/> interface.
		/// </summary>
		/// <remarks>
		/// <para>Stops threads, closes socket, etc.</para>
		/// <seealso cref="Diacom.ISP"/>
		///</remarks>
		public void Dispose()
		{
			GC.SuppressFinalize(this);
			Dispose(true);
		}

		/// <summary>
		/// Implements <see cref="Diacom.ISP.Disconnect"/> method of <see cref="Diacom.ISP"/> interface.
		/// Closes the SP - logoffs from the server, releases TCP connections, etc.
		/// </summary>
		/// <remarks>
		/// <seealso cref="Diacom.ISP"/>
		/// </remarks>
		public void Disconnect()
		{
			Dispose();
		}

		/// <summary>
		/// Implements <see cref="Diacom.ISP.Receive"/> method of <see cref="Diacom.ISP"/> interface.
		/// Retrieves next available event (one of <see cref="Diacom.Ev"/> namespace classes).
		/// </summary>
		/// <returns>Next available event.</returns>
		/// <remarks>
		/// <para>Waits while queue is empty.</para>
		/// <para>If execution is not successfull, raises <see cref="Diacom.ISP.SPStatusEvent"/> event.</para>
		/// <seealso cref="Diacom.ISP"/>
		/// </remarks>
		public Diacom.Ev.EvBase Receive()
		{
			return (Diacom.Ev.EvBase) eventsQueue.Dequeue();
		}

		/// <summary>
		/// Implements <see cref="Diacom.ISP.Send"/> method of <see cref="Diacom.ISP"/> interface.
		/// Sends command (one of <see cref="Diacom.Cmd"/> namespace classes).
		/// </summary>
		/// <param name="cmd">Command to send.</param>
		/// <remarks>
		/// <para>Returns immediately.</para>
		/// <seealso cref="Diacom.ISP"/>
		/// </remarks>
		public void Send(Diacom.Cmd.CmdBase cmd)
		{
			commandsQueue.Enqueue(cmd);
		}

		/// <summary>
		/// Implements <see cref="Diacom.ISP.GetLines"/> method of <see cref="Diacom.ISP"/> interface.
		/// Provides an access to all the lines available.
		/// </summary>
		/// <returns>Pointer to array of <see cref="Diacom.SPLine"/> type (null if error).</returns>
		/// <remarks>
		/// <seealso cref="Diacom.ISP"/>
		/// </remarks>
		public SPLine [] GetLines()
		{
			SPLine [] _result = null;
			try
			{
				// Need to return a new instance of SPLine [].
				_result = new SPLine[lines.Count];
				// Passing thru all the lines.
				int _index = 0;
				foreach(Line line in lines.Values)
				{
					// Fill the resulting array.
					_result[_index++] = (SPLine)line.Info.Clone();
				}
				spStatus = SPStatus.OK;
			}
			catch(OutOfMemoryException _e)
			{
				TraceOut.Put(_e);
				spStatus = SPStatus.ERROR_CREATING_COMPONENTS;
			}
			catch
			{
				TraceOut.Put("Unknown exception");
				spStatus = SPStatus.ERROR_UNHANDLED_EXCEPTION;
			}
			Array.Reverse(_result);
			return _result;
		}

		/// <summary>
		/// Implements <see cref="Diacom.ISP.GetLine"/> method of <see cref="Diacom.ISP"/> interface.
		/// Provides an access to specified line.
		/// </summary>
		/// <param name="lineID">Line ID.</param>
		/// <returns><see cref="Diacom.SPLine"/> instance if line exists, null otherwise.</returns>
		/// <remarks>
		/// <seealso cref="Diacom.ISP"/>
		/// </remarks>
		public SPLine GetLine(object lineID)
		{
			try
			{
				spStatus = SPStatus.OK;
				return ((SPLine)(((Line)(lines[lineID])).Info.Clone()));
			}
			catch(OutOfMemoryException _e)
			{
				TraceOut.Put(_e);
				spStatus = SPStatus.ERROR_CREATING_COMPONENTS;
			}
			catch
			{
				TraceOut.Put("Unknown Exception");
				spStatus = SPStatus.ERROR_UNHANDLED_EXCEPTION;
			}
			return null;
		}
		#endregion

		#region ------------------------------------------------------------------------- Commands conversion ---
		/// <summary>
		/// Converts object into AltiLink Plus v2.0 format and returns it.
		/// </summary>
		/// <param name="obj">Command to convert (of "object" type).</param>
		/// <returns>Command in AltiLink Plus v2.0 format (or null).</returns>
		private AltiLinkPlus.ALPCommand ProcessCommand(object obj)
		{
			AltiLinkPlus.ALPCommand cmd = null;
			Line _line = null;

			if(obj is Cmd.CmdBase)
			{
				_line = (Line)lines[((Cmd.CmdBase)(obj)).Line];
				// Check if the line exists.
				if(_line == null)
				{
					// Where is no such line - bug? Telling him his command did not work properly.
					Cmd.CmdBase _cmd = ((Cmd.CmdBase)(obj));
					eventsQueue.Enqueue(new Ev.CommandStatus(_cmd.Sender, _cmd.Line, _cmd.ID, Ev.CmdStatus.ERROR));
					return null;
				}
			}
			// Have the line the command is for. Processing the command.

			// Aswer the call.
			if(obj is Cmd.AnswerCall)
			{
				Cmd.AnswerCall _cmd = ((Cmd.AnswerCall)(obj));
				// Creating command.
				cmd = new AltiLinkPlus.ALPCommand(this.locationID, (Convert.ToInt32(ALPCmdID.ANSWER)));
				cmd[0] = new AltiLinkPlus.ALPParameter(Convert.ToInt32(_cmd.Line));
				cmd[1] = new AltiLinkPlus.ALPParameter(0);
				// Saving information about the command in line's Line class.
				_line.LastCommand.Set(cmd.SequenceId, ALPCmdID.ANSWER, _cmd.Line, ALPCmdStatus.STARTED);
				return cmd;
			} 

			// Connect lines command.
			if(obj is Cmd.Connect)
			{
				Cmd.Connect _cmd = ((Cmd.Connect)obj);
				Line _line1 = (Line)lines[_cmd.LineOne];
				Line _line2 = (Line)lines[_cmd.LineTwo];
				if(_line1 != null && _line2 != null)
				{
					// Creating command.
					cmd = new AltiLinkPlus.ALPCommand(this.locationID, (Convert.ToInt32(ALPCmdID.APC_CONNECT_CALL)));
					cmd[0] = new AltiLinkPlus.ALPParameter(Convert.ToInt32(_cmd.LineOne));
					cmd[1] = new AltiLinkPlus.ALPParameter(Convert.ToInt32(_cmd.LineTwo));
					cmd[2] = new AltiLinkPlus.ALPParameter(0);
					// Saving information about the command in line's Line class.
					_line1.LastCommand.Set(cmd.SequenceId, ALPCmdID.APC_CONNECT_CALL, _cmd.Sender, ALPCmdStatus.STARTED);
					_line2.LastCommand.Set(cmd.SequenceId, ALPCmdID.APC_CONNECT_CALL, _cmd.Sender, ALPCmdStatus.STARTED);
				}
				else
				{
					// Where are no such lines to connect - bug? Telling the command will not work properly.
					eventsQueue.Enqueue(new Ev.CommandStatus(_cmd.Sender, _cmd.Line, _cmd.ID, Ev.CmdStatus.ERROR));
				}
				return cmd;
			} 

			// Dial the number.
			if(obj is Cmd.Dial)
			{
				Cmd.Dial _cmd = ((Cmd.Dial)obj);
				string _numberToDial = _line.Info.AccessCode + 
					((_cmd.Destination == string.Empty) ?  "**5" : _cmd.Destination);
				// Creating command.
				cmd = new AltiLinkPlus.ALPCommand(this.locationID, (Convert.ToInt32(ALPCmdID.APC_MAKE_CALL)));
				cmd[0] = new AltiLinkPlus.ALPParameter(_numberToDial);
				cmd[1] = new AltiLinkPlus.ALPParameter(_cmd.Account);
				cmd[2] = new AltiLinkPlus.ALPParameter(_cmd.Tone);
				cmd[3] = new AltiLinkPlus.ALPParameter(_cmd.Source);
				cmd[4] = new AltiLinkPlus.ALPParameter(0);
				// Saving information about the command.
				OutcallCommandStruct odc = new OutcallCommandStruct(_cmd.Sender, _cmd.Line, Cmd.CommandID.DIAL);
				outcallNumbersDialingCommands.Add(cmd.SequenceId, odc);
				return cmd;
			} 

			// Disconnect.
			if(obj is Cmd.Disconnect)
			{
				Cmd.Disconnect _cmd = ((Cmd.Disconnect)obj);
				// Creating command.
				cmd = new AltiLinkPlus.ALPCommand(this.locationID, (Convert.ToInt32(ALPCmdID.APC_DROP_CALL)));
				cmd[0] = new AltiLinkPlus.ALPParameter(Convert.ToInt32(_cmd.Line));
				cmd[1] = new AltiLinkPlus.ALPParameter(0);
				// Saving information about the command in line's Line class.
				_line.LastCommand.Set(cmd.SequenceId, ALPCmdID.APC_DROP_CALL, _cmd.Sender, ALPCmdStatus.STARTED);
				return cmd;
			} 

			// Play DTMF.
			if(obj is Cmd.PlayDTMF)
			{
				Cmd.PlayDTMF _cmd = ((Cmd.PlayDTMF)obj);
				// Creating command.
				cmd = new AltiLinkPlus.ALPCommand(this.locationID, (Convert.ToInt32(ALPCmdID.APC_PLAY_DTMF)));
				cmd[0] = new AltiLinkPlus.ALPParameter(Convert.ToInt32(_cmd.Line));
				cmd[1] = new AltiLinkPlus.ALPParameter(_cmd.DTMFCode);
				cmd[2] = new AltiLinkPlus.ALPParameter(0);
				// Saving information about the command in line's Line class.
				_line.LastCommand.Set(cmd.SequenceId, ALPCmdID.APC_PLAY_DTMF, _cmd.Sender, ALPCmdStatus.STARTED);
				return cmd;
			} 

			// Play file.
			if(obj is Cmd.PlayFile)
			{
				Cmd.PlayFile _cmd = ((Cmd.PlayFile)obj);
				// Creating command.
				cmd = new AltiLinkPlus.ALPCommand(this.locationID, (Convert.ToInt32(ALPCmdID.APC_PLAY_VOICE)));
				cmd[0] = new AltiLinkPlus.ALPParameter(Convert.ToInt32(_cmd.Line));
				cmd[1] = new AltiLinkPlus.ALPParameter(_cmd.CutOffString);
				cmd[2] = new AltiLinkPlus.ALPParameter(_cmd.FilePath);
				cmd[3] = new AltiLinkPlus.ALPParameter(0);
				cmd[4] = new AltiLinkPlus.ALPParameter(0);
				// Saving information about the command in line's Line class.
				_line.LastCommand.Set(cmd.SequenceId, ALPCmdID.APC_PLAY_VOICE, _cmd.Sender, ALPCmdStatus.STARTED);
				return cmd;
			} 

			// Record file.
			if(obj is Cmd.RecordFile)
			{
				Cmd.RecordFile _cmd = ((Cmd.RecordFile)obj);
				// Creating command.
				cmd = new AltiLinkPlus.ALPCommand(this.locationID, (Convert.ToInt32(ALPCmdID.APC_RECORD_VOICE)));
				// Adding parameters.
				cmd[0] = new AltiLinkPlus.ALPParameter(Convert.ToInt32(_cmd.Line));
				cmd[1] = new AltiLinkPlus.ALPParameter(_cmd.CutOffString);
				cmd[2] = new AltiLinkPlus.ALPParameter(_cmd.FilePath);
				cmd[3] = new AltiLinkPlus.ALPParameter(0);
				cmd[4] = new AltiLinkPlus.ALPParameter(_cmd.AppendMode);
				cmd[5] = new AltiLinkPlus.ALPParameter(0);
				// Saving information about the command in line's Line class.
				_line.LastCommand.Set(cmd.SequenceId, ALPCmdID.APC_RECORD_VOICE, _cmd.Sender, ALPCmdStatus.STARTED);
				return cmd;
			}

			// Reset.
			if(obj is Cmd.Reset)
			{
				Cmd.Reset _cmd = ((Cmd.Reset)obj);
				switch(_line.LastCommand.ID)
				{
					case ALPCmdID.APC_PLAY_VOICE:
					{
						cmd = new AltiLinkPlus.ALPCommand(this.locationID, Convert.ToInt32(ALPCmdID.APC_STOP_PLAY_VOICE));
						cmd[0] = new AltiLinkPlus.ALPParameter(Convert.ToInt32(_cmd.Line));
						// Type (0 - stop play current, 1 - stop play all).
						cmd[1] = new AltiLinkPlus.ALPParameter(1);
						cmd[2] = new AltiLinkPlus.ALPParameter(0);
						break;
					}
					case ALPCmdID.APC_RECORD_VOICE:
					{
						cmd = new AltiLinkPlus.ALPCommand(this.locationID, Convert.ToInt32(ALPCmdID.APC_STOP_RECORD_VOICE));
						cmd[0] = new AltiLinkPlus.ALPParameter(Convert.ToInt32(_cmd.Line));
						// Type (0 - stop record current, 1 - stop record all).
						cmd[1] = new AltiLinkPlus.ALPParameter(1);
						cmd[2] = new AltiLinkPlus.ALPParameter(0);
						break;
					}
					default:
					{
						break;
					}
				}
				return cmd;
			}

			// Reject call.
			if(obj is Cmd.RejectCall)
			{
				Cmd.RejectCall _cmd = ((Cmd.RejectCall)obj);
				// Creating command.
				cmd = new AltiLinkPlus.ALPCommand(this.locationID, (Convert.ToInt32(ALPCmdID.APC_DROP_CALL)));
				cmd[0] = new AltiLinkPlus.ALPParameter(Convert.ToInt32(_cmd.Line));
				cmd[1] = new AltiLinkPlus.ALPParameter(0);
				// Saving information about the command in line's Line class.
				_line.LastCommand.Set(cmd.SequenceId, ALPCmdID.APC_DROP_CALL, _cmd.Sender, ALPCmdStatus.STARTED);
				return cmd;
			}

			// Ring extension.
			if(obj is Cmd.RingExtension)
			{
				Cmd.RingExtension _cmd = ((Cmd.RingExtension)obj);
				string _numberToRing = _line.Info.AccessCode +  _cmd.Extension;
				// Creating command.
				cmd = new AltiLinkPlus.ALPCommand(this.locationID, (Convert.ToInt32(ALPCmdID.APC_RING_EXT)));
				// Adding parameters.
				cmd[0] = new AltiLinkPlus.ALPParameter(_cmd.RingType);
				cmd[1] = new AltiLinkPlus.ALPParameter(_numberToRing);
				cmd[2] = new AltiLinkPlus.ALPParameter(0);
				OutcallCommandStruct odc = new OutcallCommandStruct(_cmd.Sender, _cmd.Line, Cmd.CommandID.RING_EXTENSION);
				outcallNumbersDialingCommands.Add(cmd.SequenceId, odc);
				return cmd;
			}

			// Snatch line.
			if(obj is Cmd.SnatchLine)
			{
				Cmd.SnatchLine _cmd = ((Cmd.SnatchLine)obj);
				// Creating command.
				cmd = new AltiLinkPlus.ALPCommand(this.locationID, (Convert.ToInt32(ALPCmdID.APC_SNATCH_LINE)));
				// Adding parameters.
				cmd[0] = new AltiLinkPlus.ALPParameter(Convert.ToInt32(_cmd.Line));
				cmd[1] = new AltiLinkPlus.ALPParameter(0);
				// Saving information about the command in line's Line class.
				_line.LastCommand.Set(cmd.SequenceId, ALPCmdID.APC_SNATCH_LINE, _cmd.Sender, ALPCmdStatus.STARTED);
				return cmd;
			}

			// Switch music.
			if(obj is Cmd.SwitchMusic)
			{
				Cmd.SwitchMusic _cmd = ((Cmd.SwitchMusic)obj);
				// Creating command.
				cmd = new AltiLinkPlus.ALPCommand(this.locationID, (Convert.ToInt32(ALPCmdID.APC_SWITCH_MUSIC)));
				// Adding parameters.
				cmd[0] = new AltiLinkPlus.ALPParameter(Convert.ToInt32(_cmd.Line));
				cmd[1] = new AltiLinkPlus.ALPParameter(_cmd.MusicMode);
				cmd[2] = new AltiLinkPlus.ALPParameter(0);
				// Saving information about the command in line's Line class.
				_line.LastCommand.Set(cmd.SequenceId, ALPCmdID.APC_SWITCH_MUSIC, _cmd.Sender, ALPCmdStatus.STARTED);
				return cmd;
			}

			// Transfer call.
			if(obj is Cmd.TransferCall)
			{
				const int TRANSFER_CALL_TIMEOUT = 10;
				Cmd.TransferCall _cmd = ((Cmd.TransferCall)obj);
				Line _target = (Line)lines[_cmd.Target];

				if(_target != null)
				{
					string _numberToRing = _target.Info.AccessCode + _cmd.Destination;
					cmd = new AltiLinkPlus.ALPCommand(this.locationID, (Convert.ToInt32(ALPCmdID.APC_TRANSFER_CALL)));
					cmd[0] = new AltiLinkPlus.ALPParameter(Convert.ToInt32(_cmd.Line));
					cmd[1] = new AltiLinkPlus.ALPParameter(Convert.ToInt32(_cmd.Type));
					switch(_cmd.Type)
					{
						case Cmd.TransferCallType.EXTENSION:
						case Cmd.TransferCallType.EXTENSION_VOICE_MESSAGE:
						{
							cmd[2] = new AltiLinkPlus.ALPParameter(Convert.ToInt32(_numberToRing));
							cmd[3] = new AltiLinkPlus.ALPParameter("");
							break;
						}

						case Cmd.TransferCallType.AUTOATEDDANT:
						{
							if(_cmd.Destination == string.Empty ) cmd[2] = new AltiLinkPlus.ALPParameter(1);
							else cmd[2] = new AltiLinkPlus.ALPParameter(Convert.ToInt32(_cmd.Destination));
							cmd[3] = new AltiLinkPlus.ALPParameter("");
							break;
						}

						case Cmd.TransferCallType.OPERATOR:
						{
							cmd[2] = new AltiLinkPlus.ALPParameter(0);
							cmd[3] = new AltiLinkPlus.ALPParameter(_cmd.Destination);
							break;
						}

						case Cmd.TransferCallType.TRUNK:
						{
							_numberToRing += (_cmd.Destination == string.Empty) ?  "**5" : string.Empty;
							cmd[2] = new AltiLinkPlus.ALPParameter(0);
							cmd[3] = new AltiLinkPlus.ALPParameter(_numberToRing);
							break;
						}
					}
					cmd[4] = new AltiLinkPlus.ALPParameter(TRANSFER_CALL_TIMEOUT);
					cmd[5] = new AltiLinkPlus.ALPParameter(0);
					// Saving information about the command in line's Line class.
					_line.LastCommand.Set(cmd.SequenceId, ALPCmdID.APC_TRANSFER_CALL, _cmd.Sender, ALPCmdStatus.STARTED);
				}
				else
				{
					// Where is no line to transfer call to - bug? Telling the command did not work properly.
					eventsQueue.Enqueue(new Ev.CommandStatus(_cmd.Sender, _cmd.Line, Cmd.CommandID.TRANSFER, Ev.CmdStatus.ERROR));
				}
				return cmd;
			}

			// Inner command.
			if(obj is AltiLinkPlus.ALPCommand)
			{
				return ((AltiLinkPlus.ALPCommand)obj);
			}
			
			// Not a command at all.
			return null;
		}
		#endregion

		#region ----------------------------------------------------------------- Events/responces conversion ---
		/// <summary>
		/// Processes all events.
		/// </summary>
		/// <param name="altiEvent">Block of data to process.</param>
		/// <returns>An object whith the event.</returns>
		private void ProcessEvent(AltiLinkPlus.ALPEvent altiEvent)
		{
			// Getting information only if event is for our SP control line.
			if(altiEvent.LocationId == locationID)
			{
				switch((ALPEvID)(altiEvent.CommandId))
				{
						// Event Ev.Ring.
					case ALPEvID.APC_CALLPRESENT:
					{
						// Creating new CallInfo object.
						CallInfo rInfo = new CallInfo(((AltiLinkPlus.ALPParameter)(altiEvent[0])));
						// Getting current line from hashtable.
						Line _line = ((Line)(lines[rInfo.lineID]));
						if(_line != null)
						{
							// Updating line information.
							_line.Info.CalledNumber = rInfo.callerID;
							_line.Info.CalledName = rInfo.callerName;
							_line.Info.DNISNumber = rInfo.DNISID;
							_line.Info.DNISName = rInfo.DNISName;
							_line.Info.CIDNumber = rInfo.ANIID;
							_line.Info.CIDName = rInfo.ANIName;
							_line.Info.DIDNumber = rInfo.calleeID;
							_line.Info.DIDName = rInfo.calleeName;
							if(!_line.IsUnderAPCControl) eventsQueue.Enqueue(new Ev.Ring(rInfo.lineID, _line.Info));
						}
						break;
					}
						// Event Ev.RingBack.
					case ALPEvID.APC_RINGBACK:
					{
						// Creating new CallInfo object.
						CallInfo rbInfo = new CallInfo(((AltiLinkPlus.ALPParameter)(altiEvent[0])));
						// Getting current line from hashtable.
						Line _line = ((Line)(lines[rbInfo.lineID]));
						if(_line != null)
						{
							// Updating line information.
							if(_line.Info.Type == "T")
							{
								_line.Info.CalledNumber = _line.Info.AccessCode + rbInfo.calleeID;
								_line.Info.CalledName = rbInfo.calleeName;
							}
							else
							{
								_line.Info.CalledNumber = rbInfo.callerID;
								_line.Info.CalledName = rbInfo.callerName;
							}
							_line.Info.DNISNumber = rbInfo.DNISID;
							_line.Info.DNISName = rbInfo.DNISName;
							_line.Info.CIDNumber = rbInfo.ANIID;
							_line.Info.CIDName = rbInfo.ANIName;
							_line.Info.DIDNumber = rbInfo.calleeID;
							_line.Info.DIDName = rbInfo.calleeName;
							if(!_line.IsUnderAPCControl) eventsQueue.Enqueue(new Ev.RingBack(rbInfo.lineID, _line.Info));
						}
						break;
					}
						// Event Ev.Connect.
					case ALPEvID.STCHG:
					{
						// Getting parameters.
						ALPLineState state = ((ALPLineState)(altiEvent[0].ReadInt32()));
						int lineID = altiEvent[1].ReadInt32();
						// Getting current line from hashtable.
						Line _line = ((Line)(lines[lineID]));
						if((_line != null)&&(state == ALPLineState.SP))
						{
							// Initializing line as new uncracked line.
							_line.LastCommand.Set(0, ALPCmdID.NOT_A_COMMAND, 0, ALPCmdStatus.STARTED);
							if(!_line.IsUnderAPCControl)
							{
								_line.IsUnderAPCControl = true;
								// Have new connected line. Creating and sending event.
								eventsQueue.Enqueue(new Ev.Connect(lineID));
							}
						}
						break;
					}
						// Event Ev.Digit.
					case ALPEvID.APC_DIGIT:
					{
						// Getting parameters.
						AltiLinkPlus.ALPParameter param = (AltiLinkPlus.ALPParameter)altiEvent[0];
						int lineID = param.ReadInt32();
						char digit = Convert.ToChar(param.ReadByte());
						// Creating and sending event.
						eventsQueue.Enqueue(new Ev.Digit(lineID, digit));
						break;
					}
						// Event Ev.Disconnect.
					case ALPEvID.APC_CALLDROP:
					{
						// Getting parameters.
						int lineID = altiEvent[0].ReadInt32();
						Line _line = ((Line)(lines[lineID]));
						// Check if somebody commands to this line to disconnect - then command is OK.
						if(_line.LastCommand.ID == ALPCmdID.APC_DROP_CALL) eventsQueue.Enqueue(new Ev.CommandStatus(lineID, _line.LastCommand.SenderLine, Cmd.CommandID.DISCONNECT, Ev.CmdStatus.OK));
						break;
					}
						// Event Ev.CommandStatus.
					case ALPEvID.APC_STATUS:
					{
						// Creating new StateInfo object.
						StateInfo sInfo = new StateInfo((AltiLinkPlus.ALPParameter)(altiEvent[0]));
						ALPCmdStatus lastCmdStatus = (ALPCmdStatus) sInfo.cmdStatus;
						// Getting current line from hashtable.
						Line _line = ((Line)(lines[sInfo.lineID]));
						if ( _line == null) break;
						// Remember line status.
						_line.LastCommand.Status = lastCmdStatus;
						// What a command?
						switch((ALPCmdID)(sInfo.commandID))
						{
								// Status of APC_PLAY_DTMF command.
							case ALPCmdID.APC_PLAY_DTMF:
							{
								if(_line.LastCommand.ID == ALPCmdID.APC_PLAY_DTMF)
								{
									if(lastCmdStatus == ALPCmdStatus.FAILED) eventsQueue.Enqueue(new Ev.CommandStatus(sInfo.lineID, _line.LastCommand.SenderLine, Cmd.CommandID.PLAY_DTMF, Ev.CmdStatus.ERROR));
									else eventsQueue.Enqueue(new Ev.CommandStatus(sInfo.lineID, _line.LastCommand.SenderLine, Cmd.CommandID.PLAY_DTMF, Ev.CmdStatus.OK));
									break;
								}
								break;
							}
								// Status of APC_PLAY_VOICE command.
							case ALPCmdID.APC_PLAY_VOICE:
							{
								if(_line.LastCommand.ID == ALPCmdID.APC_PLAY_VOICE)
								{
									if(lastCmdStatus == ALPCmdStatus.FINISHED) eventsQueue.Enqueue(new Ev.CommandStatus(sInfo.lineID, _line.LastCommand.SenderLine, Cmd.CommandID.PLAY_FILE, Ev.CmdStatus.OK));
									if(lastCmdStatus == ALPCmdStatus.FAILED) eventsQueue.Enqueue(new Ev.CommandStatus(sInfo.lineID, _line.LastCommand.SenderLine, Cmd.CommandID.PLAY_FILE, Ev.CmdStatus.ERROR));
									break;
								}
								break;
							}
								// Status of APC_RECORD_VOICE command.
							case ALPCmdID.APC_RECORD_VOICE:
							{
								if(_line.LastCommand.ID == ALPCmdID.APC_RECORD_VOICE)
								{
									if(lastCmdStatus == ALPCmdStatus.FINISHED) eventsQueue.Enqueue(new Ev.CommandStatus(sInfo.lineID, _line.LastCommand.SenderLine, Cmd.CommandID.RECORD_RILE, Ev.CmdStatus.OK));
									if(lastCmdStatus == ALPCmdStatus.FAILED) eventsQueue.Enqueue(new Ev.CommandStatus(sInfo.lineID, _line.LastCommand.SenderLine, Cmd.CommandID.RECORD_RILE, Ev.CmdStatus.ERROR));
									break;
								}
								break;
							}
								// Status of APC_CONNECT_CALL command.
							case ALPCmdID.APC_CONNECT_CALL:
							{
								if((_line.LastCommand.ID == ALPCmdID.APC_CONNECT_CALL)&&(lastCmdStatus == ALPCmdStatus.SUCCEED))
								{
									eventsQueue.Enqueue(new Ev.CommandStatus(sInfo.lineID, _line.LastCommand.SenderLine, Cmd.CommandID.CONNECT, Ev.CmdStatus.OK));
								}
								break;
							}
						}
						break;
					}
					default:
					{
						// Debug stuff.
						TraceOut.Put(altiEvent);
						break;
					}
				}
			}
				// Getting information for all other line PADs - just want to know that is going on.
			else
			{
				switch((ALPEvID)(altiEvent.CommandId))
				{
						// Event Ev.Tone.
					case ALPEvID.TONE:
					{
						// Getting parameters.
						AltiLinkPlus.ALPParameter param = (AltiLinkPlus.ALPParameter)altiEvent[0];
						int line = param.ReadInt32();
						param = (AltiLinkPlus.ALPParameter)altiEvent[1];
						int tone = Convert.ToChar(param.ReadInt32());
						// Creating and sending event.
						eventsQueue.Enqueue(new Ev.Tone(line, ((Ev.ToneType)(tone))));
						break;
					}
						// Event Ev.LineStateChange.
					case ALPEvID.STCHG:
					{
						if(lines.ContainsKey(altiEvent.LocationId))
						{
							// Saving line state.
							SPLineState state = ((SPLineState)(altiEvent[0].ReadInt32()));
							Line _line = ((Line)(lines[altiEvent.LocationId]));
							// Rasing event the line is disconnected if status is DISCONNECT.
							if((state.Equals(SPLineState.DISCONNECT))&&(!(_line.Info.State.Equals(SPLineState.OFFHOOK))))
							{
								_line.LastCommand.Set(0, ALPCmdID.NOT_A_COMMAND, 0, ALPCmdStatus.STARTED);
								if(_line.IsUnderAPCControl) eventsQueue.Enqueue(new Ev.Disconnect(_line.Info.ID));
								_line.IsUnderAPCControl = false;
							}
							_line.Info.State = state;
							// Rasing event state of the line changed.
							eventsQueue.Enqueue(new Ev.LineStateChanged(_line.Info.ID, state));
						}
						break;
					}
						// Line information changed.
					case ALPEvID.CONFIG_CHG:
					{
						const int CONFIG_CHANGED_TYPE_LINEINFO = 0x8000;
						AltiLinkPlus.ALPParameter param = (AltiLinkPlus.ALPParameter)altiEvent[0];
						int type = param.ReadInt32();
						if((lines.ContainsKey(altiEvent.LocationId))&&(type == CONFIG_CHANGED_TYPE_LINEINFO))
						{
							// Command to Get Lines Information.
							commandsQueue.Enqueue(new Diacom.AltiGen.AltiLinkPlus.ALPCommand(altiEvent.LocationId, Convert.ToInt32(ALPCmdID.GET_LINEINFO)));
						}
						break;
					}
						// System configuration changed.
					case ALPEvID.SYSCONFIG_CHG:
					{
						AltiLinkPlus.ALPParameter param = (AltiLinkPlus.ALPParameter)altiEvent[0];
						SystemConfigChangeCodes type = ((SystemConfigChangeCodes)(param.ReadInt32()));
						switch(type)
						{
								// Line added.
							case SystemConfigChangeCodes.LINEADD:
							{
								if(!(lines.ContainsKey(altiEvent.LocationId)))
								{
									// Command to Get Lines Information.
									commandsQueue.Enqueue(new Diacom.AltiGen.AltiLinkPlus.ALPCommand(altiEvent.LocationId, Convert.ToInt32(ALPCmdID.GET_LINEINFO)));
									LineInfo lineInfo = new LineInfo();
									Line line = new Line(lineInfo);
									line.Info.State = SPLineState.LINE_ADD;
									lines.Add(altiEvent.LocationId, line);
								}
								break;
							}
								// Line removed.
							case SystemConfigChangeCodes.LINEREMOVE:
							{
								if(lines.ContainsKey(altiEvent.LocationId))
								{
									lines.Remove(altiEvent.LocationId);
									eventsQueue.Enqueue(new Ev.LineStateChanged(altiEvent.LocationId, SPLineState.LINE_REMOVE));
								}
								break;
							}
						}
						break;
					}
					default:
					{
						//Debug stuff.
						TraceOut.Put(altiEvent);
						break;
					}
				}
			}
		}

		/// <summary>
		/// Process all responses.
		/// </summary>
		/// <param name="altiResponse">Block of data to process.</param>
		/// <returns>An object with the response.</returns>
		private void ProcessResponse(AltiLinkPlus.ALPResponse altiResponse)
		{
			switch((ALPCmdID)(altiResponse.CommandId))
			{
					// Responses on commands.
					// Logon.
				case ALPCmdID.LOGON:
				{
					if(altiResponse.ResponseCode == 0)
					{
						// Getting our control line PAD.
						locationID = altiResponse.LocationId;
						// Setting event if already have lines hashtable.
						if(lines != null) connectionEstablished.Set();
						TraceOut.Put("Logon saccessfull...");
					}
					else
					{
						// Have a bad response - can't logon.
						locationID = 0;
						RaiseEvent(SPStatus.ERROR_LOGON, ((ALPRespID)(altiResponse.ResponseCode)).ToString());
						TraceOut.Put("Logon failed... Responce is "+((ALPRespID)(altiResponse.ResponseCode)).ToString());
						connectionEstablished.Set();
					}
					break;
				}

				case ALPCmdID.GET_VERSION:
				{
					int versionID = altiResponse[0].ReadInt32();
					bool enableOptionPack = Convert.ToBoolean(altiResponse[1].ReadByte());
					
					break;
				}
					// Get lines info.
				case ALPCmdID.GET_LINEINFO:
				{
					if(altiResponse.ResponseCode == 0)
					{
						// First parameter in response is number of entries.
						int	linesCount = altiResponse[0].ReadInt32();
						// Creating lines hashtable.
						if(lines == null) lines = new Hashtable();
						// Second parameter holds lines information.
						AltiLinkPlus.ALPParameter param = altiResponse[1];
						// Filling lines hashtable.
						for(int i = 0; i < linesCount; i++)
						{
							LineInfo lineInfo = new LineInfo(param);
							if(!(lines.ContainsKey(lineInfo.lineHandle)))
							{
								Line line = new Line(lineInfo);
								lines.Add(lineInfo.lineHandle, line);
							}
							else
							{
								lines[lineInfo.lineHandle] = new Line(lineInfo);
								eventsQueue.Enqueue(new Ev.LineStateChanged(altiResponse.LocationId, SPLineState.LINE_ADD));
							}
						}
						TraceOut.Put(linesCount.ToString()+" lines added...");
					}
					else
					{
						// Setting event if already have our control line PAD.
						connectionEstablished.Set();
						TraceOut.Put("All lines received.");
					}
					break;
				}
				case ALPCmdID.PING:
				{
					TraceOut.Put("PING response received.");
					this.ConnectionIsAlive = true;
					break;
				}
					// Answer call.
				case ALPCmdID.ANSWER:
				{
					// What to do?
					break;
				}
					// Connect.
				case ALPCmdID.APC_CONNECT_CALL:
				{
					if(altiResponse.LocationId == locationID)
					{
						if(altiResponse.ResponseCode != 0)
						{
							// If all is terrible...
							foreach(Line _line in lines.Values)
							{
								// Searching for a lines command to connect was sent on.
								if(_line.LastCommand.SequenceID == altiResponse.SequenceId)
								{
									// Have appropriate line.
									_line.LastCommand.Set(0, ALPCmdID.NOT_A_COMMAND, 0, ALPCmdStatus.STARTED);
									eventsQueue.Enqueue(new Ev.CommandStatus(_line.Info.ID, _line.LastCommand.SenderLine, Cmd.CommandID.CONNECT, Ev.CmdStatus.ERROR));
								}
							}
						}
					}
					break;
				}
					// Disconnect.
				case ALPCmdID.APC_DROP_CALL:
				{
					// 15:26:37:796:
					// Thread 64C: 22.06.2004 15:26:37 796: Unhandled response: APC_DROP_CALL
					// Sequence ID = 4, location ID = 23, response ID = 1051, number of parameters = 0, response code: 3998
					// Check if somebody commands to this line to disconnect.
					foreach(DictionaryEntry _lineDE in lines)
					{
						Line _line = ((Line)(_lineDE.Value));
						if((_line.LastCommand.ID == ALPCmdID.APC_DROP_CALL)&&(_line.LastCommand.SequenceID == altiResponse.SequenceId))
						{
							eventsQueue.Enqueue(new Ev.CommandStatus(_line.Info.ID, _line.LastCommand.SenderLine, Cmd.CommandID.DISCONNECT, Ev.CmdStatus.ERROR));
						}
					}
					break;
				}
					// Dial.
				case ALPCmdID.APC_MAKE_CALL:
				{
					if(altiResponse.LocationId == locationID)
					{
						// Searching for a line command to dial was sent on.
						OutcallCommandStruct odc = ((OutcallCommandStruct)(outcallNumbersDialingCommands[altiResponse.SequenceId]));
						if(odc != null)
						{
							outcallNumbersDialingCommands.Remove(altiResponse.SequenceId);
							if(odc.ID == Cmd.CommandID.DIAL)
							{
								// Have appropriate line.
								if(altiResponse.ResponseCode != 0) eventsQueue.Enqueue(new Ev.CommandStatus(odc.Line, odc.Sender, Cmd.CommandID.DIAL, Ev.CmdStatus.ERROR));
								else eventsQueue.Enqueue(new Ev.CommandStatus(odc.Line, odc.Sender, Cmd.CommandID.DIAL, Ev.CmdStatus.OK));
							}
							break;
						}
					}
					break;
				}
					// Play DTMF.
				case ALPCmdID.APC_PLAY_DTMF:
				{
					if((altiResponse.LocationId == locationID)&&(altiResponse.ResponseCode != 0))
					{
						foreach(Line _line in lines.Values)
						{
							// Searching for a line command to play DTMF was sent on.
							if((_line.LastCommand.ID == ALPCmdID.APC_PLAY_DTMF)&&
								(_line.LastCommand.SequenceID == altiResponse.SequenceId))
							{
								// Have appropriate line.
								eventsQueue.Enqueue(new Ev.CommandStatus(_line.Info.ID, _line.LastCommand.SenderLine, Cmd.CommandID.PLAY_DTMF, Ev.CmdStatus.ERROR));
								break;
							}
						}
					}
					break;
				}
				case ALPCmdID.APC_RECORD_VOICE:
				{
					if((altiResponse.LocationId == locationID)&&(altiResponse.ResponseCode != 0))
					{
						foreach(Line _line in lines.Values)
						{
							// Searching for a line command to record file was sent on.
							if((_line.LastCommand.ID == ALPCmdID.APC_RECORD_VOICE)&&
								(_line.LastCommand.SequenceID == altiResponse.SequenceId))
							{
								// Have appropriate line.
								eventsQueue.Enqueue(new Ev.CommandStatus(_line.Info.ID, _line.LastCommand.SenderLine, Cmd.CommandID.RECORD_RILE, Ev.CmdStatus.ERROR));
								break;
							}
						}
					}
					break;
				}
					// Play file.
				case ALPCmdID.APC_PLAY_VOICE:
				{
					if((altiResponse.LocationId == locationID)&&(altiResponse.ResponseCode != 0))
					{
						foreach(Line _line in lines.Values)
						{
							// Searching for a line command to play file was sent on.
							if((_line.LastCommand.ID == ALPCmdID.APC_PLAY_VOICE)&&
								(_line.LastCommand.SequenceID == altiResponse.SequenceId))
							{
								// Have appropriate line.
								eventsQueue.Enqueue(new Ev.CommandStatus(_line.Info.ID, _line.LastCommand.SenderLine, Cmd.CommandID.PLAY_FILE, Ev.CmdStatus.ERROR));
								break;
							}
						}
					}
					break;
				}
					// Ring extension.
				case ALPCmdID.APC_RING_EXT:
				{
					if(altiResponse.LocationId == locationID)
					{
						// Find the line command to dial was sent on.
						OutcallCommandStruct odc = ((OutcallCommandStruct)(outcallNumbersDialingCommands[altiResponse.SequenceId]));
						if(odc != null)
						{
							outcallNumbersDialingCommands.Remove(altiResponse.SequenceId);
							if(odc.ID == Cmd.CommandID.RING_EXTENSION)
							{
								// Have appropriate line.
								if(altiResponse.ResponseCode != 0) eventsQueue.Enqueue(new Ev.CommandStatus(odc.Line, odc.Sender, Cmd.CommandID.RING_EXTENSION, Ev.CmdStatus.ERROR));
								else eventsQueue.Enqueue(new Ev.CommandStatus(odc.Line, odc.Sender, Cmd.CommandID.RING_EXTENSION, Ev.CmdStatus.OK));
							}
						}
					}
					break;
				}
					// Snatch line.
				case ALPCmdID.APC_SNATCH_LINE:
				{
					if(altiResponse.LocationId == locationID)
					{
						foreach(Line _line in lines.Values)
						{
							// Searching for a line which sends a command to snatch line.
							if((_line.LastCommand.ID == ALPCmdID.APC_SNATCH_LINE)&&
								(_line.LastCommand.SequenceID == altiResponse.SequenceId))
							{
								// Have appropriate line.
								int lineConnectedToThis = altiResponse[0].ReadInt32();
								if(lines.ContainsKey(lineConnectedToThis)) eventsQueue.Enqueue(new Ev.CommandStatus(_line.Info.ID, _line.LastCommand.SenderLine, Cmd.CommandID.SNATCH_LINE, Ev.CmdStatus.OK));
								else eventsQueue.Enqueue(new Ev.CommandStatus(_line.Info.ID, _line.LastCommand.SenderLine, Cmd.CommandID.SNATCH_LINE, Ev.CmdStatus.ERROR));
								break;
							}
						}
					}
					break;
				}
					// Switch music.
				case ALPCmdID.APC_SWITCH_MUSIC:
				{
					if(altiResponse.LocationId == locationID)
					{
						// Searching for a line command to switch music was sent on.
						foreach(Line _line in lines.Values)
						{
							if((_line.LastCommand.ID == ALPCmdID.APC_SWITCH_MUSIC)&&
								(_line.LastCommand.SequenceID == altiResponse.SequenceId))
							{
								// Have one.
								if(altiResponse.ResponseCode == 0) eventsQueue.Enqueue(new Ev.CommandStatus(_line.Info.ID, _line.LastCommand.SenderLine, Cmd.CommandID.SWITCH_MUSIC, Ev.CmdStatus.OK));
								else eventsQueue.Enqueue(new Ev.CommandStatus(_line.Info.ID, _line.LastCommand.SenderLine, Cmd.CommandID.SWITCH_MUSIC, Ev.CmdStatus.ERROR));
								break;
							}
						}
					}
					break;
				}
					// Transfer call.
				case ALPCmdID.APC_TRANSFER_CALL:
				{
					foreach(Line _line in lines.Values)
					{
						// Searching for a line command to transfer call was sent on.
						if(_line.LastCommand.SequenceID == altiResponse.SequenceId)
						{
							// Have one.
							if(altiResponse.ResponseCode == 0)
							{
								// Not all is bad (not all is good).
								if(_line.LastCommand.Status == ALPCmdStatus.FAILED) eventsQueue.Enqueue(new Ev.CommandStatus(_line.LastCommand.SenderLine, _line.Info.ID, Cmd.CommandID.TRANSFER, Ev.CmdStatus.ERROR));
								else eventsQueue.Enqueue(new Ev.CommandStatus(_line.Info.ID, _line.LastCommand.SenderLine, Cmd.CommandID.TRANSFER, Ev.CmdStatus.OK));
							}
							else  eventsQueue.Enqueue(new Ev.CommandStatus(_line.Info.ID, _line.LastCommand.SenderLine, Cmd.CommandID.TRANSFER, Ev.CmdStatus.ERROR));
						}
					}
					break;
				}
				default:
				{
					// Debug stuff.
					TraceOut.Put(altiResponse);
					break;
				}
			}
		}
		#endregion

		#region -------------------------------------------- Commands and events/responces processing threads ---
		/// <summary>
		/// Thread which extracts commands from commandsQueue queue and sends them to SP.
		/// </summary>
		private void outThread()
		{
			//An object to be sent (command).
			AltiLinkPlus.ALPCommand cmd = null;
			// While outThreadLivingStatus == true we should dequeue commands from outgoing queue and send them.
			while(outThreadLivingStatus)
			{
				try
				{
					// Getting next command and processing it.
					cmd = ProcessCommand(commandsQueue.Dequeue());
					// Storing command.
					StreamStorage.Put(DateTime.Now, (AltiLinkPlus.ALPDataBlock)(cmd));
					// If not null then command is valid - sending it.
					if(cmd != null)
					{
						// Creating new packet.
						AltiLinkPlus.ALPPacket outThreadPacket = new AltiLinkPlus.ALPPacket();
						// Putting command into a packet.
						outThreadPacket.Add(cmd);
						// Sending out the packet.
						outThreadPacket.Write(bw);
					}
				}
					// Parameters are invalid (most cases) - so what? Not last parameters... 
				catch(ArgumentNullException x)
				{
					TraceOut.Put(x);
					RaiseEvent(SPStatus.ERROR_INVALID_PARAMETERS, String.Format("{1}Sending command - have problems.{1}Raised exception: {0}", x.Message, Environment.NewLine));
				}
				catch(InvalidCastException x)
				{
					TraceOut.Put(x);
					RaiseEvent(SPStatus.ERROR_INVALID_PARAMETERS, String.Format("{1}Sending command - have problems.{1}Raised exception: {0}", x.Message, Environment.NewLine));
				}
					// Not enough memory - so what? Soon will be more memory, possible...
				catch(OutOfMemoryException x)
				{
					TraceOut.Put(x);
					RaiseEvent(SPStatus.ERROR_CREATING_COMPONENTS, String.Format("{1}Sending command - have problems.{1}Raised exception: {0}", x.Message, Environment.NewLine));
				}
					// Socket problems - reason to finish the thread.
				catch(SocketException x)
				{
					TraceOut.Put(x);
					RaiseEvent(SPStatus.ERROR_CONNECTION, String.Format("{1}Sending command - have problems.{1}Raised exception: {0}", x.Message, Environment.NewLine));
					return;
				}
				catch(System.IO.IOException x)
				{
					TraceOut.Put(x);
					RaiseEvent(SPStatus.ERROR_CONNECTION, String.Format("{1}Sending command - have problems.{1}Raised exception: {0}", x.Message, Environment.NewLine));
					return;
				}
					// Object was disposed - so what? We'll create new...
				catch(ObjectDisposedException x)
				{
					TraceOut.Put(x);
					RaiseEvent(SPStatus.ERROR_OBJECT_DISPOSED, String.Format("{1}Sending command - have problems.{1}Raised exception: {0}", x.Message, Environment.NewLine));
				}
					// Operation is invalid - so what? Not last operation...
				catch(InvalidOperationException x)
				{
					TraceOut.Put(x);
					RaiseEvent(SPStatus.ERROR_INVALID_OPERATION, String.Format("{1}Sending command - have problems.{1}Raised exception: {0}", x.Message, Environment.NewLine));
				}
					// Exception - so what?
				catch(Exception x)
				{
					TraceOut.Put(x);
					RaiseEvent(SPStatus.ERROR_UNHANDLED_EXCEPTION, String.Format("{1}Sending command - have problems.{1}Raised exception: {0}", x.Message, Environment.NewLine));
				}
			}
			return;
		}

		/// <summary>
		/// Thread which receives packets from AltiGen Server and 
		/// calls the appropriate procedures - to process event or response.
		/// </summary>
		private void inThread()
		{
			AltiLinkPlus.ALPPacket inThreadPacket = null;
			try
			{
				inThreadPacket = new AltiLinkPlus.ALPPacket();
			}
			catch(OutOfMemoryException e)
			{
				TraceOut.Put(e);
				spStatus = SPStatus.ERROR_CREATING_COMPONENTS;
			}
			// While inThreadLivingStatus == true we should receive incoming packets.
			while(inThreadLivingStatus)
			{
				try
				{
					// Reding next packet.
					inThreadPacket.Read(br);
					// Loop throw every block of data in the received packet.
					for(int j = 0; j < inThreadPacket.BlockCount; j++)
					{
						// Getting block.
						AltiLinkPlus.ALPDataBlock dataBlock = inThreadPacket.PacketData[j];
						// Storing data.
						StreamStorage.Put(DateTime.Now, dataBlock);
						// Processing data.
						if(dataBlock is AltiLinkPlus.ALPEvent)
						{
							// Process received event.
							ProcessEvent((AltiLinkPlus.ALPEvent)dataBlock);
						}
						else if(dataBlock is AltiLinkPlus.ALPResponse)
						{
							// Process received response.
							ProcessResponse((AltiLinkPlus.ALPResponse)dataBlock);
						}
					}
				}
					// Parameters are invalid (most cases) - so what? Not last parameters... 
				catch(ArgumentNullException x)
				{
					TraceOut.Put(x);
					RaiseEvent(SPStatus.ERROR_INVALID_PARAMETERS, String.Format("{1}Receiving event or response - have problems.{1}Raised exception: {0}", x.Message, Environment.NewLine));
				}
				catch(InvalidCastException x)
				{
					TraceOut.Put(x);
					RaiseEvent(SPStatus.ERROR_INVALID_PARAMETERS, String.Format("{1}Receiving event or response - have problems.{1}Raised exception: {0}", x.Message, Environment.NewLine));
				}
					// Not enough memory - so what? Soon will be more memory, possible...
				catch(OutOfMemoryException x)
				{
					TraceOut.Put(x);
					RaiseEvent(SPStatus.ERROR_CREATING_COMPONENTS, String.Format("{1}Receiving event or response - have problems.{1}Raised exception: {0}", x.Message, Environment.NewLine));
				}
					// Socket problems - reason to finish the thread.
				catch(SocketException x)
				{
					TraceOut.Put(x);
					RaiseEvent(SPStatus.ERROR_CONNECTION, String.Format("{1}Receiving event or response - have problems.{1}Raised exception: {0}", x.Message, Environment.NewLine));
					return;
				}
				catch(System.IO.IOException x)
				{
					TraceOut.Put(x);
					RaiseEvent(SPStatus.ERROR_CONNECTION, String.Format("{1}Receiving event or response - have problems.{1}Raised exception: {0}", x.Message, Environment.NewLine));
					return;
				}
					// Object was disposed - so what? We'll create new...
				catch(ObjectDisposedException x)
				{
					TraceOut.Put(x);
					RaiseEvent(SPStatus.ERROR_OBJECT_DISPOSED, String.Format("{1}Receiving event or response - have problems.{1}Raised exception: {0}", x.Message, Environment.NewLine));
				}
					// Operation is invalid - so what? Not last operation...
				catch(InvalidOperationException x)
				{
					TraceOut.Put(x);
					RaiseEvent(SPStatus.ERROR_INVALID_OPERATION, String.Format("{1}Receiving event or response - have problems.{1}Raised exception: {0}", x.Message, Environment.NewLine));
				}
					// Exception - so what?
				catch(Exception x)
				{
					TraceOut.Put(x);
					RaiseEvent(SPStatus.ERROR_UNHANDLED_EXCEPTION, String.Format("{1}Receiving event or response - have problems.{1}Raised exception: {0}", x.Message, Environment.NewLine));
				}
			}
			return;
		}
		#endregion

		#region --------------------------------------------------------------------------------------- Other ---
		/// <summary>
		/// Represents information about a class.
		/// </summary>
		/// <returns>Information about a class.</returns>
		public override string ToString()
		{
			return "AltiGen Service Provider";
		}
		#endregion
	} // end of ASPHdw class.
} // end of AltiGen namespace.
