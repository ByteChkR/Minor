using System;

namespace Engine.Exceptions
{

    /// <summary>
    /// This exception gets thrown when an Object Pool is full or has been initialized incorrectly.
    /// </summary>
    public class ObjectPoolException : EngineException
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="errorMessage">The message why this exception occurred</param>
        /// <param name="inner">Inner exeption</param>
        public ObjectPoolException(string errorMessage, Exception inner) : base(errorMessage, inner)
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="errorMessage">The message why this exception occurred</param>
        /// <param name="inner">Inner exeption</param>
        public ObjectPoolException(string errorMessage) : base(errorMessage)
        {
        }
    }
}