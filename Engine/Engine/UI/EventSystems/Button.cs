using System;
using Engine.Core;
using Engine.DataTypes;
using Engine.Rendering;
using OpenTK;

namespace Engine.UI.EventSystems
{
    public class Button : UIImageRendererComponent, ISelectable
    {
        private Texture btnClick;
        private Texture btnHover;

        private Texture btnIdle;
        private Action<Button> OnClick;
        private Action<Button> OnEnter;
        private Action<Button> OnHover;
        private Action<Button> OnLeave;

        public Button(Texture btnIdle, ShaderProgram shader, float alpha, Texture btnClick = null,
            Texture btnHover = null, Action<Button> onClick = null, Action<Button> onEnter = null,
            Action<Button> onHover = null, Action<Button> onLeave = null) : base(btnIdle, false, alpha, shader)
        {
            this.btnIdle = btnIdle;
            if (btnClick != null)
            {
                this.btnClick = btnClick;
            }
            else
            {
                this.btnClick = btnIdle;
            }

            if (btnHover != null)
            {
                this.btnHover = btnHover;
            }
            else
            {
                this.btnHover = btnIdle;
            }

            System = GameEngine.Instance.UISystem;
            OnClick = onClick;
            OnEnter = onEnter;
            OnHover = onHover;
            OnLeave = onLeave;
        }

        public SelectableState state { get; private set; }
        public EventSystem System { get; }


        public override Texture Texture
        {
            get
            {
                if (state == SelectableState.Hovered)
                {
                    return btnHover;
                }

                if (state == SelectableState.Selected)
                {
                    return btnClick;
                }

                return btnIdle;
            }
        }

        public int TabStop { get; set; }


        public Box2 BoundingBox => new Box2(Position - Scale, Position + Scale);

        public void SetState(SelectableState newState)
        {
            if (newState == SelectableState.Selected && state != SelectableState.Selected)
            {
                OnClick?.Invoke(this);
            }
            else if (newState == SelectableState.Hovered && state == SelectableState.None)
            {
                OnEnter?.Invoke(this);
            }
            else if (newState == SelectableState.Hovered && state == SelectableState.Hovered)
            {
                OnHover?.Invoke(this);
            }
            else if (newState == SelectableState.None && state == SelectableState.Hovered)
            {
                OnLeave?.Invoke(this);
            }

            state = newState;
        }

        protected override void Awake()
        {
            base.Awake();
            System.Register(this);
        }

        public static void AddToEvent(ref Action<Button> ev, Action<Button> action)
        {
            if (ev == null)
            {
                ev = action;
            }
            else
            {
                ev += action;
            }
        }

        public static void RemoveFromEvent(ref Action<Button> ev, Action<Button> action)
        {
            if (ev == null)
            {
            }
            else
            {
                ev -= ev;
            }
        }

        public void AddToClickEvent(Action<Button> action)
        {
            AddToEvent(ref OnClick, action);
        }

        public void RemoveFromClickEvent(Action<Button> action)
        {
            RemoveFromEvent(ref OnClick, action);
        }

        public void AddToEnterEvent(Action<Button> action)
        {
            AddToEvent(ref OnEnter, action);
        }

        public void RemoveFromEnterEvent(Action<Button> action)
        {
            RemoveFromEvent(ref OnEnter, action);
        }

        public void AddToHoverEvent(Action<Button> action)
        {
            AddToEvent(ref OnHover, action);
        }

        public void RemoveFromHoverEvent(Action<Button> action)
        {
            RemoveFromEvent(ref OnHover, action);
        }

        public void AddToLeaveEvent(Action<Button> action)
        {
            AddToEvent(ref OnLeave, action);
        }

        public void RemoveFromLeaveEvent(Action<Button> action)
        {
            RemoveFromEvent(ref OnLeave, action);
        }


        protected override void OnDestroy()
        {
            base.OnDestroy();
            System.Unregister(this);
        }
    }
}