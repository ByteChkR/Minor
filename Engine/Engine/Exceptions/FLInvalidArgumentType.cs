using System;

namespace Engine.Exceptions
{
    public class FLInvalidArgumentType : EngineException
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="varname">Variable name that is affected</param>
        /// <param name="expected">The expected value for the variable</param>
        /// <param name="inner">Inner exeption</param>
        public FLInvalidArgumentType(string varname, string expected, Exception inner) : base(
            $"Argument {varname} has the wrong type or is Null. Expected: {expected}", inner)
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="varname">Variable name that is affected</param>
        /// <param name="expected">The expected value for the variable</param>
        public FLInvalidArgumentType(string varname, string expected) : this(varname, expected, null)
        {
        }
    }
}