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

        ///Namespaces
        AI = 1 << 4,
        AssetPackaging = 1 << 5,
        Audio = 1 << 6,
        Core = 1 << 7,
        Debug = 1 << 8,
        IO = 1 << 9,
        OpenCL = 1 << 10,
        OpenGL = 1 << 11,
        OpenFL = 1 << 12,
        Physics = 1 << 13,
        Rendering = 1 << 14,
        TextProcessor = 1 << 15,
        UI = 1 << 16,
        Animations = 1 << 17,
        WFC = 1 << 18,

        //Helping Flags
        Exception = 1 << 19,
        Engine = 1 << 20,

        //Combinations
        EngineAI = Engine | AI,
        EngineAssetPackaging = Engine | AssetPackaging,
        EngineAudio = Engine | Audio,
        EngineCore = Engine | Core,
        EngineDebug = Engine | Debug,
        EngineIO = Engine | IO,
        EngineOpenCL = Engine | OpenCL,
        EngineOpenGL = Engine | OpenGL,
        EngineOpenFL = Engine | OpenFL,
        EnginePhysics = Engine | Physics,
        EngineRendering = Engine | Rendering,
        EngineTextProcessor = Engine | TextProcessor,
        EngineUI = Engine | UI,
        EngineUIAnimations = EngineUI | Animations,
        EngineWFC = Engine | WFC,

        GameAI = Game | AI,
        GameAssetPackaging = Game | AssetPackaging,
        GameAudio = Game | Audio,
        GameCore = Game | Core,
        GameDebug = Game | Debug,
        GameIO = Game | IO,
        GameOpenCL = Game | OpenCL,
        GameOpenGL = Game | OpenGL,
        GameOpenFL = Game | OpenFL,
        GamePhysics = Game | Physics,
        GameRendering = Game | Rendering,
        GameTextProcessor = Game | TextProcessor,
        GameUI = Game | UI,
        GameUIAnimations = GameUI | Animations,
        GameWFC = Game | WFC,

    }
}