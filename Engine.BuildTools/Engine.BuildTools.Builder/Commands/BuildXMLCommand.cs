using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using CommandRunner;
using Engine.BuildTools.Common;
using Engine.BuildTools.PackageCreator;

namespace Engine.BuildTools.Builder.Commands
{
    public class BuildXMLCommand :AbstractCommand
    {

        private static string[] CreateFileList(string path, string searchPatterns, char separator = '+')
        {
            string[] patterns = searchPatterns.Split(new[] { separator }, StringSplitOptions.RemoveEmptyEntries);
            List<string> ret = new List<string>();
            for (int i = 0; i < patterns.Length; i++)
            {
                ret.AddRange(Directory.GetFiles(path, patterns[i], SearchOption.AllDirectories));
            }

            return ret.ToArray();
        }

        private static void BuildXML(StartupInfo info, string[] args)
        {
            if (args.Length != 0)
            {
                string file = args[0];
                if (File.Exists(file))
                {
                    BuildSettings bs = Builder.LoadSettings(file);


                    string homeDir = AppDomain.CurrentDomain.BaseDirectory;

                    Directory.SetCurrentDirectory(Path.GetDirectoryName(Path.GetFullPath(file)));


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
                    if ((bs.BuildFlags == BuildType.PackEmbed || bs.BuildFlags == BuildType.PackOnly) && Directory.Exists(assetFolder))
                    {
                        Builder.PackAssets(packFolder, bs.PackSize, bs.MemoryFiles, bs.UnpackFiles,
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

                    Builder.BuildProject(projectFolder);

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

                    if (bs.BuildFlags == BuildType.Copy)
                    {
                        Console.WriteLine("Copying Assets to output");
                        string buildAssetDir = assetFolder.Replace(projectFolder, buildOutput);
                        Directory.CreateDirectory(buildAssetDir);

                        foreach (string dirPath in Directory.GetDirectories(assetFolder, "*",
                            SearchOption.AllDirectories))
                            Directory.CreateDirectory(dirPath.Replace(projectFolder, buildOutput));

                        //Copy all the files & Replaces any files with the same name
                        foreach (string newPath in Directory.GetFiles(assetFolder, "*.*",
                            SearchOption.AllDirectories))
                            File.Copy(newPath, newPath.Replace(projectFolder, buildOutput), true);
                    }


                    if (bs.CreateGamePackage)
                    {
                        string packagerVersion = string.IsNullOrEmpty(bs.PackagerVersion) ? Creator.DefaultVersion : bs.PackagerVersion;
                        if (info.GetCommandEntries("--packager-version") != 0)
                        {
                            packagerVersion = info.GetValues("--packager-version")[0];
                        }

                        string startArg = projectName + ".dll";
                        if (packagerVersion == "v2")
                        {
                            if (info.GetCommandEntries("--set-start-args") == 0)
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
                        string[] files = Builder.ParseFileList(bs.GamePackageFileList, buildOutput, projectName,
                            bs.BuildFlags == BuildType.Copy, bs.BuildFlags == BuildType.PackOnly, false);
                        Creator.CreateGamePackage(projectName, startArg, outputFolder + "/" + projectName + ".game",
                            buildOutput, files,
                            fvi.FileVersion, packagerVersion);
                    }
                }
            }
        }

        public BuildXMLCommand() : base(BuildXML,  new []{ "--xml" }, "--xml <Path/To/File.xml>", true)
        {

        }
    }
}