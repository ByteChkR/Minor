using System;
using OpenTK.Audio.OpenAL;

namespace GameEngine.engine.audio
{
    public class AudioClip
    {
        public readonly int Bits;
        public readonly int Channel;
        public readonly int BitRate;
        public readonly int Buffer;
        public readonly int BufferSize;
        
        public AudioClip(int bits, int channel, int bitRate, byte[] data)
        {
            this.Bits = bits;
            this.Channel = channel;
            this.BitRate = bitRate;


            
            Buffer = AL.GenBuffer();
            
            AL.BufferData(Buffer, AudioManager.GetSoundFormat(this.Channel, bits), data, data.Length, bitRate);
            
            
            AL.GetBuffer(Buffer, ALGetBufferi.Size, out BufferSize);
            
            
        }

        ~AudioClip()
        {
            AL.DeleteBuffer(Buffer);
        }


        

    }
}