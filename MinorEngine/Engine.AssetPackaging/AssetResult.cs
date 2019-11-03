using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Security.Principal;
using System.Xml.Serialization;

namespace Engine.AssetPackaging
{
    [Serializable]
    public class AssetResult
    {
        [XmlElement(ElementName = "AssetIndexList")]
        public List<AssetPointer> indexList = new List<AssetPointer>();

        [XmlElement(ElementName = "Compression")]
        public bool Compression = false;

        [XmlIgnore] public List<AssetPack> packs = new List<AssetPack>();

        public void AddFile(string file, string packPath, AssetPackageType type)
        {
            FileStream fs = new FileStream(file, FileMode.Open);


            int assetPack = FindAssetPackWithSpace((int) fs.Length);

            Console.WriteLine("Adding file: " + file + " To pack: " + (assetPack + 1));

            AssetPointer ap = new AssetPointer
            {
                PackageID = assetPack,
                Offset = packs[assetPack].content.Count,
                Length = (int) fs.Length,
                Path = packPath,
                PackageType = type
            };
            //assets/textures/texture.png

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
            if (!Directory.Exists(outputFolder))
            {
                Directory.CreateDirectory(outputFolder);
            }

            if (!Directory.Exists(outputFolder + "\\packs"))
            {
                Directory.CreateDirectory(outputFolder + "\\packs");
            }

            XmlSerializer xs = new XmlSerializer(typeof(AssetResult));
            FileStream fs = new FileStream(outputFolder + "\\packs\\index.xml", FileMode.Create);
            xs.Serialize(fs, this);
            fs.Close();
            for (int i = 0; i < packs.Count; i++)
            {
                string path = outputFolder + "\\packs\\" + i + ".pack";
                byte[] buf = packs[i].content.ToArray();
                if (Compression)
                {
                    MemoryStream ms = new MemoryStream();
                    GZipStream gzs = new GZipStream(ms, CompressionLevel.Optimal);
                    gzs.Write(buf, 0, buf.Length);
                    buf =  new byte[ms.Length];
                    ms.Position = 0;
                    ms.Read(buf, 0, buf.Length);
                    gzs.Close();
                    ms.Close();
                    
                }

                FileStream packstream = new FileStream(path, FileMode.Create);
                packstream.Write(buf, 0, buf.Length);
                packstream.Close();
            }
        }
    }
}