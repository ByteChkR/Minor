using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;
using Engine.BuildTools.Common;

namespace Engine.BuildTools.Builder.GUI
{
    public partial class frmMain : Form
    {
        private BuildSettings bs;
        private XmlSerializer xs;
        private string SaveLocation;
        private bool Initialized;
        public frmMain()
        {
            InitializeComponent();
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            bs = new BuildSettings();
            SaveLocation = "";
            xs = new XmlSerializer(typeof(BuildSettings));
        }

        private void Initialize()
        {
            if (Initialized) return;
            Initialized = true;
            SetGlobalState(true);
            InvalidateFormOnContent();
        }
        private void SetGlobalState(bool state)
        {

            tbAssetFolder.Enabled = state;
            tbEngineProject.Enabled = state;
            tbFileList.Enabled = state;
            tbOutputFolder.Enabled = state;
            tbProject.Enabled = state;

            btnSelectEngineProject.Enabled = state;
            btnSelectAssetFolder.Enabled = state;
            btnSelectGamePackageFileList.Enabled = state;
            btnSelectOutputFolder.Enabled = state;
            btnSelectProject.Enabled = state;
            btnRun.Enabled = state;
            btnSave.Enabled = state;

            rtbPackedExts.Enabled = state;
            rtbUnpackedExts.Enabled = state;

            cbAskOutputFolderOnBuild.Enabled = state;
            cbBuildFlags.Enabled = state;
            cbCreateEnginePackage.Enabled = state;
            cbCreateGamePackage.Enabled = state;


        }

