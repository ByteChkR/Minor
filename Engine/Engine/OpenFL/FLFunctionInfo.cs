namespace Engine.OpenFL
{
    /// <summary>
    /// Info object around a FLFunction Delegate
    /// </summary>
    public struct FlFunctionInfo
    {
        /// <summary>
        /// A delegate used for special functions in the interpreter
        /// </summary>
        public delegate void FlFunction();

        /// <summary>
        /// The FL Function
        /// </summary>
        public readonly FlFunction Function;


        /// <summary>
        /// Flag if the Stack should be left for the next operation
        /// </summary>
        public readonly bool LeaveStack;

        /// <summary>
        /// The Public Constructor
        /// </summary>
        /// <param name="flFunc">The FLFunction</param>
        /// <param name="leaveStack">Flag if the stack shouldbe left</param>
        public FlFunctionInfo(FlFunction flFunc, bool leaveStack)
        {
            Function = flFunc;
            LeaveStack = leaveStack;
        }


        /// <summary>
        /// Executes the FL Function
        /// </summary>
        public void Run()
        {
            Function?.Invoke();
        }
    }
}