using Engine.Core;
using Engine.DataTypes;
using Engine.Debug;
using Engine.Rendering.Contexts;

namespace Engine.Rendering
{
    public class MeshRendererComponent : AbstractComponent, IRenderingComponent
    {
        private RenderContext _context;
        private bool _contextInvalid = true;
        private ShaderProgram _shader;
        private Mesh _model;
        private Texture _texture;
        private Renderer.RenderType _renderType;

        public RenderContext Context
        {
            get
            {
                if (_context == null || _contextInvalid)
                {
                    _contextInvalid = false;
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

        

        public int RenderMask { get; set; }


        public ShaderProgram Shader
        {
            get => _shader;
            set
            {
                if (_shader != value)
                {
                    _shader = value;
                    _contextInvalid = true;
                }
            }
        }
        public Mesh Model
        {
            get => _model;
            set
            {
                if (_model != value)
                {
                    _model = value;
                    _contextInvalid = true;
                }
            }
        }

        public Texture Texture
        {
            get => _texture;
            set
            {
                if (_texture != value)
                {
                    _texture = value;
                    _contextInvalid = true;
                }
            }
        }


        public Renderer.RenderType RenderType
        {
            get => _renderType;
            set
            {
                if (_renderType != value)
                {
                    _renderType = value;
                    _contextInvalid = true;
                }
            }
        }
        public bool DisposeMeshOnDestroy { get; set; }

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