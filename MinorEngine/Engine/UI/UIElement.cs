using Engine.Core;
using Engine.Rendering;
using Engine.Rendering.Contexts;
using OpenTK;

namespace Engine.UI
{
    /// <summary>
    /// Implements Common Properties between UI Elements
    /// </summary>
    public abstract class UIElement : AbstractComponent, IRenderingComponent
    {
        /// <summary>
        /// The Render Context
        /// </summary>
        public abstract RenderContext Context { get; }

        /// <summary>
        /// The Shader program that is used
        /// </summary>
        public ShaderProgram Shader { get; set; }

        /// <summary>
        /// The render mask that this element belongs to
        /// </summary>
        public int RenderMask { get; set; }

        /// <summary>
        /// The position of the UI element in uv coordinates
        /// </summary>
        public Vector2 Position
        {
            get => new Vector2(Owner.LocalPosition.X, Owner.LocalPosition.Y);
            set => Owner.LocalPosition = new Vector3(value.X, value.Y, Owner.LocalPosition.Z);
        }

        /// <summary>
        /// The position of the UI element
        /// </summary>
        public Vector2 Scale
        {
            get => new Vector2(Owner.Scale.X, Owner.Scale.Y);
            set => Owner.Scale = new Vector3(value.X, value.Y, Owner.Scale.Z);
        }

        /// <summary>
        /// A flag if the Element is positioned in world space
        /// </summary>
        public bool WorldSpace { get; set; }

        /// <summary>
        /// Alpha value of the texture
        /// </summary>
        public float Alpha { get; set; }

        /// <summary>
        /// The Render queue
        /// </summary>
        public int RenderQueue { get; set; }

        /// <summary>
        /// The UI Element Constructor
        /// </summary>
        /// <param name="shader">The Shader to be used</param>
        /// <param name="worldSpace">Is the Element in world space?</param>
        /// <param name="alpha">Initial ALpha value(0 = transparent; 1 = opaque)</param>
        protected UIElement(ShaderProgram shader, bool worldSpace, float alpha)
        {
            RenderMask = 1 << 30;
            Alpha = alpha;
            WorldSpace = worldSpace;

            if (shader != null)
            {
                Shader = shader;
            }
            else
            {
                Shader = UIHelper.Instance.DefaultUIShader;
            }
        }
    }
}