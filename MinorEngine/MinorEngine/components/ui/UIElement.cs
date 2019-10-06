using MinorEngine.engine.components;
using MinorEngine.engine.rendering;
using MinorEngine.engine.rendering.contexts;
using MinorEngine.engine.rendering.ui;
using OpenTK;

namespace MinorEngine.components.ui
{
    public abstract class UIElement : AbstractComponent, IRenderingComponent
    {
        public abstract RenderContext Context { get; }

        public ShaderProgram Shader { get; set; }
        public int RenderMask { get; set; }
        public Vector2 Position
        {
            get => new Vector2(Owner.LocalPosition.X, Owner.LocalPosition.Y);
            set => Owner.LocalPosition = new Vector3(value.X, value.Y, Owner.LocalPosition.Z);
        }

        public Vector2 Scale
        {
            get => new Vector2(Owner.Scale.X, Owner.Scale.Y);
            set => Owner.Scale = new Vector3(value.X, value.Y, Owner.Scale.Z);
        }
        public bool WorldSpace { get; set; }
        public float Alpha { get; set; }
        public int RenderQueue { get; set; }

        protected UIElement(ShaderProgram shader, bool worldSpace, float alpha)
        {

            RenderMask = 1 << 30;
            Alpha = alpha;
            WorldSpace=worldSpace;

            if (shader != null)
                Shader = shader;
            else
                Shader = UIHelper.Instance.DefaultUIShader;
        }
    }
}