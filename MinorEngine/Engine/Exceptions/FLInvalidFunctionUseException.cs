using System;

namespace Engine.Exceptions
{
    /// <summary>
    /// This exception gets thrown when a FL instruction or kernel was used incorrectly in the program script.
    /// </summary>
    public class FLInvalidFunctionUseException : EngineException
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="function">Function that was used wrongly</param>
        /// <param name="errorMessage">The Error message that explains what went wrong</param>
        /// <param name="inner">Inner exeption</param>
        public FLInvalidFunctionUseException(string function, string errorMessage, Exception inner) : base(
            "The function " + function + " is used incorrectly: \n" + errorMessage, inner)
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="function">Function that was used wrongly</param>
        /// <param name="errorMessage">The Error message that explains what went wrong</param>
        public FLInvalidFunctionUseException(string function, string errorMessage) : this(function, errorMessage, null)
        {
        }
    }
}