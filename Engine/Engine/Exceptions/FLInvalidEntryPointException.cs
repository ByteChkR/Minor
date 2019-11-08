using System;

namespace Engine.Exceptions
{
    /// <summary>
    /// This Exception occurs when the FLInterpreter is not able to find the Main: method
    /// </summary>
    public class FLInvalidEntryPointException : EngineException
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="errorMessage">The message why this exception occurred</param>
        /// <param name="inner">Inner exeption</param>
        public FLInvalidEntryPointException(string errorMessage, Exception inner) : base(errorMessage, inner)
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="errorMessage">The message why this exception occurred</param>
        public FLInvalidEntryPointException(string errorMessage) : base(errorMessage)
        {
        }
    }
}