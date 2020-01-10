using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using Engine.Debug;
using Engine.Exceptions;
using Engine.OpenCL.DotNetCore.CommandQueues;
using Engine.OpenCL.DotNetCore.Contexts;
using Engine.OpenCL.DotNetCore.DataTypes;
using Engine.OpenCL.DotNetCore.Devices;
using Engine.OpenCL.DotNetCore.Kernels;
using Engine.OpenCL.DotNetCore.Memory;
using Engine.OpenCL.DotNetCore.Platforms;
using Engine.OpenCL.DotNetCore.Programs;

namespace Engine.OpenCL
{
    /// <summary>
    /// A wrapper class that is handling all the CL operations.
    /// </summary>
    public class Clapi : IDisposable
    {
        /// <summary>
        /// A Delegate to create random numbers for every data type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns> a random value of type T</returns>
        public delegate T RandomFunc<out T>() where T : struct;

        /// <summary>
        /// Field that holds the instance of the CL wrapper
        /// </summary>
        private static Clapi _instance;

        /// <summary>
        /// The Command queue that the wrapper is using
        /// </summary>
        private CommandQueue commandQueue;

        /// <summary>
        /// The CL Context that the wrapper is using
        /// </summary>
        private Context context;

        /// <summary>
        /// Private constructor
        /// </summary>
        private Clapi()
        {
            InitializeOpenCl();
        }

        /// <summary>
        /// Helpful property for initializing the singleton
        /// </summary>
        public static Clapi MainThread => _instance ?? (_instance = new Clapi());

        private static void ApiDisposed(Clapi obj)
        {
            if (obj == _instance)
            {
                _instance = null;
            }
        }

        public void Dispose()
        {
            ApiDisposed(this);


            context.Dispose();
            commandQueue.Dispose();
        }

        /// <summary>
        /// Returns the Command queue(dont use, its just for debugging if something is wrong)
        /// </summary>
        /// <returns>The Internal Command queue</returns>
        internal static CommandQueue GetQueue(Clapi instance)
        {
            return instance.commandQueue;
        }

        internal static Clapi GetInstance()
        {
            return new Clapi();
        }

        /// <summary>
        /// Reinitializes the CL API
        /// Used by The unit tests when requireing actual CL api calls(e.g. not in NO_CL mode)
        /// </summary>
        public static void Reinitialize()
        {
            _instance = new Clapi();
        }

        /// <summary>
        /// Initializes the OpenCL API
        /// </summary>
        private void InitializeOpenCl()
        {

            IEnumerable<Platform> platforms = Platform.GetPlatforms();
            List<Device> devs = new List<Device>();
            for (int i = 0; i < platforms.Count(); i++)
            {
                IEnumerable<Device> ds = platforms.ElementAt(i).GetDevices(DeviceType.All);

                for (int j = 0; j < ds.Count(); j++)
                {
                    Console.WriteLine("Adding Device: " + ds.ElementAt(j).Name + "@" + ds.ElementAt(j).Vendor);
                    devs.Add(ds.ElementAt(j));
                }

            }

            Device chosenDevice = null;

            for (int i = 0; i < devs.Count; i++)
            {
                if (devs[i].IsAvailable)
                {
                    Console.WriteLine("Choosing Device: " + devs[i].Name + "@" + devs[i].Vendor);
                    chosenDevice = devs[i];
                    break;
                }
            }

            if (chosenDevice == null)
            {
                throw new Exception("Could not Get Device. Total Devices: " + devs.Count);
            }

            try
            {
                context = Context.CreateContext(chosenDevice);
                commandQueue = CommandQueue.CreateCommandQueue(context, chosenDevice);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw new Exception("Could not Create Context with Device: " + chosenDevice.Name + "@" + chosenDevice.Vendor, e);
            }

        }


        /// <summary>
        /// Creates a CL Kernel from name
        /// </summary>
        /// <param name="program">The program that contains the kernel</param>
        /// <param name="name">The name of the kernel</param>
        /// <returns>The Compiled and Linked Kernel</returns>
        internal static Kernel CreateKernelFromName(Program program, string name)
        {

            if (program == null)
            {
                return null;
            }

            return program.CreateKernel(name);
        }

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
        /// <param name="instance">Clapi Instance for the current thread</param>
        /// <param name="buf">MemoryBuffer containing the values to overwrite</param>
        /// <param name="rnd">the RandomFunc delegate providing the random numbers.</param>
        /// <param name="enabledChannels">the channels that are enables(aka. get written with bytes)</param>
        /// <param name="uniform">Should every channel receive the same value on the same pixel?</param>
        public static void WriteRandom<T>(Clapi instance, MemoryBuffer buf, RandomFunc<T> rnd, byte[] enabledChannels,
            bool uniform)
            where T : struct
        {

            MemoryBuffer buffer = buf;

            T[] data = instance.commandQueue.EnqueueReadBuffer<T>(buffer, (int)buffer.Size);


            WriteRandom(data, enabledChannels, rnd, uniform);

            instance.commandQueue.EnqueueWriteBuffer(buffer, data);
        }

        /// <summary>
        /// Writes random values to a Memory Buffer
        /// </summary>
        /// <typeparam name="T">Type of the values</typeparam>
        /// <param name="instance">Clapi Instance for the current thread</param>
        /// <param name="buf">MemoryBuffer containing the values to overwrite</param>
        /// <param name="rnd">the RandomFunc delegate providing the random numbers.</param>
        /// <param name="enabledChannels">the channels that are enables(aka. get written with bytes)</param>
        public static void WriteRandom<T>(Clapi instance, MemoryBuffer buf, RandomFunc<T> rnd, byte[] enabledChannels)
            where T : struct
        {
            WriteRandom(instance, buf, rnd, enabledChannels, true);
        }


