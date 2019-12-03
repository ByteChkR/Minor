using System;

namespace Engine.BuildTools.Common
{
    /// <summary>
    /// Contains all settings needed to build a csproj file with the game engine
    /// </summary>
    [Serializable]
    public class BuildSettings
    {
        public string AssetFolder { get; set; } = "";
        public BuildType BuildFlags { get; set; } = BuildType.PackOnly;
        public bool CreateEnginePackage { get; set; } = false;
        public bool CreateGamePackage { get; set; } = false;
        public string EngineProject { get; set; } = "";
        public string GamePackageFileList { get; set; } = "";
        public string MemoryFiles { get; set; } = "";
        public string OutputFolder { get; set; } = "";
        public int PackSize { get; set; } = 1024;
        public string Project { get; set; } = "";
        public string UnpackFiles { get; set; } = "";
    }
}