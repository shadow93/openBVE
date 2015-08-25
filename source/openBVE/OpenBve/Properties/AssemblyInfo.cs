using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

[assembly: AssemblyTitle("openBVE")]
[assembly: AssemblyProduct("openBVE")]
[assembly: AssemblyCopyright("The openBVE Project")]
[assembly: ComVisible(false)]
[assembly: AssemblyVersion("1.4.3.1")]
[assembly: AssemblyFileVersion("1.4.3.1")]
[assembly: CLSCompliant(true)]

namespace OpenBve {
	internal static partial class Program {
		internal const bool IsDevelopmentVersion = true;
		internal const string VersionSuffix = "-dev";
	}
}