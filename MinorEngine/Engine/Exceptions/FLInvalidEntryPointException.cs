using System;

namespace Engine.Exceptions
{
    /// <summary>
    /// This Exception occurs when the FLInterpreter is not able to find the Main: method
    /// </summary>
    public class FLInvalidEntryPointException : EngineException
    {
        public FLInvalidEntryPointException(string errorMessage, Exception inner) : base(errorMessage, inner)
        {
        }

        public FLInvalidEntryPointException(string errorMessage) : base(errorMessage)
        {
        }
    }
}