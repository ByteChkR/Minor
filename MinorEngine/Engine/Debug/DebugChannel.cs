namespace Engine.Debug
{
    /// <summary>
    /// An enum of Channels that can be used to send logs
    /// </summary>
    public enum DebugChannel
    {
        /// <summary>
        /// Wildcard. This Sends to all Channels at once
        /// </summary>
        All = -1,

        /// <summary>
        /// Everything that will be sent over this channel is ONLY reachable when creating another log stream with mask = 0 because of the nature of bitwise operations
        /// </summary>
        None = 0,

        /// <summary>
        /// Used to Write Logs to the Debug System
        /// </summary>
        Log = 1,

        /// <summary>
        /// Used to Write Warnings to the Debug System
        /// </summary>
        Warning = 2,

        /// <summary>
        /// Used to Write Errors to the Debug System
        /// </summary>
        Error = 4
    }
}