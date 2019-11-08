using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Xml.Serialization;
using Engine.AssetPackaging;
using Engine.Player.Common;
using Engine.Player.PackageCreator;

namespace Engine.Player.Builder
{
    class Program
    {
        private class StartupInfo
        {
            private Dictionary<string, List<string>> values = new Dictionary<string, List<string>>();
            public StartupInfo(string[] args)
            {
                for (int i = 0; i < args.Length; i++)
                {
                    if (args[i].StartsWith("--") || i == 0)
                    {
                        List<string> argValues = new List<string>();
                        for (int j = i + 1; j < args.Length; j++)
                        {
                            if (args[j].StartsWith("--")) break;
                            argValues.Add(args[j]);
                        }

                        if (i == 0 && !args[i].StartsWith("--"))
                        {
                            values.Add("noflag", argValues);
                        }
                        else
                        {
                            values.Add(args[i], argValues);
                        }
                    }
                }

            }

            public List<string> GetValues(string flag)
            {
                return values[flag];
            }
            public bool HasFlag(string flag)
            {
                return values.ContainsKey(flag);
            }

            public bool HasValueFlag(string flag)
            {
                return HasFlag(flag) && values[flag].Count != 0;
            }

            public static List<string> ResolveFileReferences(string arg)
            {
                if (arg.StartsWith('@')) return File.ReadAllLines(arg.Remove(0, 1)).ToList();
                return new List<string>() { arg };
            }
        }
        private static BuildSettings LoadSettings(string path)
        {
            XmlSerializer xs = new XmlSerializer(typeof(BuildSettings));
            FileStream fs = new FileStream(path, FileMode.Open);
            BuildSettings bs = (BuildSettings)xs.Deserialize(fs);
            fs.Close();
            return bs;
        }

        static void _Main(StartupInfo info)
        {

            if (info.HasValueFlag("--packer"))
            {
                _PackAssets(info.GetValues("--packer").ToArray());
            }

            if (info.HasValueFlag("--embed"))
            {
                _EmbedFiles(info.GetValues("--embed").ToArray());
            }

            if (info.HasValueFlag("--build"))
            {
                _Build(info.GetValues("--build").ToArray());

            }

            if (info.HasValueFlag("--unembed"))
            {
                _UnembedFiles(info.GetValues("--unembed").ToArray());

            }

            if (info.HasValueFlag("--create-package"))
            {
                _CreateGamePackage(info.GetValues("--create-package").ToArray());
            }

        }

        static void _CreateGamePackage(string[] args)
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
                FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(Path.GetFullPath(args[0]) + "/Engine.dll");
                string fileList;
                if (args.Length > 4)
                {
                    fileList = Path.GetFullPath(args[4]);
                    Console.WriteLine(Path.GetFullPath(args[4]));
                }
                else
                {
                    fileList = "";
                }

