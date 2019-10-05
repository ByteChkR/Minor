using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Runtime.CompilerServices;
using Common;
using MinorEngine.BEPUphysics.BroadPhaseEntries;
using MinorEngine.BEPUphysics.BroadPhaseEntries.MobileCollidables;
using MinorEngine.BEPUphysics.CollisionTests;
using MinorEngine.BEPUphysics.NarrowPhaseSystems.Pairs;
using MinorEngine.debug;
using MinorEngine.components;
using MinorEngine.engine.components;
using OpenTK;
using OpenTK.Input;

namespace MinorEngine.engine.core
{
    public class GameObject : IDestroyable
    {
        internal static void _KeyDown(object sender, KeyboardKeyEventArgs e)
        {
            GameEngine.Instance.World.OnKeyDown(sender, e);
        }

        internal static void _KeyUp(object sender, KeyboardKeyEventArgs e)
        {
            GameEngine.Instance.World.OnKeyUp(sender, e);
        }

        internal static void _KeyPress(object sender, KeyPressEventArgs e)
        {
            GameEngine.Instance.World.OnKeyPress(sender, e);
        }

        internal static List<GameObject> ObjsWithAttachedRenderers = new List<GameObject>();
        private static Dictionary<Collidable, Collider> ObjsWithAttachedColliders = new Dictionary<Collidable, Collider>();
        internal Matrix4 _worldTransformCache;

        public IRenderingComponent RenderingComponent { get; private set; }

        public Matrix4 Transform
        {
            get
            {
                Matrix4 mat = Matrix4.Identity;
                mat *= Matrix4.CreateScale(Scale);
                mat *= Matrix4.CreateFromQuaternion(Rotation);
                mat *= Matrix4.CreateTranslation(LocalPosition);
                return mat;
            }
        }

        public Vector3 LocalPosition { get; set; } = new Vector3();
        public Vector3 Scale { get; set; } = new Vector3(1);
        public Quaternion Rotation { get; set; } = Quaternion.Identity;

        public World World { get; internal set; }
        private static int _objId;
        private readonly Dictionary<Type, AbstractComponent> _components = new Dictionary<Type, AbstractComponent>();
        private readonly List<GameObject> _children = new List<GameObject>();
        public string Name { get; set; }
        public int ChildCount => _children.Count;
        public bool Destroyed { get; private set; }
        private bool _hasRendererInHierarchy;

        public GameObject Parent { get; private set; }

        private bool _destructionPending;

        ~GameObject()
        {
            if (!Destroyed)
                Logger.Log("Object " + Name + " was garbage collected. This can cause nullpointers.",
                    DebugChannel.Warning);
        }

        public void Destroy()
        {
            _destructionPending = true;
            foreach (var gameObject in _children)
            {
                gameObject.Destroy();
            }
        }

        protected void _Destroy()
        {
            Destroyed = true;

            if (Parent != null) Remove(this);
            //this.Log("Destroying GameObject: " + Name, DebugChannel.Log);
            GameObject[] objs = new List<GameObject>(_children).ToArray();

            foreach (GameObject gameObject in objs) gameObject._Destroy();


            KeyValuePair<Type, AbstractComponent>[] comps = new List<KeyValuePair<Type, AbstractComponent>>(_components).ToArray();

            foreach (var abstractComponent in comps)
            {
                if (abstractComponent.Value is Collider collider)
                {
                    ObjsWithAttachedColliders.Remove(collider.PhysicsCollider.CollisionInformation);
                    unregisterCollider(collider);
                }
                abstractComponent.Value._Destroy();
            }
        }

        public GameObject(Vector3 localPosition, string name, GameObject parent)
        {
            LocalPosition = localPosition;
            World = World;
            Parent = parent;

            if (name == String.Empty)
            {
                Name = "Gameobject" + _objId;
                addObjCount();
            }
            else
            {
                Name = name;
            }
        }

        private static void addObjCount()
        {
            _objId++;
        }

        public GameObject(Vector3 localPosition, string name) : this(localPosition, name, null)
        {
        }

        public GameObject(string name) : this(new Vector3(), name, null)
        {
        }


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

