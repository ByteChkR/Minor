using System;
using System.IO;
using Engine.Core;
using Engine.Physics.BEPUutilities;
using Xunit;

namespace Engine.Tests
{
    public class ConfigurationLoader
    {
        public ConfigurationLoader()
        {
            TestSetup.ApplyDebugSettings();
        }

        [Serializable]
        public struct TestStruct
        {
            public int TestInt;
            public string TestString;
            public Vector4 Position;
        }

        [Serializable]
        public struct TestContainer
        {
            [ConfigVariable] public string ContainerName;
            [ConfigVariable] public TestStruct Data;
        }

        [Fact]
        public void TestSaving()
        {
            TestContainer container = new TestContainer()
            {
                ContainerName = "TestContainer",
                Data = new TestStruct() {TestString = "TestString", Position = Vector4.UnitY, TestInt = 123}
            };

            EngineConfig.CreateConfig(container, "resources/ConfigTest_Save.xml");

            string created = File.ReadAllText("resources/ConfigTest_Save.xml");
            string original = File.ReadAllText("resources/io/test_struct_original.xml");
            System.Diagnostics.Debug.Assert(original == created);
        }

        [Fact]
        public void TestLoading()
        {
            TestContainer reference = new TestContainer()
            {
                ContainerName = "TestContainer",
                Data = new TestStruct() {TestString = "TestString", Position = Vector4.UnitY, TestInt = 123}
            };

            object container = new TestContainer();
            EngineConfig.LoadConfig("resources/io/test_struct_original.xml", ref container);

            TestContainer c = (TestContainer) container;
            System.Diagnostics.Debug.Assert(reference.ContainerName == c.ContainerName);
            System.Diagnostics.Debug.Assert(reference.Data.TestString == c.Data.TestString);
            System.Diagnostics.Debug.Assert(reference.Data.TestInt == c.Data.TestInt);
            System.Diagnostics.Debug.Assert(reference.Data.Position == c.Data.Position);
        }
    }
}