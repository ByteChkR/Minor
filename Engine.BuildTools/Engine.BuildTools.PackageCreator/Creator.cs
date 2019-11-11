using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Xml.Serialization;
using Engine.BuildTools.PackageCreator.Versions;
using Engine.BuildTools.PackageCreator.Versions.v1;

namespace Engine.BuildTools.PackageCreator
{
    public static class Creator
    {
        public const string DEFAULT_VERSION = "v1";
        private static Dictionary<string, IPackageVersion> _packageVersions = new Dictionary<string, IPackageVersion>
        {
            { "legacy", new Legacy() },
            { "v1", new Version1() }
        };
        public static PackageManifest ReadManifest(string path)
        {
            ZipArchive archive = ZipFile.OpenRead(path);
            foreach (KeyValuePair<string, IPackageVersion> packageVersion in _packageVersions)
            {
                for (int i = 0; i < archive.Entries.Count; i++)
                {
                    if (archive.Entries[i].FullName == packageVersion.Value.ManifestPath)
                    {
                        archive.Dispose();
                        return packageVersion.Value.GetPackageManifest(path);
                    }
                }
            }
            throw new IOException("The file is not a supported format.");
        }

        public static PackageManifest ReadManifest(Stream s)
        {
            XmlSerializer xs = new XmlSerializer(typeof(PackageManifest));
            return (PackageManifest)xs.Deserialize(s);
        }

        public static void WriteManifest(Stream s, object o)
        {
            XmlSerializer xs = new XmlSerializer(typeof(PackageManifest));
            xs.Serialize(s, o);
        }

        public static void CreateGamePackage(string packageName, string executable, string outputFile, string workingDir, string[] files, string version, string packageVersion = DEFAULT_VERSION)
        {
            _packageVersions[packageVersion].CreateGamePackage(packageName, executable, outputFile, workingDir, files, version);
        }

        public static void CreateEnginePackage(string outputFile, string workingDir, string[] files, string packageVersion = DEFAULT_VERSION)
        {
            _packageVersions[packageVersion].CreateEnginePackage( outputFile, workingDir, files);
        }

        public static void UnpackPackage(string file, string outPutDir, string packageVersion = DEFAULT_VERSION)
        {
            _packageVersions[packageVersion].UnpackPackage(file, outPutDir);
        }

        public static string GetEngineVersion(string workingDir)
        {
            FileVersionInfo v = FileVersionInfo.GetVersionInfo(workingDir + "/Engine.dll");

            return v.FileVersion;
        }
    }
}