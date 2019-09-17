using System;
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
        public OpenTK.Vector3 Bitangent;

        public static int Size = sizeof(float) * 14;
    }

    public class GameMesh
    {
        private GameVertex[] vertices;
        private uint[] indices;
        public GameTexture[] textures;
        //private int idxBuffer, vertexBuffer, normalBuffer, uvBuffer, tanBuffer, bitanBuffer;
        private int VAO, VBO, EBO;

        public GameMesh(List<GameVertex> vertices, List<uint> indices, List<GameTexture> textures)
        {
            this.vertices = vertices.ToArray();
            this.indices = indices.ToArray();
            this.textures = textures.ToArray();
            setupMesh();

        }

        public void Draw(ShaderProgram prog)
        {

            uint diff, spec, norm, hegt;
            diff = spec = norm = hegt = 1;

            for (int i = 0; i < textures.Length; i++)
            {
                GL.ActiveTexture(TextureUnit.Texture0 + i);

                string name = "";
                string number = "";
                switch (textures[i].texType)
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
                GL.BindTexture(TextureTarget.Texture2D, textures[i].textureID);
            }



            GL.BindVertexArray(VAO);
            GL.DrawElements(PrimitiveType.Triangles, indices.Length, DrawElementsType.UnsignedInt, 0);


            GL.BindVertexArray(0);
            GL.ActiveTexture(TextureUnit.Texture0);

        }

        private void setupMesh()
        {
            GL.GenVertexArrays(1, out VAO);
            GL.GenBuffers(1, out VBO);
            GL.GenBuffers(1, out EBO);

            //VAO
            GL.BindVertexArray(VAO);

            //VBO
            GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(vertices.Length * GameVertex.Size), vertices, BufferUsageHint.StaticDraw);

            //EBO

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, EBO);
            GL.BufferData(BufferTarget.ElementArrayBuffer, (IntPtr)(indices.Length * sizeof(uint)), indices, BufferUsageHint.StaticDraw);

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
            GL.VertexAttribPointer(4, 3, VertexAttribPointerType.Float, false, GameVertex.Size, offsetOf("Bitangent"));



            GL.BindVertexArray(0);


        }

        private IntPtr offsetOf(string name)
        {
            IntPtr off = Marshal.OffsetOf(typeof(GameVertex), name);
            return off;
        }
    }
}