using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using CommandRunner;
using Engine.Player.Commands;

namespace Engine.Player
{
    internal static class Program
    {
        

        private static void Main(string[] args)
        {
            Runner.AddCommand(new SetDefaultProgramCommand());
            Runner.AddCommand(new AddToPathCommand());
            Core.EnginePlayer.RunCommands(args);
        }

    }
}