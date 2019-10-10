using System;
using System.Collections.Generic;
using System.Reflection;
using Common;
using Demo.scenes;
using Engine.Core;
using Engine.Debug;
using OpenTK;
using OpenTK.Graphics;

namespace Demo
{
    internal class Program
    {
        private static bool AskForDebugLogSending()
        {
            Console.WriteLine("Allow Sending Debug Logs? [y/N]:");
            if (Console.ReadLine().ToLower() == "y")
            {
                return true;
            }

            return false;
        }

        private static void Main(string[] args)
        {
            GraphicsMode gm = new GraphicsMode(ColorFormat.Empty, 8, 0, 16);

            DebugSettings dbgSettings = DebugSettings.Default;

#if COLLECT_LOGS
            if (AskForDebugLogSending())
            {
                List<ILogStreamSettings> streams = new List<ILogStreamSettings>(dbgSettings.Streams);

                LogStreamSettings network = new LogStreamSettings()
                {
                    Mask = -1,
                    Destination = "213.109.162.193",
                    NetworkAppID = 2,
                    NetworkAuthVersion =
                        Assembly.GetExecutingAssembly().GetName()
                            .Version, //We Want the debug system to take the game version
                    NetworkPort = 420,
                    PrefixMode = 1,
                    Timestamp = true
                };
                streams.Add(network);
            }
#endif

            EngineSettings es = new EngineSettings
            {
                WindowFlags = GameWindowFlags.Default,
                Mode = gm,
                InitWidth = 1280,
                InitHeight = 720,
                Title = "Test",
                PhysicsThreadCount = 4,
                VSync = VSyncMode.Off,
                DebugSettings = dbgSettings,
            };


            GameEngine engine = new GameEngine(es);
            engine.Initialize();
            engine.InitializeScene<FLDemoScene>();
            engine.Run();
        }
    }
}