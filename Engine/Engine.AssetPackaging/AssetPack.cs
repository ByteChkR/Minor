using System.IO;

namespace Engine.AssetPackaging
{
    /// <summary>
    /// A Container for the raw byte content of a package
    /// </summary>
    public class AssetPack
    {
        private Stream Content;
        public int BytesWritten { get; private set; }
        public AssetPack(Stream s)
        {
            Content = s;
        }

        public void Save()

        {
            Content.Close();
        }

        public void Write(byte[] buf, int start, int count)
        {
            Content.Write(buf, start, count);
            BytesWritten += count;
        }

        public int SpaceLeft => AssetPacker.MaxsizeKilobytes * AssetPacker.Kilobyte - BytesWritten;
    }
}