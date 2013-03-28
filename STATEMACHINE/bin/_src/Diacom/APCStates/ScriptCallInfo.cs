//
// Decompiled with: Decompiler.NET, Version=2.0.0.230, Culture=neutral, PublicKeyToken=null, Version: 2.0.0.230
// Decompilation Started at: 25.08.2006 9:36:12
// Copyright 2003 - 2004, Jungle Creatures, Inc., All Rights Reserved. http://www.junglecreatures.com/
// Written by Jonathan Pierce, Email: support@junglecreatures.com
//

namespace Diacom.APCStates

{
		
		#region Namespace Import Declarations
		
			using System.Diagnostics;
			using System;
			using System.Reflection;
			
		#endregion
		
	///<Summary>
	///
 Internal structure to keep information about scripting calls - 
 Procedure name, on which host, instance of the class and method itself.
 
	///</Summary>
	internal class ScriptCallInfo
	
	{
		
		#region Fields
			public ScriptHost CallHost;
			public object CallInstance;
			public MethodInfo CallMethod;
			public string CallName;
		#endregion
		
		#region Constructors
		
			[DebuggerNonUserCodeAttribute()]
			public ScriptCallInfo ()
			
			{
			}
			
		#endregion
	}
	
}

