#pragma warning disable 0659, 0661

using System;
using System.Text;

namespace OpenBveApi {

	/* ----------------------------------------
	 * TODO: This part of the API is unstable.
	 *       Modifications can be made at will.
	 * ---------------------------------------- */

	/// <summary>Provides path-related functions for accessing files and directories in a cross-platform manner.</summary>
	public static class Path {
		
		// --- read-only fields ---
		
		/// <summary>The list of characters that are invalid in platform-independent relative paths.</summary>
		private static readonly char[] InvalidPathChars = new char[] { ':', '*', '?', '"', '<', '>', '|' };
		
		/// <summary>The list of characters at which relative paths are separated into parts.</summary>
		private static readonly char[] PathSeparationChars = new char[] { '/', '\\' };
		
		
		// --- managed content ---
		
		/// <summary>The list of package names.</summary>
		private static string[] PackageNames = null;
		
		/// <summary>The list of package directories.</summary>
		private static string[] PackageDirectories = null;
		
		/// <summary>The object that serves as an authentication for the SetPackageLookupDirectories call.</summary>
		private static object SetPackageLookupDirectoriesAuthentication = null;
		
		
		// --- public functions ---
		
		/// <summary>Provides a list of package names and associated directories.</summary>
		/// <param name="names">The list of names.</param>
		/// <param name="directories">The list of fully qualified directories.</param>
		/// <param name="authentication">A null reference on the first process-wide call to this function, otherwise the object returned by this function in the previous call.</param>
		/// <exception cref="System.Security.SecurityException">Raised when the authentication failed.</exception>
		public static object SetPackageLookupDirectories(string[] names, string[] directories, object authentication) {
			if (authentication == SetPackageLookupDirectoriesAuthentication) {
				PackageNames = (string[])names.Clone();
				PackageDirectories = (string[])directories.Clone();
				Array.Sort<string, string>(PackageNames, PackageDirectories);
				SetPackageLookupDirectoriesAuthentication = new object();
				return SetPackageLookupDirectoriesAuthentication;
			} else {
				throw new System.Security.SecurityException();
			}
		}

		/// <summary>Combines a platform-specific absolute path with a platform-independent relative path that points to a directory.</summary>
		/// <param name="absolute">The platform-specific absolute path.</param>
		/// <param name="relative">The platform-independent relative path.</param>
		/// <returns>A platform-specific absolute path to the specified directory.</returns>
		/// <exception cref="System.Exception">Raised when combining the paths failed, for example due to malformed paths or due to unauthorized access.</exception>
		public static string CombineDirectory(string absolute, string relative) {
			int index = relative.IndexOf("??");
			if (index >= 0) {
				string directory = CombineDirectory(absolute, relative.Substring(0, index).TrimEnd());
				if (System.IO.Directory.Exists(directory)) {
					return directory;
				} else {
					return CombineDirectory(absolute, relative.Substring(index + 2).TrimStart());
				}
			}
			if (relative.IndexOfAny(InvalidPathChars) >= 0) {
				throw new ArgumentException("The relative path contains invalid characters.");
			}
			ResolvePackageReference(ref absolute, ref relative);
			string[] parts = relative.Split(PathSeparationChars, StringSplitOptions.RemoveEmptyEntries);
			for (int i = 0; i < parts.Length; i++) {
				if (parts[i].Length != 0) {
					/*
					 * Consider only non-empty parts.
					 * */
					if (IsAllPeriods(parts[i])) {
						/*
						 * A string of periods is a reference to an
						 * upper directory. A single period is the
						 * current directory. For each additional
						 * period, jump one directory up.
						 * */
						for (int j = 1; j < parts[i].Length; j++) {
							absolute = System.IO.Path.GetDirectoryName(absolute);
						}
					} else {
						/*
						 * This part references a directory.
						 * */
						string directory = System.IO.Path.Combine(absolute, parts[i]);
						if (System.IO.Directory.Exists(directory)) {
							absolute = directory;
						} else {
							/*
							 * Try to find the directory case-insensitively.
							 * */
							bool found = false;
							if (System.IO.Directory.Exists(absolute)) {
								string[] directories = System.IO.Directory.GetDirectories(absolute);
								for (int j = 0; j < directories.Length; j++) {
									string name = System.IO.Path.GetFileName(directories[j]);
									if (name.Equals(parts[i], StringComparison.OrdinalIgnoreCase)) {
										absolute = directories[j];
										found = true;
										break;
									}
								}
							}
							if (!found) {
								absolute = directory;
							}
						}
					}
				}
			}
			return absolute;
		}
		/// <summary>
		/// Iteratively combines a platform-specific absolute path with an array of platform-independent relative paths that point to a directory.
		/// </summary>
		/// <returns>The platform-specific absolute path.</returns>
		/// <param name="absolute">The platform-specific absolute path.</param>
		/// <param name="relatives">The array of platform-independent relative paths.</param>
		/// <exception cref="System.Exception">Raised when combining the paths failed, for example due to malformed paths or due to unauthorized access.</exception>
		public static string CombineDirectoryParams(string absolute, params string[] relatives){
			StringBuilder str = new StringBuilder();
			foreach (var rel in relatives) {
				str.Append(rel).Append('/');
			}
			return CombineDirectory(absolute,str.ToString());
		}

		/// <summary>Iteratively combines a platform-specific absolute path with an array of platform-independent relative paths that point to a file.</summary>
		/// <param name="absolute">The platform-specific absolute path.</param>
		/// <param name="relatives">The array of platform-independent relative paths.</param>
		/// <returns>Whether the operation succeeded and the specified file was found.</returns>
		/// <exception cref="System.Exception">Raised when combining the paths failed, for example due to malformed paths or due to unauthorized access.</exception>
		public static string CombineFileParams(string absolute, params string[] relatives){
			StringBuilder str = new StringBuilder();
			for (int i = 0; i < relatives.Length; i++) {
				str.Append(relatives[i]);
				if (i < (relatives.Length - 1))
					str.Append('/');
			}
			return CombineDirectory(absolute,str.ToString());
		}

