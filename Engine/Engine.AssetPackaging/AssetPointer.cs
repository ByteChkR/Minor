﻿using System;

namespace Engine.AssetPackaging
{
    [Serializable]
    public class AssetPointer
    {
        public int Length;
        public int Offset;
        public int PackageID;
        public int PackageSize;
        public AssetPackageType PackageType;
        public string Path;

        public static int GetPackageCount(int offset, int length, int packageSize)
        {
            if (offset + length <= packageSize)
            {
                return 1;
            }

            int leftBytes = length - (packageSize - offset);
            int ceil = (int) Math.Ceiling(1 + leftBytes / (float) packageSize);
            return ceil;
        }

        public override string ToString()
        {
            return $"Path: {Path}, PID: {PackageID}, Offset: {Offset}, Length: {Length}";
        }
    }
}