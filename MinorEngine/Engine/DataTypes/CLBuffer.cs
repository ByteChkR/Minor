using System;
using Engine.OpenCL;

namespace Engine.DataTypes
{
    public class CLBuffer : IDisposable
    {
        public readonly int Handle;
        public long Size => BufferAbstraction.ResolveAbstraction(this).Size;


        internal CLBuffer(int handle)
        {
            Handle = handle;
        }

        public void Dispose()
        {
            BufferAbstraction.ResolveAbstraction(this).Dispose();
        }
    }
}