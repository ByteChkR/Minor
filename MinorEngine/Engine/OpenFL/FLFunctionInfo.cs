using System.Collections.Generic;
using Engine.OpenCL;

namespace Engine.OpenFL
{
    public struct FLFunctionInfo
    {
        /// <summary>
        /// A delegate used for special functions in the interpreter
        /// </summary>
        public delegate void FlFunction();
        public readonly FlFunction Function;
        public readonly bool LeaveStack;
        public FLFunctionInfo(FlFunction flFunc, bool leaveStack)
        {
            Function = flFunc;
            LeaveStack = leaveStack;
        }

        public void Run()
        {
            Function?.Invoke();
        }
    }
}