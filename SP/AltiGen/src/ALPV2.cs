using System;
using System.IO;
using System.Text;
using System.Net.Sockets;
using System.Collections;
using System.Collections.Generic;

namespace Diacom
{
	namespace AltiGen
	{
		/// <summary>
		/// Represents all aspects of AltiLink Plus protocol.
		/// </summary>
		internal class AltiLinkPlus
		{
			/// <summary>
			/// Encoding as integer type.
			/// </summary>
			private const int  ENCODING_INT = 0x10101010;

			/// <summary>
			/// Encoding as byte type.
			/// </summary>
			private const byte ENCODING_BYTE = 0x10;

			/// <summary>
			/// Specifies the type of AltiLink Plus packet.
			/// </summary>
			internal enum ALP_PACKET_TYPE
			{
				/// <summary>
				/// Empty.
				/// </summary>
				ALP_PACKET_NONE=0,

				/// <summary>
				/// Command.
				/// </summary>
				ALP_PACKET_CMD,

				/// <summary>
				/// Responce.
				/// </summary>
				ALP_PACKET_RESP,

				/// <summary>
				/// Event.
				/// </summary>
				ALP_PACKET_EVENT
			}

			/// <summary>
			/// Specifies the type of the parameters.
			/// </summary>
			internal enum ALP_PARAM_TYPE
			{
				/// <summary>
				/// Byte type of parameter.
				/// </summary>
				PARAM_TYPE_BYTE = 0,

				/// <summary>
				/// Int type of parameter.
				/// </summary>
				PARAM_TYPE_INT,

				/// <summary>
				/// Char type of parameter.
				/// </summary>
				PARAM_TYPE_CHAR,

				/// <summary>
				/// Wide char type of parameter.
				/// </summary>
				PARAM_TYPE_WCHAR,

				/// <summary>
				/// Parameter is struct.
				/// </summary>
				PARAM_TYPE_STRUCT,

				/// <summary>
				/// Parameter is block.
				/// </summary>
				PARAM_TYPE_BBLOCK
			}

			/// <summary>
			/// Calculates checksum for array of bytes.
			/// </summary>
			/// <param name="bytesData">Array of bytes.</param>
			/// <returns>Checksum for array.</returns>
			private static int CalculateCheckSum(byte [] bytesData)
			{
				int cs = 0;
				foreach (byte c in bytesData)
				{
					cs += c;
				}
				return  cs;
			}

			/// <summary>
			/// Calculates checksum for int type.
			/// </summary>
			/// <param name="value">Integer.</param>
			/// <returns>Checksum for integer.</returns>
			private static int CalculateCheckSum(int value)
			{
				return  CalculateCheckSum(BitConverter.GetBytes(value));
			}

			/// <summary>
			/// Calculates encoded checksum for byte type.
			/// </summary>
			/// <param name="value">Byte type variable.</param>
			/// <returns>Checksum for byte type variable.</returns>
			private static int EncodeCheckSum(byte value)
			{
				return  value ^ ENCODING_BYTE;
			}

			/// <summary>
			/// Calculates encoded checksum for short type.
			/// </summary>
			/// <param name="value">Short type variable.</param>
			/// <returns>Checksum for short type variable.</returns>
			private static int EncodeCheckSum(short value)
			{
				return  EncodeCheckSum(BitConverter.GetBytes(value));
			}

			/// <summary>
			/// Calculates encoded checksum for int type.
			/// </summary>
			/// <param name="value">Int type variable.</param>
			/// <returns>Checksum for int type variable.</returns>
			private static int EncodeCheckSum(int value)
			{
				return  EncodeCheckSum(BitConverter.GetBytes(value));
			}

			/// <summary>
			/// Calculates encoded checksum for array of bytes.
			/// </summary>
			/// <param name="bytesData">Array of bytes.</param>
			/// <returns>Checksum for the array.</returns>
			private static int  EncodeCheckSum(byte [] bytesData)
			{
				int cs = 0;
				foreach (byte c in bytesData)
				{
					cs += (byte)(c ^ ENCODING_BYTE) ;
				}
				return  cs;
			}
		
			/// <summary>
			/// Encodes byte type variable.
			/// </summary>
			/// <param name="byteData">Byte type variable.</param>
			/// <returns>Encoded byte type variable.</returns>
			private static byte EncodeData(byte byteData)
			{
				return  (byte)(byteData ^ ENCODING_BYTE);
			}

			/// <summary>
			/// Encodes short type variable.
			/// </summary>
			/// <param name="shortData">Short type variable.</param>
			/// <returns>Encoded short type variable.</returns>
			private static short EncodeData(short shortData)
			{
				return  (short)(shortData ^ ENCODING_INT);
			}

			/// <summary>
			/// Encodes int type variable.
			/// </summary>
			/// <param name="intData">Int type variable.</param>
			/// <returns>Encoded int type variable.</returns>
			private static int EncodeData(int intData)
			{
				return  intData ^ ENCODING_INT;
			}

			/// <summary>
			/// Encodes array of bytes.
			/// </summary>
			/// <param name="bytesData">Array of bytes.</param>
			/// <returns>Encoded array of bytes.</returns>
			private static byte [] EncodeData(byte [] bytesData)
			{
				byte [] result = new Byte[bytesData.Length];
				for(int i = 0; i < bytesData.Length; i++)
				{
					result[i] = (byte)(bytesData[i] ^ ENCODING_BYTE) ;
				}
				return  result;
			}

