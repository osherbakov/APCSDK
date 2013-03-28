using System;

namespace Diacom
{
	namespace AltiGen
	{
		/// <summary>
		/// AltiLink Plus v.2 protocol commands codes.
		/// </summary>
		internal enum ALPCmdID : int
		{
			/// <summary>
			/// Not a command at all.
			/// </summary>
			NOT_A_COMMAND                          = 0000,

			/// <summary>
			/// Register application.
			/// </summary>
			REGISTER_APPID                         = 1000,

			/// <summary>
			/// Set version ID.
			/// </summary>
			SET_VERSION                            = 1001,

			/// <summary>
			/// Make a call.
			/// </summary>
			MAKECALL                               = 1002,

			/// <summary>
			/// Answer the call.
			/// </summary>
			ANSWER                                 = 1003,

			/// <summary>
			/// Drop (finish) the call.
			/// </summary>
			DROPCALL                               = 1004,

			/// <summary>
			/// Redirect the call.
			/// </summary>
			REDIRECT                               = 1005,

			/// <summary>
			/// Hold line.
			/// </summary>
			HOLD                                   = 1006,

			/// <summary>
			/// Reset the line.
			/// </summary>
			RESET                                  = 1007,

			/// <summary>
			/// Ring the line.
			/// </summary>
			RING                                   = 1008,

			/// <summary>
			/// Play media.
			/// </summary>
			PLAY                                   = 1009,

			/// <summary>
			/// Record media.
			/// </summary>
			RECORD                                 = 1010,

			/// <summary>
			/// Ping line.
			/// </summary>
			PING                                   = 1011,

			/// <summary>
			/// Simulate chars.
			/// </summary>
			SIM_CHARS                              = 1012,

			/// <summary>
			/// Simulate onhook event.
			/// </summary>
			SIM_ONHOOK                             = 1013,

			/// <summary>
			/// Call monitoring.
			/// </summary>
			CALLMONITORING                         = 1014,

			/// <summary>
			/// Realtime call monitoring.
			/// </summary>
			REALTIMEMONITOR                        = 1015,

			/// <summary>
			/// Logon (see <see cref="Diacom.SPLogonType"/>) to server.
			/// </summary>
			LOGON                                  = 1016,

			/// <summary>
			/// Logoff from server.
			/// </summary>
			LOGOFF                                 = 1017,

			/// <summary>
			/// Logon to workgroup.
			/// </summary>
			LOGON_WORKGROUP                        = 1018,

			/// <summary>
			/// Web call setup.
			/// </summary>
			WEBCALL_SETUP                          = 1019,

			/// <summary>
			/// Push URL.
			/// </summary>
			PUSH_URL                               = 1020,

			/// <summary>
			/// Get line information.
			/// </summary>
			GET_LINEINFO                           = 1021,

			/// <summary>
			/// Get line configuration information.
			/// </summary>
			GET_LINECONFIG                         = 1022,

			/// <summary>
			/// Set line configuration information.
			/// </summary>
			SET_LINECONFIG                         = 1023,

			/// <summary>
			/// Get call information.
			/// </summary>
			GET_CALLINFO                           = 1024,

			/// <summary>
			/// Get more information on call.
			/// </summary>
			GET_EXTRACALLINFO                      = 1025,

			/// <summary>
			/// Set user information.
			/// </summary>
			SET_USERDATA                           = 1026,

			/// <summary>
			/// Get system configuration.
			/// </summary>
			GET_SYSCONFIG                          = 1027,

			/// <summary>
			/// Get information on workgroup.
			/// </summary>
			GET_WORKGROUPINFO                      = 1028,

			/// <summary>
			/// Set server time stamp.
			/// </summary>
			GET_SERVERTIMESTAMP                    = 1029,

			/// <summary>
			/// Get agent state.
			/// </summary>
			GET_AGENTSTATE                         = 1030,

