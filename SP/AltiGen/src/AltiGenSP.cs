using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Net.Sockets;
using System.Collections;

namespace Diacom.AltiGen
{
	/// <summary>
	/// Implements <see cref="Diacom.ISP"/> interface for AltiGen servers.
	/// </summary>
	internal class AltiGenServiceProvider : AltiGenSPCore
	{
		private const string USER_DATA = "UserData";

		/// <summary>
		/// Hashtable for storing information on commands DIAL.
		/// </summary>
		private Hashtable outcallNumbersDialingCommands = new Hashtable();

		/// <summary>
		/// Hashtable for storing information on incoming Ring.
		/// </summary>
		private Hashtable StoredRingInfo = new Hashtable();

		/// <summary>
		/// Converts object into AltiLink Plus v2.0 format and returns it.
		/// </summary>
		/// <param name="obj">Command to convert (of "object" type).</param>
		/// <returns>Command in AltiLink Plus v2.0 format (or null).</returns>
		protected override AltiLinkPlus.ALPCommand ProcessCommand(Cmd.CmdBase obj)
		{
			AltiLinkPlus.ALPCommand cmd = null;
			ALPLine _line = null;

			if(obj == null ) return null;
			
			_line = this[obj.Line];
			// Check if the line exists.
			if(_line == null)
			{
				SendSpEvent(new Ev.CommandStatus(obj.Line, obj.ID, Ev.CmdStatus.ERROR));
				return null;
			}

			// Answer the call.
			if(obj is Cmd.AnswerCall)
			{
				Cmd.AnswerCall _cmd = ((Cmd.AnswerCall)(obj));
				// Creating command.
				cmd = new AltiLinkPlus.ALPCommand(ControlLocationID, (Convert.ToInt32(ALPCmdID.APC_ANSWER_CALL)));
				cmd[0] = new AltiLinkPlus.ALPParameter(Convert.ToInt32(_cmd.Line));
				cmd[1] = new AltiLinkPlus.ALPParameter(0);
				// Saving information about the command in line's Line class.
				_line.LastCommand.Set(cmd.SequenceId, ALPCmdID.APC_ANSWER_CALL, _cmd.Sender);
				return cmd;
			} 

			// Reject the call.
			if(obj is Cmd.RejectCall)
			{
				Cmd.RejectCall _cmd = ((Cmd.RejectCall)(obj));
				// Creating command.
				cmd = new AltiLinkPlus.ALPCommand(ControlLocationID, (Convert.ToInt32(ALPCmdID.APC_REJECT_CALL)));
				cmd[0] = new AltiLinkPlus.ALPParameter(Convert.ToInt32(_cmd.Line));
				cmd[1] = new AltiLinkPlus.ALPParameter(_cmd.rejectReasonCode);
				cmd[2] = new AltiLinkPlus.ALPParameter(0);
				// Saving information about the command in line's Line class.
				_line.LastCommand.Set(cmd.SequenceId, ALPCmdID.APC_REJECT_CALL, _cmd.Sender);
				return cmd;
			} 

			// Pass the call.
			if(obj is Cmd.PassCall)
			{
				Cmd.PassCall _cmd = ((Cmd.PassCall)(obj));
				// Creating command.
				cmd = new AltiLinkPlus.ALPCommand(ControlLocationID, (Convert.ToInt32(ALPCmdID.APC_PASS_CALL)));
				cmd[0] = new AltiLinkPlus.ALPParameter(Convert.ToInt32(_cmd.Line));
				cmd[1] = new AltiLinkPlus.ALPParameter(0);
				// Saving information about the command in line's Line class.
				_line.LastCommand.Set(cmd.SequenceId, ALPCmdID.APC_PASS_CALL, _cmd.Sender);
				return cmd;
			} 

			// Connect lines command.
			if(obj is Cmd.Connect)
			{
				Cmd.Connect _cmd = ((Cmd.Connect)obj);
				ALPLine _line1 = this[_cmd.LineOne];
				ALPLine _line2 = this[_cmd.LineTwo];
				// Creating command.
				cmd = new AltiLinkPlus.ALPCommand(ControlLocationID, (Convert.ToInt32(ALPCmdID.APC_CONNECT_CALL)));
				cmd[0] = new AltiLinkPlus.ALPParameter(Convert.ToInt32(_cmd.LineOne));
				cmd[1] = new AltiLinkPlus.ALPParameter(Convert.ToInt32(_cmd.LineTwo));
				cmd[2] = new AltiLinkPlus.ALPParameter(0);
				// Saving information about the command in line's Line class.
				_line1.LastCommand.Set(cmd.SequenceId, ALPCmdID.APC_CONNECT_CALL, _cmd.Sender);
				return cmd;
			} 

			// Disconnect.
			if(obj is Cmd.Disconnect)
			{
				Cmd.Disconnect _cmd = ((Cmd.Disconnect)obj);
				// Creating command.
				cmd = new AltiLinkPlus.ALPCommand(ControlLocationID, (Convert.ToInt32(ALPCmdID.APC_SNATCH_LINE)));
				cmd[0] = new AltiLinkPlus.ALPParameter(Convert.ToInt32(_cmd.Line));
				cmd[1] = new AltiLinkPlus.ALPParameter(0);
				// Saving information about the command in line's Line class.
				_line.LastCommand.Set(cmd.SequenceId, ALPCmdID.APC_SNATCH_LINE, _cmd.Sender);
				return cmd;
			} 

			// Dial the number.
			if(obj is Cmd.Dial)
			{
				Cmd.Dial _cmd = ((Cmd.Dial)obj);
				string _numberToDial = _line.SPLineInfo.AccessCode +  _cmd.Destination;
				// Creating command.
				cmd = new AltiLinkPlus.ALPCommand(ControlLocationID, (Convert.ToInt32(ALPCmdID.APC_MAKE_CALL)));
				cmd[0] = new AltiLinkPlus.ALPParameter(_numberToDial);
				cmd[1] = new AltiLinkPlus.ALPParameter(_cmd.Account);
				cmd[2] = new AltiLinkPlus.ALPParameter(_cmd.Tone);
				cmd[3] = new AltiLinkPlus.ALPParameter(_cmd.Source);
				cmd[4] = new AltiLinkPlus.ALPParameter(0);
				// Saving information about the command.
				OutcallCommandStruct odc = new OutcallCommandStruct(_cmd.Sender, _cmd.Line, Cmd.CommandID.DIAL);
				outcallNumbersDialingCommands.Add(cmd.SequenceId, odc);
				_line.LastCommand.Set(cmd.SequenceId, ALPCmdID.APC_MAKE_CALL, _cmd.Sender);
				return cmd;
			} 

			// Drop Call.
			if(obj is Cmd.DropCall)
			{
				Cmd.DropCall _cmd = ((Cmd.DropCall)obj);
				// Creating command.
				cmd = new AltiLinkPlus.ALPCommand(ControlLocationID, (Convert.ToInt32(ALPCmdID.APC_DROP_CALL)));
				cmd[0] = new AltiLinkPlus.ALPParameter(Convert.ToInt32(_cmd.Line));
				cmd[1] = new AltiLinkPlus.ALPParameter(0);
				// Saving information about the command in line's Line class.
				_line.LastCommand.Set(cmd.SequenceId, ALPCmdID.APC_DROP_CALL, _cmd.Sender);
				return cmd;
			} 

			// Play DTMF.
			if(obj is Cmd.PlayDTMF)
			{
				Cmd.PlayDTMF _cmd = ((Cmd.PlayDTMF)obj);
				// Creating command.
				cmd = new AltiLinkPlus.ALPCommand(ControlLocationID, (Convert.ToInt32(ALPCmdID.APC_PLAY_DTMF)));
				cmd[0] = new AltiLinkPlus.ALPParameter(Convert.ToInt32(_cmd.Line));
				cmd[1] = new AltiLinkPlus.ALPParameter(_cmd.DTMFCode);
				cmd[2] = new AltiLinkPlus.ALPParameter(0);
				// Saving information about the command in line's Line class.
				_line.LastCommand.Set(cmd.SequenceId, ALPCmdID.APC_PLAY_DTMF, _cmd.Sender);
				return cmd;
			} 

			// Play file.
			if(obj is Cmd.PlayFile)
			{
				Cmd.PlayFile _cmd = ((Cmd.PlayFile)obj);
				// Creating command.
				cmd = new AltiLinkPlus.ALPCommand(ControlLocationID, (Convert.ToInt32(ALPCmdID.APC_PLAY_VOICE)));
				cmd[0] = new AltiLinkPlus.ALPParameter(Convert.ToInt32(_cmd.Line));
				cmd[1] = new AltiLinkPlus.ALPParameter(_cmd.CutOffString);
				cmd[2] = new AltiLinkPlus.ALPParameter(_cmd.FilePath);
				cmd[3] = new AltiLinkPlus.ALPParameter(0);
				cmd[4] = new AltiLinkPlus.ALPParameter(0);
				// Saving information about the command in line's Line class.
				_line.LastCommand.Set(cmd.SequenceId, ALPCmdID.APC_PLAY_VOICE, _cmd.Sender);
				return cmd;
			} 

			// Record file.
			if(obj is Cmd.RecordFile)
			{
				Cmd.RecordFile _cmd = ((Cmd.RecordFile)obj);
				// Creating command.
				cmd = new AltiLinkPlus.ALPCommand(ControlLocationID, (Convert.ToInt32(ALPCmdID.APC_RECORD_VOICE)));
				// Adding parameters.
				cmd[0] = new AltiLinkPlus.ALPParameter(Convert.ToInt32(_cmd.Line));
				cmd[1] = new AltiLinkPlus.ALPParameter(_cmd.CutOffString);
				cmd[2] = new AltiLinkPlus.ALPParameter(_cmd.FilePath);
				cmd[3] = new AltiLinkPlus.ALPParameter(0);
				cmd[4] = new AltiLinkPlus.ALPParameter(_cmd.AppendMode);
				cmd[5] = new AltiLinkPlus.ALPParameter(0);
				// Saving information about the command in line's Line class.
				_line.LastCommand.Set(cmd.SequenceId, ALPCmdID.APC_RECORD_VOICE, _cmd.Sender);
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
						cmd = new AltiLinkPlus.ALPCommand(ControlLocationID, Convert.ToInt32(ALPCmdID.APC_STOP_PLAY_VOICE));
						cmd[0] = new AltiLinkPlus.ALPParameter(Convert.ToInt32(_cmd.Line));
						// Type (0 - stop play current, 1 - stop play all).
						cmd[1] = new AltiLinkPlus.ALPParameter(1);
						cmd[2] = new AltiLinkPlus.ALPParameter(0);
						break;
					}
					case ALPCmdID.APC_RECORD_VOICE:
					{
						cmd = new AltiLinkPlus.ALPCommand(ControlLocationID, Convert.ToInt32(ALPCmdID.APC_STOP_RECORD_VOICE));
						cmd[0] = new AltiLinkPlus.ALPParameter(Convert.ToInt32(_cmd.Line));
						// Type (0 - stop record current, 1 - stop record all).
						cmd[1] = new AltiLinkPlus.ALPParameter(1);
						cmd[2] = new AltiLinkPlus.ALPParameter(0);
						break;
					}
				}
				return cmd;
			}

