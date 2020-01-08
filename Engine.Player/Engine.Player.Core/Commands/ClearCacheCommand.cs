using System;
using System.IO;
using CommandRunner;

namespace Engine.Player.Core.Commands
{
    public class ClearCacheCommand : AbstractCommand
    {

        private static void ClearCache(StartupInfo info, string[] args)
        {
            Console.WriteLine("Deleting Engine Cache...");
            if (Directory.Exists(EnginePlayer.EngineDir))
            {
                string[] files = Directory.GetFiles(EnginePlayer.EngineDir, "*", SearchOption.AllDirectories);
                for (int i = 0; i < files.Length; i++)
                {
                    File.Delete(files[i]);
                }
            }
        }
        public ClearCacheCommand() : base(ClearCache, new[] { "--clear-cache", "-cC" }, "--clear-cache\nClears all engines in the cache", false)
        {

        }
    }
}