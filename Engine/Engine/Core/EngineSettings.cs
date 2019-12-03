using Engine.Debug;
using OpenTK;
using OpenTK.Graphics;

namespace Engine.Core
{
    /// <summary>
    /// Settings for the engine
    /// </summary>
    public class EngineSettings
    {
        [ConfigVariable] public static EngineSettings Settings;

        /// <summary>
        /// The Engine Default Settings.
        /// </summary>
        public static EngineSettings DefaultSettings => new EngineSettings
        {
            DebugSettings = DebugSettings.GetDefault(),
            Depth = 8,
            Stencil = 0,
            Samples = 16,
            InitHeight = 720,
            InitWidth = 1280,
            Title = "GameTitle",
            PhysicsThreadCount = 1,
            VSync = VSyncMode.On,
            WindowFlags = GameWindowFlags.Default
        };

        /// <summary>
        /// The Graphics Mode of the Window
        /// </summary>
        public GraphicsMode Mode => new GraphicsMode(ColorFormat.Empty, Depth, Stencil, Samples);

        /// <summary>
        /// The Depth Bits
        /// </summary>
        public int Depth { get; set; } = 8;

        /// <summary>
        /// The Stencil Bits
        /// </summary>
        public int Stencil { get; set; }

        /// <summary>
        /// The FXAA Samples
        /// </summary>
        public int Samples { get; set; } = 16;


        /// <summary>
        /// Width of the Window
        /// </summary>
        public int InitWidth { get; set; }

        /// <summary>
        /// Height of the Window
        /// </summary>
        public int InitHeight { get; set; }

        /// <summary>
        /// Physics Threads
        /// </summary>
        public int PhysicsThreadCount { get; set; } = 1;

        /// <summary>
        /// Title of the Window
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Vsync Enable State
        /// </summary>
        public VSyncMode VSync { get; set; } = VSyncMode.Off;

        /// <summary>
        /// The Game window flags
        /// </summary>
        public GameWindowFlags WindowFlags { get; set; }

        /// <summary>
        /// Debug settings
        /// </summary>
        public DebugSettings DebugSettings { get; set; }
    }
}