using System.Collections.Generic;

namespace Engine.AssetPackaging
{
    /// <summary>
    /// A Container for the raw byte content of a package
    /// </summary>
    public class AssetPack
    {
        public List<byte> Content = new List<byte>();

        public AssetPack()
        {
            Content = new List<byte>();
        }

        public int SpaceLeft => AssetPacker.MaxsizeKilobytes * AssetPacker.Kilobyte - Content.Count;
    }
}