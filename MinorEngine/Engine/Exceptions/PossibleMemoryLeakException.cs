using System;

namespace Engine.Exceptions
{

    /// <summary>
    /// Gets thrown if an Object is beeing garbage colllected but has still references in the active scene that would otherwise cause Memory Leaks
    /// </summary>
    public class PossibleMemoryLeakException : EngineException
    {

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="LeakType">The Type of Possible Memory leaks</param>
        /// <param name="inner">Inner exeption</param>
        public PossibleMemoryLeakException(string LeakType, Exception inner) : base("Possible Memory Leak: " + LeakType,
            inner)
        {
        }


        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="LeakType">The Type of Possible Memory leaks</param>
        public PossibleMemoryLeakException(string LeakType) : this(LeakType, null)
        {
        }
    }
}