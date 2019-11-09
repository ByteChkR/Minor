namespace Engine.BuildTools.Builder.GUI
{
    partial class frmMain
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmMain));
            this.gbBuildSettings = new System.Windows.Forms.GroupBox();
            this.pbError = new System.Windows.Forms.PictureBox();
            this.pbidle = new System.Windows.Forms.PictureBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.cbCreateGamePackage = new System.Windows.Forms.CheckBox();
            this.btnCreateEnginePackage = new System.Windows.Forms.Button();
            this.cbExperimentalPackaging = new System.Windows.Forms.CheckBox();
            this.cbCreateEnginePackage = new System.Windows.Forms.CheckBox();
            this.btnSelectGamePackageFileList = new System.Windows.Forms.Button();
            this.btnSelectEngineProject = new System.Windows.Forms.Button();
            this.tbFileList = new System.Windows.Forms.TextBox();
            this.tbEngineProject = new System.Windows.Forms.TextBox();
            this.lblGameFileList = new System.Windows.Forms.Label();
            this.lblEngineProject = new System.Windows.Forms.Label();
            this.btnRun = new System.Windows.Forms.Button();
            this.btnNew = new System.Windows.Forms.Button();
            this.gbUnpackFileExtensions = new System.Windows.Forms.GroupBox();
            this.rtbUnpackedExts = new System.Windows.Forms.RichTextBox();
            this.gbPackFileExtensions = new System.Windows.Forms.GroupBox();
            this.rtbPackedExts = new System.Windows.Forms.RichTextBox();
            this.cbAskOutputFolderOnBuild = new System.Windows.Forms.CheckBox();
            this.btnSelectOutputFolder = new System.Windows.Forms.Button();
            this.tbOutputFolder = new System.Windows.Forms.TextBox();
            this.lblOutputFolder = new System.Windows.Forms.Label();
            this.btnSelectAssetFolder = new System.Windows.Forms.Button();
            this.tbAssetFolder = new System.Windows.Forms.TextBox();
            this.lblAssetFolder = new System.Windows.Forms.Label();
            this.btnSelectProject = new System.Windows.Forms.Button();
            this.tbProject = new System.Windows.Forms.TextBox();
            this.lblProject = new System.Windows.Forms.Label();
            this.lblBuildFlags = new System.Windows.Forms.Label();
            this.cbBuildFlags = new System.Windows.Forms.ComboBox();
            this.lblPackSize = new System.Windows.Forms.Label();
            this.nudPackageSize = new System.Windows.Forms.NumericUpDown();
            this.btnLoad = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.pbBusy = new System.Windows.Forms.PictureBox();
            this.ofdBuildSettings = new System.Windows.Forms.OpenFileDialog();
            this.sfdBuildSettings = new System.Windows.Forms.SaveFileDialog();
            this.gbOutput = new System.Windows.Forms.GroupBox();
            this.rtbOutput = new System.Windows.Forms.RichTextBox();
            this.ofdAny = new System.Windows.Forms.OpenFileDialog();
            this.ofdProjectFile = new System.Windows.Forms.OpenFileDialog();
            this.fbdAssetFolder = new System.Windows.Forms.FolderBrowserDialog();
            this.fbdOutputFolder = new System.Windows.Forms.FolderBrowserDialog();
            this.gbBuildSettings.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbError)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbidle)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.gbUnpackFileExtensions.SuspendLayout();
            this.gbPackFileExtensions.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudPackageSize)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbBusy)).BeginInit();
            this.gbOutput.SuspendLayout();
            this.SuspendLayout();
            // 
            // gbBuildSettings
            // 
            this.gbBuildSettings.Controls.Add(this.pbidle);
            this.gbBuildSettings.Controls.Add(this.groupBox1);
            this.gbBuildSettings.Controls.Add(this.btnRun);
            this.gbBuildSettings.Controls.Add(this.btnNew);
            this.gbBuildSettings.Controls.Add(this.gbUnpackFileExtensions);
            this.gbBuildSettings.Controls.Add(this.gbPackFileExtensions);
            this.gbBuildSettings.Controls.Add(this.cbAskOutputFolderOnBuild);
            this.gbBuildSettings.Controls.Add(this.btnSelectOutputFolder);
            this.gbBuildSettings.Controls.Add(this.tbOutputFolder);
            this.gbBuildSettings.Controls.Add(this.lblOutputFolder);
            this.gbBuildSettings.Controls.Add(this.btnSelectAssetFolder);
            this.gbBuildSettings.Controls.Add(this.tbAssetFolder);
            this.gbBuildSettings.Controls.Add(this.lblAssetFolder);
            this.gbBuildSettings.Controls.Add(this.btnSelectProject);
            this.gbBuildSettings.Controls.Add(this.tbProject);
            this.gbBuildSettings.Controls.Add(this.lblProject);
            this.gbBuildSettings.Controls.Add(this.lblBuildFlags);
            this.gbBuildSettings.Controls.Add(this.cbBuildFlags);
            this.gbBuildSettings.Controls.Add(this.lblPackSize);
            this.gbBuildSettings.Controls.Add(this.nudPackageSize);
            this.gbBuildSettings.Controls.Add(this.btnLoad);
            this.gbBuildSettings.Controls.Add(this.btnSave);
            this.gbBuildSettings.Controls.Add(this.pbBusy);
            this.gbBuildSettings.Controls.Add(this.pbError);
            this.gbBuildSettings.Dock = System.Windows.Forms.DockStyle.Top;
            this.gbBuildSettings.Location = new System.Drawing.Point(0, 0);
            this.gbBuildSettings.Name = "gbBuildSettings";
            this.gbBuildSettings.Size = new System.Drawing.Size(1073, 372);
            this.gbBuildSettings.TabIndex = 0;
            this.gbBuildSettings.TabStop = false;
            this.gbBuildSettings.Text = "Build Settings:";
            // 
            // pbError
            // 
            this.pbError.Image = global::Engine.BuildTools.Builder.GUI.Properties.Resources.procbusy;
            this.pbError.Location = new System.Drawing.Point(1001, 48);
            this.pbError.Name = "pbError";
            this.pbError.Size = new System.Drawing.Size(60, 60);
            this.pbError.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pbError.TabIndex = 36;
            this.pbError.TabStop = false;
            // 
            // pbidle
            // 
            this.pbidle.Image = global::Engine.BuildTools.Builder.GUI.Properties.Resources.procidle;
            this.pbidle.Location = new System.Drawing.Point(1001, 48);
            this.pbidle.Name = "pbidle";
            this.pbidle.Size = new System.Drawing.Size(60, 60);
            this.pbidle.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pbidle.TabIndex = 33;
            this.pbidle.TabStop = false;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.cbCreateGamePackage);
            this.groupBox1.Controls.Add(this.btnCreateEnginePackage);
            this.groupBox1.Controls.Add(this.cbExperimentalPackaging);
            this.groupBox1.Controls.Add(this.cbCreateEnginePackage);
            this.groupBox1.Controls.Add(this.btnSelectGamePackageFileList);
            this.groupBox1.Controls.Add(this.btnSelectEngineProject);
            this.groupBox1.Controls.Add(this.tbFileList);
            this.groupBox1.Controls.Add(this.tbEngineProject);
            this.groupBox1.Controls.Add(this.lblGameFileList);
            this.groupBox1.Controls.Add(this.lblEngineProject);
            this.groupBox1.Location = new System.Drawing.Point(6, 274);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(1061, 92);
            this.groupBox1.TabIndex = 32;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Build Packaging";
            // 
            // cbCreateGamePackage
            // 
            this.cbCreateGamePackage.AutoSize = true;
            this.cbCreateGamePackage.Enabled = false;
            this.cbCreateGamePackage.Location = new System.Drawing.Point(6, 19);
            this.cbCreateGamePackage.Name = "cbCreateGamePackage";
            this.cbCreateGamePackage.Size = new System.Drawing.Size(135, 17);
            this.cbCreateGamePackage.TabIndex = 3;
            this.cbCreateGamePackage.Text = "Create .game Package";
            this.cbCreateGamePackage.UseVisualStyleBackColor = true;
            this.cbCreateGamePackage.CheckedChanged += new System.EventHandler(this.cbCreateGamePackage_CheckedChanged);
            // 
            // btnCreateEnginePackage
            // 
            this.btnCreateEnginePackage.Location = new System.Drawing.Point(908, 63);
            this.btnCreateEnginePackage.Name = "btnCreateEnginePackage";
            this.btnCreateEnginePackage.Size = new System.Drawing.Size(147, 23);
            this.btnCreateEnginePackage.TabIndex = 30;
            this.btnCreateEnginePackage.Text = "Create Engine Package";
            this.btnCreateEnginePackage.UseVisualStyleBackColor = true;
            this.btnCreateEnginePackage.Click += new System.EventHandler(this.btnCreateEnginePackage_Click);
            // 
            // cbExperimentalPackaging
            // 
            this.cbExperimentalPackaging.AutoSize = true;
            this.cbExperimentalPackaging.Enabled = false;
            this.cbExperimentalPackaging.Location = new System.Drawing.Point(158, 43);
            this.cbExperimentalPackaging.Name = "cbExperimentalPackaging";
            this.cbExperimentalPackaging.Size = new System.Drawing.Size(162, 17);
            this.cbExperimentalPackaging.TabIndex = 31;
            this.cbExperimentalPackaging.Text = "Use Experimental Packaging";
            this.cbExperimentalPackaging.UseVisualStyleBackColor = true;
            // 
            // cbCreateEnginePackage
            // 
            this.cbCreateEnginePackage.AutoSize = true;
            this.cbCreateEnginePackage.Enabled = false;
            this.cbCreateEnginePackage.Location = new System.Drawing.Point(6, 42);
            this.cbCreateEnginePackage.Name = "cbCreateEnginePackage";
            this.cbCreateEnginePackage.Size = new System.Drawing.Size(141, 17);
            this.cbCreateEnginePackage.TabIndex = 4;
            this.cbCreateEnginePackage.Text = "Create .engine Package";
            this.cbCreateEnginePackage.UseVisualStyleBackColor = true;
            this.cbCreateEnginePackage.CheckedChanged += new System.EventHandler(this.cbCreateEnginePackage_CheckedChanged);
            // 
            // btnSelectGamePackageFileList
            // 
            this.btnSelectGamePackageFileList.Enabled = false;
            this.btnSelectGamePackageFileList.Location = new System.Drawing.Point(1016, 15);
            this.btnSelectGamePackageFileList.Name = "btnSelectGamePackageFileList";
            this.btnSelectGamePackageFileList.Size = new System.Drawing.Size(39, 23);
            this.btnSelectGamePackageFileList.TabIndex = 25;
            this.btnSelectGamePackageFileList.Text = "...";
            this.btnSelectGamePackageFileList.UseVisualStyleBackColor = true;
            this.btnSelectGamePackageFileList.Click += new System.EventHandler(this.btnSelectGamePackageFileList_Click);
            // 
            // btnSelectEngineProject
            // 
            this.btnSelectEngineProject.Enabled = false;
            this.btnSelectEngineProject.Location = new System.Drawing.Point(863, 63);
            this.btnSelectEngineProject.Name = "btnSelectEngineProject";
            this.btnSelectEngineProject.Size = new System.Drawing.Size(39, 23);
            this.btnSelectEngineProject.TabIndex = 24;
            this.btnSelectEngineProject.Text = "...";
            this.btnSelectEngineProject.UseVisualStyleBackColor = true;
            this.btnSelectEngineProject.Click += new System.EventHandler(this.btnSelectEngineProject_Click);
            // 
            // tbFileList
            // 
            this.tbFileList.Enabled = false;
            this.tbFileList.Location = new System.Drawing.Point(221, 17);
            this.tbFileList.Name = "tbFileList";
            this.tbFileList.Size = new System.Drawing.Size(789, 20);
            this.tbFileList.TabIndex = 26;
            this.tbFileList.Text = "autodetect";
            this.tbFileList.TextChanged += new System.EventHandler(this.tbFileList_TextChanged);
            // 
            // tbEngineProject
            // 
            this.tbEngineProject.Enabled = false;
            this.tbEngineProject.Location = new System.Drawing.Point(131, 65);
            this.tbEngineProject.Name = "tbEngineProject";
            this.tbEngineProject.Size = new System.Drawing.Size(726, 20);
            this.tbEngineProject.TabIndex = 23;
            this.tbEngineProject.TextChanged += new System.EventHandler(this.tbEngineProject_TextChanged);
            // 
            // lblGameFileList
            // 
            this.lblGameFileList.AutoSize = true;
            this.lblGameFileList.Location = new System.Drawing.Point(173, 20);
            this.lblGameFileList.Name = "lblGameFileList";
            this.lblGameFileList.Size = new System.Drawing.Size(42, 13);
            this.lblGameFileList.TabIndex = 27;
            this.lblGameFileList.Text = "FileList:";
            // 
            // lblEngineProject
            // 
            this.lblEngineProject.AutoSize = true;
            this.lblEngineProject.Location = new System.Drawing.Point(12, 68);
            this.lblEngineProject.Name = "lblEngineProject";
            this.lblEngineProject.Size = new System.Drawing.Size(116, 13);
            this.lblEngineProject.TabIndex = 22;
            this.lblEngineProject.Text = "Engine Project(.csproj):";
            // 
            // btnRun
            // 
            this.btnRun.Enabled = false;
            this.btnRun.Location = new System.Drawing.Point(988, 19);
            this.btnRun.Name = "btnRun";
            this.btnRun.Size = new System.Drawing.Size(75, 23);
            this.btnRun.TabIndex = 29;
            this.btnRun.Text = "Run";
            this.btnRun.UseVisualStyleBackColor = true;
            this.btnRun.Click += new System.EventHandler(this.btnRun_Click);
            // 
            // btnNew
            // 
            this.btnNew.Location = new System.Drawing.Point(168, 19);
            this.btnNew.Name = "btnNew";
            this.btnNew.Size = new System.Drawing.Size(75, 23);
            this.btnNew.TabIndex = 28;
            this.btnNew.Text = "New";
            this.btnNew.UseVisualStyleBackColor = true;
            this.btnNew.Click += new System.EventHandler(this.btnNew_Click);
            // 
            // gbUnpackFileExtensions
            // 
            this.gbUnpackFileExtensions.Controls.Add(this.rtbUnpackedExts);
            this.gbUnpackFileExtensions.Location = new System.Drawing.Point(866, 114);
            this.gbUnpackFileExtensions.Name = "gbUnpackFileExtensions";
            this.gbUnpackFileExtensions.Size = new System.Drawing.Size(200, 127);
            this.gbUnpackFileExtensions.TabIndex = 21;
            this.gbUnpackFileExtensions.TabStop = false;
            this.gbUnpackFileExtensions.Text = "File extensions that will be unpacked:";
            // 
            // rtbUnpackedExts
            // 
            this.rtbUnpackedExts.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rtbUnpackedExts.Enabled = false;
            this.rtbUnpackedExts.Location = new System.Drawing.Point(3, 16);
            this.rtbUnpackedExts.Name = "rtbUnpackedExts";
            this.rtbUnpackedExts.Size = new System.Drawing.Size(194, 108);
            this.rtbUnpackedExts.TabIndex = 19;
            this.rtbUnpackedExts.Text = "";
            // 
            // gbPackFileExtensions
            // 
            this.gbPackFileExtensions.Controls.Add(this.rtbPackedExts);
            this.gbPackFileExtensions.Location = new System.Drawing.Point(675, 114);
            this.gbPackFileExtensions.Name = "gbPackFileExtensions";
            this.gbPackFileExtensions.Size = new System.Drawing.Size(161, 127);
            this.gbPackFileExtensions.TabIndex = 20;
            this.gbPackFileExtensions.TabStop = false;
            this.gbPackFileExtensions.Text = "File Extensions(One per line):";
            // 
            // rtbPackedExts
            // 
            this.rtbPackedExts.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rtbPackedExts.Enabled = false;
            this.rtbPackedExts.Location = new System.Drawing.Point(3, 16);
            this.rtbPackedExts.Name = "rtbPackedExts";
            this.rtbPackedExts.Size = new System.Drawing.Size(155, 108);
            this.rtbPackedExts.TabIndex = 19;
            this.rtbPackedExts.Text = "*";
            // 
            // cbAskOutputFolderOnBuild
            // 
            this.cbAskOutputFolderOnBuild.AutoSize = true;
            this.cbAskOutputFolderOnBuild.Enabled = false;
            this.cbAskOutputFolderOnBuild.Location = new System.Drawing.Point(549, 222);
            this.cbAskOutputFolderOnBuild.Name = "cbAskOutputFolderOnBuild";
            this.cbAskOutputFolderOnBuild.Size = new System.Drawing.Size(87, 17);
            this.cbAskOutputFolderOnBuild.TabIndex = 18;
            this.cbAskOutputFolderOnBuild.Text = "Ask On Build";
            this.cbAskOutputFolderOnBuild.UseVisualStyleBackColor = true;
            this.cbAskOutputFolderOnBuild.CheckedChanged += new System.EventHandler(this.cbAskOutputFolderOnBuild_CheckedChanged);
            // 
            // btnSelectOutputFolder
            // 
            this.btnSelectOutputFolder.Enabled = false;
            this.btnSelectOutputFolder.Location = new System.Drawing.Point(504, 218);
            this.btnSelectOutputFolder.Name = "btnSelectOutputFolder";
            this.btnSelectOutputFolder.Size = new System.Drawing.Size(39, 23);
            this.btnSelectOutputFolder.TabIndex = 17;
            this.btnSelectOutputFolder.Text = "...";
            this.btnSelectOutputFolder.UseVisualStyleBackColor = true;
            this.btnSelectOutputFolder.Click += new System.EventHandler(this.btnSelectOutputFolder_Click);
            // 
            // tbOutputFolder
            // 
            this.tbOutputFolder.Enabled = false;
            this.tbOutputFolder.Location = new System.Drawing.Point(131, 220);
            this.tbOutputFolder.Name = "tbOutputFolder";
            this.tbOutputFolder.Size = new System.Drawing.Size(359, 20);
            this.tbOutputFolder.TabIndex = 16;
            this.tbOutputFolder.TextChanged += new System.EventHandler(this.tbOutputFolder_TextChanged);
            // 
            // lblOutputFolder
            // 
            this.lblOutputFolder.AutoSize = true;
            this.lblOutputFolder.Location = new System.Drawing.Point(12, 223);
            this.lblOutputFolder.Name = "lblOutputFolder";
            this.lblOutputFolder.Size = new System.Drawing.Size(74, 13);
            this.lblOutputFolder.TabIndex = 15;
            this.lblOutputFolder.Text = "Output Folder:";
            // 
            // btnSelectAssetFolder
            // 
            this.btnSelectAssetFolder.Enabled = false;
            this.btnSelectAssetFolder.Location = new System.Drawing.Point(549, 192);
            this.btnSelectAssetFolder.Name = "btnSelectAssetFolder";
            this.btnSelectAssetFolder.Size = new System.Drawing.Size(39, 23);
            this.btnSelectAssetFolder.TabIndex = 14;
            this.btnSelectAssetFolder.Text = "...";
            this.btnSelectAssetFolder.UseVisualStyleBackColor = true;
            this.btnSelectAssetFolder.Click += new System.EventHandler(this.btnSelectAssetFolder_Click);
            // 
            // tbAssetFolder
            // 
            this.tbAssetFolder.Enabled = false;
            this.tbAssetFolder.Location = new System.Drawing.Point(131, 194);
            this.tbAssetFolder.Name = "tbAssetFolder";
            this.tbAssetFolder.Size = new System.Drawing.Size(412, 20);
            this.tbAssetFolder.TabIndex = 13;
            this.tbAssetFolder.TextChanged += new System.EventHandler(this.tbAssetFolder_TextChanged);
            // 
            // lblAssetFolder
            // 
            this.lblAssetFolder.AutoSize = true;
            this.lblAssetFolder.Location = new System.Drawing.Point(12, 197);
            this.lblAssetFolder.Name = "lblAssetFolder";
            this.lblAssetFolder.Size = new System.Drawing.Size(68, 13);
            this.lblAssetFolder.TabIndex = 12;
            this.lblAssetFolder.Text = "Asset Folder:";
            // 
            // btnSelectProject
            // 
            this.btnSelectProject.Enabled = false;
            this.btnSelectProject.Location = new System.Drawing.Point(549, 166);
            this.btnSelectProject.Name = "btnSelectProject";
            this.btnSelectProject.Size = new System.Drawing.Size(39, 23);
            this.btnSelectProject.TabIndex = 11;
            this.btnSelectProject.Text = "...";
            this.btnSelectProject.UseVisualStyleBackColor = true;
            this.btnSelectProject.Click += new System.EventHandler(this.btnSelectProject_Click);
            // 
            // tbProject
            // 
            this.tbProject.Enabled = false;
            this.tbProject.Location = new System.Drawing.Point(131, 168);
            this.tbProject.Name = "tbProject";
            this.tbProject.Size = new System.Drawing.Size(412, 20);
            this.tbProject.TabIndex = 10;
            this.tbProject.TextChanged += new System.EventHandler(this.tbProject_TextChanged);
            // 
            // lblProject
            // 
            this.lblProject.AutoSize = true;
            this.lblProject.Location = new System.Drawing.Point(12, 171);
            this.lblProject.Name = "lblProject";
            this.lblProject.Size = new System.Drawing.Size(80, 13);
            this.lblProject.TabIndex = 9;
            this.lblProject.Text = "Project(.csproj):";
            // 
            // lblBuildFlags
            // 
            this.lblBuildFlags.AutoSize = true;
            this.lblBuildFlags.Location = new System.Drawing.Point(9, 51);
            this.lblBuildFlags.Name = "lblBuildFlags";
            this.lblBuildFlags.Size = new System.Drawing.Size(61, 13);
            this.lblBuildFlags.TabIndex = 8;
            this.lblBuildFlags.Text = "Build Flags:";
            // 
            // cbBuildFlags
            // 
            this.cbBuildFlags.Enabled = false;
            this.cbBuildFlags.FormattingEnabled = true;
            this.cbBuildFlags.Items.AddRange(new object[] {
            "PackOnly",
            "PackEmbed",
            "Embed"});
            this.cbBuildFlags.Location = new System.Drawing.Point(76, 48);
            this.cbBuildFlags.Name = "cbBuildFlags";
            this.cbBuildFlags.Size = new System.Drawing.Size(121, 21);
            this.cbBuildFlags.TabIndex = 7;
            this.cbBuildFlags.Text = "PackOnly";
            this.cbBuildFlags.SelectedIndexChanged += new System.EventHandler(this.cbBuildFlags_SelectedIndexChanged);
            // 
            // lblPackSize
            // 
            this.lblPackSize.AutoSize = true;
            this.lblPackSize.Location = new System.Drawing.Point(12, 138);
            this.lblPackSize.Name = "lblPackSize";
            this.lblPackSize.Size = new System.Drawing.Size(125, 13);
            this.lblPackSize.TabIndex = 6;
            this.lblPackSize.Text = "Asset Package Size(KB):";
            // 
            // nudPackageSize
            // 
            this.nudPackageSize.Enabled = false;
            this.nudPackageSize.Location = new System.Drawing.Point(143, 136);
            this.nudPackageSize.Maximum = new decimal(new int[] {
            4194304,
            0,
            0,
            0});
            this.nudPackageSize.Name = "nudPackageSize";
            this.nudPackageSize.Size = new System.Drawing.Size(66, 20);
            this.nudPackageSize.TabIndex = 5;
            this.nudPackageSize.Value = new decimal(new int[] {
            1024,
            0,
            0,
            0});
            // 
            // btnLoad
            // 
            this.btnLoad.Location = new System.Drawing.Point(6, 19);
            this.btnLoad.Name = "btnLoad";
            this.btnLoad.Size = new System.Drawing.Size(75, 23);
            this.btnLoad.TabIndex = 1;
            this.btnLoad.Text = "Load";
            this.btnLoad.UseVisualStyleBackColor = true;
            this.btnLoad.Click += new System.EventHandler(this.btnLoad_Click);
            // 
            // btnSave
            // 
            this.btnSave.Enabled = false;
            this.btnSave.Location = new System.Drawing.Point(87, 19);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 2;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // pbBusy
            // 
            this.pbBusy.Image = global::Engine.BuildTools.Builder.GUI.Properties.Resources.loading_pac;
            this.pbBusy.Location = new System.Drawing.Point(1001, 48);
            this.pbBusy.Name = "pbBusy";
            this.pbBusy.Size = new System.Drawing.Size(60, 60);
            this.pbBusy.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pbBusy.TabIndex = 35;
            this.pbBusy.TabStop = false;
            // 
            // ofdBuildSettings
            // 
            this.ofdBuildSettings.DefaultExt = "xml";
            this.ofdBuildSettings.FileName = "BuildSettings";
            this.ofdBuildSettings.Filter = "BuildSettings|*.xml";
            this.ofdBuildSettings.Title = "Select Build Settings to Load";
            // 
            // sfdBuildSettings
            // 
            this.sfdBuildSettings.DefaultExt = "xml";
            this.sfdBuildSettings.FileName = "BuildSettings";
            this.sfdBuildSettings.Filter = "BuildSettings|*.xml";
            this.sfdBuildSettings.Title = "Save Build Settings...";
            // 
            // gbOutput
            // 
            this.gbOutput.Controls.Add(this.rtbOutput);
            this.gbOutput.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gbOutput.Location = new System.Drawing.Point(0, 372);
            this.gbOutput.Name = "gbOutput";
            this.gbOutput.Size = new System.Drawing.Size(1073, 230);
            this.gbOutput.TabIndex = 3;
            this.gbOutput.TabStop = false;
            this.gbOutput.Text = "Output:";
            // 
            // rtbOutput
            // 
            this.rtbOutput.BackColor = System.Drawing.SystemColors.WindowText;
            this.rtbOutput.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rtbOutput.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rtbOutput.ForeColor = System.Drawing.SystemColors.Window;
            this.rtbOutput.Location = new System.Drawing.Point(3, 16);
            this.rtbOutput.Name = "rtbOutput";
            this.rtbOutput.Size = new System.Drawing.Size(1067, 211);
            this.rtbOutput.TabIndex = 0;
            this.rtbOutput.Text = "Bla";
            // 
            // ofdAny
            // 
            this.ofdAny.Title = "Open File..";
            // 
            // ofdProjectFile
            // 
            this.ofdProjectFile.DefaultExt = "csproj";
            this.ofdProjectFile.Filter = "C# Project|*.csproj";
            this.ofdProjectFile.Title = "Open .csproj";
            // 
            // fbdAssetFolder
            // 
            this.fbdAssetFolder.Description = "Select the Folder Containing the Game Assets";
            // 
            // fbdOutputFolder
            // 
            this.fbdOutputFolder.Description = "Select the Folder Containing the Game Assets";
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1073, 602);
            this.Controls.Add(this.gbOutput);
            this.Controls.Add(this.gbBuildSettings);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximumSize = new System.Drawing.Size(1089, 3000);
            this.MinimumSize = new System.Drawing.Size(1089, 318);
            this.Name = "frmMain";
            this.Text = "Main Window";
            this.Load += new System.EventHandler(this.frmMain_Load);
            this.gbBuildSettings.ResumeLayout(false);
            this.gbBuildSettings.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbError)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbidle)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.gbUnpackFileExtensions.ResumeLayout(false);
            this.gbPackFileExtensions.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.nudPackageSize)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbBusy)).EndInit();
            this.gbOutput.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox gbBuildSettings;
        private System.Windows.Forms.Button btnLoad;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.OpenFileDialog ofdBuildSettings;
        private System.Windows.Forms.SaveFileDialog sfdBuildSettings;
        private System.Windows.Forms.GroupBox gbOutput;
        private System.Windows.Forms.RichTextBox rtbOutput;
        private System.Windows.Forms.Label lblPackSize;
        private System.Windows.Forms.NumericUpDown nudPackageSize;
        private System.Windows.Forms.CheckBox cbCreateEnginePackage;
        private System.Windows.Forms.CheckBox cbCreateGamePackage;
        private System.Windows.Forms.Label lblBuildFlags;
        private System.Windows.Forms.ComboBox cbBuildFlags;
        private System.Windows.Forms.CheckBox cbAskOutputFolderOnBuild;
        private System.Windows.Forms.Button btnSelectOutputFolder;
        private System.Windows.Forms.TextBox tbOutputFolder;
        private System.Windows.Forms.Label lblOutputFolder;
        private System.Windows.Forms.Button btnSelectAssetFolder;
        private System.Windows.Forms.TextBox tbAssetFolder;
        private System.Windows.Forms.Label lblAssetFolder;
        private System.Windows.Forms.Button btnSelectProject;
        private System.Windows.Forms.TextBox tbProject;
        private System.Windows.Forms.Label lblProject;
        private System.Windows.Forms.Button btnSelectEngineProject;
        private System.Windows.Forms.TextBox tbEngineProject;
        private System.Windows.Forms.Label lblEngineProject;
        private System.Windows.Forms.GroupBox gbUnpackFileExtensions;
        private System.Windows.Forms.RichTextBox rtbUnpackedExts;
        private System.Windows.Forms.GroupBox gbPackFileExtensions;
        private System.Windows.Forms.RichTextBox rtbPackedExts;
        private System.Windows.Forms.Label lblGameFileList;
        private System.Windows.Forms.TextBox tbFileList;
        private System.Windows.Forms.Button btnSelectGamePackageFileList;
        private System.Windows.Forms.OpenFileDialog ofdAny;
        private System.Windows.Forms.Button btnNew;
        private System.Windows.Forms.OpenFileDialog ofdProjectFile;
        private System.Windows.Forms.FolderBrowserDialog fbdAssetFolder;
        private System.Windows.Forms.FolderBrowserDialog fbdOutputFolder;
        private System.Windows.Forms.Button btnRun;
        private System.Windows.Forms.Button btnCreateEnginePackage;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.CheckBox cbExperimentalPackaging;
        private System.Windows.Forms.PictureBox pbidle;
        private System.Windows.Forms.PictureBox pbBusy;
        private System.Windows.Forms.PictureBox pbError;
    }
}

