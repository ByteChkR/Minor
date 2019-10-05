using System;

namespace MinorEngine.exceptions
{
    /// <summary>
    /// This Exception occurs when the FLInterpreter is not able to find the Main: method
    /// </summary>
    public class InvalidFLEntryPointException : EngineException
    {
        public InvalidFLEntryPointException(string errorMessage, Exception inner) : base(errorMessage, inner)
        {
        }

        public InvalidFLEntryPointException(string errorMessage) : base(errorMessage)
        {
        }
    }
}