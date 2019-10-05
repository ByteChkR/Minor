using Assimp;
using MinorEngine.debug;
using MinorEngine.engine.components;
using MinorEngine.engine.rendering;
using OpenTK;

namespace MinorEngine.components
{
    public class MeshRendererComponent : AbstractComponent, IRenderingComponent
    {
        public Renderer.RenderContext Context => new Renderer.MeshRenderContext(Shader, new[] { Model });

        public ShaderProgram Shader { get; set; }
        public int RenderMask { get; set; }
        public GameMesh Model { get; set; }
        public bool DisposeMeshOnDestroy { get; set; }

        public MeshRendererComponent(ShaderProgram shader, GameMesh model, int renderMask, bool disposeOnDestroy = true)
        {
            Shader = shader;
            Model = model;
            RenderMask = renderMask;
            DisposeMeshOnDestroy = disposeOnDestroy;

        }

        protected override void OnDestroy()
        {
            if (DisposeMeshOnDestroy) Model.Dispose();
        }

        protected override void Awake()
        {

            Logger.Log($"Mesh Info:{Owner.Name} (IDs: VAO: {Model.Vao}, VBO: {Model.Vbo}, EBO: {Model.Ebo})..", DebugChannel.Log);
            Logger.Log("Attached Textures: "+Owner.Name+" : "+ Model.GetTextureBuffer()[0].TextureId, DebugChannel.Log);

        }
    }
}