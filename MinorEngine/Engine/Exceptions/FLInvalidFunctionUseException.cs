using System;

namespace Engine.Exceptions
{
    /// <summary>
    /// This exception gets thrown when a FL instruction or kernel was used incorrectly in the program script.
    /// </summary>
    public class FLInvalidFunctionUseException : EngineException
    {
        public FLInvalidFunctionUseException(string function, string errorMessage, Exception inner) : base(
            "The function " + function + " is used incorrectly: \n" + errorMessage, inner)
        {
        }

        public FLInvalidFunctionUseException(string function, string errorMessage) : this(function, errorMessage, null)
        {
        }
    }
}