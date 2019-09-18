using System;

using System.Collections.Generic;
using System.Linq;
using OpenCl.DotNetCore.DataTypes;
using OpenCl.DotNetCore.CommandQueues;
using OpenCl.DotNetCore.Kernels;
using OpenCl.DotNetCore.Memory;

namespace CLHelperLibrary
{
    public class CLKernel
    {
        public Dictionary<string, KernelParameter> parameter;
        public Kernel kernel;

        public CLKernel(Kernel k, KernelParameter[] parameter)
        {
            kernel = k;
            this.parameter = new Dictionary<string, KernelParameter>(parameter.Select(x => new KeyValuePair<string, KernelParameter>(x.Name, x)));
        }

        public void SetBuffer(string parameterName, MemoryObject obj)
        {
            SetBuffer(parameter[parameterName].Id, obj);
        }

        public void SetArg(string parameterName, object value)
        {
            SetArg(parameter[parameterName].Id, parameter[parameterName].CastToType(value));
        }

        public void SetBuffer(int index, MemoryObject obj)
        {
            kernel.SetKernelArgument(index, obj);
        }

        public void SetArg(int index, object value)
        {
            if (value is MemoryObject)
            {
                SetBuffer(index, value as MemoryObject);
                return;
            }

            if (parameter.ElementAt(index).Value.IsArray)
            {

                //TODO Buffer handling
            }
            else
            {
                kernel.SetKernelArgumentVal(index, parameter.ElementAt(index).Value.CastToType(value));
            }

        }

        internal void Run(CommandQueue cq, MemoryBuffer image, int3 dimensions, MemoryBuffer enabledChannels, int channelCount)
        {
            int size = dimensions.x * dimensions.y * dimensions.z * channelCount;

            SetArg(0, image);
            SetArg(1, dimensions);
            SetArg(2, channelCount);
            SetArg(3, enabledChannels);
            cq.EnqueueNDRangeKernel(kernel, 1, size);
            
        }
    }
}
