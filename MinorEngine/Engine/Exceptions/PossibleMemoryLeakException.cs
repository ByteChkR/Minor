using System;

namespace Engine.Exceptions
{
    public class PossibleMemoryLeakException : EngineException
    {
        public PossibleMemoryLeakException(string LeakType, Exception inner) : base("Possible Memory Leak: " + LeakType,
            inner)
        {
        }

        public PossibleMemoryLeakException(string LeakType) : this(LeakType, null)
        {
        }
    }
}