using System;

namespace Engine.Exceptions
{
    public class NullPointerException : EngineException
    {
        public NullPointerException(string varname, Exception inner) : base(varname + " Was null.", inner)
        {
        }

        public NullPointerException(string varname) : this(varname, null)
        {
        }
    }
}