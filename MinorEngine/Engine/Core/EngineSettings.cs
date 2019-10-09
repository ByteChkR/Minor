﻿using System;
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
        /// <summary>
        /// The Graphics Mode of the Window
        /// </summary>
        public GraphicsMode Mode { get; set; }

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