﻿using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Assimp;
using Common;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using PrimitiveType = OpenTK.Graphics.OpenGL.PrimitiveType;

namespace GameEngine.engine.rendering
{
    [StructLayout(LayoutKind.Explicit)]
    public struct GameVertex
    {

        [FieldOffset(0)]
        public OpenTK.Vector3 Position;
        [FieldOffset(12)]
        public OpenTK.Vector3 Normal;
        [FieldOffset(24)]
        public OpenTK.Vector2 UV;
        [FieldOffset(32)]
        public OpenTK.Vector3 Tangent;
        [FieldOffset(44)]
        public OpenTK.Vector3 Bittangent;

        public static int Size = sizeof(float) * 14;
    }

    public class GameMesh
    {
        private readonly GameVertex[] _vertices;
        private readonly uint[] _indices;
        public GameTexture[] Textures;
        private int _vao, _vbo, _ebo;

        public GameMesh(List<GameVertex> vertices, List<uint> indices, List<GameTexture> textures)
        {
            this._vertices = vertices.ToArray();
            this._indices = indices.ToArray();
            this.Textures = textures.ToArray();
            setupMesh();

        }

        public void Draw(ShaderProgram prog)
        {

            uint diff, spec, norm, hegt;
            diff = spec = norm = hegt = 1;

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
                }

                GL.Uniform1(prog.GetUniformLocation(name + number), i);
                GL.BindTexture(TextureTarget.Texture2D, Textures[i].TextureId);
            }



            GL.BindVertexArray(_vao);
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
            GL.BindVertexArray(_vao);

            //VBO
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(_vertices.Length * GameVertex.Size), _vertices, BufferUsageHint.StaticDraw);

            //EBO

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _ebo);
            GL.BufferData(BufferTarget.ElementArrayBuffer, (IntPtr)(_indices.Length * sizeof(uint)), _indices, BufferUsageHint.StaticDraw);

            //Attribute Pointers
            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, GameVertex.Size, IntPtr.Zero);

            GL.EnableVertexAttribArray(1);
            GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, GameVertex.Size, offsetOf("Normal"));

            GL.EnableVertexAttribArray(2);
            GL.VertexAttribPointer(2, 2, VertexAttribPointerType.Float, false, GameVertex.Size, offsetOf("UV"));

            GL.EnableVertexAttribArray(3);
            GL.VertexAttribPointer(3, 3, VertexAttribPointerType.Float, false, GameVertex.Size, offsetOf("Tangent"));

            GL.EnableVertexAttribArray(4);
            GL.VertexAttribPointer(4, 3, VertexAttribPointerType.Float, false, GameVertex.Size, offsetOf("Bittangent"));



            GL.BindVertexArray(0);


        }

        private IntPtr offsetOf(string name)
        {
            IntPtr off = Marshal.OffsetOf(typeof(GameVertex), name);
            return off;
        }
    }
}