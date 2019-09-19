using System;
using System.Collections.Generic;
using ADL;
using ADL.Configs;
using ADL.Streams;
using ADL.Crash;

namespace Common
{
    internal class CrashLogDictionary : SerializableDictionary<string, ApplicationException>
    {
        public CrashLogDictionary():this( new Dictionary<string, ApplicationException>()) { }
        public CrashLogDictionary(Dictionary<string, ApplicationException> dict) : base(dict) { }
    }

    public static class DebugHelper
    {
        private static bool _initialized;
        private static readonly CrashLogDictionary CrashLog = new CrashLogDictionary();
        private static readonly bool CrashOnException = true;
        private static LogTextStream _lts;

        public static DebugChannel ListeningMask
        {
            get
            {
                if (_lts == null) return DebugChannel.ALL;
                return (DebugChannel) _lts.Mask;
            }
            set
            {
                if (!_initialized)
                {
                    Initialize();
                }

                _lts.Mask = (int) value;
            }
        }


        private static void Initialize()
        {
            _initialized = true;
            Debug.AdlEnabled = true;
            Debug.CheckForUpdates = false;
            Debug.AdlWarningMask = 2;
            Debug.SendWarnings = true;
            Debug.PrefixLookupMode = PrefixLookupSettings.Addprefixifavailable | PrefixLookupSettings.Bakeprefixes |
                                     PrefixLookupSettings.Deconstructmasktofind;
            Debug.UpdateMask = 8;
            Debug.SetAllPrefixes("[Error]", "[Warning]", "[Log]", "[Internal Error]", "[Progress]");
            Debug.AddPrefixForMask(-1, "[ALL]");
            Debug.AddPrefixForMask(-1, "[NONE]");

            CrashConfig c = new CrashConfig();
            c.CrashMask = (int)DebugChannel.Internal_Error;
            c.CheckForUpdates = false;
            CrashHandler.Initialize(c);

            _lts = new LogTextStream(Console.OpenStandardOutput(), BitMask.WildCard, MatchType.MatchAll, true);
            Debug.AddOutputStream(_lts);
        }

        public static void Log(this object obj, string message, DebugChannel channel)
        {
            if(!_initialized)
            {
                Initialize();
            }
            
            Debug.LogGen(channel, message);
        }

        public static void Crash(this object obj, ApplicationException ex)
        {
            if (!_initialized)
            {
                Initialize();
            }
            CrashHandler.Log(ex, (int)DebugChannel.Internal_Error);

            if (CrashOnException)
            {
                throw ex;
            }
            CrashLog.Keys.Add(ADL.Utils.TimeStamp);
            CrashLog.Values.Add(ex);
        }


    }
}
