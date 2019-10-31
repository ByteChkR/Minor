using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
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
            BuildProject();
        }


        private bool isFile(string file, string[] ext)
        {
            for (int i = 0; i < ext.Length; i++)
            {
                if (file.EndsWith(ext[i])) return true;
            }

            return false;
        }
        private void PackAssets()
        {
            string[] files = Directory.GetFiles(tbAssetFolder.Text, "*", SearchOption.AllDirectories).Where(x => isFile(x, tbPackagedFiles.Text.Split('+'))).ToArray();
            string[] packFiles = new string[files.Length];
            Uri p2 = new Uri(Path.GetDirectoryName(tbProject.Text));
            for (int i = 0; i < files.Length; i++)
            {
                Uri p1 = new Uri(files[i]);
                Uri p3 = p2.MakeRelativeUri(p1);
                packFiles[i] = p3.ToString().Replace(Path.GetFileName(Path.GetDirectoryName(tbProject.Text)) + "/", "");
                WriteLine("Packing File: " + packFiles[i]);
            }

            AssetPacker.PackAssets(files, packFiles, Path.GetDirectoryName(tbProject.Text) + "\\" + Path.GetFileNameWithoutExtension(tbProject.Text));
        }

        private void BuildProject()
        {

            CheckForIllegalCrossThreadCalls = false;
            string command =
                $"{tbProject.Text} {Path.GetDirectoryName(tbProject.Text)+"\\"+ Path.GetFileNameWithoutExtension(tbProject.Text) + "\\packs\\+*.xml+*.pack"} {tbAssetFolder.Text}\\+{tbUnpackagedFiles.Text} { Path.GetDirectoryName(tbProject.Text)}\\bin\\Release\\netcoreapp2.1\\publish {tbOutputFolder.Text} {Path.GetDirectoryName(tbProject.Text)} {Path.GetFileNameWithoutExtension(tbProject.Text)}";
            ProcessStartInfo psi = new ProcessStartInfo("resources/Build.bat", command);
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
