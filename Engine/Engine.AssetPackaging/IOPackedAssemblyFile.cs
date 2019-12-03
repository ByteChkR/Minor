using System.IO;

namespace Engine.AssetPackaging
{
    public class IoPackedAssemblyFile : AssemblyFile
    {
        private AssetPointer ptr;

        public IoPackedAssemblyFile(bool compressed, string packFilepath, AssetPointer ptr) : base(compressed,
            packFilepath, null)
        {
            this.ptr = ptr;
        }

        public IoPackedAssemblyFile(bool compressed, string[] packFilepaths, AssetPointer ptr) : base(compressed,
            packFilepaths, null)
        {
            this.ptr = ptr;
        }

        public override Stream GetResourceStream(int index)
        {
            return new FileStream(ManifestFilepaths[index], FileMode.Open);
        }

        public override Stream GetFileStream()
        {
            if (ManifestFilepaths.Length > 1)
            {
                return ReadSplittedFile(ptr);
            }


            FileStream fs = new FileStream(ManifestFilepaths[0], FileMode.Open);
            Stream s = Compression ? UncompressZip(fs) : fs;
            s.Position = ptr.Offset;
            byte[] buf = new byte[ptr.Length];
            int r = s.Read(buf, 0, buf.Length);
            MemoryStream ms = new MemoryStream(buf);
            s.Close();
            return new MemoryStream(buf);
        }
    }
}