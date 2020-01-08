using System;
using CommandRunner;

namespace Engine.Player.Core.Commands
{
    public class DownloadEngineCommand : AbstractCommand
    {

        private static void DownloadEngine(StartupInfo info, string[] args)
        {
            if (!EnginePlayer.IsEngineVersionAvailable(args[0]))
            {
                Console.WriteLine("Downloading Version " + args[0]);
                EnginePlayer.DownloadEngineVersion(args[0]);
            }
            else
            {
                Console.WriteLine("Engine Version not available. Skipping Download.");
            }
        }

        public DownloadEngineCommand() : base(DownloadEngine, new[] { "--download-engine", "-d" }, "--download-engine <Version>\n Tries to download a specified engine version", false)
        {

        }
    }
}