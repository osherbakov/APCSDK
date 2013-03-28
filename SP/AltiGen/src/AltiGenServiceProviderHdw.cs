using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Net.Sockets;
using System.Collections;
using System.Collections.Generic;
using AltiComLib;

namespace Diacom.AltiGen
{
	/// <summary>
	/// Implements connection to AltiGen servers.
	/// </summary>
	internal class AltiGenSPCore : IDisposable
	{
#region "Private Fields"
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
		private WaitingQueue<Diacom.Ev.EvBase> eventsQueue = null;

		/// <summary>
		/// Thread handle for incoming messages.
		/// </summary>
		private Thread inThreadHandle = null;

		/// <summary>
		/// Flag for thread for incoming messages to end his job.
		/// </summary>
		private bool inThreadLivingStatus = false;

		/// <summary>
		/// Queue for commands (outgoing).
		/// </summary>
		private WaitingQueue<Diacom.Cmd.CmdBase> commandsQueue = null;

		/// <summary>
		/// Thread handle for outcoming messages.
		/// </summary>
		private Thread outThreadHandle = null;

        /// <summary>
        /// Flag for thread for outcoming messages to end his job.
        /// </summary>
        private bool outThreadLivingStatus = false;

        /// <summary>
        /// Queue for ALP commands (outgoing).
        /// </summary>
        private WaitingQueue<AltiGen.AltiLinkPlus.ALPCommand> commandsALPQueue = null;

        /// <summary>
        /// Thread handle for outcoming ALP Commands.
        /// </summary>
        private Thread outALPThreadHandle = null;

        /// <summary>
        /// Flag for thread for outcoming messages to end his job.
        /// </summary>
        private bool outALPThreadLivingStatus = false;

		/// <summary>
		/// SP account ID (control line PAD).
		/// </summary>
		protected int ControlLocationID = 0;

		/// <summary>
		/// Hashtable of all lines in system.
		/// </summary>
		private Dictionary<int, ALPLine> lines = null;

		/// <summary>
		/// Hashtable of all commands that are waiting.
		/// </summary>
		private Dictionary<int, WaitingCommandInfo> waitingCommands = null;

		/// <summary>
		/// Status of SP.
		/// </summary>
		protected SPStatus spStatus = SPStatus.DISCONNECTED;

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

		/// <summary>
		/// Version of AltiLink Plus protocol.
		/// </summary>
		private int VersionID = 0;

		private SPLogonType logonType = SPLogonType.ADMINISTRATOR;
		#endregion

		/// <summary>
		/// Raises event with information about IAltiGenSP status.
		/// </summary>
		protected void RaiseEvent(SPStatus _status, string _info)
		{
			SPStatusEventArgs e = new SPStatusEventArgs(_status, _info);
			spStatus = _status;
			SPStatusEventHandler eh = this.SPStatusEvent;
			if(eh != null)	eh(this, e);
		}

