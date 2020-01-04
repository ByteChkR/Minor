using System;
using System.IO;
using CommandRunner;

namespace Engine.Player.Commands
{
    public class RemoveEngineCommand : AbstractCommand
    {

        private static void RemoveEngine(StartupInfo info, string[] args)
        {
            if (EnginePlayer.IsEngineVersionAvailable(args[0]))
            {
                Console.WriteLine("Deleting Version " + args[0]);
                File.Delete(EnginePlayer.EngineDir + "/" + args[0] + ".engine");
            }
            else
            {
                Console.WriteLine("Engine Version not available. Skipping Deletion.");
            }
        }

        public RemoveEngineCommand() : base(RemoveEngine, new[] { "--remove-engine", "-r" }, "--remove-engine <Version>\nRemoves an engine Version from the engine cache", true)
        {

        }
    }
}