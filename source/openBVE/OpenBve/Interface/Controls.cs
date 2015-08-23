using System;
using System.Globalization;
using VirtualKeys = OpenBveApi.Runtime.VirtualKeys;
using Scancode = SDL2.SDL.SDL_Scancode;
using Keycode = SDL2.SDL.SDL_Keycode;
namespace OpenBve
{
	internal static class Controls
	{
		/// <remarks>OpenBve.Controls.SecurityToVirtualKey function requires the naming of security commands to be Security+short code (SecurityS,SecurityA1,...).</remarks>
		internal enum Command {
			None = 0,
			PowerIncrease, PowerDecrease, PowerHalfAxis, PowerFullAxis,
			BrakeIncrease, BrakeDecrease, BrakeEmergency, BrakeHalfAxis, BrakeFullAxis,
			SinglePower, SingleNeutral, SingleBrake, SingleEmergency, SingleFullAxis,
			ReverserForward, ReverserBackward, ReverserFullAxis,
			DoorsLeft, DoorsRight,
			HornPrimary, HornSecondary, HornMusic,
			DeviceConstSpeed,
			SecurityS, SecurityA1, SecurityA2, SecurityB1, SecurityB2, SecurityC1, SecurityC2,
			SecurityD, SecurityE, SecurityF, SecurityG, SecurityH, SecurityI, SecurityJ, SecurityK, SecurityL,
			SecurityM, SecurityN, SecurityO, SecurityP, SecurityQ, SecurityR, SecurityT, SecurityU, SecurityV,
			SecurityW, SecurityX, SecurityY, SecurityZ,
			CameraInterior, CameraExterior, CameraTrack, CameraFlyBy,
			CameraMoveForward, CameraMoveBackward, CameraMoveLeft, CameraMoveRight, CameraMoveUp, CameraMoveDown,
			CameraRotateLeft, CameraRotateRight, CameraRotateUp, CameraRotateDown, CameraRotateCCW, CameraRotateCW,
			CameraZoomIn, CameraZoomOut, CameraPreviousPOI, CameraNextPOI, CameraReset, CameraRestriction,
			TimetableToggle, TimetableUp, TimetableDown,
			MiscClock, MiscSpeed, MiscFps, MiscAI, MiscInterfaceMode, MiscBackfaceCulling, MiscCPUMode,
			MiscTimeFactor, MiscPause, MiscMute, MiscFullscreen, MiscQuit,
			MenuActivate, MenuUp, MenuDown, MenuEnter, MenuBack,
			DebugWireframe, DebugNormals, DebugBrakeSystems
		}
		/// <summary>
		/// Converts the specified security command to a virtual key.
		/// </summary>
		/// <returns>Virtual key for plugins.</returns>
		/// <param name="cmd">Security command. If this isn't security command, ArgumentException will be thrown.</param>
		internal static VirtualKeys SecurityToVirtualKey(Command cmd){
			string cmdname = Enum.GetName(typeof(Command),cmd);
			if (!cmdname.StartsWith("Security", StringComparison.InvariantCulture))
				throw new ArgumentException("Command is not a security command.","cmd");
			string ending = cmdname.Substring(8).ToUpperInvariant();
			VirtualKeys key;
			if (!Enum.TryParse(ending, out key))
				throw new ArgumentException("VirtualKeys does not contain following security key: " + ending, "cmd");
			return key;
		}
		internal enum CommandType { Digital, AnalogHalf, AnalogFull }
		internal struct CommandInfo {
			internal readonly Command Command;
			internal readonly CommandType Type;
			internal readonly string Name;
			internal string Description;
			internal CommandInfo(Command Command, CommandType Type, string Name) {
				this.Command = Command;
				this.Type = Type;
				this.Name = Name;
				this.Description = "N/A";
			}
		}
		// key infos
		internal struct KeyInfo {
			internal readonly Scancode Scancode;
			internal readonly int StorageCode;
			internal readonly string Name;
			internal string Description;
			internal KeyInfo(Scancode Key, SDL1_Keycode storage, string Name, string Description) {
				this.Scancode = Key;
				this.Name = Name;
				this.Description = Description;
				this.StorageCode = (int)storage;
			}
		}
		internal static KeyInfo[] Keys = new KeyInfo[] {
			new KeyInfo(Scancode.SDL_SCANCODE_ESCAPE,SDL1_Keycode.SDLK_ESCAPE, "ESCAPE", "Escape"),
			new KeyInfo(Scancode.SDL_SCANCODE_F1,SDL1_Keycode.SDLK_F1, "F1", "F1"),
			new KeyInfo(Scancode.SDL_SCANCODE_F2,SDL1_Keycode.SDLK_F2, "F2", "F2"),
			new KeyInfo(Scancode.SDL_SCANCODE_F3,SDL1_Keycode.SDLK_F3, "F3", "F3"),
			new KeyInfo(Scancode.SDL_SCANCODE_F4,SDL1_Keycode.SDLK_F4, "F4", "F4"),
			new KeyInfo(Scancode.SDL_SCANCODE_F5,SDL1_Keycode.SDLK_F5, "F5", "F5"),
			new KeyInfo(Scancode.SDL_SCANCODE_F6,SDL1_Keycode.SDLK_F6, "F6", "F6"),
			new KeyInfo(Scancode.SDL_SCANCODE_F7,SDL1_Keycode.SDLK_F7, "F7", "F7"),
			new KeyInfo(Scancode.SDL_SCANCODE_F8,SDL1_Keycode.SDLK_F8, "F8", "F8"),
			new KeyInfo(Scancode.SDL_SCANCODE_F9,SDL1_Keycode.SDLK_F9, "F9", "F9"),
			new KeyInfo(Scancode.SDL_SCANCODE_F10,SDL1_Keycode.SDLK_F10, "F10", "F10"),
			new KeyInfo(Scancode.SDL_SCANCODE_F11,SDL1_Keycode.SDLK_F11, "F11", "F11"),
			new KeyInfo(Scancode.SDL_SCANCODE_F12,SDL1_Keycode.SDLK_F12, "F12", "F12"),
			new KeyInfo(Scancode.SDL_SCANCODE_F13,SDL1_Keycode.SDLK_F13, "F13", "F13"),
			new KeyInfo(Scancode.SDL_SCANCODE_F14,SDL1_Keycode.SDLK_F14, "F14", "F14"),
			new KeyInfo(Scancode.SDL_SCANCODE_F15,SDL1_Keycode.SDLK_F15, "F15", "F15"),

			new KeyInfo(Scancode.SDL_SCANCODE_GRAVE,SDL1_Keycode.SDLK_BACKQUOTE, "BACKQUOTE", "Backquote"),
			new KeyInfo(Scancode.SDL_SCANCODE_1,SDL1_Keycode.SDLK_1, "1", "1"),
			new KeyInfo(Scancode.SDL_SCANCODE_2,SDL1_Keycode.SDLK_2, "2", "2"),
			new KeyInfo(Scancode.SDL_SCANCODE_3,SDL1_Keycode.SDLK_3, "3", "3"),
			new KeyInfo(Scancode.SDL_SCANCODE_4,SDL1_Keycode.SDLK_4, "4", "4"),
			new KeyInfo(Scancode.SDL_SCANCODE_5,SDL1_Keycode.SDLK_5, "5", "5"),
			new KeyInfo(Scancode.SDL_SCANCODE_6,SDL1_Keycode.SDLK_6, "6", "6"),
			new KeyInfo(Scancode.SDL_SCANCODE_7,SDL1_Keycode.SDLK_7, "7", "7"),
			new KeyInfo(Scancode.SDL_SCANCODE_8,SDL1_Keycode.SDLK_8, "8", "8"),
			new KeyInfo(Scancode.SDL_SCANCODE_9,SDL1_Keycode.SDLK_9, "9", "9"),
			new KeyInfo(Scancode.SDL_SCANCODE_0,SDL1_Keycode.SDLK_0, "0", "0"),
			new KeyInfo(Scancode.SDL_SCANCODE_MINUS,SDL1_Keycode.SDLK_MINUS, "MINUS", "Minus"),
			new KeyInfo(Scancode.SDL_SCANCODE_EQUALS,SDL1_Keycode.SDLK_EQUALS, "EQUALS", "Equals"),
			new KeyInfo(Scancode.SDL_SCANCODE_BACKSPACE,SDL1_Keycode.SDLK_BACKSPACE, "BACKSPACE", "Backspace"),

			new KeyInfo(Scancode.SDL_SCANCODE_TAB,SDL1_Keycode.SDLK_TAB, "TAB", "Tab"),
			new KeyInfo(Scancode.SDL_SCANCODE_Q,SDL1_Keycode.SDLK_q, "q", "Q"),
			new KeyInfo(Scancode.SDL_SCANCODE_W,SDL1_Keycode.SDLK_w, "w", "W"),
			new KeyInfo(Scancode.SDL_SCANCODE_E,SDL1_Keycode.SDLK_e, "e", "E"),
			new KeyInfo(Scancode.SDL_SCANCODE_R,SDL1_Keycode.SDLK_r, "r", "R"),
			new KeyInfo(Scancode.SDL_SCANCODE_T,SDL1_Keycode.SDLK_t, "t", "T"),
			new KeyInfo(Scancode.SDL_SCANCODE_Y,SDL1_Keycode.SDLK_y, "y", "Y"),
			new KeyInfo(Scancode.SDL_SCANCODE_U,SDL1_Keycode.SDLK_u, "u", "U"),
			new KeyInfo(Scancode.SDL_SCANCODE_I,SDL1_Keycode.SDLK_i, "i", "I"),
			new KeyInfo(Scancode.SDL_SCANCODE_O,SDL1_Keycode.SDLK_o, "o", "O"),
			new KeyInfo(Scancode.SDL_SCANCODE_P,SDL1_Keycode.SDLK_p, "p", "P"),
			new KeyInfo(Scancode.SDL_SCANCODE_LEFTBRACKET,SDL1_Keycode.SDLK_LEFTBRACKET, "LEFTBRACKET", "Left bracket"),
			new KeyInfo(Scancode.SDL_SCANCODE_RIGHTBRACKET,SDL1_Keycode.SDLK_RIGHTBRACKET, "RIGHTBRACKET", "Right bracket"),
			new KeyInfo(Scancode.SDL_SCANCODE_BACKSLASH,SDL1_Keycode.SDLK_BACKSLASH, "BACKSLASH", "Backslash"),

			new KeyInfo(Scancode.SDL_SCANCODE_CAPSLOCK,SDL1_Keycode.SDLK_CAPSLOCK, "CAPSLOCK", "Capslock"),
			new KeyInfo(Scancode.SDL_SCANCODE_A,SDL1_Keycode.SDLK_a, "a", "A"),
			new KeyInfo(Scancode.SDL_SCANCODE_S,SDL1_Keycode.SDLK_s, "s", "S"),
			new KeyInfo(Scancode.SDL_SCANCODE_D,SDL1_Keycode.SDLK_d, "d", "D"),
			new KeyInfo(Scancode.SDL_SCANCODE_F,SDL1_Keycode.SDLK_f, "f", "F"),
			new KeyInfo(Scancode.SDL_SCANCODE_G,SDL1_Keycode.SDLK_g, "g", "G"),
			new KeyInfo(Scancode.SDL_SCANCODE_H,SDL1_Keycode.SDLK_h, "h", "H"),
			new KeyInfo(Scancode.SDL_SCANCODE_J,SDL1_Keycode.SDLK_j, "j", "J"),
			new KeyInfo(Scancode.SDL_SCANCODE_K,SDL1_Keycode.SDLK_k, "k", "K"),
			new KeyInfo(Scancode.SDL_SCANCODE_L,SDL1_Keycode.SDLK_l, "l", "L"),
			new KeyInfo(Scancode.SDL_SCANCODE_SEMICOLON,SDL1_Keycode.SDLK_SEMICOLON, "SEMICOLON", "Semicolon"),
			new KeyInfo(Scancode.SDL_SCANCODE_APOSTROPHE,SDL1_Keycode.SDLK_QUOTE, "QUOTE", "Quote"),
			new KeyInfo(Scancode.SDL_SCANCODE_RETURN,SDL1_Keycode.SDLK_RETURN, "RETURN", "Return"),

			new KeyInfo(Scancode.SDL_SCANCODE_LSHIFT,SDL1_Keycode.SDLK_LSHIFT, "LSHIFT", "Left Shift"),
			new KeyInfo(Scancode.SDL_SCANCODE_Z,SDL1_Keycode.SDLK_z, "z", "Z"),
			new KeyInfo(Scancode.SDL_SCANCODE_X,SDL1_Keycode.SDLK_x, "x", "X"),
			new KeyInfo(Scancode.SDL_SCANCODE_C,SDL1_Keycode.SDLK_c, "c", "C"),
			new KeyInfo(Scancode.SDL_SCANCODE_V,SDL1_Keycode.SDLK_v, "v", "V"),
			new KeyInfo(Scancode.SDL_SCANCODE_B,SDL1_Keycode.SDLK_b, "b", "B"),
			new KeyInfo(Scancode.SDL_SCANCODE_N,SDL1_Keycode.SDLK_n, "n", "N"),
			new KeyInfo(Scancode.SDL_SCANCODE_M,SDL1_Keycode.SDLK_m, "m", "M"),
			new KeyInfo(Scancode.SDL_SCANCODE_COMMA,SDL1_Keycode.SDLK_COMMA, "COMMA", "Comma"),
			new KeyInfo(Scancode.SDL_SCANCODE_PERIOD,SDL1_Keycode.SDLK_PERIOD, "PERIOD", "Period"),
			new KeyInfo(Scancode.SDL_SCANCODE_SLASH,SDL1_Keycode.SDLK_SLASH, "SLASH", "Slash"),
			new KeyInfo(Scancode.SDL_SCANCODE_RSHIFT,SDL1_Keycode.SDLK_RSHIFT, "RSHIFT", "Right Shift"),

			new KeyInfo(Scancode.SDL_SCANCODE_LCTRL,SDL1_Keycode.SDLK_LCTRL, "LCTRL", "Left Ctrl"),
			new KeyInfo(Scancode.SDL_SCANCODE_LGUI,SDL1_Keycode.SDLK_LSUPER, "LSUPER", "Left Application"),
			new KeyInfo(Scancode.SDL_SCANCODE_LALT,SDL1_Keycode.SDLK_LALT, "LALT", "Left Alt"),
			new KeyInfo(Scancode.SDL_SCANCODE_SPACE,SDL1_Keycode.SDLK_SPACE, "SPACE", "Space"),
			new KeyInfo(Scancode.SDL_SCANCODE_RALT,SDL1_Keycode.SDLK_RALT, "RALT", "Right Alt"),
			new KeyInfo(Scancode.SDL_SCANCODE_MODE,SDL1_Keycode.SDLK_MODE, "MODE", "Alt Gr"),
			new KeyInfo(Scancode.SDL_SCANCODE_RGUI,SDL1_Keycode.SDLK_RSUPER, "RSUPER", "Right Application"),
			new KeyInfo(Scancode.SDL_SCANCODE_MENU,SDL1_Keycode.SDLK_MENU, "MENU", "Menu"),
			new KeyInfo(Scancode.SDL_SCANCODE_RCTRL,SDL1_Keycode.SDLK_RCTRL, "RCTRL", "Right Ctrl"),


			new KeyInfo(Scancode.SDL_SCANCODE_SYSREQ,SDL1_Keycode.SDLK_SYSREQ, "SYSREQ", "SysRq"),
			new KeyInfo(Scancode.SDL_SCANCODE_PRINTSCREEN,SDL1_Keycode.SDLK_PRINT, "PRINT", "Print Screen"),
			new KeyInfo(Scancode.SDL_SCANCODE_SCROLLLOCK,SDL1_Keycode.SDLK_SCROLLOCK, "SCROLLLOCK", "Scrolllock"),
			new KeyInfo(Scancode.SDL_SCANCODE_PAUSE,SDL1_Keycode.SDLK_PAUSE, "PASCROLLLOCKUSE", "Pause/Break"),
			new KeyInfo(Scancode.SDL_SCANCODE_INSERT,SDL1_Keycode.SDLK_INSERT, "INSERT", "Insert"),
			new KeyInfo(Scancode.SDL_SCANCODE_DELETE,SDL1_Keycode.SDLK_DELETE, "DELETE", "Delete"),
			new KeyInfo(Scancode.SDL_SCANCODE_HOME,SDL1_Keycode.SDLK_HOME, "HOME", "Home"),
			new KeyInfo(Scancode.SDL_SCANCODE_END,SDL1_Keycode.SDLK_END, "END", "End"),
			new KeyInfo(Scancode.SDL_SCANCODE_PAGEUP,SDL1_Keycode.SDLK_PAGEUP, "PAGEUP", "Page up"),
			new KeyInfo(Scancode.SDL_SCANCODE_PAGEDOWN,SDL1_Keycode.SDLK_PAGEDOWN, "PAGEDOWN", "Page down"),


			new KeyInfo(Scancode.SDL_SCANCODE_UP,SDL1_Keycode.SDLK_UP, "UP", "Up"),
			new KeyInfo(Scancode.SDL_SCANCODE_DOWN,SDL1_Keycode.SDLK_DOWN, "DOWN", "Down"),
			new KeyInfo(Scancode.SDL_SCANCODE_LEFT,SDL1_Keycode.SDLK_LEFT, "LEFT", "Left"),
			new KeyInfo(Scancode.SDL_SCANCODE_RIGHT,SDL1_Keycode.SDLK_RIGHT, "RIGHT", "Right"),

			new KeyInfo(Scancode.SDL_SCANCODE_NUMLOCKCLEAR,SDL1_Keycode.SDLK_NUMLOCK, "NUMLOCK", "Numlock"),
			new KeyInfo(Scancode.SDL_SCANCODE_KP_DIVIDE,SDL1_Keycode.SDLK_KP_DIVIDE, "KP_DIVIDE", "Keypad Divide"),
			new KeyInfo(Scancode.SDL_SCANCODE_KP_MULTIPLY,SDL1_Keycode.SDLK_KP_MULTIPLY, "KP_MULTIPLY", "Keypad Multiply"),
			new KeyInfo(Scancode.SDL_SCANCODE_KP_MINUS,SDL1_Keycode.SDLK_KP_MINUS, "KP_MINUS", "Keypad Minus"),
			new KeyInfo(Scancode.SDL_SCANCODE_KP_PLUS,SDL1_Keycode.SDLK_KP_PLUS, "KP_PLUS", "Keypad Plus"),
			new KeyInfo(Scancode.SDL_SCANCODE_KP_ENTER,SDL1_Keycode.SDLK_KP_ENTER,"KP_ENTER", "Keypad Enter"),
			new KeyInfo(Scancode.SDL_SCANCODE_KP_0,SDL1_Keycode.SDLK_KP0, "KP0", "Keypad 0"),
			new KeyInfo(Scancode.SDL_SCANCODE_KP_1,SDL1_Keycode.SDLK_KP1,  "KP1", "Keypad 1"),
			new KeyInfo(Scancode.SDL_SCANCODE_KP_2,SDL1_Keycode.SDLK_KP2,  "KP2", "Keypad 2"),
			new KeyInfo(Scancode.SDL_SCANCODE_KP_3,SDL1_Keycode.SDLK_KP3,  "KP3", "Keypad 3"),
			new KeyInfo(Scancode.SDL_SCANCODE_KP_4,SDL1_Keycode.SDLK_KP4,  "KP4", "Keypad 4"),
			new KeyInfo(Scancode.SDL_SCANCODE_KP_5,SDL1_Keycode.SDLK_KP5,  "KP5", "Keypad 5"),
			new KeyInfo(Scancode.SDL_SCANCODE_KP_6,SDL1_Keycode.SDLK_KP6,  "KP6", "Keypad 6"),
			new KeyInfo(Scancode.SDL_SCANCODE_KP_7,SDL1_Keycode.SDLK_KP7,  "KP7", "Keypad 7"),
			new KeyInfo(Scancode.SDL_SCANCODE_KP_8,SDL1_Keycode.SDLK_KP8,  "KP8", "Keypad 8"),
			new KeyInfo(Scancode.SDL_SCANCODE_KP_9,SDL1_Keycode.SDLK_KP9,  "KP9", "Keypad 9"),
			new KeyInfo(Scancode.SDL_SCANCODE_KP_PERIOD,SDL1_Keycode.SDLK_KP_PERIOD, "KP_PERIOD", "Keypad Period"),

			new KeyInfo(Scancode.SDL_SCANCODE_CLEAR,SDL1_Keycode.SDLK_CLEAR,  "CLEAR", "Clear"),
			new KeyInfo(Scancode.SDL_SCANCODE_SLEEP,SDL1_Keycode.SDLK_LAST + 1, "SLEEP", "Sleep"), // not in sdl 1.2
			new KeyInfo(Scancode.SDL_SCANCODE_POWER,SDL1_Keycode.SDLK_POWER, "POWER", "Power"),
			new KeyInfo(Scancode.SDL_SCANCODE_HELP,SDL1_Keycode.SDLK_HELP, "HELP", "Help"),
		};

