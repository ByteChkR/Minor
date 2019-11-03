using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ReleaseBuilder
{
    public partial class FilePathDialog : Form
    {
        public string Path;
        public FilePathDialog()
        {
            InitializeComponent();
        }

        private void FilePathDialog_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Path = textBox1.Text;
            Close();
        }
    }
}
