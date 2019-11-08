using System;

namespace Engine.BuildTools.Builder
{
    

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