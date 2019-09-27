using System;
using Common;
using MinorEngine.engine.core;
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
