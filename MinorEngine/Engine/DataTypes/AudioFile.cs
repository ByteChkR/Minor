namespace Engine.DataTypes
{
    public class AudioFile
    {
        public int Buffer { get; }
        public int BufferSize { get; }

        internal AudioFile(int buffer, int bufferSize)
        {
            BufferSize = bufferSize;
            Buffer = buffer;
        }

    }
}