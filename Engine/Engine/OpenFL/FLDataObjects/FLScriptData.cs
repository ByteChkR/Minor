using System.Collections.Generic;

namespace Engine.OpenFL.FLDataObjects
{
    public struct FlScriptData
    {
        public List<string> Source;
        public Dictionary<string, ClBufferInfo> Defines;
        public Dictionary<string, int> JumpLocations;
        public List<FlInstructionData> ParsedSource;

        public FlScriptData(List<string> source)
        {
            Source = source;
            Defines = new Dictionary<string, ClBufferInfo>();
            JumpLocations = new Dictionary<string, int>();
            ParsedSource = new List<FlInstructionData>();
        }
    }
}