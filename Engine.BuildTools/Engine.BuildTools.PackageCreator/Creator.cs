using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Reflection;

namespace Engine.BuildTools.PackageCreator
{
    public static class Creator
    {
        public static void CreateGamePackage(string outputFile, string workingDir, string[] files, string version)
        {
            File.WriteAllBytes(outputFile,
                new byte[] {80, 75, 5, 6, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0});
            FileStream fs = new FileStream(outputFile, FileMode.Open);
            ZipArchive a = new ZipArchive(fs, ZipArchiveMode.Update);
            Uri wdir = new Uri(workingDir);
            string wdirname = Path.GetFileName(workingDir);
            foreach (string file in files)
            {
                Uri f = new Uri(file);
                string fname = wdir.MakeRelativeUri(f).ToString().Remove(0, wdirname.Length + 1);
                ZipArchiveEntry e = a.CreateEntry(fname);

                byte[] content = File.ReadAllBytes(file);
                Stream s = e.Open();
                s.Write(content, 0, content.Length);
                s.Close();
            }

            ZipArchiveEntry engVersion = a.CreateEntry("EngineVersion");
            TextWriter tw = new StreamWriter(engVersion.Open());
            tw.WriteLine(version);
            tw.Close();
            a.Dispose();
            fs.Close();
        }

        public static void CreateEnginePackage(string outputFile, string workingDir, string[] files)
        {
            File.WriteAllBytes(outputFile,
                new byte[] {80, 75, 5, 6, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0});
            FileStream fs = new FileStream(outputFile, FileMode.Open);

            ZipArchive a = new ZipArchive(fs, ZipArchiveMode.Update);
            Uri wdir = new Uri(workingDir);
            string wdirname = Path.GetFileName(workingDir);
            string version = GetEngineVersion(workingDir);
            foreach (string file in files)
            {
                Uri f = new Uri(file);
                string fname = wdir.MakeRelativeUri(f).ToString().Remove(0, wdirname.Length + 1);
                ZipArchiveEntry e = a.CreateEntry(fname);

                byte[] content = File.ReadAllBytes(file);
                Stream s = e.Open();
                s.Write(content, 0, content.Length);
                s.Close();
            }

            ZipArchiveEntry engVersion = a.CreateEntry("EngineVersion");
            TextWriter tw = new StreamWriter(engVersion.Open());
            tw.WriteLine(version);
            tw.Close();
            a.Dispose();
            fs.Close();
        }

        private static string GetEngineVersion(string workingDir)
        {
            FileVersionInfo v = FileVersionInfo.GetVersionInfo(workingDir + "/Engine.dll");

            return v.FileVersion;
        }
    }
}