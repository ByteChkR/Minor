using System;

namespace MinorEngine.exceptions
{
    public class InvalidFLArgumentType : EngineException
    {
        public InvalidFLArgumentType(string varname, string expected, Exception inner) : base($"Argument {varname} has the wrong type or is Null. Expected: {expected}", inner)
        {
        }

        public InvalidFLArgumentType(string varname, string expected) : this(varname, expected, null)
        {
        }
    }
}