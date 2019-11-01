using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using AssetPackaging;

namespace ReleaseBuilder
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            DoubleBuffered = true;
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
            PackAssets();
            BuildProject("resources/Build.bat");
        }


        private void button2_Click(object sender, EventArgs e)
        {
            rtbBuildOutput.Text = "";
            PackAssets();
            BuildProject("resources/Build_NoDelete.bat");
        }

        private T GetAssemblyAttrib<T>(Assembly asm) where T : Attribute
        {
            // Get attributes of this type.
            object[] attributes =
                asm.GetCustomAttributes(typeof(T), true);

            // If we didn't get anything, return null.
            if (attributes.Length == 0)
                return null;

            // Convert the first attribute value into
            // the desired type and return it.
            return (T)attributes[0];
        }

        private void WriteInfo()
        {
            Assembly asm = Assembly.GetExecutingAssembly();

            AssemblyCopyrightAttribute attrib = GetAssemblyAttrib<AssemblyCopyrightAttribute>(asm);

            string info = asm.GetName().Name + "(" +
                          asm.GetName().Version + ")\n" +
                          attrib.Copyright;

            WriteLine(info);
        }

        private void PackAssets()
        {
            if (!cbOnlyEmbed.Checked)
            {
                WriteLine("Parsing File info...");

                AssetPackageInfo info = new AssetPackageInfo();
                List<string> unpackExts = tbPackagedFiles.Text.Split('+').ToList();
                for (int i = 0; i < unpackExts.Count; i++)
                {
                    info.FileInfos.Add(unpackExts[i], new AssetFileInfo() { packageType = AssetPackageType.Unpack });
                }
                List<string> packExts = tbUnpackagedFiles.Text.Split('+').ToList();
                for (int i = 0; i < packExts.Count; i++)
                {
                    info.FileInfos.Add(packExts[i], new AssetFileInfo() { packageType = AssetPackageType.Memory });
                }

                WriteLine("Creating Asset Pack(" + tbAssetFolder.Text + ")...");
                AssetResult ret = AssetPacker.PackAssets(tbAssetFolder.Text, info);
                WriteLine("Packaging " + ret.indexList.Count + " Assets in " + ret.packs.Count + " Packs.");

                WriteLine("Saving Asset Pack to " + Path.GetDirectoryName(tbProject.Text) + "\\" + Path.GetFileNameWithoutExtension(tbProject.Text));
                ret.Save(Path.GetDirectoryName(tbProject.Text) + "\\" + Path.GetFileNameWithoutExtension(tbProject.Text));

                WriteLine("Packaging Assets Finished.");
            }
        }

        private void BuildProject(string buildFile)
        {

            WriteLine("Starting Build Process.");

            string incdir = cbOnlyEmbed.Checked ? 
                tbAssetFolder.Text + "\\+" + tbUnpackagedFiles.Text : //Normal Asset Folder
                Path.GetDirectoryName(tbProject.Text) + "\\" + Path.GetFileNameWithoutExtension(tbProject.Text) + "\\packs\\+*.xml+*.pack";
            string publishDir = Path.GetDirectoryName(tbProject.Text) + "\\bin\\Release\\netcoreapp2.1\\publish";
            string projectDir = Path.GetDirectoryName(tbProject.Text);
            string plainProjectName = Path.GetFileNameWithoutExtension(tbProject.Text);
            string command =
                $"{tbProject.Text} {incdir} {publishDir} {tbOutputFolder.Text} {projectDir} {plainProjectName}";


            CheckForIllegalCrossThreadCalls = false;

            ProcessStartInfo psi = new ProcessStartInfo(buildFile, command);
            psi.RedirectStandardOutput = true;
            psi.RedirectStandardError = true;
            psi.CreateNoWindow = true;
            psi.UseShellExecute = false;


            Process p = new Process();

            p.StartInfo = psi;
            p.Start();
            ConsoleRedirector redir = ConsoleRedirector.CreateRedirector(p.StandardOutput, p.StandardError, p, WriteLine);
            redir.StartThreads();

            while (!p.HasExited)
            {
                lock (rtbBuildOutput)
                {
                    Application.DoEvents();
                }
            }


            redir.StopThreads();

            WriteLine(redir.GetRemainingLogs());

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

        private void Form1_Load(object sender, EventArgs e)
        {
            WriteInfo();
        }

    }
}