			/// <summary>
			/// Set agent state.
			/// </summary>
			SET_AGENTSTATE                         = 1031,

			/// <summary>
			/// Get monitoring entry.
			/// </summary>
			GET_MONITORENTRY                       = 1032,

			/// <summary>
			/// Set monitoring entry.
			/// </summary>
			SET_MONITORENTRY                       = 1033,

			/// <summary>
			/// Get voice mail list ID.
			/// </summary>
			GET_VMDLISTID                          = 1034,

			/// <summary>
			/// Get voice mail list entry.
			/// </summary>
			GET_VMDLISTENTRY                       = 1035,

			/// <summary>
			/// Get voice mail list.
			/// </summary>
			SET_VMDLIST                            = 1036,

			/// <summary>
			/// Get reason code.
			/// </summary>
			GET_REASONCODE                         = 1037,

			/// <summary>
			/// Query CDR - Constant Density Recording.
			/// </summary>
			CDR_QUERYCDR                           = 1038,

			/// <summary>
			/// Log Registered Transfer Module to CDR.
			/// </summary>
			CDR_LOGRTMCALL                         = 1039,

			/// <summary>
			/// Get total number of CDR records.
			/// </summary>
			CDR_GETTOTALCOUNT                      = 1040,

			/// <summary>
			/// Delete CDR record.
			/// </summary>
			CDR_DELETECDR                          = 1041,

			/// <summary>
			/// Start query in CDR.
			/// </summary>
			CDR_QUERYSTART                         = 1042,

			/// <summary>
			/// Next query in CDR.
			/// </summary>
			CDR_QUERYNEXT                          = 1043,

			/// <summary>
			/// Get number of queries in CDR.
			/// </summary>
			CDR_GETQUERYCOUNT                      = 1044,

			/// <summary>
			/// Close query in CDR.
			/// </summary>
			CDR_QUERYCLOSE                         = 1045,

			/// <summary>
			/// Send voice mail.
			/// </summary>
			SEND_VM                                = 1046,

			/// <summary>
			/// DDR responce.
			/// </summary>
			DDR_RESPONSE                           = 1047,

			/// <summary>
			/// Get version ID.
			/// </summary>
			GET_VERSION                            = 1048,

			/// <summary>
			/// Ring SP extension.
			/// </summary>
			APC_RING_EXT                           = 1049,

			/// <summary>
			/// Make SP call.
			/// </summary>
			APC_MAKE_CALL                          = 1050,

			/// <summary>
			/// Drop (finish) SP call.
			/// </summary>
			APC_DROP_CALL                          = 1051,

			/// <summary>
			/// Transfer SP call.
			/// </summary>
			APC_TRANSFER_CALL                      = 1052,

			/// <summary>
			/// Play file on SP line.
			/// </summary>
			APC_PLAY_VOICE                         = 1053,

			/// <summary>
			/// Stop playing file on SP line.
			/// </summary>
			APC_STOP_PLAY_VOICE                    = 1054,

			/// <summary>
			/// Record file on SP line.
			/// </summary>
			APC_RECORD_VOICE                       = 1055,

			/// <summary>
			/// Stop recording file on SP line.
			/// </summary>
			APC_STOP_RECORD_VOICE                  = 1056,

			/// <summary>
			/// Play DTMF sequence on SP line.
			/// </summary>
			APC_PLAY_DTMF                          = 1057,

			/// <summary>
			/// Swith music mode (on/off) on SP line.
			/// </summary>
			APC_SWITCH_MUSIC                       = 1058,

			/// <summary>
			/// Get data from SP line.
			/// </summary>
			APC_GET_DATA                           = 1059,

			/// <summary>
			/// Set data on SP line.
			/// </summary>
			APC_SET_DATA                           = 1060,

			/// <summary>
			/// Connect call on SP line.
			/// </summary>
			APC_CONNECT_CALL                       = 1061,

