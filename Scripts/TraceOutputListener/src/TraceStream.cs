using System;
using System.IO;
using System.Text;

namespace Diacom
{
	/// <summary>
	/// Implements main functionality of TextWriter.
	/// </summary>
	internal class TraceStream : TextWriter
	{
		/// <summary>
		/// Storage for the line terminator string used by the current class.
		/// </summary>
		private string _NewLine = null;
		private StringBuilder StreamStorage = null;

		public TraceStream()
		{
			this._NewLine = Environment.NewLine;
			this.StreamStorage = new StringBuilder();
		}

		/// <summary>
		/// Returns the Encoding in which the output is written.
		/// </summary>
		public override Encoding Encoding
		{
			get
			{
				return Encoding.Default;
			}
		}

		/// <summary>
		/// Writes the text representation of a Boolean value to the text stream.
		/// </summary>
		/// <param name="value">The boolean to write.</param>
		public override void Write(bool value)
		{
			this.StreamStorage.Append(value);
		}
		
		/// <summary>
		/// Writes a character to the text stream.
		/// </summary>
		/// <param name="value">The character to write.</param>
		public override void Write(char value)
		{
			this.StreamStorage.Append(value);
		}
		
		/// <summary>
		/// Writes a character array to the text stream.
		/// </summary>
		/// <param name="buffer">The character array to write to the text stream.</param>
		public override void Write(char[] buffer)
		{
			this.StreamStorage.Append(buffer);
		}
		
		/// <summary>
		/// Writes a subarray of characters to the text stream.
		/// </summary>
		/// <param name="buffer">The character array to write data from.</param>
		/// <param name="index">Starting index in the buffer.</param>
		/// <param name="count">The number of characters to write.</param>
		public override void Write(char[] buffer, int index, int count)
		{
			this.StreamStorage.Append(new String(buffer, index, count)); 
		}
		
		/// <summary>
		/// Writes the text representation of a decimal value followed by a line terminator to the text stream.
		/// </summary>
		/// <param name="value">The decimal to write.</param>
		public override void Write(decimal value)
		{
			this.StreamStorage.Append(value);
		}

		/// <summary>
		/// Writes the text representation of an 8-byte floating-point value to the text stream.
		/// </summary>
		/// <param name="value">The double to write.</param>
		public override void Write(double value)
		{
			this.StreamStorage.Append(value);
		}
		
		/// <summary>
		/// Writes the text representation of a 4-byte floating-point value to the text stream.
		/// </summary>
		/// <param name="value">The float to write.</param>
		public override void Write(float value)
		{
			this.StreamStorage.Append(value);
		}
		
		/// <summary>
		/// Writes the text representation of a 4-byte signed integer to the text stream.
		/// </summary>
		/// <param name="value">The integer to write.</param>
		public override void Write(int value)
		{
			this.StreamStorage.Append(value);
		}
		
		/// <summary>
		/// Writes the text representation of an 8-byte signed integer to the text stream.
		/// </summary>
		/// <param name="value">The long to write.</param>
		public override void Write(long value)
		{
			this.StreamStorage.Append(value);
		}
		
		/// <summary>
		/// Writes the text representation of an object to the text stream by calling ToString on that object.
		/// </summary>
		/// <param name="value">The object to write.</param>
		public override void Write(object value)
		{
			this.StreamStorage.Append(value);
		}
		
		/// <summary>
		/// Writes out a formatted string, using the same semantics as <see cref="String.Format"/>.
		/// </summary>
		/// <param name="format">The formatting string.</param>
		/// <param name="arg0">An object to write into the formatted string.</param>
		public override void Write(string format, object arg0)
		{
			this.StreamStorage.AppendFormat(format, arg0);
		}
		
		/// <summary>
		/// Writes out a formatted string, using the same semantics as <see cref="String.Format"/>.
		/// </summary>
		/// <param name="format">The formatting string.</param>
		/// <param name="arg0">An object to write into the formatted string.</param>
		/// <param name="arg1">An object to write into the formatted string.</param>
		public override void Write(string format, object arg0, object arg1)
		{
			this.StreamStorage.AppendFormat(format, arg0, arg1);
		}
		
