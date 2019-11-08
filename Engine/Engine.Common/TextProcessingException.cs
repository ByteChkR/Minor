using System;

namespace Engine.Common
{
    public class TextProcessingException : ApplicationException
    {
        public TextProcessingException(string errorMessage, ApplicationException inner) : base(errorMessage, inner)
        {
        }
    }
}