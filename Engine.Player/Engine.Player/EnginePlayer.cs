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
using Engine.BuildTools.Common;
using Engine.BuildTools.PackageCreator;
using Engine.BuildTools.PackageCreator.Versions;
using Microsoft.Win32;

namespace Engine.Player
{
    internal class EnginePlayer
    {
        private const int MfBycommand = 0x00000000;
        public const int ScClose = 0xF060;
        private static List<string> _engineversions = new List<string>();
        private static Process _p;
        private static bool _dontReadLine;
        public static WebClient Wc = new WebClient();

        private static string _engineVersion;
        private static string _engineDir = AppDomain.CurrentDomain.BaseDirectory + "/engine";
        private static string _gameTempDir = AppDomain.CurrentDomain.BaseDirectory + "/_game";
        private static string _gameDir = AppDomain.CurrentDomain.BaseDirectory + "/game";


        private static int _last;

        private static Version[] AvailableVersionsOnServer;


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

        private static void AddToPathVariable()
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

        private static void UpdatePathVariable()
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

        private static bool IsInPathVariable()
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
        private static void RegisterExtensions()
        {
            RegisterExtension(".game");
            RegisterExtension(".engine");
        }

        private static void ReadLine()
        {
            if (!_dontReadLine)
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
            }
            else if (args[0].EndsWith(".engine"))
            {
                AddEngineCommand(info, args);
            }
            else if (args[0].EndsWith(".game"))
            {
                RunGameCommand(info, args);
            }
        }

        private static void SetUpDirectoryStructure()
        {
            if (Directory.Exists(_gameDir))
            {
                Directory.Delete(_gameDir, true);
            }

            if (Directory.Exists(_gameTempDir))
            {
                Directory.Delete(_gameTempDir, true);
            }

            DirectoryInfo di = Directory.CreateDirectory(_gameDir);
            DirectoryInfo tempUnpackForGame = Directory.CreateDirectory(_gameTempDir);
            if ((di.Attributes & FileAttributes.Hidden) != FileAttributes.Hidden)
            {
                //Add Hidden flag    
                di.Attributes |= FileAttributes.Hidden;
            }

            if ((tempUnpackForGame.Attributes & FileAttributes.Hidden) != FileAttributes.Hidden)
            {
                //Add Hidden flag    
                tempUnpackForGame.Attributes |= FileAttributes.Hidden;
            }
        }

        private static string GetEnginePath(string version)
        {
            return _engineDir + "/" + version + ".engine";
        }

