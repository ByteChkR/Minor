using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using Engine.DataTypes;
using Engine.OpenCL.DotNetCore.CommandQueues;
using Engine.OpenCL.DotNetCore.Contexts;
using Engine.OpenCL.DotNetCore.DataTypes;
using Engine.OpenCL.DotNetCore.Devices;
using Engine.OpenCL.DotNetCore.Kernels;
using Engine.OpenCL.DotNetCore.Memory;
using Engine.OpenCL.DotNetCore.Platforms;
using Engine.OpenCL.DotNetCore.Programs;
using Engine.OpenCL.TypeEnums;
using OpenTK.Platform.Windows;


#if NO_CL
using Engine.Debug;

#endif

namespace Engine.OpenCL
{
    /// <summary>
    /// A wrapper class that is handling all the CL operations.
    /// </summary>
    public class CLAPI
    {
        /// <summary>
        /// Field that holds the instance of the CL wrapper
        /// </summary>
        private static CLAPI _instance;

        /// <summary>
        /// Helpful property for initializing the singleton
        /// </summary>
        private static CLAPI Instance => _instance ?? (_instance = new CLAPI());

        /// <summary>
        /// The CL Context that the wrapper is using
        /// </summary>
        private Context _context;

        /// <summary>
        /// The Command queue that the wrapper is using
        /// </summary>
        private CommandQueue _commandQueue;

        internal static CommandQueue GetQueue()
        {
            return Instance._commandQueue;
        }

        /// <summary>
        /// Private constructor
        /// </summary>
        private CLAPI()
        {
            InitializeOpenCL();
        }

        /// <summary>
        /// Initializes the OpenCL API
        /// </summary>
        private void InitializeOpenCL()
        {
#if NO_CL
            Logger.Log("Starting in NO_CL Mode", DebugChannel.Warning);
#else
            IEnumerable<Platform> platforms = Platform.GetPlatforms();

            Device chosenDevice = platforms.FirstOrDefault()?.GetDevices(DeviceType.All).FirstOrDefault();

            _context = Context.CreateContext(chosenDevice);
            Device CLDevice = chosenDevice;
            _commandQueue = CommandQueue.CreateCommandQueue(_context, CLDevice);
#endif
        }

        /// <summary>
        /// Creates a CL Program from source
        /// </summary>
        /// <param name="source">the source of the program</param>
        /// <returns>The CL Program that was created from the code.</returns>
        internal static Program CreateCLProgramFromSource(string source)
        {
#if NO_CL
            Logger.Log("Creating CL Program", DebugChannel.Warning);
            return null;
#else
            return Instance._context.CreateAndBuildProgramFromString(source);
#endif
        }

        /// <summary>
        /// Creates a CL Program from source
        /// </summary>
        /// <param name="source">the source of the program</param>
        /// <returns>The CL Program that was created from the code.</returns>
        internal static Program CreateCLProgramFromSource(string[] source)
        {
#if NO_CL
            Logger.Log("Creating CL Program", DebugChannel.Warning);
            return null;
#else
            return Instance._context.CreateAndBuildProgramFromString(source);
#endif
        }

        /// <summary>
        /// Creates a CL Kernel from name
        /// </summary>
        /// <param name="program">The program that contains the kernel</param>
        /// <param name="name">The name of the kernel</param>
        /// <returns>The Compiled and Linked Kernel</returns>
        internal static Kernel CreateKernelFromName(Program program, string name)
        {
#if NO_CL
            Logger.Log("Creating CL Kernel From Name", DebugChannel.Warning);
            return null;
#else
            return program.CreateKernel(name);
#endif
        }

        /// <summary>
        /// Creates an empty buffer of type T with the specified size and MemoryFlags
        /// </summary>
        /// <typeparam name="T">The type of the struct</typeparam>
        /// <param name="size">The size of the buffer(Total size in bytes: size*sizeof(T)</param>
        /// <param name="flags">The memory flags for the buffer creation</param>
        /// <returns></returns>
        public static MemoryBuffer CreateEmpty<T>(int size, MemoryFlag flags) where T : struct
        {
            T[] arr = new T[size];
            return CreateBuffer(arr, flags);
        }

        /// <summary>
        /// Creates a Buffer with the specified content and Memory Flags
        /// </summary>
        /// <typeparam name="T">Type of the struct</typeparam>
        /// <param name="data">The array of T</param>
        /// <param name="flags">The memory flags for the buffer creation</param>
        /// <returns></returns>
        public static MemoryBuffer CreateBuffer<T>(T[] data, MemoryFlag flags) where T : struct
        {
            object[] arr = Array.ConvertAll(data, x => (object) x);
            return CreateBuffer(arr, typeof(T), flags);
        }