        internal void RemoveDestroyedObjects()
        {
            if (_destructionPending)//Either Remove the object as a whole
            {
                _Destroy();
            }
            else //Or check every component if it needs removal
            {
                KeyValuePair<Type, AbstractComponent>[] comps = new List<KeyValuePair<Type, AbstractComponent>>(_components).ToArray();

                foreach (var abstractComponent in comps)
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

                foreach (var gameObject in go)
                {
                    gameObject.RemoveDestroyedObjects();
                }

            }
            //List<GameObject> go = new List<GameObject>(_children);
            //foreach (var gameObject in go)
            //{
            //    if (gameObject._destructionPending)
            //    {
            //        gameObject._Destroy();
            //    }
            //    else
            //    {

            //        KeyValuePair<Type, AbstractComponent>[] comps = new List<KeyValuePair<Type, AbstractComponent>>(gameObject._components).ToArray();

            //        foreach (var abstractComponent in comps)
            //        {
            //            if (abstractComponent.Value._destructionPending)
            //            {
            //                if (abstractComponent.Value is Collider collider)
            //                {
            //                    ObjsWithAttachedColliders.Remove(collider.PhysicsCollider.CollisionInformation);
            //                    unregisterCollider(collider);
            //                }
            //                abstractComponent.Value._Destroy();
            //            }
            //        }
            //        gameObject.RemoveDestroyedObjects();
            //    }
            //}
        }

        private void RemoveFromRenderLoop()
        {
            Logger.Log("Removing Object: " + Name + " from Rendering Loop", DebugChannel.Log);
            RemoveComponent(typeof(IRenderingComponent));
            ObjsWithAttachedRenderers.Remove(this);
        }

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

        public void RemoveComponent<T>() where T : AbstractComponent
        {
            RemoveComponent(typeof(T));
        }



        public T GetComponentIterative<T>() where T : AbstractComponent
        {
            foreach (KeyValuePair<Type, AbstractComponent> abstractComponent in _components)
                if (typeof(T).IsAssignableFrom(abstractComponent.Key))
                    return (T)abstractComponent.Value;

            return null;
        }

        public T GetComponent<T>() where T : AbstractComponent
        {
            if (_components.ContainsKey(typeof(T))) return (T)_components[typeof(T)];

            return null;
        }


        public void ComputeWorldTransformCache(Matrix4 parentTransform)
        {
            _worldTransformCache = Transform * parentTransform;
            foreach (var gameObject in _children)
                if (gameObject._hasRendererInHierarchy) //We only need to update the worldspace cache when we need to
                    gameObject.ComputeWorldTransformCache(parentTransform);
        }

        private void innerRemove(GameObject child)
        {
            for (int i = _children.Count - 1; i >= 0; i--)
                if (_children[i] == child)
                {
                    _children.RemoveAt(i);
                    child.Parent = null;
                    return;
                }
        }

        private void
            applyRenderHierarchy(
                bool hasRenderer) //This gets called from the AddComponent/RemoveComponent function and recursively from applyRenderHierarchyFromBelow
        {
            _hasRendererInHierarchy = hasRenderer;
            Parent?.applyRenderHierarchyFromBelow(hasRenderer); //Call the parent 
        }

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
                foreach (var gameObject in _children)
                    if (gameObject._hasRendererInHierarchy)
                    {
                        childhaveRenderers = true;
                        break;
                    }

                if (!childhaveRenderers) applyRenderHierarchy(hasRenderer);
            }

            //bool ret = true;
            //foreach (var gameObject in _children)
            //{
            //    if (gameObject._hasRendererInHierarchy)
            //    {
            //        ret = false;
            //    }
            //}

