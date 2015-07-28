using System;
using Tao.Sdl;

namespace OpenBve {
	internal static class Timers {

		// members
		private static double SdlTime = 0.0;

		// initialize
		internal static void Initialize() {
			SdlTime = 0.001 * (double)Sdl.SDL_GetTicks();
		}

		// get elapsed time
		internal static double GetElapsedTime() {
			double a = 0.001 * (double)Sdl.SDL_GetTicks();
			double d = a - SdlTime;
			SdlTime = a;
			return d;
		}

	}
}