		// controls
		internal enum ControlMethod {
			Invalid = 0,
			Keyboard = 1,
			Joystick = 2
		}
		[Flags]
		internal enum KeyboardModifier {
			None = 0,
			Shift = 1,
			Ctrl = 2,
			Alt = 4
		}
		internal enum JoystickComponent { Invalid, Axis, Hat, Button, Ball }
		internal enum DigitalControlState {
			ReleasedAcknowledged = 0,
			Released = 1,
			Pressed = 2,
			PressedAcknowledged = 3
		}
		internal struct Control {
			/// <summary>Common values</summary>
			internal Command Command;
			internal CommandType InheritedType;
			/// <summary>Controller type</summary>
			internal ControlMethod Method;
			/// <summary>Keyboard modifiers</summary>
			internal KeyboardModifier Modifier;
			/// <summary>Joystick device index</summary>
			internal int Device;
			/// <summary>Joystick event type</summary>
			internal JoystickComponent Component;
			/// <summary>Control code (joystick element index, key, mouse button) </summary>
			internal int Element;
			/// <summary>Direction of joystick element movement</summary>
			internal int Direction;
			/// <summary>on/off state</summary>
			internal DigitalControlState DigitalState;
			/// <summary>analog state</summary>
			internal double AnalogState;
		}

