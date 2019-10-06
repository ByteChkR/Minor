using MinorEngine.engine.rendering.contexts;

namespace MinorEngine.components
{
    public interface IRenderingComponent
    {
        RenderContext Context { get; }
        int RenderMask { get; set; }
    }
}