		/// <summary>
		/// Initializes main SP components.
		/// </summary>
		public AltiGenSPCore()
		{
			// Creating Status object;
			this.spStatus = SPStatus.DISCONNECTED;
			// Creating new TCPClient to communicate using AltiLink Plus.
			this.tcpc = new TcpClient();
			// Creating 3 queues for commands, events & responses.
			this.commandsQueue = new WaitingQueue<Diacom.Cmd.CmdBase>();
            this.commandsALPQueue = new WaitingQueue<AltiLinkPlus.ALPCommand>();
            this.eventsQueue = new WaitingQueue<Diacom.Ev.EvBase>();

            this.waitingCommands = new Hashtable();
            
            // Thread to get messages from SP.
			this.inThreadHandle = new Thread(new ThreadStart(inThread));
			this.inThreadHandle.IsBackground = inThreadLivingStatus = true;
			this.inThreadHandle.Name = "AltiGenSP: IN";
			// Thread to send messages to SP.
			this.outThreadHandle = new Thread(new ThreadStart(outThread));
			this.outThreadHandle.IsBackground = outThreadLivingStatus = true;
			this.outThreadHandle.Name = "AltiGenSP: OUT";
            // Thread to send ALP messages.
            this.outALPThreadHandle = new Thread(new ThreadStart(outALPThread));
            this.outALPThreadHandle.IsBackground = outALPThreadLivingStatus = true;
            this.outALPThreadHandle.Name = "AltiGenSP: ALP OUT";
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
		/// </remarks>
		public void Connect(string serverIP, int serverPort, SPLogonType logonType, string account, string password, int timeout)
		{
			try
			{
				// Connect to the AltiWare server.
				this.tcpc.Connect(serverIP, serverPort);
				this.ConnectionIsAlive = true;
				// Assign network stream.
				this.st = tcpc.GetStream();
				// To Binaryreader.
				this.bw = new BinaryWriter(st);
				// To BinaryWriter.
				this.br = new BinaryReader(st);

				this.lines = new Hashtable();

				// Starting threads.
                this.outALPThreadHandle.Start();
                this.outThreadHandle.Start();
                this.inThreadHandle.Start();

				if ( timeout == 0 )
					timeout = System.Threading.Timeout.Infinite;
				else if( timeout < 1000 )
					timeout *= 1000;

				// Command to Register Application.
				AltiLinkPlus.ALPCommand ac = new AltiLinkPlus.ALPCommand(0, Convert.ToInt32(ALPCmdID.REGISTER_APPID));
				// AltiGen SDK application.
				ac[0] = new AltiLinkPlus.ALPParameter("GATORS11");
				this.SendALPCommandAndWait(ac, timeout);

				// Command to Logon
				ac = new AltiLinkPlus.ALPCommand(0, Convert.ToInt32(ALPCmdID.LOGON));
				ac[0] = new AltiLinkPlus.ALPParameter((int)logonType);
				ac[1] = new AltiLinkPlus.ALPParameter(account);
				ac[2] = new AltiLinkPlus.ALPParameter(password);
				ac[3] = new AltiLinkPlus.ALPParameter(0);
				ac[4] = new AltiLinkPlus.ALPParameter(0);
				ac[5] = new AltiLinkPlus.ALPParameter(0);
				ac[6] = new AltiLinkPlus.ALPParameter(0);
				this.SendALPCommandAndWait(ac, timeout);

				// Get version
				this.SendALPCommandAndWait(new AltiLinkPlus.ALPCommand(0, Convert.ToInt32(ALPCmdID.GET_VERSION)), timeout);

				// Issue command to Get Lines Information.
				this.SendALPCommandAndWait(new Diacom.AltiGen.AltiLinkPlus.ALPCommand(0, Convert.ToInt32(ALPCmdID.GET_LINEINFO)), timeout);

				// Have a bad response on REGISTER_APPID, LOGON or GET_LINES command.
				if((lines == null)  || (ControlLocationID == 0) )
				{
					TraceOut.Put("Logon unsuccessful");
					RaiseEvent(SPStatus.ERROR_LOGON, "Logon Error");
					return;
				}
				this.ConnectionChecker = new System.Threading.Timer(new System.Threading.TimerCallback(this.CheckConnection), null, 30000, 30000);
				spStatus = SPStatus.OK;
				RaiseEvent(SPStatus.OK, "Logon Success");
			}
			catch(Exception _x)
			{
				TraceOut.Put("Connect Exception");
				TraceOut.Put("_x.Message");
				RaiseEvent(SPStatus.ERROR_LOGON, _x.Message);
			}
		}

		/// <summary>
		/// Destructor.
		/// </summary>
		~AltiGenSPCore()
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
				RaiseEvent(SPStatus.ERROR_CONNECTION, "Connection Timer expired");
			}
			this.ConnectionIsAlive = false;
			AltiLinkPlus.ALPCommand ac = new AltiLinkPlus.ALPCommand(this.ControlLocationID, Convert.ToInt32(ALPCmdID.PING));
			ac[0] = new AltiLinkPlus.ALPParameter(1);
			this.SendALPCommand(ac);
			TraceOut.Put("[Timer] Checking connection("+this.ConnectionIsAlive.ToString()+")...");
		}

