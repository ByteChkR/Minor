using System;

namespace Engine.Exceptions
{
    public class FLInvalidArgumentType : EngineException
    {
        public FLInvalidArgumentType(string varname, string expected, Exception inner) : base(
            $"Argument {varname} has the wrong type or is Null. Expected: {expected}", inner)
        {
        }

        public FLInvalidArgumentType(string varname, string expected) : this(varname, expected, null)
        {
        }
    }
}