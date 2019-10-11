using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using Engine.Debug;
using Engine.Physics;
using Engine.Physics.BEPUphysics.BroadPhaseEntries;
using Engine.Physics.BEPUphysics.BroadPhaseEntries.MobileCollidables;
using Engine.Physics.BEPUphysics.CollisionTests;
using Engine.Physics.BEPUphysics.NarrowPhaseSystems.Pairs;
using Engine.Rendering;
using OpenTK;
using OpenTK.Input;

namespace Engine.Core
{
    /// <summary>
    /// The Object is implementing the transform logic and contains the components
    /// </summary>
    public class GameObject : IDestroyable
    {
        /// <summary>
        /// Redirection for the KeyDown Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        internal static void _KeyDown(object sender, KeyboardKeyEventArgs e)
        {
            GameEngine.Instance.CurrentScene.OnKeyDown(sender, e);
        }

        /// <summary>
        /// Redirection for the KeyUp Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        internal static void _KeyUp(object sender, KeyboardKeyEventArgs e)
        {
            GameEngine.Instance.CurrentScene.OnKeyUp(sender, e);
        }

        /// <summary>
        /// Redirection for the KeyPress Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        internal static void _KeyPress(object sender, KeyPressEventArgs e)
        {
            GameEngine.Instance.CurrentScene.OnKeyPress(sender, e);
        }

        /// <summary>
        /// Internal cache of objects with renderers to loop through them quickly
        /// </summary>
        internal static List<GameObject> ObjsWithAttachedRenderers = new List<GameObject>();

        /// <summary>
        /// Internal cache of objects with colliders to loop through them quickly
        /// </summary>
        private static Dictionary<Collidable, Collider> ObjsWithAttachedColliders =
            new Dictionary<Collidable, Collider>();

        /// <summary>
        /// Internal cache for the world transform
        /// </summary>
        internal Matrix4 _worldTransformCache;

        /// <summary>
        /// The IRenderingComponent that will be set to a value when a renderer is added and set to null when a renderer is removed
        /// </summary>
        public IRenderingComponent RenderingComponent { get; private set; }

        /// <summary>
        /// The Transform component
        /// </summary>
        public Matrix4 Transform
        {
            get
            {
                Matrix4 mat = Matrix4.Identity;
                if (Scale != Physics.BEPUutilities.Vector3.Zero)
                    mat *= Matrix4.CreateScale(Scale);
                mat *= Matrix4.CreateFromQuaternion(Rotation);
                mat *= Matrix4.CreateTranslation(LocalPosition);
                return mat;
            }
        }

        /// <summary>
        /// The Local position
        /// </summary>
        [ConfigVariable]
        [XmlElement(Order = 1)]
        public Engine.Physics.BEPUutilities.Vector3 LocalPosition { get; set; }

        /// <summary>
        /// The Scale
        /// </summary>
        [ConfigVariable]
        [XmlElement(Order = 2)]
        public Engine.Physics.BEPUutilities.Vector3 Scale { get; set; } = new Physics.BEPUutilities.Vector3(1f, 1f, 1f);

        /// <summary>
        /// The Rotation
        /// </summary>
        public Engine.Physics.BEPUutilities.Quaternion Rotation { get; set; } = Quaternion.Identity;

        /// <summary>
        /// The Current Scene the object belongs to
        /// </summary>
        public AbstractScene Scene { get; internal set; }

        /// <summary>
        /// An object counter to *almost* never have name collisions
        /// </summary>
        private static int _objId;

        /// <summary>
        /// the list of components in the object
        /// </summary>
        private readonly Dictionary<Type, AbstractComponent> _components = new Dictionary<Type, AbstractComponent>();

        /// <summary>
        /// The list of children of this object
        /// </summary>
        private readonly List<GameObject> _children = new List<GameObject>();

        /// <summary>
        /// The object name
        /// </summary>
        [ConfigVariable]
        [XmlElement(Order = 0)]
        public string Name { get; set; }

