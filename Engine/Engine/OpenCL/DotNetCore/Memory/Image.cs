#region Using Directives

using System;

#endregion

namespace Engine.OpenCL.DotNetCore.Memory
{
    /// <summary>
    /// Represents an OpenCL image.
    /// </summary>
    public class Image : MemoryObject
    {
        #region Constructors

        /// <summary>
        /// Initializes a new <see cref="Image"/> instance.
        /// </summary>
        /// <param name="handle">The handle to the OpenCL image.</param>
        ///<param name="bytes">Size of the Memory Object(For Statistics)</param>
        public Image(IntPtr handle, long bytes)
            : base(handle, bytes)
        {
        }

        #endregion
    }
}