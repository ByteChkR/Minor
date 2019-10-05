using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Assimp;
using MinorEngine.debug;
using MinorEngine.engine.core;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using PrimitiveType = OpenTK.Graphics.OpenGL.PrimitiveType;

namespace MinorEngine.engine.rendering
{
    [StructLayout(LayoutKind.Explicit)]
    public struct GameVertex : IEquatable<GameVertex>
    {
        [FieldOffset(0)] public Vector3 Position;
        [FieldOffset(12)] public Vector3 Normal;
        [FieldOffset(24)] public Vector2 UV;
        [FieldOffset(32)] public Vector3 Tangent;
        [FieldOffset(44)] public Vector3 Bittangent;

        public static readonly int VERTEX_BYTE_SIZE = sizeof(float) * 14;

        public bool Equals(GameVertex other)
        {
            return Vector3.Distance(Position, other.Position) < 0.001f;
        }
    }

    public class GameMesh : IDisposable
    {
        private readonly GameVertex[] _vertices;
        private readonly uint[] _indices;
        private int _ebo;
        private int _vbo;
        private int _vao;
        private bool _disposed;
        private string _debugName;
        private GameTexture[] Textures { get; set; }

        public bool DisposeTexturesOnDestroy { get; set; } = true;
        public int DrawCount => _indices.Length;

        public int Vao => _vao;


        public int Ebo => _ebo;

        public int Vbo => _vbo;

        internal GameMesh(List<GameVertex> vertices, List<uint> indices, List<GameTexture> textures, string DebugName)
        {
            _vertices = vertices.ToArray();
            _indices = indices.ToArray();
            _debugName = DebugName;
            Textures = textures.ToArray();
            setupMesh();
        }

        public void SetTextureBuffer(GameTexture[] tex)
        {
            Textures = tex;
        }

        public GameTexture[] GetTextureBuffer()
        {
            return Textures;
        }

        public Vector3[] ToSequentialVertexList()
        {
            Vector3[] verts = new Vector3[_indices.Length];
            for (int i = 0; i < _indices.Length; i++) verts[i] = _vertices[_indices[i]].Position;

            return verts;
        }

        public void Draw(ShaderProgram prog)
        {
            uint diff, spec, norm, hegt, unknown;
            diff = spec = norm = hegt = unknown = 1;

            for (int i = 0; i < Textures.Length; i++)
            {
                GL.ActiveTexture(TextureUnit.Texture0 + i);

                string name = "";
                string number = "";
                switch (Textures[i].TexType)
                {
                    case TextureType.Diffuse:
                        name = "texture_diffuse";
                        number = (diff++).ToString();
                        break;
                    case TextureType.Specular:
                        name = "texture_specular";
                        number = (spec++).ToString();
                        break;
                    case TextureType.Normals:
                        name = "texture_normal";
                        number = (norm++).ToString();
                        break;
                    case TextureType.Height:
                        name = "texture_height";
                        number = (hegt++).ToString();
                        break;
                    default:
                        name = "texture";
                        number = (unknown++).ToString();
                        break;
                }

                GL.Uniform1(prog.GetUniformLocation(name + number), i);
                GL.BindTexture(TextureTarget.Texture2D, Textures[i].TextureId);
            }


            GL.BindVertexArray(Vao);
            GL.DrawElements(PrimitiveType.Triangles, _indices.Length, DrawElementsType.UnsignedInt, 0);


            GL.BindVertexArray(0);
            GL.ActiveTexture(TextureUnit.Texture0);
        }

        private void setupMesh()
        {
            GL.GenVertexArrays(1, out _vao);
            GL.GenBuffers(1, out _vbo);
            GL.GenBuffers(1, out _ebo);

            //VAO
            GL.BindVertexArray(Vao);

            //VBO
            GL.BindBuffer(BufferTarget.ArrayBuffer, Vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(_vertices.Length * GameVertex.VERTEX_BYTE_SIZE),
                _vertices, BufferUsageHint.StaticDraw);

            //EBO

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, Ebo);
            GL.BufferData(BufferTarget.ElementArrayBuffer, (IntPtr)(_indices.Length * sizeof(uint)), _indices,
                BufferUsageHint.StaticDraw);

            //Attribute Pointers
            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, GameVertex.VERTEX_BYTE_SIZE,
                IntPtr.Zero);

            GL.EnableVertexAttribArray(1);
            GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, GameVertex.VERTEX_BYTE_SIZE,
                offsetOf("Normal"));

            GL.EnableVertexAttribArray(2);
            GL.VertexAttribPointer(2, 2, VertexAttribPointerType.Float, false, GameVertex.VERTEX_BYTE_SIZE,
                offsetOf("UV"));

            GL.EnableVertexAttribArray(3);
            GL.VertexAttribPointer(3, 3, VertexAttribPointerType.Float, false, GameVertex.VERTEX_BYTE_SIZE,
                offsetOf("Tangent"));

            GL.EnableVertexAttribArray(4);
            GL.VertexAttribPointer(4, 3, VertexAttribPointerType.Float, false, GameVertex.VERTEX_BYTE_SIZE,
                offsetOf("Bittangent"));


            GL.BindVertexArray(0);
        }


        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }

            this.Log($"Deleting Mesh:{_debugName} (IDs: VAO: {_vao}, VBO: {_vbo}, EBO: {_ebo})..", DebugChannel.Log);

            _disposed = true;


            GL.DeleteBuffer(Ebo);
            GL.DeleteBuffer(Vbo);
            GL.DeleteVertexArray(Vao);

            if (DisposeTexturesOnDestroy)
                foreach (var gameTexture in Textures)
                {
                    gameTexture.Dispose();
                }


        }

        private static IntPtr offsetOf(string name)
        {
            IntPtr off = Marshal.OffsetOf(typeof(GameVertex), name);
            return off;
        }
    }
}