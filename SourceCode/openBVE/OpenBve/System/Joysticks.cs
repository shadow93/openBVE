using System;
using Tao.Sdl;

namespace OpenBve {
	/// <summary>Provides functions for dealing with joysticks.</summary>
	internal static class Joysticks {
		
		// --- structures ---
		
		/// <summary>Represents a joystick.</summary>
		internal struct Joystick {
			// --- members ---
			/// <summary>The textual representation of the joystick.</summary>
			internal string Name;
			/// <summary>The SDL handle to the joystick.</summary>
			internal IntPtr SdlHandle;
			// --- constructors ---
			/// <summary>Creates a new joystick.</summary>
			/// <param name="name">The textual representation of the joystick.</param>
			/// <param name="sdlHandle">The SDL handle to the joystick.</param>
			internal Joystick(string name, IntPtr sdlHandle) {
				this.Name = name;
				this.SdlHandle = sdlHandle;
			}
		}
		
		
		// --- members ---
		
		/// <summary>Whether joysticks are initialized.</summary>
		private static bool Initialized = false;
		
		/// <summary>Holds all joysticks currently attached to the computer.</summary>
		internal static Joystick[] AttachedJoysticks = new Joystick[] { };
		
		
		// --- functions ---
		
		/// <summary>Initializes joysticks. A call to SDL_Init must have been made before calling this function. A call to Deinitialize must be made when terminating the program.</summary>
		/// <returns>Whether initializing joysticks was successful.</returns>
		internal static bool Initialize() {
			if (!Initialized) {
				if (Sdl.SDL_Init(Sdl.SDL_INIT_JOYSTICK) != 0) {
					return false;
				} else {
					int count = Sdl.SDL_NumJoysticks();
					AttachedJoysticks = new Joystick[count];
					for (int i = 0; i < count; i++) {
						string name = Sdl.SDL_JoystickName(i);
						/* Due to an apparent bug in Tao or SDL, the joystick
						 * name returned is actually ASCII packed in UTF-16. */
						char[] characters = new char[2 * name.Length];
						for (int j = 0; j < name.Length; j++) {
							int value = (int)name[j];
							characters[2 * j + 0] = (char)(value & 0xFF);
							characters[2 * j + 1] = (char)(value >> 8);
						}
						AttachedJoysticks[i].Name = null;
						for (int j = 0; j < characters.Length; j++) {
							if (characters[j] == '\0') {
								AttachedJoysticks[i].Name = new string(characters, 0, j);
							}
						}
						if (AttachedJoysticks[i].Name == null) {
							AttachedJoysticks[i].Name = new string(characters);
						}
						AttachedJoysticks[i].SdlHandle = Sdl.SDL_JoystickOpen(i);
					}
					Initialized = true;
					return true;
				}
			} else {
				return true;
			}
		}
		
		/// <summary>Deinitializes joysticks.</summary>
		internal static void Deinitialize() {
			if (Initialized) {
				for (int i = 0; i < AttachedJoysticks.Length; i++) {
					Sdl.SDL_JoystickClose(AttachedJoysticks[i].SdlHandle);
				}
				AttachedJoysticks = new Joystick[] { };
				Sdl.SDL_QuitSubSystem(Sdl.SDL_INIT_JOYSTICK);
				Initialized = false;
			}
		}
		
	}
}