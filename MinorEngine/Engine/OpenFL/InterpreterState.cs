using System.Collections.Generic;
using Engine.DataTypes;
using Engine.OpenCL.DotNetCore.Memory;

namespace Engine.OpenFL
{
    /// <summary>
    /// The current interpreter state
    /// This struct is used to store the interpreter state when jumping to a function within the call of another function
    /// </summary>
    public class InterpreterState
    {
        /// <summary>
        /// The line where the interpreter was
        /// </summary>
        public int Line { get; }

        /// <summary>
        /// The active buffer
        /// </summary>
        public CLBufferInfo ActiveBuffer { get; }

        /// <summary>
        /// The unfinished argument stack
        /// </summary>
        public Stack<object> ArgumentStack { get; }

        /// <summary>
        /// Creates an interpreter state
        /// </summary>
        /// <param name="line">The line where the interpreter is</param>
        /// <param name="activeBuffer">The active buffer</param>
        /// <param name="argumentStack">The unfinished argument stack</param>
        internal InterpreterState(int line, CLBufferInfo activeBuffer, Stack<object> argumentStack)
        {
            Line = line;
            ActiveBuffer = activeBuffer;
            ArgumentStack = argumentStack;
        }
    }
}