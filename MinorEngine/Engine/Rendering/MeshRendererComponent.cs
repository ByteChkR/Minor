using Engine.Core;
using Engine.DataTypes;
using Engine.Debug;
using Engine.Rendering.Contexts;

namespace Engine.Rendering
{
    /// <summary>
    /// Implements A mesh Renderer component that can be used to add 3D Objects to the game world
    /// </summary>
    public class MeshRendererComponent : AbstractComponent, IRenderingComponent
    {
        /// <summary>
        /// The Context Backing field
        /// </summary>
        private MeshRenderContext _context;

        /// <summary>
        /// A flag that indicates if the context needs to be updated
        /// </summary>
        private bool _contextInvalid = true;

        /// <summary>
        /// The Shader backing field
        /// </summary>
        private ShaderProgram _shader;

        /// <summary>
        /// The Mesh Backing field
        /// </summary>
        private Mesh _model;

        /// <summary>
        /// The Texture Backing Field
        /// </summary>
        private Texture _texture;

        /// <summary>
        /// The render type backing field
        /// </summary>
        private Renderer.RenderType _renderType;

        /// <summary>
        /// Public context property that is used to get the context for the current object
        /// </summary>
        public RenderContext Context
        {
            get
            {
                if (_context == null)
                {
                    _contextInvalid = false;
                    _context = new MeshRenderContext(Shader, Owner._worldTransformCache, new[] {Model}, new[] {Texture},
                        RenderType);
                }
                else if (_contextInvalid || Owner._worldTransformCache != _context.ModelMat)
                {
                    _contextInvalid = false;

                    _context.ModelMat = Owner._worldTransformCache;
                    _context.Program = Shader;
                    _context.Meshes = new[] {Model};
                    _context.Textures = new[] {Texture};
                    _context.ModelMat = Owner._worldTransformCache;
                    _context.RenderType = RenderType;
                }

                return _context;
            }
        }


        /// <summary>
        /// The property that implements the render mask requirement of IRendereringComponent
        /// </summary>
        public int RenderMask { get; set; }

        /// <summary>
        /// The Shader that is used
        /// </summary>
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

        /// <summary>
        /// The Mesh of the MeshRendererComponent
        /// </summary>
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


        /// <summary>
        /// The Texture that is used to Render the Mesh
        /// </summary>
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

        /// <summary>
        /// The render type
        /// </summary>
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

        /// <summary>
        /// A flag that if set, will dispose the meshes once it has been destroyed
        /// </summary>
        public bool DisposeMeshOnDestroy { get; set; }

        /// <summary>
        /// Public constructor
        /// </summary>
        /// <param name="shader">The Shader to be used</param>
        /// <param name="model">The mesh to be drawn</param>
        /// <param name="texture">The Texture to drawn on the mesh</param>
        /// <param name="renderMask">The render mask</param>
        /// <param name="disposeOnDestroy">The DisposeMeshOnDestroy Flag</param>
        public MeshRendererComponent(ShaderProgram shader, Mesh model, Texture texture, int renderMask,
            bool disposeOnDestroy = true)
        {
            Shader = shader;
            Texture = texture;
            Model = model;
            RenderMask = renderMask;
            DisposeMeshOnDestroy = disposeOnDestroy;
        }

        /// <summary>
        /// Dispose implementation
        /// </summary>
        protected override void OnDestroy()
        {
            if (DisposeMeshOnDestroy)
            {
                Model.Dispose();
            }
        }

        /// <summary>
        /// Awake override to do some logging
        /// </summary>
        protected override void Awake()
        {
            Logger.Log($"Mesh Info:{Owner.Name} (IDs: VAO: {Model._vao}, VBO: {Model._vbo}, EBO: {Model._ebo})..",
                DebugChannel.Log);
        }
    }
}