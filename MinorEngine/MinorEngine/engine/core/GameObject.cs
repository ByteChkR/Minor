﻿using System;
using System.Collections.Generic;
using Common;
using MinorEngine.components;
using MinorEngine.engine.components;
using OpenTK;
using OpenTK.Input;
using Quaternion = BepuUtilities.Quaternion;

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
        internal Matrix4 _worldTransformCache;

        public IRenderingComponent RenderingComponent { get; private set; }
        public Matrix4 Transform { get; set; } = Matrix4.Identity;
        public World World { get; internal set; }
        private static int _objId;
        private readonly Dictionary<Type, AbstractComponent> _components = new Dictionary<Type, AbstractComponent>();
        private readonly List<GameObject> _children = new List<GameObject>();
        public string Name { get; set; }
        public int ChildCount => _children.Count;
        private bool _destroyed;
        private bool _hasRendererInHierarchy;

        public GameObject Parent { get; private set; }

        ~GameObject()
        {
            if (!_destroyed)
                this.Log("Object " + Name + " was garbage collected. This can cause nullpointers.",
                    DebugChannel.Warning);
        }

        public void Destroy()
        {
            _destroyed = true;

            if (RenderingComponent != null) ObjsWithAttachedRenderers.Remove(this);

            if (Parent != null) Remove(this);
            this.Log("Destroying GameObject: " + Name, DebugChannel.Log);
            GameObject[] objs = new List<GameObject>(_children).ToArray();

            foreach (GameObject gameObject in objs) gameObject.Destroy();

            foreach (var abstractComponent in _components) abstractComponent.Value.Destroy();
        }

        public GameObject(Vector3 position, string name, GameObject parent)
        {
            Transform *= Matrix4.CreateTranslation(position);
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

        public GameObject(Vector3 position, string name) : this(position, name, null)
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
                    RenderingComponent = (IRenderingComponent) component;
                }

                _components.Add(t, component);
                component.Owner = this;
            }
        }

        public void RemoveComponent<T>() where T : AbstractComponent
        {
            Type t = typeof(T);
            if (_components.ContainsKey(t))
            {
                applyRenderHierarchy(false);
                ObjsWithAttachedRenderers.Remove(this);

                AbstractComponent component = _components[t];
                _components.Remove(t);
                component.Owner = null;
            }
        }

        public T GetComponentIterative<T>() where T : AbstractComponent
        {
            foreach (KeyValuePair<Type, AbstractComponent> abstractComponent in _components)
                if (typeof(T).IsAssignableFrom(abstractComponent.Key))
                    return (T) abstractComponent.Value;

            return null;
        }

        public T GetComponent<T>() where T : AbstractComponent
        {
            if (_components.ContainsKey(typeof(T))) return (T) _components[typeof(T)];

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
            Transform *= Matrix4.CreateTranslation(translation);
        }

        public void Scale(Vector3 scaleAmount)
        {
            Transform *= Matrix4.CreateScale(scaleAmount);
        }

        public Vector3 GetScale()
        {
            return Transform.ExtractScale();
        }

        public void Rotate(Vector3 axis, float angle)
        {
            Vector3 translation = GetLocalPosition();
            Transform = Transform.ClearTranslation() * Matrix4.CreateFromAxisAngle(axis, angle);
            Translate(translation);
        }

        public void SetRotation(Quaternion rot)
        {
            Transform = Matrix4.CreateFromQuaternion(new OpenTK.Quaternion(rot.X, rot.Y, rot.Z, rot.W)) *
                        Transform.ClearRotation();
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

        public void SetLocalPosition(Vector3 pos)
        {
            Matrix4 f = Transform;
            f.Row3 = new Vector4(pos, 1);
            Transform = f;
        }

        public void LookAt(Vector3 worldPos)
        {
            Vector3 eye, up, target;
            eye = GetLocalPosition();
            target = new Vector3(new Vector4(worldPos) * Matrix4.Invert(GetWorldTransform()));
            up = Vector3.UnitY;
            Transform = Matrix4.LookAt(eye, target, up) * Transform.ClearRotation();
        }


        public void LookAt(GameObject other)
        {
            Matrix4 worldThis = GetWorldTransform();
            Matrix4 worldOther = other.GetWorldTransform();
            Matrix4 otherThis = worldOther * Matrix4.Invert(worldThis);
            Vector3 position = GetLocalPosition();
            Vector3 target = new Vector3(new Vector4(other.GetLocalPosition(), 0) * otherThis);
            Vector3 scale = GetScale();

            Vector3 t = target - position;

            Vector3 newForward = Vector3.Normalize(t);

            //New Right Vector
            Vector3 newRight = Vector3.Cross(Vector3.UnitY, newForward);
            Vector3 newUp = Vector3.Cross(newForward, newRight);

            Transform = new Matrix4(new Vector4(-newRight), new Vector4(newUp), new Vector4(-newForward),
                new Vector4(position, 1));

            //Translate(position);

            //Scale(scale);

            ////Positions in THIS local space
            ////If other.LocalPosition.w == 1 then it glitches
            ////If its 0 the target.Y = this.Y
            //Vector3 target = new Vector3(new Vector4(other.GetLocalPosition(), 1) * worldOther * Matrix4.Invert(worldThis));
            //Vector3 eye = GetLocalPosition();


            ////Non Parallel Vector to forward
            //Vector3 up = Vector3.UnitY;

            ////New Forward Vector
            //Vector3 newForward = target - eye;
            //newForward = Vector3.Normalize(newForward);

            ////New Right Vector
            //Vector3 newRight = Vector3.Cross(up, newForward);
            //Vector3 newUp = Vector3.Cross(newForward, newRight);


            //Transform = new Matrix4(new Vector4(newRight), new Vector4(newUp), new Vector4(newForward), new Vector4(0, 0, 0, 1));
            //Matrix4 translation = Matrix4.CreateTranslation(eye);
            ////Transform *= translation;


            ////Looking at Target with new Up vector
            ////Transform = Matrix4.LookAt(eye, target, up);
        }
    }
}