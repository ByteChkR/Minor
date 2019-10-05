using System;

namespace MinorEngine.exceptions
{
    /// <summary>
    /// This exception gets thrown when a FL instruction or kernel was used incorrectly in the program script.
    /// </summary>
    public class InvalidFLFunctionUseException : EngineException
    {
        public InvalidFLFunctionUseException(string function, string errorMessage, Exception inner) : base(
            "The function " + function + " is used incorrectly: \n" + errorMessage, inner)
        {
        }

        public InvalidFLFunctionUseException(string function, string errorMessage) : this(function, errorMessage, null)
        {
        }
    }
}