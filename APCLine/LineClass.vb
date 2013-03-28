Option Explicit On 
Option Strict On

Namespace Diacom

    ''' <summary>
    ''' APC line state.
    ''' </summary>
    <Serializable()> _
    Public Enum APCLineState

        ''' <summary>
        ''' Line is idle.
        ''' </summary>
        IDLE

        ''' <summary>
        ''' EAP line state.
        ''' </summary>
        EAP

        ''' <summary>
        ''' Line is connected.
        ''' </summary>
        CONNECTED

        ''' <summary>
        ''' Line is in use.
        ''' </summary>
        INUSE

        ''' <summary>
        ''' Offhook.
        ''' </summary>
        OFFHOOK

        ''' <summary>
        ''' Pending call is on the line.
        ''' </summary>
        CALLPENDING

        ''' <summary>
        ''' Ringback state.
        ''' </summary>
        RINGBACK

        ''' <summary>
        ''' Conference on the line.
        ''' </summary>
        CONFERENCING

        ''' <summary>
        ''' Autoateddant state.
        ''' </summary>
        AA

        ''' <summary>
        ''' Voice mail state.
        ''' </summary>
        VM

        ''' <summary>
        ''' Line is on hold.
        ''' </summary>
        HOLD

        ''' <summary>
        ''' On hold is pending on the line.
        ''' </summary>
        HOLDPENDING

        ''' <summary>
        ''' Line is porceeding something.
        ''' </summary>
        PROCEEDING

        ''' <summary>
        ''' Line disconnects.
        ''' </summary>
        DISCONNECT

        ''' <summary>
        ''' Error is on the line.
        ''' </summary>
        [ERROR]

        ''' <summary>
        ''' Ring is on the line.
        ''' </summary>
        RING

        ''' <summary>
        ''' Line is parked.
        ''' </summary>
        PARK

        ''' <summary>
        ''' Softoffhook.
        ''' </summary>
        SOFTOFFHOOK

        ''' <summary>
        ''' Line in in music on hold state.
        ''' </summary>
        MUSICONHOLD

        ''' <summary>
        ''' Line is waiting for onhook for XFER.
        ''' </summary>
        XFER_WAITFORONHOOK

        ''' <summary>
        ''' Ringback on line for XFER.
        ''' </summary>
        XFER_RINGBACK

        ''' <summary>
        ''' Line is busy.
        ''' </summary>
        BUSY

        ''' <summary>
        ''' Something is playing on the line.
        ''' </summary>
        PLAY

        ''' <summary>
        ''' Something is recording on the line.
        ''' </summary>
        RECORD

        ''' <summary>
        ''' Line is under APC control.
        ''' </summary>
        APC

        ''' <summary>
        ''' Voice recording started on the line.
        ''' </summary>
        VOICE_RECORD_START

        ''' <summary>
        ''' Voice recording ended on the line.
        ''' </summary>
        VOICE_RECORD_STOP

        ''' <summary>
        ''' Silent monitor on the line.
        ''' </summary>
        SILENTMONITOR

        ''' <summary>
        ''' Line is barged in.
        ''' </summary>
        BARGEIN

        ''' <summary>
        ''' Conference is held on the line (for conference mode).
        ''' </summary>
        HOLD_CONFERENCE

        ''' <summary>
        ''' Line is idle for offhook when hands free / dialtone disabled.
        ''' </summary>
        HFDTD_IDLE

        ''' <summary>
        ''' Playing call is finished on the line.
        ''' </summary>
        PLAY_CALL_STOP

        ''' <summary>
        ''' Line is playing call.
        ''' </summary>
        PLAY_CALL

        ''' <summary>
        ''' Line is added to the system.
        ''' </summary>
        LINE_ADD

        ''' <summary>
        ''' Line is removed from the system.
        ''' </summary>
        LINE_REMOVE
    End Enum

    ''' <summary>
    ''' Describes a common information about a line shared by all components of the APCSDK.
    ''' </summary>
    <Serializable()> _
    Public Class LineClass
        Implements ICloneable

        ''' <summary>
        ''' Line identificator (ID) - unique object that identifies the line.
        ''' </summary>
        Public LineID As Object

        ''' <summary>
        ''' Current line status/state (see <see cref="APCLineState"/> enumeration).
        ''' </summary>
        Public LineStatus As APCLineState

        ''' <summary>
        ''' DID number - the CO supplied DID.
        ''' </summary>
        Public DIDNumber As String

        ''' <summary>
        ''' CID number - the CO supplied CID.
        ''' </summary>
        Public CIDNumber As String

        ''' <summary>
        ''' DNIS number - the CO supplied DNIS.
        ''' </summary>
        Public DNISNumber As String

        ''' <summary>
        ''' User number.
        ''' </summary>
        Public UserNumber As String

        ''' <summary>
        ''' Call number - the number that is currently called.
        ''' </summary>
        Public CallNumber As String

        ''' <summary>
        ''' Line name - the name (if any) assigned to that line.
        ''' </summary>
        Public LineName As String

        ''' <summary>
        ''' DID name - the CO supplied DID name.
        ''' </summary>
        Public DIDName As String

        ''' <summary>
        ''' CID name - the CO supplied CID name.
        ''' </summary>
        Public CIDName As String

        ''' <summary>
        ''' DNIS name  - the CO supplied DNIS name.
        ''' </summary>
        Public DNISName As String

        ''' <summary>
        ''' User name.
        ''' </summary>
        Public UserName As String

        ''' <summary>
        ''' Call name - name that is currently called.
        ''' </summary>
        Public CallName As String

        ''' <summary>
        ''' Line port.
        ''' </summary>
        Public LinePort As String

        ''' <summary>
        ''' Line number - the assigned line number.
        ''' </summary>
        Public LineNumber As String

        ''' <summary>
        ''' Line access code - the code that this line can be accessed.
        ''' </summary>
        Public LineAccessCode As String

        ''' <summary>
        ''' Line type.
        ''' </summary>
        Public LineType As String

        ''' <summary>
        ''' Creates a new object that is a shallow copy of the current <see cref="LineClass"/> instance.
        ''' </summary>
        ''' <returns>A new object that is a copy of this instance.</returns>
        Public Function Clone() As Object Implements ICloneable.Clone
            Return Me.MemberwiseClone()
        End Function

        ''' <summary>
        ''' Default class constructor.
        ''' </summary>
        ''' <remarks>
        ''' <para>All the fields of the class will have default (<c>Nothing</c> in most cases) values.</para>
        ''' </remarks>
        Public Sub New()
            Me.CallName = String.Empty
            Me.CallNumber = String.Empty
            Me.CIDName = String.Empty
            Me.CIDNumber = String.Empty
            Me.DIDName = String.Empty
            Me.DIDNumber = String.Empty
            Me.DNISName = String.Empty
            Me.DNISNumber = String.Empty
            Me.LineAccessCode = String.Empty
            Me.LineID = 0
            Me.LineName = String.Empty
            Me.LineNumber = String.Empty
            Me.LinePort = String.Empty
            Me.LineStatus = APCLineState.IDLE
            Me.LineType = String.Empty
            Me.UserName = String.Empty
            Me.UserNumber = String.Empty
        End Sub
    End Class
End Namespace
