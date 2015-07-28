using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

using TrainsimApi.Codecs;
using TrainsimApi.Geometry;
using TrainsimApi.Vectors;

namespace OpenBveObjectValidator {
	public partial class MainForm : Form {
		public MainForm() {
			InitializeComponent();
		}
		
		
		// --- classes ---
		
		private class FileData {
			internal string FullPath;
			internal string ErrorMessage;
			internal ListViewItem Item;
			internal FileData(string fullPath, ListViewItem item) {
				this.FullPath = fullPath;
				this.ErrorMessage = null;
				this.Item = item;
			}
		}
		
		private class MyTexture : Texture {
			private string File;
			private Bitmap Bitmap;
			private bool TransparentColorUsed;
			private Vector3b TransparentColor;
			public MyTexture(Bitmap bitmap) {
				this.File = null;
				this.Bitmap = bitmap;
				this.TransparentColorUsed = false;
				this.TransparentColor = Vector3b.Black;
			}
			public MyTexture(string file, Vector3b transparentColor) {
				this.File = file;
				this.Bitmap = null;
				this.TransparentColorUsed = true;
				this.TransparentColor = transparentColor;
			}
			public MyTexture(string file) {
				this.File = file;
				this.Bitmap = null;
				this.TransparentColorUsed = false;
				this.TransparentColor = Vector3b.Black;
			}
			public override int GetHashCode() {
				int hashCode = 0;
				unchecked {
					if (File != null) {
						hashCode += 1000000007 * File.GetHashCode();
					}
					if (Bitmap != null) {
						hashCode += 1000000009 * Bitmap.GetHashCode();
					}
					hashCode += 1000000021 * TransparentColorUsed.GetHashCode();
					if (this.TransparentColorUsed) {
						hashCode += 1000000033 * TransparentColor.GetHashCode();
					}
				}
				return hashCode;
			}
			public override bool Equals(object obj) {
				MyTexture t = obj as MyTexture;
				if (t == null) return false;
				if (this.File != t.File) return false;
				if (this.Bitmap != t.Bitmap) return false;
				if (this.TransparentColorUsed != t.TransparentColorUsed) return false;
				if (this.TransparentColorUsed) {
					if (this.TransparentColor != t.TransparentColor) return false;
				}
				return true;
			}
			public override bool Equals(Texture other) {
				MyTexture t = other as MyTexture;
				if (t == null) return false;
				if (this.File != t.File) return false;
				if (this.Bitmap != t.Bitmap) return false;
				if (this.TransparentColorUsed != t.TransparentColorUsed) return false;
				if (this.TransparentColorUsed) {
					if (this.TransparentColor != t.TransparentColor) return false;
				}
				return true;
			}
		}
		
		private class MyTextureManager : TextureManager {
			private List<MyTexture> Textures;
			internal MyTextureManager() {
				this.Textures = new List<MyTexture>();
			}
			public override Texture Add(Bitmap bitmap) {
				this.Textures.Add(new MyTexture(bitmap));
				return this.Textures[this.Textures.Count - 1];
			}
			public override Texture Add(string file, Vector3b transparentColor) {
				MyTexture t = new MyTexture(file, transparentColor);
				for (int i = 0; i < this.Textures.Count; i++) {
					if (this.Textures[i].Equals(t)) {
						return this.Textures[i];
					}
				}
				this.Textures.Add(t);
				return this.Textures[this.Textures.Count - 1];
			}
			public override Texture Add(string file) {
				MyTexture t = new MyTexture(file);
				for (int i = 0; i < this.Textures.Count; i++) {
					if (this.Textures[i].Equals(t)) {
						return this.Textures[i];
					}
				}
				this.Textures.Add(t);
				return this.Textures[this.Textures.Count - 1];
			}
		}
		
		private class MyErrorLogger : ErrorLogger {
			internal StringBuilder Builder = new StringBuilder();
			internal int Count;
			public MyErrorLogger() {
				this.Builder = new StringBuilder();
				this.Count = 0;
			}
			public override void Add(string text) {
				if (this.Builder.Length != 0) {
					this.Builder.AppendLine();
				}
				this.Builder.AppendLine(text);
				this.Count++;
			}
		}
		
		private class MyListViewItemSorter : IComparer {
			private int Column;
			private int Multiplier;
			internal MyListViewItemSorter(int column, bool descending) {
				this.Column = column;
				this.Multiplier = descending ? -1 : 1;
			}
			public int Compare(object x, object y) {
				return this.Multiplier * string.Compare(((ListViewItem)x).SubItems[this.Column].Text, ((ListViewItem)y).SubItems[this.Column].Text);
			}
		}
		
		
		// --- events ---
		
		private void ButtonAddFilesClick(object sender, EventArgs e) {
			var dialog = new OpenFileDialog();
			dialog.CheckFileExists = true;
			dialog.Filter = "Compatible files|*.csv;*.b3d|CSV files|*.csv|B3D files|*.b3d|All files|*";
			dialog.Multiselect = true;
			if (dialog.ShowDialog() == DialogResult.OK) {
				string[] files = dialog.FileNames;
				this.Cursor = Cursors.WaitCursor;
				AddFiles(files);
				this.Cursor = Cursors.Default;
			}
		}
		
