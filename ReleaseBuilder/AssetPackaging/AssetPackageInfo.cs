using System.Collections.Generic;

namespace AssetPackaging
{
    public class AssetPackageInfo
    {
        //Key = Search Pattern | Value = FileMetaData
        public Dictionary<string, AssetFileInfo> FileInfos = new Dictionary<string, AssetFileInfo>();

    }
}