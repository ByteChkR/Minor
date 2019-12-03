using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using Engine.BuildTools.PackageCreator.Versions;
using Engine.BuildTools.PackageCreator.Versions.Legacy;
using Engine.BuildTools.PackageCreator.Versions.v1;
using Engine.BuildTools.PackageCreator.Versions.v2;

namespace Engine.BuildTools.PackageCreator
{
    public static class Creator
    {
        public const string DefaultVersion = "v1";

        private static Dictionary<string, IPackageVersion> _packageVersions = new Dictionary<string, IPackageVersion>
        {
            {"legacy", new LegacyVersion()},
            {"v1", new Version1()},
            {"v2", new Version2()}
        };

        public static IPackageManifest ReadManifest(string path)
        {
            ZipArchive archive = ZipFile.OpenRead(path);
            foreach (KeyValuePair<string, IPackageVersion> packageVersion in _packageVersions)
            {
                if (packageVersion.Value.IsVersion(path))
                {
                    archive.Dispose();
                    return packageVersion.Value.GetPackageManifest(path);
                }
            }

            throw new IOException("The file is not a supported format.");
        }

        private static string GetPackageVersion(string path)
        {
            ZipArchive archive = ZipFile.OpenRead(path);
            foreach (KeyValuePair<string, IPackageVersion> packageVersion in _packageVersions)
            {
                if (packageVersion.Value.IsVersion(path))
                {
                    archive.Dispose();
                    return packageVersion.Key;
                }
            }

            return "unknown";
        }

        public static void WriteManifest(Stream s, IPackageManifest o)
        {
            _packageVersions[o.PackageVersion].WriteManifest(s, o);
        }

        public static void CreateGamePackage(string packageName, string executable, string outputFile,
            string workingDir, string[] files, string version, string packageVersion = DefaultVersion)
        {
            _packageVersions[packageVersion]
                .CreateGamePackage(packageName, executable, outputFile, workingDir, files, version);
        }

        public static void CreateEnginePackage(string outputFile, string workingDir, string[] files,
            string packageVersion = DefaultVersion)
        {
            _packageVersions[packageVersion].CreateEnginePackage(outputFile, workingDir, files);
        }

        public static void UnpackPackage(string file, string outPutDir)
        {
            _packageVersions[GetPackageVersion(file)].UnpackPackage(file, outPutDir);
        }

        public static string GetEngineVersion(string workingDir)
        {
            FileVersionInfo v = FileVersionInfo.GetVersionInfo(workingDir + "/Engine.dll");

            return v.FileVersion;
        }
    }
}