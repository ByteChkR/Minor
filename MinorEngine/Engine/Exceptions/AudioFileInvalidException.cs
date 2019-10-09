using System;

namespace Engine.Exceptions
{
    public class AudioFileInvalidException : EngineException
    {
        public AudioFileInvalidException(string errorMessage, Exception inner) : base(errorMessage, inner)
        {
        }

        public AudioFileInvalidException(string errorMessage) : base(errorMessage)
        {
        }
    }
}