		/// <summary>
		/// Implements <see cref="System.IDisposable.Dispose"/> method of 
		/// <see cref="System.IDisposable"/> interface.
		/// </summary>
		/// <remarks>
		/// <para>Stops threads, closes socket, etc.</para>
		/// <seealso cref="System.IDisposable"/>
		///</remarks>
		public void Dispose(bool disposing)
		{
			if(isDisposed) return;
			lock(this)
			{
				isDisposed = true;
				GC.SuppressFinalize(this);
				if(disposing)
				{
					if (this.ConnectionChecker != null) this.ConnectionChecker.Dispose();
					// Command to Logoff from SP.
					AltiLinkPlus.ALPCommand ac = new AltiLinkPlus.ALPCommand(0, Convert.ToInt32(ALPCmdID.LOGOFF));
					this.SendALPCommand(ac);
					this.spStatus = SPStatus.DISCONNECTED;
					System.Threading.Thread.Sleep(300);
					// Setting flags for threads to finish the work.
					this.inThreadLivingStatus = false;
					this.outThreadLivingStatus = false;
                    this.outALPThreadLivingStatus = false;
                    this.commandsALPQueue.Enqueue(null);
                    this.commandsQueue.Enqueue(null);
					this.eventsQueue.Enqueue(null);
					this.tcpc.Close();
				}
			}
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
			Dispose(true);
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
			return this.eventsQueue.Dequeue();
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
			this.commandsQueue.Enqueue(cmd);
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
			SPLine [] _result;
			if (lines == null) return null;
			lock(lines)
			{
				_result = new SPLine[lines.Count];
				int _index = 0;
				foreach(ALPLine line in lines.Values)
				{
					// Fill the resulting array.
					_result[_index++] = (SPLine)line.SPLineInfo.Clone();
				}
			}
			Array.Reverse(_result);
			return _result;
		}

		/// <summary>
		/// Provides an access to specified line.
		/// </summary>
		/// <param name="lineID">Line ID.</param>
		/// <returns><see cref="Diacom.SPLine"/> instance if line exists, null otherwise.</returns>
		public SPLine GetLine(object lineID)
		{
			if (lines == null) return null;
			lock(lines)
			{
				return (SPLine)lines[lineID].SPLineInfo.Clone();
			}
		}

		public object GetLineId(string lineName)
		{
			if (lines == null) return null;
			lock(lines)
			{
				foreach (ALPLine _line in lines.Values)
				{
					if (_line.SPLineInfo.Name == lineName)
						return _line.SPLineInfo.ID;
				}
				return null;
			}
		}

		protected ALPLine this[object lineID]
		{
			get 
			{
				if (lines == null) return null;
				lock(lines)
				{
					return lines[lineID];
				}
			}
			set 
			{ 
				if (lines == null) return;
				lock(lines)
				{
					lines[lineID] = value;
				}
			}
		}

		protected ALPLine GetLineBySequenceId(int sequenceID)
		{
			if (lines == null) return null;
			lock(lines)
			{
				foreach (ALPLine _line in lines.Values)
				{
					if (_line.LastCommand.SequenceID == sequenceID)
						return _line;
				}
			}
            return null;
        }
		
		/// <summary>
		/// Converts object into AltiLink Plus v2.0 format and returns it.
		/// </summary>
		/// <param name="cmd">Command to convert (of "object" type).</param>
		/// <returns>Command in AltiLink Plus v2.0 format (or null).</returns>
		protected virtual AltiLinkPlus.ALPCommand ProcessCommand(Cmd.CmdBase cmd)
		{			
			return null;
		}

		/// <summary>
		/// Processes all events.
		/// </summary>
		/// <param name="altiEvent">Block of data to process.</param>
		/// <returns>An object with the event.</returns>
		protected virtual void ProcessEvent(AltiLinkPlus.ALPEvent altiEvent)
		{
		}

