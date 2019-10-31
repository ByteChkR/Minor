using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace AssetPackaging
{
    

    


    public static class AssetPacker
    {
        public const int KILOBYTE = 1024;
        public const int MAXSIZE_KILOBYTES = 1024;

        public static AssetResult PackAssets(string assetFolder, AssetPackageInfo info) // [...]/assets
        {
            AssetResult ret = new AssetResult();
            Uri assetPath = new Uri(assetFolder);
            foreach (KeyValuePair<string, AssetFileInfo> assetFileInfo in info.FileInfos)
            {
                string[] files = Directory.GetFiles(assetFolder, assetFileInfo.Key, SearchOption.AllDirectories);
                AssetPackageType type = assetFileInfo.Value.packageType;

                for (int i = 0; i < files.Length; i++)
                {
                    Uri file = new Uri(files[i]);
                    Uri packPath = assetPath.MakeRelativeUri(file);
                    ret.AddFile(files[i], packPath.ToString(), type);
                }

            }

            return ret;

        }

        public static Dictionary<string, Tuple<int, MemoryStream>> UnpackAssets(Stream indexList, Stream[] packs)
        {
            XmlSerializer xs = new XmlSerializer(typeof(AssetResult));
            AssetResult r = (AssetResult)xs.Deserialize(indexList);
            Dictionary<string, Tuple<int, MemoryStream>> assetList = new Dictionary<string, Tuple<int, MemoryStream>>();
            for (int i = 0; i < r.indexList.Count; i++)
            {
                if(r.indexList[i].PackageType== AssetPackageType.Memory)continue;
                
                MemoryStream ms = new MemoryStream(r.indexList[i].Length);
                
                byte[] buf = new byte[packs[r.indexList[i].PackageID].Length];
                packs[r.indexList[i].PackageID].Position = r.indexList[i].Offset;
                packs[r.indexList[i].PackageID].Read(buf, 0, buf.Length);
                
                ms.Write(buf, 0, buf.Length);
                assetList.Add(r.indexList[i].Path, new Tuple<int, MemoryStream>(r.indexList[i].Length, ms));
            }

            for (int i = 0; i < packs.Length; i++)
            {
                packs[i].Close();
            }
            return assetList;

        }

    }


}
