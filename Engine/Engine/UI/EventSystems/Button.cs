using System;
using Engine.Core;
using Engine.DataTypes;
using Engine.Rendering;
using OpenTK;

namespace Engine.UI.EventSystems
{
    /// <summary>
    /// UI Component implementing a typical button implementation
    /// </summary>
    public class Button : UiImageRendererComponent, ISelectable
    {
        private Texture btnClick;
        private Texture btnHover;

        private Texture btnIdle;
        private Action<Button> onClick;
        private Action<Button> onEnter;
        private Action<Button> onHover;
        private Action<Button> onLeave;

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

            System = GameEngine.Instance.UiSystem;
            this.onClick = onClick;
            this.onEnter = onEnter;
            this.onHover = onHover;
            this.onLeave = onLeave;
        }

        public SelectableState State { get; private set; }
        public EventSystem System { get; }


        public override Texture Texture
        {
            get
            {
                if (State == SelectableState.Hovered)
                {
                    return btnHover;
                }

                if (State == SelectableState.Selected)
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
            if (newState == SelectableState.Selected && State != SelectableState.Selected)
            {
                onClick?.Invoke(this);
            }
            else if (newState == SelectableState.Hovered && State == SelectableState.None)
            {
                onEnter?.Invoke(this);
            }
            else if (newState == SelectableState.Hovered && State == SelectableState.Hovered)
            {
                onHover?.Invoke(this);
            }
            else if (newState == SelectableState.None && State == SelectableState.Hovered)
            {
                onLeave?.Invoke(this);
            }

            State = newState;
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
            AddToEvent(ref onClick, action);
        }

        public void RemoveFromClickEvent(Action<Button> action)
        {
            RemoveFromEvent(ref onClick, action);
        }

        public void AddToEnterEvent(Action<Button> action)
        {
            AddToEvent(ref onEnter, action);
        }

        public void RemoveFromEnterEvent(Action<Button> action)
        {
            RemoveFromEvent(ref onEnter, action);
        }

        public void AddToHoverEvent(Action<Button> action)
        {
            AddToEvent(ref onHover, action);
        }

        public void RemoveFromHoverEvent(Action<Button> action)
        {
            RemoveFromEvent(ref onHover, action);
        }

        public void AddToLeaveEvent(Action<Button> action)
        {
            AddToEvent(ref onLeave, action);
        }

        public void RemoveFromLeaveEvent(Action<Button> action)
        {
            RemoveFromEvent(ref onLeave, action);
        }


        protected override void OnDestroy()
        {
            base.OnDestroy();
            System.Unregister(this);
        }
    }
}