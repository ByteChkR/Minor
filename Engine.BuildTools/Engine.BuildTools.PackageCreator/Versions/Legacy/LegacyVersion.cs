using System;
using System.IO;
using System.IO.Compression;
using System.Xml.Serialization;
using Engine.BuildTools.PackageCreator.Versions.Legacy;

namespace Engine.BuildTools.PackageCreator.Versions.Legacy
{
    public class LegacyVersion : IPackageVersion
    {
        public  string ManifestPath => "EngineVersion";

        public  string PackageVersion => "legacy";

        public void WriteManifest(Stream s, IPackageManifest manifest)
        {
            XmlSerializer xs = new XmlSerializer(typeof(PackageManifest));
            xs.Serialize(s, manifest);
        }

        public  bool IsVersion(string path)
        {
            TextReader tr = null;
            Stream s = null;
            try
            {
                ZipArchive pack = ZipFile.OpenRead(path);
                s = pack.GetEntry(ManifestPath).Open();
                tr = new StreamReader(s);

            }
            catch (Exception)
            {
                s?.Close();
                return false;
            }
            string v = tr.ReadLine();
            tr.Close();
            return true;
        }

        public  IPackageManifest GetPackageManifest(string path)
        {
            string name = "Engine";
            if (!path.EndsWith(".engine"))
            {
                name = Path.GetFileNameWithoutExtension(path);
            }
            ZipArchive pack = ZipFile.OpenRead(path);
            Stream s = pack.GetEntry(ManifestPath).Open();
            TextReader tr = new StreamReader(s);
            string v = tr.ReadLine();
            tr.Close();
            return new PackageManifest(name, v);
        }

        public  void UnpackPackage(string file, string outPutDir)
        {
            ZipFile.ExtractToDirectory(file, outPutDir);
            File.Delete(outPutDir + "/" + ManifestPath);
        }

        public  void CreateGamePackage(string packageName, string executable, string outputFile, string workingDir, string[] files, string version)
        {
            File.WriteAllBytes(outputFile,
                new byte[] { 80, 75, 5, 6, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 });
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

            ZipArchiveEntry engVersion = a.CreateEntry(ManifestPath);
            TextWriter tw = new StreamWriter(engVersion.Open());
            tw.WriteLine(version);
            tw.Close();
            a.Dispose();
            fs.Close();
        }

        public virtual void CreateEnginePackage(string outputFile, string workingDir, string[] files, string version = null)
        {
            File.WriteAllBytes(outputFile,
                new byte[] { 80, 75, 5, 6, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 });
            FileStream fs = new FileStream(outputFile, FileMode.Open);

            ZipArchive a = new ZipArchive(fs, ZipArchiveMode.Update);
            Uri wdir = new Uri(workingDir);
            string wdirname = Path.GetFileName(workingDir);
            if (string.IsNullOrEmpty(version))
            {
                version = Creator.GetEngineVersion(workingDir);
            }
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

            ZipArchiveEntry engVersion = a.CreateEntry(ManifestPath);
            TextWriter tw = new StreamWriter(engVersion.Open());
            tw.WriteLine(version);
            tw.Close();
            a.Dispose();
            fs.Close();
        }
    }
}