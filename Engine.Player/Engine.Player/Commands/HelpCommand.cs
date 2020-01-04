using System;
using CommandRunner;

namespace Engine.Player.Commands
{
    public class HelpCommand : AbstractCommand
    {

        public static void Help(StartupInfo info, string[] args)
        {
            Console.WriteLine("Commands:");
            for (int i = 0; i < Runner.CommandCount; i++)
            {
                Console.WriteLine(Runner.GetCommandAt(i));
            }
        }

        public HelpCommand() : base(Help, new[] { "--help", "-h", "-?" }, "Display this help message", false)
        {

        }
    }
}