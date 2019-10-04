﻿using System.Collections.Generic;
using System.Linq;
using OpenCl.DotNetCore.CommandQueues;
using OpenCl.DotNetCore.DataTypes;
using OpenCl.DotNetCore.Kernels;
using OpenCl.DotNetCore.Memory;

namespace MinorEngine.CLHelperLibrary
{
    /// <summary>
    /// A wrapper class that holds a OpenCL kernel and the parsed informations for the kernel.
    /// </summary>
    public class CLKernel
    {
        /// <summary>
        /// Dictionary containing the Parsed Kernel Parameters Indexed by their name
        /// </summary>
        public Dictionary<string, KernelParameter> Parameter { get; }

        /// <summary>
        /// The Compiled and Linked OpenCL Kernel
        /// </summary>
        public Kernel Kernel { get; }

        /// <summary>
        /// The name of the CLKernel
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="k">The Compiled and Linked Kernel</param>
        /// <param name="name">The name of the kernel</param>
        /// <param name="parameter">The parsed KernelParameter</param>
        public CLKernel(Kernel k, string name, KernelParameter[] parameter)
        {
            Kernel = k;
            Name = name;
            Parameter = new Dictionary<string, KernelParameter>(parameter.Select(x =>
                new KeyValuePair<string, KernelParameter>(x.Name, x)));
        }

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
            SetArg(Parameter[parameterName].Id, Parameter[parameterName].CastToType(value));
        }

        /// <summary>
        /// Sets the buffer as argument
        /// </summary>
        /// <param name="index">The index of the argument</param>
        /// <param name="obj">The buffer to be set</param>
        public void SetBuffer(int index, MemoryObject obj)
        {
#if TRAVIS_TEST
            index.Log("Setting Kernel Argument " + index, DebugChannel.Warning);
#else
            Kernel.SetKernelArgument(index, obj);
#endif
        }

        /// <summary>
        /// Sets the value as argument
        /// </summary>
        /// <param name="index">The index of the argument</param>
        /// <param name="value">The value to be set</param>
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
                //Just create a buffer and pass it as MemoryObject
            }
            else
            {
#if TRAVIS_TEST
                index.Log("Setting Kernel Argument " + index, DebugChannel.Warning);
#else
                Kernel.SetKernelArgumentVal(index, Parameter.ElementAt(index).Value.CastToType(value));
#endif
            }
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
#if !TRAVIS_TEST
            int size = dimensions.x * dimensions.y * dimensions.z * channelCount;

            SetArg(0, image);
            SetArg(1, dimensions);
            SetArg(2, channelCount);
            SetArg(3, genTypeMaxVal);
            SetArg(4, enabledChannels);
            cq.EnqueueNDRangeKernel(Kernel, 1, size);
#endif
        }
    }
}