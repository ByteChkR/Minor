using System;

namespace ReleaseBuilder
{
    public enum BuildType
    {
        PackOnly,
        PackEmbed,
        Embed
    }

    [Serializable]
    public class BuildSettings
    {
        public bool CreateGamePackage;
        public string GamePackageFileList;
        public bool CreateEnginePackage;

        public string Project;
        public string AssetFolder;
        public string OutputFolder;
        public string MemoryFiles;
        public string UnpackFiles;
        public int PackSize;
        public BuildType BuildFlags;
    }
}