using System;
using CommandRunner;

namespace Engine.Player.Commands
{
    public class SetEnginePathCommand : AbstractCommand
    {
        private static void SetEnginePath(StartupInfo info, string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("No engine path specified");
            }
            else
            {
                Console.WriteLine("Overriding Engine Path: " + args[0]);
                EnginePlayer.EngineVersion = "path:" + args[0];
            }
        }

        public SetEnginePathCommand() : base(SetEnginePath, new[] { "--engine-path", "-eP" }, "--engine-path <Path/To/File.game>\nSpecify a manual path to a .engine file", true)
        {

        }
    }
}