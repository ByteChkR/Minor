using OpenTK.Audio.OpenAL;

namespace MinorEngine.engine.audio
{
    public class AudioClip
    {
        public int Buffer { get; }
        public int BufferSize { get; }

        public AudioClip(int bits, int channel, int bitRate, byte[] data)
        {
            Buffer = AL.GenBuffer();

            AL.BufferData(Buffer, AudioManager.GetSoundFormat(channel, bits), data, data.Length, bitRate);


            AL.GetBuffer(Buffer, ALGetBufferi.Size, out int bufSize);

            BufferSize = bufSize;
        }

        ~AudioClip()
        {
            AL.DeleteBuffer(Buffer);
        }
    }
}