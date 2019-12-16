using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Xml.Serialization;

namespace Engine.AssetPackaging
{
    /// <summary>
    /// The Result Class for the Packing Process
    /// </summary>
    [Serializable]
    public class AssetResult
    {
        [XmlElement(ElementName = "AssetIndexList")]
        public List<AssetPointer> IndexList = new List<AssetPointer>();

        [XmlElement(ElementName = "Compression")]
        public bool Compression;


        [XmlIgnore] public List<AssetPack> Packs = new List<AssetPack>();

        public void AddFile(string file, string packPath, AssetPackageType type)
        {
            FileStream fs = new FileStream(file, FileMode.Open);
            StoreInPackages(fs, file, packPath, type);
        }

        private void StoreInPackages(Stream s, string file, string packPath, AssetPackageType type)
        {
            int firstPack = FindAssetPackWithSpace();


            Console.WriteLine("Adding file: " + file + " To pack: " + (firstPack + 1));

            AssetPointer ap = new AssetPointer
            {
                PackageId = firstPack,
                Offset = Packs[firstPack].Content.Count,
                PackageSize = AssetPacker.PackSize,
                Length = (int) s.Length,
                Path = packPath,
                PackageType = type
            };

            IndexList.Add(ap);


            int packid = firstPack;
            int bytesLeft = ap.Length;
            do
            {
                int write = bytesLeft;
                if (write > Packs[packid].SpaceLeft)
                {
                    write = Packs[packid].SpaceLeft;
                }

                byte[] b = new byte[write];
                s.Read(b, 0, write);
                Packs[packid].Content.AddRange(b);
                bytesLeft -= write;
                if (bytesLeft != 0)
                {
                    packid = FindAssetPackWithSpace();
                }
            } while (bytesLeft != 0);

            s.Close();
        }

        private int FindAssetPackWithSpace()
        {
            if (Packs.Count == 0 || Packs[Packs.Count - 1].SpaceLeft == 0)
            {
                Packs.Add(new AssetPack());
            }

            return Packs.Count - 1;
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
            for (int i = 0; i < Packs.Count; i++)
            {
                string path = outputFolder + "\\packs\\" + i + ".pack";
                byte[] buf = Packs[i].Content.ToArray();
                if (Compression)
                {
                    MemoryStream ms = new MemoryStream();
                    GZipStream gzs = new GZipStream(ms, CompressionLevel.Optimal);
                    gzs.Write(buf, 0, buf.Length);
                    buf = new byte[ms.Length];
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