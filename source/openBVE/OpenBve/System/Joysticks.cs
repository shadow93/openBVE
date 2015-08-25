using System;
using SDL2;
using System.Collections.Generic;

namespace OpenBve
{
	/// <summary>Provides functions for dealing with joysticks.</summary>
	internal static class Joysticks
	{
		
		// --- structures ---
		
		/// <summary>Represents a joystick.</summary>
		internal class Joystick
		{
			// --- members ---
			/// <summary>The textual representation of the joystick.</summary>
			internal string Name {
				get{ return SDL.SDL_JoystickName(Handle); }
			}

			/// <summary>The SDL handle to the joystick.</summary>
			internal IntPtr Handle;
			internal readonly int Index;
			/// <summary>The SDL GUID of this joystick.</summary>
			internal Guid GUID {
				get{ return SDL.SDL_JoystickGetGUID(Handle); }
			}
			// --- constructors ---
			/// <summary>Creates a new joystick.</summary>
			/// <param name="sdlHandle">The SDL handle to the joystick.</param>
			/// <param name="index">The SDL index of the joystick.</param>
			internal Joystick(IntPtr sdlHandle, int index)
			{
				this.Handle = sdlHandle;
				this.Index = index;
			}
		}
		
		
		// --- members ---
		
		/// <summary>Whether joysticks are initialized.</summary>
		private static bool Initialized = false;
		
		/// <summary>Holds all joysticks currently attached to the computer.</summary>
		internal static LinkedList<Joystick> AttachedJoysticks = new LinkedList<Joystick>();
		
		
		// --- functions ---
		
		/// <summary>Initializes joysticks. A call to SDL_Init must have been made before calling this function. A call to Deinitialize must be made when terminating the program.</summary>
		/// <returns>Whether initializing joysticks was successful.</returns>
		internal static bool Initialize()
		{
			if (Initialized)
				return true;
			if (SDL.SDL_InitSubSystem(SDL.SDL_INIT_JOYSTICK) != 0)
				return false;
			int count = SDL.SDL_NumJoysticks();
			for (int i = 0; i < count; i++) {
				IntPtr handle = SDL.SDL_JoystickOpen(i);
				AttachedJoysticks.AddLast(new Joystick(handle, i));
			}
			Initialized = true;
			return true;
		}

		/// <summary>Deinitializes joysticks.</summary>
		internal static void Deinitialize()
		{
			if (Initialized) {
				foreach(var joy in AttachedJoysticks) {
					SDL.SDL_JoystickClose(joy.Handle);
				}
				AttachedJoysticks.Clear();
				SDL.SDL_QuitSubSystem(SDL.SDL_INIT_JOYSTICK);
				Initialized = false;
			}
		}

	}
}