			/// <summary>
			/// Defines AltiLink Plus v.2 packet tag.
			/// </summary>
			private struct ALV2Tag
			{
				/// <summary>
				/// AltiLink Plus v2 'ALV2' signature.
				/// </summary>
				private static byte [] ALV2 = {(byte) 'A', (byte)'L', (byte)'V', (byte)'2'};

				/// <summary>
				/// AltiLink Plus v2 'ALV2' signature as int32 type variable.
				/// </summary>
				private static int  ALV2_SIG = BitConverter.ToInt32(ALV2, 0);

				/// <summary>
				/// AltiLink Plus v2 'ALV2' signature as int32 type variable (bigendian coding).
				/// </summary>
				private static int  ALV2_SIG_BIG_ENDIAN = ALV2[0]<<24 | ALV2[1]<<16 | ALV2[2]<<8 | ALV2[3];

				/// <summary>
				/// Finds the AltiLink Plus Tag structure in the stream.
				/// </summary>
				/// <remarks>Will be reading all bytes in the stream until it finds the Tag.</remarks>
				/// <param name="br">Binary reader for the stream.</param>
				internal void Find(BinaryReader br)
				{
					int currentData = 0;
					while (true)
					{
						currentData = (currentData << 8) | br.ReadByte();
						if (currentData == ALV2_SIG_BIG_ENDIAN)
							break;
					}
				}
				/// <summary>
				/// Reads the AltiLink Plus Tag structure from the stream.
				/// </summary>
				/// <exception cref="System.ApplicationException">Raised when data read is not a valid AltiLink Plus Tag.</exception>
				/// <param name="br">Binary reader for the stream.</param>
				internal void Read(BinaryReader br)
				{
					if(br.ReadInt32() != ALV2_SIG)
						throw new System.ApplicationException("The ALV2 signature is not valid");
				}

				/// <summary>
				/// Writes the AltiLink Plus Tag structure to the stream.
				/// </summary>
				/// <param name="bw">Binary writer for the stream.</param>
				internal void Write(BinaryWriter bw) 
				{
					bw.Write(ALV2);
				}

				/// <summary>
				/// Gets the Tag structure length in bytes.
				/// </summary>
				internal int Length
				{
					get	{return 4;}
				}

				/// <summary>
				/// Gets the Tag structure checksum.
				/// </summary>
				internal int CheckSum
				{
					get	{return CalculateCheckSum(ALV2);}
				}
			}

			///<summary>
			/// Defines the packet header for AltiLink Plus; immediately follows the Tag.
			///</summary>
			private struct PacketHeader
			{
				/// <summary>
				/// Version ID.
				/// </summary>
				private static int versionId = 20;

				/// <summary>
				/// Quantity of blocks.
				/// </summary>
				private int blockCount;

				/// <summary>
				/// Session ID.
				/// </summary>
				private int sessionId;

				/// <summary>
				/// Length of the packet.
				/// </summary>
				private int packetLength;

				/// <summary>
				/// Reads the packet header structure from the stream.
				/// </summary>
				/// <exception cref="System.ApplicationException">Raised when version of the packet does not match.</exception>
				/// <param name="br">Binary reader for the stream.</param>
				internal void Read(BinaryReader br)
				{
					int version = br.ReadInt32();
					blockCount = br.ReadInt32();
					sessionId = br.ReadInt32();
					packetLength = br.ReadInt32();
					if(version != versionId)
						throw new System.ApplicationException("Invalid AltiLink version packet");
				}

				/// <summary>
				/// Writes the packet header structure to the stream.
				/// </summary>
				/// <param name="bw">Binary writer for the stream.</param>
				internal void Write(BinaryWriter bw)
				{
					bw.Write(versionId);
					bw.Write(blockCount);
					bw.Write(sessionId);
					bw.Write(packetLength);
				}

				/// <summary>
				/// Gets or sets the total packet length in bytes.
				/// </summary>
				internal int PacketLength
				{
					get { return packetLength;}
					set { packetLength = value; }
				}

				/// <summary>
				/// Gets or sets the number of blocks in the packet.
				/// </summary>
				internal int BlockCount
				{
					get { return blockCount;  }
					set { blockCount = value; }
				}

				/// <summary>
				/// Gets or sets the session ID for that connection.
				/// </summary>
				internal int SessionId
				{
					get { return sessionId;  }
					set { sessionId = value; }
				}

				/// <summary>
				/// Gets the length of the packet header in bytes.
				/// </summary>
				internal int  Length
				{
					get	{return 16;}
				}

				/// <summary>
				/// Gets the packet header checksum.
				/// </summary>
				internal int CheckSum
				{
					get	
					{
						return CalculateCheckSum(versionId) + 
							CalculateCheckSum(blockCount) +
							CalculateCheckSum(sessionId) +
							CalculateCheckSum(packetLength);
					}
				}
			}

			///<summary>
			/// Represents Data block header structure for the AltiLink Plus packet.
			///</summary>
			internal struct BlockHeader
			{
				/// <summary>
				/// Length of the block.
				/// </summary>
				internal int blockLength;

				/// <summary>
				/// Type of the block.
				/// </summary>
				internal ALP_PACKET_TYPE blockType;

				/// <summary>
				/// Sequence ID for block.
				/// </summary>
				internal int blockSequenceId;

				/// <summary>
				/// Location ID for block.
				/// </summary>
				internal int locationId;

				/// <summary>
				/// Command code for block.
				/// </summary>
				internal int commandCode;

