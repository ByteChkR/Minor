using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Xml.Serialization;
using Engine.AssetPackaging;
using Engine.BuildTools.Common;
using Engine.BuildTools.PackageCreator;

namespace Engine.BuildTools.Builder
{
    public static class Builder
    {
        private static BuildSettings LoadSettings(string path)
        {
            XmlSerializer xs = new XmlSerializer(typeof(BuildSettings));
            FileStream fs = new FileStream(path, FileMode.Open);
            BuildSettings bs = (BuildSettings)xs.Deserialize(fs);
            fs.Close();
            return bs;
        }

        public static void RunCommand(string args)
        {
            string[] a = args.Split(' ', '\n');
            RunCommand(a);
        }

        public static void RunCommand(string[] args)
        {
            StartupInfo info = new StartupInfo(args);
            info = new StartupInfo(args);
            if (!info.HasFlag("noflag"))
            {
                if (info.HasFlag("--help"))
                {
                    Console.WriteLine("--help -> Displays this Text.");
                    Console.WriteLine("--packer -> Packs Assets into the Package Format.");
                    Console.WriteLine("--embed -> Embeds Files into the .csproj File of the game.");
                    Console.WriteLine("--build -> Builds the specified .csproj File");
                    Console.WriteLine("--unembed -> Restores the Project when previously embedded with --embed.");
                    Console.WriteLine(
                        "--create-package -> Creates a Game Package that is executable by the Engine.Player");
                }

                _Main(info);
            }
            else
            {
                BuildWithXML(args, info);
            }
        }

