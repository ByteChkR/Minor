using System;
using System.Xml;
using System.Xml.Serialization;

namespace Engine.BuildTools.PackageCreator
{
    [Serializable]
    [XmlRoot(ElementName = "PackageManifest")]
    public class PackageManifestHeader
    {
        
        public string PackageVersion { get; set; } = "legacy";

        public PackageManifestHeader() { }
        public PackageManifestHeader(string packageVersion)
        {
            PackageVersion = packageVersion;
        }
    }
}