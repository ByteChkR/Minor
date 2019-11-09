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
        private static bool Run;

        private const int MF_BYCOMMAND = 0x00000000;
        public const int SC_CLOSE = 0xF060;

        [DllImport("user32.dll")]
        public static extern int DeleteMenu(IntPtr hMenu, int nPosition, int wFlags);

        [DllImport("user32.dll")]
        private static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);

        [DllImport("kernel32.dll", ExactSpelling = true)]
        private static extern IntPtr GetConsoleWindow();

        public static WebClient wc = new WebClient();

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


        private static void ParseArgs(string[] args)
        {
            bool del = false;
            bool up = false;
            if (args.Length == 0)
            {
                return;
            }

            Run = !args[0].StartsWith("-");

            for (int i = 0; i < args.Length; i++)
            {
                if (args[i] == "-rc")
                {
                    del = true;
                }
                else if (args[i] == "-u")
                {
                    up = true;
                }
                else if (args[i] == "-rl")
                {
                    DontReadLine = true;
                }
                else if (args[i] == "--setdefault")
                {
                    RegisterExtensions();
                }
                else if (args[i] == "-r" && args.Length > i + 1)
                {
                    if (IsEngineVersionAvailable(args[i + 1]))
                    {
                        Console.WriteLine("Deleting Version " + args[i + 1]);
                        File.Delete("engine/" + args[i + 1] + ".engine");
                    }
                    else
                    {
                        Console.WriteLine("Engine Version not available. Skipping Deletion.");
                    }
                }
                else if (args[i] == "-d" && args.Length > i + 1)
                {
                    if (!IsEngineVersionAvailable(args[i + 1]))
                    {
                        Console.WriteLine("Downloading Version " + args[i + 1]);
                        DownloadEngineVersion(args[i + 1]);
                    }
                    else
                    {
                        Console.WriteLine("Engine Version not available. Skipping Download.");
                    }
                }
            }

            if (del)
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

            if (up)
            {
                CheckUpdates();
            }
        }

        private static void Main(string[] args)
        {
            wc.DownloadProgressChanged += WcOnDownloadProgressChanged;
            wc.DownloadFileCompleted += WcOnDownloadFileCompleted;


            Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);

            Console.CancelKeyPress += ConsoleOnCancelKeyPress;
            if (!Directory.Exists("engine"))
            {
                Directory.CreateDirectory("engine");
            }

            engineversions = Directory.GetFiles("engine", "*.engine", SearchOption.TopDirectoryOnly).Select(x => Path.GetFileNameWithoutExtension(x)).ToList();
            ParseArgs(args);
            if (!Run)
            {
                if (!DontReadLine)
                {
                    Console.WriteLine("Press Enter to Exit...");
                    Console.ReadLine();
                }

                return;
            }

            if (args.Length == 0 || !File.Exists(args[0]))
            {
                Console.WriteLine("Drag a file onto the executable, or specify the path in the command line.");
                if (!DontReadLine)
                {
                    Console.WriteLine("Press Enter to Exit...");
                    Console.ReadLine();
                }

                return;
            }
            else if (args[0].EndsWith(".engine"))
            {
                AddEngine(args[0]);
                if (!DontReadLine)
                {
                    Console.WriteLine("Press Enter to Exit...");
                    Console.ReadLine();
                }

                return;
            }

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

            PackageManifest pm = new PackageManifest();
            try
            {
                pm = Creator.ReadManifest(args[0]);
                Console.Title = pm.Title;
                LoadEngine(pm.Version);
                Creator.UnpackPackage(args[0], "_game", pm.PackageVersion);
                string[] files = Directory.GetFiles("_game", "*", SearchOption.AllDirectories);
                foreach (string file in files)
                {
                    bool over = File.Exists(file.Replace("_game", "game"));

                    CreateFolder(file.Replace("_game", "game"));

                    File.Copy(file, file.Replace("_game", "game"), over);
                }

                Directory.Delete("_game", true);
            }
            catch (Exception e)
            {
                Directory.Delete("_game", true);
                Directory.Delete("game", true);
                Console.WriteLine("Error Unpacking File.");
                Console.WriteLine(e);
                if (!DontReadLine)
                {
                    Console.WriteLine("Press Enter to Exit...");
                    Console.ReadLine();
                }

                return;
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
            if (!DontReadLine)
            {
                Console.WriteLine("Press Enter to Exit...");
                Console.ReadLine();
            }
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

        private static void LoadEngine(string version)
        {
            if (version == "standalone")
            {
                Console.WriteLine("Engine is Contained in Game Package. Using engine from there.");
                return;
            }

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

            Console.WriteLine("Loading Engine Version: " + version);
            ZipFile.ExtractToDirectory("engine/" + version + ".engine", "game");
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