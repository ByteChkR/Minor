using System.Collections.Generic;

namespace Engine.AssetPackaging
{
    public class AssetPackageInfo
    {
        public Dictionary<string, AssetFileInfo> FileInfos { get; set; } = new Dictionary<string, AssetFileInfo>();
    }
}