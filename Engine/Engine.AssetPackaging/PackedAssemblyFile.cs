﻿using System.IO;
using System.Reflection;

namespace Engine.AssetPackaging
{
    public class PackedAssemblyFile : AssemblyFile
    {
        private AssetPointer _ptr;

        public PackedAssemblyFile(bool compression, string manifestFilepath, Assembly assembly, AssetPointer ptr) :
            base(compression, manifestFilepath,
                assembly)
        {
            this._ptr = ptr;
        }

        public PackedAssemblyFile(bool compression, string[] manifestFilepaths, Assembly assembly, AssetPointer ptr) :
            base(compression, manifestFilepaths,
                assembly)
        {
            this._ptr = ptr;
        }


        public override Stream GetFileStream()
        {
            if (ManifestFilepaths.Length > 1)
            {
                return ReadSplittedFile(_ptr);
            }

            Stream s = GetResourceStream(0);
            s.Position = _ptr.Offset;
            byte[] buf = new byte[_ptr.Length];
            s.Read(buf, 0, _ptr.Length);
            s.Close();
            return new MemoryStream(buf);
        }
    }
}