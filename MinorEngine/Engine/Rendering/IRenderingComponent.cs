
using Engine.Rendering.Contexts;

namespace Engine.Rendering
{
    public interface IRenderingComponent
    {
        RenderContext Context { get; }
        int RenderMask { get; set; }
    }
}