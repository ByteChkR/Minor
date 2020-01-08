using System;
using System.IO;
using CommandRunner;
using Engine.BuildTools.PackageCreator;

namespace Engine.BuildTools.Builder.Commands
{
    public class CreateEnginePackageCommand : AbstractCommand
    {
        private static void CreateEnginePackage(StartupInfo info, string[] args)
        {
            //1 Directory of unpacked game build
            //2 The Project Name(Must have the same name as the dll that is used to start)
            //3 The OutputFile
            //4 True/False flag that enables copying asset files from the project dir if no filelist has been given.
            //5 Optional File List

            try
            {
                Console.WriteLine(Path.GetFullPath(args[0]));
                Console.WriteLine(Path.GetFullPath(args[1]));
                string fileList;
                if (args.Length > 2)
                {
                    fileList = Path.GetFullPath(args[2]);
                    Console.WriteLine(Path.GetFullPath(args[2]));
                }
                else
                {
                    fileList = "";
                }

                string csF = Path.GetFullPath(args[0]);

                string folder = Path.GetDirectoryName(csF) + "/bin/Release/netcoreapp2.1/publish";
                if (Directory.Exists(folder + "/bin"))
                {
                    Console.WriteLine("Deleting publish folder to prevent copying the wrong assemblies.");
                    Directory.Delete(folder + "/bin", true);
                }

                if (Directory.Exists(folder + "/obj"))
                {
                    Console.WriteLine("Deleting publish folder to prevent copying the wrong assemblies.");
                    Directory.Delete(folder + "/obj", true);
                }

                string packagerVersion = Creator.DefaultVersion;
                if (info.GetCommandEntries("--packager-version") != 0)
                {
                    packagerVersion = info.GetValues("--packager-version")[0];
                }

                Builder.BuildProject(csF);
                string[] files = Builder.ParseEngineFileList(fileList, folder);
                Creator.CreateEnginePackage(Path.GetFullPath(args[1]), folder, files, packagerVersion);
            }
            catch (Exception e)
            {

                throw new ApplicationException("Input Error", e);
            }
        }

        public CreateEnginePackageCommand() : base(CreateEnginePackage, new[] { "--create-engine-package", "-cep" }, "--create-engine-package <Engine.csproj file> <OutputFile> <optional:FileList>\nCreates an Engine Package from an Engine.csproj file\n--packager-version <packagerVersion> overrides the packager version that is used.", false)
        {

        }
    }
}