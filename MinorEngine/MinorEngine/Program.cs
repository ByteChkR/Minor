using System;
using Common;
using GameEngine.engine.core;
using OpenTK;
using OpenTK.Graphics;

namespace GameEngine
{
    class Program
    {
        static void Main(string[] args)
        {

            EngineSettings es = new EngineSettings()
            {
                gameWindowFlags = GameWindowFlags.Default,
                graphicsMode = GraphicsMode.Default,
                width = 1334,
                height = 1000,
                title = "Test"
            };

            DemoScene demo = new DemoScene(es);
            demo.Initialize();
            demo.Run();
        }
    }
}
