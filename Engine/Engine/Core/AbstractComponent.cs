using Engine.Physics;
using Engine.Physics.BEPUphysics.CollisionTests;
using Engine.Physics.BEPUphysics.NarrowPhaseSystems.Pairs;
using OpenTK;
using OpenTK.Input;

namespace Engine.Core
{
    /// <summary>
    /// Base class of every component
    /// </summary>
    public abstract class AbstractComponent : IDestroyable
    {
        /// <summary>
        /// A private flag indicating if the object is awake
        /// </summary>
        private bool _awake;

        /// <summary>
        /// Internal flag that is set to true if the Component or the owning game object is beeing destroyed
        /// </summary>
        internal bool _destructionPending;

        /// <summary>
        /// The Owner of the component(null if not attached to any Game object)
        /// </summary>
        public GameObject Owner { get; set; }

        /// <summary>
        /// A public flag that is set to true when the object has been truely removed from the systems
        /// </summary>
        public bool Destroyed { get; private set; }

        /// <summary>
        /// Adds this component to the list of components that will be removed at the end of the frame
        /// </summary>
        public void Destroy()
        {
            if (_destructionPending)
            {
                return;
            }

            _destructionPending = true;
        }

        /// <summary>
        /// Virtual Function that gets called when the script is used by the system for the first time
        /// </summary>
        protected virtual void Awake()
        {
        }

        /// <summary>
        /// Internal _Destroy function that will completely remove the component from the system
        /// This will get called outside any update loop. so its save to remove stuff here
        /// </summary>
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

        /// <summary>
        /// On Destroy gets called at the end of the frame when the destroyed objects are collected from the system.
        /// </summary>
        protected virtual void OnDestroy()
        {
        }


        /// <summary>
        /// Gets Called whenever a KeyPressEvent gets raised
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void OnKeyPress(object sender, KeyPressEventArgs e)
        {
        }

        /// <summary>
        /// Gets Called whenever a KeyDown Event gets raised
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void OnKeyDown(object sender, KeyboardKeyEventArgs e)
        {
        }

        /// <summary>
        /// Gets Called whenever a KeyUp Event gets raised
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void OnKeyUp(object sender, KeyboardKeyEventArgs e)
        {
        }

        /// <summary>
        /// Internal Function that is redirecting the Event but checking if the object has been woken up first
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        internal void onPress(object sender, KeyPressEventArgs e)
        {
            if (!_awake)
            {
                Awake();
                _awake = true;
            }

            OnKeyPress(sender, e);
        }

        /// <summary>
        /// Internal Function that is redirecting the Event but checking if the object has been woken up first
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        internal void onKeyDown(object sender, KeyboardKeyEventArgs e)
        {
            if (!_awake)
            {
                Awake();
                _awake = true;
            }

            OnKeyDown(sender, e);
        }

        /// <summary>
        /// Internal Function that is redirecting the Event but checking if the object has been woken up first
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        internal void onKeyUp(object sender, KeyboardKeyEventArgs e)
        {
            if (!_awake)
            {
                Awake();
                _awake = true;
            }

            OnKeyUp(sender, e);
        }

        /// <summary>
        /// Update Function
        /// </summary>
        /// <param name="deltaTime">Delta Time in Seconds</param>
        protected virtual void Update(float deltaTime)
        {
        }


        /// <summary>
        /// internal function that redirects the update call but checks if the object has been woken up first
        /// </summary>
        /// <param name="deltaTime">Delta Time in Seconds</param>
        internal void updateObject(float deltaTime)
        {
            if (!_awake)
            {
                Awake();
                _awake = true;
            }

            Update(deltaTime);
        }

        /// <summary>
        /// Gets called whenever a Initial Collision was detected on this game object
        /// </summary>
        /// <param name="other">The other game object</param>
        /// <param name="handler">The Handler Containing Collision Data</param>
        protected virtual void OnInitialCollisionDetected(Collider other, CollidablePairHandler handler)
        {
        }

        /// <summary>
        /// Gets called whenever a the object goes from contactpoints > 0 to contact points == 0
        /// </summary>
        /// <param name="other">The other game object</param>
        /// <param name="handler">The Handler Containing Collision Data</param>
        protected virtual void OnCollisionEnded(Collider other, CollidablePairHandler handler)
        {
        }

        /// <summary>
        /// Gets called whenever a Contact Point was removed on this game object
        /// </summary>
        /// <param name="other">The other game object</param>
        /// <param name="handler">The Handler Containing Collision Data</param>
        protected virtual void OnContactRemoved(Collider other, CollidablePairHandler handler, ContactData contact)
        {
        }

        /// <summary>
        /// Gets called whenever a Contact Point was added on this game object
        /// </summary>
        /// <param name="other">The other game object</param>
        /// <param name="handler">The Handler Containing Collision Data</param>
        protected virtual void OnContactCreated(Collider other, CollidablePairHandler handler, ContactData contact)
        {
        }

        /// <summary>
        /// Internal Function that is redirecting the Event but checking if the object has been woken up first
        /// </summary>
        /// <param name="other">The other game object</param>
        /// <param name="handler">The Handler Containing Collision Data</param>
        internal void onInitialCollisionDetected(Collider other, CollidablePairHandler handler)
        {
            if (!_awake)
            {
                Awake();
                _awake = true;
            }

            OnInitialCollisionDetected(other, handler);
        }

        /// <summary>
        /// Internal Function that is redirecting the Event but checking if the object has been woken up first
        /// </summary>
        /// <param name="other">The other game object</param>
        /// <param name="handler">The Handler Containing Collision Data</param>
        internal void onCollisionEnded(Collider other, CollidablePairHandler handler)
        {
            if (!_awake)
            {
                Awake();
                _awake = true;
            }

            OnCollisionEnded(other, handler);
        }

        /// <summary>
        /// Internal Function that is redirecting the Event but checking if the object has been woken up first
        /// </summary>
        /// <param name="other">The other game object</param>
        /// <param name="handler">The Handler Containing Collision Data</param>
        internal void onContactRemoved(Collider other, CollidablePairHandler handler, ContactData contact)
        {
            if (!_awake)
            {
                Awake();
                _awake = true;
            }

            OnContactRemoved(other, handler, contact);
        }

        /// <summary>
        /// Internal Function that is redirecting the Event but checking if the object has been woken up first
        /// </summary>
        /// <param name="other">The other game object</param>
        /// <param name="handler">The Handler Containing Collision Data</param>
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