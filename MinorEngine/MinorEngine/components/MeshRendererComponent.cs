using Assimp;
using MinorEngine.engine.components;
using MinorEngine.engine.rendering;
using OpenTK;

namespace MinorEngine.components
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

        protected override void OnDestroy()
        {
            Model.Destroy();
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