		// control descriptions
		internal static string[] ControlDescriptions = new string[] { };
		internal static CommandInfo[] CommandInfos = new CommandInfo[] {
			new CommandInfo(Command.PowerIncrease, CommandType.Digital, "POWER_INCREASE"),
			new CommandInfo(Command.PowerDecrease, CommandType.Digital, "POWER_DECREASE"),
			new CommandInfo(Command.PowerHalfAxis, CommandType.AnalogHalf, "POWER_HALFAXIS"),
			new CommandInfo(Command.PowerFullAxis, CommandType.AnalogFull, "POWER_FULLAXIS"),
			new CommandInfo(Command.BrakeDecrease, CommandType.Digital, "BRAKE_DECREASE"),
			new CommandInfo(Command.BrakeIncrease, CommandType.Digital, "BRAKE_INCREASE"),
			new CommandInfo(Command.BrakeHalfAxis, CommandType.AnalogHalf, "BRAKE_HALFAXIS"),
			new CommandInfo(Command.BrakeFullAxis, CommandType.AnalogFull, "BRAKE_FULLAXIS"),
			new CommandInfo(Command.BrakeEmergency, CommandType.Digital, "BRAKE_EMERGENCY"),
			new CommandInfo(Command.SinglePower, CommandType.Digital, "SINGLE_POWER"),
			new CommandInfo(Command.SingleNeutral, CommandType.Digital, "SINGLE_NEUTRAL"),
			new CommandInfo(Command.SingleBrake, CommandType.Digital, "SINGLE_BRAKE"),
			new CommandInfo(Command.SingleEmergency, CommandType.Digital, "SINGLE_EMERGENCY"),
			new CommandInfo(Command.SingleFullAxis, CommandType.AnalogFull, "SINGLE_FULLAXIS"),
			new CommandInfo(Command.ReverserForward, CommandType.Digital, "REVERSER_FORWARD"),
			new CommandInfo(Command.ReverserBackward, CommandType.Digital, "REVERSER_BACKWARD"),
			new CommandInfo(Command.ReverserFullAxis, CommandType.AnalogFull, "REVERSER_FULLAXIS"),
			new CommandInfo(Command.DoorsLeft, CommandType.Digital, "DOORS_LEFT"),
			new CommandInfo(Command.DoorsRight, CommandType.Digital, "DOORS_RIGHT"),
			new CommandInfo(Command.HornPrimary, CommandType.Digital, "HORN_PRIMARY"),
			new CommandInfo(Command.HornSecondary, CommandType.Digital, "HORN_SECONDARY"),
			new CommandInfo(Command.HornMusic, CommandType.Digital, "HORN_MUSIC"),
			new CommandInfo(Command.DeviceConstSpeed, CommandType.Digital, "DEVICE_CONSTSPEED"),
			new CommandInfo(Command.SecurityS, CommandType.Digital, "SECURITY_S"),
			new CommandInfo(Command.SecurityA1, CommandType.Digital, "SECURITY_A1"),
			new CommandInfo(Command.SecurityA2, CommandType.Digital, "SECURITY_A2"),
			new CommandInfo(Command.SecurityB1, CommandType.Digital, "SECURITY_B1"),
			new CommandInfo(Command.SecurityB2, CommandType.Digital, "SECURITY_B2"),
			new CommandInfo(Command.SecurityC1, CommandType.Digital, "SECURITY_C1"),
			new CommandInfo(Command.SecurityC2, CommandType.Digital, "SECURITY_C2"),
			new CommandInfo(Command.SecurityD, CommandType.Digital, "SECURITY_D"),
			new CommandInfo(Command.SecurityE, CommandType.Digital, "SECURITY_E"),
			new CommandInfo(Command.SecurityF, CommandType.Digital, "SECURITY_F"),
			new CommandInfo(Command.SecurityG, CommandType.Digital, "SECURITY_G"),
			new CommandInfo(Command.SecurityH, CommandType.Digital, "SECURITY_H"),
			new CommandInfo(Command.SecurityI, CommandType.Digital, "SECURITY_I"),
			new CommandInfo(Command.SecurityJ, CommandType.Digital, "SECURITY_J"),
			new CommandInfo(Command.SecurityK, CommandType.Digital, "SECURITY_K"),
			new CommandInfo(Command.SecurityL, CommandType.Digital, "SECURITY_L"),
			new CommandInfo(Command.SecurityM, CommandType.Digital, "SECURITY_M"),
			new CommandInfo(Command.SecurityN, CommandType.Digital, "SECURITY_N"),
			new CommandInfo(Command.SecurityO, CommandType.Digital, "SECURITY_O"),
			new CommandInfo(Command.SecurityP, CommandType.Digital, "SECURITY_P"),
			new CommandInfo(Command.SecurityQ, CommandType.Digital, "SECURITY_Q"),
			new CommandInfo(Command.SecurityR, CommandType.Digital, "SECURITY_R"),
			new CommandInfo(Command.SecurityT, CommandType.Digital, "SECURITY_T"),
			new CommandInfo(Command.SecurityU, CommandType.Digital, "SECURITY_U"),
			new CommandInfo(Command.SecurityV, CommandType.Digital, "SECURITY_V"),
			new CommandInfo(Command.SecurityW, CommandType.Digital, "SECURITY_W"),
			new CommandInfo(Command.SecurityX, CommandType.Digital, "SECURITY_X"),
			new CommandInfo(Command.SecurityY, CommandType.Digital, "SECURITY_Y"),
			new CommandInfo(Command.SecurityZ, CommandType.Digital, "SECURITY_Z"),
			new CommandInfo(Command.CameraInterior, CommandType.Digital, "CAMERA_INTERIOR"),
			new CommandInfo(Command.CameraExterior, CommandType.Digital, "CAMERA_EXTERIOR"),
			new CommandInfo(Command.CameraTrack, CommandType.Digital, "CAMERA_TRACK"),
			new CommandInfo(Command.CameraFlyBy, CommandType.Digital, "CAMERA_FLYBY"),
			new CommandInfo(Command.CameraMoveForward, CommandType.AnalogHalf, "CAMERA_MOVE_FORWARD"),
			new CommandInfo(Command.CameraMoveBackward, CommandType.AnalogHalf, "CAMERA_MOVE_BACKWARD"),
			new CommandInfo(Command.CameraMoveLeft, CommandType.AnalogHalf, "CAMERA_MOVE_LEFT"),
			new CommandInfo(Command.CameraMoveRight, CommandType.AnalogHalf, "CAMERA_MOVE_RIGHT"),
			new CommandInfo(Command.CameraMoveUp, CommandType.AnalogHalf, "CAMERA_MOVE_UP"),
			new CommandInfo(Command.CameraMoveDown, CommandType.AnalogHalf, "CAMERA_MOVE_DOWN"),
			new CommandInfo(Command.CameraRotateLeft, CommandType.AnalogHalf, "CAMERA_ROTATE_LEFT"),
			new CommandInfo(Command.CameraRotateRight, CommandType.AnalogHalf, "CAMERA_ROTATE_RIGHT"),
			new CommandInfo(Command.CameraRotateUp, CommandType.AnalogHalf, "CAMERA_ROTATE_UP"),
			new CommandInfo(Command.CameraRotateDown, CommandType.AnalogHalf, "CAMERA_ROTATE_DOWN"),
			new CommandInfo(Command.CameraRotateCCW, CommandType.AnalogHalf, "CAMERA_ROTATE_CCW"),
			new CommandInfo(Command.CameraRotateCW, CommandType.AnalogHalf, "CAMERA_ROTATE_CW"),
			new CommandInfo(Command.CameraZoomIn, CommandType.AnalogHalf, "CAMERA_ZOOM_IN"),
			new CommandInfo(Command.CameraZoomOut, CommandType.AnalogHalf, "CAMERA_ZOOM_OUT"),
			new CommandInfo(Command.CameraPreviousPOI, CommandType.Digital, "CAMERA_POI_PREVIOUS"),
			new CommandInfo(Command.CameraNextPOI, CommandType.Digital, "CAMERA_POI_NEXT"),
			new CommandInfo(Command.CameraReset, CommandType.Digital, "CAMERA_RESET"),
			new CommandInfo(Command.CameraRestriction, CommandType.Digital, "CAMERA_RESTRICTION"),
			new CommandInfo(Command.TimetableToggle, CommandType.Digital, "TIMETABLE_TOGGLE"),
			new CommandInfo(Command.TimetableUp, CommandType.AnalogHalf, "TIMETABLE_UP"),
			new CommandInfo(Command.TimetableDown, CommandType.AnalogHalf, "TIMETABLE_DOWN"),
			new CommandInfo(Command.MenuActivate, CommandType.Digital, "MENU_ACTIVATE"),
			new CommandInfo(Command.MenuUp, CommandType.Digital, "MENU_UP"),
			new CommandInfo(Command.MenuDown, CommandType.Digital, "MENU_DOWN"),
			new CommandInfo(Command.MenuEnter, CommandType.Digital, "MENU_ENTER"),
			new CommandInfo(Command.MenuBack, CommandType.Digital, "MENU_BACK"),
			new CommandInfo(Command.MiscClock, CommandType.Digital, "MISC_CLOCK"),
			new CommandInfo(Command.MiscSpeed, CommandType.Digital, "MISC_SPEED"),
			new CommandInfo(Command.MiscFps, CommandType.Digital, "MISC_FPS"),
			new CommandInfo(Command.MiscAI, CommandType.Digital, "MISC_AI"),
			new CommandInfo(Command.MiscFullscreen, CommandType.Digital, "MISC_FULLSCREEN"),
			new CommandInfo(Command.MiscMute, CommandType.Digital, "MISC_MUTE"),
			new CommandInfo(Command.MiscPause, CommandType.Digital, "MISC_PAUSE"),
			new CommandInfo(Command.MiscTimeFactor, CommandType.Digital, "MISC_TIMEFACTOR"),
			new CommandInfo(Command.MiscQuit, CommandType.Digital, "MISC_QUIT"),
			new CommandInfo(Command.MiscInterfaceMode, CommandType.Digital, "MISC_INTERFACE"),
			new CommandInfo(Command.MiscBackfaceCulling, CommandType.Digital, "MISC_BACKFACE"),
			new CommandInfo(Command.MiscCPUMode, CommandType.Digital, "MISC_CPUMODE"),
			new CommandInfo(Command.DebugWireframe, CommandType.Digital, "DEBUG_WIREFRAME"),
			new CommandInfo(Command.DebugNormals, CommandType.Digital, "DEBUG_NORMALS"),
			new CommandInfo(Command.DebugBrakeSystems, CommandType.Digital, "DEBUG_BRAKE"),
		};
		internal static Control[] CurrentControls = new Control[] { };

