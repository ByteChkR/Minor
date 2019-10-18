using System;
using OpenTK.Audio.OpenAL;

namespace Engine.DataTypes
{
    /// <summary>
    /// A Data Type that is containing the information that is needed to reference a OpenAL Audio file.
    /// </summary>
    public class AudioFile : IDisposable
    {
        /// <summary>
        /// The OpenAL Buffer handle
        /// </summary>
        public int Buffer { get; }

        /// <summary>
        /// The buffer size in bytes
        /// </summary>
        public int BufferSize { get; }

        /// <summary>
        /// Internal Constructor to create a Audio file Data object.
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="bufferSize"></param>
        internal AudioFile(int buffer, int bufferSize)
        {
            BufferSize = bufferSize;
            Buffer = buffer;
        }


        /// <summary>
        /// Disposable implementation to free the Buffer once it is no longer needed.
        /// </summary>
        public void Dispose()
        {
            AL.DeleteBuffer(Buffer);
        }
    }
}