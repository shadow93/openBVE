using System;
using System.Text;

namespace TrainsimApi {
	/// <summary>Provides functions for combining paths in a platform-agnostic way.</summary>
	public static partial class Platform {

		
		// --- read-only fields ---
		
		private static readonly char[] InvalidPathChars = new char[] { ':', '*', '?', '"', '<', '>', '|' };
		
		private static readonly char[] PathSeparationChars = new char[] { '/', '\\' };
		
		
		// --- public functions ---
		
		public static string CombineDirectory(string absolute, string relative) {
			if (relative.IndexOfAny(InvalidPathChars) >= 0) {
				throw new ArgumentException("The relative path contains invalid characters.");
			}
			string[] parts = relative.Split(PathSeparationChars, StringSplitOptions.RemoveEmptyEntries);
			for (int i = 0; i < parts.Length; i++) {
				if (parts[i].Length != 0) {
					if (IsAllPeriods(parts[i])) {
						for (int j = 1; j < parts[i].Length; j++) {
							absolute = System.IO.Path.GetDirectoryName(absolute);
						}
					} else {
						string directory = System.IO.Path.Combine(absolute, parts[i]);
						if (System.IO.Directory.Exists(directory)) {
							absolute = directory;
						} else {
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
		
		public static string CombineFile(string absolute, string relative) {
			if (relative.IndexOfAny(InvalidPathChars) >= 0) {
				throw new ArgumentException("The relative path contains invalid characters.");
			}
			string[] parts = relative.Split(PathSeparationChars, StringSplitOptions.RemoveEmptyEntries);
			for (int i = 0; i < parts.Length; i++) {
				if (parts[i].Length != 0) {
					if (IsAllPeriods(parts[i])) {
						if (i == parts.Length - 1) {
							throw new ArgumentException("The relative path is malformed.");
						} else {
							for (int j = 1; j < parts[i].Length; j++) {
								absolute = System.IO.Path.GetDirectoryName(absolute);
							}
						}
					} else if (i == parts.Length - 1) {
						string file = System.IO.Path.Combine(absolute, parts[i]);
						if (System.IO.File.Exists(file)) {
							return file;
						} else {
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
						string directory = System.IO.Path.Combine(absolute, parts[i]);
						if (System.IO.Directory.Exists(directory)) {
							absolute = directory;
						} else {
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
		
		private static bool IsAllPeriods(string text) {
			for (int i = 0; i < text.Length; i++) {
				if (text[i] != '.') {
					return false;
				}
			}
			return true;
		}

		
	}
}