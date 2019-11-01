using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using Engine.AssetPackaging;

namespace Engine.ReleaseBuilder
{
    public partial class Form1 : Form
    {
        private static Form1 Instance;

        public Form1()
        {
            Instance = this;
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            DoubleBuffered = true;
            WriteInfo();
        }

        private void btnOpenProject_Click(object sender, EventArgs e)
        {
            if (ofdProject.ShowDialog() == DialogResult.OK)
            {
                tbProject.Text = ofdProject.FileName;
            }
        }

        private void btnOpenAssetFolder_Click(object sender, EventArgs e)
        {
            if (fbdAssetFolder.ShowDialog() == DialogResult.OK)
            {
                tbAssetFolder.Text = fbdAssetFolder.SelectedPath;
            }
        }

        private void btnOpenOutputFolder_Click(object sender, EventArgs e)
        {
            if (fbdOutputFolder.ShowDialog() == DialogResult.OK)
            {
                tbOutputFolder.Text = fbdOutputFolder.SelectedPath;
            }
        }


        private void button1_Click(object sender, EventArgs e)
        {
            rtbBuildOutput.Text = "";
            if (!cbOnlyEmbed.Checked)
            {
                PackAssets(
                    Path.GetDirectoryName(tbProject.Text) + "\\" + Path.GetFileNameWithoutExtension(tbProject.Text),
                    (int) nudPackSize.Value, tbPackagedFiles.Text, tbUnpackagedFiles.Text, tbAssetFolder.Text);
            }

            BuildProject("resources/Build.bat", tbAssetFolder.Text, tbPackagedFiles.Text, tbUnpackagedFiles.Text,
                tbProject.Text, tbOutputFolder.Text, cbOnlyEmbed.Checked);
        }


        private void button2_Click(object sender, EventArgs e)
        {
            rtbBuildOutput.Text = "";
            if (!cbOnlyEmbed.Checked)
            {
                PackAssets(
                    Path.GetDirectoryName(tbProject.Text) + "\\" + Path.GetFileNameWithoutExtension(tbProject.Text),
                    (int) nudPackSize.Value, tbPackagedFiles.Text, tbUnpackagedFiles.Text, tbAssetFolder.Text);
            }

            BuildProject("resources/Build_NoDelete.bat", tbAssetFolder.Text, tbPackagedFiles.Text,
                tbUnpackagedFiles.Text, tbProject.Text, tbOutputFolder.Text, cbOnlyEmbed.Checked);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (fbdOutputFolder.ShowDialog() == DialogResult.OK)
            {
                string output = fbdOutputFolder.SelectedPath;
                if (Directory.Exists(output + "/packs"))
                {
                    Directory.Delete(output + "/packs", true);
                }

                PackAssets(output, (int) nudPackSize.Value, tbPackagedFiles.Text, tbUnpackagedFiles.Text,
                    tbAssetFolder.Text);
            }
        }

        private static T GetAssemblyAttribute<T>(Assembly asm) where T : Attribute
        {
            object[] attributes =
                asm.GetCustomAttributes(typeof(T), true);

            if (attributes.Length == 0)
            {
                return null;
            }

            return (T) attributes[0];
        }

        private static void WriteInfo()
        {
            Assembly asm = Assembly.GetExecutingAssembly();

            AssemblyCopyrightAttribute attrib = GetAssemblyAttribute<AssemblyCopyrightAttribute>(asm);

            string info = asm.GetName().Name + "(" +
                          asm.GetName().Version + ")\n" +
                          attrib.Copyright;

            Instance.WriteLine(info);
        }

        private static void PackAssets(string outputFolder, int packSize, string packedFileExts,
            string unpackedFileExts, string assetFolder)
        {
            AssetPacker.MAXSIZE_KILOBYTES = packSize;

            Instance.WriteLine("Parsing File info...");

            AssetPackageInfo info = new AssetPackageInfo();
            List<string> unpackExts = packedFileExts.Split('+').ToList();
            for (int i = 0; i < unpackExts.Count; i++)
            {
                info.FileInfos.Add(unpackExts[i], new AssetFileInfo() {packageType = AssetPackageType.Unpack});
            }

            List<string> packExts = unpackedFileExts.Split('+').ToList();
            for (int i = 0; i < packExts.Count; i++)
            {
                info.FileInfos.Add(packExts[i], new AssetFileInfo() {packageType = AssetPackageType.Memory});
            }

            Instance.WriteLine("Creating Asset Pack(" + assetFolder + ")...");
            AssetResult ret = AssetPacker.PackAssets(assetFolder, info);
            Instance.WriteLine("Packaging " + ret.indexList.Count + " Assets in " + ret.packs.Count + " Packs.");

            Instance.WriteLine("Saving Asset Pack to " + outputFolder);
            ret.Save(outputFolder);

            Instance.WriteLine("Packaging Assets Finished.");
        }

        private static void BuildProject(string buildFile, string assetFolder, string packedFilesExt,
            string unpackedFilesExt, string projectFile, string outputFolder, bool onlyEmbed)
        {
            Instance.WriteLine("Starting Build Process.");

            string incdir = onlyEmbed
                ? assetFolder + "\\+" + unpackedFilesExt
                : //Normal Asset Folder
                Path.GetDirectoryName(projectFile) + "\\" + Path.GetFileNameWithoutExtension(projectFile) +
                "\\packs\\+*.xml+*.pack";
            string publishDir = Path.GetDirectoryName(projectFile) + "\\bin\\Release\\netcoreapp2.1\\publish";
            string projectDir = Path.GetDirectoryName(projectFile);
            string plainProjectName = Path.GetFileNameWithoutExtension(projectFile);
            string command =
                $"{projectFile} {incdir} {publishDir} {outputFolder} {projectDir} {plainProjectName}";


            CheckForIllegalCrossThreadCalls = false;

            ProcessStartInfo psi = new ProcessStartInfo(buildFile, command)
            {
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true,
                UseShellExecute = false
            };


            Process p = new Process {StartInfo = psi};

            p.Start();
            ConsoleRedirector redir =
                ConsoleRedirector.CreateRedirector(p.StandardOutput, p.StandardError, p, Instance.WriteLine);
            redir.StartThreads();

            while (!p.HasExited)
            {
                lock (Instance.rtbBuildOutput)
                {
                    Application.DoEvents();
                }
            }


            redir.StopThreads();

            Instance.WriteLine(redir.GetRemainingLogs());

            CheckForIllegalCrossThreadCalls = true;
        }

        private void WriteLine(string line)
        {
            lock (rtbBuildOutput)
            {
                if (line != null)
                {
                    rtbBuildOutput.AppendText(line + "\n");
                }
            }
        }
    }
}