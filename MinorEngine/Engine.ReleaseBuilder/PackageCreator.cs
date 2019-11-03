using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ReleaseBuilder
{
    public partial class PackageCreator : Form
    {
        private string workingdir;
        public Dictionary<string, string> fileMap = new Dictionary<string, string>();
        public PackageCreator()
        {
            InitializeComponent();
        }

        private void PackageCreator_Load(object sender, EventArgs e)
        {
            if (fbdWorkingDir.ShowDialog() != DialogResult.OK)
            {
                Close();
                return;
            }

            workingdir = fbdWorkingDir.SelectedPath;
        }

        private void btnAddFolder_Click(object sender, EventArgs e)
        {
            if (fbdFolder.ShowDialog() == DialogResult.OK)
            {
                string[] files = Directory.GetFiles(fbdFolder.SelectedPath, "*", SearchOption.AllDirectories);
                for (int i = 0; i < files.Length; i++)
                {
                    Uri folder =new Uri(fbdFolder.SelectedPath);
                    Uri file = new Uri(files[i]);
                    string relPath = folder.MakeRelativeUri(file).ToString();
                    fileMap.Add(files[i], relPath);
                }
                lbFileList.Items.AddRange(files);
            }
        }

        private void btnAddFiles_Click(object sender, EventArgs e)
        {
            if (ofdFiles.ShowDialog() == DialogResult.OK)
            {
                FilePathDialog fpd = new FilePathDialog();
                if (fpd.ShowDialog() == DialogResult.OK)
                {
                    for (int i = 0; i < ofdFiles.FileNames.Length; i++)
                    {
                        fileMap.Add(ofdFiles.FileNames[i], fpd.Path + Path.GetFileName(ofdFiles.FileNames[i]));
                        lbFileList.Items.Add(ofdFiles.FileNames[i]);
                    }
                }
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            lbFileList.Items.Clear();
            if (checkBox1.Checked)
            {
                lbFileList.Items.AddRange(fileMap.Values.ToArray());
            }
            else
            {
                lbFileList.Items.AddRange(fileMap.Keys.ToArray());
            }
        }

        private void btnAbort_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Abort;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
        }
    }
}