        private static void BuildWithXML(string[] args, StartupInfo info)
        {
            if (args.Length != 0)
            {
                if (File.Exists(args[0]))
                {
                    BuildSettings bs = LoadSettings(args[0]);


                    string homeDir = AppDomain.CurrentDomain.BaseDirectory;

                    Directory.SetCurrentDirectory(Path.GetDirectoryName(Path.GetFullPath(args[0])));


                    string outFolder = Path.GetFullPath(bs.OutputFolder);
                    if (!Directory.Exists(outFolder))
                    {
                        Directory.CreateDirectory(outFolder);
                    }

                    string buildOutput = bs.CreateGamePackage ? outFolder + "/build" : outFolder;

                    string outputFolder = Path.GetFullPath(bs.OutputFolder);

                    string projectFile = Path.GetFullPath(bs.Project);

                    string projectFolder = Path.GetDirectoryName(projectFile);
                    projectFolder = Path.GetFullPath(projectFolder);

                    string assetFolder = Path.GetFullPath(bs.AssetFolder);

                    string projectName = Path.GetFileNameWithoutExtension(projectFile);

                    string publishFolder = projectFolder + "/bin/Release/netcoreapp2.1/publish";

                    if (Directory.Exists(projectFolder + "/bin"))
                    {
                        Console.WriteLine("Deleting publish folder to prevent copying the wrong assemblies.");
                        Directory.Delete(projectFolder + "/bin", true);
                    }

                    if (Directory.Exists(projectFolder + "/obj"))
                    {
                        Console.WriteLine("Deleting publish folder to prevent copying the wrong assemblies.");
                        Directory.Delete(projectFolder + "/obj", true);
                    }

                    string filePatterns = bs.UnpackFiles + "+" + bs.MemoryFiles;
                    string outputPackFolder = bs.BuildFlags == BuildType.PackOnly ? "/packs" : "/" + projectName;
                    string packSubFolder = bs.BuildFlags == BuildType.PackOnly ? "" : "/" + projectName;
                    string packFolder = projectFolder + packSubFolder;


                    bool packsCreated = false;
                    if (bs.BuildFlags == BuildType.PackEmbed || bs.BuildFlags == BuildType.PackOnly)
                    {
                        PackAssets(packFolder, bs.PackSize, bs.MemoryFiles, bs.UnpackFiles,
                            assetFolder, false);


                        packsCreated = true;
                    }

                    if (bs.BuildFlags == BuildType.PackEmbed || bs.BuildFlags == BuildType.Embed)
                    {
                        string[] files = new string[0];
                        if (packsCreated)
                        {
                            Console.WriteLine("Embedding Packs.");
                            files = Directory.GetFiles(projectFolder + "/" + projectName, "*",
                                SearchOption.AllDirectories);
                        }
                        else
                        {
                            Console.WriteLine("Embedding Files.");
                            files = CreateFileList(assetFolder, filePatterns);
                        }

                        AssemblyEmbedder.EmbedFilesIntoProject(projectFile, files);
                    }

                    BuildProject(projectFolder);

                    if (Directory.Exists(buildOutput))
                    {
                        Directory.Delete(buildOutput, true);
                    }

                    Thread.Sleep(500);
                    Directory.Move(publishFolder, buildOutput);
                    string[] debugFiles = Directory.GetFiles(buildOutput, "*.pdb", SearchOption.AllDirectories);
                    for (int i = 0; i < debugFiles.Length; i++)
                    {
                        File.Delete(debugFiles[i]);
                    }

                    if (bs.BuildFlags == BuildType.PackEmbed || bs.BuildFlags == BuildType.Embed)
                    {
                        Console.WriteLine("Unembedding items.");
                        AssemblyEmbedder.UnEmbedFilesFromProject(projectFile);
                    }


                    if (packsCreated && bs.BuildFlags == BuildType.PackOnly)
                    {
                        Console.WriteLine("Copying Packs to Output.");
                        Directory.Move(projectFolder + outputPackFolder, buildOutput + outputPackFolder);
                    }
                    else if (packsCreated && bs.BuildFlags == BuildType.PackEmbed)
                    {
                        Console.WriteLine("Deleting Generated Pack Folder.");
                        Directory.Delete(projectFolder + outputPackFolder, true);
                    }


                    if (bs.CreateGamePackage)
                    {
                        string packagerVersion = Creator.DefaultVersion;
                        if (info.HasValueFlag("--packager-version"))
                        {
                            packagerVersion = info.GetValues("--packager-version")[0];
                        }

                        string startArg = projectName + ".dll";
                        if (packagerVersion == "v2")
                        {
                            if (!info.HasValueFlag("--set-start-args"))
                            {
                                Console.WriteLine("Warning. no startup arguments specifed!");
                                startArg = "dotnet " + projectName + ".dll";
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

                        FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(buildOutput + "/Engine.dll");
                        string[] files = ParseFileList(bs.GamePackageFileList, buildOutput, projectName,
                            bs.BuildFlags == BuildType.Embed, bs.BuildFlags == BuildType.PackOnly, false);
                        Creator.CreateGamePackage(projectName, startArg, outputFolder + "/" + projectName + ".game",
                            buildOutput, files,
                            fvi.FileVersion, packagerVersion);
                    }
                }
            }
        }

        private static void _Main(StartupInfo info)
        {
            if (info.HasValueFlag("--packer"))
            {
                _PackAssets(info);
            }

            if (info.HasValueFlag("--embed"))
            {
                _EmbedFiles(info);
            }

            if (info.HasValueFlag("--build"))
            {
                _Build(info);
            }

            if (info.HasValueFlag("--unembed"))
            {
                _UnembedFiles(info);
            }

            if (info.HasValueFlag("--create-package"))
            {
                _CreateGamePackage(info);
            }

            if (info.HasValueFlag("--create-engine-package"))
            {
                _CreateEnginePackage(info);
            }
        }

        private static void BuildProject(string filepath)
        {
            int ret = BuildCommand(filepath);
            if (ret != 0)
            {
                throw new Exception("Compilation Command Failed.");
            }

            ret = PublishCommand(filepath);
            if (ret != 0)
            {
                throw new Exception("Publish Command Failed.");
            }
        }

        private static int BuildCommand(string filepath)
        {
            return ProcessUtils.RunProcess("cmd.exe", $"/C dotnet build {filepath} -c Release",
                null);
        }

        private static int PublishCommand(string filepath)
        {
            return ProcessUtils.RunProcess("cmd.exe", $"/C dotnet publish {filepath} -c Release",
                null);
        }

        private static void _CreateEnginePackage(StartupInfo info)
        {
            //1 Directory of unpacked game build
            //2 The Project Name(Must have the same name as the dll that is used to start)
            //3 The OutputFile
            //4 True/False flag that enables copying asset files from the project dir if no filelist has been given.
            //5 Optional File List

            string[] args = info.GetValues("--create-engine-package").ToArray();
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
                if (info.HasValueFlag("--packager-version"))
                {
                    packagerVersion = info.GetValues("--packager-version")[0];
                }

                BuildProject(csF);
                string[] files = ParseEngineFileList(fileList, folder);
                Creator.CreateEnginePackage(Path.GetFullPath(args[1]), folder, files, packagerVersion);
            }
            catch (Exception e)
            {
                Console.WriteLine("Could not Create Engine Package. Wrong Arguments?");
                Console.WriteLine("Arguments: <CSProjFile of engine> <TheOutputFile> <optional: file list.>");
                Console.WriteLine(e);
                throw;
            }
        }

        private static void _CreateGamePackage(StartupInfo info)
        {
            //1 Directory of unpacked game build
            //2 The Project Name(Must have the same name as the dll that is used to start)
            //3 The OutputFile
            //4 True/False flag that enables copying asset files from the project dir if no filelist has been given.
            //5 Optional File List

            string[] args = info.GetValues("--create-package").ToArray();
            try
            {
                Console.WriteLine(Path.GetFullPath(args[0]));
                Console.WriteLine(Path.GetFullPath(args[2]));
                string version = "";
                if (!info.HasValueFlag("--packer-override-engine-version"))
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
                if (info.HasValueFlag("--packager-version"))
                {
                    packagerVersion = info.GetValues("--packager-version")[0];
                    if (packagerVersion == "v2")
                    {
                        if (!info.HasValueFlag("--set-start-args"))
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

                string[] files = ParseFileList(fileList, Path.GetFullPath(args[0]), args[1], bool.Parse(args[3]),
                    bool.Parse(args[4]), standalone);
                Creator.CreateGamePackage(args[1], startArg, Path.GetFullPath(args[2]),
                    Path.GetFullPath(args[0]), files, version, packagerVersion);
            }
            catch (Exception e)
            {
                Console.WriteLine("Could not Create Game Package. Wrong Arguments?");
                Console.WriteLine(
                    "Arguments: <DirectoryOfUnpackedBuild> <ProjectName> <TheOutputFile> <CopyAssetsOnError(bool)> <CopyPacksOnError(bool)> <optional: file list.>");
                Console.WriteLine(e);
                throw;
            }
        }

        private static void _EmbedFiles(StartupInfo info)
        {
            string[] args = info.GetValues("--embed").ToArray();
            try
            {
                string[] files = Directory.GetFiles(Path.GetFullPath(args[1]), "*", SearchOption.AllDirectories);
                AssemblyEmbedder.EmbedFilesIntoProject(Path.GetFullPath(args[0]), files);
            }
            catch (Exception e)
            {
                Console.WriteLine("Could not Embed Folder into Project. Wrong Arguments?");
                Console.WriteLine(
                    "Arguments: <ProjectFile(.csproj)> <DirectoryToEmbed(Has to be in subdirectories of the ProjectFile>");
                Console.WriteLine(e);
                throw;
            }
        }

        private static void _UnembedFiles(StartupInfo info)
        {
            string[] args = info.GetValues("--unembed").ToArray();
            try
            {
                AssemblyEmbedder.UnEmbedFilesFromProject(Path.GetFullPath(args[0]));
            }
            catch (Exception e)
            {
                Console.WriteLine("Could not Unembed Assets from Project. Wrong Arguments/No Backup File?");
                Console.WriteLine("Arguments: <ProjectFile(.csproj)>");
                Console.WriteLine(e);
                throw;
            }
        }

        private static void _Build(StartupInfo info)
        {
            string[] args = info.GetValues("--build").ToArray();
            try
            {
                string projectFolder = Path.GetDirectoryName(Path.GetFullPath(args[0]));
                string publishFolder = projectFolder + "/bin/Release/netcoreapp2.1/publish";
                Console.WriteLine(publishFolder);
                if (Directory.Exists(projectFolder + "/bin"))
                {
                    Console.WriteLine("Deleting publish folder to prevent copying the wrong assemblies.");
                    Directory.Delete(projectFolder + "/bin", true);
                }

                if (Directory.Exists(projectFolder + "/obj"))
                {
                    Console.WriteLine("Deleting publish folder to prevent copying the wrong assemblies.");
                    Directory.Delete(projectFolder + "/obj", true);
                }

                BuildProject(args[0]);
                

                //Making sure that the root path Path is existing
                IoUtils.CreateDirectoryPath(Path.GetFullPath(args[1]));
                Directory.Delete(Path.GetFullPath(args[1]), true);

                Directory.Move(publishFolder, Path.GetFullPath(args[1]));
                string[] debugFiles =
                    Directory.GetFiles(Path.GetFullPath(args[1]), "*.pdb", SearchOption.AllDirectories);
                for (int i = 0; i < debugFiles.Length; i++)
                {
                    File.Delete(debugFiles[i]);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Could not Build or Copy Project Project. Wrong Arguments/No Backup File?");
                Console.WriteLine("Arguments: <ProjectFile(.csproj)> <OutputFolder>");
                Console.WriteLine(e);
                throw;
            }
        }

        private static void _PackAssets(StartupInfo info)
        {
            string[] args = info.GetValues("--packer").ToArray();
            try
            {
                PackAssets(Path.GetFullPath(args[0]), int.Parse(args[1]), args[2], args[3],
                    Path.GetFullPath(args[4]), false);
            }
            catch (Exception e)
            {
                Console.WriteLine("Could not Package Assets. Wrong Arguments?");
                Console.WriteLine("Arguments: <OutputFolder> <PackSize> <memoryExts> <unpackExts> <assetFolder>");
                Console.WriteLine(e);
                throw;
            }
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
                info.FileInfos.Add(unpackExts[i], new AssetFileInfo { packageType = AssetPackageType.Unpack });
            }

            List<string> packExts = memoryFileExts.Split("+".ToCharArray(), StringSplitOptions.RemoveEmptyEntries)
                .ToList();
            for (int i = 0; i < packExts.Count; i++)
            {
                info.FileInfos.Add(packExts[i], new AssetFileInfo { packageType = AssetPackageType.Memory });
            }

            return info;
        }

        private static void PackAssets(string outputFolder, int packSize, string memoryFileExts,
            string unpackedFileExts, string assetFolder, bool compression)
        {
            AssetPacker.MaxsizeKilobytes = packSize;

            Console.WriteLine("Parsing File info...");

            AssetPackageInfo info = CreatePackageInfo(memoryFileExts, unpackedFileExts);

            Console.WriteLine("Creating Asset Pack(" + assetFolder + ")...");
            AssetResult ret = AssetPacker.PackAssets(assetFolder, info, compression);
            Console.WriteLine("Packaging " + ret.indexList.Count + " Assets in " + ret.packs.Count + " Packs.");

            Console.WriteLine("Saving Asset Pack to " + outputFolder);
            ret.Save(outputFolder);

            Console.WriteLine("Packaging Assets Finished.");
        }

        private static string[] ParseFileList(string fileList, string projectFolder, string projectName,
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

        private static string[] ParseEngineFileList(string fileList, string projectFolder)
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