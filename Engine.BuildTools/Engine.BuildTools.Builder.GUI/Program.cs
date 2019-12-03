using System;
using System.Windows.Forms;

namespace Engine.BuildTools.Builder.GUI
{
    /// <summary>
    /// CLI Wrapper for the Engine.BuildTools.Builder Library
    /// </summary>
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new frmMain());
        }
    }
}