using System;
using System.Xml.Serialization;

namespace Engine.BuildTools.PackageCreator
{
    [Serializable]
    [XmlRoot(ElementName = "PackageManifest")]
    public class PackageManifestHeader
    {
        public PackageManifestHeader()
        {
        }

        public PackageManifestHeader(string packageVersion)
        {
            PackageVersion = packageVersion;
        }

        public string PackageVersion { get; set; } = "legacy";
    }
}