using System.IO;

namespace Engine.AssetPackaging
{
    /// <summary>
    /// Assembly File Class that is used when the pack file is on disk
    /// </summary>
    public class IoPackedAssemblyFile : AssemblyFile
    {
        private AssetPointer _ptr;

        public IoPackedAssemblyFile(bool compressed, string packFilepath, AssetPointer ptr) : base(compressed,
            packFilepath, null)
        {
            this._ptr = ptr;
        }

        public IoPackedAssemblyFile(bool compressed, string[] packFilepaths, AssetPointer ptr) : base(compressed,
            packFilepaths, null)
        {
            this._ptr = ptr;
        }

        public override Stream GetResourceStream(int index)
        {
            return new FileStream(ManifestFilepaths[index], FileMode.Open);
        }

        public override Stream GetFileStream()
        {
            if (ManifestFilepaths.Length > 1)
            {
                return ReadSplittedFile(_ptr);
            }


            FileStream fs = new FileStream(ManifestFilepaths[0], FileMode.Open);
            Stream s = Compression ? UncompressZip(fs) : fs;
            s.Position = _ptr.Offset;
            byte[] buf = new byte[_ptr.Length];
            int r = s.Read(buf, 0, buf.Length);
            MemoryStream ms = new MemoryStream(buf);
            s.Close();
            return new MemoryStream(buf);
        }
    }
}