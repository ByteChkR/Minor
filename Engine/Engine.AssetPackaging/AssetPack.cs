using System.Collections.Generic;

namespace Engine.AssetPackaging
{
    public class AssetPack
    {
        public List<byte> content = new List<byte>();

        public AssetPack()
        {
            content = new List<byte>();
        }

        public int SpaceLeft => AssetPacker.MAXSIZE_KILOBYTES * AssetPacker.KILOBYTE - content.Count;
    }
}