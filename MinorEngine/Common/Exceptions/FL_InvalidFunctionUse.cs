using System;

namespace Common.Exceptions
{
    public class FL_InvalidFunctionUse : ApplicationException
    {
        public FL_InvalidFunctionUse(string function, string errorMessage, Exception inner = null) : base("The function " + function + " is used incorrectly: \n" + errorMessage, inner)
        {

        }
    }
}