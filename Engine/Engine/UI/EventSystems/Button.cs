
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

        private Texture btnIdle;
        private Texture btnHover;
        private Texture btnClick;
        private Action OnClick;

        public Box2 BoundingBox => new Box2(Position - Scale, Position + Scale);


        public override Texture Texture
        {
            get
            {
                if (state == SelectableState.Hovered) return btnHover;
                if (state == SelectableState.Selected) return btnClick;
                return btnIdle;
            }
        }

        public Button(Texture btnIdle, ShaderProgram shader, float alpha, Texture btnClick = null, Texture btnHover = null, Action onClick = null) : base(btnIdle, false, alpha, shader)
        {
            this.btnIdle = btnIdle;
            if (btnClick != null) this.btnClick = btnClick;
            else this.btnClick = btnIdle;
            if (btnHover != null) this.btnHover = btnHover;
            else this.btnHover = btnIdle;
            System = GameEngine.Instance.UISystem;
            OnClick = onClick;
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

        public void SetState(SelectableState newState)
        {
            if (newState == SelectableState.Selected && state != SelectableState.Selected)
            {
                OnClick?.Invoke();
            }
            state = newState;
        }
    }
}