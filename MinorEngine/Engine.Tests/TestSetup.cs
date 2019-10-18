using Engine.Core;
using Xunit;
[assembly: CollectionBehavior(DisableTestParallelization = true)]
namespace Engine.Tests
{
    public static class TestSetup
    {
        public static void ApplyDebugSettings()
        {

            EngineConfig.LoadConfig("resources/engine.settings.xml", typeof(GameEngine).Assembly, "Engine.Core");

        }
    }
}