		/// <summary>
		/// Process all responses.
		/// </summary>
		/// <param name="altiResponse">Block of data to process.</param>
		/// <returns>An object with the response.</returns>
		protected virtual void ProcessResponse(AltiLinkPlus.ALPResponse altiResponse)
		{
			TraceOut.Put("AltiGenSPCore::ProcessResponse SeqId=" + altiResponse.SequenceId + " LocId=" + altiResponse.LocationId + " CmdId=" + ((ALPCmdID)altiResponse.CommandId).ToString() + " code=" + ((ALPRespID)altiResponse.ResponseCode).ToString());
			WaitingCommandInfo wci;
			lock(this.waitingCommands)
			{
				wci = this.waitingCommands[altiResponse.SequenceId];
			}
			switch((ALPCmdID)(altiResponse.CommandId))
			{
				case ALPCmdID.REGISTER_APPID:
					if(wci != null)
					{
						wci.respCode = altiResponse.ResponseCode;
						wci.errorOccured = (altiResponse.ResponseCode == 0) ? false : true;
						wci.cmdReady.Set();
					}
					break;

				// Logon.
				case ALPCmdID.LOGON:
				{
					if(altiResponse.ResponseCode == 0)
					{
						TraceOut.Put("Logon saccessfull...");
						// Getting our control line PAD.
						if(this.logonType == SPLogonType.ADMINISTRATOR)
						{
							this.ControlLocationID = altiResponse.LocationId;
						}
					}
					if(wci != null)
					{
						wci.respCode = altiResponse.ResponseCode;
						wci.errorOccured = (altiResponse.ResponseCode == 0) ? false : true;
						wci.cmdReady.Set();
					}
					break;
				}

				case ALPCmdID.GET_VERSION:
					if(altiResponse.ResponseCode == 0)
					{
						this.VersionID = altiResponse[0].ReadInt32();
						TraceOut.Put("GetVersion successful. Version = " +  VersionID.ToString("X"));
						bool enableOptionPack = Convert.ToBoolean(altiResponse[1].ReadByte());
					}
					if(wci != null)
					{
						wci.errorOccured = (altiResponse.ResponseCode == 0) ? false : true;
						wci.respCode = altiResponse.ResponseCode;
						wci.cmdReady.Set();
					}
					break;

				// Get lines info.
				case ALPCmdID.GET_LINEINFO:
					if(altiResponse.ResponseCode == 0)
					{
						// First parameter in response is number of entries.
						int	linesCount = altiResponse[0].ReadInt32();
						// Second parameter holds lines information.
						AltiLinkPlus.ALPParameter param = altiResponse[1];
						// Filling lines hashtable.
						for(int i = 0; i < linesCount; i++)
						{
							LineInfo _li = new LineInfo(param, this.VersionID);
							this[_li.lineHandle] = new ALPLine(_li);
							// If there is no command waiting - send the event that new line was added
							if(wci == null)
							{
								this.SendSpEvent(new Ev.LineStateChanged(_li.lineHandle, SPLineState.LINE_ADD));
							}
						}
						TraceOut.Put(linesCount.ToString()+" lines added...");
					}
					else if(altiResponse.ResponseCode == (int)Diacom.AltiGen.ALPRespID.LIST_END)
					{	// We received "No more Data" response
						TraceOut.Put("All lines received.");
						if(wci != null)
						{
							wci.errorOccured = false;
							wci.cmdReady.Set();
						}
					}
					else
					{
						TraceOut.Put("Failed to get lines info : "+((ALPRespID)(altiResponse.ResponseCode)).ToString());
						if(wci != null)
						{
							wci.errorOccured = true;
							wci.respCode = altiResponse.ResponseCode;
							wci.cmdReady.Set();
						}
					}
					break;

				case ALPCmdID.PING:
					TraceOut.Put("PING response received.");
					this.ConnectionIsAlive = true;
					break;
			}
		}

