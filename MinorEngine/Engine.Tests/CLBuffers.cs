using System;
using Common;
using Engine.OpenCL;
using Engine.OpenCL.DotNetCore.Memory;
using Xunit;

namespace Engine.Tests
{
#if !NO_CL
    public class CLBuffers
    {
        [Fact]
        public void CreateBuffer()
        {
            DebugHelper.ThrowOnAllExceptions = true;
            byte[] b = new byte[255];
            for (int i = 0; i < b.Length; i++)
            {
                b[i] = (byte)i;
            }

            MemoryBuffer buffer = CLAPI.CreateBuffer(b, MemoryFlag.CopyHostPointer | MemoryFlag.ReadWrite);

            Assert.True(buffer != null);
            Assert.True(buffer.Size == 255);
        }

        [Fact]
        public void ReadBuffer()
        {
            DebugHelper.ThrowOnAllExceptions = true;
            float[] b = new float[255];
            for (int i = 0; i < b.Length; i++)
            {
                b[i] = i;
            }

            MemoryBuffer buffer = CLAPI.CreateBuffer(b, MemoryFlag.CopyHostPointer | MemoryFlag.ReadWrite);

            float[] c = CLAPI.ReadBuffer<float>(buffer, b.Length);


            Assert.True(CheckValues(c, b));
        }

        [Fact]
        public void WriteBuffer()
        {
            DebugHelper.ThrowOnAllExceptions = true;
            float[] b = new float[255];
            for (int i = 0; i < b.Length; i++)
            {
                b[i] = i;
            }

            MemoryBuffer buffer = CLAPI.CreateEmpty<float>(b.Length, MemoryFlag.CopyHostPointer | MemoryFlag.ReadWrite);


            CLAPI.WriteToBuffer(buffer, b);

            float[] c = CLAPI.ReadBuffer<float>(buffer, b.Length);


            Assert.True(CheckValues(c, b));
        }

        private static bool CheckValues(float[] values, float[] reference)
        {
            DebugHelper.ThrowOnAllExceptions = true;
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
    }

#endif
}