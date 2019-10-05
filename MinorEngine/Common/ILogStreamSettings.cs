using System;

namespace Common
{
    internal enum StreamType
    {
        Console,
        File,
        Network
    }

    public interface ILogStreamSettings
    {
        int StreamType{ get; set; }
        string Destination{ get; set; }
        int Mask{ get; set; }
        int PrefixMode{ get; set; }
        int MatchMode{ get; set; }
        bool Timestamp{ get; set; }

        //NetworkSpecific:
        int NetworkPort { get; set; }
        int NetworkAppID { get; set; }
        Version NetworkAuthVersion { get; set; }
    }
}