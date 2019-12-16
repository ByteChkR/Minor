using System.Collections.Generic;

namespace Engine.AssetPackaging
{
    /// <summary>
    /// Class containing a map of filepaths to their AssetFileInfo objects
    /// </summary>
    public class AssetPackageInfo
    {
        public Dictionary<string, AssetFileInfo> FileInfos { get; set; } = new Dictionary<string, AssetFileInfo>();
    }
}