			/// <summary>
			/// Snatch the line to SP control.
			/// </summary>
			APC_SNATCH_LINE                        = 1062,

			/// <summary>
			/// Play files on SP line.
			/// </summary>
			APC_PLAY_FILES                         = 1063,

			/// <summary>
			/// Set the rejection timeout.
			/// </summary>
			APC_CALL_REJECT_SET                    = 1064,

			/// <summary>
			/// Answer the incoming call.
			/// </summary>
			APC_ANSWER_CALL						   = 1065,

			/// <summary>
			/// Reject the incoming call.
			/// </summary>
			APC_REJECT_CALL						   = 1066,

			/// <summary>
			/// Pass the incoming call.
			/// </summary>
			APC_PASS_CALL						   = 1067,

			/// <summary>
			/// Forced workgroup logoff.
			/// </summary>
			FORCE_WORKGROUP_LOGOFF                 = 1070,

			/// <summary>
			/// Query to database.
			/// </summary>
			ALTIDB_QUERY                           = 1071,

			/// <summary>
			/// Get data from database.
			/// </summary>
			ALTIDB_GETDATA                         = 1072,

			/// <summary>
			/// Close query to database.
			/// </summary>
			ALTIDB_QUERYCLOSE                      = 1073,

			/// <summary>
			/// Get value from database.
			/// </summary>
			ALTIDB_GETVALUE                        = 1074,

			/// <summary>
			/// Voice recording.
			/// </summary>
			VOICE_RECORD                           = 1075,

			/// <summary>
			/// Unregister application.
			/// </summary>
			UNREGISTER_APPID                       = 1076,

			/// <summary>
			/// Pickup the call.
			/// </summary>
			CALLPICKUP                             = 1077,

			/// <summary>
			/// Get data in realtime mode.
			/// </summary>
			GETREALTIMEDATA                        = 1078,

			/// <summary>
			/// Voice mail action.
			/// </summary>
			VMACTION                               = 1079,

			/// <summary>
			/// Storage.
			/// </summary>
			STORAGE                                = 1086,

			/// <summary>
			/// Request offhook.
			/// </summary>
			REQUEST_OFFHOOK                        = 1087,

			/// <summary>
			/// Start call recording.
			/// </summary>
			START_CALLRECORD                       = 1090,

			/// <summary>
			/// Stop call recording.
			/// </summary>
			STOP_CALLRECORD                        = 1091,

			/// <summary>
			/// Pause call recording.
			/// </summary>
			PAUSE_CALLRECORD                       = 1092,

			/// <summary>
			/// Application configuration request.
			/// </summary>
			APPCFG_REQUEST                         = 1119,

			/// <summary>
			/// Application configuration responce.
			/// </summary>
			APPCFG_RESPONSE                        = 1120
		}

		/// <summary>
		/// AltiLink Plus v.2 protocol possible data request types.
		/// </summary>
		internal enum ALPInfoType
		{
			APC_DATATYPE_IVR			= 0,
			APC_DATATYPE_FORM			= 1,
			APC_DATATYPE_USER			= 2,
			APC_DATATYPE_URL			= 3,
			APC_DATATYPE_URLHISTORY		= 4,
			APC_DATATYPE_HISTORYDATA	= 5	
		}

		/// <summary>
		/// AltiLink Plus v.2 protocol possible call types.
		/// </summary>
		internal enum ALPCallType
		{
			/// <summary>
			/// PSTN - Public Switched Telephone Network.
			/// </summary>
			PSTN                                   = 0001,

			/// <summary>
			/// IP.
			/// </summary>
			IP                                     = 0002,

			/// <summary>
			/// Voice mail.
			/// </summary>
			VM                                     = 0004,

			/// <summary>
			/// AutoAttendant.
			/// </summary>
			AA                                     = 0008
		}

		/// <summary>
		/// AltiLink Plus v.2 protocol possible ring modes.
		/// </summary>
		internal enum ALPRingMode
		{
			/// <summary>
			/// Outside operator.
			/// </summary>
			OUTSIDE_OPERATOR                       = 0,

