using System.IO;
using System.Reflection;

namespace AssetPackaging
{
    public class PackedAssemblyFile : AssemblyFile
    {
        private AssetPointer ptr;
        public PackedAssemblyFile(string manifestFilepath, Assembly assembly, AssetPointer ptr) : base(manifestFilepath, assembly)
        {
            this.ptr = ptr;
        }


        public override Stream GetFileStream()
        {
            Stream s = base.GetFileStream();
            s.Position = ptr.Offset;
            byte[] buf = new byte[ptr.Length];
            s.Read(buf, 0, ptr.Length);
            s.Close();
            return new MemoryStream(buf);
        }
    }
}