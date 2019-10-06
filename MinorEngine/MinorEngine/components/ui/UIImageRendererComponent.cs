using MinorEngine.components.ui;
using MinorEngine.engine.core;
using MinorEngine.engine.rendering;
using MinorEngine.engine.rendering.contexts;

namespace MinorEngine.engine.components.ui
{
    public class UIImageRendererComponent : UIElement
    {
        private UIImageRenderContext _context;
        public override RenderContext Context
        {
            get
            {
                if (_context == null)
                {
                    _context = new UIImageRenderContext(Position, Scale, Owner._worldTransformCache, Texture, WorldSpace, Alpha, Shader, RenderQueue);
                }
                else
                {
                    _context.ModelMat = Owner._worldTransformCache;
                    _context.Position = Position;
                    _context.Scale = Scale;
                    _context.Texture = Texture;
                    _context.WorldSpace = WorldSpace;
                    _context.Alpha = Alpha;
                }

                return _context;
            }
        }

        public GameTexture Texture { get; set; }


        public UIImageRendererComponent(int width, int height, bool worldSpace, float alpha, ShaderProgram shader) : this(
            ResourceManager.TextureIO.ParameterToTexture(width, height, "UIRendererDefaultTexture"), worldSpace, alpha, shader)
        {
        }

        protected override void OnDestroy()
        {
            Texture?.Dispose();
        }

        public UIImageRendererComponent(GameTexture texture, bool worldSpace, float alpha, ShaderProgram shader) : base(shader, worldSpace, alpha)
        {
            Texture = texture;
        }


        //public virtual void Render(Matrix4 modelMat, Matrix4 viewMat, Matrix4 projMat)
        //{
        //    if (Shader != null)
        //    {
        //        Shader.Use();
        //        Matrix4 m;
        //        if (WorldSpace)
        //            m = modelMat * viewMat * projMat;
        //        else
        //            m = Matrix4.CreateTranslation(Position.X, Position.Y, 0);

        //        GL.UniformMatrix4(Shader.GetUniformLocation("transform"), false, ref m);

        //        GL.ActiveTexture(TextureUnit.Texture0);
        //        GL.BindTexture(TextureTarget.Texture2D, Texture.TextureId);

        //        GL.Uniform1(Shader.GetUniformLocation("texture"), 0);
        //        GL.BindVertexArray(UIHelper.Instance.quadVAO);
        //        GL.DrawArrays(PrimitiveType.Triangles, 0, 6);
        //        GL.BindVertexArray(0);
        //    }
        //}
    }
}