using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using Common;
using OpenCl.DotNetCore.CommandQueues;
using OpenCl.DotNetCore.Contexts;
using OpenCl.DotNetCore.DataTypes;
using OpenCl.DotNetCore.Devices;
using OpenCl.DotNetCore.Kernels;
using OpenCl.DotNetCore.Memory;
using OpenCl.DotNetCore.Platforms;
using OpenCl.DotNetCore.Programs;

namespace CLHelperLibrary
{
    public class CL
    {
        private static CL _instance;
        private static CL Instance => _instance ?? (_instance = new CL());

        private Context _context;
        private CommandQueue _commandQueue;
        private CL()
        {
            InitializeOpenCL();
        }

        private void InitializeOpenCL()
        {
#if NO_CL
            this.Log("Starting in NO_CL Mode", DebugChannel.Warning);
#else
            IEnumerable<Platform> platforms = Platform.GetPlatforms();

            Device chosenDevice = platforms.FirstOrDefault().GetDevices(DeviceType.All).FirstOrDefault();

            _context = Context.CreateContext(chosenDevice);
            Device CLDevice = chosenDevice;
            _commandQueue = CommandQueue.CreateCommandQueue(_context, CLDevice);
#endif
        }


        internal static Program CreateCLProgramFromSource(string source)
        {
#if NO_CL
            source.Log("Creating CL Program", DebugChannel.Warning);
            return null;
#else
            return Instance._context.CreateAndBuildProgramFromString(source);
#endif
        }

        internal static Program CreateCLProgramFromSource(string[] source)
        {
#if NO_CL
            source.Log("Creating CL Program", DebugChannel.Warning);
            return null;
#else
            return Instance._context.CreateAndBuildProgramFromString(source);
#endif
        }

        internal static Kernel CreateKernelFromName(Program program, string name)
        {
#if NO_CL
            name.Log("Creating CL Kernel From Name", DebugChannel.Warning);
            return null;
#else
            return program.CreateKernel(name);
#endif
        }


        public static MemoryBuffer CreateEmpty<T>(int size, MemoryFlag flags) where T : struct
        {
            T[] arr = new T[size];
            return CreateBuffer(arr, flags);
        }

        public static MemoryBuffer CreateBuffer<T>(T[] data, MemoryFlag flags) where T : struct
        {
            object[] arr = Array.ConvertAll(data, x => (object)x);
            return CreateBuffer(arr, typeof(T), flags);
        }
        public static MemoryBuffer CreateBuffer(object[] data, Type t, MemoryFlag flags)
        {
#if NO_CL
            data.Log("Creating CL Buffer of Type: " + t, DebugChannel.Warning);
            return null;
#else
            MemoryBuffer mb = Instance._context.CreateBuffer(flags, t, data);
            return mb;
#endif

        }

        public static MemoryBuffer CreateFromImage(Bitmap bmp, MemoryFlag flags)
        {
            bmp.RotateFlip(RotateFlipType.RotateNoneFlipY);

            BitmapData data = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadOnly,
                PixelFormat.Format32bppArgb);
            byte[] buffer = new byte[bmp.Width * bmp.Height * 4];
            Marshal.Copy(data.Scan0, buffer, 0, buffer.Length);
            bmp.UnlockBits(data);
#if NO_CL
            bmp.Log("Creating CL Buffer from Image", DebugChannel.Warning);
            return null;
#else
            MemoryBuffer mb = CreateBuffer(buffer, flags);
            return mb;
#endif
        }

        public delegate T RandomFunc<out T>() where T : struct;
        public static T[] CreateRandom<T>(int size, byte[] channelEnableState, RandomFunc<T> rnd, bool uniform) where T : struct
        {
            T[] buffer = new T[size];
            WriteRandom(buffer, channelEnableState, rnd, uniform);
            return buffer;
        }

        public static T[] CreateRandom<T>(int size, byte[] channelEnableState, RandomFunc<T> rnd) where T : struct
        {
            return CreateRandom(size, channelEnableState, rnd, true);
        }

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

        public static void WriteRandom<T>(T[] buffer, byte[] channelEnableState, RandomFunc<T> rnd) where T : struct
        {
            WriteRandom(buffer, channelEnableState, rnd, true);
        }

        public static void WriteRandom<T>(MemoryBuffer buf, RandomFunc<T> rnd, byte[] enabledChannels, bool uniform) where T : struct
        {
#if NO_CL
            T[] data = new T[1];
#else
            T[] data = Instance._commandQueue.EnqueueReadBuffer<T>(buf, (int)buf.Size);
#endif

            WriteRandom(data, enabledChannels, rnd, uniform);

#if NO_CL
            enabledChannels.Log("Writing Random Data to Buffer", DebugChannel.Warning);
#else
            Instance._commandQueue.EnqueueWriteBuffer(buf, data);
#endif
        }

        public static void WriteRandom<T>(MemoryBuffer buf, RandomFunc<T> rnd, byte[] enabledChannels)
            where T : struct
        {
            WriteRandom(buf, rnd, enabledChannels, true);
        }



        public static void WriteToBuffer<T>(MemoryBuffer buf, T[] values) where T : struct
        {
#if NO_CL
            values.Log("Writing To Buffer..", DebugChannel.Warning);
#else
            Instance._commandQueue.EnqueueWriteBuffer<T>(buf, values);
#endif
        }

        public static T[] ReadBuffer<T>(MemoryBuffer buf, int size) where T : struct
        {
#if NO_CL
            size.Log("Reading From Buffer..", DebugChannel.Warning);
            return new T[size];
#else
            return Instance._commandQueue.EnqueueReadBuffer<T>(buf, size);
#endif
        }

        public static void Run(CLKernel kernel, MemoryBuffer image, int3 dimensions, MemoryBuffer enabledChannels,
            int channelCount)
        {
#if NO_CL
            kernel.Log("Running CL Kernel: " + kernel.Name, DebugChannel.Warning);
#else
            kernel.Run(Instance._commandQueue, image, dimensions, enabledChannels, channelCount);
#endif
        }

    }
}