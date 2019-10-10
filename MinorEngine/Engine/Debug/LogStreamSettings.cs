using System;
using Common;

namespace Engine.Debug
{
    /// <summary>
    /// The Settings for a Log Stream
    /// </summary>
    public struct LogStreamSettings : ILogStreamSettings
    {
        /// <summary>
        /// 0 = Console Output
        /// 1 = File Output
        /// 2 = Network output
        /// </summary>
        public int StreamType { get; set; }

        /// <summary>
        /// empty if consoleouput
        /// Filepath if Fileoutuput
        /// IP of the server if Networkoutput
        /// </summary>
        public string Destination { get; set; }

        /// <summary>
        /// The mask used to filter the logs
        /// </summary>
        public int Mask { get; set; }

        /// <summary>
        /// Governs how the masks get compared
        /// 0 = Only Match exact flags
        /// 1 = Match flags
        /// </summary>
        public int MatchMode { get; set; }

        /// <summary>
        /// Flag that if enabled sill prepend a timestamp infront of the log message.
        /// </summary>
        public bool Timestamp { get; set; }

        //NetworkSpecific:
        /// <summary>
        /// Network Port
        /// is empy if not a network stream
        /// </summary>
        public int NetworkPort { get; set; }

        /// <summary>
        /// Network Authentication ID
        /// is empy if not a network stream
        /// </summary>
        public int NetworkAppID { get; set; }

        /// <summary>
        /// Assembly Version that is reported to the server.
        /// is empty if not a network stream
        /// </summary>
        public Version NetworkAuthVersion { get; set; }
    }
}