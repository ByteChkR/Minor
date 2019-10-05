using System;

namespace MinorEngine.exceptions
{
    public class ObjectPoolException : EngineException
    {
        public ObjectPoolException(string errorMessage, Exception inner) : base(errorMessage, inner)
        {
        }

        public ObjectPoolException(string errorMessage) : base(errorMessage)
        {
        }
    }
}