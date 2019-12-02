#region Using Directives

using System;
using System.Collections.Generic;
using System.Linq;
using Engine.OpenCL.DotNetCore.Interop;
using Engine.OpenCL.DotNetCore.Interop.Devices;

#endregion

namespace Engine.OpenCL.DotNetCore.Devices
{
    /// <summary>
    /// Represents an OpenCL device.
    /// </summary>
    public class Device : HandleBase
    {
        #region Constructors

        /// <summary>
        /// Initializes a new <see cref="Device"/> instance.
        /// </summary>
        /// <param name="handle">The handle to the OpenCL device.</param>
        internal Device(IntPtr handle)
            : base(handle)
        {
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Retrieves the specified information about the device.
        /// </summary>
        /// <typeparam name="T">The type of the data that is to be returned.</param>
        /// <param name="deviceInformation">The kind of information that is to be retrieved.</param>
        /// <exception cref="OpenClException">If the information could not be retrieved, then an <see cref="OpenClException"/> is thrown.</exception>
        /// <returns>Returns the specified information.</returns>
        public T GetDeviceInformation<T>(DeviceInformation deviceInformation)
        {
            // Retrieves the size of the return value in bytes, this is used to later get the full information
            UIntPtr returnValueSize;
            Result result =
                DevicesNativeApi.GetDeviceInformation(Handle, deviceInformation, UIntPtr.Zero, null,
                    out returnValueSize);
            if (result != Result.Success)
            {
                throw new OpenClException("The device information could not be retrieved.", result);
            }

            // Allocates enough memory for the return value and retrieves it
            byte[] output = new byte[returnValueSize.ToUInt32()];
            result = DevicesNativeApi.GetDeviceInformation(Handle, deviceInformation, new UIntPtr((uint) output.Length),
                output, out returnValueSize);
            if (result != Result.Success)
            {
                throw new OpenClException("The device information could not be retrieved.", result);
            }

            // Returns the output
            return InteropConverter.To<T>(output);
        }

        #endregion

        #region IDisposable Implementation

        /// <summary>
        /// Disposes of the resources that have been acquired by the command queue.
        /// </summary>
        /// <param name="disposing">Determines whether managed object or managed and unmanaged resources should be disposed of.</param>
        protected override void Dispose(bool disposing)
        {
            // Checks if the device has already been disposed of, if not, then the device is disposed of
            if (!IsDisposed)
            {
                DevicesNativeApi.ReleaseDevice(Handle);
            }

            // Makes sure that the base class can execute its dispose logic
            base.Dispose(disposing);
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Contains the name of the device.
        /// </summary>
        private string name;

        /// <summary>
        /// Gets the name of the device.
        /// </summary>
        public string Name
        {
            get
            {
                if (string.IsNullOrWhiteSpace(name))
                {
                    name = GetDeviceInformation<string>(DeviceInformation.Name);
                }

                return name;
            }
        }

        /// <summary>
        /// Contains the name of the vendor of the device.
        /// </summary>
        private string vendor;

        /// <summary>
        /// Gets the name of the vendor of the device.
        /// </summary>
        public string Vendor
        {
            get
            {
                if (string.IsNullOrWhiteSpace(vendor))
                {
                    vendor = GetDeviceInformation<string>(DeviceInformation.Vendor);
                }

                return vendor;
            }
        }

        /// <summary>
        /// Contains the version of the device driver.
        /// </summary>
        private string driverVersion;

        /// <summary>
        /// Gets the version of the device driver
        /// </summary>
        public string DriverVersion
        {
            get
            {
                if (string.IsNullOrWhiteSpace(driverVersion))
                {
                    driverVersion = GetDeviceInformation<string>(DeviceInformation.DriverVersion);
                }

                return driverVersion;
            }
        }

        /// <summary>
        /// Contains the global memory size of the device.
        /// </summary>
        private long? globalMemorySize;

        /// <summary>
        /// Gets the global memory size of the device.
        /// </summary>
        public long GlobalMemorySize
        {
            get
            {
                if (!globalMemorySize.HasValue)
                {
                    globalMemorySize = (long) GetDeviceInformation<ulong>(DeviceInformation.GlobalMemorySize);
                }

                return globalMemorySize.Value;
            }
        }

        /// <summary>
        /// Contains the number of bits, that the device can use to address its memory.
        /// </summary>
        private int? addressBits;

        /// <summary>
        /// Gets the number of bits, that the device can use to address its memory.
        /// </summary>
        public int AddressBits
        {
            get
            {
                if (!addressBits.HasValue)
                {
                    addressBits = (int) GetDeviceInformation<uint>(DeviceInformation.AddressBits);
                }

                return addressBits.Value;
            }
        }

        /// <summary>
        /// Contains the maximum clock frequency of the device in MHz.
        /// </summary>
        private int? maximumClockFrequency;

        /// <summary>
        /// Gets the maximum clock frequency of the device in MHz.
        /// </summary>
        public int MaximumClockFrequency
        {
            get
            {
                if (!maximumClockFrequency.HasValue)
                {
                    maximumClockFrequency = (int) GetDeviceInformation<uint>(DeviceInformation.MaximumClockFrequency);
                }

                return maximumClockFrequency.Value;
            }
        }

        /// <summary>
        /// Contains a value that determines whether the device is currently available.
        /// </summary>
        private bool? isAvailable;

        /// <summary>
        /// Gets a value that determines whether the device is currently available.
        /// </summary>
        public bool IsAvailable
        {
            get
            {
                if (!isAvailable.HasValue)
                {
                    isAvailable = GetDeviceInformation<uint>(DeviceInformation.Available) == 1;
                }

                return isAvailable.Value;
            }
        }

        /// <summary>
        /// Contains a list of all the built-in kernels.
        /// </summary>
        private IEnumerable<string> builtInKernels;

        /// <summary>
        /// Gets a list of all the built-in kernels.
        /// </summary>
        public IEnumerable<string> BuiltInKernels
        {
            get
            {
                if (builtInKernels == null)
                {
                    builtInKernels = GetDeviceInformation<string>(DeviceInformation.BuiltInKernels).Split(';').ToList();
                }

                return builtInKernels;
            }
        }

        #endregion
    }
}