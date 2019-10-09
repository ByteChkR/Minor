
using Engine.Rendering.Contexts;

namespace Engine.Rendering
{
    /// <summary>
    /// Defines the Properties of an object that can be used for rendering
    /// </summary>
    public interface IRenderingComponent
    {
        /// <summary>
        /// The Context that the Component is using
        /// </summary>
        RenderContext Context { get; }

        /// <summary>
        /// The Render mask
        /// </summary>
        int RenderMask { get; set; }
    }
}