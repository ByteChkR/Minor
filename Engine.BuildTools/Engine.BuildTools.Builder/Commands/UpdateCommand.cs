using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using CommandRunner;

namespace Engine.BuildTools.Builder.Commands
{
    public class UpdateCommand : AbstractCommand
    {
        private static void Update(StartupInfo info, string[] args)
        {
            WebClient wc = new WebClient();
            Console.WriteLine("Updating Build Tools...");
            string destination = Path.GetTempFileName() + ".exe";
            wc.DownloadFile(@"http://213.109.162.193/apps/Installer.exe", destination);
            Process.Start(destination, "--silent --pid " + Process.GetCurrentProcess().Id);
            wc.Dispose();
            Console.WriteLine("Update Downloaded. Update will be applied when application exits.");
        }

        public UpdateCommand() : base(Update, new[] { "--update" }, "--update Updates the Build Tools", false)
        {

        }
    }
}