				/// <summary>
				/// Reads the data block header structure from the stream.
				/// </summary>
				/// <exception cref="System.ApplicationException">Raised when packet type is not valid.</exception>
				/// <param name="br">Binary reader for the stream.</param>
				internal void Read(BinaryReader br)
				{
					blockLength = EncodeData(br.ReadInt32());
					blockType = (ALP_PACKET_TYPE)(EncodeData(br.ReadInt32()));
					if(!Enum.IsDefined(typeof (ALP_PACKET_TYPE), blockType))
						throw new System.ApplicationException("Packet type is not valid");
					blockSequenceId = EncodeData(br.ReadInt32());
					locationId = EncodeData(br.ReadInt32());
					commandCode = EncodeData(br.ReadInt32());
				}

				/// <summary>
				/// Writes the data block header structure to the stream.
				/// </summary>
				/// <param name="bw">Binary writer for the stream.</param>
				internal void Write(BinaryWriter bw)
				{
					bw.Write(EncodeData(blockLength));
					bw.Write(EncodeData((int)blockType));
					bw.Write(EncodeData(blockSequenceId));
					bw.Write(EncodeData(locationId));
					bw.Write(EncodeData(commandCode));
				}

				/// <summary>
				/// Gets the length of the header in bytes.
				/// </summary>
				internal int Length
				{
					get	{return 20;}
				}

				/// <summary>
				/// Gets the checksum of the header.
				/// </summary>
				internal int CheckSum
				{
					get	
					{
						return EncodeCheckSum(blockLength) + 
							EncodeCheckSum((int)blockType) +
							EncodeCheckSum(blockSequenceId) +
							EncodeCheckSum(locationId) +
							EncodeCheckSum(commandCode);
					}
				}
			}

			/// <summary>
			/// <para>Defines the base class to read and write the data for AltiLink Plus packet.</para>
			/// <para>Supports both ReadXXXX and Write(XXX) methods from BinaryReader and BinaryWriter.</para>
			/// </summary>
			internal abstract class ALPData
			{
				/// <summary>
				/// BinaryReader for data.
				/// </summary>
				private BinaryReader br;

				/// <summary>
				/// BinaryWriter for data.
				/// </summary>
				private BinaryWriter bw;

				/// <summary>
				/// Initializes an instance of the ALPData class without parameters.
				/// </summary>
				public ALPData()
				{
					MemoryStream _memStream = new MemoryStream();
					br = new BinaryReader(_memStream);
					bw = new BinaryWriter(_memStream);
				}

				/// <summary>
				/// Initializes the storage in ALPData class from the supplied array of bytes.
				/// </summary>
				/// <param name="buffer">Byte array that is used to fill and initialize the storage.</param>
				protected void ToALPData(byte [] buffer)
				{
					MemoryStream _memStream = new MemoryStream(buffer,0, buffer.Length, true, true);
					br.Close();
					bw.Close();
					br = new BinaryReader(_memStream); 
					bw = new BinaryWriter(_memStream); 
				}

				/// <summary>
				/// Initializes the storage in ALPData class from the supplied array of bytes.
				/// </summary>
				/// <param name="buffer">Byte array that is used to fill and initialize the storage.</param>
				/// <param name="index">Starting position in a byte array the data will be read from.</param>
				/// <param name="count">The number of bytes that will be taken from the buffer to initialize the Read storage.</param>
				protected void ToALPData(byte [] buffer, int index, int count)
				{
					byte [] readData = new byte [count];
					MemoryStream _memStream = new MemoryStream(buffer, index, count, true, false);			
					br.Close();
					bw.Close();
					br = new BinaryReader(_memStream); 
					bw = new BinaryWriter(_memStream); 
				}

				/// <summary>
				/// Reads the next byte from the data storage and advances the current position by one byte.
				/// </summary>
				/// <returns>The byte from the curent position in a data block.</returns>
				public byte ReadByte()
				{
					return br.ReadByte();
				}

				/// <summary>
				/// Reads <paramref name="count" /> bytes from the data block into a byte array and advances the current position by <paramref name="count" /> bytes.
				/// </summary>
				/// <param name="count">The number of bytes to read.</param>
				/// <returns>A byte array containing data read from the data block.</returns>
				public byte [] ReadBytes(int count)
				{
					return br.ReadBytes(count);
				}

				/// <summary>
				/// Reads short (2 bytes) integer from the datablock and advances the current position by two byte.
				/// </summary>
				/// <returns>The short (2 bytes) from the curent position in a data block.</returns>
				public short ReadInt16()
				{
					return br.ReadInt16();
				}

				/// <summary>
				/// Reads int (4 bytes) integer from the datablock and advances the current position by four bytes.
				/// </summary>
				/// <returns>The int (4 bytes) from the curent position in a data block.</returns>
				public int ReadInt32()
				{
					return br.ReadInt32();
				}
				/// <summary>
				/// <para>Reads the string from the data block and advances the current position to the next byte after string terminating null character.</para>
				/// <para>Converts all bytes from the current position till the first null character.</para>
				/// </summary>
				/// <returns>A string from the curent position in a data block.</returns>
				public string ReadString()
				{
					int		_size = (int ) (br.BaseStream.Length - br.BaseStream.Position);
					byte [] _buff = new byte [_size];
					int		_count = 0;
					string	result;
					byte	_currentByte;

					// Find the length of the string - the ending \0 byte
					while( _count < _size )
					{
						_currentByte = br.ReadByte();
						if( _currentByte == 0)
							break;
						_buff[_count++] = _currentByte;
					}
					result =  Encoding.Default.GetString(_buff, 0, _count);
					return result;
				}