		/// <summary>
		/// Writes out a formatted string, using the same semantics as <see cref="String.Format"/>.
		/// </summary>
		/// <param name="format">The formatting string.</param>
		/// <param name="arg0">An object to write into the formatted string.</param>
		/// <param name="arg1">An object to write into the formatted string.</param>
		/// <param name="arg2">An object to write into the formatted string.</param>
		public override void Write(string format, object arg0, object arg1, object arg2)
		{
			this.StreamStorage.AppendFormat(format, arg0, arg1, arg2);
		}
		
		/// <summary>
		/// Writes out a formatted string, using the same semantics as <see cref="String.Format"/>.
		/// </summary>
		/// <param name="format">The formatting string.</param>
		/// <param name="arg">The object array to write into the formatted string.</param>
		public override void Write(string format, params object[] arg)
		{
			this.StreamStorage.AppendFormat(format, arg);
		}
		
		/// <summary>
		/// Writes a string to the text stream.
		/// </summary>
		/// <param name="value">The string to write.</param>
		public override void Write(string value)
		{
			this.StreamStorage.Append(value);
		}
		
		/// <summary>
		/// Writes the text representation of a 4-byte unsigned integer to the text stream. This method is not CLS-compliant.
		/// </summary>
		/// <param name="value">The unsigned integer to write.</param>
		public override void Write(uint value)
		{
			this.StreamStorage.Append(value);
		}
		
		/// <summary>
		/// Writes the text representation of an 8-byte unsigned integer to the text stream. This method is not CLS-compliant.
		/// </summary>
		/// <param name="value">The 8-byte unsigned integer to write.</param>
		public override void Write(ulong value)
		{
			this.StreamStorage.Append(value);
		}
		
		/// <summary>
		/// Writes a line terminator to the text stream.
		/// </summary>
		public override void WriteLine()
		{
			this.StreamStorage.Append(this._NewLine);
		}
	
		/// <summary>
		/// Writes the text representation of a Boolean followed by a line terminator to the text stream.
		/// </summary>
		/// <param name="value">The boolean to write.</param>
		public override void WriteLine(bool value)
		{
			this.Write(value);
			this.Write(this._NewLine);
		}
		
		/// <summary>
		/// Writes a character followed by a line terminator to the text stream.
		/// </summary>
		/// <param name="value">The character to write.</param>
		public override void WriteLine(char value)
		{
			this.Write(value);
			this.Write(this._NewLine);
		}
		
		/// <summary>
		/// Writes an array of characters followed by a line terminator to the text stream.
		/// </summary>
		/// <param name="buffer">The array of characters to write.</param>
		public override void WriteLine(char[] buffer)
		{
			this.Write(buffer);
			this.Write(this._NewLine);
		}
		
		/// <summary>
		/// Writes a subarray of characters followed by a line terminator to the text stream.
		/// </summary>
		/// <param name="buffer">The character array from which data is read.</param>
		/// <param name="index">The index into buffer at which to begin reading.</param>
		/// <param name="count">The maximum number of characters to write.</param>
		public override void WriteLine(char[] buffer, int index, int count)
		{
			this.Write(buffer, index, count);
			this.Write(this._NewLine);
		}
		
		/// <summary>
		/// Writes the text representation of a decimal value followed by a line terminator to the text stream.
		/// </summary>
		/// <param name="value">The decimal to write.</param>
		public override void WriteLine(decimal value)
		{
			this.Write(value);
			this.Write(this._NewLine);
		}
		
		/// <summary>
		/// Writes the text representation of a 8-byte floating-point value followed by a line terminator to the text stream.
		/// </summary>
		/// <param name="value">The double to write.</param>
		public override void WriteLine(double value)
		{
			this.Write(value);
			this.Write(this._NewLine);
		}
		
		/// <summary>
		/// Writes the text representation of a 4-byte floating-point value followed by a line terminator to the text stream.
		/// </summary>
		/// <param name="value">The float to write.</param>
		public override void WriteLine(float value)
		{
			this.Write(value);
			this.Write(this._NewLine);
		}
		
