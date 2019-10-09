using System;
using Common;

namespace Engine.Debug
{
    public struct LogStreamSettings : ILogStreamSettings
    {
        public int StreamType { get; set; }
        public string Destination { get; set; }
        public int Mask { get; set; }
        public int PrefixMode { get; set; }
        public int MatchMode { get; set; }
        public bool Timestamp { get; set; }

        //NetworkSpecific:
        public int NetworkPort { get; set; }
        public int NetworkAppID { get; set; }
        public Version NetworkAuthVersion { get; set; }
    }
}