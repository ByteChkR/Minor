using System;
using System.Collections.Generic;
using System.Text;

namespace Engine.OpenFL
{
    /// <summary>
    /// A struct that gets returned every step of the interpreter. Mostly for debuggin purposes and when hitting a break point
    /// </summary>
    public struct InterpreterStepResult : IEquatable<InterpreterStepResult>
    {
        /// <summary>
        /// A flag that is set when the Interpreter jumped in the program code in the last step
        /// </summary>
        public bool HasJumped { get; set; }

        /// <summary>
        /// A flag that is set when the interpreter reached the end of the script
        /// </summary>
        public bool Terminated { get; set; }

        /// <summary>
        /// A flag that is set when the interpreter is currently at a break statement
        /// </summary>
        public bool TriggeredDebug { get; set; }

        /// <summary>
        /// The CPU Side verison of the active channels
        /// </summary>
        public byte[] ActiveChannels { get; set; }

        /// <summary>
        /// The list of Memory buffer that were defined. Matched by key
        /// </summary>
        public List<string> DefinedBuffers { get; set; }

        /// <summary>
        /// The list of Memory buffer that were defined. Matched by key
        /// </summary>
        public List<string> BuffersInJumpStack { get; set; }

        /// <summary>
        /// The Currently active buffer.
        /// </summary>
        public string DebugBufferName { get; set; }

        /// <summary>
        /// The Current line the interpreter is operating on
        /// </summary>
        public string SourceLine { get; set; }

        /// <summary>
        /// IEquatable Implementation
        /// </summary>
        /// <param name="other">Other</param>
        /// <returns>a Not implemented exception</returns>
        public bool Equals(InterpreterStepResult other)
        {
            return false;
        }

        /// <summary>
        /// String builder instance that is used when doing some more heaview string operations
        /// </summary>
        private static StringBuilder _sb = new StringBuilder();

        /// <summary>
        /// ToString Implementation
        /// </summary>
        /// <returns>Returns Current Processor State</returns>
        public override string ToString()
        {
            _sb.Clear();
            for (int i = 0; i < ActiveChannels.Length; i++)
            {
                _sb.Append(ActiveChannels[i]);
            }

            string channels = _sb.ToString();
            _sb.Clear();
            foreach (string definedBuffer in DefinedBuffers)
            {
                _sb.Append($"\n  {definedBuffer}");
            }

            string definedBuffers = _sb.ToString();

            _sb.Clear();
            foreach (string jumpBuffer in BuffersInJumpStack)
            {
                _sb.Append($"\n  {jumpBuffer}");
            }

            string jumpBuffers = _sb.ToString();
            return
                $"Debug Step Info:\n Active Buffer: {DebugBufferName}\n SourceLine:{SourceLine}\n HasJumped:{HasJumped}\n Triggered Breakpoint:{TriggeredDebug}\n Terminated:{Terminated}\n Active Channels:{channels}\n Defined Buffers:{definedBuffers}\n JumpStack:{jumpBuffers}";
        }
    }
}