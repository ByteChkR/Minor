﻿using Engine.Common;
using Engine.Exceptions;

namespace Engine.Debug
{
    /// <summary>
    /// Logging Class that binds the ADL Debugging framework and is used for all Console Output/Crashes.
    /// </summary>
    public static class Logger
    {

        /// <summary>
        /// A static extension to receive logs from every point in the code base.
        /// </summary>
        /// <param name="message">The message</param>
        /// <param name="channel">The Channel on where the message is sent(Can be multiple)</param>
        public static void Log(string message, DebugChannel channel, int importance)
        {
            DebugHelper.Log(message, (int) channel, importance);
        }


        /// <summary>
        /// A static extension to throw exceptions at one place to have a better control what to throw and when to throw
        /// </summary>
        /// <param name="obj">The object throwing the exception</param>
        /// <param name="ex">The exception that led to the crash</param>
        public static void Crash(EngineException ex, bool recoverable)
        {
            DebugHelper.Crash(ex, recoverable);
        }
    }
}