			/// <summary>
			/// Inside operator.
			/// </summary>
			INSIDE_OPERATOR                        = 1,

			/// <summary>
			/// External mode.
			/// </summary>
			EXTERNAL                               = 2,

			/// <summary>
			/// Internal mode.
			/// </summary>
			INTERNAL                               = 3
		}

		/// <summary>
		/// AltiLink Plus v.2 protocol events codes.
		/// </summary>
		internal enum ALPEvID
		{
			/// <summary>
			/// Line reseted.
			/// </summary>
			RESET                                  = 5000,

			/// <summary>
			/// Ring on line.
			/// </summary>
			RING                                   = 5001,

			/// <summary>
			/// Ring is back on line.
			/// </summary>
			RING_BACK                              = 5002,

			/// <summary>
			/// State changed.
			/// </summary>
			STCHG                                  = 5003,

			/// <summary>
			/// Button pressed.
			/// </summary>
			DIGIT                                  = 5004,

			/// <summary>
			/// Message from line.
			/// </summary>
			MWI                                    = 5005,

			/// <summary>
			/// Line parked.
			/// </summary>
			PARK                                   = 5006,

			/// <summary>
			/// Call is droped on line.
			/// </summary>
			CALLDROP                               = 5007,

			/// <summary>
			/// Query is empty on line.
			/// </summary>
			QEMPTY                                 = 5008,

			/// <summary>
			/// Reinitialisation of the system.
			/// </summary>
			SYSREINIT                              = 5009,

			/// <summary>
			/// Incoming call queued.
			/// </summary>
			INQUEUE                                = 5010,

			/// <summary>
			/// Outgoing call queued.
			/// </summary>
			OUTQUEUE                               = 5011,

			/// <summary>
			/// Reinitialisation of the line.
			/// </summary>
			LINEREINIT                             = 5012,

			/// <summary>
			/// IP call started.
			/// </summary>
			IPCALL_STARTED                         = 5013,

			/// <summary>
			/// UTL pushed.
			/// </summary>
			PUSHED_URL                             = 5014,

			/// <summary>
			/// Agent state changed.
			/// </summary>
			AGENT_STATE_CHG                        = 5015,

			/// <summary>
			/// Registered Transfer Module data.
			/// </summary>
			RTM_DATA                               = 5016,

			/// <summary>
			/// Configuration of line changed.
			/// </summary>
			CONFIG_CHG                             = 5017,

			/// <summary>
			/// System configuration changed.
			/// </summary>
			SYSCONFIG_CHG                          = 5018,

			/// <summary>
			/// DDR request.
			/// </summary>
			DDR_REQUEST                            = 5019,

			/// <summary>
			/// Voice message proceed.
			/// </summary>
			OC_EXPROC                              = 5020,

			/// <summary>
			/// SP call present.
			/// </summary>
			APC_CALLPRESENT                        = 5021,

			/// <summary>
			/// Button was pressed in line under SP control.
			/// </summary>
			APC_DIGIT                              = 5022,

			/// <summary>
			///  Status of the line under SP control changed.
			/// </summary>
			APC_STATUS                             = 5023,

			/// <summary>
			/// Ring is back to SP. 
			/// </summary>
			APC_RINGBACK                           = 5024,

			/// <summary>
			/// Call is dropped (finished) on line under SP control.
			/// </summary>
			APC_CALLDROP                           = 5025,

			/// <summary>
			/// Reserved for SP.
			/// </summary>
			APC_RESERVE1                           = 5026,

			/// <summary>
			/// Reserved for SP.
			/// </summary>
			APC_RESERVE2                           = 5027,

			/// <summary>
			/// Reserved for SP.
			/// </summary>
			APC_RESERVE3                           = 5028,

