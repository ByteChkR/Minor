using MinorEngine.engine.rendering;
using MinorEngine.engine.rendering.contexts;
using OpenTK;

namespace MinorEngine.components
{
    public interface IRenderingComponent
    {
        RenderContext Context { get; }
        int RenderMask { get; set; }
    }
}