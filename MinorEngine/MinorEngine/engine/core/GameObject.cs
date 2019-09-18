using System;
using System.Collections.Generic;
using Assimp;
using GameEngine.engine.components;
using GameEngine.engine.rendering;
using OpenTK;

namespace GameEngine.engine.core
{
    public class GameObject
    {
        public ShaderProgram Shader { get; set; }
        public GameModel Model { get; set; }
        public Matrix4 Transform { get; set; } = Matrix4.Identity;
        protected World World { get; set; }
        private static int _objId = 1;
        private readonly Dictionary<Type, AbstractComponent> _components = new Dictionary<Type, AbstractComponent>();
        private readonly List<GameObject> _children = new List<GameObject>();
        public string Name { get; set; }
        public int ChildCount => _children.Count;

        public GameObject Parent { get; private set; }


        public GameObject(Vector3 position, string name, GameObject parent)
        {
            Transform *= Matrix4.CreateTranslation(position);
            this.World = World;
            this.Parent = parent;
            if (name == string.Empty)
            {
                Name = "Gameobject" + _objId;
                _objId++;
            }
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
                _components.Add(t, component);
                component.Owner = this;
            }
        }

        public void RemoveComponent<T>() where T : AbstractComponent
        {
            Type t = typeof(T);
            if (_components.ContainsKey(t))
            {
                AbstractComponent component = _components[t];
                _components.Remove(t);
                component.Owner = null;
            }
        }

        public T GetComponent<T>() where T : AbstractComponent
        {
            if (_components.ContainsKey(typeof(T)))
            {
                return (T)_components[typeof(T)];
            }

            return null;
        }

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

        private void innerAdd(GameObject child)
        {
            _children.Add(child);
            child.Parent = this;
        }


        public void Add(GameObject child)
        {
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
            {
                setWorldRecursively(Parent.World);
            }
            else
            {
                setWorldRecursively(null);
            }
        }

        public void Update(float deltaTime)
        {
            foreach (var abstractComponent in _components)
            {
                abstractComponent.Value.Update(deltaTime);
            }

            for (int i = 0; i < _children.Count; i++)
            {
                _children[i].Update(deltaTime);
            }
        }


        private void setWorldRecursively(World newWorld)
        {
            World = newWorld;

            for (int i = 0; i < _children.Count; i++)
            {
                _children[i].setWorldRecursively(newWorld);
            }
        }

        public GameObject GetChildAt(int idx)
        {
            if (idx >= 0 && idx < _children.Count)
            {
                return _children[idx];
            }
            return null;
        }


        public void Translate(Vector3 translation)
        {
            Transform *= Matrix4.CreateTranslation(translation);
        }

        public void Scale(Vector3 scale)
        {
            Transform *= Matrix4.CreateScale(scale);
        }

        public void Rotate(Vector3 axis, float angle)
        {
            Transform *= Matrix4.CreateFromAxisAngle(axis, angle);
        }

        public Matrix4 GetWorldTransform()
        {
            if (Parent == null)
            {
                return Transform;
            }
            else
            {
                return Parent.GetWorldTransform() * Transform;
            }
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

        public void LookAt(GameObject other)
        {
            Matrix4 worldThis = GetWorldTransform();
            Matrix4 worldOther = other.GetWorldTransform();
            Vector3 eye, up, target;
            eye = GetLocalPosition();
            target = new Vector3(new Vector4(other.GetLocalPosition()) * worldOther * Matrix4.Invert(worldThis));
            up = Vector3.UnitY;
            Transform = Matrix4.LookAt(eye, target, up);
        }


    }
}