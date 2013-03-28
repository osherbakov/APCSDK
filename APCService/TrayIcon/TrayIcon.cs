using System;
using System.Windows.Forms;
using System.Collections;
using System.Reflection;
using System.Runtime.InteropServices;


namespace Diacom.APCService.Control
{
	/// <summary>
	/// APCService System Tray Control.
	/// </summary>
	public class SystemTrayControl : System.ComponentModel.Component, IDisposable
	{
		/// <summary>
		/// Occurs when user clicks on balloon tooltip (for Shell32.dll version 6, i.e. WindowsXP).
		/// </summary>
		public event EventHandler BalloonClick;

		/// <summary>
		/// Occurs when user clicks twice on icon.
		/// </summary>
		public event EventHandler LeftMouseButtonDoubleClick;

		/// <summary>
		/// Implements target for Windows messages.
		/// </summary>
		private class WinMsgTarget : System.Windows.Forms.Form
		{
			/// <summary>
			/// ID for callback messages.
			/// </summary>
			public readonly int UserMessage;

			/// <summary>
			/// ID to receive notifications on taskbar creation.
			/// </summary>
			public readonly int TrayCreatedMessage;

			/// <summary>
			/// Double click message.
			/// </summary>
			private const int WM_LEFT_MOUSE_BUTTON_DOUBLE_CLICK		= 0x203;

			/// <summary>
			/// Right mouse button click message.
			/// </summary>
			private const int WM_RIGHT_MOUSE_BUTTON_UP				= 0x205;

			/// <summary>
			/// Click on balloon tooltip.
			/// </summary>
			private const int NIN_BALLOON_CLICK						= 0x405;

			private readonly SystemTrayControl TrayControl;

			/// <summary>
			/// Creates new instance of the class.
			/// </summary>
			public WinMsgTarget(SystemTrayControl TrayControl )
			{
				this.TrayControl = TrayControl;
				this.Text = "Windows messages target window";
				UserMessage = RegisterWindowMessage("Diacom Tray Control Message");
				TrayCreatedMessage = RegisterWindowMessage("TaskbarCreated");
			}

			/// <summary>
			/// Registers the Unique message with the system and returns the ID of that message.
			/// </summary>
			/// <param name="text">Unique text, that identifies the message.</param>
			/// <returns></returns>
			[DllImport("User32.Dll")]
			private static extern System.Int32 RegisterWindowMessage(System.String text);

			/// <summary>
			/// The TrackPopupMenuEx function displays a shortcut menu at the specified location and tracks the selection of items on the shortcut menu. The shortcut menu can appear anywhere on the screen.
			/// </summary>
			/// <param name="hMenu">Handle to the shortcut menu to be displayed. This handle can be obtained by calling the <see href="ms-help://MS.VSCC.2003/MS.MSDNQTR.2003APR.1033/winui/winui/windowsuserinterface/resources/menus/menureference/menufunctions/trackpopupmenuex.htm">CreatePopupMenu</see> function to create a new shortcut menu or by calling the <see href="ms-help://MS.VSCC.2003/MS.MSDNQTR.2003APR.1033/winui/winui/windowsuserinterface/resources/menus/menureference/menufunctions/trackpopupmenuex.htm">GetSubMenu</see> function to retrieve a handle to a submenu associated with an existing menu item.</param>
			/// <param name="uFlags">Specifies function options.</param>
			/// <param name="x">Horizontal location of the shortcut menu, in screen coordinates.</param>
			/// <param name="y">Vertical location of the shortcut menu, in screen coordinates.</param>
			/// <param name="hWnd">Handle to the window that owns the shortcut menu. This window receives all messages from the menu.</param>
			/// <param name="ignore">Pointer to a TPMPARAMS structure that specifies an area of the screen the menu should not overlap. This parameter can be NULL.</param>
			/// <returns>
			/// <para>If you specify TPM_RETURNCMD in the fuFlags parameter, the return value is the menu-item identifier of the item that the user selected. If the user cancels the menu without making a selection, or if an error occurs, then the return value is zero.</para>
			/// <para>If you do not specify TPM_RETURNCMD in the fuFlags parameter, the return value is nonzero if the function succeeds and zero if it fails</para>
			/// </returns>
			[DllImport("User32.Dll")]
			private static extern System.Int32 TrackPopupMenuEx(System.IntPtr hMenu, System.UInt32 uFlags, System.Int32 x, System.Int32 y, System.IntPtr hWnd, System.IntPtr ignore);

