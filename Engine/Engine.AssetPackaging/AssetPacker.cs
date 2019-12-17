using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace Engine.AssetPackaging
{
    /// <summary>
    /// Main Class that is used to interface with the Asset Packaging Library
    /// </summary>
    public static class AssetPacker
    {
        public const int Kilobyte = 1024;
        public static int MaxsizeKilobytes = 1024;
        public static int PackSize => Kilobyte * MaxsizeKilobytes;

        public static AssetResult
            PackAssets(string assetFolder, string outputFolder, AssetPackageInfo info, bool compression = false) // [...]/assets
        {
            AssetResult ret = new AssetResult(outputFolder);
            ret.Compression = compression;
            Uri assetPath = new Uri(assetFolder);
            foreach (KeyValuePair<string, AssetFileInfo> assetFileInfo in info.FileInfos)
            {
                Console.WriteLine("Checking Folder: " + assetFolder + " with pattern: " + assetFileInfo.Key);
                string[] files = Directory.GetFiles(assetFolder, assetFileInfo.Key, SearchOption.AllDirectories);
                AssetPackageType type = assetFileInfo.Value.PackageType;

                for (int i = 0; i < files.Length; i++)
                {
                    Uri file = new Uri(files[i]);
                    Uri packPath = assetPath.MakeRelativeUri(file);

                    ret.AddFile(files[i], packPath.ToString(), type);
                }
            }

            return ret;
        }

        private static AssetResult ParseResult(Stream s)
        {
            XmlSerializer xs = new XmlSerializer(typeof(AssetResult));
            AssetResult ret = (AssetResult) xs.Deserialize(s);
            s.Close();
            return ret;
        }

        public static List<Tuple<string, AssetPointer>> GetPointers(Stream indexList, string[] packPaths,
            out bool compression)
        {
            AssetResult r = ParseResult(indexList);
            List<Tuple<string, AssetPointer>> assetList = new List<Tuple<string, AssetPointer>>();
            for (int i = 0; i < r.IndexList.Count; i++)
            {
                if (r.IndexList[i].PackageType == AssetPackageType.Unpack)
                {
                    Console.WriteLine("Skipping File Parsing");
                    continue;
                }

                assetList.Add(new Tuple<string, AssetPointer>(packPaths[GetId(packPaths, r.IndexList[i].PackageId)],
                    r.IndexList[i]));
            }

            compression = r.Compression;
            return assetList;
        }

        private static int GetId(string[] path, int id)
        {
            for (int i = 0; i < path.Length; i++)
            {
                if (Path.GetFileNameWithoutExtension(path[i]) == id.ToString())
                {
                    return i;
                }
            }

            return -1;
        }

        public static Dictionary<string, Tuple<int, MemoryStream>> UnpackAssets(Stream indexList, Stream[] packs)
        {
            AssetResult r = ParseResult(indexList);
            Dictionary<string, Tuple<int, MemoryStream>> assetList = new Dictionary<string, Tuple<int, MemoryStream>>();
            for (int i = 0; i < r.IndexList.Count; i++)
            {
                if (r.IndexList[i].PackageType == AssetPackageType.Memory)
                {
                    continue;
                }

                MemoryStream ms = new MemoryStream(r.IndexList[i].Length);

                byte[] buf = new byte[packs[r.IndexList[i].PackageId].Length];
                packs[r.IndexList[i].PackageId].Position = r.IndexList[i].Offset;
                packs[r.IndexList[i].PackageId].Read(buf, 0, buf.Length);

                ms.Write(buf, 0, buf.Length);
                ms.Position = 0;
                assetList.Add(r.IndexList[i].Path, new Tuple<int, MemoryStream>(r.IndexList[i].Length, ms));
            }

            for (int i = 0; i < packs.Length; i++)
            {
                packs[i].Close();
            }

            indexList.Close();
            return assetList;
        }
    }
}