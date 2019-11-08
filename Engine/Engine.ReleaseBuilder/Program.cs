using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;
using ReleaseBuilder;

namespace Engine.ReleaseBuilder
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main(string[] args)
        {
            if (args.Length != 0)
            {
                if (File.Exists(args[0]))
                {
                    XmlSerializer xs = new XmlSerializer(typeof(BuildSettings));
                    FileStream fs = new FileStream(args[0], FileMode.Open);
                    BuildSettings bs = (BuildSettings)xs.Deserialize(fs);
                    fs.Close();
                    Form1.RunConfig(bs);

                    if (bs.CreateGamePackage)
                    {
                        CreateGamePackage(bs);
                    }

                    return;
                }
            }



            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }


        //private static void CreateEnginePackage(BuildSettings bs)
        //{
        //    Directory.SetCurrentDirectory(bs.OutputFolder);
        //    Assembly asm = Assembly.LoadFile("/Engine.dll");
        //    string projName = Path.GetFileNameWithoutExtension(bs.Project);
        //    string[] files;
        //    if (File.Exists(bs.GamePackageFileList))
        //    {
        //        files = File.ReadAllLines(bs.GamePackageFileList);
        //        for (int i = 0; i < files.Length; i++)
        //        {
        //            files[i] = Path.GetFullPath(files[i]);
        //        }
        //    }
        //    else
        //    {
        //        Console.WriteLine("Warning. No Game Package File list. Using /asset /pack folder as well as projectname.dll");
        //        List<string> f = new List<string>();
        //        f.AddRange(Directory.GetFiles(bs.AssetFolder, "*", SearchOption.AllDirectories));
        //        f.AddRange(Directory.GetFiles(Path.GetDirectoryName(bs.AssetFolder) + "/packs", "*", SearchOption.AllDirectories));
        //        f.Add(".dll");
        //        f.Add(".runtimeconfig.json");
        //        f.Add(".deps.json");
        //        files = f.ToArray();
        //    }


        //    RunProcess("resources/PlayerCreator.exe", $"--game {projName}.game {bs.OutputFolder} {bs.OutputFolder}/tmpList {asm.GetName().Version}");

        //}

        private static void CreateGamePackage(BuildSettings bs)
        {
            Directory.SetCurrentDirectory(bs.OutputFolder);
            string projName = Path.GetFileNameWithoutExtension(bs.Project);
            string[] files;
            if (File.Exists(bs.GamePackageFileList))
            {
                files = File.ReadAllLines(bs.GamePackageFileList);
                for (int i = 0; i < files.Length; i++)
                {
                    files[i] = Path.GetFullPath(files[i]);
                }
            }
            else
            {
                Console.WriteLine("Warning. No Game Package File list. Using /asset /pack folder as well as projectname.dll");
                List<string> f = new List<string>();
                string packFolder = bs.OutputFolder + "/packs";
                string assetFolder = bs.OutputFolder + "/" + Path.GetFileName(bs.AssetFolder);
                if (Directory.Exists(assetFolder)) f.AddRange(Directory.GetFiles(assetFolder, "*", SearchOption.AllDirectories));
                if (Directory.Exists(packFolder)) f.AddRange(Directory.GetFiles(packFolder, "*", SearchOption.AllDirectories));
                f.Add(Path.GetFullPath(projName + ".dll"));
                f.Add(Path.GetFullPath(projName + ".runtimeconfig.json"));
                f.Add(Path.GetFullPath(projName + ".deps.json"));
                files = f.ToArray();
            }

            File.WriteAllLines(bs.OutputFolder + "/tmpList", files);

            FileVersionInfo fvi = FileVersionInfo.GetVersionInfo("Engine.dll");
            string outputFile = Path.GetFullPath($"{projName}.game");
            RunProcess("resources/PlayerCreator.exe", $"--game {outputFile} {bs.OutputFolder} {bs.OutputFolder}/tmpList {fvi.FileVersion}", null);
            File.Delete(bs.OutputFolder + "/tmpList");
        }



        private static Tuple<int, string[]> RunProcess(string file, string args, Action waitAction)
        {
            ProcessStartInfo psi = new ProcessStartInfo(file, args)
            {
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true,
                UseShellExecute = false
            };


            Process p = new Process { StartInfo = psi };

            p.Start();

            
            while (!p.HasExited)
            {
                waitAction?.Invoke();
            }

            return new Tuple<int, string[]>(p.ExitCode, p.StandardOutput.ReadToEnd().Replace("\r","").Split('\n'));
        }
    }
}