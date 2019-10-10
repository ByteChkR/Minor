using System;
using System.Linq;
using System.Text;
using Common;

namespace Engine.Debug
{
    /// <summary>
    /// Debug Settings used to Configure The debugging system
    /// </summary>
    public class DebugSettings : IDebugSettings
    {
        /// <summary>
        /// Should we send logs in the first place?
        /// </summary>
        public bool Enabled { get; set; }

        /// <summary>
        /// Is the TextProcessor and Debug Framework allowed to send warnings themselves(when used incorrectly)
        /// </summary>
        public bool SendInternalWarnings { get; set; }

        /// <summary>
        /// Is the Debug Framework allowed to check if any updates are available on github. Default: False
        /// </summary>
        public bool SearchForUpdates { get; set; }

        /// <summary>
        /// The mask that the DebugFramework uses to inform the user about an update
        /// </summary>
        public int InternalUpdateMask { get; set; }

        /// <summary>
        /// Flags to govern how the Prefixes for the messages get assembled and computed
        /// </summary>
        public int PrefixLookupFlags { get; set; }

        /// <summary>
        /// The Stage names of the Different Channels
        /// </summary>
        public string[] StageNames { get; set; }

        /// <summary>
        /// The encoding used to encode logs
        /// </summary>
        public Encoding LogEncoding { get; set; }

        /// <summary>
        /// List of streams that will be added to the Debug System.
        /// </summary>
        public ILogStreamSettings[] Streams { get; set; }

        /// <summary>
        /// Property that returns the default settings for debug logging.
        /// </summary>
        public static DebugSettings Default => new DebugSettings
        {
            Enabled = true,
            SendInternalWarnings = true,
            SearchForUpdates = false,
            InternalUpdateMask = 0,
            LogEncoding = Encoding.Default,
            StageNames = Enum.GetNames(typeof(DebugStage)).Select(x => "[" + x + "]").ToArray(),
            PrefixLookupFlags = 1 | 2 | 8,
            Streams = new[] {new LogStreamSettings {Mask = -1, Timestamp = true}}
                .Cast<ILogStreamSettings>().ToArray()
        };
    }
}