        /// <summary>
        /// The amount of children in this object
        /// </summary>
        public int ChildCount => _children.Count;

        /// <summary>
        /// Flag that indicates if an object is truly removed from all systems
        /// </summary>
        public bool Destroyed { get; private set; }


        /// <summary>
        /// Internal flag that is used to increase performance by not calculating matrices for objects that wont show up anyway
        /// </summary>
        private bool _hasRendererInHierarchy;

        /// <summary>
        /// The parent of the object
        /// </summary>
        public GameObject Parent { get; private set; }

        /// <summary>
        /// Private flag that indicates that the object has been destroyed but is not yet removed from the systems
        /// </summary>
        private bool _destructionPending;


        [ConfigVariable]
        [XmlElement(Order = 3)]
        public Engine.Physics.BEPUutilities.Vector4 AxisAngle
        {
            get => ((Quaternion)Rotation).ToAxisAngle();
            set => Rotation =
                Physics.BEPUutilities.Quaternion.CreateFromAxisAngle(
                    new Physics.BEPUutilities.Vector3(value.X, value.Y, value.Z), value.W);
        }

        /// <summary>
        /// Destructor that will create a warning if undestroyed objects get garbage collected
        /// </summary>
        ~GameObject()
        {
            if (!Destroyed)
            {
                Logger.Log("Object " + Name + " was garbage collected. This can cause nullpointers.",
                    DebugChannel.Warning);
            }
        }

        /// <summary>
        /// Adds this game object to the list of game objects that will be removed at the end of the frame
        /// </summary>
        public void Destroy()
        {
            _destructionPending = true;
            foreach (GameObject gameObject in _children)
            {
                gameObject.Destroy();
            }
        }

        /// <summary>
        /// Internal _Destroy function that will completely remove the object from the system
        /// This will get called outside any update loop. so its save to remove stuff here
        /// </summary>
        internal void _Destroy()
        {
            Destroyed = true;

            if (Parent != null)
            {
                Remove(this);
            }

            //this.Log("Destroying GameObject: " + Name, DebugChannel.Log);
            GameObject[] objs = new List<GameObject>(_children).ToArray();

            foreach (GameObject gameObject in objs)
            {
                gameObject._Destroy();
            }


            KeyValuePair<Type, AbstractComponent>[] comps =
                new List<KeyValuePair<Type, AbstractComponent>>(_components).ToArray();

            foreach (KeyValuePair<Type, AbstractComponent> abstractComponent in comps)
            {
                if (abstractComponent.Value is Collider collider)
                {
                    ObjsWithAttachedColliders.Remove(collider.PhysicsCollider.CollisionInformation);
                    unregisterCollider(collider);
                }

                abstractComponent.Value._Destroy();
            }
        }

        /// <summary>
        /// public constructor
        /// </summary>
        /// <param name="localPosition">Initial object position</param>
        /// <param name="name">Name of the object</param>
        /// <param name="parent">The parent of the object</param>
        public GameObject(Vector3 localPosition, string name, GameObject parent)
        {
            LocalPosition = localPosition;
            Parent = parent;

            if (name == string.Empty)
            {
                Name = "Gameobject" + _objId;
                addObjCount();
            }
            else
            {
                Name = name;
            }
        }

        /// <summary>
        /// Adds the object count
        /// is used for giving objects names that are different
        /// </summary>
        private static void addObjCount()
        {
            _objId++;
        }

        /// <summary>
        /// public constructor
        /// </summary>
        /// <param name="localPosition">Initial object position</param>
        /// <param name="name">Name of the object</param>
        public GameObject(Vector3 localPosition, string name) : this(localPosition, name, null)
        {
        }

        /// <summary>
        /// public constructor
        /// </summary>
        /// <param name="name">Name of the object</param>
        public GameObject(string name) : this(new Vector3(), name, null)
        {
        }