				/// <summary>
				/// <para>Reads the <paramref name="stringSize" /> bytes from the data block and converts them into the string.</para>
				/// <para>Advances the current position by <paramref name="stringSize" /> bytes.</para>
				/// </summary>
				/// <param name="stringSize">Number of bytes to read.</param>
				/// <returns>A string from the curent position in a data block.</returns>
				public string ReadString(int stringSize)
				{
					int		_size = (int ) (br.BaseStream.Length - br.BaseStream.Position);
					byte [] _buff = br.ReadBytes(stringSize);

					int		_count = 0;
					while( (_count < stringSize) && (_buff[_count] != 0) )
					{
						_count++;
					}

					return Encoding.Default.GetString(_buff, 0, _count); 
				}

				/// <summary>
				/// Converts all data stored in the ALPData class to an array of bytes. 
				/// </summary>
				/// <returns>An array of bytes that represents all internal storage of ALPData class.</returns>
				public virtual byte [] GetBytes()
				{
					byte [] _result =  new byte [bw.BaseStream.Length];
					long _pos = bw.BaseStream.Position;		// Save the current position
					bw.BaseStream.Position = 0;
					bw.BaseStream.Read(_result, 0, (int) bw.BaseStream.Length);
					bw.BaseStream.Position = _pos;			// Restore the current position
					return _result;
				}

				/// <summary>
				/// Gets and sets the position in the data storage where the next write or read will put/get bytes.
				/// </summary>
				protected int Position
				{
					get { return (int) br.BaseStream.Position; }
					set { bw.BaseStream.Position = value; br.BaseStream.Position = value;}
				}

				/// <summary>
				/// Adds the provided array of bytes into the data block and advances the current position by the size of the <paramref name="value" /> parameter.
				/// </summary>
				/// <param name="value">Array of bytes to be added to the data block.</param>
				public void Write(byte [] value)
				{
					bw.Write(value);
				}

				/// <summary>
				/// Adds one byte into the data block and advances the current position by one bytes.
				/// </summary>
				/// <param name="value">Byte to be added to the data block.</param>
				public void Write(byte value)
				{
					bw.Write(value);
				}

				/// <summary>
				/// Adds short (2 bytes) integer into the data block and advances the current position by two bytes.
				/// </summary>
				/// <param name="value">Short to be added to the data block.</param>
				public void Write(short value)
				{
					bw.Write(value);
				}

				/// <summary>
				/// Adds int (4 bytes) integer into the data block and advances the current position by four bytes.
				/// </summary>
				/// <param name="value">Int to be added to the parameter block.</param>
				public void Write(int value)
				{
					bw.Write(value);
				}

				/// <summary>
				/// <para>Adds string into the data block and  terminating null byte.</para>
				/// <para>Advances the current position by the (length of the string + 1) bytes.</para>
				/// </summary>
				/// <param name="value">String to be added to the data block.</param>
				public void Write(string value)
				{
					int _count;
					byte[] _data;

					if ( (value == null) || (value.Length == 0))
					{
						_data = new byte[4];
					}
					else
					{
						_count = Encoding.Default.GetByteCount(value) + 1;
						_data = new byte[_count];
						Encoding.Default.GetBytes(value, 0, value.Length, _data, 0);
					}
					bw.Write(_data);
				}

				/// <summary>
				/// <para>Adds string of fixed size into the data block and terminating null byte.</para>
				/// <para>The rest will be filled with null bytes if the string is shorter that a specified length.</para>
				/// <para>Advances the current position by <see ref ="Size"/> bytes.</para>
				/// </summary>
				/// <param name="value">String to be added to the data block.</param>
				/// <param name="Size">Size of the string - exactly that many bytes will be placed into the data block.</param>
				public void Write(string value, int Size)
				{
					byte[]	_data;

					_data = new byte[Size];
					if ( value != null)
					{
						int	_count = Math.Min(Encoding.Default.GetByteCount(value), Size - 1);
						Encoding.Default.GetBytes(value, 0, _count, _data, 0);
					}
					bw.Write(_data);
				}

				/// <summary>
				/// Gets and sets the total number of bytes in data storage.
				/// </summary>
				public virtual int Length
				{
					get { return (int) br.BaseStream.Length ; }
					set { bw.BaseStream.SetLength(value); }
				}
			}

			/// <summary>
			/// Defines the base class for Command, Event and Response data blocks.
			/// </summary>
			internal class ALPDataBlock : ALPData, IEnumerable, IEnumerator
			{
				/// <summary>
				/// Storage for sequence ID.
				/// </summary>
				private static int internalSequenceID = 0;

				/// <summary>
				/// Header of the block.
				/// </summary>
				private BlockHeader header;
			
				/// <summary>
				/// The array of all parameters read - we keep it here for index Method to work.
				/// </summary>
				private List<ALPParameter> paramArray = new List<ALPParameter>();

				/// <summary>
				/// Current parameter counter.
				/// </summary>
				private int currentParam = 0;

				/// <summary>
				/// Current parameter value.
				/// </summary>
				private ALPParameter currentValue = null;

				/// <summary>
				/// Initializes a new instance of <see cref="ALPDataBlock"/> class using information from supplied Data header.
				/// </summary>
				/// <param name="dataHeader">Provides the information for new data block.</param>
				protected ALPDataBlock (BlockHeader dataHeader)
				{
					header = dataHeader;
				}

