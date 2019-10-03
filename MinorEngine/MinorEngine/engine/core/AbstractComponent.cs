using Common;
using MinorEngine.engine.core;
using OpenTK;
using OpenTK.Input;

namespace MinorEngine.engine.components
{
    public abstract class AbstractComponent : IDestroyable
    {
        public GameObject Owner { get; set; }
        private bool _awake;

        protected virtual void Awake()
        {
        }

        public void Destroy()
        {
            this.Log("Destroying Component of Type: " + GetType().Name, DebugChannel.Log);
            OnDestroy();
            _awake = false;
        }

        protected virtual void OnDestroy()
        {
        }

        protected virtual void OnKeyPress(object sender, KeyPressEventArgs e)
        {
        }

        protected virtual void OnKeyDown(object sender, KeyboardKeyEventArgs e)
        {
        }

        protected virtual void OnKeyUp(object sender, KeyboardKeyEventArgs e)
        {
        }

        internal void onPress(object sender, KeyPressEventArgs e)
        {
            if (!_awake)
            {
                Awake();
                _awake = true;
            }

            OnKeyPress(sender, e);
        }

        internal void onKeyDown(object sender, KeyboardKeyEventArgs e)
        {
            if (!_awake)
            {
                Awake();
                _awake = true;
            }

            OnKeyDown(sender, e);
        }

        internal void onKeyUp(object sender, KeyboardKeyEventArgs e)
        {
            if (!_awake)
            {
                Awake();
                _awake = true;
            }

            OnKeyUp(sender, e);
        }

        internal void updateObject(float deltaTime)
        {
            if (!_awake)
            {
                Awake();
                _awake = true;
            }

            Update(deltaTime);
        }

        protected virtual void Update(float deltaTime)
        {
        }
    }
}