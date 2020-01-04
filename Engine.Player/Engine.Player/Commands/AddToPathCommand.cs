using System;
using CommandRunner;

namespace Engine.Player.Commands
{
    public class AddToPathCommand : AbstractCommand
    {
        private static void AddToPath(StartupInfo info, string[] args)
        {
            try
            {
                if (EnginePlayer.IsInPathVariable()) EnginePlayer.UpdatePathVariable();
                else EnginePlayer.AddToPathVariable();
            }
            catch (Exception e)
            {
                Console.WriteLine("Could not access registry. Administrator rights are reequired once.");
                Console.WriteLine(e);
            }
        }

        public AddToPathCommand() : base(AddToPath, new[] { "--add-to-path" }, "Requires Admin Permissions", false)
        {

        }
    }
}