using System.Collections.Generic;
using System.Drawing.Drawing2D;
using Assimp;
using Engine.DataTypes;
using Engine.IO;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using Mesh = Engine.DataTypes.Mesh;
using PrimitiveType = OpenTK.Graphics.OpenGL.PrimitiveType;
using TextureType = Engine.DataTypes.TextureType;

namespace Engine.Rendering.Contexts
{
    /// <summary>
    /// Class that implements rendering meshes
    /// </summary>
    public class LitMeshRenderContext : MeshRenderContext
    {

        /// <summary>
        /// Public Constructor
        /// </summary>
        /// <param name="program">The Shader to be used</param>
        /// <param name="modelMatrix">The model matrix</param>
        /// <param name="meshes">The meshes to be drawn</param>
        /// <param name="textures">Textures to be drawn</param>
        /// <param name="renderType">The render type of the context</param>
        /// <param name="offset">the offset of the textures</param>
        /// <param name="tiling">the tiling of the textures</param>
        public LitMeshRenderContext(ShaderProgram program, Matrix4 modelMatrix, Mesh[] meshes, Texture[] textures,
            Renderer.RenderType renderType, Vector2 offset, Vector2 tiling) : base(program, modelMatrix, meshes, textures, renderType, offset, tiling)
        {
        }


        private void Init()
        {
            _init = true;
            Program.AddUniformCache("modelMatrix");
            Program.AddUniformCache("viewMatrix");
            Program.AddUniformCache("projectionMatrix");
            Program.AddUniformCache("mvpMatrix");
            Program.AddUniformCache("tiling");
            Program.AddUniformCache("offset");
            Program.AddUniformCache("shininess");
            Program.AddUniformCache("lightCount");
            string s = "lights[{0}].{1}";
            for (int i = 0; i < 8; i++)
            {

                Program.AddUniformCache(string.Format(s, i, "intensity"));
                Program.AddUniformCache(string.Format(s, i, "type"));
                Program.AddUniformCache(string.Format(s, i, "ambientContribution"));
                Program.AddUniformCache(string.Format(s, i, "position"));
                Program.AddUniformCache(string.Format(s, i, "attenuation"));
                Program.AddUniformCache(string.Format(s, i, "color"));
            }

            for (int i = 0; i < 8; i++)
            {
                Program.AddUniformCache("texture_diffuse" + i);
                Program.AddUniformCache("texture_normal" + i);
                Program.AddUniformCache("texture_specular" + i);
                Program.AddUniformCache("texture_height" + i);
            }

        }

        /// <summary>
        /// The Code for rendering a Mesh
        /// </summary>
        /// <param name="viewMat">View Matrix</param>
        /// <param name="projMat">Projection Matrix</param>
        public override void Render(Matrix4 viewMat, Matrix4 projMat)
        {
            Program.Use();

            if (!_init)
            {
                Init();
            }
            Matrix4 mat = ModelMat;
            GL.UniformMatrix4(Program.GetUniformLocation("modelMatrix"), false, ref mat);
            GL.UniformMatrix4(Program.GetUniformLocation("viewMatrix"), false, ref viewMat);
            GL.UniformMatrix4(Program.GetUniformLocation("projectionMatrix"), false, ref projMat);
            Matrix4 mvp = ModelMat * viewMat * projMat;
            GL.UniformMatrix4(Program.GetUniformLocation("mvpMatrix"), false, ref mvp);
            GL.Uniform2(Program.GetUniformLocation("tiling"), Tiling);
            GL.Uniform2(Program.GetUniformLocation("offset"), Offset);
            GL.Uniform1(Program.GetUniformLocation("shininess"), 15f);


            string s = "lights[{0}].{1}";

            if (Renderer.Lights.Count > 0)
            {
                GL.Uniform1(Program.GetUniformLocation("lightCount"), Renderer.Lights.Count);
                for (int i = 0; i < Renderer.Lights.Count; i++)
                {
                    float lightInt = Renderer.Lights[i].Intensity;
                    GL.Uniform1(Program.GetUniformLocation(string.Format(s, i, "intensity")), lightInt);

                    int type = Renderer.Lights[i].IsPoint ? 1 : 0;
                    GL.Uniform1(Program.GetUniformLocation(string.Format(s, i, "type")), type);

                    float ambientContrib = Renderer.Lights[i].AmbientContribution;
                    GL.Uniform1(Program.GetUniformLocation(string.Format(s, i, "ambientContribution")), ambientContrib);

                    Vector3 pos = Renderer.Lights[i].Owner.GetLocalPosition();
                    GL.Uniform3(Program.GetUniformLocation(string.Format(s, i, "position")), pos);
                    Vector3 att = Renderer.Lights[i].Attenuation;
                    GL.Uniform3(Program.GetUniformLocation(string.Format(s, i, "attenuation")), att);
                    Vector3 col = new Vector3(Renderer.Lights[i].LightColor.R, Renderer.Lights[i].LightColor.G,
                        Renderer.Lights[i].LightColor.B) / 255f;
                    GL.Uniform3(Program.GetUniformLocation(string.Format(s, i, "color")), col);

                }
            }
            else
            {
                GL.Uniform1(Program.GetUniformLocation("lightCount"), 1);
                LightComponent lc = new LightComponent();

                float lightInt = lc.Intensity;
                GL.Uniform1(Program.GetUniformLocation(string.Format(s, 0, "intensity")), lightInt);

                int type = lc.IsPoint ? 1 : 0;
                GL.Uniform1(Program.GetUniformLocation(string.Format(s, 0, "type")), type);

                float ambientContrib = lc.AmbientContribution;
                GL.Uniform1(Program.GetUniformLocation(string.Format(s, 0, "ambientContribution")), ambientContrib);

                Vector3 pos = new Vector3(0, 10, 0);
                GL.Uniform3(Program.GetUniformLocation(string.Format(s, 0, "position")), pos);
                Vector3 att = lc.Attenuation;
                GL.Uniform3(Program.GetUniformLocation(string.Format(s, 0, "attenuation")), att);
                Vector3 col = new Vector3(lc.LightColor.R, lc.LightColor.G,
                    lc.LightColor.B) / 255f;
                GL.Uniform3(Program.GetUniformLocation(string.Format(s, 0, "color")), col);
            }



            RenderMeshes();
        }
    }
}