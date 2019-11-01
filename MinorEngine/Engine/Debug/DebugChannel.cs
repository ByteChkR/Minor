using System;

namespace Engine.Debug
{
    /// <summary>
    /// An enum of Channels that can be used to send logs
    /// </summary>
    [Flags]
    public enum DebugChannel : int
    {
        /// <summary>
        /// Used to Write Logs to the Debug System
        /// </summary>
        Log = 1,

        /// <summary>
        /// Used to Write Warnings to the Debug System
        /// </summary>
        Warning = 1 << 1,

        /// <summary>
        /// Used to Write Errors to the Debug System
        /// </summary>
        Error = 1 << 2,

        /// <summary>
        /// Used to Write Logs from Game Code to the Debug System
        /// </summary>
        Game = 1 << 3,

        UNNAMED_CHANNEL0 = 1 << 4,
        UNNAMED_CHANNEL1 = 1 << 5,
        UNNAMED_CHANNEL2 = 1 << 6,
        UNNAMED_CHANNEL3 = 1 << 7,
        UNNAMED_CHANNEL4 = 1 << 8,
        UNNAMED_CHANNEL5 = 1 << 9,
        UNNAMED_CHANNEL6 = 1 << 10,
        UNNAMED_CHANNEL7 = 1 << 11,
        UNNAMED_CHANNEL8 = 1 << 12,
        UNNAMED_CHANNEL9 = 1 << 13,
        UNNAMED_CHANNEL10 = 1 << 14,
        UNNAMED_CHANNEL11 = 1 << 15,
        UNNAMED_CHANNEL12 = 1 << 16,

        Physics = 1 << 17,
        Exception = 1 << 18,
        Core = 1 << 19,
        Rendering = 1 << 20,
        TextProcessor = 1 << 21,
        OpenFL = 1 << 22,
        OpenCL = 1 << 23,
        OpenGL = 1 << 24,
        UI = 1 << 25,
        Audio = 1 << 26,
        Engine = 1 << 27,
        Debug = 1 << 28,
        IO = 1 << 29,
        WFC = 1 << 30,

        EngineCore = Engine | Core,
        EngineRendering = Engine | Rendering,
    }
}