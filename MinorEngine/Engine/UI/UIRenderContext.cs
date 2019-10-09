using Engine.Rendering;
using Engine.Rendering.Contexts;
using OpenTK;

namespace Engine.UI
{

    /// <summary>
    /// Abstraction for UI Rendering
    /// </summary>
    public abstract class UIRenderContext : RenderContext
    {

        /// <summary>
        /// Position in UV Space
        /// </summary>
        public Vector2 Position { get; set; }

        /// <summary>
        /// The Scale of the object
        /// </summary>
        public Vector2 Scale { get; set; }

        /// <summary>
        /// the Alpha value of the Element that will be drawn
        /// </summary>
        public float Alpha { get; set; }


        /// <summary>
        /// Protected constructor
        /// </summary>
        /// <param name="position">Position in screen space</param>
        /// <param name="scale">The Scale of the Image</param>
        /// <param name="modelMatrix">Model matrix</param>
        /// <param name="worldSpace">Is the Object in world space</param>
        /// <param name="alpha">Alpha value of the image</param>
        /// <param name="program">The shader to be used</param>
        /// <param name="renderQueue">The Render queue</param>
        protected UIRenderContext(Vector2 position, Vector2 scale, Matrix4 modelMatrix, bool worldSpace, float alpha,
            ShaderProgram program, int renderQueue) : base(program, modelMatrix, worldSpace,
            Renderer.RenderType.Transparent, renderQueue)
        {
            Position = position;
            Scale = scale;
            Alpha = alpha;
        }
    }
}