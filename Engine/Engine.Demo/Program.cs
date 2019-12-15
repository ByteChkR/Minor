﻿using System;
using System.Reflection;
using Engine.Core;
using Engine.DataTypes;
using Engine.Debug;
using Engine.Demo.scenes;
using Engine.Demo.scenes.testing;
using Engine.IO;

namespace Engine.Demo
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

        private static void RunTests(Assembly asm, string[] scenes)
        {
            ManifestReader.PrepareManifestFiles(true); //Load eventual packs from disk
            ManifestReader.RegisterAssembly(Assembly.GetExecutingAssembly()); //Register This Assembly
            if (IOManager.Exists("assemblyList.txt")) //Alternative, load assembly list to register from text file.
            {
                Logger.Log("Loading Assembly List", DebugChannel.Log, 10);

                ManifestReader.LoadAssemblyListFromFile("assemblyList.txt");
            }

            EngineConfig.LoadConfig("assets/configs/engine.settings.xml", Assembly.GetAssembly(typeof(GameEngine)),
                "Engine.Core");


            GameEngine engine = new GameEngine(EngineSettings.DefaultSettings);
            SceneRunner sr = new SceneRunner(engine, asm, scenes);


            engine.SetSettings(EngineSettings.Settings);


            engine.Initialize();

            sr.NextScene();

            engine.Run();
        }

        private static void Main(string[] args)
        {
            //PhysicsEngine.Initialize();
            //RunTests(Assembly.GetExecutingAssembly(), new []{ "Engine.Demo.scenes.testing" });
            //return;
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
            ManifestReader.PrepareManifestFiles(true); //Load eventual packs from disk
            ManifestReader.RegisterAssembly(Assembly.GetExecutingAssembly()); //Register This Assembly
            if (IOManager.Exists("assemblyList.txt")) //Alternative, load assembly list to register from text file.
            {
                Logger.Log("Loading Assembly List", DebugChannel.Log, 10);

                ManifestReader.LoadAssemblyListFromFile("assemblyList.txt");
            }


            GameEngine engine = new GameEngine(EngineSettings.DefaultSettings);

            

            engine.Initialize();
            engine.InitializeScene<FlDemoScene>();
            engine.Run();
        }
    }
}