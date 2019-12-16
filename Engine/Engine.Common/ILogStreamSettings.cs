using System;

namespace Engine.Common
{
    /// <summary>
    /// The Stream Type of the Debug Log Stream
    /// </summary>
    internal enum StreamType
    {
        Console,
        File,
        Network
    }

    /// <summary>
    /// The Log Stream Settings as they are added to the Debug Log System
    /// </summary>
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