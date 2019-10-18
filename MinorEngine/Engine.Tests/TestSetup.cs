using Engine.Core;
using Engine.OpenCL;
using Xunit;
[assembly: CollectionBehavior(DisableTestParallelization = true)]
namespace Engine.Tests
{
    public static class TestSetup
    {
        private static KernelDatabase kernelDB;
        public static KernelDatabase KernelDB
        {
            get
            {
                if (kernelDB == null)
                {
                    kernelDB=new KernelDatabase("resources/kernel", OpenCL.TypeEnums.DataTypes.UCHAR1);
                }

                return kernelDB;
            }
        }
        public static void ApplyDebugSettings()
        {

            EngineConfig.LoadConfig("resources/engine.settings.xml", typeof(GameEngine).Assembly, "Engine.Core");

        }
    }
}