using Common;
using MinorEngine.exceptions;

namespace MinorEngine.debug
{
    public static class Logger
    {
        internal static void SetDebugStage(DebugStage stage)
        {
            DebugHelper.SetStage((int) stage);
        }

        /// <summary>
        /// A static extension to receive logs from every point in the code base.
        /// </summary>
        /// <param name="obj">The object sending a message</param>
        /// <param name="message">The message</param>
        /// <param name="channel">The Channel on where the message is sent(Can be multiple)</param>
        public static void Log(string message, DebugChannel channel, int importance = 0)
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