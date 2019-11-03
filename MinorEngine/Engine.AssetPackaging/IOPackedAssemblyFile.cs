using System.IO;

namespace Engine.AssetPackaging
{
    public class IOPackedAssemblyFile : AssemblyFile
    {
        private AssetPointer ptr;

        public IOPackedAssemblyFile(bool compressed, string packFilepath, AssetPointer ptr) : base(compressed, packFilepath, null)
        {
            this.ptr = ptr;
        }

        public override Stream GetFileStream()
        {
            FileStream fs = new FileStream(ManifestFilepath, FileMode.Open) { Position = ptr.Offset };
            Stream s = Compression ? UncompressZip(fs) : fs;
            byte[] buf = new byte[ptr.Length];
            s.Read(buf, 0, buf.Length);
            MemoryStream ms = new MemoryStream(buf);
            s.Close();
            return new MemoryStream(buf);
        }
    }
}