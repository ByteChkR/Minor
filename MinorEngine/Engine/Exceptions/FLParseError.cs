using System;

namespace Engine.Exceptions
{

    /// <summary>
    /// This exception gets thrown when a FL instruction or kernel was not found or could not be resolved.
    /// </summary>
    public class FLParseError : EngineException
    {

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="varname">Code that was used wrongly</param>
        /// <param name="inner">Inner exeption</param>
        public FLParseError(string varname, Exception inner) : base("Can not resolve symbol: " + varname, inner)
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="varname">Code that was used wrongly</param>
        public FLParseError(string varname) : this(varname, null)
        {
        }
    }
}