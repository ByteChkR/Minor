using Demo.scenes;
using MinorEngine.engine;
using OpenTK;
using OpenTK.Graphics;

namespace Demo
{
    class Program
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
                PhysicsThreadCount = 4,
                VSync = VSyncMode.Off,

#if LOG_NETWORK
                DebugNetwork = true,
                NetworkMask = -1,
                ProgramID = 1,
                ProgramVersion = null, //We Want the debug system to take the engine assembly
#endif
            };

            MinorEngine.engine.core.GameEngine engine = new MinorEngine.engine.core.GameEngine(es);
            engine.Initialize();
            engine.InitializeScene<FLDemoScene>();
            engine.Run();
        }
    }
}