        /// <summary>
        /// Adds a component to the object
        /// </summary>
        /// <param name="component">the component to add</param>
        public void AddComponent(AbstractComponent component)
        {
            Type t = component.GetType();
            if (!_components.ContainsKey(t))
            {
                if (typeof(IRenderingComponent).IsAssignableFrom(t))
                {
                    applyRenderHierarchy(true);
                    ObjsWithAttachedRenderers.Add(this);
                    RenderingComponent = (IRenderingComponent)component;
                }
                else if (component is Collider collider)
                {
                    ObjsWithAttachedColliders.Add(collider.PhysicsCollider.CollisionInformation, collider);
                    registerCollider(collider);
                }

                _components.Add(t, component);
                component.Owner = this;
            }
        }

        /// <summary>
        /// Internal function that removes the gameobjects and components in its hierarchy that have the _destructionPending flag set to true
        /// </summary>
        internal void RemoveDestroyedObjects()
        {
            if (_destructionPending) //Either Remove the object as a whole
            {
                _Destroy();
            }
            else //Or check every component if it needs removal
            {
                KeyValuePair<Type, AbstractComponent>[] comps =
                    new List<KeyValuePair<Type, AbstractComponent>>(_components).ToArray();

                foreach (KeyValuePair<Type, AbstractComponent> abstractComponent in comps)
                {
                    if (abstractComponent.Value._destructionPending)
                    {
                        if (abstractComponent.Value is Collider collider)
                        {
                            ObjsWithAttachedColliders.Remove(collider.PhysicsCollider.CollisionInformation);
                            unregisterCollider(collider);
                        }

                        abstractComponent.Value._Destroy();
                    }
                }

                List<GameObject> go = new List<GameObject>(_children);

                foreach (GameObject gameObject in go)
                {
                    gameObject.RemoveDestroyedObjects();
                }
            }
        }

        /// <summary>
        /// Removes an object from the render loop
        /// </summary>
        private void RemoveFromRenderLoop()
        {
            Logger.Log("Removing Object: " + Name + " from Rendering Loop", DebugChannel.Log);
            RemoveComponent(typeof(IRenderingComponent));
            ObjsWithAttachedRenderers.Remove(this);
        }

        /// <summary>
        /// Removes a component of a type
        /// </summary>
        /// <param name="componentType">the type to remove</param>
        public void RemoveComponent(Type componentType)
        {
            Type t = componentType;
            if (_components.ContainsKey(t))
            {
                if (typeof(IRenderingComponent).IsAssignableFrom(t))
                {
                    applyRenderHierarchy(false);
                    RemoveFromRenderLoop();
                    RenderingComponent = null;
                }

                AbstractComponent component = _components[t];

                if (component is Collider collider)
                {
                    ObjsWithAttachedColliders.Remove(collider.PhysicsCollider.CollisionInformation);
                    unregisterCollider(collider);
                }

                _components.Remove(t);
                component.Owner = null;
            }
        }

        /// <summary>
        /// Removes a component of type T
        /// </summary>
        /// <typeparam name="T">Type of component</typeparam>
        public void RemoveComponent<T>() where T : AbstractComponent
        {
            RemoveComponent(typeof(T));
        }


        /// <summary>
        /// Gets the component of type T from this object
        /// The difference between GetComponent() and this function is that in this function you can search for "non specific types" e.g. you can search for UIElement and it will return the first UI element in the list
        /// </summary>
        /// <typeparam name="T">Type of the component</typeparam>
        /// <returns>The component; if not found it returns null</returns>
        public T GetComponentIterative<T>() where T : AbstractComponent
        {
            foreach (KeyValuePair<Type, AbstractComponent> abstractComponent in _components)
            {
                if (typeof(T).IsAssignableFrom(abstractComponent.Key))
                {
                    return (T)abstractComponent.Value;
                }
            }

            return null;
        }

