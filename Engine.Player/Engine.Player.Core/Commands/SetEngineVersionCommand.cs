using System;
using CommandRunner;

namespace Engine.Player.Core.Commands
{
    public class SetEngineVersionCommand : AbstractCommand
    {


        private static void SetEngineVersion(StartupInfo info, string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("No engine version specified");
            }
            else
            {
                Console.WriteLine("Overriding Engine Version: " + args[0]);
                EnginePlayer.EngineVersion = args[0];
            }
        }

        public SetEngineVersionCommand() : base(SetEngineVersion, new[] { "--engine", "-e" }, "--engine <Version>\nSpecify a manual version", false)
        {

        }
    }
}