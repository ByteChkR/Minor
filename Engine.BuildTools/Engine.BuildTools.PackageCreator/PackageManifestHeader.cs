using System;
using System.Xml.Serialization;

namespace Engine.BuildTools.PackageCreator
{
    /// <summary>
    /// Header of the Package Manifest
    /// Every PackageManifest can be XML Serialized in this object to inspect the packer version
    /// </summary>
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