				/// <summary>
				/// Initializes a new instance instance of <see cref="ALPDataBlock"/> class using provided information.
				/// </summary>
				/// <param name="packetType">Type of the Datablock.</param>
				/// <param name="sequenceID">Unique sequence ID for that data block.</param>
				/// <param name="locID">Unique number for line or trunk.</param>
				/// <param name="commandID">Command code for Command packet or Command code for which Responce packet will be sent.</param>
				internal ALPDataBlock(ALP_PACKET_TYPE packetType, int sequenceID, int locID, int commandID)
				{
					header.blockType = packetType;
					header.blockSequenceId = sequenceID;
					header.locationId = locID;
					header.commandCode = commandID;
					header.blockLength = Length;
				}

				/// <summary>
				/// Processes and extracts all parameters from the datablock data into the internal array.
				/// </summary>
				protected void ProcessParams()
				{
					int bytesLeft = base.Length - base.Position;
					paramArray.Clear();
					while( bytesLeft >= ALPParameter.MinimumLength )
					{
						ALPParameter _param = new ALPParameter(this);
						paramArray.Add( _param);
						bytesLeft = base.Length - base.Position;
					}
				}

				/// <summary>
				/// Reads the datablock from the stream.
				/// </summary>
				/// <param name="br">Binary reader for the stream.</param>
				internal virtual void Read(BinaryReader br)
				{
					int _size = header.blockLength - header.Length;
					ToALPData(EncodeData(br.ReadBytes(_size)), 0, _size);
					paramArray.Clear();
                    currentParam = 0;
				}

				/// <summary>
				/// Writes the datablock into the stream.
				/// </summary>
				/// <param name="bw">Binary writer for the stream.</param>
				internal virtual void Write(BinaryWriter bw)
				{
					header.blockLength = Length;
					header.Write(bw);
					bw.Write(EncodeData(GetBytes()));
				}

				/// <summary>
				/// Adds any object derived from ALPData into the datablock.
				/// </summary>
				/// <param name="value">Parameter to be added.</param>
				public void Add(ALPData value)
				{
					base.Write(value.GetBytes());
				}

				/// <summary>
				/// Gets the total length of the datablock in bytes.
				/// </summary>
				public override int Length
				{
					get	{return header.Length + base.Length;}
				}

				/// <summary>
				/// Gets the current sequence Id number for the datablock.
				/// </summary>
				public int SequenceId
				{
					get { return header.blockSequenceId;}
				}

				/// <summary>
				/// Gets the line or trunk number to which DataBlock referrs to.
				/// </summary>
				public int LocationId
				{
					get { return header.locationId;}
				}

				/// <summary>
				/// Gets the current command ID.
				/// </summary>
				public int CommandId
				{
					get { return header.commandCode;}
				}

				/// <summary>
				/// Generates the next unique sequence number.
				/// </summary>
				/// <returns>An int that is the next value for the sequence.</returns>
				protected static int NextSequenceId()
				{
					return System.Threading.Interlocked.Increment(ref internalSequenceID);
				}

				/// <summary>
				/// Gets the checksum of the datablock.
				/// </summary>
				internal virtual int CheckSum
				{
					get	
					{
						header.blockLength = Length;
						return header.CheckSum + EncodeCheckSum(base.GetBytes()) ;
					}
				}

				/// <summary>
				/// Implements the indexer functionality to get and set parameters into datablock.
				/// <para>Returns <see langword="null"/> if parameter does not exist.</para>
				/// </summary>
				public ALPParameter this [int paramID]
				{
					get 
					{
						ALPParameter result = (ALPParameter) paramArray[paramID];
						foreach( ALPParameter _curr in paramArray)
						{
							if(_curr.ParameterID == paramID)
							{
								result = _curr;
								break;
							}
						}
						return result;
					}
					set
					{
						value.ParameterID = paramID;
						this.Add(value);
					}
				}

				/// <summary>
				/// Gets the number of parameters in the datablock.
				/// </summary>
				public int Count
				{
					get	{ return paramArray.Count; }
				}

				/// <summary>
				/// Implements <see cref="IEnumerable"/> method.
				/// </summary>
				/// <returns>Enumerator for the class.</returns>
				IEnumerator IEnumerable.GetEnumerator()
				{
					return this;
				}

				/// <summary>
				/// <para>Implements the <see cref="IEnumerator.Reset"/> interface for simple iteration.</para>
				/// <para>Resets enumeration.</para>
				/// </summary>
				void IEnumerator.Reset()
				{
                    currentParam = 0;
				}

				/// <summary>
				/// <para>Implements the <see cref="IEnumerator.Current"/> interface for simple iteration.</para>
				/// <para>Gets the current value.</para>
				/// </summary>
                object IEnumerator.Current
				{
					get	{ return currentValue; }
				}

				/// <summary>
				/// <para>Implements the <see cref="IEnumerator.MoveNext"/> interface for simple iteration.</para>
				/// <para>Advances the iterator to the next element.</para>
				/// </summary>
				/// <returns>
				/// <para><see langword="true"/> if the next element exists;</para>
				/// <para><see langword="false"/> if iterator cannot continue.</para>
				/// </returns>
                bool IEnumerator.MoveNext()
				{
					if( currentParam >= paramArray.Count )
					{
						return false;
					}
					else
					{
						currentValue = (ALPParameter) paramArray[currentParam++];
						return true;
					}
				}
			}

			/// <summary>
			/// Represents AltiServ Command.
			/// </summary>
			internal class ALPCommand : ALPDataBlock
			{
				/// <summary>
				/// Initializes a new instance of <see cref="ALPCommand"/> class using information from supplied Data header.
				/// </summary>
				/// <param name="dataHeader">Information for new data block.</param>
				internal ALPCommand(BlockHeader dataHeader) : base(dataHeader)
				{
				}