			/// <summary>
			/// This structure defines the x- and y-coordinates of a point.
			/// </summary>
			[StructLayout(LayoutKind.Sequential)] 
				private struct Point
			{
				/// <summary>
				/// Specifies the x-coordinate of a point.
				/// </summary>
				public System.Int32 x;

				/// <summary>
				/// Specifies the y-coordinate of a point.
				/// </summary>
				public System.Int32 y;
			}

			/// <summary>
			/// Retrieves the cursor's position, in screen coordinates.
			/// </summary>
			/// <param name="point">Pointer to a <see cref="Point"/> structure that receives the screen coordinates of the cursor.</param>
			/// <returns>
			/// <para>If the function succeeds, the return value is nonzero.</para>
			/// <para>If the function fails, the return value is zero.</para>
			/// </returns>
			[DllImport("User32.Dll")]
			private static extern System.Int32 GetCursorPos(ref Point point);

			/// <summary>
			/// Puts the thread that created the specified window into the foreground and activates the window. Keyboard input is directed to the window, and various visual cues are changed for the user. The system assigns a slightly higher priority to the thread that created the foreground window than it does to other threads.
			/// </summary>
			/// <param name="hWnd">Handle to the window that should be activated and brought to the foreground.</param>
			/// <returns>
			/// <para>If the window was brought to the foreground, the return value is nonzero.</para>
			/// <para>If the window was not brought to the foreground, the return value is zero.</para>
			/// </returns>
			[DllImport("User32.Dll")]
			private static extern System.Int32 SetForegroundWindow(System.IntPtr hWnd);


			/// <summary>
			/// Handles incoming Windows messages.
			/// </summary>
			/// <param name="m">Message to handle.</param>
			protected override void DefWndProc(ref System.Windows.Forms.Message m)
			{
				if(m.Msg.Equals(UserMessage))
				{
					switch(m.LParam.ToInt32())
					{
						case WM_LEFT_MOUSE_BUTTON_DOUBLE_CLICK:
						{
							if(this.TrayControl.LeftMouseButtonDoubleClick != null) this.TrayControl.LeftMouseButtonDoubleClick(this, EventArgs.Empty);
							break;
						}
						case WM_RIGHT_MOUSE_BUTTON_UP:
						{
							if(this.TrayControl.ID != (uint)m.WParam) return;
							if(this.TrayControl.Menu == null) return;
							Point point = new Point();
							GetCursorPos(ref point);
							SetForegroundWindow(this.Handle);
							this.TrayControl.OnPopup();
							TrackPopupMenuEx(this.TrayControl.Menu.Handle, 64, point.x, point.y, this.Handle, System.IntPtr.Zero);
							break;
						}
						case NIN_BALLOON_CLICK:
						{
							if(this.TrayControl.BalloonClick != null) this.TrayControl.BalloonClick(this, EventArgs.Empty);
							break;
						}
					}
				}
				else if (m.Msg.Equals(TrayCreatedMessage))
				{
					this.TrayControl.Add();
				}
				else base.DefWndProc (ref m);
			}
		}

		/// <summary>
		/// Balloon ToolTip icon styles.
		/// </summary>
		public enum InformationFlags
		{
			/// <summary>
			/// No icon.
			/// </summary>
			None							= 0x00,

			/// <summary>
			/// 'i' icon.
			/// </summary>
			Info							= 0x01,

			/// <summary>
			/// '!' icon.
			/// </summary>
			Warning							= 0x02,

			/// <summary>
			/// 'x' icon.
			/// </summary>
			Error							= 0x03,
		}

		/// <summary>
		/// Commands for Shell_NotifyIcon function.
		/// </summary>
		private enum Commands
		{
			/// <summary>
			/// Add icon to tray.
			/// </summary>
			Add								= 0x00,

			/// <summary>
			/// Modify icon data.
			/// </summary>
			Modify							= 0x01,