		// try get command info
		internal static bool TryGetCommandInfo(Command Value, out CommandInfo Info) {
			for (int i = 0; i < CommandInfos.Length; i++) {
				if (CommandInfos[i].Command == Value) {
					Info = CommandInfos[i];
					return true;
				}
			}
			Info = new CommandInfo(Value,CommandType.Digital,"N/A");
			return false;
		}


		private static int currentIndex = 0;
		/// <summary>
		/// Looks up the Keycode for a Scancode in Keys array
		/// </summary>
		/// <returns>Corresponding Keycode or Keycode.SDLK_UNKNOWN</returns>
		/// <param name="value">The scancode for search.</param>
		private static int LookupScancode(Scancode value){
			for(int k = 0; k < Keys.Length; k++){
				int i;
				if ((k & 1) == 0) {
					i = (currentIndex + (k >> 1) + Keys.Length) % Keys.Length;
				} else {
					i = (currentIndex - (k + 1 >> 1) + Keys.Length) % Keys.Length;
				}
				if (Keys[i].Scancode == value) {
					currentIndex = (i + 1) % Keys.Length;
					return Keys[i].StorageCode;
				}
			}
			return (int)SDL1_Keycode.SDLK_UNKNOWN;
		}
		private static Scancode LookupKeycode(int value){
			for(int k = 0; k < Keys.Length; k++){
				int i;
				if ((k & 1) == 0) {
					i = (currentIndex + (k >> 1) + Keys.Length) % Keys.Length;
				} else {
					i = (currentIndex - (k + 1 >> 1) + Keys.Length) % Keys.Length;
				}
				if (Keys[i].StorageCode == value) {
					currentIndex = (i + 1) % Keys.Length;
					return Keys[i].Scancode;
				}
			}
			return Scancode.SDL_SCANCODE_UNKNOWN;
		}
		// save controls
		internal static void SaveControls(string FileOrNull) {
			CultureInfo Culture = CultureInfo.InvariantCulture;
			System.Text.StringBuilder Builder = new System.Text.StringBuilder();
			Builder.AppendLine("; Current control configuration");
			Builder.AppendLine("; =============================");
			Builder.AppendLine("; This file was automatically generated. Please modify only if you know what you're doing. There's some help: ");
			Builder.AppendLine("; keyboard, <sdl 1.2 key>, <openbve modifier mask>");
			Builder.AppendLine("; joystick, <device number>, ball, <ball identifier>, <ball direction>");
			Builder.AppendLine("; joystick, <device number>, axis, <axis identifier>, <axis direction>");
			Builder.AppendLine("; joystick, <device number>, hat, <hat identifier>, <hat direction>");
			Builder.AppendLine("; joystick, <device number>, button, <button identifier>");
			Builder.AppendLine("; joystick, <device number>, invalid");
			Builder.AppendLine();
			for (int i = 0; i < CurrentControls.Length; i++) {
				CommandInfo Info;
				TryGetCommandInfo(CurrentControls[i].Command, out Info);
				Builder.Append(Info.Name + ", ");
				switch (CurrentControls[i].Method) {
					case ControlMethod.Keyboard:
						int sdl = LookupScancode((Scancode)CurrentControls[i].Element);
						Builder.AppendFormat("keyboard, {0}, {1}", sdl, ((int)CurrentControls[i].Modifier).ToString(Culture));
						break;
					case ControlMethod.Joystick:
						Builder.AppendFormat("joystick, {0}, ", CurrentControls[i].Device.ToString(Culture));
						switch (CurrentControls[i].Component) {
							case JoystickComponent.Ball:
								Builder.AppendFormat("ball, {0}, {1}", CurrentControls[i].Element.ToString(Culture), CurrentControls[i].Direction.ToString(Culture));
								break;
							case JoystickComponent.Axis:
								Builder.AppendFormat("axis, {0}, {1}", CurrentControls[i].Element.ToString(Culture), CurrentControls[i].Direction.ToString(Culture));
								break;
							case JoystickComponent.Hat:
								Builder.AppendFormat("hat, {0}, {1}", CurrentControls[i].Element.ToString(Culture), CurrentControls[i].Direction.ToString(Culture));
								break;
							case JoystickComponent.Button:
								Builder.AppendFormat("button, {0}", CurrentControls[i].Element.ToString(Culture));
								break;
							default:
								Builder.Append("invalid");
								break;
						}
						break;
				}
				Builder.Append("\n");
			}
			string File;
			File = FileOrNull ?? OpenBveApi.Path.CombineFile(Program.FileSystem.SettingsFolder, "controls.cfg");
			System.IO.File.WriteAllText(File, Builder.ToString(), new System.Text.UTF8Encoding(true));
		}

