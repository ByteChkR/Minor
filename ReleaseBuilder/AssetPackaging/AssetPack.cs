﻿using System.Collections.Generic;

namespace AssetPackaging
{
    public class AssetPack
    {
        public List<byte> content = new List<byte>();
        public int SpaceLeft => AssetPacker.MAXSIZE_KILOBYTES * AssetPacker.KILOBYTE - content.Count;

    }
}