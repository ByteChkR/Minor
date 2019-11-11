using System;
using System.IO;
using System.Reflection;
using Engine.Debug;
using Engine.Exceptions;
using Engine.IO;
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
        public readonly int _ebo;

        /// <summary>
        /// The Buffer used to store the actual vertex data
        /// </summary>
        public readonly int _vbo;

        /// <summary>
        /// The Buffer used to connect _ebo, _vbo.
        /// In Theory you only need this buffer, but it is convenient to have the others sticking around as well.
        /// </summary>
        public readonly int _vao;

        /// <summary>
        /// Private flag that is used to prevent deleing the buffers more than once.
        /// </summary>
        private bool _disposed;

        /// <summary>
        /// Flag that is set to true by the Copy function which is used to not dispose the mesh data
        /// </summary>
        private bool _dontDispose;

        /// <summary>
        /// The amount of indices to draw(should be always indices.size)
        /// </summary>
        public readonly int DrawCount;


        /// <summary>
        /// Internal Constructor to create a Mesh Data object.
        /// </summary>
        internal Mesh(int ebo, int vbo, int vao, int drawCount)
        {
            _ebo = ebo;
            _vbo = vbo;
            _vao = vao;
            DrawCount = drawCount;
        }

        /// <summary>
        /// Returns a Copy of this Mesh(It Does not copy the buffers on the GPU)
        /// It is save to Dispose the copied object.
        /// BUT NEVER DISPOSE THE SOURCE OBJECT BEFORE ALL COPIES HAVE BEEN DISPOSED, otherwise they become unusable and will probably crash the engine once interacted with
        /// </summary>
        /// <returns>A Copy of this Mesh Object</returns>
        public Mesh Copy()
        {
            return new Mesh(_ebo, _vbo, _vao, DrawCount)
            {
                _dontDispose = true
            };
        }


        /// <summary>
        /// Disposable implementation to free the GL Mesh Buffers once they are no longer needed.
        /// </summary>
        public void Dispose()
        {
            if (_disposed || _dontDispose)
            {
                return;
            }

            _disposed = true;
            GL.DeleteBuffer(_ebo);
            GL.DeleteBuffer(_vbo);
            GL.DeleteVertexArray(_vao);
        }
    }
}