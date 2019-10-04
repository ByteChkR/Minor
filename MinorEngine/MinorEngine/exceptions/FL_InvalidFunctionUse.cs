using System;

namespace MinorEngine.exceptions
{
    /// <summary>
    /// This exception gets thrown when a FL instruction or kernel was used incorrectly in the program script.
    /// </summary>
    public class FL_InvalidFunctionUse : ApplicationException
    {
        public FL_InvalidFunctionUse(string function, string errorMessage, Exception inner) : base(
            "The function " + function + " is used incorrectly: \n" + errorMessage, inner)
        {
        }

        public FL_InvalidFunctionUse(string function, string errorMessage) : this(function, errorMessage, null)
        {
        }
    }
}