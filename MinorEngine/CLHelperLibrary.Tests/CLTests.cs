using System;
using System.IO;
using System.Linq;
using Xunit;
using OpenCl.DotNetCore.Memory;
using Xunit;
namespace CLHelperLibrary.Tests
{
    public class CLTests
    {
        [Fact]
        public void CL_CreateBuffer()
        {
            byte[] b = new byte[255];
            for (int i = 0; i < b.Length; i++)
            {
                b[i] = (byte)i;
            }

            MemoryBuffer buffer = CL.CreateBuffer(b, MemoryFlag.CopyHostPointer | MemoryFlag.ReadWrite);
            
            Assert.True(buffer != null);
            Assert.True(buffer.Size == 255);
            
        }

        [Fact]
        public void CL_ReadBuffer()
        {
            float[] b = new float[255];
            for (int i = 0; i < b.Length; i++)
            {
                b[i] = (float)i;
            }

            MemoryBuffer buffer = CL.CreateBuffer(b, MemoryFlag.CopyHostPointer | MemoryFlag.ReadWrite);

            bool working = true;
            float[] c = CL.ReadBuffer<float>(buffer, b.Length);
            for (int i = 0; i < b.Length; i++)
            {
                if (Math.Abs(b[i] - c[i]) > 0.01f)
                {
                    working = false;
                }
            }
            Assert.True(working);
        }

        [Fact]
        public void CL_WriteBuffer()
        {
            float[] b = new float[255];
            for (int i = 0; i < b.Length; i++)
            {
                b[i] = (float)i;
            }

            MemoryBuffer buffer = CL.CreateEmpty<float>(b.Length, MemoryFlag.CopyHostPointer | MemoryFlag.ReadWrite);

            bool working = true;

            CL.WriteToBuffer<float>(buffer, b);

            float[] c = CL.ReadBuffer<float>(buffer, b.Length);

            for (int i = 0; i < b.Length; i++)
            {
                if (Math.Abs(b[i] - c[i]) > 0.01f)
                {
                    working = false;
                }
            }
            Assert.True(working);
        }

        [Fact]
        public void CL_KernelSignatureAnalysis()
        {
            string path = Path.GetFullPath("../../../resources");
            KernelDatabase kdb = new KernelDatabase(path);

            Assert.True(kdb.TryGetCLKernel("addval", out CLKernel kernel));
            
            Assert.True(kernel.name=="addval");
            Assert.True(kernel.kernel != null);
            Assert.True(kernel.parameter.Count == 5);

            Assert.True(kernel.parameter.ElementAt(0).Value.isArray);
            Assert.True(kernel.parameter.ElementAt(0).Value.dataType == DataTypes.UCHAR1);
            Assert.True(kernel.parameter.ElementAt(0).Value.id == 0);
            Assert.True(kernel.parameter.ElementAt(0).Value.name == "image");

            Assert.False(kernel.parameter.ElementAt(1).Value.isArray);
            Assert.True(kernel.parameter.ElementAt(1).Value.dataType == DataTypes.INT3);
            Assert.True(kernel.parameter.ElementAt(1).Value.id == 1);
            Assert.True(kernel.parameter.ElementAt(1).Value.name == "dimensions");

            Assert.False(kernel.parameter.ElementAt(2).Value.isArray);
            Assert.True(kernel.parameter.ElementAt(2).Value.dataType == DataTypes.INT1);
            Assert.True(kernel.parameter.ElementAt(2).Value.id == 2);
            Assert.True(kernel.parameter.ElementAt(2).Value.name == "channelCount");

            Assert.True(kernel.parameter.ElementAt(3).Value.isArray);
            Assert.True(kernel.parameter.ElementAt(3).Value.dataType == DataTypes.UCHAR1);
            Assert.True(kernel.parameter.ElementAt(3).Value.id == 3);
            Assert.True(kernel.parameter.ElementAt(3).Value.name == "channelEnableState");

            Assert.False(kernel.parameter.ElementAt(4).Value.isArray);
            Assert.True(kernel.parameter.ElementAt(4).Value.dataType == DataTypes.UCHAR1);
            Assert.True(kernel.parameter.ElementAt(4).Value.id == 4);
            Assert.True(kernel.parameter.ElementAt(4).Value.name == "value");

        }
    }
}
