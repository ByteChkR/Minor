using System;
using Engine.Debug;
using OpenTK;
using OpenTK.Graphics;

namespace Engine.Core
{ 
    public class EngineSettings
    {
        public GraphicsMode Mode { get; set; }
        public int InitWidth { get; set; }
        public int InitHeight { get; set; }
        public int PhysicsThreadCount { get; set; } = 1;
        public string Title { get; set; }
        
        public VSyncMode VSync { get; set; } = VSyncMode.Off;
        public GameWindowFlags WindowFlags { get; set; }
        public DebugSettings DebugSettings { get; set; }
    }
}