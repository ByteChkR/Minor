using System;
using System.Collections.Generic;
using Engine.Debug;
using Engine.Exceptions;

namespace Engine.Core
{
    /// <summary>
    /// A Pooled object of type T
    /// </summary>
    /// <typeparam name="T">Type of Object</typeparam>
    public struct PooledObject<T> where T : IDisposable
    {
        /// <summary>
        /// The Object that it stores
        /// </summary>
        public readonly T Object;

        /// <summary>
        /// A flag that indicates if this object is currently in use
        /// </summary>
        public bool IsUsed { get; private set; }

        /// <summary>
        /// A reference to the containing pool
        /// </summary>
        public ObjectPool<T> ContainingPool { get; }

        /// <summary>
        /// The Pool handle
        /// </summary>
        public int PoolHandle { get; }


        /// <summary>
        /// Public constructor
        /// </summary>
        /// <param name="value">The Value of Type T that will be stored in this object</param>
        /// <param name="containingPool">The containing pool</param>
        /// <param name="poolHandle">The pool handle</param>
        public PooledObject(T value, ObjectPool<T> containingPool, int poolHandle)
        {
            Object = value;
            IsUsed = false;
            ContainingPool = containingPool;
            PoolHandle = poolHandle;
        }

        /// <summary>
        /// Returns this object to the pool
        /// </summary>
        public void GiveBack()
        {
            ContainingPool?.Give(this);
        }

        /// <summary>
        /// Internal Function that sets the use state for a pooled object
        /// </summary>
        /// <param name="state"></param>
        internal void SetIsUsed(bool state)
        {
            IsUsed = state;
        }

        /// <summary>
        /// Implicit cast of Pooled Object of Type T into T
        /// </summary>
        /// <param name="pooledInstance"></param>
        public static implicit operator T(PooledObject<T> pooledInstance)
        {
            return pooledInstance.Object;
        }
    }

    /// <summary>
    /// Object Pool of Type T
    /// </summary>
    /// <typeparam name="T">Type of Object</typeparam>
    public class ObjectPool<T> : IDisposable where T : IDisposable
    {
        /// <summary>
        /// Delegate that serves as a "Factory" of Objects
        /// </summary>
        /// <returns>A new instance of an object of type T</returns>
        public delegate T CreateNew();

        /// <summary>
        /// The Factory that is creating new instances of type T
        /// </summary>
        private readonly CreateNew _factory;

        /// <summary>
        /// The Maximum amount of items allowed
        /// </summary>
        private readonly int _maxItems;

        /// <summary>
        /// The internal list of pooled objects
        /// </summary>
        private List<PooledObject<T>> _InternalList = new List<PooledObject<T>>();

        /// <summary>
        /// The last objects index + 1
        /// serves as a starting point for the next unused item
        /// </summary>
        private int _nextID;

        /// <summary>
        /// Public Constructor
        /// </summary>
        /// <param name="size">The initial size of the pool</param>
        /// <param name="maxSize">The maximum size of the pool</param>
        /// <param name="factory">The factory that is providing the new objects</param>
        public ObjectPool(int size, int maxSize, CreateNew factory)
        {
            if (size > maxSize)
            {
                Logger.Crash(new ObjectPoolException("Object Pool size is bigger than its defined max size."), true);
            }

            if (factory == null)
            {
                Logger.Crash(new ObjectPoolException("No Factory passted to ObjectPool Constructor"), false);
            }


            _factory = factory;

            _maxItems = Math.Min(maxSize, size);
            InitializeSize(size);
        }

        /// <summary>
        /// Creates a new Object pool with the default values
        /// </summary>
        /// <param name="factory">The factory producing new instances of Type T</param>
        public ObjectPool(CreateNew factory) : this(0, 1000, factory)
        {
        }

        /// <summary>
        /// Disposes all objects in the pool
        /// Warning: all objects from the pool become unusable
        /// </summary>
        public void Dispose()
        {
            for (int i = 0; i < _InternalList.Count; i++)
            {
                _InternalList[i].Object.Dispose();
            }

            _InternalList.Clear();
        }

        /// <summary>
        /// Inizializes the Pool with a specified size
        /// </summary>
        /// <param name="size">The initial size</param>
        private void InitializeSize(int size)
        {
            for (int i = 0; i < size; i++)
            {
                T ret = _factory();

                _InternalList.Add(new PooledObject<T>(ret, this, i));
            }
        }

        /// <summary>
        /// Returns the next index of a pooled object that is not in use.
        /// starting at the start index
        /// if it reached the end while not finding a free object. it will do a complete iteration to make sure that there are no free objects before returning -1
        /// </summary>
        /// <param name="startidx">the start index of the search operation</param>
        /// <returns>The index of the next free object; -1 if there is none</returns>
        private int findNext(int startidx)
        {
            for (int i = startidx; i < _InternalList.Count; i++)
            {
                if (!_InternalList[i].IsUsed)
                {
                    return i;
                }
            }

            for (int i = 0; i < startidx - 1; i++)
            {
                if (!_InternalList[i].IsUsed)
                {
                    return i;
                }
            }

            return -1;
        }

        /// <summary>
        /// Returns an index of a free object
        /// Adds to the list if not maxcount if no objects have been found
        /// </summary>
        /// <returns>index of a free object</returns>
        private int GetFreeID()
        {
            if (_nextID >= _InternalList.Count)
            {
                _nextID = 0;
            }

            int id = findNext(_nextID);
            if (id == -1 && _InternalList.Count < _maxItems) //No free objects found
            {
                id = _InternalList.Count;
                _InternalList.Add(new PooledObject<T>(_factory(), this, id));
            }

            return id;
        }

        /// <summary>
        /// returns an object to the pool
        /// e.g. making it available for use again
        /// </summary>
        /// <param name="item"></param>
        public void Give(PooledObject<T> item)
        {
            if (item.ContainingPool == this && item.PoolHandle != -1 && item.PoolHandle < _InternalList.Count)
            {
                _InternalList[item.PoolHandle].SetIsUsed(false);
            }
        }

        /// <summary>
        /// Takes an object from the pool.
        /// if the pool has no free objects and it as maximum capacity it will log a warning and return unmanaged instances
        /// </summary>
        /// <returns>An object</returns>
        public PooledObject<T> Take()
        {
            int id = GetFreeID();
            if (id == -1)
            {
                Logger.Log("Object Pool is full, returning Unmanaged Instance.",
                    DebugChannel.Warning | DebugChannel.EngineCore, 10);
                PooledObject<T> item = new PooledObject<T>(_factory(), null, -1);

                return item;
            }

            _InternalList[id].SetIsUsed(true);
            return _InternalList[id];
        }

        /// <summary>
        /// Disposes all unused objects in the pool
        /// </summary>
        public void Clean()
        {
            for (int i = _InternalList.Count - 1; i >= 0; i--)
            {
                if (!_InternalList[i].IsUsed)
                {
                    _InternalList[i].Object.Dispose();
                    _InternalList.RemoveAt(i);
                }
            }
        }
    }
}