using System;
using System.IO;

namespace APCLinesListener
{
	/// <summary>
	/// Implements main functionality of TextWriter.
	/// </summary>
	internal class CTraceStream : TextWriter
	{
		/// <summary>
		/// Storage for the line terminator string used by the current class.
		/// </summary>
		private string _NewLine = Environment.NewLine;

		/// <summary>
		/// Raizes event with the string writed to a stream.
		/// </summary>
		private void _Write(string value)
		{
			if(this.DataReady != null)
				this.DataReady(value);
		}

		/// <summary>
		/// Raizes event with the string writed to a stream.
		/// </summary>
		private void _WriteLine(string value)
		{
			if(this.DataReady != null)
				this.DataReady(value+Environment.NewLine);
		}

		/// <summary>
		/// Delegate for an event the string was written to a stream (with a string as the argument).
		/// </summary>
		public delegate void DataReadyEventHandler(string aData);

		/// <summary>
		/// Event that signales the string was written to a stream.
		/// </summary>
		public event DataReadyEventHandler DataReady;

		/// <summary>
		/// Returns the Encoding in which the output is written.
		/// </summary>
		public override System.Text.Encoding Encoding
		{
			get
			{
				return System.Text.Encoding.Default;
			}
		}

		/// <summary>
		/// Writes the text representation of a Boolean value to the text stream.
		/// </summary>
		/// <param name="value">The boolean to write.</param>
		public override void Write(bool value)
		{
			this._Write(value.ToString());
		}
		
		/// <summary>
		/// Writes a character to the text stream.
		/// </summary>
		/// <param name="value">The character to write.</param>
		public override void Write(char value)
		{
			this._Write(value.ToString());
		}
		
		/// <summary>
		/// Writes a character array to the text stream.
		/// </summary>
		/// <param name="buffer">The character array to write to the text stream.</param>
		public override void Write(char[] buffer)
		{
			this._Write(new String(buffer));
		}
		
		/// <summary>
		/// Writes a subarray of characters to the text stream.
		/// </summary>
		/// <param name="buffer">The character array to write data from.</param>
		/// <param name="index">Starting index in the buffer.</param>
		/// <param name="count">The number of characters to write.</param>
		public override void Write(char[] buffer, int index, int count)
		{
			this._Write(new String(buffer, index, count)); 
		}
		
		/// <summary>
		/// Writes the text representation of a decimal value followed by a line terminator to the text stream.
		/// </summary>
		/// <param name="value">The decimal to write.</param>
		public override void Write(decimal value)
		{
			this._Write(value.ToString());
		}

		/// <summary>
		/// Writes the text representation of an 8-byte floating-point value to the text stream.
		/// </summary>
		/// <param name="value">The double to write.</param>
		public override void Write(double value)
		{
			this._Write(value.ToString());
		}
		
		/// <summary>
		/// Writes the text representation of a 4-byte floating-point value to the text stream.
		/// </summary>
		/// <param name="value">The float to write.</param>
		public override void Write(float value)
		{
			this._Write(value.ToString());
		}
		
		/// <summary>
		/// Writes the text representation of a 4-byte signed integer to the text stream.
		/// </summary>
		/// <param name="value">The integer to write.</param>
		public override void Write(int value)
		{
			this._Write(value.ToString());
		}
		
		/// <summary>
		/// Writes the text representation of an 8-byte signed integer to the text stream.
		/// </summary>
		/// <param name="value">The long to write.</param>
		public override void Write(long value)
		{
			this._Write(value.ToString());
		}
		
		/// <summary>
		/// Writes the text representation of an object to the text stream by calling ToString on that object.
		/// </summary>
		/// <param name="value">The object to write.</param>
		public override void Write(object value)
		{
			this._Write(value.ToString());
		}
		
		/// <summary>
		/// Writes out a formatted string, using the same semantics as <see cref="String.Format"/>.
		/// </summary>
		/// <param name="format">The formatting string.</param>
		/// <param name="arg0">An object to write into the formatted string.</param>
		public override void Write(string format, object arg0)
		{
			this._Write(String.Format(format, arg0));
		}
		
		/// <summary>
		/// Writes out a formatted string, using the same semantics as <see cref="String.Format"/>.
		/// </summary>
		/// <param name="format">The formatting string.</param>
		/// <param name="arg0">An object to write into the formatted string.</param>
		/// <param name="arg1">An object to write into the formatted string.</param>
		public override void Write(string format, object arg0, object arg1)
		{
			this._Write(String.Format(format, arg0, arg1));
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
			this._Write(String.Format(format, arg0, arg1, arg2));
		}
		
		/// <summary>
		/// Writes out a formatted string, using the same semantics as <see cref="String.Format"/>.
		/// </summary>
		/// <param name="format">The formatting string.</param>
		/// <param name="arg">The object array to write into the formatted string.</param>
		public override void Write(string format, params object[] arg)
		{
			this._Write(String.Format(format, arg));
		}
		
		/// <summary>
		/// Writes a string to the text stream.
		/// </summary>
		/// <param name="value">The string to write.</param>
		public override void Write(string value)
		{
			this._Write(value);
		}
		
		/// <summary>
		/// Writes the text representation of a 4-byte unsigned integer to the text stream. This method is not CLS-compliant.
		/// </summary>
		/// <param name="value">The unsigned integer to write.</param>
		public override void Write(uint value)
		{
			this._Write(value.ToString());
		}
		
