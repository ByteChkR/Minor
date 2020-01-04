using System;
using System.Diagnostics;
using System.IO;
using CommandRunner;
using Engine.BuildTools.PackageCreator;

namespace Engine.BuildTools.Builder.Commands
{
    public class CreatePackageCommand :AbstractCommand
    {
        private static void CreateGamePackage(StartupInfo info, string[] args)
        {
            //1 Directory of unpacked game build
            //2 The Project Name(Must have the same name as the dll that is used to start)
            //3 The OutputFile
            //4 True/False flag that enables copying asset files from the project dir if no filelist has been given.
            //5 Optional File List

            try
            {
                Console.WriteLine(Path.GetFullPath(args[0]));
                Console.WriteLine(Path.GetFullPath(args[2]));
                string version = "";
                if (info.GetCommandEntries("--packer-override-engine-version") == 0)
                {
                    FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(Path.GetFullPath(args[0]) + "/Engine.dll");
                    version = fvi.FileVersion;
                }
                else
                {
                    version = info.GetValues("--packer-override-engine-version")[0];
                }

                string fileList;
                if (args.Length > 5)
                {
                    fileList = Path.GetFullPath(args[5]);
                    Console.WriteLine(Path.GetFullPath(args[5]));
                }
                else
                {
                    fileList = "";
                }

                bool standalone = version == "standalone";
                string packagerVersion = Creator.DefaultVersion;
                string startArg = args[1] + ".dll";
                if (info.GetCommandEntries("--packager-version") != 0)
                {
                    packagerVersion = info.GetValues("--packager-version")[0];
                    if (packagerVersion == "v2")
                    {
                        if (info.GetCommandEntries("--set-start-args") == 0)
                        {
                            Console.WriteLine("Warning. no startup arguments specifed!");
                            startArg = "dotnet " + args[1] + ".dll";
                        }
                        else
                        {
                            string[] a = info.GetValues("--set-start-args").ToArray();
                            startArg = a[0];
                            for (int i = 1; i < a.Length; i++)
                            {
                                startArg += " " + a[i];
                            }
                        }
                    }
                }

                string[] files =Builder. ParseFileList(fileList, Path.GetFullPath(args[0]), args[1], bool.Parse(args[3]),
                    bool.Parse(args[4]), standalone);
                Creator.CreateGamePackage(args[1], startArg, Path.GetFullPath(args[2]),
                    Path.GetFullPath(args[0]), files, version, packagerVersion);
            }
            catch (Exception e)
            {

                throw new ApplicationException("Input Error", e);
            }
        }
        public CreatePackageCommand() : base(CreateGamePackage, new[] { "--create-package" , "-cp" }, "--create-package <BuildFolderOfGame> <GameName> <OutputFile> <CopyAssetsOnError> <CopyPacksOnError> <optional:FileList>\nCreates a Package from a build output of the --build command\n--packer-override-engine-version <Version> can be used to override the required engine version\n--packager-version <packagerVersion> overrides the packager version that is used.\n--set-start-args <args> can be used to specify the startup command manually.", false)
        {

        }
    }
}