            //if (ret)
            //{
            //    //applyRenderHierarchy(hasRenderer); //If no other child has a renderer in hierarchy then remove it.
            //}
        }

        private void innerAdd(GameObject child)
        {
            _children.Add(child);
            child.Parent = this;
        }


        public void Add(GameObject child)
        {
            if (child._hasRendererInHierarchy) applyRenderHierarchy(true);
            child.SetParent(this);
        }

        public static void Remove(GameObject child)
        {
            child.SetParent(null);
        }

        public void SetParent(GameObject newParent)
        {
            Parent?.innerRemove(this);
            newParent?.innerAdd(this);

            if (Parent != null)
                setWorldRecursively(Parent.World);
            else
                setWorldRecursively(null);
        }

        internal void registerCollider(Collider coll)
        {
            coll.PhysicsCollider.CollisionInformation.Events.ContactCreated += Events_ContactCreated;
            coll.PhysicsCollider.CollisionInformation.Events.ContactRemoved += Events_ContactRemoved;
            coll.PhysicsCollider.CollisionInformation.Events.CollisionEnded += Events_CollisionEnded;
            coll.PhysicsCollider.CollisionInformation.Events.InitialCollisionDetected += Events_InitialCollisionDetected;
        }
        internal void unregisterCollider(Collider coll)
        {
            coll.PhysicsCollider.CollisionInformation.Events.ContactCreated -= Events_ContactCreated;
            coll.PhysicsCollider.CollisionInformation.Events.ContactRemoved -= Events_ContactRemoved;
            coll.PhysicsCollider.CollisionInformation.Events.CollisionEnded -= Events_CollisionEnded;
            coll.PhysicsCollider.CollisionInformation.Events.InitialCollisionDetected -= Events_InitialCollisionDetected;
        }

        private void Events_InitialCollisionDetected(EntityCollidable sender, Collidable other, CollidablePairHandler pair)
        {
            if (ObjsWithAttachedColliders.TryGetValue(other, out Collider otherCol))
                foreach (var abstractComponent in _components) abstractComponent.Value.onInitialCollisionDetected(otherCol, pair);
        }

        private void Events_CollisionEnded(EntityCollidable sender, Collidable other, CollidablePairHandler pair)
        {
            if (ObjsWithAttachedColliders.TryGetValue(other, out Collider otherCol))
                foreach (var abstractComponent in _components) abstractComponent.Value.onCollisionEnded(otherCol, pair);
        }

        private void Events_ContactRemoved(EntityCollidable sender, Collidable other, CollidablePairHandler pair, ContactData contact)
        {
            if (ObjsWithAttachedColliders.TryGetValue(other, out Collider otherCol))
                foreach (var abstractComponent in _components) abstractComponent.Value.onContactRemoved(otherCol, pair, contact);
        }

        private void Events_ContactCreated(EntityCollidable sender, Collidable other, CollidablePairHandler pair, ContactData contact)
        {
            if (ObjsWithAttachedColliders.TryGetValue(other, out Collider otherCol))
                foreach (var abstractComponent in _components) abstractComponent.Value.onContactCreated(otherCol, pair, contact);
        }





        private void OnKeyPress(object sender, KeyPressEventArgs e)
        {
            foreach (var abstractComponent in _components) abstractComponent.Value.onPress(sender, e);

            for (int i = 0; i < _children.Count; i++) _children[i].OnKeyPress(sender, e);
        }

        private void OnKeyUp(object sender, KeyboardKeyEventArgs e)
        {
            foreach (var abstractComponent in _components) abstractComponent.Value.onKeyUp(sender, e);

            for (int i = 0; i < _children.Count; i++) _children[i].OnKeyUp(sender, e);
        }

        private void OnKeyDown(object sender, KeyboardKeyEventArgs e)
        {
            foreach (var abstractComponent in _components) abstractComponent.Value.onKeyDown(sender, e);

            for (int i = 0; i < _children.Count; i++) _children[i].OnKeyDown(sender, e);
        }

        public void Update(float deltaTime)
        {
            foreach (var abstractComponent in _components) abstractComponent.Value.updateObject(deltaTime);

            for (int i = 0; i < _children.Count; i++) _children[i].Update(deltaTime);
        }


        private void setWorldRecursively(World newWorld)
        {
            World = newWorld;

            for (int i = 0; i < _children.Count; i++) _children[i].setWorldRecursively(newWorld);
        }

        public GameObject GetChildAt(int idx)
        {
            if (idx >= 0 && idx < _children.Count) return _children[idx];
            return null;
        }

        public GameObject GetChildWithName(string name)
        {
            if (name == Name) return this;

            foreach (var gameObject in _children)
            {
                GameObject ret = gameObject.GetChildWithName(name);
                if (ret != null) return ret;
            }

            return null;
        }


        public void Translate(Vector3 translation)
        {
            LocalPosition += translation;
        }

        public void ScaleBy(Vector3 scaleAmount)
        {
            Scale *= scaleAmount;
        }

        public Vector3 GetScale()
        {
            return Transform.ExtractScale();
        }

        public void Rotate(Vector3 axis, float angle)
        {
            Rotation *= Quaternion.FromAxisAngle(axis, angle);
        }

        public void SetRotation(Quaternion rot)
        {
            Rotation = rot;
        }


        public Vector3 TransformToWorld(Vector3 vec, bool translate = true)
        {
            Vector4 v = translate ? new Vector4(vec, 1) : new Vector4(vec, 0);
            return new Vector3(v * GetWorldTransform());
        }

        public Vector3 GetWorldPosition()
        {
            return TransformToWorld(GetLocalPosition(), true);
        }
        public Matrix4 GetWorldTransform()
        {
            if (Parent == null)
                return Transform;
            return Parent.GetWorldTransform() * Transform;
        }

        public Vector3 GetLocalPosition()
        {
            return Transform.ExtractTranslation();
        }

        public Quaternion GetOrientation()
        {
            return Transform.ExtractRotation();
        }
        public void SetLocalPosition(Vector3 pos)
        {
            LocalPosition = pos;
        }

        public void LookAt(GameObject other)
        {

            Vector3 target = new Vector3(new Vector4(other.GetLocalPosition(), 0) * other.GetWorldTransform());
            LookAt(target);
        }

        public void LookAt(Vector3 worldPos)
        {

            Vector3 position = GetLocalPosition();

            Vector3 t = worldPos - position;


            if (t == Vector3.Zero) return;

            Vector3 newForward = Vector3.Normalize(t);

            //New Right Vector
            Vector3 newRight = Vector3.Cross(Vector3.UnitY, newForward);
            Vector3 newUp = Vector3.Cross(newForward, newRight);


            Matrix3 newMat = new Matrix3(new Vector3(-newRight), new Vector3(newUp), new Vector3(-newForward));
            Rotation = newMat.ExtractRotation();

        }

        //public void LookAtGlobal(GameObject other)
        //{
        //    Matrix4 worldOther = other.GetWorldTransform();
        //    Vector3 position = GetLocalPosition();
        //    Vector3 target = new Vector3(new Vector4(other.GetLocalPosition(), 0) * worldOther);



        //    Vector3 t = target - position;


        //    if (t == Vector3.Zero) return;


        //    Vector3 newForward = Vector3.Normalize(t);


        //    //New Right Vector
        //    Vector3 newRight = Vector3.Cross(Vector3.UnitY, newForward);
        //    Vector3 newUp = Vector3.Cross(newForward, newRight);


        //}

        //public void LookAt(GameObject other)
        //{
        //    Matrix4 worldThis = GetWorldTransform();
        //    Matrix4 worldOther = other.GetWorldTransform();
        //    Matrix4 otherThis = worldOther * Matrix4.Invert(worldThis);
        //    Vector3 position = GetLocalPosition();
        //    Vector3 target = new Vector3(new Vector4(other.GetLocalPosition(), 0) * otherThis);


        //    this.Log("New Target: " + target, DebugChannel.Log);

        //    Vector3 t = target - position;

        //    Vector3 newForward = Vector3.Normalize(t);

        //    //New Right Vector
        //    Vector3 newRight = Vector3.Cross(Vector3.UnitY, newForward);
        //    Vector3 newUp = Vector3.Cross(newForward, newRight);
        //    Matrix4 newMat = new Matrix4(new Vector4(-newRight), new Vector4(newUp), new Vector4(-newForward),
        //        new Vector4(position, 1));
        //    //Transform = newMat;

        //}
    }
}