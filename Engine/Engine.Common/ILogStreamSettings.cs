using System;

namespace Engine.Common
{
    internal enum StreamType
    {
        Console,
        File,
        Network
    }

    public interface ILogStreamSettings
    {
        int StreamType { get; set; }
        string Destination { get; set; }
        int Mask { get; set; }
        int MatchMode { get; set; }
        bool Timestamp { get; set; }

        //NetworkSpecific:
        int NetworkPort { get; set; }
        int NetworkAppId { get; set; }
        Version NetworkAuthVersion { get; set; }
    }
}