        /// <summary>
        /// Gets a component of type T from this object.
        /// </summary>
        /// <typeparam name="T">Type of component</typeparam>
        /// <returns>The component; if not found it returns null</returns>
        public T GetComponent<T>() where T : AbstractComponent
        {
            if (_components.ContainsKey(typeof(T)))
            {
                return (T)_components[typeof(T)];
            }

            return null;
        }


        /// <summary>
        /// Computes the World transform and stores it in a cache.
        /// </summary>
        /// <param name="parentTransform"></param>
        public void ComputeWorldTransformCache(Matrix4 parentTransform)
        {
            _worldTransformCache = Transform * parentTransform;
            foreach (GameObject gameObject in _children)
            {
                if (gameObject._hasRendererInHierarchy) //We only need to update the worldspace cache when we need to
                {
                    gameObject.ComputeWorldTransformCache(parentTransform);
                }
            }
        }


        /// <summary>
        /// internal function that removes the child from the _children list and sets the parent to null
        /// </summary>
        /// <param name="child">The child to remove</param>
        private void innerRemove(GameObject child)
        {
            for (int i = _children.Count - 1; i >= 0; i--)
            {
                if (_children[i] == child)
                {
                    _children.RemoveAt(i);
                    child.Parent = null;
                    return;
                }
            }
        }

        /// <summary>
        /// Optimization.
        /// If a Renderer Was attached to this object or one object in the children we set the Has Renderer flag to true.
        /// The other way around if a Renderer gets removed(but we stop unsetting the flag when we found another child with a renderer attached)
        /// This way we know which matrixes we need to compute and which ones we can discard since they wont show up in the renderer anyway
        /// </summary>
        /// <param name="hasRenderer"></param>
        private void
            applyRenderHierarchy(
                bool hasRenderer) //This gets called from the AddComponent/RemoveComponent function and recursively from applyRenderHierarchyFromBelow
        {
            _hasRendererInHierarchy = hasRenderer;
            Parent?.applyRenderHierarchyFromBelow(hasRenderer); //Call the parent 
        }

        /// <summary>
        /// Optimization.
        /// If a Renderer Was attached to this object or one object in the children we set the Has Renderer flag to true.
        /// The other way around if a Renderer gets removed(but we stop unsetting the flag when we found another child with a renderer attached)
        /// This way we know which matrixes we need to compute and which ones we can discard since they wont show up in the renderer anyway
        /// </summary>
        /// <param name="hasRenderer"></param>
        private void applyRenderHierarchyFromBelow(bool hasRenderer) //The child calls this
        {
            if (hasRenderer && !_hasRendererInHierarchy) //A child attached a render and we dont have the flag set
            {
                //_hasRendererInHierarchy = true;
                applyRenderHierarchy(hasRenderer);
            }
            else if (!hasRenderer && _hasRendererInHierarchy
            ) //A child removed a renderer and now we need to check if we can set the flag to false(if all the childs dont have renderers)
            {
                bool childhaveRenderers = false;
                foreach (GameObject gameObject in _children)
                {
                    if (gameObject._hasRendererInHierarchy)
                    {
                        childhaveRenderers = true;
                        break;
                    }
                }

                if (!childhaveRenderers)
                {
                    applyRenderHierarchy(hasRenderer);
                }
            }
        }

        /// <summary>
        /// internal function that adds the child to the _children list and sets the parent
        /// </summary>
        /// <param name="child">The child to add</param>
        private void innerAdd(GameObject child)
        {
            _children.Add(child);
            child.Parent = this;
        }

        /// <summary>
        /// Adds a child to the object
        /// </summary>
        /// <param name="child">The object to add</param>
        public void Add(GameObject child)
        {
            if (child._hasRendererInHierarchy)
            {
                applyRenderHierarchy(true);
            }

            child.SetParent(this);
        }


        /// <summary>
        /// Removes a child from the object
        /// </summary>
        /// <param name="child">the object to remove</param>
        public static void Remove(GameObject child)
        {
            child.SetParent(null);
        }


