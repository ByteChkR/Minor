namespace Engine.Debug
{
    /// <summary>
    /// The List of DebugStages used to Divide the Debug Logs and Data Statistics by.
    /// </summary>
    public enum DebugStage
    {
        /// <summary>
        /// Is used when the engine is starting up(before any user code has been executed.
        /// </summary>
        Startup = 1,

        /// <summary>
        /// Is used when the engine is initializing itself(e.g. Audio/Physics/Rendering/etc..)
        /// </summary>
        Init = 2,

        /// <summary>
        /// Is used when loading a Scene
        /// </summary>
        SceneInit = 4,

        /// <summary>
        /// Used Between the Init Stage and the Scene Init Stage
        /// </summary>
        General = 8,

        /// <summary>
        /// Is used for the Update Loop of the engine
        /// </summary>
        Update = 16,

        /// <summary>
        /// Is used for the physics update in the loop
        /// </summary>
        Physics = 32,

        /// <summary>
        /// Is Used when Cleaning up destroyed objects after the Update Frame
        /// </summary>
        CleanUp = 64,

        /// <summary>
        /// Is used during the render stages.
        /// </summary>
        Render = 128
    }
}