        /// <summary>
        /// Writes values to a MemoryBuffer
        /// </summary>
        /// <typeparam name="T">Type of the values</typeparam>
        /// <param name="instance">Clapi Instance for the current thread</param>
        /// <param name="buf">MemoryBuffer containing the values to overwrite</param>
        /// <param name="values">The values to be written to the buffer</param>
        public static void WriteToBuffer<T>(Clapi instance, MemoryBuffer buf, T[] values) where T : struct
        {
            instance.commandQueue.EnqueueWriteBuffer(buf, values);
        }

        /// <summary>
        /// Writes values to a MemoryBuffer
        /// </summary>
        /// <typeparam name="T">Type of the values</typeparam>
        /// <param name="instance">Clapi Instance for the current thread</param>
        /// <param name="buf">MemoryBuffer containing the values to overwrite</param>
        /// <param name="size">The count of structs to be read from the buffer</param>
        /// <returns>The content of the buffer</returns>
        public static T[] ReadBuffer<T>(Clapi instance, MemoryBuffer buf, int size) where T : struct
        {
            return instance.commandQueue.EnqueueReadBuffer<T>(buf, size);
        }

        /// <summary>
        /// Runs a kernel with a valid FL kernel signature
        /// </summary>
        /// <param name="instance">Clapi Instance for the current thread</param>
        /// <param name="kernel">The CLKernel to be executed</param>
        /// <param name="image">The image buffer that serves as input</param>
        /// <param name="dimensions">The dimensions of the input buffer</param>
        /// <param name="genTypeMaxVal">The max valuee of the generic type that is used.(byte = 255)</param>
        /// <param name="enabledChannels">The enabled channels for the kernel</param>
        /// <param name="channelCount">The amount of active channels.</param>
        public static void Run(Clapi instance, CLKernel kernel, MemoryBuffer image, int3 dimensions,
            float genTypeMaxVal,
            MemoryBuffer enabledChannels,
            int channelCount)
        {
            kernel.Run(instance.commandQueue, image, dimensions, genTypeMaxVal, enabledChannels, channelCount);
        }

        public static void Run(Clapi instance, CLKernel kernel, int groupSize)
        {
            kernel.Run(instance.commandQueue, 1, groupSize);
        }

        #region Instance Functions

        internal static Program CreateClProgramFromSource(Clapi instance, string source)
        {

            try
            {
                return instance.context.CreateAndBuildProgramFromString(source);
            }
            catch (Exception e)
            {
                Logger.Crash(new CLProgramException("Could not compile file", e), true);
                return null;
            }

        }

        internal static Program CreateClProgramFromSource(Clapi instance, string[] source)
        {
            return instance.context.CreateAndBuildProgramFromString(source);
        }

        /// <summary>
        /// Creates an empty buffer of type T with the specified size and MemoryFlags
        /// </summary>
        /// <typeparam name="T">The type of the struct</typeparam>
        /// <param name="instance">Clapi Instance for the current thread</param>
        /// <param name="size">The size of the buffer(Total size in bytes: size*sizeof(T)</param>
        /// <param name="flags">The memory flags for the buffer creation</param>
        /// <returns></returns>
        public static MemoryBuffer CreateEmpty<T>(Clapi instance, int size, MemoryFlag flags) where T : struct
        {
            T[] arr = new T[size];
            return CreateBuffer(instance, arr, flags);
        }

        /// <summary>
        /// Creates a Buffer with the specified content and Memory Flags
        /// </summary>
        /// <typeparam name="T">Type of the struct</typeparam>
        /// <param name="instance">Clapi Instance for the current thread</param>
        /// <param name="data">The array of T</param>
        /// <param name="flags">The memory flags for the buffer creation</param>
        /// <returns></returns>
        public static MemoryBuffer CreateBuffer<T>(Clapi instance, T[] data, MemoryFlag flags) where T : struct
        {
            object[] arr = Array.ConvertAll(data, x => (object)x);
            return CreateBuffer(instance, arr, typeof(T), flags);
        }

        public static MemoryBuffer CreateBuffer(Clapi instance, object[] data, Type t, MemoryFlag flags)
        {

            long bytes = data.Length * Marshal.SizeOf(data[0]);
            EngineStatisticsManager.ClObjectCreated(bytes);
            MemoryBuffer mb =
                instance.context.CreateBuffer(flags | MemoryFlag.CopyHostPointer, t, data);

            return mb;
        }

        /// <summary>
        /// Creates a buffer with the content of an image and the specified Memory Flags
        /// </summary>
        /// <param name="instance">Clapi Instance for the current thread</param>
        /// <param name="bmp">The image that holds the data</param>
        /// <param name="flags">The memory flags for the buffer creation</param>
        /// <returns></returns>
        public static MemoryBuffer CreateFromImage(Clapi instance, Bitmap bmp, MemoryFlag flags)
        {
            bmp.RotateFlip(RotateFlipType.RotateNoneFlipY);

            BitmapData data = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadOnly,
                PixelFormat.Format32bppArgb);
            byte[] buffer = new byte[bmp.Width * bmp.Height * 4];
            Marshal.Copy(data.Scan0, buffer, 0, buffer.Length);
            bmp.UnlockBits(data);

            MemoryBuffer mb = CreateBuffer(instance, buffer, flags);
            return mb;
        }

        #endregion
    }
}