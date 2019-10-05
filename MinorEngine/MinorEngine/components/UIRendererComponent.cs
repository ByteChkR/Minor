using MinorEngine.components;
using MinorEngine.engine.components;
using MinorEngine.engine.core;
using MinorEngine.engine.rendering;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace MinorEngine.engine.ui
{
    public class UIRendererComponent : AbstractComponent, IRenderingComponent
    {
        public virtual Renderer.RenderContext Context
        {
            get
            {
                return new Renderer.UIRenderContext(Position, Scale, Shader);
            }
        }

        public ShaderProgram Shader { get; set; }
        public int RenderMask { get; set; }
        public GameTexture Texture { get; set; }
        public Vector2 Position { get; set; }
        public Vector2 Scale { get; set; }
        public bool WorldSpace { get; set; }
        private float Alpha { get; }

        public UIRendererComponent(int width, int height, ShaderProgram shader) : this(
            ResourceManager.TextureIO.ParameterToTexture(width, height, "UIRendererDefaultTexture"), shader)
        {
        }

        protected override void OnDestroy()
        {
            Texture?.Dispose();
        }

        public UIRendererComponent(GameTexture texture, ShaderProgram shader)
        {
            Texture = texture;
            RenderMask = 1 << 30;
            if (shader != null)
                Shader = shader;
            else
                Shader = UIHelper.Instance.DefaultUIShader;
            Alpha = 1f;
            Scale = Vector2.One;
        }


        public virtual void Render(Matrix4 modelMat, Matrix4 viewMat, Matrix4 projMat)
        {
            if (Shader != null)
            {
                Shader.Use();
                Matrix4 m;
                if (WorldSpace)
                    m = modelMat * viewMat * projMat;
                else
                    m = Matrix4.CreateTranslation(Position.X, Position.Y, 0);

                GL.UniformMatrix4(Shader.GetUniformLocation("transform"), false, ref m);

                GL.ActiveTexture(TextureUnit.Texture0);
                GL.BindTexture(TextureTarget.Texture2D, Texture.TextureId);

                GL.Uniform1(Shader.GetUniformLocation("texture"), 0);
                GL.BindVertexArray(UIHelper.Instance.quadVAO);
                GL.DrawArrays(PrimitiveType.Triangles, 0, 6);
                GL.BindVertexArray(0);
            }
        }
    }
}