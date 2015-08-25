using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace OpenBve {
	/// <summary>Provides structures and functions for dealing with managed content.</summary>
	internal static partial class ManagedContent {
		
		/// <summary>Represents a source where a package can be downloaded from.</summary>
		internal struct Source {
			// --- members ---
			/// <summary>The size of the package.</summary>
			internal int Size;
			/// <summary>The MD5 of the package.</summary>
			internal byte[] MD5;
			/// <summary>The URL to the download.</summary>
			internal string Url;
		}
		
		/// <summary>Represents a reference to another package by name and version.</summary>
		internal struct Dependency {
			// --- members ---
			/// <summary>The package name.</summary>
			internal string Name;
			/// <summary>The package version.</summary>
			internal string Version;
			// --- constructors ---
			/// <summary>Creates a new dependency.</summary>
			/// <param name="name">The package name.</param>
			/// <param name="version">The package version.</param>
			internal Dependency(string name, string version) {
				this.Name = name;
				this.Version = version;
			}
		}
		
		/// <summary>Represents a key-value pair.</summary>
		internal struct KeyValuePair {
			// --- members ---
			/// <summary>The key.</summary>
			internal string Key;
			/// <summary>The language code, or a null reference.</summary>
			internal string Language;
			/// <summary>The value.</summary>
			internal string Value;
			// --- constructors ---
			/// <summary>Creates a new key-value pair.</summary>
			/// <param name="key">The key including the language code.</param>
			/// <param name="value">The value</param>
			internal KeyValuePair(string key, string value) {
				int index = key.IndexOf('[');
				if (index >= 0 && key.Length != 0 && key[key.Length - 1] == ']') {
					this.Key = key.Substring(0, index).TrimEnd();
					this.Language = key.Substring(index + 1, key.Length - index - 2).Trim();
					this.Value = value;
				} else {
					this.Key = key;
					this.Language = null;
					this.Value = value;
				}
			}
			// --- functions ---
			/// <summary>Gets the textual representation of this key-value pair.</summary>
			/// <returns>The textual representation of this key-value pair</returns>
			public override string ToString() {
				if (this.Language != null)
					return this.Key + "[" + this.Language + "] = " + this.Value;
				return this.Key + " = " + this.Value;
			}
		}
		
		/// <summary>Represents a specific version of a package.</summary>
		internal class Version {
			// --- members ---
			/// <summary>The package name.</summary>
			internal string Name;
			/// <summary>The version number.</summary>
			internal string Number;
			/// <summary>The list of available sources.</summary>
			internal Source[] Sources;
			/// <summary>The list of dependencies.</summary>
			internal Dependency[] Dependencies;
			/// <summary>The list of suggestions.</summary>
			internal Dependency[] Suggestions;
			/// <summary>The list of metadata.</summary>
			internal KeyValuePair[] Metadata;
			// --- functions ---
			/// <summary>Gets the value associated to the specified key.</summary>
			/// <param name="key">The key without the language code.</param>
			/// <param name="languageCode">The preferred language code, or null if no language-specific value is expected.</param>
			/// <param name="defaultValue">The default value in case the key is not found.</param>
			/// <returns>The value.</returns>
			internal string GetMetadata(string key, string languageCode, string defaultValue) {
				return ManagedContent.GetMetadata(this.Metadata, key, languageCode, defaultValue);
			}
			/// <summary>Checks whether the specified keyword is contained in the metadata.</summary>
			/// <param name="keyword">The keyword.</param>
			/// <returns>Whether the specified keyword is contained in the metadata.</returns>
			internal bool ContainsKeyword(string keyword) {
				for (int i = 0; i < this.Metadata.Length; i++) {
					if (this.Metadata[i].Value.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) >= 0) {
						return true;
					}
				}
				return false;
			}
		}
		
		/// <summary>Represents a package.</summary>
		internal class Package {
			// --- members ---
			/// <summary>The package name.</summary>
			internal string Name;
			/// <summary>The list of available versions.</summary>
			internal Version[] Versions;
		}
		
		/// <summary>Represents a database of available packages.</summary>
		internal partial class Database {
			// --- members ---
			/// <summary>The list of available packages.</summary>
			internal Package[] Packages;
			// --- load ---
			/// <summary>Loads a database from a byte array.</summary>
			/// <param name="bytes">The byte array.</param>
			/// <returns>The database.</returns>
			internal static Database Load(byte[] bytes) {
				/*
				 * Parse the enclosing file format that holds the
				 * compressed (gzip) data and then decompress the data.
				 * */
				int version;
				byte[] compressed;
				byte[] md5;
				using (MemoryStream stream = new MemoryStream(bytes)) {
					using (BinaryReader reader = new BinaryReader(stream, Encoding.UTF8)) {
						// 0x5453494C5F46535 = "TSF_LIST"
						if (reader.ReadUInt64() != 0x5453494C5F465354) {
							throw new InvalidDataException("The identifier in the header is invalid.");
						}
						version = reader.ReadInt32();
						if (version != 2) {
							throw new InvalidDataException("The version number in the header is invalid.");
						}
						md5 = reader.ReadBytes(16);
						int length = reader.ReadInt32();
						compressed = reader.ReadBytes(length);
						// 0x444E455F = "_END"
						if (reader.ReadUInt32() != 0x444E455F) {
							throw new InvalidDataException("The identifier in the footer is invalid.");
						}
					}
				}
				byte[] check = (new MD5CryptoServiceProvider()).ComputeHash(compressed);
				for (int i = 0; i < 16; i++) {
					if (md5[i] != check[i]) {
						throw new InvalidDataException("The MD5 does not match.");
					}
				}
				byte[] decompressed = Gzip.Decompress(compressed);
				/*
				 * Parse the raw file format and extract the database.
				 * */
				Database database = new Database();
				using (MemoryStream stream = new MemoryStream(decompressed)) {
					using (BinaryReader reader = new BinaryReader(stream)) {
						// 0x74727473 = "strt"
						if (reader.ReadUInt32() != 0x74727473) {
							throw new InvalidDataException("The uncompressed stream is invalid.");
						}
						database.Packages = new Package[reader.ReadInt32()];
						for (int i = 0; i < database.Packages.Length; i++) {
							database.Packages[i] = new Package();
							database.Packages[i].Name = reader.ReadString();
							database.Packages[i].Versions = new Version[reader.ReadInt32()];
							for (int j = 0; j < database.Packages[i].Versions.Length; j++) {
								database.Packages[i].Versions[j] = new Version();
								database.Packages[i].Versions[j].Name = database.Packages[i].Name;
								database.Packages[i].Versions[j].Number = reader.ReadString();
								database.Packages[i].Versions[j].Sources = new Source[reader.ReadInt32()];
								for (int k = 0; k < database.Packages[i].Versions[j].Sources.Length; k++) {
									database.Packages[i].Versions[j].Sources[k].Size = reader.ReadInt32();
									database.Packages[i].Versions[j].Sources[k].MD5 = reader.ReadBytes(16);
									database.Packages[i].Versions[j].Sources[k].Url = reader.ReadString();
								}
								database.Packages[i].Versions[j].Dependencies = new Dependency[reader.ReadInt32()];
								for (int k = 0; k < database.Packages[i].Versions[j].Dependencies.Length; k++) {
									database.Packages[i].Versions[j].Dependencies[k] = new Dependency();
									database.Packages[i].Versions[j].Dependencies[k].Name = reader.ReadString();
									database.Packages[i].Versions[j].Dependencies[k].Version = reader.ReadString();
								}
								database.Packages[i].Versions[j].Suggestions = new Dependency[reader.ReadInt32()];
								for (int k = 0; k < database.Packages[i].Versions[j].Suggestions.Length; k++) {
									database.Packages[i].Versions[j].Suggestions[k] = new Dependency();
									database.Packages[i].Versions[j].Suggestions[k].Name = reader.ReadString();
									database.Packages[i].Versions[j].Suggestions[k].Version = reader.ReadString();
								}
								database.Packages[i].Versions[j].Metadata = new KeyValuePair[reader.ReadInt32()];
								for (int k = 0; k < database.Packages[i].Versions[j].Metadata.Length; k++) {
									string key = reader.ReadString();
									string value = reader.ReadString();
									database.Packages[i].Versions[j].Metadata[k] = new KeyValuePair(key, value);
								}
							}
						}
						if (reader.ReadUInt32() != 0x646E655F) {
							throw new InvalidDataException("The uncompressed stream is invalid.");
						}
					}
				}
				return database;
			}
			// --- dereference ---
			/// <summary>Dereferences the specified package by name.</summary>
			/// <param name="name">The package name.</param>
			/// <returns>The package, or a null reference if not found.</returns>
			internal Package Dereference(string name) {
				for (int i = 0; i < this.Packages.Length; i++) {
					if (string.Equals(this.Packages[i].Name, name, StringComparison.OrdinalIgnoreCase)) {
						return this.Packages[i];
					}
				}
				return null;
			}
			/// <summary>Dereferences the specified package by name and version.</summary>
			/// <param name="name">The package name.</param>
			/// <param name="version">The version number.</param>
			/// <returns>The latest version of the package if the specified version number or higher is found - a null reference otherwise.</returns>
			internal Version Dereference(string name, string version) {
				for (int i = 0; i < this.Packages.Length; i++) {
					if (string.Equals(this.Packages[i].Name, name, StringComparison.OrdinalIgnoreCase)) {
						Version result = null;
						for (int j = 0; j < this.Packages[i].Versions.Length; j++) {
							if (result == null) {
								int delta = CompareVersions(this.Packages[i].Versions[j].Number, version);
								if (delta >= 0) {
									result = this.Packages[i].Versions[j];
								}
							} else {
								int delta = CompareVersions(this.Packages[i].Versions[j].Number, result.Number);
								if (delta > 0) {
									result = this.Packages[i].Versions[j];
								}
							}
						}
						return result;
					}
				}
				return null;
			}
		}
		
		// --- static functions ---
		
		/// <summary>Gets the value associated to the specified key.</summary>
		/// <param name="pairs">The list of key-value pairs.</param>
		/// <param name="key">The key without the language code.</param>
		/// <param name="preferredLanguage">The preferred language code, or null if no language-specific value is expected.</param>
		/// <param name="defaultValue">The default value in case the key is not found.</param>
		/// <returns>The value.</returns>
		internal static string GetMetadata(KeyValuePair[] pairs, string key, string preferredLanguage, string defaultValue) {
			if (preferredLanguage == null) {
				/* No language code is expected. */
				for (int i = 0; i < pairs.Length; i++) {
					if (string.Equals(pairs[i].Key, key, StringComparison.OrdinalIgnoreCase)) {
						return pairs[i].Value;
					}
				}
			} else {
				/* A language code is expected. Let's first search for the exact language. */
				for (int i = 0; i < pairs.Length; i++) {
					if (string.Equals(pairs[i].Key, key, StringComparison.OrdinalIgnoreCase)) {
						if (pairs[i].Language != null) {
							if (string.Equals(pairs[i].Language, preferredLanguage, StringComparison.OrdinalIgnoreCase)) {
								return pairs[i].Value;
							}
						}
					}
				}
				/* Let's search for the same language family. */
				int index = preferredLanguage.IndexOf('-');
				string preferredFamily = index >= 0 ? preferredLanguage.Substring(0, index) : preferredLanguage;
				for (int i = 0; i < pairs.Length; i++) {
					if (string.Equals(pairs[i].Key, key, StringComparison.OrdinalIgnoreCase)) {
						if (pairs[i].Language != null) {
							index = pairs[i].Language.IndexOf('-');
							string family = index >= 0 ? pairs[i].Language.Substring(0, index) : pairs[i].Language;
							if (string.Equals(family, preferredFamily, StringComparison.OrdinalIgnoreCase)) {
								return pairs[i].Value;
							}
						}
					}
				}
				/* Let's search for the American English language. */
				for (int i = 0; i < pairs.Length; i++) {
					if (string.Equals(pairs[i].Key, key, StringComparison.OrdinalIgnoreCase)) {
						if (pairs[i].Language != null) {
							if (string.Equals(pairs[i].Language, "en-US", StringComparison.OrdinalIgnoreCase)) {
								return pairs[i].Value;
							}
						}
					}
				}
				/* Let's search for any English language. */
				for (int i = 0; i < pairs.Length; i++) {
					if (string.Equals(pairs[i].Key, key, StringComparison.OrdinalIgnoreCase)) {
						if (pairs[i].Language != null) {
							index = pairs[i].Language.IndexOf('-');
							string family = index >= 0 ? pairs[i].Language.Substring(0, index) : pairs[i].Language;
							if (string.Equals(family, "en", StringComparison.OrdinalIgnoreCase)) {
								return pairs[i].Value;
							}
						}
					}
				}
				/* Let's return any language. */
				for (int i = 0; i < pairs.Length; i++) {
					if (string.Equals(pairs[i].Key, key, StringComparison.OrdinalIgnoreCase)) {
						return pairs[i].Value;
					}
				}
			}
			return defaultValue;
		}
		
	}
}