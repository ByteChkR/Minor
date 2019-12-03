using System.Collections.Generic;

namespace Engine.OpenFL.FLDataObjects
{
    public struct FlInstructionData
    {
        public object Instruction;
        public FlInstructionType InstructionType;
        public List<FlArgumentData> Arguments;
    }
}