namespace OpenBve
{
	/// <summary>
	/// Compile-time configuration.
	/// </summary>
    internal static class Configuration
    {
		/// <summary>
		/// List of paths for looking up the <see cref="FileSysCfg"/> file. If empty, the [assemblyFolder]/UserData/Settings/<see cref="Configuration.FileSysCfg"/>); is used.
		/// </summary>
		internal static readonly string[] FileSysCfgPaths = {};
		/// <summary>
		/// Default Filesystem configuration file name.
		/// </summary>
		internal static readonly string FileSysCfg = "filesystem.cfg";
    }
}