        /// <summary>
        /// Creates a buffer with the specified content and memory flags
        /// </summary>
        /// <param name="data">the array of objects</param>
        /// <param name="t">type of the objects in the data array</param>
        /// <param name="flags">The memory flags for the buffer creation</param>
        /// <returns></returns>
        public static MemoryBuffer CreateBuffer(object[] data, Type t, MemoryFlag flags)
        {
#if NO_CL
            Logger.Log("Creating CL Buffer of Type: " + t, DebugChannel.Warning);
            return null;
#else


            MemoryBuffer mb =
                Instance._context.CreateBuffer((MemoryFlag) (flags | MemoryFlag.CopyHostPointer), t, data);

            return mb;
#endif
        }

        /// <summary>
        /// Creates a buffer with the content of an image and the specified Memory Flags
        /// </summary>
        /// <param name="bmp">The image that holds the data</param>
        /// <param name="flags">The memory flags for the buffer creation</param>
        /// <returns></returns>
        public static MemoryBuffer CreateFromImage(Bitmap bmp, MemoryFlag flags)
        {
            bmp.RotateFlip(RotateFlipType.RotateNoneFlipY);

            BitmapData data = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadOnly,
                PixelFormat.Format32bppArgb);
            byte[] buffer = new byte[bmp.Width * bmp.Height * 4];
            Marshal.Copy(data.Scan0, buffer, 0, buffer.Length);
            bmp.UnlockBits(data);
#if NO_CL
            Logger.Log("Creating CL Buffer from Image", DebugChannel.Warning);
            return null;
#else
            MemoryBuffer mb = CreateBuffer(buffer, flags);
            return mb;
#endif
        }


        /// <summary>
        /// A Delegate to create random numbers for every data type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns> a random value of type T</returns>
        public delegate T RandomFunc<out T>() where T : struct;

        /// <summary>
        /// Creates an array with random values
        /// </summary>
        /// <typeparam name="T">Type of the values</typeparam>
        /// <param name="size">the size of the array</param>
        /// <param name="channelEnableState">the channels that are enables(aka. get written with bytes)</param>
        /// <param name="rnd">the RandomFunc delegate providing the random numbers.</param>
        /// <param name="uniform">Should every channel receive the same value on the same pixel?</param>
        /// <returns>An array filled with random values of type T</returns>
        public static T[] CreateRandom<T>(int size, byte[] channelEnableState, RandomFunc<T> rnd, bool uniform)
            where T : struct
        {
            T[] buffer = new T[size];
            WriteRandom(buffer, channelEnableState, rnd, uniform);
            return buffer;
        }

        /// <summary>
        /// Creates an array with random values
        /// </summary>
        /// <typeparam name="T">Type of the values</typeparam>
        /// <param name="size">the size of the array</param>
        /// <param name="channelEnableState">the channels that are enables(aka. get written with bytes)</param>
        /// <param name="rnd">the RandomFunc delegate providing the random numbers.</param>
        /// <returns>An array filled with random values of type T</returns>
        public static T[] CreateRandom<T>(int size, byte[] channelEnableState, RandomFunc<T> rnd) where T : struct
        {
            return CreateRandom(size, channelEnableState, rnd, true);
        }


        /// <summary>
        /// Writes random values to an array
        /// </summary>
        /// <typeparam name="T">Type of the values</typeparam>
        /// <param name="buffer">Array containing the values to overwrite</param>
        /// <param name="channelEnableState">the channels that are enables(aka. get written with bytes)</param>
        /// <param name="rnd">the RandomFunc delegate providing the random numbers.</param>
        /// <param name="uniform">Should every channel receive the same value on the same pixel?</param>
        public static void WriteRandom<T>(T[] buffer, byte[] channelEnableState, RandomFunc<T> rnd,
            bool uniform) where T : struct
        {
            T val = rnd.Invoke();
            for (int i = 0; i < buffer.Length; i++)
            {
                int channel = i % channelEnableState.Length;
                if (channel == 0 || !uniform)
                {
                    val = rnd.Invoke();
                }

                if (channelEnableState[channel] == 1)
                {
                    buffer[i] = val;
                }
            }
        }

