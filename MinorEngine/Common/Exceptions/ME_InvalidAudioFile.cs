using System;

namespace Common.Exceptions
{
    public class ME_InvalidAudioFile : ApplicationException
    {
        public ME_InvalidAudioFile(string errorMessage, Exception inner) : base(errorMessage, inner)
        {
        }

        public ME_InvalidAudioFile(string errorMessage) : this(errorMessage, null)
        {
        }
    }
}