using System.Collections.Generic;

namespace Engine.AssetPackaging
{
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