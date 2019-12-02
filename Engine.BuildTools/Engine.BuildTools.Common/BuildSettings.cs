using System;

namespace Engine.BuildTools.Common
{
    [Serializable]
    public class BuildSettings
    {
        public string AssetFolder = "";
        public BuildType BuildFlags = BuildType.PackOnly;
        public bool CreateEnginePackage = false;
        public bool CreateGamePackage = false;
        public string EngineProject = "";
        public string GamePackageFileList = "";
        public string MemoryFiles = "";
        public string OutputFolder = "";
        public int PackSize = 1024;
        public string Project = "";
        public string UnpackFiles = "";
    }
}