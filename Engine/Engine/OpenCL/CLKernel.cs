using System.Collections.Generic;
using System.Linq;
using Engine.OpenCL.DotNetCore.CommandQueues;
using Engine.OpenCL.DotNetCore.DataTypes;
using Engine.OpenCL.DotNetCore.Kernels;
using Engine.OpenCL.DotNetCore.Memory;


namespace Engine.OpenCL
{
    /// <summary>
    /// A wrapper class that holds a OpenCL kernel and the parsed informations for the kernel.
    /// </summary>
    public class CLKernel
    {
        private CLAPI _instance;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="k">The Compiled and Linked Kernel</param>
        /// <param name="name">The name of the kernel</param>
        /// <param name="parameter">The parsed KernelParameter</param>
        public CLKernel(CLAPI instance, Kernel k, string name, KernelParameter[] parameter)
        {
            _instance = instance;
            Kernel = k;
            Name = name;
            Parameter = new Dictionary<string, KernelParameter>(parameter.Select(x =>
                new KeyValuePair<string, KernelParameter>(x.Name, x)));
        }

        /// <summary>
        /// Dictionary containing the Parsed Kernel Parameters Indexed by their name
        /// </summary>
        public Dictionary<string, KernelParameter> Parameter { get; }

        /// <summary>
        /// The Compiled and Linked OpenCL Kernel
        /// </summary>
        private Kernel Kernel { get; }

        /// <summary>
        /// The name of the CLKernel
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Sets the buffer as argument.
        /// </summary>
        /// <param name="parameterName">The name of the parameter</param>
        /// <param name="obj">The buffer to be set</param>
        public void SetBuffer(string parameterName, MemoryObject obj)
        {
            SetBuffer(Parameter[parameterName].Id, obj);
        }

        /// <summary>
        /// Sets the value as argument
        /// </summary>
        /// <param name="parameterName">The name of the parameter</param>
        /// <param name="value">The value to be set</param>
        public void SetArg(string parameterName, object value)
        {
            SetArg(Parameter[parameterName].Id, Parameter[parameterName].CastToType(_instance, value));
        }

        /// <summary>
        /// Sets the buffer as argument
        /// </summary>
        /// <param name="index">The index of the argument</param>
        /// <param name="obj">The buffer to be set</param>
        public void SetBuffer(int index, MemoryObject obj)
        {

            Kernel.SetKernelArgument(index, obj);
        }

        /// <summary>
        /// Sets the value as argument
        /// </summary>
        /// <param name="index">The index of the argument</param>
        /// <param name="value">The value to be set</param>
        public void SetArg(int index, object value)
        {
            if (value is MemoryBuffer buffer)
            {
                SetBuffer(index, buffer);

                return;
            }



            Kernel.SetKernelArgumentVal(index, Parameter.ElementAt(index).Value.CastToType(_instance, value));

        }

        /// <summary>
        /// Runs the FL Compliant kernel
        /// </summary>
        /// <param name="cq">Command Queue to be used</param>
        /// <param name="image">The image buffer</param>
        /// <param name="dimensions">The dimensions of the image buffer</param>
        /// <param name="enabledChannels">The enabled channels of the input buffer</param>
        /// <param name="channelCount">The number of channels in use</param>
        internal void Run(CommandQueue cq, MemoryBuffer image, int3 dimensions, float genTypeMaxVal,
            MemoryBuffer enabledChannels, int channelCount)
        {

            int size = dimensions.x * dimensions.y * dimensions.z * channelCount;


            SetArg(0, image);
            SetArg(1, dimensions);
            SetArg(2, channelCount);
            SetArg(3, genTypeMaxVal);
            SetArg(4, enabledChannels);
            Run(cq, 1, size);

        }


        /// <summary>
        /// runs the Kernel with a command queue
        /// This function requires setting ALL parameters manually
        /// </summary>
        /// <param name="cq">Command Queue to use</param>
        /// <param name="workdim">The working dimension(usually 1)</param>
        /// <param name="groupsize">The group size(usually buffer.size)</param>
        public void Run(CommandQueue cq, int workdim, int groupsize)
        {
            cq.EnqueueNDRangeKernel(Kernel, workdim, groupsize);
        }
    }
}