using System;
using System.Collections.Generic;
using ADL;
using ADL.Configs;
using ADL.Streams;
using ADL.Crash;

namespace Common
{
    /// <summary>
    /// Internal Class to Log Crashes in a Dictionary
    /// </summary>
    internal class CrashLogDictionary : SerializableDictionary<string, ApplicationException>
    {
        public CrashLogDictionary():this( new Dictionary<string, ApplicationException>()) { }
        public CrashLogDictionary(Dictionary<string, ApplicationException> dict) : base(dict) { }
    }

    public static class DebugHelper
    {
        /// <summary>
        /// a field to test of the DebugHelper has been initialized yet
        /// </summary>
        private static bool _initialized;

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

        /// <summary>
        /// The listening mask that is applied to the text output
        /// </summary>
        public static DebugChannel ListeningMask
        {
            get
            {
                if (_lts == null)
                {
                    return DebugChannel.ALL;
                }
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

        /// <summary>
        /// Initializes ADL and the DebugHelper
        /// </summary>
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

        /// <summary>
        /// A static extension to receive logs from every point in the code base.
        /// </summary>
        /// <param name="obj">The object sending a message</param>
        /// <param name="message">The message</param>
        /// <param name="channel">The Channel on where the message is sent(Can be multiple)</param>
        public static void Log(this object obj, string message, DebugChannel channel)
        {
            if(!_initialized)
            {
                Initialize();
            }
            
            Debug.LogGen(channel, message);
        }

        /// <summary>
        /// A static extension to throw exceptions at one place to have a better control what to throw and when to throw
        /// </summary>
        /// <param name="obj">The object throwing the exception</param>
        /// <param name="ex">The exception that led to the crash</param>
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