				/// <summary>
				/// Initializes a new instance of <see cref="ALPCommand"/> class using provided information.
				/// </summary>
				/// <param name="locID">Unique number for line or trunk.</param>
				/// <param name="commandID">Command code for the packet.</param>
				public ALPCommand(int locID, int commandID) : base (ALP_PACKET_TYPE.ALP_PACKET_CMD, ALPDataBlock.NextSequenceId(), locID, commandID)
				{
				}

				/// <summary>
				/// Reads the <see cref="ALPCommand"/> information from the stream.
				/// </summary>
				/// <param name="br">Binary reader for the stream.</param>
				internal override void Read(BinaryReader br)
				{
					base.Read(br);
					base.ProcessParams();
				}
			}

			/// <summary>
			/// Represents an asynchronous event from AltiServ.
			/// </summary>
			internal class ALPEvent : ALPDataBlock
			{
				/// <summary>
				/// Initializes a new instance of <see cref="ALPEvent"/> class using information from supplied Data header.
				/// </summary>
				/// <param name="dataHeader">Information for new data block.</param>
				internal ALPEvent(BlockHeader dataHeader) : base(dataHeader)
				{
				}

				/// <summary>
				/// Initializes a new instance of <see cref="ALPEvent"/> class using provided information.
				/// </summary>
				/// <param name="locID">Unique number for line or trunk.</param>
				/// <param name="eventID">Event code for the packet.</param>
				public ALPEvent (int locID, int eventID) : base (ALP_PACKET_TYPE.ALP_PACKET_EVENT, ALPDataBlock.NextSequenceId(), locID, eventID)
				{
				}

				/// <summary>
				/// Reads the <see cref="ALPEvent"/> information from the stream.
				/// </summary>
				/// <param name="br">Binary reader for the stream.</param>
				internal override void Read(BinaryReader br)
				{
					base.Read(br);
					base.ProcessParams();
				}
			}

			/// <summary>
			/// Represents the response to the previously sent command to AltiServ.
			/// </summary>
			internal class ALPResponse : ALPDataBlock
			{
				/// <summary>
				/// Responce code.
				/// </summary>
				private int respCode;

				/// <summary>
				/// Initializes a new instance instance of <see cref="ALPResponse"/> class using information from supplied Data header.
				/// </summary>
				/// <param name="dataHeader">Information for new data block.</param>
				internal ALPResponse(BlockHeader dataHeader) : base(dataHeader)
				{
				}

				/// <summary>
				/// Initializes a new instance of <see cref="ALPResponse"/> class using provided information.
				/// </summary>
				/// <param name="sequenceID">Unique sequence ID for that datablock.</param>
				/// <param name="locID">Unique number for line or trunk.</param>
				/// <param name="commandID">Command code for which this response is sent.</param>
				/// <param name="responseCode">Response code for the previously sent command.</param>
				public ALPResponse(int sequenceID, int locID, int commandID, int responseCode)
					: base (ALP_PACKET_TYPE.ALP_PACKET_RESP, sequenceID, locID, commandID)
				{
					respCode = responseCode;
					base.Write(respCode);
				}

				/// <summary>
				/// Reads the <see cref="ALPResponse"/> information from the stream.
				/// </summary>
				/// <param name="br">Binary reader for the stream.</param>
				internal override void Read(BinaryReader br)
				{
					base.Read(br);
					respCode = base.ReadInt32();	// First 4 bytes is the Response code
					base.ProcessParams();
				}

				/// <summary>
				/// Gets the value of the responce code for the Responce packet.
				/// </summary>
				public int ResponseCode
				{
					get {return respCode; }
				}
			}

			/// <summary>
			/// Defines the parameters for the command sent to AltiServ or Event data from AltiServ.
			/// </summary>
			internal class ALPParameter : ALPData
			{
				/// <summary>
				/// Parameter ID.
				/// </summary>
				private int paramID;

				/// <summary>
				/// Parameter type.
				/// </summary>
				private ALP_PARAM_TYPE paramType;

				/// <summary>
				/// Parameter size.
				/// </summary>
				private int paramSize;

				/// <summary>
				/// <para>Initializes a new instance of <see cref="ALPParameter"/> class from the supplied data class.</para>
				/// <para>Reads from the <see ref ="data"/> class necessary number of bytes and advances the current position.</para>
				/// </summary>
				/// <exception cref="System.ApplicationException">Raised when size and type do not match.</exception>
				/// <param name="data">Array of bytes from which all parameters are extracted.</param>
				internal ALPParameter(ALPData data)
				{
					paramID = data.ReadInt32();
					paramType = (ALP_PARAM_TYPE) data.ReadByte();
					paramSize = data.ReadInt16();
					if(!Enum.IsDefined(typeof (ALP_PARAM_TYPE), paramType))
						throw new System.ApplicationException("Parameter value is not valid for given type ");
					ToALPData(data.ReadBytes(paramSize));
				}
			
				/// <summary>
				/// <para>Initializes a new instance of <see cref="ALPParameter"/> class with no parameters.</para>
				/// <para>Creates PARAM_TYPE_STRUCT block, which may hold other types.</para>
				/// </summary>
				public ALPParameter() : this(0,	ALP_PARAM_TYPE.PARAM_TYPE_STRUCT)
				{
				}

				/// <summary>
				/// Initializes a new instance of <see cref="ALPParameter"/> class of specified type.
				/// </summary>
				/// <exception cref="System.ApplicationException">Raised when parameter type is not valid.</exception>
				/// <param name="parameterType">Type of the parameters the <see cref="ALPParameter"/> will hold.</param>
				public ALPParameter(ALP_PARAM_TYPE parameterType) : this(0, parameterType)
				{
				}

