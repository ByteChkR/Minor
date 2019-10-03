using System;
using Common;

namespace FilterLanguage.debug
{
    internal static class Logger
    {
        /// <summary>
        /// A static extension to receive logs from every point in the code base.
        /// </summary>
        /// <param name="obj">The object sending a message</param>
        /// <param name="message">The message</param>
        /// <param name="channel">The Channel on where the message is sent(Can be multiple)</param>
        internal static void Log(this object obj, string message, DebugChannel channel)
        {
            DebugHelper.LogMessage(obj, message, (int)channel);
        }

        /// <summary>
        /// A static extension to throw exceptions at one place to have a better control what to throw and when to throw
        /// </summary>
        /// <param name="obj">The object throwing the exception</param>
        /// <param name="ex">The exception that led to the crash</param>
        internal static void Crash(this object obj, ApplicationException ex)
        {
            DebugHelper.LogCrash(obj, ex, (int)DebugChannel.Internal_Error);
        }
    }
}