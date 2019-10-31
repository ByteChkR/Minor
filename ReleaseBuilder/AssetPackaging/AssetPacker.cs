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

        public static void PackAssets(string[] files, string[] packFiles, string outputFolder)
        {
            AssetResult catalog = new AssetResult();

            for (int i = 0; i < files.Length; i++)
            {
                catalog.AddFile(files[i], packFiles[i]);
            }

            catalog.Save(outputFolder);


        }

        public static Dictionary<string, Tuple<int, MemoryStream>> UnpackAssets(Stream indexList, Stream[] packs)
        {
            Console.WriteLine("Unpacking File..");
            XmlSerializer xs = new XmlSerializer(typeof(AssetResult));
            AssetResult r = (AssetResult)xs.Deserialize(indexList);
            Dictionary<string, Tuple<int, MemoryStream>> assetList = new Dictionary<string, Tuple<int, MemoryStream>>();
            for (int i = 0; i < r.indexList.Count; i++)
            {
                MemoryStream ms = new MemoryStream(r.indexList[i].Length);

                Console.WriteLine("Unpacking File: "+ r.indexList[i]);
                byte[] buf = new byte[packs[r.indexList[i].PackageID].Length];
                packs[r.indexList[i].PackageID].Position = r.indexList[i].Offset;
                packs[r.indexList[i].PackageID].Read(buf, 0, buf.Length);

                Console.WriteLine("BufLength: " + buf.Length);
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
