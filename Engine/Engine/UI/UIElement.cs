using Engine.Core;
using Engine.DataTypes;
using Engine.Rendering;
using OpenTK;

namespace Engine.UI
{
    /// <summary>
    /// Implements Common Properties between UI Elements
    /// </summary>
    public abstract class UIElement : RenderingComponent
    {
        /// <summary>
        /// Flag if the context has changed and needs an update
        /// </summary>
        protected bool ContextInvalid = true;


        /// <summary>
        /// Backing field for alpha
        /// </summary>
        private float _alpha;


        /// <summary>
        /// backing field for the Render Mask
        /// </summary>
        private int _renderMask;

        /// <summary>
        /// The render mask that this element belongs to
        /// </summary>
        public int RenderMask
        {
            get => _renderMask;
            set
            {
                _renderMask = value;
                ContextInvalid = true;
            }
        }

        /// <summary>
        /// The position of the UI element in uv coordinates
        /// </summary>
        public Vector2 Position
        {
            get => new Vector2(Owner.LocalPosition.X, Owner.LocalPosition.Y);
            set
            {
                Owner.LocalPosition = new Vector3(value.X, value.Y, Owner.LocalPosition.Z);
                ContextInvalid = true;
            }
        }

        /// <summary>
        /// The position of the UI element
        /// </summary>
        public Vector2 Scale
        {
            get => new Vector2(Owner.Scale.X, Owner.Scale.Y);
            set
            {
                Owner.Scale = new Vector3(value.X, value.Y, Owner.Scale.Z);
                ContextInvalid = true;
            }
        }

        public Vector2 Tiling { get; set; } = Vector2.One;
        public Vector2 Offset { get; set; } = Vector2.Zero;


        /// <summary>
        /// Alpha value of the texture
        /// </summary>
        public float Alpha
        {
            get => _alpha;
            set
            {
                _alpha = value;
                ContextInvalid = true;
            }
        }


        /// <summary>
        /// The UI Element Constructor
        /// </summary>
        /// <param name="shader">The Shader to be used</param>
        /// <param name="worldSpace">Is the Element in world space?</param>
        /// <param name="alpha">Initial ALpha value(0 = transparent; 1 = opaque)</param>
        protected UIElement(ShaderProgram shader, bool worldSpace, float alpha) : base(shader, worldSpace,
            Renderer.RenderType.Transparent, 1 << 30)
        {
            Alpha = alpha;
            WorldSpace = worldSpace;

            if (shader != null)
            {
                Program = shader;
            }
            else
            {
                Program = DefaultFilepaths.DefaultUIImageShader;
            }
        }
    }
}