using System;
using OpenTK.Graphics.OpenGL;

namespace Engine.DataTypes
{
    public class Mesh : IDisposable
    {
        public readonly int _ebo;
        public readonly int _vbo;
        public readonly int _vao;
        private bool _disposed;
        private bool _dontDispose;
        public readonly int DrawCount;

        internal Mesh(int ebo, int vbo, int vao, int drawCount)
        {
            _ebo = ebo;
            _vbo = vbo;
            _vao = vao;
            DrawCount = drawCount;
        }

        public Mesh Copy()
        {
            return new Mesh(_ebo, _vbo, _vao, DrawCount)
            {
                _dontDispose = true
            };
        }

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