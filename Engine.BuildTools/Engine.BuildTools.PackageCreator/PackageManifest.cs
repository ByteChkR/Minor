using System;

namespace Engine.BuildTools.PackageCreator
{
    [Serializable]
    public class PackageManifest
    {
        public string PackageVersion = "legacy";
        public string Title = "unnamed";
        public string Executable = "";
        public string Version = "";
        public PackageManifest() { }

        public PackageManifest(string title, string executable, string version, string packageVersion)
        {
            Title = title;
            Executable = executable;
            PackageVersion = packageVersion;
            Version = version;
        }

        public override string ToString()
        {
            return $"[{PackageVersion}|{Title}]{Executable}: {Version}";
        }
    }
}