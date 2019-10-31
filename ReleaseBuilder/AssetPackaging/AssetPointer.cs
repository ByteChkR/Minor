using System;

namespace AssetPackaging
{
    [Serializable]
    public class AssetPointer
    {
        public string Path;
        public int PackageID;
        public int Offset;
        public int Length;
        public AssetPackageType PackageType;

        public override string ToString()
        {
            return $"Path: {Path}, PID: {PackageID}, Offset: {Offset}, Length: {Length}";
        }
    }
}