		/// <summary>Combines a platform-specific absolute path with a platform-independent relative path that points to a file.</summary>
		/// <param name="absolute">The platform-specific absolute path.</param>
		/// <param name="relative">The platform-independent relative path.</param>
		/// <returns>Whether the operation succeeded and the specified file was found.</returns>
		/// <exception cref="System.Exception">Raised when combining the paths failed, for example due to malformed paths or due to unauthorized access.</exception>
		public static string CombineFile(string absolute, string relative) {
			int index = relative.IndexOf("??");
			if (index >= 0) {
				string file = CombineFile(absolute, relative.Substring(0, index).TrimEnd());
				if (System.IO.File.Exists(file)) {
					return file;
				} else {
					return CombineFile(absolute, relative.Substring(index + 2).TrimStart());
				}
			}
			if (relative.IndexOfAny(InvalidPathChars) >= 0) {
				throw new ArgumentException("The relative path contains invalid characters.");
			}
			ResolvePackageReference(ref absolute, ref relative);
			string[] parts = relative.Split(PathSeparationChars, StringSplitOptions.RemoveEmptyEntries);
			for (int i = 0; i < parts.Length; i++) {
				if (parts[i].Length != 0) {
					/*
					 * Consider only non-empty parts.
					 * */
					if (IsAllPeriods(parts[i])) {
						if (i == parts.Length - 1) {
							/*
							 * The last part must not be all periods because
							 * it would reference a directory then, not a file.
							 * */
							throw new ArgumentException("The relative path is malformed.");
						} else {
							/*
							 * A string of periods is a reference to an
							 * upper directory. A single period is the
							 * current directory. For each additional
							 * period, jump one directory up.
							 * */
							for (int j = 1; j < parts[i].Length; j++) {
								absolute = System.IO.Path.GetDirectoryName(absolute);
							}
						}
					} else if (i == parts.Length - 1) {
						/*
						 * The last part references a file.
						 * */
						string file = System.IO.Path.Combine(absolute, parts[i]);
						if (System.IO.File.Exists(file)) {
							return file;
						} else {
							/*
							 * Try to find the file case-insensitively.
							 * */
							if (System.IO.Directory.Exists(absolute)) {
								string[] files = System.IO.Directory.GetFiles(absolute);
								for (int j = 0; j < files.Length; j++) {
									string name = System.IO.Path.GetFileName(files[j]);
									if (name.Equals(parts[i], StringComparison.OrdinalIgnoreCase)) {
										return files[j];
									}
								}
							}
							return file;
						}
					} else {
						/*
						 * This part references a directory.
						 * */
						string directory = System.IO.Path.Combine(absolute, parts[i]);
						if (System.IO.Directory.Exists(directory)) {
							absolute = directory;
						} else {
							/*
							 * Try to find the directory case-insensitively.
							 * */
							bool found = false;
							if (System.IO.Directory.Exists(absolute)) {
								string[] directories = System.IO.Directory.GetDirectories(absolute);
								for (int j = 0; j < directories.Length; j++) {
									string name = System.IO.Path.GetFileName(directories[j]);
									if (name.Equals(parts[i], StringComparison.OrdinalIgnoreCase)) {
										absolute = directories[j];
										found = true;
										break;
									}
								}
							}
							if (!found) {
								absolute = directory;
							}
						}
					}
				}
			}
			throw new ArgumentException("The reference to the file is malformed.");
		}
		
		
		// --- private functions ---
		
		/// <summary>Checks whether the specified string consists only of periods.</summary>
		/// <param name="text">The string to check.</param>
		/// <returns>Whether the string consists only of periods.</returns>
		private static bool IsAllPeriods(string text) {
			for (int i = 0; i < text.Length; i++) {
				if (text[i] != '.') {
					return false;
				}
			}
			return true;
		}
		
		/// <summary>Resolves a package reference in the relative path and adjusts the absolute path if found.</summary>
		/// <param name="absolute">The absolute path.</param>
		/// <param name="relative">The relative path.</param>
		private static void ResolvePackageReference(ref string absolute, ref string relative) {
			if (relative.Length != 0 && relative[0] == '$') {
				int index = relative.IndexOfAny(new char[] { '/', '\\' });
				if (index >= 0) {
					string package = relative.Substring(1, index - 1);
					relative = relative.Substring(index + 1);
					if (PackageNames != null) {
						index = Array.BinarySearch<string>(PackageNames, package);
						if (index >= 0 & index < PackageNames.Length) {
							absolute = PackageDirectories[index];
							return;
						}
					}
					throw new System.IO.DirectoryNotFoundException("The package " + package + " could not be found.");
				}
			}
		}
		/// <summary>
		/// Checks whether the specified path contains any invalid characters.
		/// </summary>
		/// <remarks>Invalid characters are stored in <see cref="InvalidPathChars"/>.</remarks>
		/// <returns><c>true</c>, if invalid path is invalid, <c>false</c> otherwise.</returns>
		/// <param name="Expression">The path to check.</param>
		public static bool ContainsInvalidPathChars(string Expression) {
			for (int i = 0; i < Expression.Length; i++) {
				for (int j = 0; j < InvalidPathChars.Length; j++) {
					if (Expression[i] == InvalidPathChars[j])
						return true;
				}
			}
			return false;
		}
	}
}