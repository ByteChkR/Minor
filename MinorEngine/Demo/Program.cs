using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Common;
using Demo.scenes;
using Engine.Core;
using Engine.Debug;
using OpenTK;
using OpenTK.Graphics;

namespace Demo
{
    public struct TestStruct
    {

        public string NestedText;

    }
    internal class Program
    {

        [ConfigVariable]
        public static string TestString = "This is a text.";
        [ConfigVariable]
        public static TestStruct TestVec = new TestStruct() { NestedText = "I am a member of Test Struct" };
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
            //EngineConfig.CreateConfig(Assembly.GetAssembly(typeof(GameEngine)), "Engine");
            EngineConfig.LoadConfig("configs/engine.settings.xml", Assembly.GetAssembly(typeof(GameEngine)), "Engine");
            DebugSettings dbgSettings = EngineSettings.Settings.DebugSettings;
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



            GameEngine engine = new GameEngine(EngineSettings.Settings);
            engine.Initialize();
            engine.InitializeScene<FLDemoScene>();
            engine.Run();
        }
    }
}