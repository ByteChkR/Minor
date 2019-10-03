using System;

namespace Common.exceptions
{
    /// <summary>
    /// This Exception occurs when the FLInterpreter is not able to find the Main: method
    /// </summary>
    public class FL_InvalidEntyPoint : ApplicationException
    {
        public FL_InvalidEntyPoint(string errorMessage, Exception inner) : base(errorMessage, inner)
        {
        }

        public FL_InvalidEntyPoint(string errorMessage) : this(errorMessage, null)
        {
        }
    }
}