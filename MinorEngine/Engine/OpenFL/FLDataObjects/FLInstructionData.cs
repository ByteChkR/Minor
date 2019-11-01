using System.Collections.Generic;

namespace Engine.OpenFL.FLDataObjects
{
    public struct FLInstructionData
    {
        public object Instruction;
        public FLInstructionType InstructionType;
        public List<FLArgumentData> Arguments;
    }
}