			/// <summary>
			/// Remove icon from tray.
			/// </summary>
			Remove							= 0x02,

			/// <summary>
			/// Set version for Shell32.dll we wants to work with.
			/// </summary>
			Version							= 0x04,
		}

		/// <summary>
		/// Represents tray icon properties.
		/// </summary>
		private enum Flags
		{
			/// <summary>
			/// Get Windows messages.
			/// </summary>
			Message							= 0x01,

			/// <summary>
			/// Icon to show.
			/// </summary>
			Icon							= 0x02,

			/// <summary>
			/// Tip to show.
			/// </summary>
			Tip								= 0x04,

			/// <summary>
			/// Icon state.
			/// </summary>
			State							= 0x08,

			/// <summary>
			/// Information.
			/// </summary>
			Info							= 0x10,
		}

		/// <summary>
		/// tray icon state.
		/// </summary>
		private enum State
		{
			Hidden							= 0x01,
			Shared,
		}

		/// <summary>
		/// Structure with tray object properties (we are using version 5.0 of Shell32.dll).
		/// </summary>
		[StructLayout(LayoutKind.Sequential)]
		private struct NotifyIconData
		{
			/// <summary>
			/// Size of this structure, in bytes.
			/// </summary>
			public System.UInt32 Size;
			
			/// <summary>
			///	Handle to the window that receives notification messages associated with an icon in the taskbar status area. The Shell uses hWnd and uID to identify which icon to operate on when <see href="ms-help://MS.VSCC.2003/MS.MSDNQTR.2003APR.1033/shellcc/platform/shell/reference/structures/notifyicondata.htm">Shell_NotifyIcon</see> is invoked.
			/// </summary>
			public System.IntPtr WndHandle;

			/// <summary>
			/// Application-defined identifier of the taskbar icon. The Shell uses hWnd and uID to identify which icon to operate on when Shell_NotifyIcon is invoked. You can have multiple icons associated with a single hWnd by assigning each a different uID.
			/// </summary>
			public System.UInt32 ID;

			/// <summary>
			/// Flags that indicate which of the other members contain valid data <see cref="SystemTrayControl.Flags"/>.
			/// </summary>
			public SystemTrayControl.Flags Flags;

			/// <summary>
			/// Application-defined message identifier. The system uses this identifier to send notifications to the window identified in hWnd. These notifications are sent when a mouse event occurs in the bounding rectangle of the icon, or when the icon is selected or activated with the keyboard. The wParam parameter of the message contains the identifier of the taskbar icon in which the event occurred. The lParam parameter holds the mouse or keyboard message associated with the event. For example, when the pointer moves over a taskbar icon, lParam is set to <see href="ms-help://MS.VSCC.2003/MS.MSDNQTR.2003APR.1033/shellcc/platform/shell/reference/structures/notifyicondata.htm">WM_MOUSEMOVE</see>. See the <see href="ms-help://MS.VSCC.2003/MS.MSDNQTR.2003APR.1033/shellcc/platform/shell/reference/structures/notifyicondata.htm">Taskbar</see> guide chapter for further discussion.
			/// </summary>
			public System.Int32 CallbackMessage;

			/// <summary>
			/// Handle to the icon to be added, modified, or deleted. To avoid icon distortion, be aware that notification area icons have different levels of support under different versions of Microsoft® Windows®. Windows 95, Windows 98, and Microsoft Windows NT® 4.0 support icons of up to 4 bits per pixel (BPP). Windows Millennium Edition (Windows Me) and Windows 2000 support icons of a color depth up to the current display mode. Windows XP supports icons of up to 32 BPP.
			/// </summary>
			public System.IntPtr IconHandle;

			/// <summary>
			/// Pointer to a null-terminated string with the text for a standard ToolTip. It can have a maximum of 64 characters including the terminating NULL. For Version 5.0 and later, szTip can have a maximum of 128 characters, including the terminating NULL.
			/// </summary>
			[MarshalAs(UnmanagedType.ByValTStr, SizeConst=128)]
			public System.String Tip;

			/// <summary>
			/// State of the icon. There are two flags that can be set independently (<see cref="SystemTrayControl.State"/>).
			/// </summary>
			public SystemTrayControl.State State;

