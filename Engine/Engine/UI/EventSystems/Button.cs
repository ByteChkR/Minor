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


        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="btnIdle">Texture when Idle</param>
        /// <param name="shader">Shader Used</param>
        /// <param name="alpha">Alpha Component of the Button</param>
        /// <param name="btnClick">Texture when Clicked</param>
        /// <param name="btnHover">Texture when Hovered</param>
        /// <param name="onClick">Event OnClick</param>
        /// <param name="onEnter">Event OnEnter</param>
        /// <param name="onHover">Event OnHover</param>
        /// <param name="onLeave">Event OnLeave</param>
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

        /// <summary>
        /// The Current state of the Button
        /// </summary>
        public SelectableState State { get; private set; }
        /// <summary>
        /// A reference of the Event System used with this button
        /// </summary>
        public EventSystem System { get; }

        /// <summary>
        /// Wrapper that returns the right texture depending on the State of the button
        /// </summary>
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


        /// <summary>
        /// The Bounding Box of the Button
        /// </summary>
        public Box2 BoundingBox => new Box2(Position - Scale, Position + Scale);
        /// <summary>
        /// Function to set the state of the Button
        /// </summary>
        /// <param name="newState">The new State</param>
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

        /// <summary>
        /// Registers to Event System
        /// </summary>
        protected override void Awake()
        {
            base.Awake();
            System.Register(this);
        }

        /// <summary>
        /// Wrapper that Adds the action to the ev variable
        /// ev can be null, in this case ev = action
        /// </summary>
        /// <param name="ev">Event to add to</param>
        /// <param name="action">Event to add</param>
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
        /// <summary>
        /// Wrapper that Removes the action from the ev variable
        /// ev can be null, in this case nothing is done
        /// </summary>
        /// <param name="ev">Event to remove from</param>
        /// <param name="action">Event to remove</param>
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

        /// <summary>
        /// Adds an OnClick Event
        /// </summary>
        /// <param name="action">The Event To Add</param>
        public void AddToClickEvent(Action<Button> action)
        {
            AddToEvent(ref onClick, action);
        }
        /// <summary>
        /// Removes an OnClick Event
        /// </summary>
        /// <param name="action">The Event To Remove</param>
        public void RemoveFromClickEvent(Action<Button> action)
        {
            RemoveFromEvent(ref onClick, action);
        }
        /// <summary>
        /// Adds an OnEnter Event
        /// </summary>
        /// <param name="action">The Event To Add</param>
        public void AddToEnterEvent(Action<Button> action)
        {
            AddToEvent(ref onEnter, action);
        }
        /// <summary>
        /// Removes an OnEnter Event
        /// </summary>
        /// <param name="action">The Event To Remove</param>
        public void RemoveFromEnterEvent(Action<Button> action)
        {
            RemoveFromEvent(ref onEnter, action);
        }
        /// <summary>
        /// Adds an OnHover Event
        /// </summary>
        /// <param name="action">The Event To Add</param>
        public void AddToHoverEvent(Action<Button> action)
        {
            AddToEvent(ref onHover, action);
        }
        /// <summary>
        /// Removes an OnHover Event
        /// </summary>
        /// <param name="action">The Event To Remove</param>
        public void RemoveFromHoverEvent(Action<Button> action)
        {
            RemoveFromEvent(ref onHover, action);
        }
        /// <summary>
        /// Adds an OnLeave Event
        /// </summary>
        /// <param name="action">The Event To Add</param>
        public void AddToLeaveEvent(Action<Button> action)
        {
            AddToEvent(ref onLeave, action);
        }
        /// <summary>
        /// Removes an OnLeave Event
        /// </summary>
        /// <param name="action">The Event To Remove</param>
        public void RemoveFromLeaveEvent(Action<Button> action)
        {
            RemoveFromEvent(ref onLeave, action);
        }

        /// <summary>
        /// Unregisters from the Event System
        /// </summary>
        protected override void OnDestroy()
        {
            base.OnDestroy();
            System.Unregister(this);
        }
    }
}