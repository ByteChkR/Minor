using System.Collections.Generic;

namespace Engine.OpenFL.FLDataObjects
{
    /// <summary>
    /// Contains information on a single FL Instruction
    /// </summary>
    public struct FlInstructionData
    {
        public object Instruction;
        public FlInstructionType InstructionType;
        public List<FlArgumentData> Arguments;
    }
}