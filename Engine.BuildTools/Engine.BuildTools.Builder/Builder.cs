using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using System.Xml.Serialization;
using CommandRunner;
using Engine.AssetPackaging;
using Engine.BuildTools.Common;
using Engine.BuildTools.PackageCreator;

namespace Engine.BuildTools.Builder
{

    /// <summary>
    /// Class Containing the Building Logic that is used in the CLI and GUI Wrappers
    /// </summary>
    public static class Builder
    {
        private static bool IsWindows => Type.GetType("Mono.Runtime") == null;
        public static BuildSettings LoadSettings(string path)
        {
            XmlSerializer xs = new XmlSerializer(typeof(BuildSettings));
            FileStream fs = new FileStream(path, FileMode.Open);
            BuildSettings bs = (BuildSettings)xs.Deserialize(fs);
            fs.Close();
            return bs;
        }

        public static void RunCommand(string args)
        {
            RunCommand(args.Split(' '));
        }


        public static void RunCommand(string[] args)
        {

            Console.WriteLine("Windows: " + IsWindows);
            Runner.AddAssembly(Assembly.GetExecutingAssembly());
            //Command def = Command.CreateCommand(BuildWithXML, "--xml <Path/To/File.xml>", "--xml");
            //CommandRunner.SetDefaultCommand(def);

            //CommandRunner.AddCommand(Command.CreateCommand(_Update, "--update Updates the Build Tools", "--update"));
            //CommandRunner.AddCommand(Command.CreateCommand(_CreatePatch, "--create-patch <folder> <destinationFile>", "--create-patch", "-cpatch"));
            //CommandRunner.AddCommand(Command.CreateCommand(_CreatePatchDelta, "--create-patch <oldFile> <newFile> <destinationFile>", "--create-patch-delta", "-cpatchdelta"));
            //CommandRunner.AddCommand(Command.CreateCommand(_HelpCommand, "Display this help message", "--help", "-h"));
            //CommandRunner.AddCommand(Command.CreateCommand(_PatchPackage, "--patch <targetFile> <patchFile>\nApplies the patch to the file.", "--patch", "-p"));
            //CommandRunner.AddCommand(Command.CreateCommand(_PatchPackagePermanent, "--patch-permanent <targetFile> <patchFile>\nApplies the patch to the file permanently.", "--patch-permanent", "-pp"));
            //CommandRunner.AddCommand(Command.CreateCommand(_PackAssets, "--packer <outputFolder> <packSize> <fileExtensions> <unpackFileExtensions> <assetFolder>\nPackage the Asset Files", "--pack-assets", "--packer"));
            //CommandRunner.AddCommand(Command.CreateCommand(_EmbedFiles, "--embed <Path/To/CSProj/File> <Folder/To/Embed>\nEmbeds the files in the specified folder into the .csproj file of the game project.", "--embed", "-e"));

            //CommandRunner.AddCommand(Command.CreateCommand(_Build, "--build <Path/To/CSProj/File> <OutputDirectory>\nBuilds the Specified csproj file and moves all output to the output folder.", "--build", "-b"));
            //CommandRunner.AddCommand(Command.CreateCommand(_UnembedFiles, "--unembed <Path/To/CSProj/File>\nUnembeds that were embedded into the .csproj file of the game project.", "--unembed", "-u"));
            //CommandRunner.AddCommand(Command.CreateCommand(_CreateGamePackage, "--create-package <BuildFolderOfGame> <GameName> <OutputFile> <CopyAssetsOnError> <CopyPacksOnError> <optional:FileList>\nCreates a Package from a build output of the --build command\n--packer-override-engine-version <Version> can be used to override the required engine version\n--packager-version <packagerVersion> overrides the packager version that is used.\n--set-start-args <args> can be used to specify the startup command manually.", "--create-package", "-cp"));
            //CommandRunner.AddCommand(Command.CreateCommand(_CreateEnginePackage, "--create-engine-package <Engine.csproj file> <OutputFile> <optional:FileList>\nCreates an Engine Package from an Engine.csproj file\n--packager-version <packagerVersion> overrides the packager version that is used.", "--create-engine-package", "-cep"));

            //CommandRunner.AddCommand(def);


            Runner.RunCommands(args);

        }
        

        public static void BuildProject(string filepath)
        {
            int ret = BuildCommand(filepath);
            if (ret != 0)
            {
                throw new ApplicationException("Compilation Command Failed.");
            }

            ret = PublishCommand(filepath);
            if (ret != 0)
            {
                throw new ApplicationException("Publish Command Failed.");
            }
        }

        private static int BuildCommand(string filepath)
        {
            string exec = IsWindows ? "cmd.exe" : "dotnet";
            string extra = IsWindows ? "/C dotnet " : "";
            Console.WriteLine("Windows: " + IsWindows);
            Console.WriteLine("Using Shell: " + exec);
            return ProcessUtils.RunProcess(exec, $"{extra}build {filepath} -c Release",
                null);
        }

        private static int PublishCommand(string filepath)
        {
            string exec = IsWindows ? "cmd.exe" : "dotnet";
            string extra = IsWindows ? "/C dotnet " : "";
            Console.WriteLine("Windows: " + IsWindows);
            Console.WriteLine("Using Shell: " + exec);
            return ProcessUtils.RunProcess(exec, $"{extra}publish {filepath} -c Release",
                null);
        }

        


