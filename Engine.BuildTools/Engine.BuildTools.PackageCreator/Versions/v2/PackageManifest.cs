using System;
using System.Collections.Generic;
using Engine.BuildTools.PackageCreator;

namespace Engine.BuildTools.PackageCreator.Versions.v2
{
    [Serializable]
    public struct HashEntry
    {
        public string File;
        public string Hash;
    }

    [Serializable]
    public class PackageManifest : IPackageManifest
    {
        public string PackageVersion { get; set; } = "unnamed";
        public string Title { get; set; } = "unnamed";
        public string Version { get; set; }
        public string StartCommand { get; set; }
        public List<HashEntry> Hashes { get; set; }

        public PackageManifest()
        {
        }

        public PackageManifest(string title, string startCommand, string version, List<HashEntry> entries)
        {
            Title = title;
            StartCommand = startCommand;
            PackageVersion = "v2";
            Version = version;
            Hashes = entries;
        }

        public override string ToString()
        {
            return $"[{PackageVersion}|{Title}]{StartCommand}: {Version}";
        }
    }
}