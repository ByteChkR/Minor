using MinorEngine.engine.rendering;
using OpenTK;

namespace MinorEngine.components
{
    public interface IRenderingComponent
    {
        Renderer.RenderContext Context { get; }
        int RenderMask { get; set; }
    }
}