		/// <summary>
		/// Writes the text representation of an 8-byte unsigned integer to the text stream. This method is not CLS-compliant.
		/// </summary>
		/// <param name="value">The 8-byte unsigned integer to write.</param>
		public override void Write(ulong value)
		{
			this._Write(value.ToString());
		}
		
		/// <summary>
		/// Writes a line terminator to the text stream.
		/// </summary>
		public override void WriteLine()
		{
			this._Write(Environment.NewLine);
		}
		
		/// <summary>
		/// Writes the text representation of a Boolean followed by a line terminator to the text stream.
		/// </summary>
		/// <param name="value">The boolean to write.</param>
		public override void WriteLine(bool value)
		{
			this._WriteLine(value.ToString());
		}
		
		/// <summary>
		/// Writes a character followed by a line terminator to the text stream.
		/// </summary>
		/// <param name="value">The character to write.</param>
		public override void WriteLine(char value)
		{
			this._WriteLine(value.ToString());
		}
		
		/// <summary>
		/// Writes an array of characters followed by a line terminator to the text stream.
		/// </summary>
		/// <param name="buffer">The array of characters to write.</param>
		public override void WriteLine(char[] buffer)
		{
			this._WriteLine(new String(buffer));
		}
		
		/// <summary>
		/// Writes a subarray of characters followed by a line terminator to the text stream.
		/// </summary>
		/// <param name="buffer">The character array from which data is read.</param>
		/// <param name="index">The index into buffer at which to begin reading.</param>
		/// <param name="count">The maximum number of characters to write.</param>
		public override void WriteLine(char[] buffer, int index, int count)
		{
			this._WriteLine(new String(buffer, index, count));
		}
		
		/// <summary>
		/// Writes the text representation of a decimal value followed by a line terminator to the text stream.
		/// </summary>
		/// <param name="value">The decimal to write.</param>
		public override void WriteLine(decimal value)
		{
			this._WriteLine(value.ToString());
		}
		
		/// <summary>
		/// Writes the text representation of a 8-byte floating-point value followed by a line terminator to the text stream.
		/// </summary>
		/// <param name="value">The double to write.</param>
		public override void WriteLine(double value)
		{
			this._WriteLine(value.ToString());
		}
		
		/// <summary>
		/// Writes the text representation of a 4-byte floating-point value followed by a line terminator to the text stream.
		/// </summary>
		/// <param name="value">The float to write.</param>
		public override void WriteLine(float value)
		{
			this._WriteLine(value.ToString());
		}
		
		/// <summary>
		/// Writes the text representation of a 4-byte signed integer followed by a line terminator to the text stream.
		/// </summary>
		/// <param name="value">The integer value to write.</param>
		public override void WriteLine(int value)
		{
			this._WriteLine(value.ToString());
		}
		
		/// <summary>
		/// Writes the text representation of an 8-byte signed integer followed by a line terminator to the text stream.
		/// </summary>
		/// <param name="value">The long value to write.</param>
		public override void WriteLine(long value)
		{
			this._WriteLine(value.ToString());
		}
		
		/// <summary>
		/// Writes the text representation of an object by calling ToString on this object, followed by a line terminator to the text stream.
		/// </summary>
		/// <param name="value">The object to write.</param>
		public override void WriteLine(object value)
		{
			this._WriteLine(value.ToString());
		}
		
		/// <summary>
		/// Writes out a formatted string and a new line, using the same semantics as <see cref="String.Format"/>.
		/// </summary>
		/// <param name="format">The formatting string.</param>
		/// <param name="arg0">An object to write into the formatted string.</param>
		public override void WriteLine(string format, object arg0)
		{
			this._WriteLine(String.Format(format, arg0));
		}
		
		/// <summary>
		/// Writes out a formatted string and a new line, using the same semantics as <see cref="String.Format"/>.
		/// </summary>
		/// <param name="format">The formatting string.</param>
		/// <param name="arg0">An object to write into the formatted string.</param>
		/// <param name="arg1">An object to write into the formatted string.</param>
		public override void WriteLine(string format, object arg0, object arg1)
		{
			this._WriteLine(String.Format(format, arg0, arg1));
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
			this._WriteLine(String.Format(format, arg0, arg1, arg2));
		}
		
		/// <summary>
		/// Writes out a formatted string and a new line, using the same semantics as <see cref="String.Format"/>.
		/// </summary>
		/// <param name="format">The formatting string.</param>
		/// <param name="arg">The object array to write into format string.</param>
		public override void WriteLine(string format, params object[] arg)
		{
			this._WriteLine(String.Format(format, arg));
		}
		
		/// <summary>
		/// Writes a string followed by a line terminator to the text stream.
		/// </summary>
		/// <param name="value">The string to write.</param>
		public override void WriteLine(string value)
		{
			this._WriteLine(value);
		}
		
		/// <summary>
		/// Writes the text representation of a 4-byte unsigned integer followed by a line terminator to the text stream. This method is not CLS-compliant.
		/// </summary>
		/// <param name="value">The unsigned integer to write.</param>
		public override void WriteLine(uint value)
		{
			this._WriteLine(value.ToString());
		}
		
		/// <summary>
		/// Writes the text representation of an 8-byte unsigned integer followed by a line terminator to the text stream. This method is not CLS-compliant.
		/// </summary>
		/// <param name="value">The unsigned long to write.</param>
		public override void WriteLine(ulong value)
		{
			this._WriteLine(value.ToString());
		}
		
		/// <summary>
		/// Returns a string that represents the current class.
		/// </summary>
		/// <returns>A string that represents the current class.</returns>
		public override string ToString()
		{
			return "Lines State Listener Text Writer for Trace Listener";
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
	}
}
