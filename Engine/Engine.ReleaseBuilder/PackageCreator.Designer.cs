namespace ReleaseBuilder
{
    partial class PackageCreator
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.fbdWorkingDir = new System.Windows.Forms.FolderBrowserDialog();
            this.lbFileList = new System.Windows.Forms.ListBox();
            this.btnAddFiles = new System.Windows.Forms.Button();
            this.btnAddFolder = new System.Windows.Forms.Button();
            this.btnRemoveFile = new System.Windows.Forms.Button();
            this.fbdFolder = new System.Windows.Forms.FolderBrowserDialog();
            this.ofdFiles = new System.Windows.Forms.OpenFileDialog();
            this.btnAbort = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // fbdWorkingDir
            // 
            this.fbdWorkingDir.Description = "Select Working Directory";
            // 
            // lbFileList
            // 
            this.lbFileList.FormattingEnabled = true;
            this.lbFileList.Location = new System.Drawing.Point(12, 49);
            this.lbFileList.Name = "lbFileList";
            this.lbFileList.Size = new System.Drawing.Size(397, 446);
            this.lbFileList.Sorted = true;
            this.lbFileList.TabIndex = 0;
            // 
            // btnAddFiles
            // 
            this.btnAddFiles.Location = new System.Drawing.Point(12, 20);
            this.btnAddFiles.Name = "btnAddFiles";
            this.btnAddFiles.Size = new System.Drawing.Size(23, 23);
            this.btnAddFiles.TabIndex = 1;
            this.btnAddFiles.Text = "+";
            this.btnAddFiles.UseVisualStyleBackColor = true;
            this.btnAddFiles.Click += new System.EventHandler(this.btnAddFiles_Click);
            // 
            // btnAddFolder
            // 
            this.btnAddFolder.Location = new System.Drawing.Point(70, 20);
            this.btnAddFolder.Name = "btnAddFolder";
            this.btnAddFolder.Size = new System.Drawing.Size(75, 23);
            this.btnAddFolder.TabIndex = 2;
            this.btnAddFolder.Text = "Add Folder";
            this.btnAddFolder.UseVisualStyleBackColor = true;
            this.btnAddFolder.Click += new System.EventHandler(this.btnAddFolder_Click);
            // 
            // btnRemoveFile
            // 
            this.btnRemoveFile.Location = new System.Drawing.Point(41, 20);
            this.btnRemoveFile.Name = "btnRemoveFile";
            this.btnRemoveFile.Size = new System.Drawing.Size(23, 23);
            this.btnRemoveFile.TabIndex = 3;
            this.btnRemoveFile.Text = "-";
            this.btnRemoveFile.UseVisualStyleBackColor = true;
            // 
            // ofdFiles
            // 
            this.ofdFiles.Multiselect = true;
            this.ofdFiles.Title = "Select Files";
            // 
            // btnAbort
            // 
            this.btnAbort.Location = new System.Drawing.Point(334, 20);
            this.btnAbort.Name = "btnAbort";
            this.btnAbort.Size = new System.Drawing.Size(75, 23);
            this.btnAbort.TabIndex = 4;
            this.btnAbort.Text = "Abort";
            this.btnAbort.UseVisualStyleBackColor = true;
            this.btnAbort.Click += new System.EventHandler(this.btnAbort_Click);
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(253, 20);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 5;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Location = new System.Drawing.Point(151, 24);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(95, 17);
            this.checkBox1.TabIndex = 6;
            this.checkBox1.Text = "Show Relative";
            this.checkBox1.UseVisualStyleBackColor = true;
            this.checkBox1.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // PackageCreator
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(419, 503);
            this.Controls.Add(this.checkBox1);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.btnAbort);
            this.Controls.Add(this.btnRemoveFile);
            this.Controls.Add(this.btnAddFolder);
            this.Controls.Add(this.btnAddFiles);
            this.Controls.Add(this.lbFileList);
            this.Name = "PackageCreator";
            this.Text = "PackageCreator";
            this.Load += new System.EventHandler(this.PackageCreator_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.FolderBrowserDialog fbdWorkingDir;
        private System.Windows.Forms.ListBox lbFileList;
        private System.Windows.Forms.Button btnAddFiles;
        private System.Windows.Forms.Button btnAddFolder;
        private System.Windows.Forms.Button btnRemoveFile;
        private System.Windows.Forms.FolderBrowserDialog fbdFolder;
        private System.Windows.Forms.OpenFileDialog ofdFiles;
        private System.Windows.Forms.Button btnAbort;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.CheckBox checkBox1;
    }
}