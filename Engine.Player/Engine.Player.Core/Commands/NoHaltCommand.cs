using CommandRunner;

namespace Engine.Player.Core.Commands
{
    public class NoHaltCommand : AbstractCommand
    {

        private static void NoHalt(StartupInfo info, string[] args)
        {
            EnginePlayer.ReadLine = false;
        }


        public NoHaltCommand() : base(NoHalt, new[] { "--no-halt", "-nH" }, "Does not wait for user input before exiting", false)
        {

        }
    }
}