using System;
using System.Collections.Generic;
using MinorEngine.debug;

namespace MinorEngine.engine.core
{
    public struct PooledObject<T> where T : IDisposable
    {
        public readonly T Object;
        public bool IsUsed { get; private set; }
        public ObjectPool<T> ContainingPool { get; }
        public int PoolHandle { get; }
        public PooledObject(T value, ObjectPool<T> containingPool, int poolHandle)
        {
            Object = value;
            IsUsed = false;
            ContainingPool = containingPool;
            PoolHandle = poolHandle;
        }

        public void GiveBack()
        {
            ContainingPool?.Give(this);
        }

        internal void SetIsUsed(bool state)
        {
            IsUsed = state;
        }

        public static implicit operator T(PooledObject<T> pooledInstance)
        {
            return pooledInstance.Object;
        }


    }

    public class ObjectPool<T> : IDisposable where T : IDisposable
    {
        public delegate T CreateNew();

        private List< PooledObject<T>> _InternalList = new List<PooledObject<T>>();
        private int _nextID = 0;
        private int _maxItems;
        private CreateNew _Factory;
        public ObjectPool(int size, int maxSize, CreateNew factory)
        {
            if (size > maxSize) throw new Exception("Object Pool size is bigger than its defined max size.");
            _Factory = factory ?? throw new Exception("No Factory passted to ObjectPool Constructor");

            _maxItems = maxSize;
            InitializeSize(size);
        }

        public ObjectPool(CreateNew factory) : this(0, 1000, factory)
        {
        }

        public void AddToPool(T item)
        {
            _InternalList.Add(new PooledObject<T>(item, this, _InternalList.Count - 1));
        }

        private void InitializeSize(int size)
        {
            for (int i = 0; i < size; i++)
            {
                T ret = _Factory();

                _InternalList.Add(new PooledObject<T>(ret, this, i));
            }
        }

        private int findNext(int startidx)
        {
            for (int i = startidx; i < _InternalList.Count; i++)
            {
                if (!_InternalList[i].IsUsed) return i;
            }

            for (int i = 0; i < startidx - 1; i++)
            {
                if (!_InternalList[i].IsUsed) return i;
            }

            return -1;
        }

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
                _InternalList.Add(new PooledObject<T>(_Factory(), this, id));

            }

            return id;
        }

        public void Give(PooledObject<T> item)
        {
            if (item.ContainingPool == this && item.PoolHandle != -1 && item.PoolHandle < _InternalList.Count)
            {
                _InternalList[item.PoolHandle].SetIsUsed(false);
            }
        }

        public PooledObject<T> Take()
        {
            int id = GetFreeID();
            if (id == -1)
            {
                this.Log("Object Pool is full, returning Unmanaged Instance.", DebugChannel.Warning);
                PooledObject<T> item = new PooledObject<T>(_Factory(), null, -1);

                return item;
            }

            _InternalList[id].SetIsUsed(true);
            return _InternalList[id];
        }

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

        public void Dispose()
        {
            for (int i = 0; i < _InternalList.Count; i++)
            {
                _InternalList[i].Object.Dispose();
            }

            _InternalList.Clear();
        }

    }
}