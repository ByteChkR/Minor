using System;
using System.IO;
using System.Linq;
using Xunit;
using OpenCl.DotNetCore.Memory;
namespace CLHelperLibrary.Tests
{
    public class CLTests
    {
#if NO_CL
#else
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
        
#endif
        [Fact]
        public void CL_KernelSignatureAnalysis()
        {
            string path = Path.GetFullPath("../../../resources");
            KernelDatabase kdb = new KernelDatabase(path);

            Assert.True(kdb.TryGetCLKernel("addval", out CLKernel kernel));
            
            Assert.True(kernel.parameter.Count == 5);

            Assert.True(kernel.parameter.ElementAt(0).Value.IsArray);
            Assert.True(kernel.parameter.ElementAt(0).Value.DataType == DataTypes.UCHAR1);
            Assert.True(kernel.parameter.ElementAt(0).Value.Id == 0);
            Assert.True(kernel.parameter.ElementAt(0).Value.Name == "image");

            Assert.False(kernel.parameter.ElementAt(1).Value.IsArray);
            Assert.True(kernel.parameter.ElementAt(1).Value.DataType == DataTypes.INT3);
            Assert.True(kernel.parameter.ElementAt(1).Value.Id == 1);
            Assert.True(kernel.parameter.ElementAt(1).Value.Name == "dimensions");

            Assert.False(kernel.parameter.ElementAt(2).Value.IsArray);
            Assert.True(kernel.parameter.ElementAt(2).Value.DataType == DataTypes.INT1);
            Assert.True(kernel.parameter.ElementAt(2).Value.Id == 2);
            Assert.True(kernel.parameter.ElementAt(2).Value.Name == "channelCount");

            Assert.True(kernel.parameter.ElementAt(3).Value.IsArray);
            Assert.True(kernel.parameter.ElementAt(3).Value.DataType == DataTypes.UCHAR1);
            Assert.True(kernel.parameter.ElementAt(3).Value.Id == 3);
            Assert.True(kernel.parameter.ElementAt(3).Value.Name == "channelEnableState");

            Assert.False(kernel.parameter.ElementAt(4).Value.IsArray);
            Assert.True(kernel.parameter.ElementAt(4).Value.DataType == DataTypes.UCHAR1);
            Assert.True(kernel.parameter.ElementAt(4).Value.Id == 4);
            Assert.True(kernel.parameter.ElementAt(4).Value.Name == "value");

        }
    }
}
