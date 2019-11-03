
using System;
using Engine.Core;
using Engine.DataTypes;
using Engine.Rendering;
using OpenTK;

namespace Engine.UI.EventSystems
{
    public class Button : UIImageRendererComponent, ISelectable
    {
        public int TabStop { get; set; }
        private SelectableState state;
        public EventSystem System { get; }
        private Vector2 size
        {
            get
            {
                Vector2 sz = new Vector2();
                sz.X = GameEngine.Instance.Width * Scale.X / GameEngine.Instance.Width;
                sz.Y = GameEngine.Instance.Height * Scale.Y / GameEngine.Instance.Height;
                return sz;
            }
        }

        private Vector2 intSize;
        private Texture btnIdle;
        private Texture btnHover;
        private Texture btnClick;

        public Box2 BoundingBox
        {
            get
            {
                return new Box2(Position - size / 2, Position + size / 2);
            }
            set
            {
                Vector2 center = Vector2.Lerp(new Vector2(value.Left, value.Top),
                    new Vector2(value.Right, value.Bottom), 0.5f);
                Position = center;
                Scale = new Vector2((value.Right - value.Left) / GameEngine.Instance.Width, (value.Bottom - value.Top) / GameEngine.Instance.Height);
            }
        }


        public override Texture Texture
        {
            get
            {
                if (state == SelectableState.Hovered) return btnHover;
                if (state == SelectableState.Selected) return btnClick;
                return btnIdle;
            }
        }

        public Button(EventSystem system, Texture btnIdle, ShaderProgram shader, float alpha, Texture btnClick = null, Texture btnHover = null) : base(btnIdle, false, alpha, shader)
        {
            this.btnIdle = btnIdle;
            if (btnClick != null) this.btnClick = btnClick;
            else this.btnClick = btnIdle;
            if (btnHover != null) this.btnHover = btnHover;
            else this.btnHover = btnIdle;
            System = system;
            intSize = new Vector2(btnIdle.Width, btnIdle.Height);

        }

        protected override void Awake()
        {
            base.Awake();
            System.Register(this);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            System.Unregister(this);
        }

        protected override void Update(float deltaTime)
        {
            base.Update(deltaTime);
            System.Update(); //Hack
        }

        public void SetState(SelectableState newState)
        {
            state = newState;
        }
    }
}