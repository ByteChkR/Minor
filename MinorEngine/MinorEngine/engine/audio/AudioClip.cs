using System;
using OpenTK.Audio.OpenAL;

namespace MinorEngine.engine.audio
{
    public class AudioClip
    {
        private readonly int Bits;
        private readonly int Channel;
        private readonly int BitRate;
        public int Buffer { get; }
        public int BufferSize { get; }

        public AudioClip(int bits, int channel, int bitRate, byte[] data)
        {
            this.Bits = bits;
            this.Channel = channel;
            this.BitRate = bitRate;


            
            Buffer = AL.GenBuffer();
            
            AL.BufferData(Buffer, AudioManager.GetSoundFormat(this.Channel, bits), data, data.Length, bitRate);
            
            
            AL.GetBuffer(Buffer, ALGetBufferi.Size, out int bufSize);

            BufferSize = bufSize;

        }

        ~AudioClip()
        {
            AL.DeleteBuffer(Buffer);
        }


        

    }
}