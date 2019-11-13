using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Security.Cryptography;
using System.Xml.Serialization;

namespace Engine.BuildTools.PackageCreator.Versions.v2
{
    public class Version2 : IPackageVersion
    {
        public string ManifestPath => "PackageManifest.xml";
        public string PackageVersion => "v2";

        private MD5 md5 = MD5.Create();

        public void UnpackPackage(string file, string outPutDir)
        {
            ZipFile.ExtractToDirectory(file, outPutDir);
            File.Delete(outPutDir + "/" + ManifestPath);

            if (!CheckHashes((PackageManifest) GetPackageManifest(file), outPutDir))
            {
                Console.WriteLine("Checksum verification failed!");
            }

        }

        private bool CheckHashes(PackageManifest pm, string outPutDir)
        {
            string[] files = Directory.GetFiles(outPutDir, "*", SearchOption.AllDirectories);
            Uri outDir = new Uri(Path.GetFullPath(outPutDir));
            bool isCorrect = true;
            for (int j = 0; j < files.Length; j++)
            {
                string f = (new Uri(Path.GetFullPath(files[j])).MakeRelativeUri(outDir)).ToString();
                for (int i = 0; i < pm.Hashes.Count; i++)
                {
                    if (f == pm.Hashes[i].File)
                    {
                        Stream s = File.OpenRead(files[i]);
                        isCorrect &= CompareHash(pm.Hashes[i].Hash, s);
                        s.Close();
                    }
                }
            }

            return isCorrect;
        }

        private string ComputeHash(Stream content)
        {
            return BitConverter.ToString(md5.ComputeHash(content)).Replace("-", "");
            return Convert.ToBase64String(md5.ComputeHash(content));
        }

        private string ComputeHash(byte[] content)
        {
            return BitConverter.ToString(md5.ComputeHash(content)).Replace("-", "");
            return Convert.ToBase64String(md5.ComputeHash(content));
        }

        private bool CompareHash(string should, byte[] content)
        {
            return should == ComputeHash(content);
        }

        private bool CompareHash(string should, Stream content)
        {
            return should == ComputeHash(content);
        }

        public bool IsVersion(string path)
        {
            TextReader tr = null;
            Stream s = null;
            string v = "";
            try
            {
                ZipArchive pack = ZipFile.OpenRead(path);
                s = pack.GetEntry(ManifestPath).Open();
                tr = new StreamReader(s);
                XmlSerializer xs = new XmlSerializer(typeof(PackageManifestHeader));
                PackageManifestHeader pm = (PackageManifestHeader) xs.Deserialize(tr);
                v = pm.PackageVersion;
            }
            catch (Exception)
            {
                tr?.Close();
                return false;
            }

            return v == PackageVersion;
        }
        public void WriteManifest(Stream s, IPackageManifest manifest)
        {
            XmlSerializer xs = new XmlSerializer(typeof(PackageManifest));
            xs.Serialize(s, manifest);
        }
        public IPackageManifest GetPackageManifest(string path)
        {
            ZipArchive pack = ZipFile.OpenRead(path);
            Stream s = pack.GetEntry(ManifestPath).Open();
            XmlSerializer xs = new XmlSerializer(typeof(PackageManifest));
            PackageManifest pm = (PackageManifest) xs.Deserialize(s);
            return pm;
        }

        public void CreateGamePackage(string packageName, string executable, string outputFile, string workingDir,
            string[] files, string version)
        {
            File.WriteAllBytes(outputFile,
                new byte[] {80, 75, 5, 6, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0});
            FileStream fs = new FileStream(outputFile, FileMode.Open);
            ZipArchive a = new ZipArchive(fs, ZipArchiveMode.Update);
            Uri wdir = new Uri(workingDir);
            string wdirname = Path.GetFileName(workingDir);
            List<HashEntry> entries = new List<HashEntry>();
            foreach (string file in files)
            {
                Uri f = new Uri(file);
                string fname = wdir.MakeRelativeUri(f).ToString().Remove(0, wdirname.Length + 1);
                ZipArchiveEntry e = a.CreateEntry(fname);

                byte[] content = File.ReadAllBytes(file);
                entries.Add(CreateEntry(fname, content));
                Stream s = e.Open();
                s.Write(content, 0, content.Length);
                s.Close();
            }

            PackageManifest pm = new PackageManifest(packageName, executable, version, entries);
            ZipArchiveEntry engVersion = a.CreateEntry(ManifestPath);
            Stream str = engVersion.Open();
            Creator.WriteManifest(str, pm);
            str.Close();
            a.Dispose();
            fs.Close();
        }

        private HashEntry CreateEntry(string name, byte[] content)
        {
            return new HashEntry {File = name, Hash = ComputeHash(content)};
        }

        public void CreateEnginePackage(string outputFile, string workingDir, string[] files, string version = null)
        {
            File.WriteAllBytes(outputFile,
                new byte[] {80, 75, 5, 6, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0});
            FileStream fs = new FileStream(outputFile, FileMode.Open);

            ZipArchive a = new ZipArchive(fs, ZipArchiveMode.Update);
            Uri wdir = new Uri(workingDir);
            string wdirname = Path.GetFileName(workingDir);
            if (string.IsNullOrEmpty(version))
            {
                version = Creator.GetEngineVersion(workingDir);
            }

            List<HashEntry> entries = new List<HashEntry>();
            PackageManifest pm = new PackageManifest("Engine", "", version, entries);
            foreach (string file in files)
            {
                Uri f = new Uri(file);
                string fname = wdir.MakeRelativeUri(f).ToString().Remove(0, wdirname.Length + 1);
                ZipArchiveEntry e = a.CreateEntry(fname);

                byte[] content = File.ReadAllBytes(file);
                entries.Add(CreateEntry(fname, content));
                Stream s = e.Open();
                s.Write(content, 0, content.Length);
                s.Close();
            }

            ZipArchiveEntry engVersion = a.CreateEntry(ManifestPath);
            Stream str = engVersion.Open();
            Creator.WriteManifest(str, pm);
            str.Close();
            a.Dispose();
            fs.Close();
        }
    }
}