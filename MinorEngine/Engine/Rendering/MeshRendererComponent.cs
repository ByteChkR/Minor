using Engine.Core;
using Engine.DataTypes;
using Engine.Debug;
using Engine.Rendering.Contexts;
using OpenTK;

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
        private LitMeshRenderContext _context;

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
        /// The backing field of the tiling of the texture
        /// </summary>
        private Vector2 _tiling = Vector2.One;

        /// <summary>
        /// The backing field of the offset of the texture
        /// </summary>
        private Vector2 _offset;

        /// <summary>
        /// The Texture Backing Field
        /// </summary>
        private Texture _diffuseTexture;

        /// <summary>
        /// The Texture Backing Field
        /// </summary>
        private Texture _specularTexture;

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
                    _context = new LitMeshRenderContext(Shader, Owner._worldTransformCache, new[] { Model }, new[] { DiffuseTexture, SpecularTexture },
                        RenderType, Offset, Tiling);
                }
                else if (_contextInvalid || Owner._worldTransformCache != _context.ModelMat)
                {
                    _contextInvalid = false;

                    _context.ModelMat = Owner._worldTransformCache;
                    _context.Program = Shader;
                    _context.Meshes = new[] { Model };
                    _context.Textures = new[] { DiffuseTexture, SpecularTexture };
                    _context.ModelMat = Owner._worldTransformCache;
                    _context.RenderType = RenderType;
                    _context.Offset = Offset;
                    _context.Tiling = Tiling;
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
        /// The Tiling of the Diffuse Texture
        /// </summary>
        public Vector2 Tiling
        {
            get => _tiling;
            set
            {
                if (_tiling != value)
                {
                    _tiling = value;
                    _contextInvalid = true;
                }
            }
        }

        /// <summary>
        /// The Offset of the Diffuse Texture
        /// </summary>
        public Vector2 Offset
        {
            get => _offset;
            set
            {
                if (_offset != value)
                {
                    _offset = value;
                    _contextInvalid = true;
                }
            }
        }

        /// <summary>
        /// The Texture that is used to Render the Mesh
        /// </summary>
        public Texture DiffuseTexture
        {
            get => _diffuseTexture;
            set
            {
                if (_diffuseTexture != value)
                {
                    _diffuseTexture = value;
                    _contextInvalid = true;
                }
            }
        }

        public Texture SpecularTexture
        {
            get => _specularTexture;
            set
            {
                if (_specularTexture != value)
                {
                    _specularTexture = value;
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
        /// <param name="diffuseTexture">The Texture to drawn on the mesh</param>
        /// <param name="renderMask">The render mask</param>
        /// <param name="disposeOnDestroy">The DisposeMeshOnDestroy Flag</param>
        public MeshRendererComponent(ShaderProgram shader, Mesh model, Texture diffuseTexture, int renderMask,
            bool disposeOnDestroy = true)
        {
            Shader = shader;
            DiffuseTexture = diffuseTexture;
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

        protected override void Update(float deltaTime)
        {
            if (_context != null)
                _context.TempTime += deltaTime;
        }

        /// <summary>
        /// Awake override to do some logging
        /// </summary>
        protected override void Awake()
        {
            Logger.Log($"Mesh Info:{Owner.Name} (IDs: VAO: {Model._vao}, VBO: {Model._vbo}, EBO: {Model._ebo})..",
                DebugChannel.Log | DebugChannel.EngineRendering, 3);
        }
    }
}