using System;
using System.Diagnostics;
namespace OpenBve {
	internal static class Timers {

		// members
		private static Stopwatch timer;
		private static long last;
		// initialize
		internal static void Initialize() {
			timer = new Stopwatch();
			timer.Start();
			last = timer.ElapsedMilliseconds;
		}

		// get elapsed time
		internal static double GetElapsedTime() {
			long actual = timer.ElapsedMilliseconds;
			long timespan = actual - last;
			last = actual;
			return timespan*0.001;
		}

	}
}