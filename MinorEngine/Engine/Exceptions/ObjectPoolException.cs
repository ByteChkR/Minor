using System;

namespace Engine.Exceptions
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