        private static void RunGameCommand(StartupInfo info, string[] args)
        {
            if (args.Length == 0 || !args[0].EndsWith(".game") || !File.Exists(args[0]))
            {
                Console.WriteLine("Could not load Game File");
                return;
            }

            SetUpDirectoryStructure();
            

            IPackageManifest pm = null;
            try
            {
                pm = Creator.ReadManifest(args[0]);
                Console.Title = pm.Title;
                string path = pm.Version;
                if (_engineVersion != null)
                {
                    path = _engineVersion;
                }

                LoadEngine(path);
                //Load Game
                LoadGame(args[0], pm);
            }
            catch (Exception e)
            {
                Directory.Delete(_gameTempDir, true);
                Directory.Delete(_gameDir, true);
                Console.WriteLine("Error Unpacking File.");
                Console.WriteLine(e);
            }
            string startCommand = pm.StartCommand;
            if (Type.GetType("Mono.Runtime") == null) //Not Running Mono = Windows, Running Mono = Linux
            {
                Console.WriteLine("Remove Close Button.");
                DeleteMenu(GetSystemMenu(GetConsoleWindow(), false), ScClose, MfBycommand);
            }
            _p = new Process();

            ProcessStartInfo psi = new ProcessStartInfo();
            psi.FileName = "cmd.exe";
            psi.RedirectStandardError = true;
            psi.RedirectStandardOutput = true;
            psi.UseShellExecute = false;
            psi.CreateNoWindow = true;
            psi.Arguments = "/C " + startCommand;
            psi.WorkingDirectory = _gameDir;
            _p.StartInfo = psi;
            _p.Start();

            ConsoleRedirector crd =
                ConsoleRedirector.CreateRedirector(_p.StandardOutput, _p.StandardError, _p, Console.WriteLine);

            crd.StartThreads();

            while (!_p.HasExited)
            {
                Thread.Sleep(150);
            }

            crd.StopThreads();
            Console.WriteLine(crd.GetRemainingLogs());
            Directory.Delete(_gameDir, true);
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

        private static void _Update(StartupInfo info, string[] args)
        {
            WebClient wc = new WebClient();
            Console.WriteLine("Updating Build Tools...");
            string destination = Path.GetTempFileName() + ".exe";
            wc.DownloadFile(@"http://213.109.162.193/apps/Installer.exe", destination);
            Process.Start(destination, "--silent --pid " + Process.GetCurrentProcess().Id);
            wc.Dispose();
            Console.WriteLine("Update Downloaded. Update will be applied when application exits.");
        }

        private static void NoHaltCommand(StartupInfo info, string[] args)
        {
            _dontReadLine = true;
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

        private static void AddToPathVariable(StartupInfo info, string[] args)
        {
            try
            {
                if (IsInPathVariable()) UpdatePathVariable();
                else AddToPathVariable();
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
                _engineVersion = args[0];
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
                _engineVersion = "path:" + args[0];
            }
        }

        private static void RemoveEngineCommand(StartupInfo info, string[] args)
        {
            if (IsEngineVersionAvailable(args[0]))
            {
                Console.WriteLine("Deleting Version " + args[0]);
                File.Delete(_engineDir + "/" + args[0] + ".engine");
            }
            else
            {
                Console.WriteLine("Engine Version not available. Skipping Deletion.");
            }
        }

        private static void ClearCacheCommand(StartupInfo info, string[] args)
        {
            Console.WriteLine("Deleting Engine Cache...");
            if (Directory.Exists(_engineDir))
            {
                string[] files = Directory.GetFiles(_engineDir, "*", SearchOption.AllDirectories);
                for (int i = 0; i < files.Length; i++)
                {
                    File.Delete(files[i]);
                }
            }
        }

        private static void ListPackageInfo(StartupInfo info, string[] args)
        {
            if (args.Length == 0 || !File.Exists(args[0]) || !args[0].EndsWith(".game") && !args[0].EndsWith(".engine"))
            {
                Console.WriteLine("Could not find file");
                return;
            }

            IPackageManifest pm = Creator.ReadManifest(args[0]);
            Console.WriteLine(pm);
        }

        private static void Main(string[] args)
        {
            string callDir = Directory.GetCurrentDirectory();

            Wc.DownloadProgressChanged += WcOnDownloadProgressChanged;
            Wc.DownloadFileCompleted += WcOnDownloadFileCompleted;

            string homeDir = AppDomain.CurrentDomain.BaseDirectory;

            Console.CancelKeyPress += ConsoleOnCancelKeyPress;
            if (!Directory.Exists(_engineDir))
            {
                Directory.CreateDirectory(_engineDir);
            }

            GetEngineServerVersion();
            _engineversions = Directory.GetFiles(_engineDir, "*.engine", SearchOption.TopDirectoryOnly)
                .Select(x => Path.GetFileNameWithoutExtension(x)).ToList();
            

            Command def = Command.CreateCommand(DefaultCommand, "--run <Path/To/File.game>", "--run");
            CommandRunner.SetDefaultCommand(def);

            CommandRunner.AddCommand(Command.CreateCommand(_Update, "--update Updates the Build Tools", "--update"));
            CommandRunner.AddCommand(Command.CreateCommand(SetDefaultProgramCommand, "Requires Admin Permissions", "--set-default-program", "-sD"));
            CommandRunner.AddCommand(Command.CreateCommand(AddToPathVariable, "Requires Admin Permissions", "--add-to-path"));
            CommandRunner.AddCommand(Command.CreateCommand(NoHaltCommand, "Does not wait for user input before exiting", "--no-halt", "-nH"));
            CommandRunner.AddCommand(Command.CreateCommand(HelpCommand, "Display this help message", "--help", "-h"));
            CommandRunner.AddCommand(Command.CreateCommand(SetEnginePathCommand, "--engine-path <Path/To/File.game>\nSpecify a manual path to a .engine file", "--engine-path", "-eP"));
            CommandRunner.AddCommand(Command.CreateCommand(SetEngineVersionCommand, "--engine <Version>\nSpecify a manual version", "--engine", "-e"));
            CommandRunner.AddCommand(Command.CreateCommand(ListPackageInfo, "--list-info <<Path/To/File>\nLists Information about the .engine or .game file.", "--list-info", "-l"));

            CommandRunner.AddCommand(Command.CreateCommand(RemoveEngineCommand, "--remove-engine <Version>\nRemoves an engine Version from the engine cache", "--remove-engine", "-r"));
            CommandRunner.AddCommand(Command.CreateCommand(ClearCacheCommand, "--clear-cache\nClears all engines in the cache", "--clear-cache", "-cC"));
            CommandRunner.AddCommand(Command.CreateCommand(AddEngineCommand, "--add-engine <<Path/To/File.engine>\nAdds an engine file to the engine cache", "--add-engine", "-a"));
            CommandRunner.AddCommand(Command.CreateCommand(DownloadEngineCommand, "--download-engine <Version>\n Tries to download a specified engine version", "--download-engine", "-d"));

            CommandRunner.AddCommand(def);


            CommandRunner.RunCommands(args);
            ReadLine();
        }

        private static void ConsoleOnCancelKeyPress(object sender, ConsoleCancelEventArgs e)
        {
            e.Cancel = true;
        }

        private static void CreateFolder(string path)
        {
            List<string> s = new List<string>();
            string curpath = Path.GetDirectoryName(path);
            Console.WriteLine("Path: " + curpath);
            do
            {
                s.Add(curpath);
                curpath = Path.GetDirectoryName(curpath);
                Console.WriteLine("Adding: " + curpath);
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

        private static void GetEngineServerVersion()
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


        private static void DownloadEngineVersion(string version)
        {
            if (IsVersionUrlCorrect(version))
            {
                Console.WriteLine("Downloading Version: " + version);
                Wc.DownloadFile(new Uri("http://213.109.162.193/apps/EngineArchives/" + version + ".engine"),
                    _engineDir + "/" + version + ".engine");
            }
        }

        private static void WcOnDownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            Console.WriteLine("");
            Console.WriteLine("Success: " + !e.Cancelled);
        }

        private static void WcOnDownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            if (_last < e.ProgressPercentage)
            {
                int d = e.ProgressPercentage - _last;
                float m = Console.WindowWidth / 100f;
                int fd = (int)(m * d);
                for (int j = 0; j < fd; j++)
                {
                    Console.Write("#");
                }

                _last = e.ProgressPercentage;
            }
        }


        private static void AddEngine(string path)
        {
            try
            {
                IPackageManifest pm = Creator.ReadManifest(path);
                if (!_engineversions.Contains(pm.Version))
                {
                    Console.WriteLine("Adding Engine: " + pm);
                    _engineversions.Add(pm.Version);
                    File.Copy(path, _engineDir + "/" + pm.Version + ".engine");
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
            return File.Exists(_engineDir + "/" + version + ".engine");
        }

        private static void LoadGame(string gamePath, IPackageManifest pm)
        {
            //Load Game
            Creator.UnpackPackage(gamePath, _gameTempDir);
            string[] files = Directory.GetFiles(_gameTempDir, "*", SearchOption.AllDirectories);
            foreach (string file in files)
            {
                bool over = File.Exists(file.Replace(_gameTempDir, _gameDir));

                CreateFolder(file.Replace(_gameTempDir, _gameDir));

                File.Copy(file, file.Replace(_gameTempDir, _gameDir), over);
            }

            Directory.Delete(_gameTempDir, true);
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
                    if (IsVersionUrlCorrect(version))
                    {
                        DownloadEngineVersion(version);
                    }
                    else
                    {

                        Console.WriteLine("Could not locate engine version : " + version);
                        Console.WriteLine("Finding Compatible..");

                        if (!Version.TryParse(version, out Version v))
                            throw new ArgumentException("Could not parse engine version : " + version);

                        bool foundVersion = false;
                        Version vx = new Version(v.Major, v.Minor, v.Build, 0);
                        for (int i = 0; i < AvailableVersionsOnServer.Length; i++)
                        {
                            Version avx = new Version(AvailableVersionsOnServer[i].Major, AvailableVersionsOnServer[i].Minor, AvailableVersionsOnServer[i].Build, 0);
                            if (v != AvailableVersionsOnServer[i] && vx == avx)
                            {
                                DownloadEngineVersion(AvailableVersionsOnServer[i].ToString());
                                version = AvailableVersionsOnServer[i].ToString();
                                foundVersion = true;
                                break;
                            }
                        }
                        if (!foundVersion)
                            throw new ArgumentException("Could not locate engine version : " + v);
                    }
                }

                filePath = GetEnginePath(version);
            }


            Console.WriteLine("Loading Engine Version: " + version);

            Creator.UnpackPackage(filePath, _gameDir);
        }

        private static bool IsVersionUrlCorrect(string version)
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