using System;

namespace Common.Exceptions
{
    public class FL_InvalidEntyPoint:ApplicationException
    {
        public FL_InvalidEntyPoint(string errorMessage, Exception inner) : base(errorMessage, inner)
        {

        }

        public FL_InvalidEntyPoint(string errorMessage) : this(errorMessage, null)
        {

        }
    }
}