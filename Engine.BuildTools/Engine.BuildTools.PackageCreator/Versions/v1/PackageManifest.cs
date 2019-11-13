using System;
using System.Xml.Serialization;
using Engine.BuildTools.PackageCreator;
using Engine.BuildTools.PackageCreator.Versions;

namespace Engine.BuildTools.PackageCreator.Versions.v1
{
    [Serializable]
    public class PackageManifest : IPackageManifest
    {
        public string PackageVersion { get; set; } = "unnamed";
        public string Title { get; set; } = "unnamed";
        public string Version { get; set; }
        public string Executable { get; set; }

        [XmlIgnore]
        public string StartCommand
        {
            get => $"dotnet {Executable}";
            set => throw new NotSupportedException("Changing the Start Command is a V2 Feature");
        }

        public PackageManifest()
        {
        }

        public PackageManifest(string title, string executable, string version)
        {
            Title = title;
            Executable = executable;
            PackageVersion = "v1";
            Version = version;
        }

        public override string ToString()
        {
            return $"[{PackageVersion}|{Title}]{Executable}: {Version}";
        }
    }
}