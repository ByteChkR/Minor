using System;
using Common;
using GameEngine.scenes;
using GameEngine.engine.core;
using OpenTK;
using OpenTK.Graphics;

namespace GameEngine
{
    static class Program
    {
        static void Main(string[] args)
        {
            GraphicsMode gm = new GraphicsMode(ColorFormat.Empty, 8, 0, 16);

            EngineSettings es = new EngineSettings
            {
                WindowFlags = GameWindowFlags.Default,
                Mode = gm,
                InitWidth = 1280,
                InitHeight = 720,
                Title = "Test",
                PhysicsThreadCount = 4
            };

            engine.core.SceneRunner engine = new engine.core.SceneRunner(es);
            engine.Initialize();
            engine.InitializeScene<AudioDemoScene>();
            engine.Run();
        }
    }
}