        /// <summary>
        /// Writes random values to an array
        /// </summary>
        /// <typeparam name="T">Type of the values</typeparam>
        /// <param name="buffer">Array containing the values to overwrite</param>
        /// <param name="channelEnableState">the channels that are enables(aka. get written with bytes)</param>
        /// <param name="rnd">the RandomFunc delegate providing the random numbers.</param>
        public static void WriteRandom<T>(T[] buffer, byte[] channelEnableState, RandomFunc<T> rnd) where T : struct
        {
            WriteRandom(buffer, channelEnableState, rnd, true);
        }

        /// <summary>
        /// Writes random values to a MemoryBuffer
        /// </summary>
        /// <typeparam name="T">Type of the values</typeparam>
        /// <param name="buf">MemoryBuffer containing the values to overwrite</param>
        /// <param name="rnd">the RandomFunc delegate providing the random numbers.</param>
        /// <param name="enabledChannels">the channels that are enables(aka. get written with bytes)</param>
        /// <param name="uniform">Should every channel receive the same value on the same pixel?</param>
        public static void WriteRandom<T>(MemoryBuffer buf, RandomFunc<T> rnd, byte[] enabledChannels, bool uniform)
            where T : struct
        {
#if NO_CL
            T[] data = new T[1];
#else

            MemoryBuffer buffer = buf;

            T[] data = Instance._commandQueue.EnqueueReadBuffer<T>(buffer, (int) buffer.Size);
#endif

            WriteRandom(data, enabledChannels, rnd, uniform);

#if NO_CL
            Logger.Log("Writing Random Data to Buffer", DebugChannel.Warning);
#else
            Instance._commandQueue.EnqueueWriteBuffer(buffer, data);
#endif
        }

        /// <summary>
        /// Writes random values to a Memory Buffer
        /// </summary>
        /// <typeparam name="T">Type of the values</typeparam>
        /// <param name="buf">MemoryBuffer containing the values to overwrite</param>
        /// <param name="rnd">the RandomFunc delegate providing the random numbers.</param>
        /// <param name="enabledChannels">the channels that are enables(aka. get written with bytes)</param>
        public static void WriteRandom<T>(MemoryBuffer buf, RandomFunc<T> rnd, byte[] enabledChannels)
            where T : struct
        {
            WriteRandom(buf, rnd, enabledChannels, true);
        }


        /// <summary>
        /// Writes values to a MemoryBuffer
        /// </summary>
        /// <typeparam name="T">Type of the values</typeparam>
        /// <param name="buf">MemoryBuffer containing the values to overwrite</param>
        /// <param name="values">The values to be written to the buffer</param>
        public static void WriteToBuffer<T>(MemoryBuffer buf, T[] values) where T : struct
        {
#if NO_CL
            Logger.Log("Writing To Buffer..", DebugChannel.Warning);
#else
            Instance._commandQueue.EnqueueWriteBuffer(buf, values);
#endif
        }

        /// <summary>
        /// Writes values to a MemoryBuffer
        /// </summary>
        /// <typeparam name="T">Type of the values</typeparam>
        /// <param name="buf">MemoryBuffer containing the values to overwrite</param>
        /// <param name="size">The count of structs to be read from the buffer</param>
        /// <returns>The content of the buffer</returns>
        public static T[] ReadBuffer<T>(MemoryBuffer buf, int size) where T : struct
        {
#if NO_CL
            Logger.Log("Reading From Buffer..", DebugChannel.Warning);
            return new T[size];
#else
            return Instance._commandQueue.EnqueueReadBuffer<T>(buf, size);
#endif
        }

        /// <summary>
        /// Runs a kernel with a valid FL kernel signature
        /// </summary>
        /// <param name="kernel">The CLKernel to be executed</param>
        /// <param name="image">The image buffer that serves as input</param>
        /// <param name="dimensions">The dimensions of the input buffer</param>
        /// <param name="genTypeMaxVal">The max valuee of the generic type that is used.(byte = 255)</param>
        /// <param name="enabledChannels">The enabled channels for the kernel</param>
        /// <param name="channelCount">The amount of active channels.</param>
        public static void Run(CLKernel kernel, MemoryBuffer image, int3 dimensions, float genTypeMaxVal,
            MemoryBuffer enabledChannels,
            int channelCount)
        {
#if NO_CL
            Logger.Log("Running CL Kernel: " + kernel.Name, DebugChannel.Warning);
#else
            kernel.Run(Instance._commandQueue, image, dimensions, genTypeMaxVal, enabledChannels, channelCount);
#endif
        }
    }
}