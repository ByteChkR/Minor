using System;

namespace Engine.BuildTools.Common
{
    [Serializable]
    public class BuildSettings
    {
        public bool CreateGamePackage = false;
        public bool CreateEnginePackage = false;
        public string GamePackageFileList = "";
        public string Project = "";
        public string EngineProject = "";
        public string AssetFolder = "";
        public string OutputFolder = "";
        public string MemoryFiles = "";
        public string UnpackFiles = "";
        public int PackSize = 1024;
        public BuildType BuildFlags = BuildType.PackOnly;
    }
}