			/// <summary>
			/// A value that specifies which bits of the state member are retrieved or modified. For example, setting this member to <see cref="SystemTrayControl.State.Hidden">State.Hidden</see> causes only the item's hidden state to be retrieved.
			/// </summary>
			public SystemTrayControl.State StateMask;

			/// <summary>
			/// Pointer to a null-terminated string with the text for a balloon ToolTip. It can have a maximum of 255 characters. To remove the ToolTip, set the <see cref="Diacom.APCService.Control.SystemTrayControl.Flags.Info"/>flag in <see cref="SystemTrayControl.Flags"/> and set Info to an empty string.
			/// </summary>
			[MarshalAs(UnmanagedType.ByValTStr, SizeConst=256)]
			public System.String Info;

			/// <summary>
			/// Union of Timeout and Version.
			/// <para>
			/// Timeout: the timeout value, in milliseconds, for a balloon ToolTip. The system enforces minimum and maximum timeout values. Timeout values that are too large are set to the maximum value and values that are too small default to the minimum value. The system minimum and maximum timeout values are currently set at 10 seconds and 30 seconds, respectively.
			/// </para>
			/// <para>
			/// Version: specifies whether the Shell notify icon interface should use Windows 95 or Windows 2000 behavior. This member is only employed when using Shell_NotifyIcon to send a <see cref="SystemTrayControl.Commands.Version"/> message.
			/// </para>
			/// </summary>
			public System.UInt32 TimeoutOrVersion;

			/// <summary>
			/// Pointer to a null-terminated string containing a title for a balloon ToolTip. This title appears in boldface above the text. It can have a maximum of 63 characters.
			/// </summary>
			[MarshalAs(UnmanagedType.ByValTStr, SizeConst=64)]
			public System.String InfoTitle;

			/// <summary>
			/// Flags that can be set to add an icon to a balloon ToolTip. It is placed to the left of the title. If the <see cref="InfoTitle"/> member is zero-length, the icon is not shown.
			/// </summary>
			public SystemTrayControl.InformationFlags InfoFlags;
		}

		private static uint UniqueID = 0;
		private uint _ID = 0;
		/// <summary>
		/// Icon ID (all icons in tray have one).
		/// </summary>
		public uint ID
		{
			get{return this._ID;}
		}

		WinMsgTarget msgTarget;

		/// <summary>
		/// Handle to an icon.
		/// </summary>
		private IntPtr Handle = IntPtr.Zero;

		private ContextMenu _Menu;
		private MethodInfo _OnPopup;
		/// <summary>
		/// Menu that appears on right mouse click.
		/// </summary>
		public ContextMenu Menu
		{
			get { return this._Menu; }
			set	
			{ 
				this._Menu = value; 
				_OnPopup = _Menu.GetType().GetMethod("OnPopup", 
					BindingFlags.NonPublic | BindingFlags.Instance);
			}
		}
		
		/// <summary>
		/// Calls the OnPopup function/Event in a ContextMenu class.
		/// </summary>
		public void OnPopup()
		{
			this._OnPopup.Invoke(_Menu, new Object[] { System.EventArgs.Empty });
		}

		private bool disposed = false;

		private string _Text = "";
		/// <summary>
		/// Text of tooltip.
		/// </summary>
		public string Text
		{
			get { return this._Text; }
			set
			{
				this._Text = value;
				this.Update(); 
			}
		}

		private System.Drawing.Icon _Icon = null;
		/// <summary>
		/// Icon user will see in a tray.
		/// </summary>
		public System.Drawing.Icon Icon
		{
			get { return this._Icon; }
			set 
			{
				this._Icon = value;
				this.Update();
			}
		}


		private bool _Visible = false;
		/// <summary>
		/// Visibility property.
		/// </summary>
		public bool Visible
		{
			get { return this._Visible; }
			set
			{
				this._Visible = value;
				this.Update();
			}
		}

		/// <summary>
		/// Sends a message to the taskbar's status area.
		/// </summary>
		/// <param name="cmd">Variable of type DWORD that specifies the action to be taken (<see cref="Diacom.APCService.Control.SystemTrayControl.Commands"/> enumaration).</param>
		/// <param name="data">Address of a NotifyIconData structure. The content of the structure depends on the value of cmd.</param>
		/// <returns>0 if command is invalid.</returns>
		[DllImport("shell32.Dll")]
		private static extern System.Int32 Shell_NotifyIcon(Commands cmd, ref NotifyIconData data);


