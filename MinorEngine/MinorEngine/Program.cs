using System;
using Common;
using GameEngine.engine.core;
using OpenTK;
using OpenTK.Graphics;

namespace GameEngine
{
    static class Program
    {
        static void Main(string[] args)
        {

            EngineSettings es = new EngineSettings
            {
                WindowFlags = GameWindowFlags.Default,
                Mode = GraphicsMode.Default,
                Width = 1334,
                Height = 1000,
                Title = "Test"
            };

            DemoScene demo = new DemoScene(es);
            demo.Initialize();
            demo.Run();
        }
    }
}
