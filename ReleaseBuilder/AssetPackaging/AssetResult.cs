using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Principal;
using System.Xml.Serialization;

namespace AssetPackaging
{
    [Serializable]
    public class AssetResult
    {
        [XmlElement(ElementName = "AssetIndexList")]
        public List<AssetPointer> indexList = new List<AssetPointer>();
        [XmlIgnore]
        public List<AssetPack> packs = new List<AssetPack>();

        public void AddFile(string file, string packPath, AssetPackageType type)
        {
            FileStream fs = new FileStream(file, FileMode.Open);

            int assetPack = FindAssetPackWithSpace((int)fs.Length);

            Console.WriteLine("Adding file: " + file + " To pack: " + (assetPack + 1));

            AssetPointer ap = new AssetPointer();
            ap.PackageID = assetPack;
            ap.Offset = packs[assetPack].content.Count;
            ap.Length = (int)fs.Length;
            ap.Path = packPath; //assets/textures/texture.png
            ap.PackageType = type;

            indexList.Add(ap);


            byte[] buffer = new byte[fs.Length];
            fs.Read(buffer, 0, buffer.Length);
            packs[assetPack].content.AddRange(buffer);

            fs.Close();
        }

        private int FindAssetPackWithSpace(int space)
        {
            for (int i = 0; i < packs.Count; i++)
            {
                if (packs[i].SpaceLeft >= space)
                {
                    return i;
                }
            }
            packs.Add(new AssetPack());
            return packs.Count - 1;
        }

        public void Save(string outputFolder)
        {
            if (!Directory.Exists(outputFolder)) Directory.CreateDirectory(outputFolder);
            if (!Directory.Exists(outputFolder + "\\packs")) Directory.CreateDirectory(outputFolder + "\\packs");
            XmlSerializer xs = new XmlSerializer(typeof(AssetResult));
            FileStream fs = new FileStream(outputFolder + "\\packs\\index.xml", FileMode.Create);
            xs.Serialize(fs, this);
            fs.Close();
            for (int i = 0; i < packs.Count; i++)
            {
                string path = outputFolder + "\\packs\\" + i + ".pack";
                FileStream packstream = new FileStream(path, FileMode.Create);
                packstream.Write(packs[i].content.ToArray(), 0, packs[i].content.Count);
                packstream.Close();
            }
        }

    }
}