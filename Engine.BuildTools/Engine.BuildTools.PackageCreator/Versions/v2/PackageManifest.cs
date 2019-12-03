using System;
using System.Collections.Generic;

namespace Engine.BuildTools.PackageCreator.Versions.v2
{
    [Serializable]
    public struct HashEntry
    {
        public string File { get; set; }
        public string Hash { get; set; }
    }

    [Serializable]
    public class PackageManifest : IPackageManifest
    {
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

        public List<HashEntry> Hashes { get; set; }
        public string PackageVersion { get; set; } = "unnamed";
        public string Title { get; set; } = "unnamed";
        public string Version { get; set; }
        public string StartCommand { get; set; }

        public override string ToString()
        {
            return $"[{PackageVersion}|{Title}]{StartCommand}: {Version}";
        }
    }
}