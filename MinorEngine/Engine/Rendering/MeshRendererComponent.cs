using Engine.Core;
using Engine.DataTypes;
using Engine.Debug;
using Engine.Rendering.Contexts;

namespace Engine.Rendering
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
                    _context = new MeshRenderContext(Shader, Owner._worldTransformCache, new[] { Model }, new[] { Texture }, RenderType);
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
        public Mesh Model { get; set; }
        public Texture Texture { get; set; }
        public bool DisposeMeshOnDestroy { get; set; }

        public Renderer.RenderType RenderType { get; set; }

        public MeshRendererComponent(ShaderProgram shader, Mesh model, Texture texture, int renderMask, bool disposeOnDestroy = true)
        {
            Shader = shader;
            Texture = texture;
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
            Logger.Log($"Mesh Info:{Owner.Name} (IDs: VAO: {Model._vao}, VBO: {Model._vbo}, EBO: {Model._ebo})..",
                DebugChannel.Log);
            
        }
    }
}