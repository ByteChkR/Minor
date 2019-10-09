using System;
using System.IO;
using System.Linq;
using Common;
using Engine.OpenCL;
using Engine.OpenCL.DotNetCore.Memory;
using Engine.OpenCL.TypeEnums;
using Engine.OpenFL;
using Xunit;

namespace MinorEngine.Tests
{
    public class Tests
    {
#if !TRAVIS_TEST
        [Fact]
        public void CL_CreateBuffer()
        {
            var b = new byte[255];
            for (var i = 0; i < b.Length; i++)
            {
                b[i] = (byte) i;
            }

            var buffer = CLAPI.CreateBuffer(b, MemoryFlag.CopyHostPointer | MemoryFlag.ReadWrite);

            Assert.True(buffer != null);
            Assert.True(buffer.Size == 255);
        }

        [Fact]
        public void CL_ReadBuffer()
        {
            var b = new float[255];
            for (var i = 0; i < b.Length; i++)
            {
                b[i] = i;
            }

            var buffer = CLAPI.CreateBuffer(b, MemoryFlag.CopyHostPointer | MemoryFlag.ReadWrite);

            var c = CLAPI.ReadBuffer<float>(buffer, b.Length);


            Assert.True(CheckValues(c, b));
        }

        [Fact]
        public void CL_WriteBuffer()
        {
            var b = new float[255];
            for (var i = 0; i < b.Length; i++)
            {
                b[i] = i;
            }

            var buffer = CLAPI.CreateEmpty<float>(b.Length, MemoryFlag.CopyHostPointer | MemoryFlag.ReadWrite);


            CLAPI.WriteToBuffer(buffer, b);

            var c = CLAPI.ReadBuffer<float>(buffer, b.Length);


            Assert.True(CheckValues(c, b));
        }

        private static bool CheckValues(float[] values, float[] reference)
        {
            var working = true;
            for (var i = 0; i < values.Length; i++)
            {
                if (Math.Abs(values[i] - reference[i]) > 0.01f)
                {
                    working = false;
                }
            }

            return working;
        }

#endif
        [Fact]
        public void FLInterpreterTest()
        {
            DebugHelper.SeverityFilter = 10;
            var oldPath = Directory.GetCurrentDirectory();
            var path = Path.GetFullPath("../../../resources");
            var files = Directory.GetFiles(path + "/filter/tests", "*.fl");

            Directory.SetCurrentDirectory(path);
            var db = new KernelDatabase(path + "/kernel", DataTypes.UCHAR1);

            foreach (var file in files)
            {
                var P = new Interpreter(file,
                    CLAPI.CreateEmpty<byte>(512 * 512 * 4, MemoryFlag.CopyHostPointer | MemoryFlag.ReadWrite), 512, 512, 1,
                    4, db);
                while (!P.Terminated)
                {
                    P.Step();
                }
            }

            Directory.SetCurrentDirectory(oldPath);
        }


        [Fact]
        public void CL_KernelSignatureAnalysis()
        {
            DebugHelper.SeverityFilter = 10;
            var path = Path.GetFullPath("../../../resources");
            var kdb = new KernelDatabase(path, DataTypes.UCHAR1);

            Assert.True(kdb.TryGetCLKernel("addval", out var kernel));

            Assert.True(kernel.Parameter.Count == 5);

            Assert.True(kernel.Parameter.ElementAt(0).Value.IsArray);
            Assert.True(kernel.Parameter.ElementAt(0).Value.DataType == DataTypes.UCHAR1);
            Assert.True(kernel.Parameter.ElementAt(0).Value.Id == 0);
            Assert.True(kernel.Parameter.ElementAt(0).Value.Name == "image");

            Assert.False(kernel.Parameter.ElementAt(1).Value.IsArray);
            Assert.True(kernel.Parameter.ElementAt(1).Value.DataType == DataTypes.INT3);
            Assert.True(kernel.Parameter.ElementAt(1).Value.Id == 1);
            Assert.True(kernel.Parameter.ElementAt(1).Value.Name == "dimensions");

            Assert.False(kernel.Parameter.ElementAt(2).Value.IsArray);
            Assert.True(kernel.Parameter.ElementAt(2).Value.DataType == DataTypes.INT1);
            Assert.True(kernel.Parameter.ElementAt(2).Value.Id == 2);
            Assert.True(kernel.Parameter.ElementAt(2).Value.Name == "channelCount");

            Assert.True(kernel.Parameter.ElementAt(3).Value.IsArray);
            Assert.True(kernel.Parameter.ElementAt(3).Value.DataType == DataTypes.UCHAR1);
            Assert.True(kernel.Parameter.ElementAt(3).Value.Id == 3);
            Assert.True(kernel.Parameter.ElementAt(3).Value.Name == "channelEnableState");

            Assert.False(kernel.Parameter.ElementAt(4).Value.IsArray);
            Assert.True(kernel.Parameter.ElementAt(4).Value.DataType == DataTypes.UCHAR1);
            Assert.True(kernel.Parameter.ElementAt(4).Value.Id == 4);
            Assert.True(kernel.Parameter.ElementAt(4).Value.Name == "value");
        }
    }
}