				/// <summary>
				/// Initializes a new instance of <see cref="ALPParameter"/> class with specified information.
				/// </summary>
				/// <exception cref="System.ApplicationException">Raised when parameter type is not valid.</exception>
				/// <param name="parameterID">Parameter ID - 0, 1, 2 and so on.</param>
				/// <param name="parameterType">Type of the parameter.</param>
				public ALPParameter(int parameterID, ALP_PARAM_TYPE parameterType)
				{
					paramID = parameterID;
					paramType = parameterType;
					if(!Enum.IsDefined(typeof (ALP_PARAM_TYPE), parameterType))
						throw new System.ApplicationException("Parameter type is not valid");
				}

				/// <summary>
				/// Initializes a new instance of <see cref="ALPParameter"/> class that holds only one parameter of type <see cref="ALP_PARAM_TYPE.PARAM_TYPE_BYTE"/>.
				/// </summary>
				/// <param name="value">Value of the parameter.</param>
				public ALPParameter(byte value): this(0, value)
				{
				}

				/// <summary>
				/// Initializes a new instance of <see cref="ALPParameter"/> class that holds only one parameter of type <see cref="ALP_PARAM_TYPE.PARAM_TYPE_BYTE"/>.
				/// </summary>
				/// <param name="parameterID">Parameter ID - 0, 1, 2 and so on.</param>
				/// <param name="value">Value of the parameter.</param>
				public ALPParameter(int parameterID, byte value)
				{
					paramID = parameterID;
					paramType = ALP_PARAM_TYPE.PARAM_TYPE_BYTE;
					Write(value);
				}

				/// <summary>
				/// Initializes a new instance of <see cref="ALPParameter"/> class that holds only one parameter of type <see cref="ALP_PARAM_TYPE.PARAM_TYPE_INT"/>.
				/// </summary>
				/// <param name="value">Value of the parameter.</param>
				public ALPParameter(int value): this(0, value)
				{
				}

				/// <summary>
				/// Initializes a new instance of <see cref="ALPParameter"/> class that holds only one parameter of type <see cref="ALP_PARAM_TYPE.PARAM_TYPE_INT"/>.
				/// </summary>
				/// <param name="parameterID">Parameter ID - 0, 1, 2 and so on.</param>
				/// <param name="value">Value of the parameter.</param>
				public ALPParameter(int parameterID, int value)
				{
					paramID = parameterID;
					paramType = ALP_PARAM_TYPE.PARAM_TYPE_INT;
					base.Write(value);
				}

				/// <summary>
				/// Initializes a new instance of <see cref="ALPParameter"/> class that holds only one parameter of type <see cref="ALP_PARAM_TYPE.PARAM_TYPE_CHAR"/>.
				/// </summary>
				/// <param name="value">Value of the parameter.</param>
				public ALPParameter(string value): this(0, value)
				{
				}

				/// <summary>
				/// Initializes a new instance of <see cref="ALPParameter"/> class that holds only one parameter of type <see cref="ALP_PARAM_TYPE.PARAM_TYPE_CHAR"/>.
				/// </summary>
				/// <param name="parameterID">Parameter ID - 0, 1, 2 and so on.</param>
				/// <param name="value">Value of the parameter.</param>
				public ALPParameter(int parameterID, string value)
				{
					paramID = parameterID;
					paramType = ALP_PARAM_TYPE.PARAM_TYPE_CHAR;
					base.Write(value);
				}

				/// <summary>
				/// Converts the all data stored in the parameter into an array of bytes.
				/// </summary>
				/// <returns>An array of bytes that represents all internal storage for the parameter.</returns>
				public override byte [] GetBytes()
				{
					byte [] result = new byte [this.Length];
					paramSize = base.Length;

					int		index = 0;
					BitConverter.GetBytes(paramID).CopyTo(result, index); index += 4;
					BitConverter.GetBytes((byte)paramType).CopyTo(result, index); index += 1;
					BitConverter.GetBytes((short)paramSize).CopyTo(result, index); index += 2;
					base.GetBytes().CopyTo(result, index); index += base.Length;
					return result;
				}

				/// <summary>
				/// Gets the minimum length required for the parameter block in bytes.
				/// </summary>
				internal static int MinimumLength
				{
					get { return 1 + 7;}
				}

				/// <summary>
				/// Gets the total length of the parameter block in bytes.
				/// </summary>
				public override int Length
				{
					get { return base.Length + 7;}
				}

				/// <summary>
				/// Gets and sets the Parameter ID - 0, 1, 2 ,3 and so on for that parameter block.
				/// </summary>
				public int ParameterID
				{
					get { return paramID; }
					set { paramID = value; }
				}

				/// <summary>
				/// Gets the type of the parameter.
				/// </summary>
				public int ParameterType
				{
					get { return (int) paramType; }
				}

				/// <summary>
				/// Gets the size of the parameter in bytes.
				/// </summary>
				public int ParameterSize
				{
					get { return paramSize; }
				}
			}

			/// <summary>
			/// Defines the CheckSum structure for the whole AltiLink Plus packet.
			/// </summary>
			private struct PacketCheckSum
			{
				/// <summary>
				/// Checksum storage.
				/// </summary>
				private int checkSum;

				/// <summary>
				/// Reads checksum.
				/// </summary>
				/// <param name="br">Binary reader for the stream.</param>
				internal void Read(BinaryReader br)	
				{ 
					checkSum = br.ReadInt32();
				}