        /// <summary>
        /// Sets the Parent Object to be newParent
        /// </summary>
        /// <param name="newParent">The new Parent object</param>
        public void SetParent(GameObject newParent)
        {
            Parent?.innerRemove(this);
            newParent?.innerAdd(this);

            if (Parent != null)
            {
                setSceneRecursively(Parent.Scene);
            }
            else
            {
                setSceneRecursively(null);
            }
        }

        /// <summary>
        /// Registers a collider from the game object.
        /// </summary>
        /// <param name="coll">The Collider to be registered</param>
        internal void registerCollider(Collider coll)
        {
            coll.PhysicsCollider.CollisionInformation.Events.ContactCreated += Events_ContactCreated;
            coll.PhysicsCollider.CollisionInformation.Events.ContactRemoved += Events_ContactRemoved;
            coll.PhysicsCollider.CollisionInformation.Events.CollisionEnded += Events_CollisionEnded;
            coll.PhysicsCollider.CollisionInformation.Events.InitialCollisionDetected +=
                Events_InitialCollisionDetected;
        }

        /// <summary>
        /// Unregisters a collider from the game object.
        /// </summary>
        /// <param name="coll">The Collider to be unregistered</param>
        internal void unregisterCollider(Collider coll)
        {
            coll.PhysicsCollider.CollisionInformation.Events.ContactCreated -= Events_ContactCreated;
            coll.PhysicsCollider.CollisionInformation.Events.ContactRemoved -= Events_ContactRemoved;
            coll.PhysicsCollider.CollisionInformation.Events.CollisionEnded -= Events_CollisionEnded;
            coll.PhysicsCollider.CollisionInformation.Events.InitialCollisionDetected -=
                Events_InitialCollisionDetected;
        }

        /// <summary>
        /// Receives events from the physics engine when it has a collider attached
        /// </summary>
        /// <param name="sender">Event data</param>
        /// <param name="other">Other Collidable Object(physics internal)</param>
        /// <param name="pair">The Pair Handler</param>
        private void Events_InitialCollisionDetected(EntityCollidable sender, Collidable other,
            CollidablePairHandler pair)
        {
            if (ObjsWithAttachedColliders.TryGetValue(other, out Collider otherCol))
            {
                foreach (KeyValuePair<Type, AbstractComponent> abstractComponent in _components)
                {
                    abstractComponent.Value.onInitialCollisionDetected(otherCol, pair);
                }
            }
        }

        /// <summary>
        /// Receives events from the physics engine when it has a collider attached
        /// </summary>
        /// <param name="sender">Event data</param>
        /// <param name="other">Other Collidable Object(physics internal)</param>
        /// <param name="pair">The Pair Handler</param>
        private void Events_CollisionEnded(EntityCollidable sender, Collidable other, CollidablePairHandler pair)
        {
            if (ObjsWithAttachedColliders.TryGetValue(other, out Collider otherCol))
            {
                foreach (KeyValuePair<Type, AbstractComponent> abstractComponent in _components)
                {
                    abstractComponent.Value.onCollisionEnded(otherCol, pair);
                }
            }
        }

        /// <summary>
        /// Receives events from the physics engine when it has a collider attached
        /// </summary>
        /// <param name="sender">Event data</param>
        /// <param name="other">Other Collidable Object(physics internal)</param>
        /// <param name="pair">The Pair Handler</param>
        /// <param name="contact">The Contact that has been removed</param>
        private void Events_ContactRemoved(EntityCollidable sender, Collidable other, CollidablePairHandler pair,
            ContactData contact)
        {
            if (ObjsWithAttachedColliders.TryGetValue(other, out Collider otherCol))
            {
                foreach (KeyValuePair<Type, AbstractComponent> abstractComponent in _components)
                {
                    abstractComponent.Value.onContactRemoved(otherCol, pair, contact);
                }
            }
        }