			/// <summary>
			/// Reserved for SP.
			/// </summary>
			APC_RESERVE4                           = 5029,

			/// <summary>
			/// System configuration changed (second variant).
			/// </summary>
			CONFIG_CHG2                            = 5030,

			/// <summary>
			/// Tone detected.
			/// </summary>
			TONE                                   = 5032,

			/// <summary>
			/// Vioce recording.
			/// </summary>
			VR                                     = 5040,

			/// <summary>
			/// Server information changed.
			/// </summary>
			SERVERINFO                             = 5042,

			/// <summary>
			/// Service information changed.
			/// </summary>
			SERVICEINFO                            = 5043,

			/// <summary>
			/// Application configuration request.
			/// </summary>
			APPCFG_REQUEST                         = 5046,

			/// <summary>
			/// Line is connected by SP.
			/// </summary>
			CONNECT_BY_SP                         = 5047
		}

		/// <summary>
		/// AltiLink Plus v.2 protocol responses on commands codes.
		/// </summary>
		internal enum ALPRespID
		{
			/// <summary>
			/// All is OK.
			/// </summary>
			OK                                     = 0000,

			/// <summary>
			/// ID is invalid.
			/// </summary>
			INVALID_ID                             = 3001,

			/// <summary>
			/// Command is invalid.
			/// </summary>
			INVALID_CMD                            = 3002,

			/// <summary>
			/// Parameter is invalid.
			/// </summary>
			INVALID_PARAM                          = 3003,

			/// <summary>
			/// PWD is invalid.
			/// </summary>
			INVALID_PWD                            = 3004,

			/// <summary>
			/// State is invalid.
			/// </summary>
			INVALID_STATE                          = 3005,

			/// <summary>
			/// Application ID is invalid.
			/// </summary>
			INVALID_APPID                          = 3006,

			/// <summary>
			/// Data is invalid.
			/// </summary>
			INVALID_DATA                           = 3007,

			/// <summary>
			/// Serial number is invalid.
			/// </summary>
			INVALID_SERIAL                         = 3008,

			/// <summary>
			/// System error.
			/// </summary>
			SYSTEM_ERROR                           = 3009,

			/// <summary>
			/// Protocol error.
			/// </summary>
			PROTOCOL_ERROR                         = 3010,

			/// <summary>
			/// Port is out of reach.
			/// </summary>
			PORT_OUTOFREACH                        = 3011,

			/// <summary>
			/// Application has full licence.
			/// </summary>
			APPID_LICENSE_FULL                     = 3012,

			/// <summary>
			/// User is not logged in system.
			/// </summary>
			NOT_LOGON                              = 3013,

			/// <summary>
			/// User already logged in system.
			/// </summary>
			LOGON_ALREADY                          = 3014,

			/// <summary>
			/// Web call rejected.
			/// </summary>
			WEBCALL_REJECT                         = 3015,

			/// <summary>
			/// Echo.
			/// </summary>
			ECHO                                   = 3016,

			/// <summary>
			/// Serial exists in system.
			/// </summary>
			SERIAL_EXIST                           = 3017,

			/// <summary>
			/// Insert is duplicated.
			/// </summary>
			INSERT_DUPLICATE                       = 3018,

			/// <summary>
			/// Calls database is overfulled.
			/// </summary>
			CALLDB_FULL                            = 3019,

			/// <summary>
			/// Error to link to database.
			/// </summary>
			DBLINK_ERROR                           = 3020,

			/// <summary>
			/// No record was selected.
			/// </summary>
			NO_RECORD_SELECTED                     = 3021,

			/// <summary>
			/// CDR query data.
			/// </summary>
			CDRQUERY_DATA                          = 3022,

			/// <summary>
			/// Application with this ID is already registered.
			/// </summary>
			APPID_REGISTERED                       = 3023,

			/// <summary>
			/// End of list.
			/// </summary>
			LIST_END                               = 3024,

