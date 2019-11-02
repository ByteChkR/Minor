using System;
using System.IO;
using System.Reflection.Emit;
using ADL;
using ADL.Configs;
using ADL.Crash;
using ADL.Network.Client;
using ADL.Network.Streams;
using ADL.Streams;
using MatchType = ADL.MatchType;

namespace Engine.Common
{
    public static class DebugHelper
    {
        public static int SeverityFilter = 0;
        public static bool ThrowOnAllExceptions = true;
        public static bool Init { get; private set; }

        public static void ApplySettings(IDebugSettings settings)
        {
            Init = true;
            Debug.RemoveAllOutputStreams();
            Debug.RemoveAllPrefixes();

            Debug.PrefixLookupMode = (PrefixLookupSettings) settings.PrefixLookupFlags;

            SeverityFilter = settings.SeverityFilter;

            Debug.AdlEnabled = settings.Enabled;
            Debug.AdlWarningMask = 0;
            Debug.CheckForUpdates = settings.SearchForUpdates;
            Debug.AdlWarningMask = -1;
            Debug.SendWarnings = settings.SendInternalWarnings;
            Debug.UpdateMask = -1;

            Debug.SetAllPrefixes(settings.StageNames);

            Debug.AddPrefixForMask(0, "[Silent]");

            foreach (ILogStreamSettings logStreamSettings in settings.Streams)
            {
                Debug.AddOutputStream(OpenStream(logStreamSettings));
            }


            CrashConfig cconf = new CrashConfig();
            cconf.CrashMask = -1;
            cconf.CheckForUpdates = settings.SearchForUpdates;
            CrashHandler.Initialize(cconf);
        }


        private static LogStream OpenFileStream(ILogStreamSettings settings)
        {
            if (File.Exists(settings.Destination))
            {
                File.Delete(settings.Destination);
            }

            return new LogTextStream(File.OpenWrite(settings.Destination), settings.Mask,
                (MatchType) settings.MatchMode, settings.Timestamp);
        }

        private static LogStream OpenConsoleStream(ILogStreamSettings settings)
        {
            return new LogTextStream(Console.OpenStandardOutput(), settings.Mask, (MatchType) settings.MatchMode,
                settings.Timestamp);
        }

        private static LogStream OpenNetworkStream(ILogStreamSettings settings)
        {
            NetLogStream nls = NetUtils.CreateNetworkStream(settings.NetworkAppID, settings.NetworkAuthVersion,
                settings.Destination, settings.NetworkPort, settings.Mask, (MatchType) settings.MatchMode,
                settings.Timestamp);
            return nls;
        }

        private static LogStream OpenStream(ILogStreamSettings settings)
        {
            if (settings.StreamType == 2) //Network
            {
                return OpenNetworkStream(settings);
            }

            if (settings.StreamType == 1) //File
            {
                return OpenFileStream(settings);
            }

            return OpenConsoleStream(settings);
        }


        public static void Crash(Exception ex, bool recoverable)
        {
            CrashHandler.Log(ex, 0);

            if (!recoverable || ThrowOnAllExceptions)
            {
                throw ex;
            }
        }

        public static void Log(string message, int channel)
        {
            Log(message, channel, 0);
        }

        public static void Log(string message, int channel, int severity)
        {
            if (severity < SeverityFilter)
            {
                return;
            }

            Debug.Log(channel, $"[S:{severity}]\t" + message);
        }
    }
}