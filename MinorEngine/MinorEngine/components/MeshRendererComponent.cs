using MinorEngine.debug;
using MinorEngine.engine.components;
using MinorEngine.engine.rendering;
using MinorEngine.engine.rendering.contexts;

namespace MinorEngine.components
{
    public class MeshRendererComponent : AbstractComponent, IRenderingComponent
    {
        private RenderContext _context;

        public RenderContext Context
        {
            get
            {
                if (_context == null)
                {
                    _context = new MeshRenderContext(Shader, Owner._worldTransformCache, new[] {Model}, RenderType);
                }
                else
                {
                    if (_context.ModelMat != Owner._worldTransformCache)
                    {
                        _context.ModelMat = Owner._worldTransformCache;
                    }
                }

                return _context;
            }
        }

        public ShaderProgram Shader { get; set; }
        public int RenderMask { get; set; }
        public GameMesh Model { get; set; }
        public bool DisposeMeshOnDestroy { get; set; }

        public Renderer.RenderType RenderType { get; set; }

        public MeshRendererComponent(ShaderProgram shader, GameMesh model, int renderMask, bool disposeOnDestroy = true)
        {
            Shader = shader;
            Model = model;
            RenderMask = renderMask;
            DisposeMeshOnDestroy = disposeOnDestroy;
        }

        protected override void OnDestroy()
        {
            if (DisposeMeshOnDestroy)
            {
                Model.Dispose();
            }
        }

        protected override void Awake()
        {
            Logger.Log($"Mesh Info:{Owner.Name} (IDs: VAO: {Model.Vao}, VBO: {Model.Vbo}, EBO: {Model.Ebo})..",
                DebugChannel.Log);
            if (Model.GetTextureBuffer().Length != 0)
            {
                Logger.Log("Attached Textures: " + Owner.Name + " : " + Model.GetTextureBuffer()[0].TextureId,
                    DebugChannel.Log);
            }
        }
    }
}