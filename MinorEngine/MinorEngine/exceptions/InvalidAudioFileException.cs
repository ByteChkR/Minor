using System;

namespace MinorEngine.exceptions
{
    public class InvalidAudioFileException : EngineException
    {
        public InvalidAudioFileException(string errorMessage, Exception inner) : base(errorMessage, inner)
        {
        }

        public InvalidAudioFileException(string errorMessage) : base(errorMessage)
        {
        }
    }
}