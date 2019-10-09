﻿using Engine.DataTypes;
using Engine.Debug;
using Engine.Exceptions;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace Engine.Rendering.Contexts
{
    public class MeshRenderContext : RenderContext
    {
        public Mesh[] Meshes { get; }
        public Texture[] Textures { get; }

        public MeshRenderContext(ShaderProgram program, Matrix4 modelMatrix, Mesh[] meshes, Texture[] textures,
            Renderer.RenderType renderType) : base(program, modelMatrix, true, renderType, 0)
        {
            Textures = textures;
            Meshes = meshes;
        }

        public override void Render(Matrix4 viewMat, Matrix4 projMat)
        {
            Program.Use();
            var mat = ModelMat;
            GL.UniformMatrix4(Program.GetUniformLocation("modelMatrix"), false, ref mat);
            GL.UniformMatrix4(Program.GetUniformLocation("viewMatrix"), false, ref viewMat);
            GL.UniformMatrix4(Program.GetUniformLocation("projectionMatrix"), false, ref projMat);
            var mvp = ModelMat * viewMat * projMat;
            GL.UniformMatrix4(Program.GetUniformLocation("mvpMatrix"), false, ref mvp);

            foreach (var gameMesh in Meshes)
            {
                uint diff, spec, norm, hegt, unknown;
                diff = spec = norm = hegt = unknown = 1;

                for (var i = 0; i < Textures.Length; i++)
                {
                    if (Textures[i] == null)
                    {
                        continue;
                    }


                    GL.ActiveTexture(TextureUnit.Texture0 + i);

                    var name = "";
                    var number = "";
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

                    GL.Uniform1(Program.GetUniformLocation(name + number), i);
                    GL.BindTexture(TextureTarget.Texture2D, Textures[i].TextureId);
                }


                GL.BindVertexArray(gameMesh._vao);
                GL.DrawElements(PrimitiveType.Triangles, gameMesh.DrawCount, DrawElementsType.UnsignedInt, 0);


                GL.BindVertexArray(0);
                GL.ActiveTexture(TextureUnit.Texture0);
            }
        }
    }
}