				/// <summary>
				/// Writes checksum.
				/// </summary>
				/// <param name="bw">Binary writer for the stream.</param>
				internal void Write(BinaryWriter bw)	{ bw.Write(checkSum);}

				/// <summary>
				/// Length of checksum.
				/// </summary>
				internal int Length
				{	
					get	{return 4;}	
				}
				
				/// <summary>
				/// Value of checksum.
				/// </summary>
				internal int Value
				{
					get {return checkSum;}
					set {checkSum = value;}
				}
			}

			/// <summary>
			/// <para>Represents the whole AltiLink Plus packet. 
			/// Starts with the ALV2 tag, then packed header,
			/// then one or more datablocks, and all is closed by the checksum.</para>
			/// </summary>
			public class ALPPacket
			{
				/// <summary>
				/// ALV2 tag for packet.
				/// </summary>
				private ALV2Tag tag = new ALV2Tag();

				/// <summary>
				/// Header of the packet.
				/// </summary>
				private PacketHeader header = new PacketHeader();

				/// <summary>
				/// Checksum of the packet.
				/// </summary>
				private PacketCheckSum checkSum = new PacketCheckSum();

				/// <summary>
				/// Initializes a new instance of ALPPacket class without parameters.
				/// </summary>
				public ALPPacket()
				{
				}

				/// <summary>
				/// Payload data array.
				/// </summary>
				private ArrayList payloadData = new ArrayList();

				/// <summary>
				/// Initializes a new instance of ALPPacket class with specified SessionID.
				/// </summary>
				/// <param name="SessionId">Unique number identifying the current session with AltiLink.</param>
				public ALPPacket(int SessionId)
				{
					header.SessionId = SessionId;
				}

				/// <summary>
				/// Initializes a new instance of ALPPacket class with specified SessionID and adds the specified data block.
				/// </summary>
				/// <param name="SessionId">Unique number identifying the current session.</param>
				/// <param name="PacketData">Command, Event or Response to be added to the packet.</param>
				public ALPPacket(int SessionId, ALPDataBlock PacketData)
				{
					header.SessionId = SessionId;
					this.Add(PacketData);
				}

				/// <summary>
				/// Writes the whole AltiLink Plus packet into the provided stream.
				/// </summary>
				/// <param name="bw">Binary writer for the stream.</param>
				public void Write(BinaryWriter bw)
				{
					// Calculate the total packet length and checksum for all members
					int		_totalLength = tag.Length + header.Length + checkSum.Length;
					int		_totalCheckSum = tag.CheckSum;
					foreach(ALPDataBlock data in payloadData)
					{
						_totalLength += ((ALPDataBlock)data).Length;
						_totalCheckSum += ((ALPDataBlock)data).CheckSum;
					}
					header.PacketLength = _totalLength;
					// Please note that we calculate the checksum AFTER we calculated total block length 
					_totalCheckSum += header.CheckSum ;
					checkSum.Value  = _totalCheckSum; 
				
					// Write all the data into the stream in the following order
					tag.Write(bw);
					header.Write(bw);
					foreach(ALPDataBlock data in payloadData)
					{
						((ALPDataBlock)data).Write(bw);
					}
					checkSum.Write(bw);
				}

				/// <summary>
				/// Reads the whole AltiLink Plus packet from the provided data stream.
				/// </summary>
				/// <param name="br">Binary reader for the stream.</param>
				public void Read(BinaryReader br)
				{
					payloadData.Clear();

					tag.Find(br);		// Start by looking for ALV2 tag
					header.Read(br);	// Read the packet header

					int		_blockCount = header.BlockCount; 
					int		_checkSum = tag.CheckSum + header.CheckSum ;
				
					ALPDataBlock	data;
					BlockHeader bh = new BlockHeader();
					while ( _blockCount-- != 0)
					{
						// Find out the packet type, size and create appropriate datablock
						bh.Read(br);	// Read datablock header
						switch (bh.blockType)
						{
							case ALP_PACKET_TYPE.ALP_PACKET_CMD:
								data = new ALPCommand(bh);
								break;

							case ALP_PACKET_TYPE.ALP_PACKET_EVENT:
								data = new ALPEvent(bh);
								break;

							case ALP_PACKET_TYPE.ALP_PACKET_RESP:
								data = new ALPResponse(bh);
								break;
							default:
								throw new System.ApplicationException("Packet type is unknown");
						}
						// Read the rest of the data
						data.Read(br);
						_checkSum += data.CheckSum;
						payloadData.Add(data);
					}

					checkSum.Read(br);
					// Verify the integrity of the packet by comparing the checksum
					if (_checkSum != checkSum.Value)
						throw new System.ApplicationException("Packet checksum does not match");
				}

				/// <summary>
				/// Adds the datablock into the AltiLink Plus packet.
				/// </summary>
				/// <param name="value">Command, Event or Response to be added to the packet.</param>
				public void Add(ALPDataBlock value)
				{
					payloadData.Add(value);
					header.BlockCount++;
				}

				/// <summary>
				/// Gets the data blocks from the packet.
				/// </summary>
				public ALPDataBlock [] PacketData
				{
					get { return (ALPDataBlock []) payloadData.ToArray(typeof(ALPDataBlock));}
				}

				/// <summary>
				/// Gets the session Id for the packet.
				/// </summary>
				public int SessionId
				{
					get { return header.SessionId;}
				}

				/// <summary>
				/// Gets the number of data blocks (ALPCommand, ALPEvent or ALPResponse) in the packet.
				/// </summary>
				public int BlockCount
				{
					get { return header.BlockCount; }
				}
			}
		}
	} // end of AltiGen namespace.
}