			/// <summary>
			/// logon failed.
			/// </summary>
			LOGON_FAILED                           = 3025,

			/// <summary>
			/// Call abandoned.
			/// </summary>
			CALL_ABANDONED                         = 3026,

			/// <summary>
			/// Maximum number of connections to database reached.
			/// </summary>
			MAXDBCON_REACHED                       = 3027,

			/// <summary>
			/// Buckup is in processing.
			/// </summary>
			BACKUP_INPROCESS                       = 3028,

			/// <summary>
			/// SQL temporary database is overfulled.
			/// </summary>
			SQLTEMPDB_FULL                         = 3029,

			/// <summary>
			/// No free session.
			/// </summary>
			NO_MORE_SESSION                        = 3030,

			/// <summary>
			/// Option pack is not installed.
			/// </summary>
			OPTIONPACK_NOT_INSTALLED               = 3031,

			/// <summary>
			/// IP extension not enabled.
			/// </summary>
			IPEXT_NOTENABLED                       = 3032,

			/// <summary>
			/// IP extension is fixed.
			/// </summary>
			IPEXT_IPFIXED                          = 3033,

			/// <summary>
			/// Line is busy.
			/// </summary>
			LINEBUSY                               = 3034,

			/// <summary>
			/// Invalid access control code.
			/// </summary>
			INVALID_ACCODE                         = 3035,

			/// <summary>
			/// Invalid SP controller.
			/// </summary>
			APC_INVALID_CONTROLLER                 = 3036,

			/// <summary>
			/// Extension under SP control is busy.
			/// </summary>
			APC_EXTENSION_BUSY                     = 3037,

			/// <summary>
			/// Access denied.
			/// </summary>
			ACCESSDENY                             = 3038,

			/// <summary>
			/// No basic license.
			/// </summary>
			NO_BAS_LICENSE                         = 3039,

			/// <summary>
			/// No VRD license.
			/// </summary>
			NO_VRD_LICENSE                         = 3040,

			/// <summary>
			/// No SP license.
			/// </summary>
			NO_APC_LICENSE                         = 3041,

			/// <summary>
			/// No ECDR license.
			/// </summary>
			NO_ECDR_LICENSE                        = 3042,

			/// <summary>
			/// No AltiCRT license.
			/// </summary>
			NO_ACRT_LICENSE                        = 3043,

			/// <summary>
			/// No CSLE license.
			/// </summary>
			NO_CSLE_LICENSE                        = 3044,

			/// <summary>
			/// No AltiWeb license.
			/// </summary>
			NO_AWEB_LICENSE                        = 3045,

			/// <summary>
			/// No AltiView license.
			/// </summary>
			NO_AVIEW_LICENSE                       = 3046,

			/// <summary>
			/// No AltiWorkgroup license.
			/// </summary>
			NO_AWRG_LICENSE                        = 3047,

			/// <summary>
			/// Application is not registered.
			/// </summary>
			NOT_REGISTERED                         = 3048,

			/// <summary>
			/// Application is registered alredy.
			/// </summary>
			ALREADY_REGISTERED                     = 3049,

			/// <summary>
			/// There are no more console.
			/// </summary>
			NO_MORE_CONSOLE                        = 3050,

			/// <summary>
			/// Login is blocked.
			/// </summary>
			LOGIN_ISBLOCKED                        = 3051,

			/// <summary>
			/// Login is OK (unsecure password).
			/// </summary>
			LOGIN_OK_UNSECURE_PWD                  = 3052,

			/// <summary>
			/// No IP phone license.
			/// </summary>
			NO_IPPHONE_LICENSE                     = 3053,

			/// <summary>
			/// No priviledge.
			/// </summary>
			NO_PRIVILEGE                           = 3056,

			/// <summary>
			/// Version mismatch warning.
			/// </summary>
			WARN_VERSIONMISMATCH                   = 3057,

			/// <summary>
			/// Version mismatch error.
			/// </summary>
			ERR_VERSIONMISMATCH                    = 3058,

