using System;
using System.Collections.Generic;
using OpenTK.Audio;
using OpenTK.Audio.OpenAL;

namespace Engine.Audio
{
    public static class AudioManager
    {
        private static AudioContext _context;

        public static void Initialize()
        {
            _context = new AudioContext();
        }


        public static AlcError GetCurrentALcError => _context?.CurrentError ?? AlcError.InvalidContext;

    }
}