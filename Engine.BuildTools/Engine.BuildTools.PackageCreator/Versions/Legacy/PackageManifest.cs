using System;
using System.Xml.Serialization;

namespace Engine.BuildTools.PackageCreator.Versions.Legacy
{
    [Serializable]
    public class PackageManifest : IPackageManifest
    {
        public PackageManifest()
        {
        }

        public PackageManifest(string projectName, string version)
        {
            Title = projectName;
            Executable = projectName + ".dll";
            PackageVersion = "legacy";
            Version = version;
        }

        public string Executable { get; set; }
        public string PackageVersion { get; set; } = "unnamed";
        public string Title { get; set; } = "unnamed";
        public string Version { get; set; }

        [XmlIgnore]
        public string StartCommand
        {
            get => $"dotnet {Executable}";
            set => throw new NotSupportedException("Changing the Start Command is a V2 Feature");
        }

        public override string ToString()
        {
            return $"[{PackageVersion}|{Title}]{Executable}: {Version}";
        }
    }
}