			/// <summary>
			/// System disabled voice recording.
			/// </summary>
			VR_SYSTEM_DISABLE                      = 3100,

			/// <summary>
			/// Voice recording in progress.
			/// </summary>
			VR_RECORDING_IN_PROGRESS               = 3101,

			/// <summary>
			/// System can't open file for voice recording.
			/// </summary>
			VR_FILE_CANNOT_OPEN                    = 3102,

			/// <summary>
			/// Disk is full for voice recording.
			/// </summary>
			VR_DISK_FULL                           = 3103,

			/// <summary>
			/// Voice recording sevice is not installed.
			/// </summary>
			VR_VRSERVICE_NOT_INSTALLED             = 3104,

			/// <summary>
			/// Mail box for voice recording is overfulled.
			/// </summary>
			VR_MAILBOX_IS_FULL                     = 3105,

			/// <summary>
			/// Invalid voice recording type.
			/// </summary>
			VR_INVALID_VRTYPE                      = 3106,

			/// <summary>
			/// Voice recording for this extension is disabled.
			/// </summary>
			VR_EXT_DISABLE                         = 3107,

			/// <summary>
			/// Voice recording is not supported.
			/// </summary>
			VR_NOT_SUPPORTED                       = 3108,

			/// <summary>
			/// Voice recording not started.
			/// </summary>
			VR_NOT_START                           = 3109,

			/// <summary>
			/// Voice recording processes maximum riched.
			/// </summary>
			VR_MAX                                 = 3120,

			/// <summary>
			/// Error (reason is undefined).
			/// </summary>
			ERROR                                  = 3998,

			/// <summary>
			/// Unknown error.
			/// </summary>
			UNKNOWN                                = 3999
		}

		/// <summary>
		/// Line states.
		/// </summary>
		internal enum ALPLineState
		{
			/// <summary>
			/// Line is idle.
			/// </summary>
			IDLE                                   = 0x0000,

			/// <summary>
			/// Line is in Extensible Authentication Protocol mode.
			/// </summary>
			EAP                                    = 0x0001,

			/// <summary>
			/// Line is connected.
			/// </summary>
			CONNECTED                              = 0x0002,

			/// <summary>
			/// Line is in use.
			/// </summary>
			INUSE                                  = 0x0003,

			/// <summary>
			/// Line is offhook.
			/// </summary>
			OFFHOOK                                = 0x0004,

			/// <summary>
			/// Call pending on line.
			/// </summary>
			CALLPENDING                            = 0x0005,

			/// <summary>
			/// Ring is back on line.
			/// </summary>
			RINGBACK                               = 0x0006,

			/// <summary>
			/// Line is in conferencing mode.
			/// </summary>
			CONFERENCING                           = 0x0007,

			/// <summary>
			/// Line is connected with autoateddant.
			/// </summary>
			AA                                     = 0x0008,

			/// <summary>
			/// Line is in voice message recording mode.
			/// </summary>
			VM                                     = 0x0009,

			/// <summary>
			/// Line is hold.
			/// </summary>
			HOLD                                   = 0x000A,

			/// <summary>
			/// Hold pending on line.
			/// </summary>
			HOLDPENDING                            = 0x000B,

			/// <summary>
			/// Line is proceeding.
			/// </summary>
			PROCEEDING                             = 0x000C,

			/// <summary>
			/// Line is disconnected.
			/// </summary>
			DISCONNECT                             = 0x000D,

			/// <summary>
			/// Error on line.
			/// </summary>
			ERROR                                  = 0x000E,

			/// <summary>
			/// Ring on line.
			/// </summary>
			RING                                   = 0x000F,

			/// <summary>
			/// Line is parked.
			/// </summary>
			PARK                                   = 0x0010,

			/// <summary>
			/// Line is softly offhooked.
			/// </summary>
			SOFTOFFHOOK                            = 0x0011,

