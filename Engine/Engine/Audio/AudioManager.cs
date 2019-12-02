using OpenTK.Audio;
using OpenTK.Audio.OpenAL;

namespace Engine.Audio
{
    /// <summary>
    /// Static Class that contains the OpenAL Initialization code and some useful properties to query information to the OpenAL Api
    /// </summary>
    public static class AudioManager
    {
        /// <summary>
        /// Private field for the audio context
        /// </summary>
        private static AudioContext _context;

        /// <summary>
        /// Useful Property to check if something has gone wrong with OpenAL
        /// </summary>
        public static AlcError GetCurrentALcError => _context?.CurrentError ?? AlcError.InvalidContext;

        /// <summary>
        /// Initializes the OpenAL Audio Context
        /// </summary>
        public static void Initialize()
        {
            _context = new AudioContext();
        }
    }
}