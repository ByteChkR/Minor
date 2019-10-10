using Engine.DataTypes;
using Engine.OpenCL.DotNetCore.CommandQueues;
using Engine.OpenCL.DotNetCore.DataTypes;
using Engine.OpenCL.DotNetCore.Memory;
using Engine.OpenCL.TypeEnums;
using OpenTK;

namespace Engine.OpenCL
{
    public static class BufferOperations
    {
        public static MemoryBuffer GetRegion<T>(MemoryBuffer buffer, int3 sourceBufferDimensions, int channelcount,
            int3 start, int3 bounds) where T : struct
        {
            CLProgram program = new CLProgram("internal_kernels/get_buffer_region.cl", "");

            CLKernel kernel = program.ContainedKernels["getregion"];

            int bufferSize = sourceBufferDimensions.x * sourceBufferDimensions.y * sourceBufferDimensions.z *
                             channelcount;

            int dstSize = bounds.x * bounds.y * bounds.z * channelcount;

            MemoryBuffer buf = CLAPI.CreateEmpty<T>(dstSize, MemoryFlag.ReadWrite);

            kernel.SetBuffer("image", buffer);
            kernel.SetBuffer("destination", buf);
            kernel.SetArg("start", start);
            kernel.SetArg("bounds", bounds);
            kernel.SetArg("channelCount", channelcount);
            kernel.SetArg("dimensions", sourceBufferDimensions);

            CommandQueue cq = CLAPI.GetQueue();
            kernel.Run(cq, 1, bufferSize);


            T[] ret = CLAPI.ReadBuffer<T>(buf, dstSize);
            return buf;
        }
    }
}