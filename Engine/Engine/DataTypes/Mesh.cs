using System;
using Engine.Debug;
using OpenTK.Graphics.OpenGL;

namespace Engine.DataTypes
{
    /// <summary>
    /// A Data Type that is containing the information that is needed to store a Mesh in OpenGL
    /// </summary>
    public class Mesh : IDisposable
    {
        /// <summary>
        /// Element Buffer Object used to store the Indices of the Mesh Vertexes
        /// </summary>
        public readonly int Ebo;

        /// <summary>
        /// The Buffer used to connect _ebo, _vbo.
        /// In Theory you only need this buffer, but it is convenient to have the others sticking around as well.
        /// </summary>
        public readonly int Vao;

        /// <summary>
        /// The Buffer used to store the actual vertex data
        /// </summary>
        public readonly int Vbo;

        private readonly long bytes;

        /// <summary>
        /// The amount of indices to draw(should be always indices.size)
        /// </summary>
        public readonly int DrawCount;

        /// <summary>
        /// Private flag that is used to prevent deleing the buffers more than once.
        /// </summary>
        private bool disposed;

        /// <summary>
        /// Flag that is set to true by the Copy function which is used to not dispose the mesh data
        /// </summary>
        private bool dontDispose;

        /// <summary>
        /// Internal Constructor to create a Mesh Data object.
        /// </summary>
        internal Mesh(int ebo, int vbo, int vao, int drawCount, long bytes)
        {
            this.bytes = bytes;
            Ebo = ebo;
            Vbo = vbo;
            Vao = vao;
            DrawCount = drawCount;
        }


        /// <summary>
        /// Disposable implementation to free the GL Mesh Buffers once they are no longer needed.
        /// </summary>
        public void Dispose()
        {
            if (disposed || dontDispose)
            {
                return;
            }

            EngineStatisticsManager.GlObjectDestroyed(bytes);
            disposed = true;
            GL.DeleteBuffer(Ebo);
            GL.DeleteBuffer(Vbo);
            GL.DeleteVertexArray(Vao);
        }

        /// <summary>
        /// Returns a Copy of this Mesh(It Does not copy the buffers on the GPU)
        /// It is save to Dispose the copied object.
        /// BUT NEVER DISPOSE THE SOURCE OBJECT BEFORE ALL COPIES HAVE BEEN DISPOSED, otherwise they become unusable and will probably crash the engine once interacted with
        /// </summary>
        /// <returns>A Copy of this Mesh Object</returns>
        public Mesh Copy()
        {
            return new Mesh(Ebo, Vbo, Vao, DrawCount, bytes)
            {
                dontDispose = true
            };
        }
    }
}