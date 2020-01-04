using CommandRunner;

namespace Engine.Player.Commands
{
    public class NoHaltCommand : AbstractCommand
    {

        private static void NoHalt(StartupInfo info, string[] args)
        {
            EnginePlayer.DontReadLine = true;
        }


        public NoHaltCommand() : base(NoHalt, new[] { "--no-halt", "-nH" }, "Does not wait for user input before exiting", false)
        {

        }
    }
}