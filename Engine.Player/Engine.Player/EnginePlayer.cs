using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using CommandRunner;
using Engine.BuildTools.Common;
using Engine.BuildTools.PackageCreator;
using Engine.BuildTools.PackageCreator.Versions;
using Engine.Player.Commands;
using Microsoft.Win32;

namespace Engine.Player
{
    internal class EnginePlayer
    {
        
        public static List<string> EngineVersions = new List<string>();
        public static bool DontReadLine;
        public static WebClient Wc = new WebClient();

        public static string EngineVersion;
        public static string EngineDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "engine");
        public static string GameTempDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "_game");
        public static string GameDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "game");

        public static Version[] AvailableVersionsOnServer;

        public static void RegisterExtension(string ext)
        {
            RegistryKey key = Registry.ClassesRoot.CreateSubKey(ext);
            key.SetValue("", "DotNetPlayer");
            key.Close();

            key = Registry.ClassesRoot.CreateSubKey(ext + "\\Shell\\Open\\command");
            //key = key.CreateSubKey("command");

            string loc = Assembly.GetExecutingAssembly().Location;
            key.SetValue("", "\"" + loc + "\" \"%L\"");
            key.Close();

            key = Registry.ClassesRoot.CreateSubKey(ext + "\\DefaultIcon");
            key.SetValue("", loc);
            key.Close();
        }

        public static void AddToPathVariable()
        {
            const string name = "PATH";
            string pathvar = System.Environment.GetEnvironmentVariable(name, EnvironmentVariableTarget.Machine);
            string appPath = AppDomain.CurrentDomain.BaseDirectory;
            appPath = appPath.Remove(appPath.Length - 1, 1);
            Console.WriteLine("Adding Path: " + appPath);
            var value = pathvar + ";" + appPath;
            var target = EnvironmentVariableTarget.Machine;
            System.Environment.SetEnvironmentVariable(name, value, target);
        }

        public static void UpdatePathVariable()
        {
            const string name = "PATH";
            string pathvar = System.Environment.GetEnvironmentVariable(name, EnvironmentVariableTarget.Machine);
            Console.WriteLine(pathvar);
            string appPath = AppDomain.CurrentDomain.BaseDirectory;
            appPath = appPath.Remove(appPath.Length - 1, 1);
            string[] paths = pathvar.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            string val = "";
            for (int i = 0; i < paths.Length; i++)
            {
                if (File.Exists(paths[i] + "\\Engine.BuildTools.Builder.dll"))
                {
                    Console.WriteLine("Updating Path: " + paths[i] + " >> " + appPath);
                    paths[i] = appPath;
                }

                val += paths[i] + ";";
            }

            var target = EnvironmentVariableTarget.Machine;
            System.Environment.SetEnvironmentVariable(name, val, target);
        }

        public static bool IsInPathVariable()
        {
            const string name = "PATH";
            string pathvar = System.Environment.GetEnvironmentVariable(name, EnvironmentVariableTarget.Machine);
            string[] paths = pathvar.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < paths.Length; i++)
            {
                if (File.Exists(paths[i] + "\\Engine.BuildTools.Builder.dll"))
                {
                    return true;
                }
            }

            return false;
        }

        public static void RegisterExtensions()
        {
            RegisterExtension(".game");
            RegisterExtension(".engine");
        }

        private static void ReadLine()
        {
            if (!DontReadLine)
            {
                Console.WriteLine("Press Enter to Continue...");
                Console.ReadLine();
            }
        }

       
        public static string GetEnginePath(string version)
        {
            return EngineDir + "/" + version + ".engine";
        }
        

        private static void Main(string[] args)
        {
            string callDir = Directory.GetCurrentDirectory();
            
            Wc.DownloadFileCompleted += WcOnDownloadFileCompleted;

            string homeDir = AppDomain.CurrentDomain.BaseDirectory;

            Console.CancelKeyPress += ConsoleOnCancelKeyPress;
            if (!Directory.Exists(EngineDir))
            {
                Directory.CreateDirectory(EngineDir);
            }

            GetEngineServerVersion();
            EngineVersions = Directory.GetFiles(EngineDir, "*.engine", SearchOption.TopDirectoryOnly)
                .Select(x => Path.GetFileNameWithoutExtension(x)).ToList();


            //Command def = Command.CreateCommand(DefaultCommand, "--run <Path/To/File.game>", "--run");
            //CommandRunner.SetDefaultCommand(def);

            //CommandRunner.AddCommand(Command.CreateCommand(_Update, "--update Updates the Build Tools", "--update"));
            //CommandRunner.AddCommand(Command.CreateCommand(SetDefaultProgramCommand, "Requires Admin Permissions",
            //    "--set-default-program", "-sD"));
            //CommandRunner.AddCommand(Command.CreateCommand(AddToPathVariable, "Requires Admin Permissions",
            //    "--add-to-path"));
            //CommandRunner.AddCommand(Command.CreateCommand(NoHaltCommand, "Does not wait for user input before exiting",
            //    "--no-halt", "-nH"));
            //CommandRunner.AddCommand(Command.CreateCommand(HelpCommand, "Display this help message", "--help", "-h"));
            //CommandRunner.AddCommand(Command.CreateCommand(SetEnginePathCommand,
            //    "--engine-path <Path/To/File.game>\nSpecify a manual path to a .engine file", "--engine-path", "-eP"));
            //CommandRunner.AddCommand(Command.CreateCommand(SetEngineVersionCommand,
            //    "--engine <Version>\nSpecify a manual version", "--engine", "-e"));
            //CommandRunner.AddCommand(Command.CreateCommand(ListPackageInfo,
            //    "--list-info <<Path/To/File>\nLists Information about the .engine or .game file.", "--list-info",
            //    "-l"));

            //CommandRunner.AddCommand(Command.CreateCommand(RemoveEngineCommand,
            //    "--remove-engine <Version>\nRemoves an engine Version from the engine cache", "--remove-engine", "-r"));
            //CommandRunner.AddCommand(Command.CreateCommand(ClearCacheCommand,
            //    "--clear-cache\nClears all engines in the cache", "--clear-cache", "-cC"));
            //CommandRunner.AddCommand(Command.CreateCommand(AddEngineCommand,
            //    "--add-engine <<Path/To/File.engine>\nAdds an engine file to the engine cache", "--add-engine", "-a"));
            //CommandRunner.AddCommand(Command.CreateCommand(DownloadEngineCommand,
            //    "--download-engine <Version>\n Tries to download a specified engine version", "--download-engine",
            //    "-d"));

            //CommandRunner.AddCommand(def);

            Runner.AddAssembly(Assembly.GetExecutingAssembly());
            Runner.RunCommands(args);
            ReadLine();
        }

        public static void ConsoleOnCancelKeyPress(object sender, ConsoleCancelEventArgs e)
        {
            e.Cancel = true;
        }

        public static void CreateFolder(string path)
        {
            List<string> s = new List<string>();
            string curpath = Path.GetDirectoryName(path);
            Console.WriteLine("Path: " + curpath);
            do
            {
                s.Add(curpath);
                curpath = Path.GetDirectoryName(curpath);
            } while (curpath != null && curpath.Trim() != "");

            s.Reverse();
            for (int i = 0; i < s.Count; i++)
            {
                if (!Directory.Exists(s[i]))
                {
                    Directory.CreateDirectory(s[i]);
                }
            }
        }

        public static void GetEngineServerVersion()
        {
            Console.WriteLine("Downloading Version List..");
            string s = Wc.DownloadString("http://213.109.162.193/apps/EngineArchives/version.list");

            string[] versions = s.Split('\n');
            List<Version> versionList = new List<Version>();
            for (int i = 0; i < versions.Length; i++)
            {
                if (Version.TryParse(versions[i], out Version parsedResult))
                {
                    versionList.Add(parsedResult);
                }
            }

            versionList.Sort();
            AvailableVersionsOnServer = versionList.ToArray();
            Console.WriteLine("Fetched Version from Server");
        }


        public static void DownloadEngineVersion(string version)
        {
            if (IsVersionUrlCorrect(version))
            {
                Console.WriteLine("Downloading Version: " + version);
                Wc.DownloadFile(new Uri("http://213.109.162.193/apps/EngineArchives/" + version + ".engine"),
                    EngineDir + "/" + version + ".engine");
            }
        }

        public static void WcOnDownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            Console.WriteLine("");
            Console.WriteLine("Success: " + !e.Cancelled);
        }




        public static bool IsEngineVersionAvailable(string version)
        {
            return File.Exists(EngineDir + "/" + version + ".engine");
        }

        

        
        public static bool IsVersionUrlCorrect(string version)
        {
            string addr = $"http://213.109.162.193/apps/EngineArchives/{version}.engine";
            HttpWebResponse response = null;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(addr);
            request.Method = "HEAD";

            bool ret = false;
            try
            {
                response = (HttpWebResponse)request.GetResponse();
                ret = true;
            }
            catch (Exception)
            {
                ret = false;
            }
            finally
            {
                // Don't forget to close your response.
                if (response != null)
                {
                    response.Close();
                    ret = true;
                }
            }

            return ret;
        }
    }
}