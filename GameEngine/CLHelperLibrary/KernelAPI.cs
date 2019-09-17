using System;
using System.Collections.Generic;
using System.Linq;
using OpenCl.DotNetCore.CommandQueues;
using OpenCl.DotNetCore.Contexts;
using OpenCl.DotNetCore.Devices;
using OpenCl.DotNetCore.Kernels;
using OpenCl.DotNetCore.Platforms;

namespace CLHelperLibrary
{
    public class KernelAPI
    {
        private static KernelAPI _instance = null;
        private static KernelAPI Instance => _instance ?? (_instance = new KernelAPI());

        private Context _context = null;
        private Device _device = null;
        private CommandQueue _commandQueue = null;
        private KernelAPI()
        {
            InitializeOpenCL();
        }

        private void InitializeOpenCL()
        {
            IEnumerable<Platform> platforms = Platform.GetPlatforms();

            Device chosenDevice = platforms.FirstOrDefault().GetDevices(DeviceType.All).FirstOrDefault();
            Console.WriteLine($"Using: {chosenDevice.Name} ({chosenDevice.Vendor})");
            Console.WriteLine();

            _context = Context.CreateContext(chosenDevice);
            _device = chosenDevice;
            _commandQueue = CommandQueue.CreateCommandQueue(_context, _device);
        }

    }
}