		/// <summary>
		/// Writes the text representation of a 4-byte signed integer followed by a line terminator to the text stream.
		/// </summary>
		/// <param name="value">The integer value to write.</param>
		public override void WriteLine(int value)
		{
			this.Write(value);
			this.Write(this._NewLine);
		}
		
		/// <summary>
		/// Writes the text representation of an 8-byte signed integer followed by a line terminator to the text stream.
		/// </summary>
		/// <param name="value">The long value to write.</param>
		public override void WriteLine(long value)
		{
			this.Write(value);
			this.Write(this._NewLine);
		}
		
		/// <summary>
		/// Writes the text representation of an object by calling ToString on this object, followed by a line terminator to the text stream.
		/// </summary>
		/// <param name="value">The object to write.</param>
		public override void WriteLine(object value)
		{
			this.Write(value);
			this.Write(this._NewLine);
		}
		
		/// <summary>
		/// Writes out a formatted string and a new line, using the same semantics as <see cref="String.Format"/>.
		/// </summary>
		/// <param name="format">The formatting string.</param>
		/// <param name="arg0">An object to write into the formatted string.</param>
		public override void WriteLine(string format, object arg0)
		{
			this.Write(format, arg0);
			this.Write(this._NewLine);
		}
		
		/// <summary>
		/// Writes out a formatted string and a new line, using the same semantics as <see cref="String.Format"/>.
		/// </summary>
		/// <param name="format">The formatting string.</param>
		/// <param name="arg0">An object to write into the formatted string.</param>
		/// <param name="arg1">An object to write into the formatted string.</param>
		public override void WriteLine(string format, object arg0, object arg1)
		{
			this.Write(format, arg0, arg1);
			this.Write(this._NewLine);
		}
		
		/// <summary>
		/// Writes out a formatted string and a new line, using the same semantics as <see cref="String.Format"/>.
		/// </summary>
		/// <param name="format">The formatting string.</param>
		/// <param name="arg0">An object to write into the formatted string.</param>
		/// <param name="arg1">An object to write into the formatted string.</param>
		/// <param name="arg2">An object to write into the formatted string.</param>
		public override void WriteLine(string format, object arg0, object arg1, object arg2)
		{
			this.Write(format, arg0, arg1, arg2);
			this.Write(this._NewLine);
		}
		
		/// <summary>
		/// Writes out a formatted string and a new line, using the same semantics as <see cref="String.Format"/>.
		/// </summary>
		/// <param name="format">The formatting string.</param>
		/// <param name="arg">The object array to write into format string.</param>
		public override void WriteLine(string format, params object[] arg)
		{
			this.Write(format, arg);
			this.Write(this._NewLine);
		}
		
		/// <summary>
		/// Writes a string followed by a line terminator to the text stream.
		/// </summary>
		/// <param name="value">The string to write.</param>
		public override void WriteLine(string value)
		{
			this.Write(value);
			this.Write(this._NewLine);
		}
		
		/// <summary>
		/// Writes the text representation of a 4-byte unsigned integer followed by a line terminator to the text stream. This method is not CLS-compliant.
		/// </summary>
		/// <param name="value">The unsigned integer to write.</param>
		public override void WriteLine(uint value)
		{
			this.Write(value);
			this.Write(this._NewLine);
		}
		
		/// <summary>
		/// Writes the text representation of an 8-byte unsigned integer followed by a line terminator to the text stream. This method is not CLS-compliant.
		/// </summary>
		/// <param name="value">The unsigned long to write.</param>
		public override void WriteLine(ulong value)
		{
			this.Write(value);
			this.Write(this._NewLine);
		}
		
		/// <summary>
		/// Returns a string that represents the current class.
		/// </summary>
		/// <returns>A string that represents the current class.</returns>
		public override string ToString()
		{
			return "TraceStream: to get stream contents use Contents property";
		}
		
		/// <summary>
		/// Gets or sets the line terminator string used by the current class.
		/// </summary>
		public override string NewLine
		{
			get
			{
				return this._NewLine;
			}
			set
			{
				this._NewLine = value;
			}
		}

		/// <summary>
		/// Gets whole the stream contents as a string.
		/// </summary>
		public string Contents
		{
			get
			{
				return this.StreamStorage.ToString();
			}
		}
	}
}
