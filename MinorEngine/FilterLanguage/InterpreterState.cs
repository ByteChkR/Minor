using System.Collections.Generic;
using OpenCl.DotNetCore.Memory;

namespace FilterLanguage
{
    public class InterpreterState
    {
        public int Line { get; }
        public MemoryBuffer ActiveBuffer { get; }
        public Stack<object> ArgumentStack { get; }

        public InterpreterState(int line, MemoryBuffer activeBuffer, Stack<object> argumentStack)
        {
            this.Line = line;
            this.ActiveBuffer = activeBuffer;
            this.ArgumentStack = argumentStack;
        }
    }
}