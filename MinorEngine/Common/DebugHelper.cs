using System;
using System.Collections.Generic;
using ADL;
using ADL.Configs;
using ADL.Crash;
using ADL.Network.Client;
using ADL.Network.Streams;
using ADL.Streams;

namespace Common
{
    /// <summary>
    /// Internal Class to Log Crashes in a Dictionary
    /// </summary>
    internal class CrashLogDictionary : SerializableDictionary<string, ApplicationException>
    {
        public CrashLogDictionary() : this(new Dictionary<string, ApplicationException>())
        {
        }

        public CrashLogDictionary(Dictionary<string, ApplicationException> dict) : base(dict)
        {
        }
    }

    public struct DebugSettings
    {
        public bool Enabled;
        public bool SendInternalWarnings;
        public int InternalWarningMask;
        public int InternalErrorMask;
        public int InternalLogMask;
        public string[] MaskPrefixes;
        public string WildcardPrefix;
        public string NonePrefix;

        public static DebugSettings Default => new DebugSettings
        {
            Enabled = true,
            SendInternalWarnings = true,
            InternalWarningMask = -1,
            InternalErrorMask = -1,
            InternalLogMask = -1,
            MaskPrefixes = new string[0],
            WildcardPrefix = "[ALL]",
            NonePrefix = "[NONE]"
        };
    }

    public static class DebugHelper
    {
        public static void SetDebugLoggingInformation(int id, int mask, Version version)
        {
            _programID = id;
            _currentProgramVersion = version;
            _netLogMask = mask;
        }

        private static int _programID = -1;
        private static Version _currentProgramVersion;
        private static NetLogStream _netLogStream;
        private static int _netLogMask = -1;

        /// <summary>
        /// a field to test of the DebugHelper has been initialized yet
        /// </summary>
        private static bool _initialized;

        private static int _internalLogMask;
        private static int _internalErrorMask;

        /// <summary>
        /// The dictionary of crash logs
        /// </summary>
        private static readonly CrashLogDictionary CrashLog = new CrashLogDictionary();

        /// <summary>
        /// A flag that can change the behavior of the program when encountering an exception
        /// </summary>
        private static readonly bool CrashOnException = true;

        /// <summary>
        /// The log text stream that is used to send messages to the ADL Library
        /// </summary>
        private static LogTextStream _lts;


        private const string AdlNetworkConfigPath = "configs/adl_network_config.xml";

        /// <summary>
        /// The listening mask that is applied to the text output
        /// </summary>
        public static int ListeningMask
        {
            get
            {
                if (_lts == null) return -1;
                return _lts.Mask;
            }
            set
            {
                if (!_initialized)
                {
                    Initialize(DebugSettings.Default);
                    _initialized.Log("Initialized with Default Debug Settings");
                }

                _lts.Mask = value;
            }
        }


        private static bool AskForDebugLogSending()
        {
#if !TRAVIS_TEST //To stop make unit testing wait for user input
            Console.WriteLine("Allow Sending Debug Logs? [y/N]:");
            if (Console.ReadLine().ToLower() == "y") return true;
#endif

            return false;
        }

        /// <summary>
        /// Initializes ADL and the DebugHelper
        /// </summary>
        public static void Initialize(DebugSettings debugSettings)
        {
            if (_initialized)
            {
                return;
            }
            _initialized = true;
            _internalLogMask = debugSettings.InternalLogMask;
            _internalErrorMask = debugSettings.InternalErrorMask;
            Debug.AdlEnabled = debugSettings.Enabled;
            Debug.SendWarnings = debugSettings.SendInternalWarnings;
            Debug.AdlWarningMask = debugSettings.InternalWarningMask;
            Debug.SetAllPrefixes(debugSettings.MaskPrefixes);
            Debug.AddPrefixForMask(0, debugSettings.NonePrefix);
            Debug.AddPrefixForMask(-1, debugSettings.WildcardPrefix);
            Debug.PrefixLookupMode = PrefixLookupSettings.Addprefixifavailable | PrefixLookupSettings.Bakeprefixes |
                                     PrefixLookupSettings.Deconstructmasktofind;

            Debug.CheckForUpdates = false;
            Debug.UpdateMask = 0;

            CrashConfig c = new CrashConfig();
            c.CrashMask = (int)debugSettings.InternalErrorMask;
            c.CheckForUpdates = false;
            CrashHandler.Initialize(c);

            _lts = new LogTextStream(Console.OpenStandardOutput(), BitMask.WildCard, MatchType.MatchAll, true);
            Debug.AddOutputStream(_lts);

#if !TRAVIS_TEST
            if (_programID != -1 && AskForDebugLogSending())
            {
                NetworkConfig conf = NetworkConfig.Load(AdlNetworkConfigPath);
                _netLogStream = NetUtils.CreateNetworkStream(conf, _programID, _currentProgramVersion, _netLogMask,
                    MatchType.MatchAll, false);
                Debug.AddOutputStream(_netLogStream);
            }
#endif
        }

        internal static void Log(this object obj, string message)
        {
            LogMessage(obj, message, _internalLogMask);
        }

        /// <summary>
        /// A static extension to receive logs from every point in the code base.
        /// </summary>
        /// <param name="obj">The object sending a message</param>
        /// <param name="message">The message</param>
        /// <param name="channel">The Channel on where the message is sent(Can be multiple)</param>
        public static void LogMessage(object obj, string message, int channel)
        {
            if (!_initialized)
            {
                _initialized.Log("Initialized with Default Debug Settings");
            }

            Debug.Log(channel, message);
        }

        internal static void Crash(this object obj, ApplicationException ex)
        {
            LogCrash(obj, ex, _internalErrorMask);
        }


        /// <summary>
        /// A static extension to throw exceptions at one place to have a better control what to throw and when to throw
        /// </summary>
        /// <param name="obj">The object throwing the exception</param>
        /// <param name="ex">The exception that led to the crash</param>
        public static void LogCrash(object obj, ApplicationException ex, int noteMask)
        {
            if (!_initialized)
            {
                _initialized.Log("Initialized with Default Debug Settings");
            }
            CrashHandler.Log(ex, noteMask);

            if (CrashOnException) throw ex;
            CrashLog.Keys.Add(Utils.TimeStamp);
            CrashLog.Values.Add(ex);
        }
    }
}