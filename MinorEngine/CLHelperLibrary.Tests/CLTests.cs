using System;
using System.IO;
using System.Linq;
using CLHelperLibrary.CLStructs;
using Common;
using Xunit;
using OpenCl.DotNetCore.Memory;
namespace CLHelperLibrary.Tests
{
    public class CLTests
    {
#if !TRAVIS_TEST
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
            
            float[] c = CL.ReadBuffer<float>(buffer, b.Length);


            Assert.True(CheckValues(c, b));
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
            

            CL.WriteToBuffer<float>(buffer, b);

            float[] c = CL.ReadBuffer<float>(buffer, b.Length);

            
            Assert.True(CheckValues(c,b));
        }

        private static bool CheckValues(float[] values, float[] reference)
        {
            bool working = true;
            for (int i = 0; i < values.Length; i++)
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
        public void CL_KernelSignatureAnalysis()
        {
            DebugHelper.ListeningMask = DebugChannel.Log | DebugChannel.Error | DebugChannel.Internal_Error |
                                        DebugChannel.Warning;
            string path = Path.GetFullPath("../../../resources");
            KernelDatabase kdb = new KernelDatabase(path, DataTypes.UCHAR1);

            Assert.True(kdb.TryGetCLKernel("addval", out CLKernel kernel));
            
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
