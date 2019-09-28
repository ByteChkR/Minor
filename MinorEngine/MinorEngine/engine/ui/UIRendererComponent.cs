using GameEngine.components;
using GameEngine.engine.components;
using GameEngine.engine.rendering;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace GameEngine.engine.ui
{
    public class UIRendererComponent : AbstractComponent, IRenderingComponent
    {
        public ShaderProgram Shader { get; set; }
        public int RenderMask { get; set; }
        public GameTexture Texture { get; set; }
        public Vector2 Position { get; set; }
        public Vector2 Scale { get; set; }
        public bool WorldSpace { get; set; }
        private float Alpha { get; set; }

        public UIRendererComponent(int width, int height, ShaderProgram shader) : this(GameTexture.Create(width, height), shader)
        {

        }

        public UIRendererComponent(GameTexture texture, ShaderProgram shader)
        {
            Texture = texture;
            RenderMask = 1;
            if (shader != null)
            {
                Shader = shader;
            }
            else
            {
                Shader = UIHelper.Instance.DefaultUIShader;
            }
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
                {
                    m = modelMat * viewMat * projMat;
                }
                else
                {
                    m = Matrix4.CreateTranslation(Position.X, Position.Y, 0);
                }

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