			/// <summary>
			/// Music on hold on line.
			/// </summary>
			MUSICONHOLD                            = 0x0012,

			/// <summary>
			/// Tranfering waiting for onhook on line.
			/// </summary>
			XFER_WAITFORONHOOK                     = 0x0013,

			/// <summary>
			/// Transfering ring is back.
			/// </summary>
			XFER_RINGBACK                          = 0x0014,

			/// <summary>
			/// Line is busy.
			/// </summary>
			BUSY                                   = 0x0015,

			/// <summary>
			/// Line is playing file.
			/// </summary>
			PLAY                                   = 0x0016,

			/// <summary>
			/// Line is recording file.
			/// </summary>
			RECORD                                 = 0x0017,

			/// <summary>
			/// Line is under SP control.
			/// </summary>
			APC                                    = 0x0018,

			/// <summary>
			/// Voice recording started on line.
			/// </summary>
			VOICE_RECORD_START                     = 0x0019,

			/// <summary>
			/// Voice recording stopped on line.
			/// </summary>
			VOICE_RECORD_STOP                      = 0x001A,

			/// <summary>
			/// Line is monitored silently.
			/// </summary>
			SILENTMONITOR                          = 0x001B,

			/// <summary>
			/// Barge in on line.
			/// </summary>
			BARGEIN                                = 0x001C,

			/// <summary>
			/// Line is holding AltiView conference.
			/// </summary>
			HOLD_CONFERENCE                        = 0x001F,

			/// <summary>
			/// Line is idle for offhook when hand free/dialtone disable
			/// </summary>
			HFDTD_IDLE                             = 0x0020,

			/// <summary>
			/// Play call finished on line.
			/// </summary>
			PLAY_CALL_STOP                         = 0x0021,

			/// <summary>
			/// Line is playing call.
			/// </summary>
			PLAY_CALL                              = 0x0022,

			/// <summary>
			/// New line added.
			/// </summary>
			LINE_ADD,

			/// <summary>
			/// Line not accessible any more.
			/// </summary>
			LINE_REMOVE
		}

		/// <summary>
		/// Possible codes of system configuration changes.
		/// </summary>
		internal enum SystemConfigChangeCodes
		{
			/// <summary>
			/// Number plan changed.
			/// </summary>
			NUMBERPLAN                             = 0x0000,

			/// <summary>
			/// Operator changed.
			/// </summary>
			OPERATOR                               = 0x0001,

			/// <summary>
			/// Menu names changed.
			/// </summary>
			MENUNAMES                              = 0x0002,

			/// <summary>
			/// Dial method changed.
			/// </summary>
			DIALMETHOD                             = 0x0004,

			/// <summary>
			/// Speed dial changed.
			/// </summary>
			SPEEDDIAL                              = 0x0008,

			/// <summary>
			/// Line is removed from system.
			/// </summary>
			LINEREMOVE                             = 0x0010,

			/// <summary>
			/// Line added to system.
			/// </summary>
			LINEADD                                = 0x0020,

			/// <summary>
			/// Workgroup member changed.
			/// </summary>
			WGMEMBERCHG                            = 0x0040,

			/// <summary>
			/// SQL server changed.
			/// </summary>
			SQLSERVER                              = 0x0080,

			/// <summary>
			/// Voice mail list changed.
			/// </summary>
			VMDLISTCHG                             = 0x0100,

			/// <summary>
			/// Reason code changed.
			/// </summary>
			REASONCODE                             = 0x0200,

			/// <summary>
			/// System ID changed.
			/// </summary>
			SYSTEMID                               = 0x0400,

			/// <summary>
			/// Area Communication Controller changed.
			/// </summary>
			ACC                                    = 0x0800,

			/// <summary>
			/// Paging overhead detected.
			/// </summary>
			OVERHEADPAGING                         = 0x1000
		}
	} // end of AltiGen namespace.
}