                string[] files = ParseFileList(fileList, Path.GetFullPath(args[0]), args[1], bool.Parse(args[3]));
                Creator.CreateGamePackage(Path.GetFullPath(args[2]), Path.GetFullPath(args[0]), files, fvi.FileVersion);
            }
            catch (Exception e)
            {
                Console.WriteLine("Could not Create Game Package. Wrong Arguments?");
                Console.WriteLine("Arguments: <DirectoryOfUnpackedBuild> <ProjectName> <TheOutputFile> <CopyAssetsOnError(bool)> optional file list.");
                Console.WriteLine(e);
                throw;
            }
        }

        static void _EmbedFiles(string[] args)
        {
            try
            {
                string[] files = Directory.GetFiles(Path.GetFullPath(args[1]), "*", SearchOption.AllDirectories);
                AssemblyEmbedder.EmbedFilesIntoProject(Path.GetFullPath(args[0]), files);
            }
            catch (Exception e)
            {
                Console.WriteLine("Could not Embed Folder into Project. Wrong Arguments?");
                Console.WriteLine("Arguments: <ProjectFile(.csproj)> <DirectoryToEmbed(Has to be in subdirectories of the ProjectFile>");
                Console.WriteLine(e);
                throw;
            }
        }

        static void _UnembedFiles(string[] args)
        {
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

        static void _Build(string[] args)
        {
            try
            {
                ProcessUtils.RunProcess(AppDomain.CurrentDomain.BaseDirectory + "resources/project_build.bat", args[0], null);


                string projectFolder = Path.GetDirectoryName(args[0]);
                string publishFolder = projectFolder + "/bin/Release/netcoreapp2.1/publish";
                if(Directory.Exists(Path.GetFullPath(args[1]))) Directory.Delete(Path.GetFullPath(args[1]), true);
                Directory.Move(publishFolder, Path.GetFullPath(args[1]));
                string[] debugFiles = Directory.GetFiles(Path.GetFullPath(args[1]), "*.pdb", SearchOption.AllDirectories);
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

        static void _PackAssets(string[] args)
        {
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


        static void Main(string[] args)
        {
            StartupInfo info = new StartupInfo(args);
            if (!info.HasFlag("noflag"))
            {
                if (info.HasFlag("--help"))
                {
                    Console.WriteLine("--help -> Displays this Text.");
                    Console.WriteLine("--packer -> Packs Assets into the Package Format.");
                    Console.WriteLine("--embed -> Embeds Files into the .csproj File of the game.");
                    Console.WriteLine("--build -> Builds the specified .csproj File");
                    Console.WriteLine("--unembed -> Restores the Project when previously embedded with --embed.");
                    Console.WriteLine("--create-package -> Creates a Game Package that is executable by the Engine.Player");
                }
                _Main(info);

            }
            else
            {
                if (args.Length != 0)
                {
                    if (File.Exists(args[0]))
                    {
                        BuildSettings bs = LoadSettings(args[0]);
                        if (!Directory.Exists(bs.OutputFolder)) Directory.CreateDirectory(bs.OutputFolder);



                        string homeDir = AppDomain.CurrentDomain.BaseDirectory;

                        Directory.SetCurrentDirectory(Path.GetDirectoryName(Path.GetFullPath(args[0])));

                        string buildOutput = bs.CreateGamePackage ? bs.OutputFolder + "/build" : bs.OutputFolder;
                        buildOutput = Path.GetFullPath(buildOutput);

                        string outputFolder = Path.GetFullPath(bs.OutputFolder);

                        string projectFile = Path.GetFullPath(bs.Project);

                        string projectFolder = Path.GetDirectoryName(projectFile);
                        projectFolder = Path.GetFullPath(projectFolder);

                        string assetFolder = Path.GetFullPath(bs.AssetFolder);

                        string projectName = Path.GetFileNameWithoutExtension(projectFile);

                        string publishFolder = projectFolder + "/bin/Release/netcoreapp2.1/publish";
                        string filePatterns = bs.UnpackFiles + "+" + bs.MemoryFiles;
                        string outputPackFolder = bs.BuildFlags == BuildType.PackOnly ? "/packs" : "/" + projectName;
                        string packSubFolder = bs.BuildFlags == BuildType.PackOnly ? "" : "/" + projectName;
                        string packFolder = projectFolder + packSubFolder;

                        //After Build
                        //if (!Directory.Exists(bs.OutputFolder + "/build"))
                        //    Directory.CreateDirectory(bs.OutputFolder + "/build");
                        //Directory.Move(projectFolder + outputPackFolder, bs.OutputFolder + "build/" + outputPackFolder);


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
                                files = Directory.GetFiles(projectFolder + "/" + projectName, "*",
                                    SearchOption.AllDirectories);
                            }
                            else
                            {
                                files = CreateFileList(assetFolder, filePatterns);
                            }

                            AssemblyEmbedder.EmbedFilesIntoProject(projectFile, files);
                        }
                        ProcessUtils.RunProcess(homeDir + "resources/project_build.bat", projectFile, null);

                        if (Directory.Exists(buildOutput)) Directory.Delete(buildOutput, true);
                        Thread.Sleep(500);
                        Directory.Move(publishFolder, buildOutput);
                        string[] debugFiles = Directory.GetFiles(buildOutput, "*.pdb", SearchOption.AllDirectories);
                        for (int i = 0; i < debugFiles.Length; i++)
                        {
                            File.Delete(debugFiles[i]);
                        }

                        if (bs.BuildFlags == BuildType.PackEmbed || bs.BuildFlags == BuildType.Embed)
                            AssemblyEmbedder.UnEmbedFilesFromProject(projectFile);

                        
                        if (packsCreated && bs.BuildFlags == BuildType.PackOnly)
                            Directory.Move(projectFolder + outputPackFolder, buildOutput + outputPackFolder);





                        if (bs.CreateGamePackage)
                        {
                            FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(buildOutput + "/Engine.dll");
                            string[] files = ParseFileList(bs.GamePackageFileList, buildOutput, projectName,
                                bs.BuildFlags == BuildType.Embed);
                            Creator.CreateGamePackage(outputFolder + "/" + projectName + ".game", buildOutput, files, fvi.FileVersion);
                        }
                    }
                }
            }

            

            
        }

        public static string[] CreateFileList(string path, string searchPatterns, char separator = '+')
        {
            string[] patterns = searchPatterns.Split(separator, StringSplitOptions.RemoveEmptyEntries);
            List<string> ret = new List<string>();
            for (int i = 0; i < patterns.Length; i++)
            {
                ret.AddRange(Directory.GetFiles(path, patterns[i], SearchOption.AllDirectories));
            }

            return ret.ToArray();
        }

        public static AssetPackageInfo CreatePackageInfo(string memoryFileExts, string unpackedFileExts)
        {
            AssetPackageInfo info = new AssetPackageInfo();
            List<string> unpackExts = unpackedFileExts.Split('+', StringSplitOptions.RemoveEmptyEntries).ToList();
            for (int i = 0; i < unpackExts.Count; i++)
            {
                info.FileInfos.Add(unpackExts[i], new AssetFileInfo() { packageType = AssetPackageType.Unpack });
            }

            List<string> packExts = memoryFileExts.Split("+".ToCharArray(), StringSplitOptions.RemoveEmptyEntries)
                .ToList();
            for (int i = 0; i < packExts.Count; i++)
            {
                info.FileInfos.Add(packExts[i], new AssetFileInfo() { packageType = AssetPackageType.Memory });
            }

            return info;
        }

        private static void PackAssets(string outputFolder, int packSize, string memoryFileExts,
            string unpackedFileExts, string assetFolder, bool compression)
        {
            AssetPacker.MAXSIZE_KILOBYTES = packSize;

            Console.WriteLine("Parsing File info...");

            AssetPackageInfo info = CreatePackageInfo(memoryFileExts, unpackedFileExts);

            Console.WriteLine("Creating Asset Pack(" + assetFolder + ")...");
            AssetResult ret = AssetPacker.PackAssets(assetFolder, info, compression);
            Console.WriteLine("Packaging " + ret.indexList.Count + " Assets in " + ret.packs.Count + " Packs.");

            Console.WriteLine("Saving Asset Pack to " + outputFolder);
            ret.Save(outputFolder);

            Console.WriteLine("Packaging Assets Finished.");
        }

        private static string[] ParseFileList(string fileList, string projectFolder, string projectName, bool copyAssetsWhenError)
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
                    f.AddRange(Directory.GetFiles(assetFolder, "*", SearchOption.AllDirectories));
                if (Directory.Exists(packFolder))
                    f.AddRange(Directory.GetFiles(packFolder, "*", SearchOption.AllDirectories));
                f.Add(projectFolder + "/" + projectName + ".dll");
                f.Add(projectFolder + "/" + projectName + ".runtimeconfig.json");
                f.Add(projectFolder + "/" + projectName + ".deps.json");
                files = f.ToArray();
            }

            return files;
        }
    }
}