        public static string[] CreateFileList(string path, string searchPatterns, char separator = '+')
        {
            string[] patterns = searchPatterns.Split(new[] { separator }, StringSplitOptions.RemoveEmptyEntries);
            List<string> ret = new List<string>();
            for (int i = 0; i < patterns.Length; i++)
            {
                ret.AddRange(Directory.GetFiles(path, patterns[i], SearchOption.AllDirectories));
            }

            return ret.ToArray();
        }

        public static AssetPackageInfo CreatePackageInfo(string memoryFileExts, string unpackedFileExts,
            char separator = '+')
        {
            AssetPackageInfo info = new AssetPackageInfo();
            List<string> unpackExts = unpackedFileExts.Split(new[] { separator }, StringSplitOptions.RemoveEmptyEntries)
                .ToList();
            for (int i = 0; i < unpackExts.Count; i++)
            {
                info.FileInfos.Add(unpackExts[i], new AssetFileInfo { PackageType = AssetPackageType.Unpack });
            }

            List<string> packExts = memoryFileExts.Split("+".ToCharArray(), StringSplitOptions.RemoveEmptyEntries)
                .ToList();
            for (int i = 0; i < packExts.Count; i++)
            {
                info.FileInfos.Add(packExts[i], new AssetFileInfo { PackageType = AssetPackageType.Memory });
            }

            return info;
        }

        public static void PackAssets(string outputFolder, int packSize, string memoryFileExts,
            string unpackedFileExts, string assetFolder, bool compression)
        {
            AssetPacker.MaxsizeKilobytes = packSize;

            Console.WriteLine("Parsing File info...");

            AssetPackageInfo info = CreatePackageInfo(memoryFileExts, unpackedFileExts);

            Console.WriteLine("Creating Asset Pack(" + assetFolder + ")...");
            AssetResult ret = AssetPacker.PackAssets(assetFolder, outputFolder, info, compression);
            Console.WriteLine("Packaging " + ret.IndexList.Count + " Assets in " + ret.Packs.Count + " Packs.");

            Console.WriteLine("Saving Asset Pack to " + outputFolder);
            ret.Save();

            Console.WriteLine("Packaging Assets Finished.");
        }

        public static string[] ParseFileList(string fileList, string projectFolder, string projectName,
            bool copyAssetsWhenError, bool copyPacksWhenError, bool isStandalone)
        {
            string[] files;
            if (fileList != null && File.Exists(fileList))
            {
                files = File.ReadAllLines(fileList);
                for (int i = 0; i < files.Length; i++)
                {
                    files[i] = Path.GetFullPath(files[i]);
                }
            }
            else
            {
                Console.WriteLine(
                    "Warning. No Game Package File list. Using /asset /pack folder as well as projectname.dll");
                List<string> f = new List<string>();
                string packFolder = projectFolder + "/packs";
                string assetFolder = projectFolder + "/assets";
                if (Directory.Exists(assetFolder) && copyAssetsWhenError)
                {
                    string[] ff = Directory.GetFiles(assetFolder, "*", SearchOption.AllDirectories);
                    for (int i = 0; i < ff.Length; i++)
                    {
                        if (!f.Contains(ff[i]))
                        {
                            f.Add(ff[i]);
                        }
                    }
                }

                if (Directory.Exists(packFolder) && copyPacksWhenError)
                {
                    string[] ff = Directory.GetFiles(packFolder, "*", SearchOption.AllDirectories);
                    for (int i = 0; i < ff.Length; i++)
                    {
                        if (!f.Contains(ff[i]))
                        {
                            f.Add(ff[i]);
                        }
                    }
                }

                string helper = Path.GetFullPath(projectFolder + "/" + projectName + ".dll");
                if (File.Exists(helper) && !f.Contains(helper))
                {
                    f.Add(helper);
                }

                helper = Path.GetFullPath(projectFolder + "/" + projectName + ".runtimeconfig.json");
                if (File.Exists(helper) && !f.Contains(helper))
                {
                    f.Add(helper);
                }

                helper = Path.GetFullPath(projectFolder + "/" + projectName + ".deps.json");
                if (File.Exists(helper) && !f.Contains(helper))
                {
                    f.Add(helper);
                }

                if (isStandalone)
                {
                    string[] ff = ParseEngineFileList(fileList, projectFolder);
                    for (int i = 0; i < ff.Length; i++)
                    {
                        if (!f.Contains(ff[i]))
                        {
                            f.Add(ff[i]);
                        }
                    }
                }

                files = f.ToArray();
            }

            return files;
        }

        public static string[] ParseEngineFileList(string fileList, string projectFolder)
        {
            string[] files;
            if (fileList != null && File.Exists(fileList))
            {
                files = File.ReadAllLines(fileList);
                for (int i = 0; i < files.Length; i++)
                {
                    files[i] = Path.GetFullPath(files[i]);
                }
            }
            else
            {
                Console.WriteLine(
                    "Warning. No Game Package File list. Using /asset /pack folder as well as projectname.dll");
                List<string> f = new List<string>();

                f.AddRange(Directory.GetFiles(projectFolder + "/runtimes", "*", SearchOption.AllDirectories));
                f.AddRange(Directory.GetFiles(projectFolder, "*.dll", SearchOption.TopDirectoryOnly));

                files = f.ToArray();
            }

            return files;
        }
    }
}