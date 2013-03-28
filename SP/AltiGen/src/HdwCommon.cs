using System;

namespace Diacom.AltiGen
{
	/// <summary>
	/// Implements debug output.
	/// </summary>
	internal sealed class TraceOut : Diacom.TraceOut
	{
		/// <summary>
		/// Puts given parameter to trace output, adding the time it happends.
		/// </summary>
		/// <param name="altiEvent">Event in AltiLink Plus V2 format.</param>
		public static void Put(AltiGen.AltiLinkPlus.ALPEvent altiEvent)
		{
			System.Text.StringBuilder msg = new System.Text.StringBuilder();
			msg.Append( "Unhandled event: " + ((AltiGen.ALPEvID)(altiEvent.CommandId)).ToString() + Environment.NewLine );
			msg.Append( "Sequence ID = " + altiEvent.SequenceId.ToString() + ", location ID = " + altiEvent.LocationId.ToString() + ", number of parameters = " + altiEvent.Count.ToString() + Environment.NewLine);
			for(int i = 0; i < altiEvent.Count; i++)
			{
				AltiGen.AltiLinkPlus.ALPParameter _par = altiEvent[i];
				msg.Append( System.String.Format( "Param {0} -> Type = {1}, Size = {2:D3}: ", _par.ParameterID, _par.ParameterType, _par.ParameterSize ) );
				if(_par.ParameterSize > 0) msg.Append( System.String.Concat( System.BitConverter.ToString( _par.ReadBytes( _par.ParameterSize ) ) + Environment.NewLine ) );
			}
			Put( msg.ToString() );
		}

		/// <summary>
		/// Puts given parameter to trace output, adding the time it happends.
		/// </summary>
		/// <param name="altiResponse">Response on command in AltiLink Plus V2 format.</param>
		public static void Put(AltiGen.AltiLinkPlus.ALPResponse altiResponse)
		{
			System.Text.StringBuilder msg = new System.Text.StringBuilder();
			msg.Append( "Unhandled response: " );
			if( System.Enum.IsDefined( typeof( AltiGen.ALPCmdID ), altiResponse.CommandId ) ) msg.Append( ((AltiGen.ALPCmdID)(altiResponse.CommandId)).ToString() + Environment.NewLine );
			else if( System.Enum.IsDefined( typeof( AltiGen.ALPRespID ), altiResponse.CommandId ) ) msg.Append( ((AltiGen.ALPRespID)(altiResponse.CommandId)).ToString() + Environment.NewLine );
			else msg.Append( "unrecognised" + Environment.NewLine );
			msg.Append( "Sequence ID = " + altiResponse.SequenceId.ToString() + ", location ID = " + altiResponse.LocationId.ToString() + ", response ID = " + altiResponse.CommandId.ToString() + ", number of parameters = " + altiResponse.Count.ToString() + ", response code: " + altiResponse.ResponseCode.ToString() + Environment.NewLine );
			for(int i = 0; i < altiResponse.Count; i++)
			{
				AltiGen.AltiLinkPlus.ALPParameter _par = altiResponse[i];
				msg.Append( String.Format( "Param {0} -> Type = {1}, Size = {2:D3}: ", _par.ParameterID, _par.ParameterType, _par.ParameterSize ) );
				if(_par.ParameterSize > 0) msg.Append( System.BitConverter.ToString( _par.ReadBytes( _par.ParameterSize ) ) +Environment.NewLine );
			}
			Put( msg.ToString() );
		}
	}
}
