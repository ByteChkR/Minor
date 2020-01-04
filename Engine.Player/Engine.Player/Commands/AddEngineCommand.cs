using System;
using System.IO;
using CommandRunner;
using Engine.BuildTools.PackageCreator;
using Engine.BuildTools.PackageCreator.Versions;

namespace Engine.Player.Commands
{
    public class AddEngineCommand : AbstractCommand
    {
        private static void AddEngine(string path)
        {
            try
            {
                IPackageManifest pm = Creator.ReadManifest(path);
                if (!EnginePlayer.EngineVersions.Contains(pm.Version))
                {
                    Console.WriteLine("Adding Engine: " + pm);
                    EnginePlayer.EngineVersions.Add(pm.Version);
                    File.Copy(path, EnginePlayer.EngineDir + "/" + pm.Version + ".engine");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Could not add Engine to Player.");
                Console.WriteLine(e);
                throw;
            }
        }
        public static void AddEngine(StartupInfo info, string[] args)
        {
            if (args.Length == 0 || !File.Exists(args[0]) || !args[0].EndsWith(".engine"))
            {
                Console.WriteLine("Could not load Engine Path");
                return;
            }

            AddEngine(args[0]);
        }

        public AddEngineCommand() : base(AddEngine, new[] { "--add-engine", "-a" }, "--add-engine <<Path/To/File.engine>\nAdds an engine file to the engine cache", true)
        {

        }
    }
}