        /// <summary>
        /// Receives events from the physics engine when it has a collider attached
        /// </summary>
        /// <param name="sender">Event data</param>
        /// <param name="other">Other Collidable Object(physics internal)</param>
        /// <param name="pair">The Pair Handler</param>
        /// <param name="contact">The Contact that has been created</param>
        private void Events_ContactCreated(EntityCollidable sender, Collidable other, CollidablePairHandler pair,
            ContactData contact)
        {
            if (ObjsWithAttachedColliders.TryGetValue(other, out Collider otherCol))
            {
                foreach (KeyValuePair<Type, AbstractComponent> abstractComponent in _components)
                {
                    abstractComponent.Value.onContactCreated(otherCol, pair, contact);
                }
            }
        }

        /// <summary>
        /// Gets Called whenever a KeyPress Event gets raised
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnKeyPress(object sender, KeyPressEventArgs e)
        {
            foreach (KeyValuePair<Type, AbstractComponent> abstractComponent in _components)
            {
                abstractComponent.Value.onPress(sender, e);
            }

            for (int i = 0; i < _children.Count; i++)
            {
                _children[i].OnKeyPress(sender, e);
            }
        }

        /// <summary>
        /// Gets Called whenever a KeyUp Event gets raised
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnKeyUp(object sender, KeyboardKeyEventArgs e)
        {
            foreach (KeyValuePair<Type, AbstractComponent> abstractComponent in _components)
            {
                abstractComponent.Value.onKeyUp(sender, e);
            }

            for (int i = 0; i < _children.Count; i++)
            {
                _children[i].OnKeyUp(sender, e);
            }
        }


        /// <summary>
        /// Gets Called whenever a KeyDown Event gets raised
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnKeyDown(object sender, KeyboardKeyEventArgs e)
        {
            foreach (KeyValuePair<Type, AbstractComponent> abstractComponent in _components)
            {
                abstractComponent.Value.onKeyDown(sender, e);
            }

            for (int i = 0; i < _children.Count; i++)
            {
                _children[i].OnKeyDown(sender, e);
            }
        }

        /// <summary>
        /// Update Function
        /// </summary>
        /// <param name="deltaTime">Delta Time in Seconds</param>
        public void Update(float deltaTime)
        {
            foreach (KeyValuePair<Type, AbstractComponent> abstractComponent in _components)
            {
                abstractComponent.Value.updateObject(deltaTime);
            }

            for (int i = 0; i < _children.Count; i++)
            {
                _children[i].Update(deltaTime);
            }
        }

        /// <summary>
        /// Sets the scene recursively for all children
        /// </summary>
        /// <param name="newScene">Scene to set</param>
        private void setSceneRecursively(AbstractScene newScene)
        {
            Scene = newScene;

            for (int i = 0; i < _children.Count; i++)
            {
                _children[i].setSceneRecursively(newScene);
            }
        }


        /// <summary>
        /// Returns a Child of this object with the specifed index
        /// </summary>
        /// <param name="idx">Index of the child</param>
        /// <returns>The child; if not found it returns null</returns>
        public GameObject GetChildAt(int idx)
        {
            if (idx >= 0 && idx < _children.Count)
            {
                return _children[idx];
            }

            return null;
        }

        /// <summary>
        /// Returns a Child(Or this Object) of this object with the specifed name
        /// </summary>
        /// <param name="name">Name of the child</param>
        /// <returns>The child; if not found it returns null</returns>
        public GameObject GetChildWithName(string name)
        {
            if (name == Name)
            {
                return this;
            }

            foreach (GameObject gameObject in _children)
            {
                GameObject ret = gameObject.GetChildWithName(name);
                if (ret != null)
                {
                    return ret;
                }
            }

            return null;
        }

