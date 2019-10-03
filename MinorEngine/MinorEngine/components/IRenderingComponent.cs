using MinorEngine.engine.rendering;
using OpenTK;

namespace MinorEngine.components
{
    public interface IRenderingComponent
    {
        void Render(Matrix4 modelMat, Matrix4 viewMat, Matrix4 projMat);
        ShaderProgram Shader { get; set; }
        int RenderMask { get; set; }
    }
}