using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Xml.Serialization;

namespace Engine.BuildTools.PackageCreator.Versions.v1
{
    public class Version1 : IPackageVersion
    {
        public string ManifestPath => "PackageManifest.xml";
        public string PackageVersion => "v1";

        public void UnpackPackage(string file, string outPutDir)
        {
            ZipFile.ExtractToDirectory(file, outPutDir);
            File.Delete(outPutDir + "/" + ManifestPath);
        }

        public PackageManifest GetPackageManifest(string path)
        {
            ZipArchive pack = ZipFile.OpenRead(path);
            Stream s = pack.GetEntry(ManifestPath).Open();
            XmlSerializer xs = new XmlSerializer(typeof(PackageManifest));
            PackageManifest pm = (PackageManifest)xs.Deserialize(s);
            return pm;
        }

        public void CreateGamePackage(string packageName, string executable, string outputFile, string workingDir, string[] files, string version)
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
            Stream str = engVersion.Open();
            PackageManifest pm = new PackageManifest(packageName, executable, version, PackageVersion);
            Creator.WriteManifest(str, pm);
            str.Close();
            a.Dispose();
            fs.Close();
        }

        public void CreateEnginePackage(string outputFile, string workingDir, string[] files, string version = null)
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
            Stream str = engVersion.Open();
            PackageManifest pm = new PackageManifest("Engine", "", version, PackageVersion);
            Creator.WriteManifest(str, pm);
            str.Close();
            a.Dispose();
            fs.Close();
        }
    }
}