#region Using Directives

using System;
using System.Runtime.InteropServices;
using Engine.Debug;
using Engine.OpenCL.DotNetCore.Interop;
using Engine.OpenCL.DotNetCore.Interop.Memory;

#endregion

namespace Engine.OpenCL.DotNetCore.Memory
{
    /// <summary>
    /// Represents an OpenCL memory object.
    /// </summary>
    public abstract class MemoryObject : HandleBase
    {
        #region Constructors

        /// <summary>
        /// Initializes a new <see cref="MemoryObject"/> instance.
        /// </summary>
        /// <param name="handle">The handle to the OpenCL memory object.</param>
        internal MemoryObject(IntPtr handle, long bytes)
            : base(handle)
        {
            this.bytes = bytes;
        }

        private long bytes;

        #endregion

        #region Public Properties

        /// <summary>
        /// Contains the size of the contents of the memory object in bytes.
        /// </summary>
        private Nullable<long> size;

        /// <summary>
        /// Gets the size of the contents of the memory object in bytes.
        /// </summary>
        public long Size
        {
            get
            {
                if (!size.HasValue)
                {
                    if (Marshal.SizeOf<IntPtr>() == sizeof(long))
                    {
                        size = GetMemoryObjectInformation<long>(MemoryObjectInformation.Size);
                    }
                    else
                    {
                        size = (long) GetMemoryObjectInformation<int>(MemoryObjectInformation.Size);
                    }
                }

                return size.Value;
            }
        }

        /// <summary>
        /// Contains the flags with which the memory object was created.
        /// </summary>
        private Nullable<MemoryFlag> flags;

        /// <summary>
        /// Gets the flags with which the memory object was created.
        /// </summary>
        public MemoryFlag Flags
        {
            get
            {
                if (!flags.HasValue)
                {
                    flags = (MemoryFlag) GetMemoryObjectInformation<ulong>(MemoryObjectInformation.Flags);
                }

                return flags.Value;
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Retrieves the specified information about the OpenCL memory object.
        /// </summary>
        /// <typeparam name="T">The type of the data that is to be returned.</param>
        /// <param name="memoryObjectInformation">The kind of information that is to be retrieved.</param>
        /// <exception cref="OpenClException">If the information could not be retrieved, then an <see cref="OpenClException"/> is thrown.</exception>
        /// <returns>Returns the specified information.</returns>
        private T GetMemoryObjectInformation<T>(MemoryObjectInformation memoryObjectInformation)
        {
            // Retrieves the size of the return value in bytes, this is used to later get the full information
            UIntPtr returnValueSize;
            Result result = MemoryNativeApi.GetMemoryObjectInformation(Handle, memoryObjectInformation, UIntPtr.Zero,
                null,
                out returnValueSize);
            if (result != Result.Success)
            {
                throw new OpenClException("The memory object information could not be retrieved.", result);
            }

            // Allocates enough memory for the return value and retrieves it
            byte[] output = new byte[returnValueSize.ToUInt32()];
            result = MemoryNativeApi.GetMemoryObjectInformation(Handle, memoryObjectInformation,
                new UIntPtr((uint) output.Length), output, out returnValueSize);
            if (result != Result.Success)
            {
                throw new OpenClException("The memory object information could not be retrieved.", result);
            }

            // Returns the output
            return InteropConverter.To<T>(output);
        }

        #endregion

        #region IDisposable Implementation

        /// <summary>
        /// Disposes of the resources that have been acquired by the memory object.
        /// </summary>
        /// <param name="disposing">Determines whether managed object or managed and unmanaged resources should be disposed of.</param>
        protected override void Dispose(bool disposing)
        {
            // Checks if the memory object has already been disposed of, if not, then the memory object is disposed of
            if (!IsDisposed)
            {
                EngineStatisticsManager.CLObjectDestroyed(bytes);
                MemoryNativeApi.ReleaseMemoryObject(Handle);
            }

            // Makes sure that the base class can execute its dispose logic
            base.Dispose(disposing);
        }

        #endregion
    }
}