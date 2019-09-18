using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
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
            IEnumerable<Platform> platforms = Platform.GetPlatforms();

            Device chosenDevice = platforms.FirstOrDefault().GetDevices(DeviceType.All).FirstOrDefault();

            _context = Context.CreateContext(chosenDevice);
            Device CLDevice = chosenDevice;
            _commandQueue = CommandQueue.CreateCommandQueue(_context, CLDevice);
        }


        internal static Program CreateCLProgramFromSource(string source)
        {
            return Instance._context.CreateAndBuildProgramFromString(source);
        }

        internal static Program CreateCLProgramFromSource(string[] source)
        {
            return Instance._context.CreateAndBuildProgramFromString(source);
        }


        public static MemoryBuffer CreateEmpty<T>(int size, MemoryFlag flags) where T : struct
        {
            T[] arr = new T[size];
            return CreateBuffer(arr, flags);
        }

        public static MemoryBuffer CreateBuffer<T>(T[] data, MemoryFlag flags) where T : struct
        {
            object[] arr = Array.ConvertAll(data, x => (object)x);
            MemoryBuffer mb = Instance._context.CreateBuffer(flags, typeof(T), arr);
            return mb;
        }
        public static MemoryBuffer CreateBuffer(object[] data, Type t, MemoryFlag flags)
        {
            MemoryBuffer mb = Instance._context.CreateBuffer(flags, t, data);
            return mb;
        }

        public static MemoryBuffer CreateFromImage(Bitmap bmp, MemoryFlag flags)
        {
            bmp.RotateFlip(RotateFlipType.RotateNoneFlipY);

            BitmapData data = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadOnly,
                PixelFormat.Format32bppArgb);
            byte[] buffer = new byte[bmp.Width * bmp.Height * 4];
            Marshal.Copy(data.Scan0, buffer, 0, buffer.Length);
            bmp.UnlockBits(data);
            MemoryBuffer mb = CreateBuffer(buffer, flags);
            return mb;
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

        private static void WriteRandom<T>(T[] buffer, byte[] channelEnableState, RandomFunc<T> rnd) where T : struct
        {
            WriteRandom(buffer, channelEnableState, rnd, true);
        }

        public static void WriteRandom<T>(MemoryBuffer buf, RandomFunc<T> rnd, byte[] enabledChannels, bool uniform) where T : struct
        {
            T[] data = Instance._commandQueue.EnqueueReadBuffer<T>(buf, (int)buf.Size);
            WriteRandom(data, enabledChannels, rnd, uniform);
            Instance._commandQueue.EnqueueWriteBuffer(buf, data);
        }

        public static void WriteRandom<T>(MemoryBuffer buf, RandomFunc<T> rnd, byte[] enabledChannels)
            where T : struct
        {
            WriteRandom(buf, rnd, enabledChannels, true);
        }



        public static void WriteToBuffer<T>(MemoryBuffer buf, T[] values) where T : struct
        {
            Instance._commandQueue.EnqueueWriteBuffer<T>(buf, values);
        }

        public static T[] ReadBuffer<T>(MemoryBuffer buf, int size) where T : struct
        {
            return Instance._commandQueue.EnqueueReadBuffer<T>(buf, size);
        }

        public static void Run(CLKernel kernel, MemoryBuffer image, int3 dimensions, MemoryBuffer enabledChannels,
            int channelCount)
        {
            kernel.Run(Instance._commandQueue, image, dimensions, enabledChannels, channelCount);
        }

    }
}