using Engine.Core;
using Engine.DataTypes;
using Engine.IO;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace Engine.Rendering
{
    /// <summary>
    /// Implements A mesh Renderer component that can be used to add 3D Objects to the game world
    /// </summary>
    public class LitMeshRendererComponent : RenderingComponent
    {
        private bool _init;

        /// <summary>
        /// Public constructor
        /// </summary>
        /// <param name="shader">The Shader to be used</param>
        /// <param name="model">The mesh to be drawn</param>
        /// <param name="diffuseTexture">The Texture to drawn on the mesh</param>
        /// <param name="renderMask">The render mask</param>
        /// <param name="disposeOnDestroy">The DisposeMeshOnDestroy Flag</param>
        public LitMeshRendererComponent(ShaderProgram shader, Mesh model, Texture diffuseTexture, int renderMask,
            bool disposeOnDestroy = true) : base(shader, true, Renderer.RenderType.Opaque, renderMask)
        {
            Textures = new[] {diffuseTexture};
            Meshes = new[] {model};
            RenderMask = renderMask;
            DisposeMeshOnDestroy = disposeOnDestroy;
        }

        /// <summary>
        /// The property that implements the render mask requirement of IRendereringComponent
        /// </summary>
        public int RenderMask { get; set; }

        /// <summary>
        /// The Tiling of the Diffuse Texture
        /// </summary>
        public Vector2 Tiling { get; set; } = Vector2.One;

        /// <summary>
        /// The Offset of the Diffuse Texture
        /// </summary>
        public Vector2 Offset { get; set; }

        /// <summary>
        /// The meshes that are drawn to the screen
        /// </summary>
        public Mesh[] Meshes { get; set; }

        /// <summary>
        /// The Textures that are used to draw the meshes
        /// </summary>
        public Texture[] Textures { get; set; }


        /// <summary>
        /// A flag that if set, will dispose the meshes once it has been destroyed
        /// </summary>
        public bool DisposeMeshOnDestroy { get; set; }

        /// <summary>
        /// Dispose implementation
        /// </summary>
        protected override void OnDestroy()
        {
            if (DisposeMeshOnDestroy)
            {
                foreach (Mesh mesh in Meshes)
                {
                    mesh.Dispose();
                }
            }
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
            Program.AddUniformCache("cameraPos");
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

            for (int i = 1; i <= 8; i++)
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
            Render(viewMat, projMat, Program);
        }

        public override void Render(Matrix4 viewMat, Matrix4 projMat, int depthMap)
        {
            Program.Use();


            GL.ActiveTexture(TextureUnit.Texture0 + Textures.Length + 1);
            GL.Uniform1(Program.GetUniformLocationUncached("texture_shadow_map"), Textures.Length + 1);
            GL.BindTexture(TextureTarget.Texture2D, depthMap);

            Render(viewMat, projMat, Program);
        }

        public override void Render(Matrix4 viewMat, Matrix4 projMat, ShaderProgram prog)
        {
            prog.Use();

            if (!_init)
            {
                Init();
            }

            Matrix4 mat = Owner._worldTransformCache;
            GL.UniformMatrix4(prog.GetUniformLocation("modelMatrix"), false, ref mat);
            GL.UniformMatrix4(prog.GetUniformLocation("viewMatrix"), false, ref viewMat);
            GL.UniformMatrix4(prog.GetUniformLocation("projectionMatrix"), false, ref projMat);
            Matrix4 mvp = mat * viewMat * projMat;
            GL.UniformMatrix4(prog.GetUniformLocation("mvpMatrix"), false, ref mvp);
            GL.Uniform2(prog.GetUniformLocation("tiling"), Tiling);
            GL.Uniform2(prog.GetUniformLocation("offset"), Offset);
            GL.Uniform3(prog.GetUniformLocation("cameraPos"), GameEngine.Instance.CurrentScene.Camera.LocalPosition);
            GL.Uniform1(prog.GetUniformLocation("shininess"), 15f);


            string s = "lights[{0}].{1}";

            if (Renderer.Lights.Count > 0)
            {
                GL.Uniform1(prog.GetUniformLocation("lightCount"), Renderer.Lights.Count);
                for (int i = 0; i < Renderer.Lights.Count; i++)
                {
                    float lightInt = Renderer.Lights[i].Intensity;
                    GL.Uniform1(prog.GetUniformLocation(string.Format(s, i, "intensity")), lightInt);

                    int type = Renderer.Lights[i].IsPoint ? 1 : 0;
                    GL.Uniform1(prog.GetUniformLocation(string.Format(s, i, "type")), type);

                    float ambientContrib = Renderer.Lights[i].AmbientContribution;
                    GL.Uniform1(prog.GetUniformLocation(string.Format(s, i, "ambientContribution")), ambientContrib);

                    Vector3 pos = Renderer.Lights[i].Owner.GetWorldPosition();
                    GL.Uniform3(prog.GetUniformLocation(string.Format(s, i, "position")), pos);
                    Vector3 att = Renderer.Lights[i].Attenuation;
                    GL.Uniform3(prog.GetUniformLocation(string.Format(s, i, "attenuation")), att);
                    Vector3 col = new Vector3(Renderer.Lights[i].LightColor.R, Renderer.Lights[i].LightColor.G,
                                      Renderer.Lights[i].LightColor.B) / 255f;
                    GL.Uniform3(prog.GetUniformLocation(string.Format(s, i, "color")), col);
                }
            }
            else
            {
                GL.Uniform1(prog.GetUniformLocation("lightCount"), 1);
                LightComponent lc = new LightComponent();

                float lightInt = lc.Intensity;
                GL.Uniform1(prog.GetUniformLocation(string.Format(s, 0, "intensity")), lightInt);

                int type = lc.IsPoint ? 1 : 0;
                GL.Uniform1(prog.GetUniformLocation(string.Format(s, 0, "type")), type);

                float ambientContrib = lc.AmbientContribution;
                GL.Uniform1(prog.GetUniformLocation(string.Format(s, 0, "ambientContribution")), ambientContrib);

                Vector3 pos = new Vector3(0, 10, 0);
                GL.Uniform3(prog.GetUniformLocation(string.Format(s, 0, "position")), pos);
                Vector3 att = lc.Attenuation;
                GL.Uniform3(prog.GetUniformLocation(string.Format(s, 0, "attenuation")), att);
                Vector3 col = new Vector3(lc.LightColor.R, lc.LightColor.G,
                                  lc.LightColor.B) / 255f;
                GL.Uniform3(prog.GetUniformLocation(string.Format(s, 0, "color")), col);
            }


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