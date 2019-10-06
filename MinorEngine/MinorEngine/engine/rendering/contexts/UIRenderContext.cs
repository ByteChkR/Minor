using OpenTK;

namespace MinorEngine.engine.rendering.contexts
{


    public abstract class UIRenderContext : RenderContext
    {
        public Vector2 Position { get; }
        public Vector2 Scale { get; }
        public float Alpha { get; }



        public UIRenderContext(Vector2 position, Vector2 scale, Matrix4 modelMatrix, bool worldSpace, float alpha, ShaderProgram program, int renderQueue) : base(program, modelMatrix, worldSpace, Renderer.RenderType.Transparent, renderQueue)
        {
            Position = position;
            Scale = scale;
            Alpha = alpha;
        }
    }
}