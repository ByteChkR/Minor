using System;
using System.Collections.Generic;
using Engine.DataTypes;
using Engine.Debug;
using Engine.Exceptions;
using Engine.IO.audioformats;
using OpenTK.Audio.OpenAL;

namespace Engine.IO
{

    /// <summary>
    /// Static class responsible to load Audio Files from Disk
    /// </summary>
    public static class AudioLoader
    {
        /// <summary>
        /// List of supported audio formats
        /// </summary>
        private static Dictionary<string, Type> _formats = new Dictionary<string, Type>
        {
            {".wav", typeof(WAVLoader)}
        };

        /// <summary>
        /// Tries to load the specified file type provider
        /// </summary>
        /// <param name="filename">Input File</param>
        /// <param name="formatProvider">The provider loaded</param>
        /// <returns>True if the loading succeeded</returns>
        private static bool TryGetFormatProvider(string filename, out IAudioFormatLoader formatProvider)
        {
            foreach (var format in _formats)
            {
                if (filename.EndsWith(format.Key))
                {
                    formatProvider = (IAudioFormatLoader)Activator.CreateInstance(format.Value);
                    return true;
                }
            }


            Logger.Crash(new AudioFileInvalidException("Could not open Format. Unsupported."), false);
            formatProvider = null;
            return false;
        }

        /// <summary>
        /// Returns the AL Format by Channel and bits
        /// </summary>
        /// <param name="channels"></param>
        /// <param name="bits"></param>
        /// <returns></returns>
        private static ALFormat GetSoundFormat(int channels, int bits)
        {
            switch (channels)
            {
                case 1: return bits == 8 ? ALFormat.Mono8 : ALFormat.Mono16;
                case 2: return bits == 8 ? ALFormat.Stereo8 : ALFormat.Stereo16;
                default:
                    Logger.Crash(new AudioFileInvalidException("The specified sound format is not supported."), false);
                    return ALFormat.Mono8;
            }
        }


        /// <summary>
        /// Tries to load the specified file and pass the loaded data through the out parameters
        /// </summary>
        /// <param name="filename">Input File</param>
        /// <param name="clip">Data that has been loaded</param>
        public static bool TryLoad(string filename, out AudioFile clip)
        {
            var ret = TryGetFormatProvider(filename, out var formatProvider);

            if (ret)
            {
                ret = formatProvider.TryLoadFile(filename, out var data, out var channel, out var bits,
                    out var bitRate);
                if (ret)
                {

                    int buf = AL.GenBuffer();

                    AL.BufferData(buf, GetSoundFormat(channel, bits), data, data.Length, bitRate);


                    AL.GetBuffer(buf, ALGetBufferi.Size, out var bufSize);
                    
                    clip = new AudioFile(buf, bufSize);

                    return true;
                }

                Logger.Crash(new AudioFileInvalidException("Could not open File. Invalid File"), false);
                clip = null;
                return true;
            }

            clip = null;
            return false;
        }

    }
}