		private void ButtonAddDirectoryClick(object sender, EventArgs e) {
			var dialog = new FolderBrowserDialog();
			if (dialog.ShowDialog() == DialogResult.OK) {
				List<string> files = new List<string>();
				switch (MessageBox.Show("Include subdirectories?", "Add directory", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question)) {
					case DialogResult.Yes:
						AddDirectory(dialog.SelectedPath, true, files);
						break;
					case DialogResult.No:
						AddDirectory(dialog.SelectedPath, false, files);
						break;
					default:
						break;
				}
				if (files.Count != 0) {
					this.Cursor = Cursors.WaitCursor;
					AddFiles(files.ToArray());
					this.Cursor = Cursors.Default;
				}
			}
		}
		
		private void ButtonClearListClick(object sender, EventArgs e) {
			listviewFiles.Items.Clear();
		}
		
		private void ButtonValidateClick(object sender, EventArgs e) {
			foreach (ListViewItem item in listviewFiles.Items) {
				FileData data = item.Tag as FileData;
				if (data != null) {
					data.ErrorMessage = null;
					#if !DEBUG
					Task.Factory.StartNew(() => ValidateData(data));
					#else
					ValidateData(data);
					#endif
				}
			}
		}
		
		private void ButtonRemoveValidFilesClick(object sender, EventArgs e) {
			for (int i = 0; i < listviewFiles.Items.Count; i++) {
				FileData data = listviewFiles.Items[i].Tag as FileData;
				if (data != null && data.ErrorMessage == string.Empty) {
					listviewFiles.Items.RemoveAt(i);
					i--;
				}
			}
		}
		
		private void ListviewFilesSelectedIndexChanged(object sender, EventArgs e) {
			if (listviewFiles.SelectedItems.Count != 0) {
				FileData data = listviewFiles.SelectedItems[0].Tag as FileData;
				if (data != null) {
					string prefix = data.FullPath + Environment.NewLine + Environment.NewLine + "--------------------" + Environment.NewLine + Environment.NewLine;
					string suffix;
					if (data.ErrorMessage == null) {
						suffix = "Processing. Please wait...";
					} else if (data.ErrorMessage == string.Empty) {
						suffix = "No errors.";
					} else {
						suffix = data.ErrorMessage;
					}
					textboxErrors.Text = prefix + suffix;
				} else {
					textboxErrors.Text = "Internal error.";
				}
			} else {
				textboxErrors.Text = "No file selected.";
			}
		}
		
		private void ListviewFilesColumnClick(object sender, ColumnClickEventArgs e) {
			listviewFiles.ListViewItemSorter = new MyListViewItemSorter(e.Column, e.Column == 1);
			listviewFiles.Sort();
		}
		
		
		// --- functions ---
		
		private void AddFiles(string[] files) {
			listviewFiles.SuspendLayout();
			listviewFiles.BeginUpdate();
			foreach (string file in files) {
				bool add = true;
				foreach (ListViewItem item in listviewFiles.Items) {
					FileData data = item.Tag as FileData;
					if (data != null && data.FullPath.Equals(file, StringComparison.OrdinalIgnoreCase)) {
						add = false;
						break;
					}
				}
				if (add) {
					ListViewItem item = new ListViewItem(Path.GetFileName(file));
					item.SubItems.Add("Processing...");
					var data = new FileData(file, item);
					item.Tag = data;
					listviewFiles.Items.Add(item);
				}
			}
			listviewFiles.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
			int[] widths = new int[listviewFiles.Columns.Count];
			for (int i = 0; i < listviewFiles.Columns.Count; i++) {
				widths[i] = listviewFiles.Columns[i].Width;
			}
			listviewFiles.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
			for (int i = 0; i < listviewFiles.Columns.Count; i++) {
				if (widths[i] > listviewFiles.Columns[i].Width) {
					listviewFiles.Columns[i].Width = widths[i];
				}
			}
			listviewFiles.EndUpdate();
			listviewFiles.ResumeLayout();
			ButtonValidateClick(null, null);
		}
		
		private void AddDirectory(string path, bool includeSubDirectories, List<string> listOfFiles) {
			string[] files = Directory.GetFiles(path);
			foreach (string file in files) {
				if (file.EndsWith(".csv", StringComparison.OrdinalIgnoreCase) || file.EndsWith(".b3d", StringComparison.OrdinalIgnoreCase)) {
					listOfFiles.Add(file);
				}
			}
			if (includeSubDirectories) {
				string[] subDirectories = Directory.GetDirectories(path);
				foreach (string subDirectory in subDirectories) {
					AddDirectory(subDirectory, true, listOfFiles);
				}
			}
		}
		
		private void ValidateData(FileData data) {
			var decoder = new CsvB3dDecoder.Decoder();
			var manager = new MyTextureManager();
			var logger = new MyErrorLogger();
			var options = new TrainsimApi.Codecs.MeshDecodingOptions(manager, logger);
			#if !DEBUG
			try {
				#endif
				Mesh mesh = decoder.Load(data.FullPath, options);
				Validator.CheckMeshFaces(mesh, logger);
				data.ErrorMessage = logger.Builder.ToString();
				#if !DEBUG
			} catch (Exception ex) {
				data.ErrorMessage = "Unhandled exception:" + Environment.NewLine + ex.Message + Environment.NewLine + Environment.NewLine + "Please forward this error to the author of this program along with the file that caused the error.";
			}
			#endif
			listviewFiles.Invoke(new ThreadStart(() => {
			                                     	if (listviewFiles.Items.Contains(data.Item)) {
			                                     		data.Item.SubItems[1].Text = logger.Count.ToString();
			                                     		if (listviewFiles.SelectedItems.Count != 0 && listviewFiles.SelectedItems[0] == data.Item) {
			                                     			ListviewFilesSelectedIndexChanged(null, null);
			                                     		}
			                                     	}
			                                     }));
		}
		
		
	}
}