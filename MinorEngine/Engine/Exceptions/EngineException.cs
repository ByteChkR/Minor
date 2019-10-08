using System;

namespace Engine.Exceptions
{
    public class EngineException : ApplicationException
    {
        public EngineException(string errorMessage, Exception inner) : base(errorMessage, inner)
        {
        }

        public EngineException(string errorMessage) : this(errorMessage, null)
        {
        }
    }
}