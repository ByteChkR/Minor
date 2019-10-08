using System.Collections.Generic;
using Engine.DataTypes;
using OpenCl.DotNetCore.Memory;

namespace Engine.OpenCL
{
    internal static class BufferAbstraction
    {
        private static Dictionary<int, MemoryBuffer> _storage = new Dictionary<int, MemoryBuffer>();
        private static int _currentID = 0;

        internal static CLBuffer MakeHandle(MemoryBuffer buffer)
        {
            _storage.Add(_currentID, buffer);
            return new CLBuffer(_currentID++);
        }

        internal static MemoryBuffer ResolveAbstraction(CLBuffer buffer)
        {
            return _storage[buffer.Handle];
        }
    }
}