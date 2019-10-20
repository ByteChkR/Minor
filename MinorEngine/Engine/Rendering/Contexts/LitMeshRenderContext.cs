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
    public class LitMeshRenderContext : RenderContext
    {
        /// <summary>
        /// The meshes that are drawn to the screen
        /// </summary>
        public Mesh[] Meshes { get; set; }

        /// <summary>
        /// The Textures that are used to draw the meshes
        /// </summary>
        public Texture[] Textures { get; set; }

        /// <summary>
        /// The Tiling of the Textures
        /// </summary>
        public Vector2 Tiling { get; set; }

        /// <summary>
        /// The Offset of the Textures
        /// </summary>
        public Vector2 Offset { get; set; }

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
            Renderer.RenderType renderType, Vector2 offset, Vector2 tiling) : base(program, modelMatrix, true,
            renderType, 0)
        {
            Tiling = tiling;
            Offset = offset;
            Textures = textures;
            Meshes = meshes;
        }

        public float TempTime;

        /// <summary>
        /// The Code for rendering a Mesh
        /// </summary>
        /// <param name="viewMat">View Matrix</param>
        /// <param name="projMat">Projection Matrix</param>
        public override void Render(Matrix4 viewMat, Matrix4 projMat)
        {
            Program.Use();
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

                    Vector3 pos = Renderer.Lights[i].Owner.GetWorldPosition();
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
                    lc.LightColor.B)/255f;
                GL.Uniform3(Program.GetUniformLocation(string.Format(s, 0, "color")), col);
            }


            foreach (Mesh gameMesh in Meshes)
            {
                uint diff, spec, norm, hegt, unknown;
                diff = spec = norm = hegt = unknown = 1;
                bool hasSpec = false;
                for (int i = 0; i < Textures.Length; i++)
                {
                    if (Textures[i] == null)
                    {
                        continue;
                    }


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
                            hasSpec = true;
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
                            name = "texture_diffuse";
                            number = (unknown++).ToString();
                            break;
                    }

                    GL.Uniform1(Program.GetUniformLocation(name + number), i);
                    GL.BindTexture(TextureTarget.Texture2D, Textures[i].TextureId);
                }
                if (!hasSpec)
                {
                    int loc = Program.GetUniformLocation("texture_specular1");
                    GL.ActiveTexture(TextureUnit.Texture0 + Textures.Length);
                    GL.Uniform1(loc, Textures.Length);
                    GL.BindTexture(TextureTarget.Texture2D, Prefabs.White.TextureId);
                }



                GL.BindVertexArray(gameMesh._vao);
                GL.DrawElements(PrimitiveType.Triangles, gameMesh.DrawCount, DrawElementsType.UnsignedInt, 0);


                GL.BindVertexArray(0);
                GL.ActiveTexture(TextureUnit.Texture0);
            }
        }
    }
}