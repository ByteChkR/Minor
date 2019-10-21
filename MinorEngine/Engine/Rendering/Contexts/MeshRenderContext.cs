using Engine.DataTypes;
using Engine.Debug;
using Engine.Exceptions;
using Engine.IO;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace Engine.Rendering.Contexts
{
    /// <summary>
    /// Class that implements rendering meshes
    /// </summary>
    public class MeshRenderContext : RenderContext
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
        public MeshRenderContext(ShaderProgram program, Matrix4 modelMatrix, Mesh[] meshes, Texture[] textures,
            Renderer.RenderType renderType, Vector2 offset, Vector2 tiling) : base(program, modelMatrix, true,
            renderType, 0)
        {
            Tiling = tiling;
            Offset = offset;
            Textures = textures;
            Meshes = meshes;
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



            for (int i = 0; i < 8; i++)
            {
                Program.AddUniformCache("texture_diffuse" + i);
                Program.AddUniformCache("texture_normal" + i);
                Program.AddUniformCache("texture_specular" + i);
                Program.AddUniformCache("texture_height" + i);
            }

        }


        protected bool _init = false;

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




            RenderMeshes();
        }

        protected void RenderMeshes()
        {
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