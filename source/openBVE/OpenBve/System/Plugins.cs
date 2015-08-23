using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

namespace OpenBve {
	/// <summary>Represents plugins loaded by the program.</summary>
	internal static class Plugins {
		
		// --- classes ---
		
		/// <summary>Represents a plugin.</summary>
		internal class Plugin {
			// --- members ---
			/// <summary>The plugin file.</summary>
			internal string File;
			/// <summary>The plugin title.</summary>
			internal string Title;
			/// <summary>The interface to load textures as exposed by the plugin, or a null reference.</summary>
			internal OpenBveApi.Textures.TextureInterface[] TextureLoaders;
			/// <summary>The interface to load sounds as exposed by the plugin, or a null reference.</summary>
			internal OpenBveApi.Sounds.SoundInterface[] SoundLoaders;
			/// <summary>The interface to load objects as exposed by the plugin, or a null reference.</summary>
			internal OpenBveApi.Objects.ObjectInterface[] ObjectLoaders;
			// --- constructors ---
			/// <summary>Creates a new instance of this class.</summary>
			/// <param name="file">The plugin file.</param>
			internal Plugin(string file) {
				this.File = file;
				this.Title = Path.GetFileName(file);
				this.TextureLoaders = null;
				this.SoundLoaders = null;
				this.ObjectLoaders = null;
			}
			// --- functions ---
			/// <summary>Loads all interfaces this plugin supports.</summary>
			internal void Load() {
				if (this.TextureLoaders != null) {
					foreach(var iface in this.TextureLoaders)
						iface.Load(Program.CurrentHost);
				}
				if (this.SoundLoaders != null) {
					foreach(var iface in this.SoundLoaders)
						iface.Load(Program.CurrentHost);
				}
				if (this.ObjectLoaders != null) {
					foreach(var iface in this.ObjectLoaders)
						iface.Load(Program.CurrentHost);
				}
			}
			/// <summary>Unloads all interfaces this plugin supports.</summary>
			internal void Unload() {
				if (this.TextureLoaders != null) {
					for(int i = 0; i < this.TextureLoaders.Length; i++)
						this.TextureLoaders[i].Unload();
				}
				if (this.SoundLoaders != null) {
					for(int i = 0; i < this.SoundLoaders.Length; i++)
						this.SoundLoaders[i].Unload();
				}
				if (this.ObjectLoaders != null) {
					for(int i = 0; i < this.ObjectLoaders.Length; i++)
						this.ObjectLoaders[i].Unload();
				}
			}
		}
		
		
		// --- members ---
		
		/// <summary>A list of all non-runtime plugins that are currently loaded, or a null reference.</summary>
		internal static Plugin[] LoadedPlugins = null;
		
		
		// --- functions ---

		/// <summary>Loads all non-runtime plugins.</summary>
		/// <returns>Whether loading all plugins was successful.</returns>
		internal static bool LoadPlugins() {
			UnloadPlugins();
			string folder = Program.FileSystem.GetDataFolder("Plugins");
			string[] files = Directory.GetFiles(folder);
			List<Plugin> list = new List<Plugin>();
			StringBuilder builder = new StringBuilder();
			foreach (string file in files) {
				if (file.EndsWith(".dll", StringComparison.OrdinalIgnoreCase)) {
					#if !DEBUG
					try {
						#endif
						Plugin plugin = new Plugin(file);
						Assembly assembly = Assembly.LoadFile(file);
						Type[] types = assembly.GetTypes();
						bool iruntime = false;
					var textures = new List<OpenBveApi.Textures.TextureInterface>();
					var sounds = new List<OpenBveApi.Sounds.SoundInterface>();
					var objects = new List<OpenBveApi.Objects.ObjectInterface>();
						foreach (Type type in types) {
							if (type.IsSubclassOf(typeof(OpenBveApi.Textures.TextureInterface))) {
								textures.Add((OpenBveApi.Textures.TextureInterface)assembly.CreateInstance(type.FullName));
							}
							if (type.IsSubclassOf(typeof(OpenBveApi.Sounds.SoundInterface))) {
								sounds.Add((OpenBveApi.Sounds.SoundInterface)assembly.CreateInstance(type.FullName));
							}
							if (type.IsSubclassOf(typeof(OpenBveApi.Objects.ObjectInterface))) {
								objects.Add((OpenBveApi.Objects.ObjectInterface)assembly.CreateInstance(type.FullName));
							}
							iruntime |= typeof(OpenBveApi.Runtime.IRuntime).IsAssignableFrom(type);
						}
					plugin.TextureLoaders = textures.ToArray();
					plugin.SoundLoaders = sounds.ToArray();
					plugin.ObjectLoaders = objects.ToArray();
					if (plugin.TextureLoaders.Length > 0 || plugin.SoundLoaders.Length > 0 || plugin.ObjectLoaders.Length > 0) {
							plugin.Load();
							list.Add(plugin);
						} else if (!iruntime) {
							builder.Append("Plugin ").Append(Path.GetFileName(file)).AppendLine(" does not implement compatible interfaces.");
							builder.AppendLine();
						}
						#if !DEBUG
					} catch (Exception ex) {
						builder.Append("Could not load plugin ").Append(Path.GetFileName(file)).AppendLine(":").AppendLine(ex.Message);
						builder.AppendLine();
					}
					#endif
				}
			}
			LoadedPlugins = list.ToArray();
			string message = builder.ToString().Trim();
			if (message.Length != 0)
				return MessageBox.Show(message + "Do you want to continue loading?", Application.ProductName, 
					MessageBoxButtons.YesNo, MessageBoxIcon.Hand, MessageBoxDefaultButton.Button2) == DialogResult.Yes;
			return true;
		}
		
		/// <summary>Unloads all non-runtime plugins.</summary>
		internal static void UnloadPlugins() {
			StringBuilder builder = new StringBuilder();
			if (LoadedPlugins != null) {
				foreach (Plugin plugin in LoadedPlugins) {
					#if !DEBUG
					try {
						#endif
						plugin.Unload();
						#if !DEBUG
					} catch (Exception ex) {
						builder.Append("Could not unload plugin ").Append(plugin.Title).AppendLine(":").AppendLine(ex.Message);
						builder.AppendLine();
					}
					#endif
				}
				LoadedPlugins = null;
			}
			string message = builder.ToString().Trim();
			if (message.Length != 0) {
				MessageBox.Show(message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Hand);
			}
		}
		
	}
}