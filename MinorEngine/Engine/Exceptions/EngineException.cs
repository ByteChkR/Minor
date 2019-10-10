using System;

namespace Engine.Exceptions
{
    /// <summary>
    /// Exception that the SuperClass of all exceptions that get thrown with logger.crash
    /// </summary>
    public class EngineException : ApplicationException
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="errorMessage">The message why this exception occurred</param>
        /// <param name="inner">Inner exeption</param>
        public EngineException(string errorMessage, Exception inner) : base(errorMessage, inner)
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="errorMessage">The message why this exception occurred</param>
        public EngineException(string errorMessage) : this(errorMessage, null)
        {
        }
    }
}