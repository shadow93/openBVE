
namespace OpenBveObjectValidator
{
	partial class MainForm
	{
		/// <summary>
		/// Designer variable used to keep track of non-visual components.
		/// </summary>
		private System.ComponentModel.IContainer components = null;
		
		/// <summary>
		/// Disposes resources used by the form.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing) {
				if (components != null) {
					components.Dispose();
				}
			}
			base.Dispose(disposing);
		}
		
		/// <summary>
		/// This method is required for Windows Forms designer support.
		/// Do not change the method contents inside the source code editor. The Forms designer might
		/// not be able to load this method if it was changed manually.
		/// </summary>
		private void InitializeComponent()
		{
			this.listviewFiles = new System.Windows.Forms.ListView();
			this.columnheaderFile = new System.Windows.Forms.ColumnHeader();
			this.columnheaderErrors = new System.Windows.Forms.ColumnHeader();
			this.buttonAddFiles = new System.Windows.Forms.Button();
			this.buttonAddDirectory = new System.Windows.Forms.Button();
			this.buttonClearList = new System.Windows.Forms.Button();
			this.textboxErrors = new System.Windows.Forms.TextBox();
			this.buttonValidate = new System.Windows.Forms.Button();
			this.buttonRemoveValidFiles = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// listviewFiles
			// 
			this.listviewFiles.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
									| System.Windows.Forms.AnchorStyles.Left)));
			this.listviewFiles.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
									this.columnheaderFile,
									this.columnheaderErrors});
			this.listviewFiles.FullRowSelect = true;
			this.listviewFiles.Location = new System.Drawing.Point(8, 40);
			this.listviewFiles.MultiSelect = false;
			this.listviewFiles.Name = "listviewFiles";
			this.listviewFiles.Size = new System.Drawing.Size(304, 360);
			this.listviewFiles.TabIndex = 3;
			this.listviewFiles.UseCompatibleStateImageBehavior = false;
			this.listviewFiles.View = System.Windows.Forms.View.Details;
			this.listviewFiles.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.ListviewFilesColumnClick);
			this.listviewFiles.SelectedIndexChanged += new System.EventHandler(this.ListviewFilesSelectedIndexChanged);
			// 
			// columnheaderFile
			// 
			this.columnheaderFile.Text = "File";
			// 
			// columnheaderErrors
			// 
			this.columnheaderErrors.Text = "Errors";
			// 
			// buttonAddFiles
			// 
			this.buttonAddFiles.Location = new System.Drawing.Point(8, 8);
			this.buttonAddFiles.Name = "buttonAddFiles";
			this.buttonAddFiles.Size = new System.Drawing.Size(96, 24);
			this.buttonAddFiles.TabIndex = 0;
			this.buttonAddFiles.Text = "Add files...";
			this.buttonAddFiles.UseVisualStyleBackColor = true;
			this.buttonAddFiles.Click += new System.EventHandler(this.ButtonAddFilesClick);
			// 
			// buttonAddDirectory
			// 
			this.buttonAddDirectory.Location = new System.Drawing.Point(112, 8);
			this.buttonAddDirectory.Name = "buttonAddDirectory";
			this.buttonAddDirectory.Size = new System.Drawing.Size(96, 24);
			this.buttonAddDirectory.TabIndex = 1;
			this.buttonAddDirectory.Text = "Add directory...";
			this.buttonAddDirectory.UseVisualStyleBackColor = true;
			this.buttonAddDirectory.Click += new System.EventHandler(this.ButtonAddDirectoryClick);
			// 
			// buttonClearList
			// 
			this.buttonClearList.Location = new System.Drawing.Point(216, 8);
			this.buttonClearList.Name = "buttonClearList";
			this.buttonClearList.Size = new System.Drawing.Size(96, 24);
			this.buttonClearList.TabIndex = 2;
			this.buttonClearList.Text = "Clear list";
			this.buttonClearList.UseVisualStyleBackColor = true;
			this.buttonClearList.Click += new System.EventHandler(this.ButtonClearListClick);
			// 
			// textboxErrors
			// 
			this.textboxErrors.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
									| System.Windows.Forms.AnchorStyles.Left) 
									| System.Windows.Forms.AnchorStyles.Right)));
			this.textboxErrors.BackColor = System.Drawing.SystemColors.Window;
			this.textboxErrors.Location = new System.Drawing.Point(320, 8);
			this.textboxErrors.Multiline = true;
			this.textboxErrors.Name = "textboxErrors";
			this.textboxErrors.ReadOnly = true;
			this.textboxErrors.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.textboxErrors.Size = new System.Drawing.Size(448, 424);
			this.textboxErrors.TabIndex = 6;
			// 
			// buttonValidate
			// 
			this.buttonValidate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.buttonValidate.Location = new System.Drawing.Point(8, 408);
			this.buttonValidate.Name = "buttonValidate";
			this.buttonValidate.Size = new System.Drawing.Size(96, 24);
			this.buttonValidate.TabIndex = 4;
			this.buttonValidate.Text = "Validate again";
			this.buttonValidate.UseVisualStyleBackColor = true;
			this.buttonValidate.Click += new System.EventHandler(this.ButtonValidateClick);
			// 
			// buttonRemoveValidFiles
			// 
			this.buttonRemoveValidFiles.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.buttonRemoveValidFiles.Location = new System.Drawing.Point(112, 408);
			this.buttonRemoveValidFiles.Name = "buttonRemoveValidFiles";
			this.buttonRemoveValidFiles.Size = new System.Drawing.Size(200, 24);
			this.buttonRemoveValidFiles.TabIndex = 5;
			this.buttonRemoveValidFiles.Text = "Remove all valid files";
			this.buttonRemoveValidFiles.UseVisualStyleBackColor = true;
			this.buttonRemoveValidFiles.Click += new System.EventHandler(this.ButtonRemoveValidFilesClick);
			// 
			// MainForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(776, 440);
			this.Controls.Add(this.buttonRemoveValidFiles);
			this.Controls.Add(this.buttonValidate);
			this.Controls.Add(this.textboxErrors);
			this.Controls.Add(this.buttonClearList);
			this.Controls.Add(this.buttonAddDirectory);
			this.Controls.Add(this.buttonAddFiles);
			this.Controls.Add(this.listviewFiles);
			this.Name = "MainForm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "OpenBveObjectValidator";
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		private System.Windows.Forms.Button buttonRemoveValidFiles;
		private System.Windows.Forms.Button buttonValidate;
		private System.Windows.Forms.TextBox textboxErrors;
		private System.Windows.Forms.Button buttonClearList;
		private System.Windows.Forms.Button buttonAddDirectory;
		private System.Windows.Forms.Button buttonAddFiles;
		private System.Windows.Forms.ColumnHeader columnheaderErrors;
		private System.Windows.Forms.ColumnHeader columnheaderFile;
		private System.Windows.Forms.ListView listviewFiles;
	}
}
