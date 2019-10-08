using Engine.Physics;
using Engine.Physics.BEPUphysics.CollisionTests;
using Engine.Physics.BEPUphysics.NarrowPhaseSystems.Pairs;
using OpenTK;
using OpenTK.Input;

namespace Engine.Core
{
    public abstract class AbstractComponent : IDestroyable
    {
        public GameObject Owner { get; set; }
        private bool _awake;
        internal bool _destructionPending;
        public bool Destroyed { get; private set; }

        protected virtual void Awake()
        {
        }

        internal void _Destroy()
        {
            OnDestroy();

            if (Owner != null)
            {
                Owner.RemoveComponent(GetType());
            }

            Destroyed = true;
            _awake = false;
        }

        public void Destroy()
        {
            _destructionPending = true;
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

        protected virtual void Update(float deltaTime)
        {
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

        protected virtual void OnInitialCollisionDetected(Collider other, CollidablePairHandler handler)
        {
        }

        protected virtual void OnCollisionEnded(Collider other, CollidablePairHandler handler)
        {
        }

        protected virtual void OnContactRemoved(Collider other, CollidablePairHandler handler, ContactData contact)
        {
        }

        protected virtual void OnContactCreated(Collider other, CollidablePairHandler handler, ContactData contact)
        {
        }

        internal void onInitialCollisionDetected(Collider other, CollidablePairHandler handler)
        {
            if (!_awake)
            {
                Awake();
                _awake = true;
            }

            OnInitialCollisionDetected(other, handler);
        }

        internal void onCollisionEnded(Collider other, CollidablePairHandler handler)
        {
            if (!_awake)
            {
                Awake();
                _awake = true;
            }

            OnCollisionEnded(other, handler);
        }

        internal void onContactRemoved(Collider other, CollidablePairHandler handler, ContactData contact)
        {
            if (!_awake)
            {
                Awake();
                _awake = true;
            }

            OnContactRemoved(other, handler, contact);
        }

        internal void onContactCreated(Collider other, CollidablePairHandler handler, ContactData contact)
        {
            if (!_awake)
            {
                Awake();
                _awake = true;
            }

            OnContactCreated(other, handler, contact);
        }
    }
}