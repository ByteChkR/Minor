using System;
using CommandRunner;

namespace Engine.Player.Commands
{
    public class SetDefaultProgramCommand : AbstractCommand
    {

        private static void SetDefaultProgram(StartupInfo info, string[] args)
        {
            try
            {
                EnginePlayer.RegisterExtensions();
            }
            catch (Exception e)
            {
                Console.WriteLine("Could not access registry. Administrator rights are reequired once.");
                Console.WriteLine(e);
            }
        }

        public SetDefaultProgramCommand() : base(SetDefaultProgram, new[] { "--set-default-program", "-sD" }, "Requires Admin Permissions", false)
        {

        }
    }
}