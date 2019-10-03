﻿namespace CLHelperLibrary.debug
{
    /// <summary>
    /// An enum of Channels that can be used to send logs
    /// </summary>
    internal enum DebugChannel
    {
        ALL = -1,
        None = 0,
        Error = 1,
        Warning = 2,
        Log = 4,
        Internal_Error = 8,
        Progress = 16
    }
}