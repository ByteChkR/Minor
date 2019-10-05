using System;
using System.Linq;
using System.Text;
using Common;

namespace MinorEngine.debug
{
    public class DebugSettings : IDebugSettings
    {
        public bool Enabled { get; set; }
        public bool SendInternalWarnings { get; set; }
        public bool SearchForUpdates { get; set; }
        public int InternalUpdateMask { get; set; }
        public int PrefixLookupFlags { get; set; }
        public string[] StageNames { get; set; }
        public Encoding LogEncoding { get; set; }
        public ILogStreamSettings[] Streams { get; set; }

        public static DebugSettings Default => new DebugSettings
        {
            Enabled = true,
            SendInternalWarnings = true,
            SearchForUpdates = false,
            InternalUpdateMask = 0,
            LogEncoding = Encoding.Default,
            StageNames = Enum.GetNames(typeof(DebugStage)).Select(x => "[" + x + "]").ToArray(),
            PrefixLookupFlags = 1 | 2 | 8,
            Streams = new[] { new LogStreamSettings() { Mask = -1, PrefixMode = 1, Timestamp = true } }.Cast<ILogStreamSettings>().ToArray()
        };

    }

    //Noprefix = 0,
    //Addprefixifavailable = 1,
    //Deconstructmasktofind = 2,
    //Onlyoneprefix = 4,
    //Bakeprefixes = 8

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