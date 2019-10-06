using System;

namespace MinorEngine.BEPUutilities.ResourceManagement
{
    /// <summary>
    /// Contains locking and thread static buffer pools for the specified type.
    /// </summary>
    /// <typeparam name="T">Type of element in the buffers stored in the pools.</typeparam>
    public static class BufferPools<T>
    {
        /// <summary>
        /// Gets a buffer pool for this type which provides thread safe resource acquisition and return.</summary>
        public static LockingBufferPool<T> Locking { get; }

        [ThreadStatic] private static BufferPool<T> threadPool;

        /// <summary>
        /// Gets the pool associated with this thread.
        /// </summary>
        public static BufferPool<T> Thread => threadPool ?? (threadPool = new BufferPool<T>());

        static BufferPools()
        {
            Locking = new LockingBufferPool<T>();
        }
    }
}