		// load controls
		internal static void LoadControls(string FileOrNull, out Control[] Controls) {
			string File;
			if (FileOrNull == null) {
				File = OpenBveApi.Path.CombineFile(Program.FileSystem.SettingsFolder, "controls.cfg");
				if (!System.IO.File.Exists(File)) {
					File = OpenBveApi.Path.CombineFile(Program.FileSystem.GetDataFolder("Controls"), "Default keyboard assignment.controls");
				}
			} else {
				File = FileOrNull;
			}
			Controls = new Control[256];
			int length = 0;
			CultureInfo culture = CultureInfo.InvariantCulture;
			if (System.IO.File.Exists(File)) {
				string[] lines = System.IO.File.ReadAllLines(File, new System.Text.UTF8Encoding());
				for (int i = 0; i < lines.Length; i++) {
					lines[i] = lines[i].Trim();
					if (lines[i].Length != 0 && !lines[i].StartsWith(";", StringComparison.OrdinalIgnoreCase)) {
						string[] Terms = lines[i].Split(new char[] { ',' });
						for (int j = 0; j < Terms.Length; j++) {
							Terms[j] = Terms[j].Trim();
						}
						if (Terms.Length >= 2) {
							if (length >= Controls.Length) {
								Array.Resize<Control>(ref Controls, Controls.Length << 1);
							}
							int j;
							for (j = 0; j < CommandInfos.Length; j++) {
								if (string.Compare(CommandInfos[j].Name, Terms[0], StringComparison.OrdinalIgnoreCase) == 0) break;
							}
							if (j == CommandInfos.Length) {
								Controls[length].Command = Command.None;
								Controls[length].InheritedType = CommandType.Digital;
								Controls[length].Method = ControlMethod.Invalid;
								Controls[length].Device = -1;
								Controls[length].Component = JoystickComponent.Invalid;
								Controls[length].Element = -1;
								Controls[length].Direction = 0;
								Controls[length].Modifier = KeyboardModifier.None;
							} else {
								Controls[length].Command = CommandInfos[j].Command;
								Controls[length].InheritedType = CommandInfos[j].Type;
								string Method = Terms[1].ToLowerInvariant();
								bool Valid = false;
								if (Method == "keyboard" && Terms.Length == 4) {
									int Element, Modifiers;
									if (int.TryParse(Terms[2], NumberStyles.Integer, culture, out Element)) {
										if (int.TryParse(Terms[3], NumberStyles.Integer, culture, out Modifiers)) {
											Controls[length].Method = ControlMethod.Keyboard;
											Controls[length].Device = -1;
											Controls[length].Component = JoystickComponent.Invalid;
											Controls[length].Element = (int)LookupKeycode(Element);
											Controls[length].Direction = 0;
											Controls[length].Modifier = (KeyboardModifier)Modifiers;
											Valid = true;
										}
									}
								} else if (Method == "joystick" && Terms.Length >= 4) {
									int Device;
									if (int.TryParse(Terms[2], NumberStyles.Integer, culture, out Device)) {
										string Component = Terms[3].ToLowerInvariant();
										if (Component == "axis" & Terms.Length == 6) {
											int Element, Direction;
											if (int.TryParse(Terms[4], NumberStyles.Integer, culture, out Element)) {
												if (int.TryParse(Terms[5], NumberStyles.Integer, culture, out Direction)) {
													Controls[length].Method = ControlMethod.Joystick;
													Controls[length].Device = Device;
													Controls[length].Component = JoystickComponent.Axis;
													Controls[length].Element = Element;
													Controls[length].Direction = Direction;
													Controls[length].Modifier = KeyboardModifier.None;
													Valid = true;
												}
											}
										} else if (Component == "ball" & Terms.Length == 6) {
											int Element, Direction;
											if (int.TryParse(Terms[4], NumberStyles.Integer, culture, out Element)) {
												if (int.TryParse(Terms[5], NumberStyles.Integer, culture, out Direction)) {
													Controls[length].Method = ControlMethod.Joystick;
													Controls[length].Device = Device;
													Controls[length].Component = JoystickComponent.Ball;
													Controls[length].Element = Element;
													Controls[length].Direction = Direction;
													Controls[length].Modifier = KeyboardModifier.None;
													Valid = true;
												}
											}
										} else if (Component == "hat" & Terms.Length == 6) {
											int Element, Direction;
											if (int.TryParse(Terms[4], NumberStyles.Integer, culture, out Element)) {
												if (int.TryParse(Terms[5], NumberStyles.Integer, culture, out Direction)) {
													Controls[length].Method = ControlMethod.Joystick;
													Controls[length].Device = Device;
													Controls[length].Component = JoystickComponent.Hat;
													Controls[length].Element = Element;
													Controls[length].Direction = Direction;
													Controls[length].Modifier = KeyboardModifier.None;
													Valid = true;
												}
											}
										} else if (Component == "button" & Terms.Length == 5) {
											int Element;
											if (int.TryParse(Terms[4], NumberStyles.Integer, culture, out Element)) {
												Controls[length].Method = ControlMethod.Joystick;
												Controls[length].Device = Device;
												Controls[length].Component = JoystickComponent.Button;
												Controls[length].Element = Element;
												Controls[length].Direction = 0;
												Controls[length].Modifier = KeyboardModifier.None;
												Valid = true;

											}
										}
									}
								}
								if (!Valid) {
									Controls[length].Method = ControlMethod.Invalid;
									Controls[length].Device = -1;
									Controls[length].Component = JoystickComponent.Invalid;
									Controls[length].Element = -1;
									Controls[length].Direction = 0;
									Controls[length].Modifier = KeyboardModifier.None;
								}
							}
							length++;
						}
					}
				}
			}
			Array.Resize<Control>(ref Controls, length);
		}
		// add controls
		internal static void AddControls(ref Control[] Base, Control[] Add) {
			for (int i = 0; i < Add.Length; i++) {
				int j;
				for (j = 0; j < Base.Length; j++) {
					if (Add[i].Command == Base[j].Command) break;
				}
				if (j == Base.Length) {
					Array.Resize<Control>(ref Base, Base.Length + 1);
					Base[Base.Length - 1] = Add[i];
				}
			}
		}
		internal enum SDL1_Keycode {
			// ascii-mapped start
			SDLK_UNKNOWN		= 0,
			SDLK_FIRST		= 0,
			SDLK_BACKSPACE		= 8,
			SDLK_TAB		= 9,
			SDLK_CLEAR		= 12,
			SDLK_RETURN		= 13,
			SDLK_PAUSE		= 19,
			SDLK_ESCAPE		= 27,
			SDLK_SPACE		= 32,
			SDLK_EXCLAIM		= 33,
			SDLK_QUOTEDBL		= 34,
			SDLK_HASH		= 35,
			SDLK_DOLLAR		= 36,
			SDLK_AMPERSAND		= 38,
			SDLK_QUOTE		= 39,
			SDLK_LEFTPAREN		= 40,
			SDLK_RIGHTPAREN		= 41,
			SDLK_ASTERISK		= 42,
			SDLK_PLUS		= 43,
			SDLK_COMMA		= 44,
			SDLK_MINUS		= 45,
			SDLK_PERIOD		= 46,
			SDLK_SLASH		= 47,
			SDLK_0			= 48,
			SDLK_1			= 49,
			SDLK_2			= 50,
			SDLK_3			= 51,
			SDLK_4			= 52,
			SDLK_5			= 53,
			SDLK_6			= 54,
			SDLK_7			= 55,
			SDLK_8			= 56,
			SDLK_9			= 57,
			SDLK_COLON		= 58,
			SDLK_SEMICOLON		= 59,
			SDLK_LESS		= 60,
			SDLK_EQUALS		= 61,
			SDLK_GREATER		= 62,
			SDLK_QUESTION		= 63,
			SDLK_AT			= 64,
	        // Skip uppercase letters
			SDLK_LEFTBRACKET	= 91,
			SDLK_BACKSLASH		= 92,
			SDLK_RIGHTBRACKET	= 93,
			SDLK_CARET		= 94,
			SDLK_UNDERSCORE		= 95,
			SDLK_BACKQUOTE		= 96,
			SDLK_a			= 97,
			SDLK_b			= 98,
			SDLK_c			= 99,
			SDLK_d			= 100,
			SDLK_e			= 101,
			SDLK_f			= 102,
			SDLK_g			= 103,
			SDLK_h			= 104,
			SDLK_i			= 105,
			SDLK_j			= 106,
			SDLK_k			= 107,
			SDLK_l			= 108,
			SDLK_m			= 109,
			SDLK_n			= 110,
			SDLK_o			= 111,
			SDLK_p			= 112,
			SDLK_q			= 113,
			SDLK_r			= 114,
			SDLK_s			= 115,
			SDLK_t			= 116,
			SDLK_u			= 117,
			SDLK_v			= 118,
			SDLK_w			= 119,
			SDLK_x			= 120,
			SDLK_y			= 121,
			SDLK_z			= 122,
			SDLK_DELETE		= 127,
			// end of ascii-mapped