		/// <summary>
		/// Updates tray object.
		/// </summary>
		private void Update()
		{
			NotifyIconData data = new NotifyIconData();
			data.Size = ((uint)(Marshal.SizeOf(data)));
			data.WndHandle = this.Handle = msgTarget.Handle;
			data.ID = this.ID;
			data.Flags |= Flags.Icon|Flags.Tip|Flags.State;
			data.IconHandle = this._Icon.Handle;
			data.Tip = this._Text;
			if(!this.Visible) data.State |= State.Hidden;
			data.StateMask |= State.Hidden;
			Shell_NotifyIcon(Commands.Modify, ref data);
		}

		/// <summary>
		/// Removes object from tray.
		/// </summary>
		private void Remove()
		{
			if(this.ID == 0) return;
			NotifyIconData data = new NotifyIconData();
			data.Size = ((uint)(Marshal.SizeOf(data)));
			data.WndHandle = this.Handle;
			data.ID = this.ID;
			Shell_NotifyIcon(Commands.Remove, ref data);
			this._ID = 0;
		}

		/// <summary>
		/// Shows balloon tooltip.
		/// </summary>
		/// <param name="aTitle">Title for tip.</param>
		/// <param name="aText">Text of tip.</param>
		/// <param name="aType">Type of icon.</param>
		/// <param name="aTimeoutInMilliSeconds">Time to show.</param>
		public void ShowBalloon(string aTitle, string aText, InformationFlags aType, int aTimeoutInMilliSeconds)
		{
			if(aTimeoutInMilliSeconds < 0) return;
			NotifyIconData data = new NotifyIconData();
			data.Size = ((uint)(Marshal.SizeOf(data)));
			data.CallbackMessage = this.msgTarget.UserMessage;
			data.WndHandle = this.msgTarget.Handle;
			data.ID = this.ID;
			data.IconHandle = this.Icon.Handle;
			data.Flags |= Flags.Info|Flags.Icon|Flags.Message;
			data.TimeoutOrVersion = ((uint)(aTimeoutInMilliSeconds));
			data.Tip = "Balloon";
			data.InfoTitle = aTitle;
			data.Info = aText;
			data.InfoFlags = aType;
			Shell_NotifyIcon(Commands.Modify, ref data);
		}

		/// <summary>
		/// Adds an Icon to a System Tray.
		/// </summary>
		public void Add()
		{
			// Creating Tray control icon.
			NotifyIconData data = new NotifyIconData();
			data.Size = ((uint)(Marshal.SizeOf(data)));
			data.WndHandle = this.Handle;
			data.ID = this.ID;
			data.CallbackMessage = msgTarget.UserMessage;
			data.Flags |= Flags.Icon|Flags.Message;
			data.IconHandle = this._Icon.Handle;
			data.Tip = this._Text;
			data.TimeoutOrVersion = 1;
			if(!this.Visible) data.State |= State.Hidden;
			data.StateMask |= State.Hidden;
			Shell_NotifyIcon(Commands.Add, ref data);
			Shell_NotifyIcon(Commands.Version, ref data);
		}

		/// <summary>
		/// Creates tray icon.
		/// </summary>
		/// <param name="TrayIcon">Initial Icon or null to display in a tray.</param>
		public SystemTrayControl(System.Drawing.Icon TrayIcon)
		{
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(GetType());
			this._Icon = ((System.Drawing.Icon)(resources.GetObject("Icon.ico")));
			this._ID = ++UniqueID;
			this.msgTarget = new WinMsgTarget(this);
			this.Handle = this.msgTarget.Handle;
			if(TrayIcon != null) this._Icon = TrayIcon;
			Add();
		}

		/// <summary>
		/// Cleans up any resources being used.
		/// </summary>
		protected override void Dispose(bool disposing)
		{
			if(disposed) return;
			disposed = true;
			GC.SuppressFinalize(this);
			if(disposing)
			{
				this.Remove();
			}
			base.Dispose(disposing);
		}
	}
}
