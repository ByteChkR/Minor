using Engine.Core;
using Engine.OpenCL;
using Xunit;

[assembly: CollectionBehavior(DisableTestParallelization = true)]

namespace Engine.Tests
{
    public static class TestSetup
    {
        private static KernelDatabase _kernelDb;

        public static KernelDatabase KernelDb
        {
            get
            {
                if (_kernelDb == null)
                {
                    _kernelDb = new KernelDatabase(Clapi.MainThread, "resources/kernel",
                        OpenCL.TypeEnums.DataTypes.Uchar1);
                }

                return _kernelDb;
            }
        }

        public static void ApplyDebugSettings()
        {
            EngineConfig.LoadConfig("resources/engine.settings.xml", typeof(GameEngine).Assembly, "Engine.Core");
        }
    }
}