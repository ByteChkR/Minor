using System;
using System.CodeDom;
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
using Engine.BuildTools.Builder;
using Engine.BuildTools.Common;
using Engine.BuildTools.PackageCreator;
using Microsoft.Win32;

namespace Engine.Player
{
    internal class EnginePlayer
    {
        private static List<string> engineversions = new List<string>();
        private static Process p;
        private static bool DontReadLine;
        private static StartupInfo info;
        public static WebClient wc = new WebClient();

        private static string EngineVersion = null;


        private const int MF_BYCOMMAND = 0x00000000;
        public const int SC_CLOSE = 0xF060;

        [DllImport("user32.dll")]
        public static extern int DeleteMenu(IntPtr hMenu, int nPosition, int wFlags);

        [DllImport("user32.dll")]
        private static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);

        [DllImport("kernel32.dll", ExactSpelling = true)]
        private static extern IntPtr GetConsoleWindow();


        private static void RegisterExtension(string ext)
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

        private static void RegisterExtensions()
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

        private static void DefaultCommand(StartupInfo info, string[] args)
        {
            if (args.Length == 0 || !File.Exists(args[0]))
            {
                Console.WriteLine("Drag a file onto the executable, or specify the path in the command line.");
                HelpCommand(info, args);

                return;
            }
            else if (args[0].EndsWith(".engine"))
            {
                AddEngineCommand(info, args);
                return;
            }
            else if (args[0].EndsWith(".game"))
            {
                RunGameCommand(info, args);
                return;
            }
        }

        private static void SetUpDirectoryStructure()
        {
            if (Directory.Exists("game"))
            {
                Directory.Delete("game", true);
            }

            if (Directory.Exists("_game"))
            {
                Directory.Delete("_game", true);
            }

            DirectoryInfo di = Directory.CreateDirectory("game");
            DirectoryInfo _di = Directory.CreateDirectory("_game");
            if ((di.Attributes & FileAttributes.Hidden) != FileAttributes.Hidden)
            {
                //Add Hidden flag    
                di.Attributes |= FileAttributes.Hidden;
            }

            if ((_di.Attributes & FileAttributes.Hidden) != FileAttributes.Hidden)
            {
                //Add Hidden flag    
                _di.Attributes |= FileAttributes.Hidden;
            }

        }

        private static string GetEnginePath(string version)
        {
            return "engine/" + version + ".engine";
        }

        private static void RunGameCommand(StartupInfo info, string[] args)
        {
            //ParseArgs(args);
            if (args.Length == 0 || !args[0].EndsWith(".game") || !File.Exists(args[0]))
            {
                Console.WriteLine("Could not load Game File");
                return;
            }

            SetUpDirectoryStructure();
            PackageManifest pm = new PackageManifest();
            try
            {
                pm = Creator.ReadManifest(args[0]);
                Console.Title = pm.Title;
                string path = pm.Version;
                if (EngineVersion != null)
                {
                    path = EngineVersion;
                }
                LoadEngine(path);
                //Load Game
                LoadGame(args[0], pm);
            }
            catch (Exception e)
            {
                Directory.Delete("_game", true);
                Directory.Delete("game", true);
                Console.WriteLine("Error Unpacking File.");
                Console.WriteLine(e);
            }

            string fileToStart = pm.Executable;
            DeleteMenu(GetSystemMenu(GetConsoleWindow(), false), SC_CLOSE, MF_BYCOMMAND);
            p = new Process();

            ProcessStartInfo psi = new ProcessStartInfo();
            psi.FileName = "cmd.exe";
            psi.RedirectStandardError = true;
            psi.RedirectStandardOutput = true;
            psi.UseShellExecute = false;
            psi.CreateNoWindow = true;
            psi.Arguments = $"/C dotnet {fileToStart}";
            psi.WorkingDirectory = Directory.GetCurrentDirectory() + "/game";
            p.StartInfo = psi;
            p.Start();

            ConsoleRedirector crd =
                ConsoleRedirector.CreateRedirector(p.StandardOutput, p.StandardError, p, Console.WriteLine);

            crd.StartThreads();

            while (!p.HasExited)
            {
                Thread.Sleep(150);
            }

            crd.StopThreads();
            Console.WriteLine(crd.GetRemainingLogs());
            Directory.Delete("game", true);
        }

        private static void HelpCommand(StartupInfo info, string[] args)
        {
            Console.WriteLine("Commands:");
            for (int i = 0; i < CommandRunner.CommandCount; i++)
            {
                Console.WriteLine(CommandRunner.GetCommandAt(i));
            }
        }

