using System;

namespace Engine.Exceptions
{
    /// <summary>
    /// Exception that gets thrown when the Audio Loader was not able to load the file.
    /// </summary>
    public class AudioFileInvalidException : EngineException
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="errorMessage">The message why this exception occurred</param>
        /// <param name="inner">Inner exeption</param>
        public AudioFileInvalidException(string errorMessage, Exception inner) : base(errorMessage, inner)
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="errorMessage">The message why this exception occurred</param>
        public AudioFileInvalidException(string errorMessage) : base(errorMessage)
        {
        }
    }
}