using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Reflection;
using System.Text;

namespace Engine.AssetPackaging
{

    /// <summary>
    /// Container Class used to Group Files by Assembly
    /// </summary>
    public class AssemblyFile
    {
        /// <summary>
        /// The Assembly containing all files in this object
        /// </summary>
        public readonly Assembly Assembly;

        /// <summary>
        /// All files in this object
        /// </summary>
        public readonly string[] ManifestFilepaths;

        /// <summary>
        /// If the assembly has compressed files
        /// </summary>
        public bool Compression { get; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="compression"></param>
        /// <param name="manifestFilepath"></param>
        /// <param name="assembly"></param>
        public AssemblyFile(bool compression, string manifestFilepath, Assembly assembly) : this(compression,
            new[] {manifestFilepath}, assembly)
        {
        }

        public AssemblyFile(bool compression, string[] manifestFilepaths, Assembly assembly)
        {
            Compression = compression;
            ManifestFilepaths = manifestFilepaths;
            Assembly = assembly;
        }


        public virtual Stream GetResourceStream(int index)
        {
            return Assembly.GetManifestResourceStream(ManifestFilepaths[index]);
        }

        public virtual Stream GetFileStream()
        {
            using (Stream resourceStream = GetResourceStream(0))
            {
                if (resourceStream == null)
                {
                    return null;
                }

                Stream rstream = Compression ? UncompressZip(resourceStream) : resourceStream;

                byte[] buf = new byte[rstream.Length];
                rstream.Read(buf, 0, (int) rstream.Length);

                MemoryStream ms = new MemoryStream(buf);
                rstream.Close();
                return ms;
            }
        }

        public static Stream UncompressZip(Stream inStream)
        {
            Stream s = new GZipStream(inStream, CompressionMode.Decompress);
            List<byte> ret = new List<byte>();
            byte[] chunk = new byte[1024];
            int read;
            do
            {
                read = s.Read(chunk, 0, 1024);
                if (read != 1024)
                {
                    byte[] rest = new byte[read];
                    Array.Copy(chunk, rest, read);
                    ret.AddRange(rest);
                }
                else
                {
                    ret.AddRange(chunk);
                }
            } while (read == 1024);

            return new MemoryStream(ret.ToArray());
        }

        protected Stream ReadSplittedFile(AssetPointer ptr)
        {
            List<byte> ret = new List<byte>();
            int bytesRead = ptr.Offset;
            int readEndPosition = ptr.Offset + ptr.Length;
            for (int i = 0; i < ManifestFilepaths.Length; i++)
            {
                Stream str = GetResourceStream(i);
                if (i == 0)
                {
                    int readLength = ptr.PackageSize - ptr.Offset;
                    str.Position = ptr.Offset;
                    byte[] rbuf = new byte[ptr.PackageSize - ptr.Offset];
                    str.Read(rbuf, 0, rbuf.Length);
                    ret.AddRange(rbuf);
                    bytesRead += readLength;
                }
                else
                {
                    int readLength = readEndPosition - bytesRead;
                    if (readLength > ptr.PackageSize)
                    {
                        readLength = ptr.PackageSize;
                    }

                    byte[] rbuf = new byte[readLength];
                    str.Read(rbuf, 0, readLength);
                    ret.AddRange(rbuf);
                    bytesRead += readLength;
                }

                str.Close();
            }

            string r = Encoding.UTF8.GetString(ret.ToArray());
            return new MemoryStream(ret.ToArray());
        }
    }
}