			// international keysyms
			SDLK_WORLD_0		= 160,
			SDLK_WORLD_1		= 161,
			SDLK_WORLD_2		= 162,
			SDLK_WORLD_3		= 163,
			SDLK_WORLD_4		= 164,
			SDLK_WORLD_5		= 165,
			SDLK_WORLD_6		= 166,
			SDLK_WORLD_7		= 167,
			SDLK_WORLD_8		= 168,
			SDLK_WORLD_9		= 169,
			SDLK_WORLD_10		= 170,
			SDLK_WORLD_11		= 171,
			SDLK_WORLD_12		= 172,
			SDLK_WORLD_13		= 173,
			SDLK_WORLD_14		= 174,
			SDLK_WORLD_15		= 175,
			SDLK_WORLD_16		= 176,
			SDLK_WORLD_17		= 177,
			SDLK_WORLD_18		= 178,
			SDLK_WORLD_19		= 179,
			SDLK_WORLD_20		= 180,
			SDLK_WORLD_21		= 181,
			SDLK_WORLD_22		= 182,
			SDLK_WORLD_23		= 183,
			SDLK_WORLD_24		= 184,
			SDLK_WORLD_25		= 185,
			SDLK_WORLD_26		= 186,
			SDLK_WORLD_27		= 187,
			SDLK_WORLD_28		= 188,
			SDLK_WORLD_29		= 189,
			SDLK_WORLD_30		= 190,
			SDLK_WORLD_31		= 191,
			SDLK_WORLD_32		= 192,
			SDLK_WORLD_33		= 193,
			SDLK_WORLD_34		= 194,
			SDLK_WORLD_35		= 195,
			SDLK_WORLD_36		= 196,
			SDLK_WORLD_37		= 197,
			SDLK_WORLD_38		= 198,
			SDLK_WORLD_39		= 199,
			SDLK_WORLD_40		= 200,
			SDLK_WORLD_41		= 201,
			SDLK_WORLD_42		= 202,
			SDLK_WORLD_43		= 203,
			SDLK_WORLD_44		= 204,
			SDLK_WORLD_45		= 205,
			SDLK_WORLD_46		= 206,
			SDLK_WORLD_47		= 207,
			SDLK_WORLD_48		= 208,
			SDLK_WORLD_49		= 209,
			SDLK_WORLD_50		= 210,
			SDLK_WORLD_51		= 211,
			SDLK_WORLD_52		= 212,
			SDLK_WORLD_53		= 213,
			SDLK_WORLD_54		= 214,
			SDLK_WORLD_55		= 215,
			SDLK_WORLD_56		= 216,
			SDLK_WORLD_57		= 217,
			SDLK_WORLD_58		= 218,
			SDLK_WORLD_59		= 219,
			SDLK_WORLD_60		= 220,
			SDLK_WORLD_61		= 221,
			SDLK_WORLD_62		= 222,
			SDLK_WORLD_63		= 223,
			SDLK_WORLD_64		= 224,
			SDLK_WORLD_65		= 225,
			SDLK_WORLD_66		= 226,
			SDLK_WORLD_67		= 227,
			SDLK_WORLD_68		= 228,
			SDLK_WORLD_69		= 229,
			SDLK_WORLD_70		= 230,
			SDLK_WORLD_71		= 231,
			SDLK_WORLD_72		= 232,
			SDLK_WORLD_73		= 233,
			SDLK_WORLD_74		= 234,
			SDLK_WORLD_75		= 235,
			SDLK_WORLD_76		= 236,
			SDLK_WORLD_77		= 237,
			SDLK_WORLD_78		= 238,
			SDLK_WORLD_79		= 239,
			SDLK_WORLD_80		= 240,
			SDLK_WORLD_81		= 241,
			SDLK_WORLD_82		= 242,
			SDLK_WORLD_83		= 243,
			SDLK_WORLD_84		= 244,
			SDLK_WORLD_85		= 245,
			SDLK_WORLD_86		= 246,
			SDLK_WORLD_87		= 247,
			SDLK_WORLD_88		= 248,
			SDLK_WORLD_89		= 249,
			SDLK_WORLD_90		= 250,
			SDLK_WORLD_91		= 251,
			SDLK_WORLD_92		= 252,
			SDLK_WORLD_93		= 253,
			SDLK_WORLD_94		= 254,
			SDLK_WORLD_95		= 255,
			// keypad
			SDLK_KP0		= 256,
			SDLK_KP1		= 257,
			SDLK_KP2		= 258,
			SDLK_KP3		= 259,
			SDLK_KP4		= 260,
			SDLK_KP5		= 261,
			SDLK_KP6		= 262,
			SDLK_KP7		= 263,
			SDLK_KP8		= 264,
			SDLK_KP9		= 265,
			SDLK_KP_PERIOD		= 266,
			SDLK_KP_DIVIDE		= 267,
			SDLK_KP_MULTIPLY	= 268,
			SDLK_KP_MINUS		= 269,
			SDLK_KP_PLUS		= 270,
			SDLK_KP_ENTER		= 271,
			SDLK_KP_EQUALS		= 272,
			// arrows, home, etc.
			SDLK_UP			= 273,
			SDLK_DOWN		= 274,
			SDLK_RIGHT		= 275,
			SDLK_LEFT		= 276,
			SDLK_INSERT		= 277,
			SDLK_HOME		= 278,
			SDLK_END		= 279,
			SDLK_PAGEUP		= 280,
			SDLK_PAGEDOWN		= 281,
			// function keysyms
			SDLK_F1			= 282,
			SDLK_F2			= 283,
			SDLK_F3			= 284,
			SDLK_F4			= 285,
			SDLK_F5			= 286,
			SDLK_F6			= 287,
			SDLK_F7			= 288,
			SDLK_F8			= 289,
			SDLK_F9			= 290,
			SDLK_F10		= 291,
			SDLK_F11		= 292,
			SDLK_F12		= 293,
			SDLK_F13		= 294,
			SDLK_F14		= 295,
			SDLK_F15		= 296,

			// modifiers
			SDLK_NUMLOCK		= 300,
			SDLK_CAPSLOCK		= 301,
			SDLK_SCROLLOCK		= 302,
			SDLK_RSHIFT		= 303,
			SDLK_LSHIFT		= 304,
			SDLK_RCTRL		= 305,
			SDLK_LCTRL		= 306,
			SDLK_RALT		= 307,
			SDLK_LALT		= 308,
			SDLK_RMETA		= 309,
			SDLK_LMETA		= 310,
			SDLK_LSUPER		= 311, // Left "Windows" key 
			SDLK_RSUPER		= 312, // Right "Windows" key
			SDLK_MODE		= 313, // "Alt Gr" key
			SDLK_COMPOSE		= 314, // Multi-key compose key
			// misc
			SDLK_HELP		= 315,
			SDLK_PRINT		= 316,
			SDLK_SYSREQ		= 317,
			SDLK_BREAK		= 318,
			SDLK_MENU		= 319,
			SDLK_POWER		= 320,
			SDLK_EURO		= 321,
			SDLK_UNDO		= 322,
			// add new keys here
			SDLK_LAST
		}
    }
}

