using System;

using System.Collections.Generic;
using System.Linq;
using Common;
using OpenCl.DotNetCore.DataTypes;
using OpenCl.DotNetCore.CommandQueues;
using OpenCl.DotNetCore.Kernels;
using OpenCl.DotNetCore.Memory;

namespace CLHelperLibrary
{
    public class CLKernel
    {
        public Dictionary<string, KernelParameter> Parameter { get; }
        public Kernel Kernel { get; }
        public string Name { get; }

        public CLKernel(Kernel k, string name, KernelParameter[] parameter)
        {
            Kernel = k;
            Name = name;
            this.Parameter = new Dictionary<string, KernelParameter>(parameter.Select(x => new KeyValuePair<string, KernelParameter>(x.Name, x)));
        }

        public void SetBuffer(string parameterName, MemoryObject obj)
        {
            SetBuffer(Parameter[parameterName].Id, obj);
        }

        public void SetArg(string parameterName, object value)
        {
            SetArg(Parameter[parameterName].Id, Parameter[parameterName].CastToType(value));
        }

        public void SetBuffer(int index, MemoryObject obj)
        {
#if NO_CL
            index.Log("Setting Kernel Argument " + index, DebugChannel.Warning);
#else
            Kernel.SetKernelArgument(index, obj);
#endif
        }

        public void SetArg(int index, object value)
        {
            if (value is MemoryObject)
            {
                SetBuffer(index, value as MemoryObject);
                return;
            }

            if (Parameter.ElementAt(index).Value.IsArray)
            {

                //TODO Buffer handling
            }
            else
            {
#if NO_CL
                index.Log("Setting Kernel Argument " + index, DebugChannel.Warning);
#else
                Kernel.SetKernelArgumentVal(index, Parameter.ElementAt(index).Value.CastToType(value));
#endif
            }

        }

        internal void Run(CommandQueue cq, MemoryBuffer image, int3 dimensions, MemoryBuffer enabledChannels, int channelCount)
        {
#if NO_CL

#else
            int size = dimensions.x * dimensions.y * dimensions.z * channelCount;

            SetArg(0, image);
            SetArg(1, dimensions);
            SetArg(2, channelCount);
            SetArg(3, enabledChannels);
            cq.EnqueueNDRangeKernel(Kernel, 1, size);
#endif
        }
    }
}
