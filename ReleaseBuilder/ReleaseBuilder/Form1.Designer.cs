namespace ReleaseBuilder
{
    partial class Form1
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
            this.btnOpenProject = new System.Windows.Forms.Button();
            this.tbProject = new System.Windows.Forms.TextBox();
            this.lblProject = new System.Windows.Forms.Label();
            this.lblAssetFolder = new System.Windows.Forms.Label();
            this.tbAssetFolder = new System.Windows.Forms.TextBox();
            this.btnOpenAssetFolder = new System.Windows.Forms.Button();
            this.lblOutputFolder = new System.Windows.Forms.Label();
            this.tbOutputFolder = new System.Windows.Forms.TextBox();
            this.btnOpenOutputFolder = new System.Windows.Forms.Button();
            this.ofdProject = new System.Windows.Forms.OpenFileDialog();
            this.fbdAssetFolder = new System.Windows.Forms.FolderBrowserDialog();
            this.fbdOutputFolder = new System.Windows.Forms.FolderBrowserDialog();
            this.button1 = new System.Windows.Forms.Button();
            this.rtbBuildOutput = new System.Windows.Forms.RichTextBox();
            this.tbUnpackagedFiles = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.tbPackagedFiles = new System.Windows.Forms.TextBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.button2 = new System.Windows.Forms.Button();
            this.cbOnlyEmbed = new System.Windows.Forms.CheckBox();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnOpenProject
            // 
            this.btnOpenProject.Location = new System.Drawing.Point(813, 4);
            this.btnOpenProject.Name = "btnOpenProject";
            this.btnOpenProject.Size = new System.Drawing.Size(75, 23);
            this.btnOpenProject.TabIndex = 0;
            this.btnOpenProject.Text = "...";
            this.btnOpenProject.UseVisualStyleBackColor = true;
            this.btnOpenProject.Click += new System.EventHandler(this.btnOpenProject_Click);
            // 
            // tbProject
            // 
            this.tbProject.Location = new System.Drawing.Point(61, 6);
            this.tbProject.Name = "tbProject";
            this.tbProject.Size = new System.Drawing.Size(746, 20);
            this.tbProject.TabIndex = 1;
            this.tbProject.Text = "D:\\Users\\Tim\\Documents\\Minor\\MinorEngine\\Engine.Demo\\Engine.Demo.csproj";
            // 
            // lblProject
            // 
            this.lblProject.AutoSize = true;
            this.lblProject.Location = new System.Drawing.Point(12, 9);
            this.lblProject.Name = "lblProject";
            this.lblProject.Size = new System.Drawing.Size(43, 13);
            this.lblProject.TabIndex = 2;
            this.lblProject.Text = "Project:";
            // 
            // lblAssetFolder
            // 
            this.lblAssetFolder.AutoSize = true;
            this.lblAssetFolder.Location = new System.Drawing.Point(12, 35);
            this.lblAssetFolder.Name = "lblAssetFolder";
            this.lblAssetFolder.Size = new System.Drawing.Size(41, 13);
            this.lblAssetFolder.TabIndex = 5;
            this.lblAssetFolder.Text = "Assets:";
            // 
            // tbAssetFolder
            // 
            this.tbAssetFolder.Location = new System.Drawing.Point(61, 32);
            this.tbAssetFolder.Name = "tbAssetFolder";
            this.tbAssetFolder.Size = new System.Drawing.Size(746, 20);
            this.tbAssetFolder.TabIndex = 4;
            this.tbAssetFolder.Text = "D:\\Users\\Tim\\Documents\\Minor\\MinorEngine\\Engine.Demo\\assets";
            // 
            // btnOpenAssetFolder
            // 
            this.btnOpenAssetFolder.Location = new System.Drawing.Point(813, 30);
            this.btnOpenAssetFolder.Name = "btnOpenAssetFolder";
            this.btnOpenAssetFolder.Size = new System.Drawing.Size(75, 23);
            this.btnOpenAssetFolder.TabIndex = 3;
            this.btnOpenAssetFolder.Text = "...";
            this.btnOpenAssetFolder.UseVisualStyleBackColor = true;
            this.btnOpenAssetFolder.Click += new System.EventHandler(this.btnOpenAssetFolder_Click);
            // 
            // lblOutputFolder
            // 
            this.lblOutputFolder.AutoSize = true;
            this.lblOutputFolder.Location = new System.Drawing.Point(12, 61);
            this.lblOutputFolder.Name = "lblOutputFolder";
            this.lblOutputFolder.Size = new System.Drawing.Size(42, 13);
            this.lblOutputFolder.TabIndex = 8;
            this.lblOutputFolder.Text = "Output:";
            // 
            // tbOutputFolder
            // 
            this.tbOutputFolder.Location = new System.Drawing.Point(61, 58);
            this.tbOutputFolder.Name = "tbOutputFolder";
            this.tbOutputFolder.Size = new System.Drawing.Size(746, 20);
            this.tbOutputFolder.TabIndex = 7;
            this.tbOutputFolder.Text = "D:\\Users\\Tim\\Desktop\\Test";
            // 
            // btnOpenOutputFolder
            // 
            this.btnOpenOutputFolder.Location = new System.Drawing.Point(813, 56);
            this.btnOpenOutputFolder.Name = "btnOpenOutputFolder";
            this.btnOpenOutputFolder.Size = new System.Drawing.Size(75, 23);
            this.btnOpenOutputFolder.TabIndex = 6;
            this.btnOpenOutputFolder.Text = "...";
            this.btnOpenOutputFolder.UseVisualStyleBackColor = true;
            this.btnOpenOutputFolder.Click += new System.EventHandler(this.btnOpenOutputFolder_Click);
            // 
            // ofdProject
            // 
            this.ofdProject.Filter = "C# Project File|*.csproj";
            this.ofdProject.Title = "Open C# Minor Engine Project";
            // 
            // fbdAssetFolder
            // 
            this.fbdAssetFolder.Description = "Open Asset Folder";
            this.fbdAssetFolder.ShowNewFolderButton = false;
            // 
            // fbdOutputFolder
            // 
            this.fbdOutputFolder.Description = "Select Output Folder";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(12, 151);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(876, 22);
            this.button1.TabIndex = 9;
            this.button1.Text = "Build and Deploy";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // rtbBuildOutput
            // 
            this.rtbBuildOutput.BackColor = System.Drawing.Color.Black;
            this.rtbBuildOutput.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rtbBuildOutput.ForeColor = System.Drawing.Color.White;
            this.rtbBuildOutput.Location = new System.Drawing.Point(0, 0);
            this.rtbBuildOutput.Name = "rtbBuildOutput";
            this.rtbBuildOutput.Size = new System.Drawing.Size(900, 331);
            this.rtbBuildOutput.TabIndex = 10;
            this.rtbBuildOutput.Text = "";
            // 
            // tbUnpackagedFiles
            // 
            this.tbUnpackagedFiles.Location = new System.Drawing.Point(111, 85);
            this.tbUnpackagedFiles.Name = "tbUnpackagedFiles";
            this.tbUnpackagedFiles.Size = new System.Drawing.Size(328, 20);
            this.tbUnpackagedFiles.TabIndex = 11;
            this.tbUnpackagedFiles.Text = "*.png+*.jpg+*.obj+*.wav+*.vs+*.fs+*.xml+*.ttf";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 88);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(71, 13);
            this.label1.TabIndex = 12;
            this.label1.Text = "Packed Files:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(471, 88);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(84, 13);
            this.label2.TabIndex = 13;
            this.label2.Text = "Unpacked Files:";
            // 
            // tbPackagedFiles
            // 
            this.tbPackagedFiles.Location = new System.Drawing.Point(560, 85);
            this.tbPackagedFiles.Name = "tbPackagedFiles";
            this.tbPackagedFiles.Size = new System.Drawing.Size(328, 20);
            this.tbPackagedFiles.TabIndex = 14;
            this.tbPackagedFiles.Text = "*.cl+*.fl";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.rtbBuildOutput);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 207);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(900, 331);
            this.panel1.TabIndex = 15;
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(12, 179);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(876, 22);
            this.button2.TabIndex = 16;
            this.button2.Text = "Build and Deploy (NO DELETE/BACKUP)";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // cbOnlyEmbed
            // 
            this.cbOnlyEmbed.AutoSize = true;
            this.cbOnlyEmbed.Location = new System.Drawing.Point(15, 128);
            this.cbOnlyEmbed.Name = "cbOnlyEmbed";
            this.cbOnlyEmbed.Size = new System.Drawing.Size(117, 17);
            this.cbOnlyEmbed.TabIndex = 17;
            this.cbOnlyEmbed.Text = "Only Embed Assets";
            this.cbOnlyEmbed.UseVisualStyleBackColor = true;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(900, 538);
            this.Controls.Add(this.cbOnlyEmbed);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.tbPackagedFiles);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.tbUnpackagedFiles);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.lblOutputFolder);
            this.Controls.Add(this.tbOutputFolder);
            this.Controls.Add(this.btnOpenOutputFolder);
            this.Controls.Add(this.lblAssetFolder);
            this.Controls.Add(this.tbAssetFolder);
            this.Controls.Add(this.btnOpenAssetFolder);
            this.Controls.Add(this.lblProject);
            this.Controls.Add(this.tbProject);
            this.Controls.Add(this.btnOpenProject);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "Form1";
            this.Text = "Release Builder";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnOpenProject;
        private System.Windows.Forms.TextBox tbProject;
        private System.Windows.Forms.Label lblProject;
        private System.Windows.Forms.Label lblAssetFolder;
        private System.Windows.Forms.TextBox tbAssetFolder;
        private System.Windows.Forms.Button btnOpenAssetFolder;
        private System.Windows.Forms.Label lblOutputFolder;
        private System.Windows.Forms.TextBox tbOutputFolder;
        private System.Windows.Forms.Button btnOpenOutputFolder;
        private System.Windows.Forms.OpenFileDialog ofdProject;
        private System.Windows.Forms.FolderBrowserDialog fbdAssetFolder;
        private System.Windows.Forms.FolderBrowserDialog fbdOutputFolder;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.RichTextBox rtbBuildOutput;
        private System.Windows.Forms.TextBox tbUnpackagedFiles;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox tbPackagedFiles;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.CheckBox cbOnlyEmbed;
    }
}