        private void SaveFile(string fileName)
        {
            if (File.Exists(fileName)) File.Delete(fileName);

            bs.MemoryFiles = PackFileString(rtbPackedExts.Text);
            bs.UnpackFiles = PackFileString(rtbUnpackedExts.Text);
            FileStream fs = new FileStream(fileName, FileMode.Create);
            xs.Serialize(fs, bs);
            fs.Close();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (sfdBuildSettings.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    SaveFile(sfdBuildSettings.FileName);
                    SaveLocation = sfdBuildSettings.FileName;
                    InvalidateFormOnContent();
                    WriteOutput("Build Settings Saved.");
                }
                catch (Exception exception)
                {
                    WriteOutput("Exception Saving Build Settings: " + sfdBuildSettings.FileName);
                    WriteOutput(exception.ToString());
                    throw;
                }
            }
        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            if (ofdBuildSettings.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    FileStream fs = new FileStream(ofdBuildSettings.FileName, FileMode.Open);
                    bs = (BuildSettings)xs.Deserialize(fs);
                    fs.Close();
                    SaveLocation = ofdBuildSettings.FileName;
                    Initialize();
                    InvalidateForm();
                    WriteOutput("Build Settings Loaded.");
                    Initialized = true;
                }
                catch (Exception exception)
                {
                    WriteOutput("Exception Loading Build Settings: " + ofdBuildSettings.FileName);
                    WriteOutput(exception.ToString());
                    throw;
                }
            }
        }
        private void btnNew_Click(object sender, EventArgs e)
        {
            if (sfdBuildSettings.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    if (File.Exists(sfdBuildSettings.FileName)) File.Delete(sfdBuildSettings.FileName);
                    FileStream fs = new FileStream(sfdBuildSettings.FileName, FileMode.Create);
                    xs.Serialize(fs, new BuildSettings());
                    fs.Close();
                    SaveLocation = sfdBuildSettings.FileName;
                    Initialize();
                    InvalidateForm();
                    WriteOutput("Build Settings Created.");
                }
                catch (Exception exception)
                {
                    WriteOutput("Exception Creating Build Settings: " + sfdBuildSettings.FileName);
                    WriteOutput(exception.ToString());
                    throw;
                }
            }
        }

        private void btnSelectGamePackageFileList_Click(object sender, EventArgs e)
        {
            if (ofdAny.ShowDialog() == DialogResult.OK)
            {
                tbFileList.Text = GetRelativePath(SaveLocation, ofdAny.FileName);
            }
        }

        private void InvalidateFormOnContent()
        {
            if (cbBuildFlags.SelectedIndex == (int)BuildType.PackOnly)
            {
                nudPackageSize.Enabled = true;
            }
            else if (cbBuildFlags.SelectedIndex == (int)BuildType.Embed)
            {
                nudPackageSize.Enabled = false;
            }
            else if (cbBuildFlags.SelectedIndex == (int)BuildType.PackEmbed)
            {
                nudPackageSize.Enabled = true;
            }
            tbEngineProject.Enabled = cbCreateEnginePackage.Checked;
            btnSelectEngineProject.Enabled = cbCreateEnginePackage.Checked;

            tbFileList.Enabled = cbCreateGamePackage.Checked;
            btnSelectGamePackageFileList.Enabled = cbCreateGamePackage.Checked;

            tbOutputFolder.Enabled = !cbAskOutputFolderOnBuild.Checked;
            btnSelectOutputFolder.Enabled = !cbAskOutputFolderOnBuild.Checked;
        }

        private void InvalidateForm()
        {
            tbAssetFolder.Text = bs.AssetFolder;
            tbProject.Text = bs.Project;
            tbOutputFolder.Text = bs.OutputFolder;
            tbEngineProject.Text = bs.EngineProject;
            cbBuildFlags.SelectedIndex = (int)bs.BuildFlags;
            cbCreateEnginePackage.Checked = bs.CreateEnginePackage;
            cbCreateGamePackage.Checked = bs.CreateGamePackage;
            rtbPackedExts.Text = UnpackFileString(bs.MemoryFiles);
            rtbUnpackedExts.Text = UnpackFileString(bs.UnpackFiles);
            tbFileList.Text = bs.GamePackageFileList;
        }

        private void InvalidateSettings()
        {
            bs.AssetFolder = tbAssetFolder.Text;
            bs.Project = tbProject.Text;
            bs.OutputFolder = tbOutputFolder.Text;
            bs.EngineProject = tbEngineProject.Text;
            bs.BuildFlags = (BuildType)cbBuildFlags.SelectedIndex;
            bs.CreateEnginePackage = cbCreateEnginePackage.Checked;
            bs.CreateGamePackage = cbCreateGamePackage.Checked;
            bs.MemoryFiles = PackFileString(rtbPackedExts.Text);
            bs.UnpackFiles = PackFileString(rtbUnpackedExts.Text);
            bs.GamePackageFileList = tbFileList.Text;
        }

        private void MakeSaveReady()
        {
            string fullPath = Path.GetFullPath(SaveLocation);
            bs.EngineProject = GetRelativePath(fullPath, bs.EngineProject);
            bs.Project = GetRelativePath(fullPath, bs.Project);
            bs.OutputFolder = GetRelativePath(fullPath, bs.OutputFolder);
            bs.AssetFolder = GetRelativePath(fullPath, bs.AssetFolder);
            bs.GamePackageFileList = GetRelativePath(fullPath, bs.GamePackageFileList);
        }

        private static bool FileExists(string fileInRoot, string file)
        {
            string p = Path.GetDirectoryName(Path.GetFullPath(fileInRoot));
            return File.Exists(Path.Combine(p, file));
        }

        private static string FullPath(string fileInRoot, string file)
        {
            return Path.Combine(Path.GetDirectoryName(fileInRoot), file);
        }

        private static bool DirectoryExists(string fileInRoot, string file)
        {
            string p = Path.GetDirectoryName(Path.GetFullPath(fileInRoot));
            return Directory.Exists(Path.Combine(p, file));
        }

        private static string GetRelativePath(string fileInRoot, string file)
        {
            string dir = Path.GetDirectoryName(Path.GetFullPath(fileInRoot));
            string fi = Path.GetFullPath(file);
            Uri d = new Uri(dir + '/');
            Uri f = new Uri(fi);
            return d.MakeRelativeUri(f).ToString();
        }

        private string UnpackFileString(string fileExts)
        {
            string[] r = fileExts.Split(new[] { '+' }, StringSplitOptions.RemoveEmptyEntries);
            StringBuilder sb = new StringBuilder("");
            for (int i = 0; i < r.Length; i++)
            {
                sb.AppendLine(r[i]);
            }

            return sb.ToString();
        }

        private string PackFileString(string fileExts)
        {
            string[] r = fileExts.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
            StringBuilder sb = new StringBuilder("");

            if (r.Length > 0)
            {
                sb.Append(r[0]);
                for (int i = 1; i < r.Length; i++)
                {
                    sb.Append('+' + r[i]);
                }
            }

            return sb.ToString();
        }

        private void WriteOutput(string line)
        {
            lock (rtbOutput)
            {
                rtbOutput.Text += line + '\n';
            }
        }

        private void cbBuildFlags_SelectedIndexChanged(object sender, EventArgs e)
        {
            InvalidateFormOnContent();
        }

        private void cbCreateEnginePackage_CheckedChanged(object sender, EventArgs e)
        {
            bs.CreateEnginePackage = cbCreateEnginePackage.Checked;
            InvalidateFormOnContent();
        }

        private void cbCreateGamePackage_CheckedChanged(object sender, EventArgs e)
        {
            bs.CreateGamePackage = cbCreateGamePackage.Checked;
            InvalidateFormOnContent();
        }

        private void cbAskOutputFolderOnBuild_CheckedChanged(object sender, EventArgs e)
        {
            InvalidateFormOnContent();
        }

        private void btnSelectProject_Click(object sender, EventArgs e)
        {
            if (ofdProjectFile.ShowDialog() == DialogResult.OK)
            {
                tbProject.Text = GetRelativePath(SaveLocation, ofdProjectFile.FileName);
            }
        }

        private void btnSelectAssetFolder_Click(object sender, EventArgs e)
        {
            if (fbdAssetFolder.ShowDialog() == DialogResult.OK)
            {
                tbAssetFolder.Text = GetRelativePath(SaveLocation, fbdAssetFolder.SelectedPath);
            }
        }

        private void btnSelectEngineProject_Click(object sender, EventArgs e)
        {
            if (ofdProjectFile.ShowDialog() == DialogResult.OK)
            {
                tbEngineProject.Text = GetRelativePath(SaveLocation, ofdProjectFile.FileName);
            }
        }

        private void btnSelectOutputFolder_Click(object sender, EventArgs e)
        {
            AskBuildOutput();
        }

        private void AskBuildOutput()
        {
            if (fbdOutputFolder.ShowDialog() == DialogResult.OK)
            {
                tbOutputFolder.Text = GetRelativePath(SaveLocation, fbdOutputFolder.SelectedPath);
            }
        }

        private void tbFileList_TextChanged(object sender, EventArgs e)
        {
            bool exists = false;
            if (tbFileList.Text == "autodetect" || tbFileList.Text == "")
            {
                exists = true;
                bs.GamePackageFileList = "";
            }
            else
            {
                bs.GamePackageFileList = GetRelativePath(SaveLocation, tbFileList.Text);

                exists = FileExists(SaveLocation, tbFileList.Text);
            }

            if (exists) tbFileList.BackColor = Color.Green;
            else tbFileList.BackColor = Color.Red;
        }

        private void tbProject_TextChanged(object sender, EventArgs e)
        {
            bool exists = false;

            bs.Project = tbProject.Text;

            exists = FileExists(SaveLocation, tbProject.Text);

            if (exists) tbProject.BackColor = Color.Green;
            else tbProject.BackColor = Color.Red;
        }

        private void tbAssetFolder_TextChanged(object sender, EventArgs e)
        {
            bool exists = false;

            bs.AssetFolder = tbAssetFolder.Text;

            exists = DirectoryExists(SaveLocation, tbAssetFolder.Text);


            if (exists) tbAssetFolder.BackColor = Color.Green;
            else tbAssetFolder.BackColor = Color.Red;
        }

        private void tbOutputFolder_TextChanged(object sender, EventArgs e)
        {
            bool exists = false;

            bs.OutputFolder = tbOutputFolder.Text;

            exists = DirectoryExists(SaveLocation, tbOutputFolder.Text);


            if (exists) tbOutputFolder.BackColor = Color.Green;
            else tbOutputFolder.BackColor = Color.Red;
        }

        private void tbEngineProject_TextChanged(object sender, EventArgs e)
        {
            bool exists = false;

            bs.EngineProject = tbEngineProject.Text;

            exists = FileExists(SaveLocation, tbEngineProject.Text);


            if (exists) tbEngineProject.BackColor = Color.Green;
            else tbEngineProject.BackColor = Color.Red;
        }

        private void CreateEnginePackage(string engineProjectFile, string outputPath)
        {
            WriteOutput("Creating Engine Package...");
            ProcessUtils.RunProcess("cmd.exe", $"/C dotnet Engine.BuildTools.Builder.dll --create-engine-package {engineProjectFile} {outputPath}", Application.DoEvents, WriteOutput);
        }

        private void btnRun_Click(object sender, EventArgs e)
        {
            if (cbAskOutputFolderOnBuild.Checked)
            {
                AskBuildOutput();
            }

            SaveFile(SaveLocation);
            CheckForIllegalCrossThreadCalls = false;
            WriteOutput("Running Build Settings...");
            ProcessUtils.RunProcess("cmd.exe", "/C dotnet Engine.BuildTools.Builder.dll " + SaveLocation, Application.DoEvents, WriteOutput);

            if (cbCreateEnginePackage.Checked)
            {
                CreateEnginePackage($"{FullPath(SaveLocation, tbEngineProject.Text)}", $"{FullPath(SaveLocation, tbOutputFolder.Text + "/" + Path.GetFileNameWithoutExtension(tbProject.Text))}.engine");
            }

            CheckForIllegalCrossThreadCalls = true;
            OpenBuildFolder();
        }

        private void btnCreateEnginePackage_Click(object sender, EventArgs e)
        {
            CheckForIllegalCrossThreadCalls = false;
            CreateEnginePackage(FullPath(SaveLocation, tbEngineProject.Text), $"{FullPath(SaveLocation, tbOutputFolder.Text + "/" + Path.GetFileNameWithoutExtension(tbProject.Text))}.engine");

            CheckForIllegalCrossThreadCalls = true;

            OpenBuildFolder();
        }

        private void OpenBuildFolder()
        {
            if (DirectoryExists(SaveLocation, tbOutputFolder.Text))
                Process.Start("explorer.exe", FullPath(SaveLocation, tbOutputFolder.Text));
        }
    }
}
