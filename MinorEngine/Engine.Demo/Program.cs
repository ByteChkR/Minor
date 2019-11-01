using System;
using System.Reflection;
using Engine.Core;
using Engine.DataTypes;
using Engine.Debug;
using Engine.Demo.scenes;

namespace Engine.Demo
{
    public struct TestStruct
    {
        public string NestedText;
    }

    internal class Program
    {
        [ConfigVariable] public static string TestString = "This is a text.";

        [ConfigVariable]
        public static TestStruct TestVec = new TestStruct() {NestedText = "I am a member of Test Struct"};

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

            ManifestReader.RegisterAssembly(Assembly
                .GetExecutingAssembly()); //Not needed anymore since now its possible to load the assemblies from file.

            GameEngine engine = new GameEngine(EngineSettings.DefaultSettings);

            //ManifestReader.PrepareManifestFiles(true);

            //EngineConfig.CreateConfig(Assembly.GetAssembly(typeof(GameEngine)), "Engine.Core" , "configs/engine.settings.xml");

            EngineConfig.LoadConfig("assets/configs/engine.settings.xml", Assembly.GetAssembly(typeof(GameEngine)),
                "Engine.Core");
            engine.SetSettings(EngineSettings.Settings);


            engine.Initialize();
            engine.InitializeScene<FLDemoScene>();
            engine.Run();
        }
    }
}