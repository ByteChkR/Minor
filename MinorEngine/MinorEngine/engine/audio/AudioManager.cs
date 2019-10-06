﻿using System;
using System.Collections.Generic;
using MinorEngine.debug;
using MinorEngine.engine.audio.formats;
using MinorEngine.exceptions;
using OpenTK.Audio;
using OpenTK.Audio.OpenAL;

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
                default:
                    Logger.Crash(new InvalidAudioFileException("The specified sound format is not supported."), false);
                    return ALFormat.Mono8;
            }
        }


        private static bool TryGetFormatProvider(string filename, out IAudioFormat formatProvider)
        {
            foreach (var format in _formats)
            {
                if (filename.EndsWith(format.Key))
                {
                    formatProvider = (IAudioFormat) Activator.CreateInstance(format.Value);
                    return true;
                }
            }


            Logger.Crash(new InvalidAudioFileException("Could not open Format. Unsupported."), false);
            formatProvider = null;
            return false;
        }

        public static bool TryLoad(string filename, out AudioClip clip)
        {
            var ret = TryGetFormatProvider(filename, out var formatProvider);

            if (ret)
            {
                ret = formatProvider.TryLoadFile(filename, out var data, out var channel, out var bits,
                    out var bitRate);
                if (ret)
                {
                    clip = new AudioClip(bits, channel, bitRate, data);

                    return true;
                }

                Logger.Crash(new InvalidAudioFileException("Could not open File. Invalid File"), false);
                clip = null;
                return true;
            }

            clip = null;
            return false;
        }
    }
}