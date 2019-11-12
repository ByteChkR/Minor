#region Using Directives

using System;

#endregion

namespace Engine.OpenCL.DotNetCore.Memory
{
    /// <summary>
    /// Represents an OpenCL pipe.
    /// </summary>
    public class Pipe : MemoryObject
    {
        #region Constructors

        /// <summary>
        /// Initializes a new <see cref="Pipe"/> instance.
        /// </summary>
        /// <param name="handle">The handle to the OpenCL pipe.</param>
        public Pipe(IntPtr handle, long bytes)
            : base(handle, bytes)
        {
        }

        #endregion
    }
}