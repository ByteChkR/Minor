using Assimp;
using GameEngine.engine.components;
using GameEngine.engine.rendering;
using OpenTK;

namespace GameEngine.components
{
    public class MeshRendererComponent : AbstractComponent, IRenderingComponent
    {
        public ShaderProgram Shader { get; set; }
        public int RenderMask { get; set; }
        public GameModel Model { get; set; }

        public MeshRendererComponent(ShaderProgram shader, GameModel model, int renderMask)
        {
            Shader = shader;
            Model = model;
            RenderMask = renderMask;
        }

        public void Render( Matrix4 modelMat, Matrix4 viewMat, Matrix4 projMat)
        {
            if (Model != null && Shader != null)
            {
                Model.Render(Shader, modelMat, viewMat, projMat);
            }
        }

    }
}