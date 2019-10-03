using System;
using System.Collections.Generic;
using Common.exceptions;
using MinorEngine.engine.audio.formats;
using OpenTK.Audio;
using OpenTK.Audio.OpenAL;
using MinorEngine.debug;

namespace MinorEngine.engine.audio
{
    public static class AudioManager
    {
        private static AudioContext _context;

        public static void Initialize()
        {
            _context = new AudioContext();
        }


        public static AlcError GetCurrentALcError => _context?.CurrentError ?? AlcError.InvalidContext;


        private static Dictionary<string, Type> _formats = new Dictionary<string, Type>
        {
            {".wav", typeof(WAVAudioFormat)}
        };

        public static ALFormat GetSoundFormat(int channels, int bits)
        {
            switch (channels)
            {
                case 1: return bits == 8 ? ALFormat.Mono8 : ALFormat.Mono16;
                case 2: return bits == 8 ? ALFormat.Stereo8 : ALFormat.Stereo16;
                default: throw new NotSupportedException("The specified sound format is not supported.");
            }
        }


        private static bool TryGetFormatProvider(string filename, out IAudioFormat formatProvider)
        {
            foreach (KeyValuePair<string, Type> format in _formats)
                if (filename.EndsWith(format.Key))
                {
                    formatProvider = (IAudioFormat) Activator.CreateInstance(format.Value);
                    return true;
                }


            filename.Crash(new ME_InvalidAudioFile("Could not open Format. Unsupported."));
            formatProvider = null;
            return false;
        }

        public static bool TryLoad(string filename, out AudioClip clip)
        {
            bool ret = TryGetFormatProvider(filename, out IAudioFormat formatProvider);

            if (ret)
            {
                ret = formatProvider.TryLoadFile(filename, out byte[] data, out int channel, out int bits,
                    out int bitRate);
                if (ret)
                {
                    clip = new AudioClip(bits, channel, bitRate, data);

                    return true;
                }

                filename.Crash(new ME_InvalidAudioFile("Could not open File. Invalid File"));
                clip = null;
                return true;
            }

            clip = null;
            return false;
        }
    }
}