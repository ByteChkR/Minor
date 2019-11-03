using System.IO;
using System.IO.Compression;
using System.Reflection;

namespace Engine.AssetPackaging
{
    public class AssemblyFile
    {
        public readonly Assembly Assembly;

        public readonly string ManifestFilepath;


        public bool Compression;


        public AssemblyFile(bool compression , string manifestFilepath, Assembly assembly)
        {
            Compression = compression;
            ManifestFilepath = manifestFilepath;
            Assembly = assembly;
        }

        public virtual Stream GetFileStream()
        {
            using (Stream resourceStream = Assembly.GetManifestResourceStream(ManifestFilepath))
            {
                if (resourceStream == null)
                {
                    return null;
                }

                Stream rstream = Compression ? UncompressZip(resourceStream) : resourceStream;
                
                byte[] buf = new byte[rstream.Length];
                rstream.Read(buf, 0, (int)rstream.Length);

                MemoryStream ms = new MemoryStream(buf);
                rstream.Close();
                return ms;
            }
        }

        public static Stream UncompressZip(Stream inStream)
        {
            return new GZipStream(inStream, CompressionMode.Decompress);
            //ZipArchive za = new ZipArchive(inStream);
            //inStream.Close();
            //BinaryReader sr = new BinaryReader(za.GetEntry("data").Open());
            //byte[] buf = new byte[sr.BaseStream.Length];
            //sr.Read(buf, 0, buf.Length);
            //sr.Close();
            //MemoryStream ms = new MemoryStream(buf);

            //return ms;

        }
    }
}