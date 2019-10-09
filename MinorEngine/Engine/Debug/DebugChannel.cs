namespace Engine.Debug
{
    /// <summary>
    /// An enum of Channels that can be used to send logs
    /// </summary>
    public enum DebugChannel
    {
        All = -1,
        None = 0,
        Log = 1,
        Warning = 2,
        Error = 4
    }
}