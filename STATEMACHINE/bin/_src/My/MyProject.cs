//
// Decompiled with: Decompiler.NET, Version=2.0.0.230, Culture=neutral, PublicKeyToken=null, Version: 2.0.0.230
// Decompilation Started at: 25.08.2006 9:36:12
// Copyright 2003 - 2004, Jungle Creatures, Inc., All Rights Reserved. http://www.junglecreatures.com/
// Written by Jonathan Pierce, Email: support@junglecreatures.com
//

namespace My

{
		
		#region Namespace Import Declarations
		
			using Microsoft.VisualBasic.ApplicationServices;
			using Microsoft.VisualBasic.CompilerServices;
			using Microsoft.VisualBasic;
			using Microsoft.VisualBasic.MyServices.Internal;
			using System.CodeDom.Compiler;
			using System.ComponentModel.Design;
			using System.ComponentModel;
			using System.Diagnostics;
			using System.Runtime.CompilerServices;
			using System.Runtime.InteropServices;
			using System;
			
		#endregion
		
	[GeneratedCodeAttribute("MyTemplate", "8.0.0.0")]
	[HideModuleNameAttribute()]
	[StandardModuleAttribute()]
	internal sealed class MyProject
	
	{
		
		#region Fields
			private static readonly ThreadSafeObjectProvider<MyApplication> m_AppObjectProvider;
			private static readonly ThreadSafeObjectProvider<MyComputer> m_ComputerObjectProvider;
			private static readonly ThreadSafeObjectProvider<MyWebServices> m_MyWebServicesObjectProvider;
			private static readonly ThreadSafeObjectProvider<User> m_UserObjectProvider;
		#endregion
		
		#region Nested Types
		
			[EditorBrowsableAttribute(EditorBrowsableState.Never)]
			[MyGroupCollectionAttribute("System.Web.Services.Protocols.SoapHttpClientProtocol", "Create__Instance__", "Dispose__Instance__", "")]
			internal sealed class MyWebServices
			
			{
				
				#region Constructors
				
					[EditorBrowsableAttribute(EditorBrowsableState.Never)]
					[DebuggerHiddenAttribute()]
					public MyWebServices ()
					
					{
					}
					
				#endregion
				
				#region Methods
				
					[DebuggerHiddenAttribute()]
					private static T Create__Instance__<T> (T instance)
					
					{
						if (instance == null)
						{
							return Activator.CreateInstance<T> ();
						}
						else
						{
							return instance;
						}
					}
					
					[DebuggerHiddenAttribute()]
					private void Dispose__Instance__<T> (ref T instance)
					
					{
						T t1 = default (T);
						instance = t1;
					}
					
					[EditorBrowsableAttribute(EditorBrowsableState.Never)]
					[DebuggerHiddenAttribute()]
					public override bool Equals (object o)
					
					{
						return base.Equals (RuntimeHelpers.GetObjectValue (o));
					}
					
					[DebuggerHiddenAttribute()]
					[EditorBrowsableAttribute(EditorBrowsableState.Never)]
					public override int GetHashCode ()
					
					{
						return base.GetHashCode ();
					}
					
					[DebuggerHiddenAttribute()]
					[EditorBrowsableAttribute(EditorBrowsableState.Never)]
					new internal Type GetType ()
					
					{
						return typeof (MyProject.MyWebServices);
					}
					
					[DebuggerHiddenAttribute()]
					[EditorBrowsableAttribute(EditorBrowsableState.Never)]
					public override string ToString ()
					
					{
						return base.ToString ();
					}
					
				#endregion
			}
			
			
			[EditorBrowsableAttribute(EditorBrowsableState.Never)]
			[ComVisibleAttribute(false)]
			internal sealed class ThreadSafeObjectProvider<T>
			
			{
				
				#region Fields
					private readonly ContextValue<T> m_Context;
				#endregion
				
				#region Constructors
				
					[EditorBrowsableAttribute(EditorBrowsableState.Never)]
					[DebuggerHiddenAttribute()]
					public ThreadSafeObjectProvider ()
					
					{
						this.m_Context = new ContextValue<T> ();
					}
					
				#endregion
				
				#region Properties
				
					internal T GetInstance
					
					{
						get
						{
							T Value1 = this.m_Context.Value;
							if (Value1 == null)
							{
								Value1 = Activator.CreateInstance<T> ();
								this.m_Context.Value = Value1;
							}
							return Value1;
						}
					}
					
				#endregion
			}
			
		#endregion
		
		#region Constructors
		
			[DebuggerNonUserCodeAttribute()]
			static MyProject ()
			
			{
				MyProject.m_ComputerObjectProvider = new ThreadSafeObjectProvider<MyComputer> ();
				MyProject.m_AppObjectProvider = new ThreadSafeObjectProvider<MyApplication> ();
				MyProject.m_UserObjectProvider = new ThreadSafeObjectProvider<Microsoft.VisualBasic.ApplicationServices.User> ();
				MyProject.m_MyWebServicesObjectProvider = new ThreadSafeObjectProvider<MyWebServices> ();
			}
			
		#endregion
		
		#region Properties
		
			[HelpKeywordAttribute("My.Application")]
			internal static MyApplication Application
			
			{
				get
				{
					return MyProject.m_AppObjectProvider.GetInstance;
				}
			}
			
			
			[HelpKeywordAttribute("My.Computer")]
			internal static MyComputer Computer
			
			{
				get
				{
					return MyProject.m_ComputerObjectProvider.GetInstance;
				}
			}
			
			
			[HelpKeywordAttribute("My.User")]
			internal static User User
			
			{
				get
				{
					return MyProject.m_UserObjectProvider.GetInstance;
				}
			}
			
			
			[HelpKeywordAttribute("My.WebServices")]
			internal static MyWebServices WebServices
			
			{
				get
				{
					return MyProject.m_MyWebServicesObjectProvider.GetInstance;
				}
			}
			
		#endregion
	}
	
}

