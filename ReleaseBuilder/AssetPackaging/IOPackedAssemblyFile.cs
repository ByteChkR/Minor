using System.IO;

namespace AssetPackaging
{
    public class IOPackedAssemblyFile : AssemblyFile
    {

        private AssetPointer ptr;
        public IOPackedAssemblyFile(string packFilepath, AssetPointer ptr) : base(packFilepath, null)
        {
            this.ptr = ptr;
        }

        public override Stream GetFileStream()
        {

            FileStream s = new FileStream(ManifestFilepath, FileMode.Open);
            s.Position = ptr.Offset;
            byte[] buf = new byte[ptr.Length];
            s.Read(buf, 0, buf.Length);
            MemoryStream ms = new MemoryStream(buf);
            s.Close();
            return new MemoryStream(buf);


        }
    }
}