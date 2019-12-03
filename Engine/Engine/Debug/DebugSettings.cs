using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using Engine.Common;

namespace Engine.Debug
{
    /// <summary>
    /// Debug Settings used to Configure The debugging system
    /// </summary>
    [Serializable]
    public class DebugSettings : IDebugSettings
    {
        [XmlElement(ElementName = "LogStreamConfig")]
        public LogStreamSettings[] LogStreamSettings;

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
        /// The mask that the DebugFramework uses to filter out less important logs
        /// </summary>
        public int SeverityFilter { get; set; }


        /// <summary>
        /// Flags to govern how the Prefixes for the messages get assembled and computed
        /// </summary>
        public int PrefixLookupFlags { get; set; }

        /// <summary>
        /// The Stage names of the Different Channels
        /// </summary>
        public string[] StageNames { get; set; }


        /// <summary>
        /// List of streams that will be added to the Debug System.
        /// </summary>
        [XmlIgnore]
        public ILogStreamSettings[] Streams
        {
            get => LogStreamSettings.Cast<ILogStreamSettings>().ToArray();
            set => LogStreamSettings = value.Cast<LogStreamSettings>().ToArray();
        }

        /// <summary>
        /// Funcction that returns the default settings for debug logging.
        /// </summary>
        public static DebugSettings GetDefault()
        {
            List<string> stageNames = new List<string>();
            for (int i = 0; i < 31; i++)
            {
                stageNames.Add("[" + Enum.GetName(typeof(DebugChannel), 1 << i) + "]");
            }

            return new DebugSettings
            {
                Enabled = true,
                SendInternalWarnings = true,
                SearchForUpdates = false,
                InternalUpdateMask = 0,
                SeverityFilter = 6,
                StageNames = stageNames.ToArray(),
                PrefixLookupFlags = 1 | 2 | 8,
                Streams = new[] {new LogStreamSettings {Mask = -1, Timestamp = true}}
                    .Cast<ILogStreamSettings>().ToArray()
            };
        }
    }
}