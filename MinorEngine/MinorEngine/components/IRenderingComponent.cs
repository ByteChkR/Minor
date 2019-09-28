using GameEngine.engine.components;
using GameEngine.engine.rendering;
using OpenTK;

namespace GameEngine.components
{
    public interface IRenderingComponent
    {
        void Render(Matrix4 modelMat, Matrix4 viewMat, Matrix4 projMat);
        ShaderProgram Shader { get; set; }
        int RenderMask { get; set; }



    }
}