			// Ring extension.
			if(obj is Cmd.RingExtension)
			{
				Cmd.RingExtension _cmd = ((Cmd.RingExtension)obj);
				string _numberToRing = _line.SPLineInfo.AccessCode +  _cmd.Extension;
				// Creating command.
				cmd = new AltiLinkPlus.ALPCommand(ControlLocationID, (Convert.ToInt32(ALPCmdID.APC_RING_EXT)));
				// Adding parameters.
				cmd[0] = new AltiLinkPlus.ALPParameter(_cmd.RingType);
				cmd[1] = new AltiLinkPlus.ALPParameter(_numberToRing);
				cmd[2] = new AltiLinkPlus.ALPParameter(0);
				OutcallCommandStruct odc = new OutcallCommandStruct(_cmd.Sender, _cmd.Line, Cmd.CommandID.RING_EXTENSION);
				outcallNumbersDialingCommands.Add(cmd.SequenceId, odc);
				_line.LastCommand.Set(cmd.SequenceId, ALPCmdID.APC_RING_EXT, _cmd.Sender);
				return cmd;
			}

			// Snatch line.
			if(obj is Cmd.Disconnect)
			{
				Cmd.Disconnect _cmd = ((Cmd.Disconnect)obj);
				// Creating command.
				cmd = new AltiLinkPlus.ALPCommand(ControlLocationID, (Convert.ToInt32(ALPCmdID.APC_SNATCH_LINE)));
				// Adding parameters.
				cmd[0] = new AltiLinkPlus.ALPParameter(Convert.ToInt32(_cmd.Line));
				cmd[1] = new AltiLinkPlus.ALPParameter(0);
				// Saving information about the command in line's Line class.
				_line.LastCommand.Set(cmd.SequenceId, ALPCmdID.APC_SNATCH_LINE, _cmd.Sender);
				return cmd;
			}

