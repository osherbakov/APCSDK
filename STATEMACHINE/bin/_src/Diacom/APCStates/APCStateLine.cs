//
// Decompiled with: Decompiler.NET, Version=2.0.0.230, Culture=neutral, PublicKeyToken=null, Version: 2.0.0.230
// Decompilation Started at: 25.08.2006 9:36:12
// Copyright 2003 - 2004, Jungle Creatures, Inc., All Rights Reserved. http://www.junglecreatures.com/
// Written by Jonathan Pierce, Email: support@junglecreatures.com
//

namespace Diacom.APCStates

{
		
		#region Namespace Import Declarations
		
			using Diacom;
			using System.Collections;
			using System.Diagnostics;
			using System.Runtime.CompilerServices;
			using System;
			
		#endregion
		
	///<Summary>
	///
 The class that keeps all command and SQL request filters and queues.
 
	///</Summary>
	internal class APCStateLine : APCLine
	
	{
		
		#region Fields
			private static ArrayList __ENCList;
			internal readonly ChannelEventsFilter EventFilter;
			internal readonly SPCommandModule spCommand;
			internal readonly SQLQueriesFilter SQLFilter;
		#endregion
		
		#region Constructors
		
			[DebuggerNonUserCodeAttribute()]
			static APCStateLine ()
			
			{
				APCStateLine.__ENCList = new ArrayList ();
			}
			
			
			internal APCStateLine (object lineReference, SPCommandModule spCmd, string oInitState)
				 : base (oInitState)
			
			{
				int i1 = APCStateLine.__ENCList.Add (new WeakReference (this));
				this.EventFilter = new ChannelEventsFilter (this);
				this.SQLFilter = new SQLQueriesFilter (this);
				base.LineID = RuntimeHelpers.GetObjectValue (lineReference);
				this.spCommand = spCmd;
			}
			
		#endregion
		
		#region Properties
		
			///<Summary>
			///
 Gets and Sets the unique line identifier.
 
			///</Summary>
			///<Returns>
			///Unique reference to the line that SP will recognize.
			///</Returns>
			public object LineReference
			
			{
				get
				{
					return base.LineID;
				}
			}
			
		#endregion
	}
	
}

