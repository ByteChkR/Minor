using Engine.OpenCL.DotNetCore.Memory;
using OpenTK.Input;

namespace Engine.OpenFL
{
    public class CLBufferInfo
    {
        public MemoryBuffer Buffer;
        public bool IsInternal { get; private set; }
        public string DefinedBufferName { get; private set; }
        internal CLBufferInfo(MemoryBuffer buffer, bool isInternal)
        {
            IsInternal = isInternal;
            Buffer = buffer;
            DefinedBufferName = "UnnamedBuffer";
        }

        internal void SetInternalState(bool internalState)
        {
            IsInternal = internalState;
        }

        internal void SetKey(string key)
        {
            DefinedBufferName = key;
        }

        public override string ToString()
        {
            return DefinedBufferName + "_" + (IsInternal ? "internal" : "unmanaged");
        }
    }
}