using System;
using System.Reflection;
using Engine.Core;
using Engine.DataTypes;
using Engine.Debug;
using Engine.Demo.scenes;
using Engine.IO;

namespace Engine.Demo
{ 

    internal class Program
    {


        private static void Main(string[] args)
        {

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