        private static void AddEngineCommand(StartupInfo info, string[] args)
        {
            if (args.Length == 0 || !File.Exists(args[0]) || !args[0].EndsWith(".engine"))
            {
                Console.WriteLine("Could not load Engine Path");
                return;
            }
            AddEngine(args[0]);


        }

        private static void UpdateEngineCommand(StartupInfo info, string[] args)
        {
            CheckUpdates();
        }

        private static void DownloadEngineCommand(StartupInfo info, string[] args)
        {
            if (!IsEngineVersionAvailable(args[0]))
            {
                Console.WriteLine("Downloading Version " + args[0]);
                DownloadEngineVersion(args[0]);
            }
            else
            {
                Console.WriteLine("Engine Version not available. Skipping Download.");
            }
        }

        private static void NoHaltCommand(StartupInfo info, string[] args)
        {
            DontReadLine = true;
        }
        private static void SetDefaultProgramCommand(StartupInfo info, string[] args)
        {
            try
            {
                RegisterExtensions();
            }
            catch (Exception e)
            {
                Console.WriteLine("Could not access registry. Administrator rights are reequired once.");
                Console.WriteLine(e);
            }
        }

        private static void SetEngineVersionCommand(StartupInfo info, string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("No engine version specified");
            }
            else
            {
                Console.WriteLine("Overriding Engine Version: " + args[0]);
                EngineVersion = args[0];
            }
        }

        private static void SetEnginePathCommand(StartupInfo info, string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("No engine path specified");
            }
            else
            {
                Console.WriteLine("Overriding Engine Path: " + args[0]);
                EngineVersion = "path:" + args[0];
            }
        }

        private static void RemoveEngineCommand(StartupInfo info, string[] args)
        {
            if (IsEngineVersionAvailable(args[0]))
            {
                Console.WriteLine("Deleting Version " + args[0]);
                File.Delete("engine/" + args[0] + ".engine");
            }
            else
            {
                Console.WriteLine("Engine Version not available. Skipping Deletion.");
            }
        }

        private static void ClearCacheCommand(StartupInfo info, string[] args)
        {
            Console.WriteLine("Deleting Engine Cache...");
            if (Directory.Exists("engine"))
            {
                string[] files = Directory.GetFiles("engine", "*", SearchOption.AllDirectories);
                for (int i = 0; i < files.Length; i++)
                {
                    File.Delete(files[i]);
                }
            }
        }

        private static void ListPackageInfo(StartupInfo info, string[] args)
        {
            if (args.Length == 0 || !File.Exists(args[0]) || (!args[0].EndsWith(".game") && !args[0].EndsWith(".engine")))
            {
                Console.WriteLine("Could not find file");
                return;
            }
            PackageManifest pm = Creator.ReadManifest(args[0]);
            Console.WriteLine(pm);
        }

        private static void Main(string[] args)
        {
            info = new StartupInfo(args);


            wc.DownloadProgressChanged += WcOnDownloadProgressChanged;
            wc.DownloadFileCompleted += WcOnDownloadFileCompleted;


            Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);

            Console.CancelKeyPress += ConsoleOnCancelKeyPress;
            if (!Directory.Exists("engine"))
            {
                Directory.CreateDirectory("engine");
            }

            engineversions = Directory.GetFiles("engine", "*.engine", SearchOption.TopDirectoryOnly).Select(x => Path.GetFileNameWithoutExtension(x)).ToList();


            Command def = Command.CreateCommand(DefaultCommand, "--run");
            CommandRunner.SetDefaultCommand(def);

            CommandRunner.AddCommand(Command.CreateCommand(SetDefaultProgramCommand, "--set-default-program", "-sD"));
            CommandRunner.AddCommand(Command.CreateCommand(NoHaltCommand, "--no-halt", "-nH"));
            CommandRunner.AddCommand(Command.CreateCommand(HelpCommand, "--help", "-h"));
            CommandRunner.AddCommand(Command.CreateCommand(UpdateEngineCommand, "--update", "-u"));
            CommandRunner.AddCommand(Command.CreateCommand(SetEnginePathCommand, "--engine-path", "-eP"));
            CommandRunner.AddCommand(Command.CreateCommand(SetEngineVersionCommand, "--engine", "-e"));
            CommandRunner.AddCommand(Command.CreateCommand(ListPackageInfo, "--list-info", "-l"));

            CommandRunner.AddCommand(Command.CreateCommand(RemoveEngineCommand, "--remove-engine", "-r"));
            CommandRunner.AddCommand(Command.CreateCommand(ClearCacheCommand, "--clear-cache", "-cC"));
            CommandRunner.AddCommand(Command.CreateCommand(AddEngineCommand, "--add-engine", "-a"));
            CommandRunner.AddCommand(Command.CreateCommand(DownloadEngineCommand, "--download-engine", "-d"));

            CommandRunner.AddCommand(def);


            CommandRunner.RunCommands(args);
            ReadLine();

        }

        private static void ConsoleOnCancelKeyPress(object sender, ConsoleCancelEventArgs e)
        {
            e.Cancel = true;
        }

        private static string GetRequiredEngineVersion(ZipArchive archive)
        {
            TextReader tr = new StreamReader(archive.GetEntry("EngineVersion").Open());
            string ver = tr.ReadToEnd().Replace("\r\n", "");
            tr.Close();
            return ver;
        }

        private static void CreateFolder(string path)
        {
            List<string> s = new List<string>();
            string curpath = Path.GetDirectoryName(path);
            do
            {
                s.Add(curpath);
                curpath = Path.GetDirectoryName(curpath);
            } while (curpath != "");

            s.Reverse();
            for (int i = 0; i < s.Count; i++)
            {
                if (!Directory.Exists(s[i]))
                {
                    Directory.CreateDirectory(s[i]);
                }
            }
        }

        private static void CheckUpdates()
        {
            Console.WriteLine("Checking for Updates...");
            string s = wc.DownloadString(
                "http://213.109.162.193/apps/EngineArchives/newest.version");

            if (!engineversions.Contains(s))
            {
                Console.WriteLine("Newest Version is not on Disk...");
                engineversions.Add(s);
                DownloadEngineVersion(s);
            }


            Console.WriteLine("Newest Engine Version Installed.");
        }

        private static void DownloadEngineVersion(string version)
        {
            if (IsVersionURLCorrect(version))
            {
                Console.WriteLine("Downloading Version: " + version);
                wc.DownloadFile(new Uri("http://213.109.162.193/apps/EngineArchives/" + version + ".engine"),
                    "engine/" + version + ".engine");
            }
        }

        private static void WcOnDownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            Console.WriteLine("");
            Console.WriteLine("Success: " + !e.Cancelled);
        }

        private static int last = 0;

        private static void WcOnDownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            if (last < e.ProgressPercentage)
            {
                int d = e.ProgressPercentage - last;
                float m = Console.WindowWidth / 100f;
                int fd = (int)(m * d);
                for (int j = 0; j < fd; j++)
                {
                    Console.Write("#");
                }

                last = e.ProgressPercentage;
            }
        }


        private static void AddEngine(string path)
        {
            try
            {
                PackageManifest pm = Creator.ReadManifest(path);
                if (!engineversions.Contains(pm.Version))
                {
                    Console.WriteLine("Adding Engine: " + pm);
                    engineversions.Add(pm.Version);
                    File.Copy(path, "engine/" + pm.Version + ".engine");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Could not add Engine to Player.");
                Console.WriteLine(e);
                throw;
            }
        }

        private static bool IsEngineVersionAvailable(string version)
        {
            return File.Exists("engine/" + version + ".engine");
        }

        private static void LoadGame(string gamePath, PackageManifest pm)
        {
            //Load Game
            Creator.UnpackPackage(gamePath, "_game", pm.PackageVersion);
            string[] files = Directory.GetFiles("_game", "*", SearchOption.AllDirectories);
            foreach (string file in files)
            {
                bool over = File.Exists(file.Replace("_game", "game"));

                CreateFolder(file.Replace("_game", "game"));

                File.Copy(file, file.Replace("_game", "game"), over);
            }

            Directory.Delete("_game", true);
        }

        private static void LoadEngine(string version)
        {
            if (version == "standalone")
            {
                Console.WriteLine("Engine is Contained in Game Package. Using engine from there.");
                return;
            }
            string filePath = version;
            if (version.StartsWith("path:") && File.Exists(version.Remove(0, 5)))
            {
                filePath = version.Remove(0, 5);
            }
            else if (!version.StartsWith("path:"))
            {
                if (!IsEngineVersionAvailable(version))
                {
                    if (IsVersionURLCorrect(version))
                    {
                        DownloadEngineVersion(version);
                    }
                    else
                    {
                        throw new ArgumentException("Could not locate engine version : " + version);
                    }
                }

                filePath = GetEnginePath(version);

            }




            Console.WriteLine("Loading Engine Version: " + version);
            ZipFile.ExtractToDirectory(filePath, "game");
        }

        private static bool IsVersionURLCorrect(string version)
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
            catch (WebException ex)
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