using System.Collections.Generic;
using OpenCl.DotNetCore.Memory;

namespace FilterLanguage
{
    public class InterpreterState
    {
        public int line;
        public MemoryBuffer activeBuffer;
        public Stack<object> argumentStack;

        public InterpreterState(int line, MemoryBuffer activeBuffer, Stack<object> argumentStack)
        {
            this.line = line;
            this.activeBuffer = activeBuffer;
            this.argumentStack = argumentStack;
        }
    }
}