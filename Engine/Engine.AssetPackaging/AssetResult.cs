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

        private string OutputFolder;

        public AssetResult()
        {

        }

        public AssetResult(string outputFolder)
        {
            OutputFolder = outputFolder;

            if (!Directory.Exists(OutputFolder))
            {
                Directory.CreateDirectory(OutputFolder);
            }

            if (!Directory.Exists(OutputFolder + "\\packs"))
            {
                Directory.CreateDirectory(OutputFolder + "\\packs");
            }
        }

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
                Offset = (int)Packs[firstPack].BytesWritten,
                PackageSize = AssetPacker.PackSize,
                Length = (int)s.Length,
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
                Packs[packid].Write(b, 0, b.Length);
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
                FileStream fs = new FileStream(OutputFolder + "/packs/" + Packs.Count + ".pack", FileMode.CreateNew);
                Packs.Add(new AssetPack(fs));
            }

            return Packs.Count - 1;
        }

        public void Save()
        {


            XmlSerializer xs = new XmlSerializer(typeof(AssetResult));
            FileStream fs = new FileStream(OutputFolder + "\\packs\\index.xml", FileMode.Create);
            xs.Serialize(fs, this);
            fs.Close();
            for (int i = 0; i < Packs.Count; i++)
            {
                //string path = outputFolder + "\\packs\\" + i + ".pack";
                Packs[i].Save();

                //byte[] buf = Packs[i].Content.ToArray();
                //if (Compression)
                //{
                //    MemoryStream ms = new MemoryStream();
                //    GZipStream gzs = new GZipStream(ms, CompressionLevel.Optimal);
                //    gzs.Write(buf, 0, buf.Length);
                //    buf = new byte[ms.Length];
                //    ms.Position = 0;
                //    ms.Read(buf, 0, buf.Length);
                //    gzs.Close();
                //    ms.Close();
                //}

                //FileStream packstream = new FileStream(path, FileMode.Create);
                //packstream.Write(buf, 0, buf.Length);
                //packstream.Close();
            }
        }
    }
}