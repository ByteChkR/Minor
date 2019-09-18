using System.Collections.Generic;
using OpenCl.DotNetCore.Memory;

namespace FilterLanguage
{
    public class InterpreterState
    {
        public int Line;
        public MemoryBuffer ActiveBuffer;
        public Stack<object> ArgumentStack;

        public InterpreterState(int line, MemoryBuffer activeBuffer, Stack<object> argumentStack)
        {
            this.Line = line;
            this.ActiveBuffer = activeBuffer;
            this.ArgumentStack = argumentStack;
        }
    }
}