using System.Resources;
using Engine.DataTypes;
using Engine.IO;
using Engine.Rendering;
using Engine.Rendering.Contexts;

namespace Engine.UI
{
    /// <summary>
    /// A Component that is rendering an image in camera space
    /// </summary>
    public class UIImageRendererComponent : UIElement
    {
        /// <summary>
        /// The backing field of the context
        /// </summary>
        private UIImageRenderContext _context;

        /// <summary>
        /// Flag if the context has changed and needs an update
        /// </summary>
        private bool _contextInvalid = true;

        /// <summary>
        /// the context property for IRenderingComponent
        /// </summary>
        public override RenderContext Context
        {
            get
            {
                if (_context == null || _contextInvalid)
                {
                    _context = new UIImageRenderContext(Position, Scale, Owner._worldTransformCache, Texture,
                        WorldSpace, Alpha, Shader, RenderQueue);
                }
                else
                {
                    _context.ModelMat = Owner._worldTransformCache;
                    _context.Position = Position;
                    _context.Scale = Scale;
                    _context.Texture = Texture;
                    _context.WorldSpace = WorldSpace;
                    _context.Alpha = Alpha;
                }

                return _context;
            }
        }

        /// <summary>
        /// Backing field for the texture
        /// </summary>
        private Texture _texture;

        /// <summary>
        /// Property that represents the Texture used to draw the screen quad
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
        /// public contstructor
        /// </summary>
        /// <param name="width">Width of the image</param>
        /// <param name="height">Height of the image</param>
        /// <param name="worldSpace">Is the image in world space</param>
        /// <param name="alpha">The alpha value of the image</param>
        /// <param name="shader">The shader that is used to draw</param>
        public UIImageRendererComponent(int width, int height, bool worldSpace, float alpha, ShaderProgram shader) :
            this(
                TextureLoader.ParameterToTexture(width, height), worldSpace,
                alpha, shader)
        {
        }

        /// <summary>
        /// public constructor
        /// </summary>
        /// <param name="texture">The texture that is used to draw</param>
        /// <param name="worldSpace">Is the image in world space</param>
        /// <param name="alpha">The alpha value of the image</param>
        /// <param name="shader">The shader that is used to draw</param>
        public UIImageRendererComponent(Texture texture, bool worldSpace, float alpha, ShaderProgram shader) : base(
            shader, worldSpace, alpha)
        {
            Texture = texture;
        }

        /// <summary>
        /// Disposes th Texture used by the context
        /// </summary>
        protected override void OnDestroy()
        {
            Texture?.Dispose();
        }
    }
}