		/// <summary>
		/// Thread which extracts commands from commandsQueue queue and sends them to SP.
		/// </summary>
		private void outThread()
		{
			//An object to be sent (command).
			AltiLinkPlus.ALPCommand cmd = null;
			Diacom.Cmd.CmdBase spCommand = null;
			// While outThreadLivingStatus == true we should dequeue commands from outgoing queue and send them.
			while(outThreadLivingStatus)
			{
				try
				{
					// Getting next command and processing it.
					object obj = commandsQueue.Dequeue();
					// First try as a raw ALP command
					cmd = obj as AltiLinkPlus.ALPCommand;
					// If not ALP command - convert to 
					if(cmd == null)
					{
						spCommand = obj as Diacom.Cmd.CmdBase;
						cmd = ProcessCommand(spCommand);					
					}
					// If not null then command is valid - sending it.
					if(cmd != null)
					{
						// Creating new packet.
						AltiLinkPlus.ALPPacket outThreadPacket = new AltiLinkPlus.ALPPacket();
						// Putting command into a packet.
						outThreadPacket.Add(cmd);
						TraceOut.Put("AltiGenSPCore::ProcessCommand LocId=" + cmd.LocationId + " CmdId=" + ((ALPCmdID)cmd.CommandId).ToString() + " SeqId=" + cmd.SequenceId);
						// Sending out the packet.
						outThreadPacket.Write(bw);
					}
				}
				catch(Exception x)
				{
					if(outThreadLivingStatus)
					{
						TraceOut.Put(x);
						RaiseEvent(SPStatus.ERROR_CONNECTION, String.Format("Sending command: {0}", x.Message));
					}
				}
			}
		}

		protected  void SendALPCommand(AltiLinkPlus.ALPCommand cmd)
		{
			commandsALPQueue.Enqueue(cmd);
		}

		private class WaitingCommandInfo
		{
			public ManualResetEvent cmdReady = new ManualResetEvent(false);
			public bool errorOccured = false;
			public int respCode = 0;
		}

		protected  void SendALPCommandAndWait(AltiLinkPlus.ALPCommand cmd, int waitTimeInMs)
		{
			commandsALPQueue.Enqueue(cmd);
			WaitingCommandInfo wci = new WaitingCommandInfo();
			lock(waitingCommands)
			{
				waitingCommands.Add(cmd.SequenceId, wci);
			}
			bool signalled = wci.cmdReady.WaitOne(waitTimeInMs , false);
			wci.cmdReady.Close();
			lock(waitingCommands)
			{
				waitingCommands.Remove(cmd.SequenceId);
			}
			if( wci.errorOccured)
			{
				throw new System.Exception(((Diacom.AltiGen.ALPRespID)wci.respCode).ToString());
			}
			if( !signalled)
			{
				throw new System.Exception("Response timeout expired");
			}
		}

		protected  void SendSpEvent( Diacom.Ev.EvBase ev)
		{
			eventsQueue.Enqueue(ev);
		}

		/// <summary>
		/// Thread which receives packets from AltiGen Server and 
		/// calls the appropriate procedures - to process event or response.
		/// </summary>
		private void inThread()
		{
			AltiLinkPlus.ALPPacket inThreadPacket =  new AltiLinkPlus.ALPPacket();
			// While inThreadLivingStatus == true we should receive incoming packets.
			while(inThreadLivingStatus)
			{
				try
				{
					// Reding next packet.
					inThreadPacket.Read(br);
					// Loop through every block of data in the received packet.
					for(int j = 0; j < inThreadPacket.BlockCount; j++)
					{
						// Getting block.
						AltiLinkPlus.ALPDataBlock dataBlock = inThreadPacket.PacketData[j];
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
				catch(Exception x)
				{
					if(inThreadLivingStatus)
					{
						TraceOut.Put(x);
						RaiseEvent(SPStatus.ERROR_CONNECTION, x.Message);
					}
				}
			}
		}

		/// <summary>
		/// Represents information about a class.
		/// </summary>
		/// <returns>Information about a class.</returns>
		public override string ToString()
		{
			return "AltiGen Service Provider";
		}
	}
}
