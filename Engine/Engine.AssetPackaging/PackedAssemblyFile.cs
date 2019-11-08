﻿using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace Engine.AssetPackaging
{
    public class PackedAssemblyFile : AssemblyFile
    {
        private AssetPointer ptr;

        public PackedAssemblyFile(bool compression, string manifestFilepath, Assembly assembly, AssetPointer ptr) :
            base(compression, manifestFilepath,
                assembly)
        {
            this.ptr = ptr;
        }

        public PackedAssemblyFile(bool compression, string[] manifestFilepaths, Assembly assembly, AssetPointer ptr) :
            base(compression, manifestFilepaths,
                assembly)
        {
            this.ptr = ptr;
        }


        public override Stream GetFileStream()
        {
            if (ManifestFilepaths.Length > 1)
            {
                return ReadSplittedFile(ptr);
            }

            Stream s = GetResourceStream(0);
            s.Position = ptr.Offset;
            byte[] buf = new byte[ptr.Length];
            s.Read(buf, 0, ptr.Length);
            s.Close();
            return new MemoryStream(buf);
        }
    }
}