using System;
using System.IO;
using CommandRunner;
using Engine.BuildTools.PackageCreator;
using Engine.BuildTools.PackageCreator.Versions;

namespace Engine.Player.Core.Commands
{
    public class ListInfoCommand : AbstractCommand
    {



        private static void ListInfo(StartupInfo info, string[] args)
        {
            if (args.Length == 0 || !File.Exists(args[0]) || !args[0].EndsWith(".game") && !args[0].EndsWith(".engine"))
            {
                Console.WriteLine("Could not find file");
                return;
            }

            IPackageManifest pm = Creator.ReadManifest(args[0]);
            Console.WriteLine(pm);
        }
        public ListInfoCommand() : base(ListInfo, new[] { "--list-info", "-l" }, "--list-info <<Path/To/File>\nLists Information about the .engine or .game file.", false)
        {

        }
    }
}