			// Switch music.
			if(obj is Cmd.SwitchMusic)
			{
				Cmd.SwitchMusic _cmd = ((Cmd.SwitchMusic)obj);
				// Creating command.
				cmd = new AltiLinkPlus.ALPCommand(ControlLocationID, (Convert.ToInt32(ALPCmdID.APC_SWITCH_MUSIC)));
				// Adding parameters.
				cmd[0] = new AltiLinkPlus.ALPParameter(Convert.ToInt32(_cmd.Line));
				cmd[1] = new AltiLinkPlus.ALPParameter(_cmd.MusicMode);
				cmd[2] = new AltiLinkPlus.ALPParameter(0);
				// Saving information about the command in line's Line class.
				_line.LastCommand.Set(cmd.SequenceId, ALPCmdID.APC_SWITCH_MUSIC, _cmd.Sender);
				return cmd;
			}

			// Transfer call.
			if(obj is Cmd.TransferCall)
			{
				const int TRANSFER_CALL_TIMEOUT = 45;
				Cmd.TransferCall _cmd = ((Cmd.TransferCall)obj);
				ALPLine _target = this[_cmd.Target];

				string _numberToRing = _target.SPLineInfo.AccessCode + _cmd.Destination;
				cmd = new AltiLinkPlus.ALPCommand(ControlLocationID, (Convert.ToInt32(ALPCmdID.APC_TRANSFER_CALL)));
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
				_line.LastCommand.Set(cmd.SequenceId, ALPCmdID.APC_TRANSFER_CALL, _cmd.Sender);
				return cmd;
			}
			return null;
		}

		/// <summary>
		/// Processes all events.
		/// </summary>
		/// <param name="altiEvent">Block of data to process.</param>
		/// <returns>An object whith the event.</returns>
		protected override void ProcessEvent(AltiLinkPlus.ALPEvent altiEvent)
		{
			// Getting information only if event is for our SP control line.
			TraceOut.Put("AltiGenSP::ProcessEvent LocId=" + altiEvent.LocationId + " EventId=" + ((ALPEvID)altiEvent.CommandId).ToString());
			if(altiEvent.LocationId == ControlLocationID)
			{
				switch((ALPEvID)(altiEvent.CommandId))
				{
						// Event Ev.Ring.
					case ALPEvID.APC_CALLPRESENT:
					{
						CallInfo rInfo = new CallInfo(((AltiLinkPlus.ALPParameter)(altiEvent[0])));
						ALPLine _line = this[rInfo.lineID];
						if(!_line.IsUnderAPCControl) 
						{
							AltiLinkPlus.ALPCommand ac = new AltiLinkPlus.ALPCommand(ControlLocationID, (int)(ALPCmdID.APC_GET_DATA));
							ac[0] = new AltiLinkPlus.ALPParameter(rInfo.lineID);
							ac[1] = new AltiLinkPlus.ALPParameter((int) Diacom.AltiGen.ALPInfoType.APC_DATATYPE_USER);
							ac[2] = new AltiLinkPlus.ALPParameter(0);
							StoredRingInfo.Add(ac.SequenceId, rInfo);
							base.SendALPCommand(ac);
							_line.InfoState = ALPLine.CallInfoState.CALL_INFO_REQ_SENT;
							_line.Digits.Clear();
							TraceOut.Put("AltiGenSP::APC_CALLPRESENT from :" + rInfo.callerID + " - requested extra info");
						}
						break;
					}
						// Event Ev.RingBack.
					case ALPEvID.APC_RINGBACK:
					{
						CallInfo rbInfo = new CallInfo(((AltiLinkPlus.ALPParameter)(altiEvent[0])));
						// Getting current line from hashtable.
						ALPLine _line = this[rbInfo.lineID];
						if(_line != null)
						{
							// Updating line information.
							if(_line.SPLineInfo.Type == "T")
							{
								_line.SPLineInfo.CalledNumber = _line.SPLineInfo.AccessCode + rbInfo.calleeID;
								_line.SPLineInfo.CalledName = rbInfo.calleeName;
							}
							else
							{
								_line.SPLineInfo.CalledNumber = rbInfo.callerID;
								_line.SPLineInfo.CalledName = rbInfo.callerName;
							}
							_line.SPLineInfo.DNISNumber = rbInfo.DNISID;
							_line.SPLineInfo.DNISName = rbInfo.DNISName;
							_line.SPLineInfo.CIDNumber = rbInfo.ANIID;
							_line.SPLineInfo.CIDName = rbInfo.ANIName;
							_line.SPLineInfo.DIDNumber = rbInfo.calleeID;
							_line.SPLineInfo.DIDName = rbInfo.calleeName;
							TraceOut.Put("AltiGenSP::APC_RINGBACK");

							if(!_line.IsUnderAPCControl) 
							{
								this.SendSpEvent(new Ev.RingBack(rbInfo.lineID, (SPLine)_line.SPLineInfo.Clone()));
							}
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
						ALPLine _line = this[lineID];
						TraceOut.Put("AltiGenSP::Line State=" + ((ALPLineState)state).ToString());

						if((_line != null) && (state == ALPLineState.APC) )
						{
							if( _line.IsUnderAPCControl && 
									(_line.InfoState == ALPLine.CallInfoState.SNATCHED_SLAVE_LINE))
							{
								this.SendSpEvent(new Ev.CommandStatus(_line.ConnectedLine.SPLineInfo.ID, Cmd.CommandID.DISCONNECT_LINE, Diacom.Ev.CmdStatus.OK));
							}else if ( !_line.IsUnderAPCControl )
							{
								if ( _line.InfoState == ALPLine.CallInfoState.CALL_INFO_RECEIVED)
								{
									// Have new connected line. Creating and sending event.
									TraceOut.Put("AltiGenSP:: Line is APC controlled now");
									_line.IsUnderAPCControl = true;
									this.SendSpEvent(new Ev.Connect(lineID));
									foreach( string _digit in _line.Digits)
									{
										this.SendSpEvent(new Ev.Digit(lineID, _digit[0]));
									}
								}
								else
								{
									_line.InfoState = ALPLine.CallInfoState.CONNECT_DELAYED;
								}
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
						ALPLine _line = this[lineID];
						if (_line.IsUnderAPCControl)
							this.SendSpEvent(new Ev.Digit(lineID, digit));
						else
							_line.Digits.Add(digit.ToString());
						break;
					}
					// Event Ev.Disconnect.
					case ALPEvID.APC_CALLDROP:
					{
						// Getting parameters.
						int lineID = altiEvent[0].ReadInt32();
						ALPLine _line = this[lineID];
						break;
					}
						// Event Ev.CommandStatus.
					case ALPEvID.APC_STATUS:
					{
						// Creating new StateInfo object.
						LineStateInfo sInfo = new LineStateInfo((AltiLinkPlus.ALPParameter)(altiEvent[0]));
						ALPCmdStatus lastCmdStatus = (ALPCmdStatus) sInfo.cmdStatus;
						TraceOut.Put("AltiGenSP::APC Status Line=" + sInfo.lineID + " CmdId=" + ((ALPCmdID)sInfo.commandID).ToString()
							+ " Status=" + lastCmdStatus.ToString());

						// Getting current line from hashtable.
						ALPLine _line = this[sInfo.lineID];
						if ( _line == null  ) break;
						
						// Save the last status
						_line.LastCommand.Status = lastCmdStatus; 
						// What was the command issued?
						switch((ALPCmdID)(sInfo.commandID))
						{
							// Status of APC_PLAY_DTMF command.
							case ALPCmdID.APC_PLAY_DTMF:
								if(lastCmdStatus == ALPCmdStatus.FAILED) 
									this.SendSpEvent(new Ev.CommandStatus(sInfo.lineID, Cmd.CommandID.PLAY_DTMF, Ev.CmdStatus.ERROR));
								else 
									this.SendSpEvent(new Ev.CommandStatus(sInfo.lineID, Cmd.CommandID.PLAY_DTMF, Ev.CmdStatus.OK));
								break;

							// Status of APC_PLAY_VOICE command.
							case ALPCmdID.APC_PLAY_VOICE:
								if(lastCmdStatus == ALPCmdStatus.FINISHED) 
									this.SendSpEvent(new Ev.CommandStatus(sInfo.lineID, Cmd.CommandID.PLAY_FILE, Ev.CmdStatus.OK));
								else if(lastCmdStatus == ALPCmdStatus.FAILED) 
									this.SendSpEvent(new Ev.CommandStatus(sInfo.lineID, Cmd.CommandID.PLAY_FILE, Ev.CmdStatus.ERROR));
								break;
							// Status of APC_RECORD_VOICE command.
							case ALPCmdID.APC_RECORD_VOICE:
								if(lastCmdStatus == ALPCmdStatus.FINISHED) 
									this.SendSpEvent(new Ev.CommandStatus(sInfo.lineID, Cmd.CommandID.RECORD_FILE, Ev.CmdStatus.OK));
								else if(lastCmdStatus == ALPCmdStatus.FAILED) 
									this.SendSpEvent(new Ev.CommandStatus(sInfo.lineID, Cmd.CommandID.RECORD_FILE, Ev.CmdStatus.ERROR));
								break;

								// Status of APC_CONNECT_CALL command.
							case ALPCmdID.APC_CONNECT_CALL:
								if(lastCmdStatus == ALPCmdStatus.SUCCEED)
									this.SendSpEvent(new Ev.CommandStatus(sInfo.lineID, Cmd.CommandID.CONNECT_LINES, Ev.CmdStatus.OK));
								else
									this.SendSpEvent(new Ev.CommandStatus(sInfo.lineID, Cmd.CommandID.CONNECT_LINES, Ev.CmdStatus.ERROR));
								break;

							// Status of APC_TRANSFER_CALL command.
							case ALPCmdID.APC_TRANSFER_CALL:
								if(lastCmdStatus != ALPCmdStatus.SUCCEED)
									this.SendSpEvent(new Ev.CommandStatus(sInfo.lineID, Cmd.CommandID.TRANSFER, Ev.CmdStatus.ERROR));
								break;

							// Status of APC_DROP_CALL command.
							case ALPCmdID.APC_DROP_CALL:
								if(lastCmdStatus == ALPCmdStatus.SUCCEED)
									this.SendSpEvent(new Ev.CommandStatus(sInfo.lineID, Cmd.CommandID.DROP_CALL, Ev.CmdStatus.OK));
								else
									this.SendSpEvent(new Ev.CommandStatus(sInfo.lineID, Cmd.CommandID.DROP_CALL, Ev.CmdStatus.ERROR));
								break;
						}
						break;
					}
					default:
					{
						base.ProcessEvent(altiEvent);
						break;
					}
				}
			}
			// Getting information for ALL line PADs - just want to know that is going on.
			switch((ALPEvID)(altiEvent.CommandId))
			{
				case ALPEvID.STCHG:
				{
					if(altiEvent.LocationId == ControlLocationID) 
						break;	// We already processed this above
					// Saving line state.
					ALPLineState state = (ALPLineState)(altiEvent[0].ReadInt32());
					ALPLine _line = this[altiEvent.LocationId];
					TraceOut.Put("AltiGenSP::Line State=" + ((SPLineState)state).ToString());

					if(_line != null)
					{
						// Rasing event the line is disconnected if status is DISCONNECT.
						if( state == ALPLineState.IDLE)
						{
							if(_line.IsUnderAPCControl)
							{
								TraceOut.Put("AltiGenSP:: Line is not APC controlled");
								_line.IsUnderAPCControl = false;
								_line.InfoState = ALPLine.CallInfoState.INITIAL;
								this.SendSpEvent(new Ev.Disconnect(_line.SPLineInfo.ID));
							}
						}
						else if ( (state == ALPLineState.CONNECTED) && (_line.LastCommand.ID == ALPCmdID.APC_TRANSFER_CALL))
						{
							this.SendSpEvent(new Ev.CommandStatus(_line.SPLineInfo.ID, Cmd.CommandID.TRANSFER, Ev.CmdStatus.OK));
						}
						_line.SPLineInfo.State = (SPLineState)state;
						// Rasing event state of the line changed.
						this.SendSpEvent(new Ev.LineStateChanged(_line.SPLineInfo.ID, (SPLineState)state));
					}
					break;
				}
					// Event Ev.Tone.
				case ALPEvID.TONE:
				{
					// Getting parameters.
					AltiLinkPlus.ALPParameter param = (AltiLinkPlus.ALPParameter)altiEvent[0];
					int lineID = param.ReadInt32();
					param = (AltiLinkPlus.ALPParameter)altiEvent[1];
					int tone = Convert.ToChar(param.ReadInt32());
					// Creating and sending event.
					this.SendSpEvent(new Ev.Tone(lineID, ((Ev.ToneType)(tone))));
					break;
				}
					// Line information changed.
				case ALPEvID.CONFIG_CHG:
				{
					const int CONFIG_CHANGED_TYPE_LINEINFO = 0x8000;
					AltiLinkPlus.ALPParameter param = (AltiLinkPlus.ALPParameter)altiEvent[0];
					int type = param.ReadInt32();
					if(type == CONFIG_CHANGED_TYPE_LINEINFO)
					{
						// Command to Get Lines Information.
						this.SendALPCommand(new Diacom.AltiGen.AltiLinkPlus.ALPCommand(altiEvent.LocationId, Convert.ToInt32(ALPCmdID.GET_LINEINFO)));
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
							// Issue command to Get Line Information. When we get the response - then the code there will send an event.
							this.SendALPCommand(new Diacom.AltiGen.AltiLinkPlus.ALPCommand(altiEvent.LocationId, Convert.ToInt32(ALPCmdID.GET_LINEINFO)));
							break;
						}
							// Line removed.
						case SystemConfigChangeCodes.LINEREMOVE:
						{
							this.SendSpEvent(new Ev.LineStateChanged(altiEvent.LocationId, SPLineState.LINE_REMOVE));
							break;
						}
					}
					break;
				}
				default:
				{
					if(altiEvent.LocationId != ControlLocationID) base.ProcessEvent(altiEvent);
					break;
				}
			}
		}

		/// <summary>
		/// Process all responses.
		/// </summary>
		/// <param name="altiResponse">Block of data to process.</param>
		/// <returns>An object with the response.</returns>
		protected override void ProcessResponse(AltiLinkPlus.ALPResponse altiResponse)
		{
			TraceOut.Put("AltiGenSP::ProcessResponse SeqId=" + altiResponse.SequenceId + " LocId=" + altiResponse.LocationId + " CmdId=" + ((ALPCmdID)altiResponse.CommandId).ToString() + " code=" + ((ALPRespID)altiResponse.ResponseCode).ToString());
			ALPLine _line = GetLineBySequenceId(altiResponse.SequenceId);
			
			switch((ALPCmdID)(altiResponse.CommandId))
			{
				// Answer call.
				case ALPCmdID.APC_ANSWER_CALL:
					if( _line == null )	return;
					if(altiResponse.ResponseCode != 0) 
						this.SendSpEvent(new Ev.CommandStatus(_line.SPLineInfo.ID,  Cmd.CommandID.ANSWER_CALL, Ev.CmdStatus.ERROR));
					else 
						this.SendSpEvent(new Ev.CommandStatus(_line.SPLineInfo.ID, Cmd.CommandID.ANSWER_CALL, Ev.CmdStatus.OK));
					break;

				// Reject.
				case ALPCmdID.APC_REJECT_CALL:
					if( _line == null )	return;
					if(altiResponse.ResponseCode != 0) 
						this.SendSpEvent(new Ev.CommandStatus(_line.SPLineInfo.ID,  Cmd.CommandID.REJECT_CALL, Ev.CmdStatus.ERROR));
					else 
						this.SendSpEvent(new Ev.CommandStatus(_line.SPLineInfo.ID, Cmd.CommandID.REJECT_CALL, Ev.CmdStatus.OK));
					break;

				// Pass.
				case ALPCmdID.APC_PASS_CALL:
					if( _line == null )	return;
					if(altiResponse.ResponseCode != 0) 
						this.SendSpEvent(new Ev.CommandStatus(_line.SPLineInfo.ID,  Cmd.CommandID.PASS_CALL, Ev.CmdStatus.ERROR));
					else 
						this.SendSpEvent(new Ev.CommandStatus(_line.SPLineInfo.ID, Cmd.CommandID.PASS_CALL, Ev.CmdStatus.OK));
					break;
				
				// Connect.
				case ALPCmdID.APC_CONNECT_CALL:
					if( _line == null )	return;
					if( altiResponse.ResponseCode != 0)
						this.SendSpEvent(new Ev.CommandStatus(_line.SPLineInfo.ID, Cmd.CommandID.CONNECT_LINES, Ev.CmdStatus.ERROR));
					break;

				// Disconnect.
				case ALPCmdID.APC_DROP_CALL:
					if( _line == null )	return;
					if(altiResponse.ResponseCode != 0)
						this.SendSpEvent(new Ev.CommandStatus(_line.SPLineInfo.ID, Cmd.CommandID.DROP_CALL, Ev.CmdStatus.ERROR));
					break;

				// Dial.
				case ALPCmdID.APC_MAKE_CALL:
				{
					// Searching for a line command to dial was sent on.
					OutcallCommandStruct odc = ((OutcallCommandStruct)(outcallNumbersDialingCommands[altiResponse.SequenceId]));
					if(odc != null)
					{
						outcallNumbersDialingCommands.Remove(altiResponse.SequenceId);
						// Have appropriate line.
						if(altiResponse.ResponseCode != 0) 
							this.SendSpEvent(new Ev.CommandStatus(odc.Line,  Cmd.CommandID.DIAL, Ev.CmdStatus.ERROR));
						else 
							this.SendSpEvent(new Ev.CommandStatus(odc.Line, Cmd.CommandID.DIAL, Ev.CmdStatus.OK));
						break;
					}
					break;
				}

				// Play DTMF.
				case ALPCmdID.APC_PLAY_DTMF:
					if( _line == null )	return;
					if( altiResponse.ResponseCode != 0)
						this.SendSpEvent(new Ev.CommandStatus(_line.SPLineInfo.ID, Cmd.CommandID.PLAY_DTMF, Ev.CmdStatus.ERROR));
					break;

				// Play file.
				case ALPCmdID.APC_PLAY_VOICE:
					if( _line == null )	return;
					if( altiResponse.ResponseCode != 0)
						this.SendSpEvent(new Ev.CommandStatus(_line.SPLineInfo.ID,  Cmd.CommandID.PLAY_FILE, Ev.CmdStatus.ERROR));
					break;

				// Record file.
				case ALPCmdID.APC_RECORD_VOICE:
					if( _line == null )	return;
					if( altiResponse.ResponseCode != 0)
						this.SendSpEvent(new Ev.CommandStatus(_line.SPLineInfo.ID,  Cmd.CommandID.RECORD_FILE, Ev.CmdStatus.ERROR));
					break;

				// Stop playing and recording.
				case ALPCmdID.APC_STOP_PLAY_VOICE:
				case ALPCmdID.APC_STOP_RECORD_VOICE:
					break;

				// Ring extension.
				case ALPCmdID.APC_RING_EXT:
				{
					// Find the line command to dial was sent on.
					OutcallCommandStruct odc = ((OutcallCommandStruct)(outcallNumbersDialingCommands[altiResponse.SequenceId]));
					if(odc != null)
					{
						outcallNumbersDialingCommands.Remove(altiResponse.SequenceId);
						// Have appropriate line.
						if(altiResponse.ResponseCode != 0) 
							this.SendSpEvent(new Ev.CommandStatus(odc.Line,  Cmd.CommandID.RING_EXTENSION, Ev.CmdStatus.ERROR));
						else 
							this.SendSpEvent(new Ev.CommandStatus(odc.Line,  Cmd.CommandID.RING_EXTENSION, Ev.CmdStatus.OK));
					}
					break;
				}

				// Snatch line.
				case ALPCmdID.APC_SNATCH_LINE:
					if( _line == null )	return;
					int lineConnectedToThisID = altiResponse[0].ReadInt32();

					TraceOut.Put("AltiGenSP::SnatchLine. Line1=" + _line.SPLineInfo.ID + " and Line2=" + lineConnectedToThisID );

					ALPLine _connectedLine = this[lineConnectedToThisID];

					if( altiResponse.ResponseCode != 0 ) 
					{
						this.SendSpEvent(new Ev.CommandStatus(_line.SPLineInfo.ID,  Cmd.CommandID.DISCONNECT_LINE, Ev.CmdStatus.ERROR));
					}
					else 
					{
						if(_connectedLine == null)	
						{
							this.SendSpEvent(new Ev.CommandStatus(_line.SPLineInfo.ID,  Cmd.CommandID.DISCONNECT_LINE, Ev.CmdStatus.OK));
						}
						else
						{
							_connectedLine.InfoState = ALPLine.CallInfoState.SNATCHED_SLAVE_LINE;
							_connectedLine.ConnectedLine = _line;
							_connectedLine.IsUnderAPCControl = true;
							this.SendSpEvent(new Ev.LineLinked(_line.SPLineInfo.ID, _connectedLine.SPLineInfo.ID));
						}
					}
					break;

				// Switch music.
				case ALPCmdID.APC_SWITCH_MUSIC:
					if( _line == null )	return;
					if(altiResponse.ResponseCode == 0) 
						this.SendSpEvent(new Ev.CommandStatus(_line.SPLineInfo.ID, Cmd.CommandID.SWITCH_MUSIC, Ev.CmdStatus.OK));
					else 
						this.SendSpEvent(new Ev.CommandStatus(_line.SPLineInfo.ID,  Cmd.CommandID.SWITCH_MUSIC, Ev.CmdStatus.ERROR));
					break;

				// Transfer call.
				case ALPCmdID.APC_TRANSFER_CALL:
					if( _line == null )	return;
					if(altiResponse.ResponseCode != 0)
						this.SendSpEvent(new Ev.CommandStatus(_line.SPLineInfo.ID,  Cmd.CommandID.TRANSFER, Ev.CmdStatus.ERROR));
					break;

				case ALPCmdID.APC_GET_DATA:
					CallInfo rInfo = (CallInfo) this.StoredRingInfo[altiResponse.SequenceId];
					if ( rInfo != null)
					{
						this.StoredRingInfo.Remove(rInfo);
						_line = this[rInfo.lineID];
						if(_line != null && !_line.IsUnderAPCControl )
						{
							// Updating line information.
							_line.SPLineInfo.CalledNumber = rInfo.callerID;
							_line.SPLineInfo.CalledName = rInfo.callerName;
							_line.SPLineInfo.DNISNumber = rInfo.DNISID;
							_line.SPLineInfo.DNISName = rInfo.DNISName;
							_line.SPLineInfo.CIDNumber = rInfo.callerID;
							_line.SPLineInfo.CIDName = rInfo.callerName;
							_line.SPLineInfo.DIDNumber = rInfo.calleeID;
							_line.SPLineInfo.DIDName = rInfo.calleeName;
							_line.SPLineInfo.UserName = rInfo.ANIName;
							_line.SPLineInfo.UserNumber = rInfo.ANIID;
							if(altiResponse.ResponseCode == 0)
							{
								int _lineID = altiResponse[0].ReadInt32();
								int DataType  = altiResponse[1].ReadInt32();
								switch ((ALPInfoType) DataType)
								{
									case ALPInfoType.APC_DATATYPE_USER:
										_line[USER_DATA] = altiResponse.ReadString();
										break;
								}
							}
							this.SendSpEvent(new Ev.Ring(rInfo.lineID, (SPLine)_line.SPLineInfo.Clone()));
							if (_line.InfoState == ALPLine.CallInfoState.CONNECT_DELAYED)
							{
								// Have new connected line. Creating and sending event.
								TraceOut.Put("AltiGenSP:: Line is APC controlled");
								_line.IsUnderAPCControl = true;
								this.SendSpEvent(new Ev.Connect(rInfo.lineID));
								foreach( string _digit in _line.Digits)
								{
									this.SendSpEvent(new Ev.Digit(rInfo.lineID, _digit[0]));
								}
							}
							else
								_line.InfoState = ALPLine.CallInfoState.CALL_INFO_RECEIVED;
						}
					}
					break;

				case ALPCmdID.APC_SET_DATA:
					break;

				default:
					base.ProcessResponse(altiResponse);
					break;
			}
		}
		public object this[object lineID, object key]
		{
			get { 
				ALPLine _line = this[lineID];
				if (_line != null)
					return _line[key]; 
				else
					return null;
			}
			set 
			{ 
				ALPLine _line = this[lineID];
				if( _line != null)
				{
					_line[ key] = value;
					if( ( key is string) && key.Equals(USER_DATA))
					{
						AltiLinkPlus.ALPCommand ac = new AltiLinkPlus.ALPCommand(this.ControlLocationID, (int)(ALPCmdID.APC_SET_DATA));
						ac[0] = new AltiLinkPlus.ALPParameter((int)lineID);
						ac[1] = new AltiLinkPlus.ALPParameter((int)ALPInfoType.APC_DATATYPE_USER);
						ac[2] = new AltiLinkPlus.ALPParameter((string) value);
						ac[3] = new AltiLinkPlus.ALPParameter(0);
						base.SendALPCommand(ac);
					}
				}
			}
		}
	}

	/// <summary>
	/// Implements <see cref="Diacom.ISP"/> interface for AltiGen servers.
	/// </summary>
	public class ASPHdw : ISP
	{
		private readonly AltiGenServiceProvider AltiGenSP = new AltiGenServiceProvider();

		/// <summary>
		/// Disconnects from the server.
		/// </summary>
		/// <remarks>
		/// <para>
		/// Stops threads, closes sockets, etc.
		/// </para>
		///</remarks>
		public void Disconnect()
		{
			this.AltiGenSP.Disconnect();
		}

		/// <summary>
		/// Sends command (one of <see cref="Diacom.Cmd"/> neamespace classes).
		/// </summary>
		/// <param name="cmd">Command to send of <see cref="Diacom.Cmd"/> type.</param>
		/// <remarks>Returns immediately.</remarks>
		public void Send(Diacom.Cmd.CmdBase cmd)
		{
			this.AltiGenSP.Send(cmd);
		}

		/// <summary>
		/// Event with information about <see cref="Diacom.ISP"/> status.
		/// </summary>
		/// <remarks>
		/// Raised when any thread raises exception - i.e. when <see cref="Diacom.ISP.Send"/> or <see cref="Diacom.ISP.Receive"/> methods of <see cref="Diacom.ISP"/> interface are failed.
		/// </remarks>
		public event Diacom.SPStatusEventHandler SPStatusEvent
		{
			add { this.AltiGenSP.SPStatusEvent += value; }
			remove { this.AltiGenSP.SPStatusEvent -= value; }
		}

		/// <summary>
		/// Retrieves next available event (one of <see cref="Diacom.Ev"/> namespace classes).
		/// </summary>
		/// <returns>Next available event of <see cref="Diacom.Ev"/> type.</returns>
		/// <remarks>Waits while queue is empty.</remarks>
		public Diacom.Ev.EvBase Receive()
		{
			return this.AltiGenSP.Receive();
		}

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
		public void Connect(string serverIP, int serverPort, Diacom.SPLogonType logonType, string account, string password, int timeout)
		{
			this.AltiGenSP.Connect(serverIP, serverPort, logonType, account, password, timeout);
		}

		/// <summary>
		/// Provides an access to all the lines available.
		/// </summary>
		/// <returns>Pointer to array of <see cref="Diacom.SPLine"/> [] type.</returns>
		public SPLine[] GetLines()
		{
			return this.AltiGenSP.GetLines();
		}

		/// <summary>
		/// Provides an access to specified line.
		/// </summary>
		/// <param name="lineID">Line ID.</param>
		/// <returns><see cref="Diacom.SPLine"/> instance if line exists, null otherwise.</returns>
		public SPLine GetLine(object lineID)
		{
			return this.AltiGenSP.GetLine(lineID);
		}

		/// <summary>
		/// Indicates status of execution of any method.
		/// </summary>
		/// <remarks>
		/// Check this property every time you think there can be an error during execution.
		/// </remarks>
		public Diacom.SPStatus Status()
		{
			return this.AltiGenSP.Status();
		}

		/// <summary>
		/// Implements <see cref="System.IDisposable.Dispose"/> method of 
		/// <see cref="System.IDisposable"/> interface.
		/// </summary>
		/// <remarks>
		/// <para>Stops threads, closes socket, etc.</para>
		/// <seealso cref="System.IDisposable"/>
		///</remarks>
		public void Dispose()
		{
			this.AltiGenSP.Dispose();
		}
		/// <summary>
		/// Gets or sets the user-specific data.
		/// </summary>
		public object this[object lineID, object key]
		{
			get {return AltiGenSP[lineID, key]; }
			set	{ AltiGenSP[lineID, key] = value; }
		}
	}
}