        /// <summary>
        /// Translates the Object by the Specified translation
        /// </summary>
        /// <param name="translation">The translation</param>
        public void Translate(Vector3 translation)
        {
            Physics.BEPUutilities.Vector3 t = translation;
            LocalPosition += t;
        }

        /// <summary>
        /// Scales the object by the specified amount
        /// </summary>
        /// <param name="scaleAmount">amount</param>
        public void ScaleBy(Vector3 scaleAmount)
        {
            Physics.BEPUutilities.Vector3 s = scaleAmount;
            Scale *= s;
        }

        /// <summary>
        /// Gets the Scale of the object
        /// </summary>
        /// <returns>The scale along the axes</returns>
        public Vector3 GetScale()
        {
            return Scale;
        }


        /// <summary>
        /// Rotates the object around an axis for the specified angle
        /// </summary>
        /// <param name="axis">The axis</param>
        /// <param name="angle">The angle in radians</param>
        public void Rotate(Vector3 axis, float angle)
        {
            Physics.BEPUutilities.Quaternion q = Quaternion.FromAxisAngle(axis, angle);
            Rotation *= q;
        }

        /// <summary>
        /// Sets the Rotation of the object
        /// </summary>
        /// <param name="rot"></param>
        public void SetRotation(Quaternion rot)
        {
            Rotation = rot;
        }

        /// <summary>
        /// Transforms the specified vector to world position
        /// </summary>
        /// <param name="vec">Local position</param>
        /// <param name="translate">Is true: takes over translation; if false only applies rotation</param>
        /// <returns></returns>
        public Vector3 TransformToWorld(Vector3 vec, bool translate = true)
        {
            Vector4 v = translate ? new Vector4(vec, 1) : new Vector4(vec, 0);
            return new Vector3(v * GetWorldTransform());
        }

        /// <summary>
        /// Returns the World Position of the Object
        /// </summary>
        /// <returns></returns>
        public Vector3 GetWorldPosition()
        {
            return TransformToWorld(GetLocalPosition());
        }

        /// <summary>
        /// Returns the World Transform of the object
        /// </summary>
        /// <returns></returns>
        public Matrix4 GetWorldTransform()
        {
            if (Parent == null)
            {
                return Transform;
            }

            return Parent.GetWorldTransform() * Transform;
        }

        /// <summary>
        /// Gets the local Position
        /// </summary>
        /// <returns>The local Object Position</returns>
        public Vector3 GetLocalPosition()
        {
            return LocalPosition;
        }

        /// <summary>
        /// Gets the Orientation
        /// </summary>
        public Quaternion GetOrientation()
        {
            return Rotation;
        }

        /// <summary>
        /// Sets the local Position
        /// </summary>
        /// <param name="pos">Position</param>
        public void SetLocalPosition(Vector3 pos)
        {
            LocalPosition = pos;
        }

        /// <summary>
        /// Rotates this Game Object so that it looks at the specified Target Object
        /// </summary>
        /// <param name="other">The target Game Object</param>
        public void LookAt(GameObject other)
        {
            Vector3 target = new Vector3(new Vector4(other.GetLocalPosition(), 0) * other.GetWorldTransform());
            LookAt(target);
        }

        /// <summary>
        /// Rotates this Game Object so that it looks at the specified vector in world coordinates
        /// </summary>
        /// <param name="worldPos">Position in world space</param>
        public void LookAt(Vector3 worldPos)
        {
            Vector3 position = GetLocalPosition();

            Vector3 t = worldPos - position;


            if (t == Vector3.Zero)
            {
                return;
            }

            Vector3 newForward = Vector3.Normalize(t);

            //New Right Vector
            Vector3 newRight = Vector3.Cross(Vector3.UnitY, newForward);
            Vector3 newUp = Vector3.Cross(newForward, newRight);


            Matrix3 newMat = new Matrix3(new Vector3(-newRight), new Vector3(newUp), new Vector3(-newForward));
            Rotation = newMat.ExtractRotation();
        }
    }
}