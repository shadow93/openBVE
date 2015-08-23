/*
 * DO NOT COMPILE - CLASS WAS DISINTEGRATED
using System;
using System.Globalization;
using OpenBveApi.Colors;
using OpenTK.Input;

namespace OpenBve {
	internal static class Interface {
//		// get corrected path separation
//		internal static string GetCorrectedPathSeparation(string Expression) {
//			if (Program.CurrentPlatform == Program.Platform.Windows) {
//				if (Expression.Length != 0 && Expression[0] == '\\') {
//					return Expression.Substring(1);
//				} else {
//					return Expression;
//				}
//			} else {
//				if (Expression.Length != 0 && Expression[0] == '\\') {
//					return Expression.Substring(1).Replace("\\", new string(new char[] { System.IO.Path.DirectorySeparatorChar }));
//				} else {
//					return Expression.Replace("\\", new string(new char[] { System.IO.Path.DirectorySeparatorChar }));
//				}
//			}
//		}

//		// get corected folder name
//		internal static string GetCorrectedFolderName(string Folder) {
//			if (Folder.Length == 0) {
//				return "";
//			} else if (Program.CurrentPlatform == Program.Platform.Linux) {
//				// find folder case-insensitively
//				if (System.IO.Directory.Exists(Folder)) {
//					return Folder;
//				} else {
//					string Parent = GetCorrectedFolderName(System.IO.Path.GetDirectoryName(Folder));
//					Folder = System.IO.Path.Combine(Parent, System.IO.Path.GetFileName(Folder));
//					if (Folder != null && System.IO.Directory.Exists(Parent)) {
//						if (System.IO.Directory.Exists(Folder)) {
//							return Folder;
//						} else {
//							string[] Folders = System.IO.Directory.GetDirectories(Parent);
//							for (int i = 0; i < Folders.Length; i++) {
//								if (string.Compare(Folder, Folders[i], StringComparison.OrdinalIgnoreCase) == 0) {
//									return Folders[i];
//								}
//							}
//						}
//					}
//					return Folder;
//				}
//			} else {
//				return Folder;
//			}
//		}
		
//		// get corrected file name
//		internal static string GetCorrectedFileName(string File) {
//			if (File.Length == 0) {
//				return "";
//			} else if (Program.CurrentPlatform == Program.Platform.Linux) {
//				// find file case-insensitively
//				if (System.IO.File.Exists(File)) {
//					return File;
//				} else {
//					string Folder = GetCorrectedFolderName(System.IO.Path.GetDirectoryName(File));
//					File = System.IO.Path.Combine(Folder, System.IO.Path.GetFileName(File));
//					if (System.IO.Directory.Exists(Folder)) {
//						if (System.IO.File.Exists(File)) {
//							return File;
//						} else {
//							string[] Files = System.IO.Directory.GetFiles(Folder);
//							for (int i = 0; i < Files.Length; i++) {
//								if (string.Compare(File, Files[i], StringComparison.OrdinalIgnoreCase) == 0) {
//									return Files[i];
//								}
//							}
//						}
//					}
//					return File;
//				}
//			} else {
//				return File;
//			}
//		}

//		// get combined file name
//		internal static string OpenBveApi.Path.CombineFile(string SafeFolderPart, string UnsafeFilePart) {
//			return GetCorrectedFileName(System.IO.Path.Combine(SafeFolderPart, GetCorrectedPathSeparation(UnsafeFilePart)));
//		}
		
//		// get combined folder name
//		internal static string OpenBveApi.Path.CombineDirectory(string SafeFolderPart, string UnsafeFolderPart) {
//			return GetCorrectedFolderName(System.IO.Path.Combine(SafeFolderPart, GetCorrectedPathSeparation(UnsafeFolderPart)));
//		}
	}
}*/