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
        private static int Stage = 1;
        public static bool ThrowOnAllExceptions = false;

        public static void ApplySettings(IDebugSettings settings)
        {
            Debug.RemoveAllOutputStreams();
            Debug.RemoveAllPrefixes();

            Debug.PrefixLookupMode = (PrefixLookupSettings) settings.PrefixLookupFlags;


            Debug.AdlEnabled = settings.Enabled;
            Debug.AdlWarningMask = 0;
            Debug.CheckForUpdates = settings.SearchForUpdates;
            Debug.AdlWarningMask = -1;
            Debug.SendWarnings = settings.SendInternalWarnings;
            Debug.UpdateMask = -1;

            Debug.SetAllPrefixes(settings.StageNames);

            Debug.AddPrefixForMask(0, "[NONE]");
            Debug.AddPrefixForMask(-1, "[EXCEPTION]");

            foreach (ILogStreamSettings logStreamSettings in settings.Streams)
            {
                Debug.AddOutputStream(OpenStream(logStreamSettings));
            }


            CrashConfig cconf = new CrashConfig();
            cconf.CrashMask = -1;
            cconf.CheckForUpdates = settings.SearchForUpdates;
            CrashHandler.Initialize(cconf);
        }

        public static void SetStage(int stage)
        {
            Stage = stage;
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

        public static void Log(string message, int channel, int severity = 0)
        {
            if (severity < SeverityFilter)
            {
                return;
            }

            switch (channel)
            {
                case 1:
                    message = "[Log " + severity + "]" + message;
                    break;
                case 2:
                    message = "[Warning " + severity + "]" + message;
                    break;
                case 4:
                    message = "[Error " + severity + "]" + message;
                    break;
            }

            Debug.Log(Stage, message);
        }
    }
}