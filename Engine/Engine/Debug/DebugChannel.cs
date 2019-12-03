using System;

namespace Engine.Debug
{
    /// <summary>
    /// An enum of Channels that can be used to send logs
    /// </summary>
    [Flags]
    public enum DebugChannel
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

        UnnamedChannel0 = 1 << 4,
        UnnamedChannel1 = 1 << 5,
        UnnamedChannel2 = 1 << 6,
        UnnamedChannel3 = 1 << 7,
        UnnamedChannel4 = 1 << 8,
        UnnamedChannel5 = 1 << 9,
        UnnamedChannel6 = 1 << 10,
        UnnamedChannel7 = 1 << 11,
        UnnamedChannel8 = 1 << 12,
        UnnamedChannel9 = 1 << 13,
        UnnamedChannel10 = 1 << 14,
        UnnamedChannel11 = 1 << 15,
        UnnamedChannel12 = 1 << 16,

        Physics = 1 << 17,
        Exception = 1 << 18,
        Core = 1 << 19,
        Rendering = 1 << 20,
        TextProcessor = 1 << 21,
        OpenFl = 1 << 22,
        OpenCl = 1 << 23,
        OpenGl = 1 << 24,
        Ui = 1 << 25,
        Audio = 1 << 26,
        Engine = 1 << 27,
        Debug = 1 << 28,
        Io = 1 << 29,
        Wfc = 1 << 30,

        EngineCore = Engine | Core,
        EngineRendering = Engine | Rendering
    }
}