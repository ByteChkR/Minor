using System;
using System.Collections.Generic;

namespace Engine.AI
{
    /// <summary>
    /// Simple Priority Queue implementation
    /// </summary>
    /// <typeparam name="T">Type of Queue Item</typeparam>
    public class PriorityQueue<T> where T : IComparable<T>
    {
        public readonly bool IsDescending;

        //The underlying structure.
        private readonly List<T> list;

        /// <summary>
        /// Default Constructor
        /// </summary>
        public PriorityQueue()
        {
            list = new List<T>();
        }

        /// <summary>
        /// Public constructor
        /// </summary>
        /// <param name="isDescending"></param>
        public PriorityQueue(bool isDescending)
            : this(0, isDescending)
        {
        }

        /// <summary>
        /// Public constructor
        /// </summary>
        /// <param name="capacity">internal capacity</param>
        public PriorityQueue(int capacity)
            : this(capacity, false)
        {
        }

        /// <summary>
        /// Public constructor
        /// </summary>
        /// <param name="collection">Base Collection. Needs to be Sorted</param>
        public PriorityQueue(IEnumerable<T> collection)
            : this(collection, false)
        {
        }

        /// <summary>
        /// Public constructor
        /// </summary>
        /// <param name="capacity">internal capacity</param>
        /// <param name="isDescending"></param>
        public PriorityQueue(int capacity, bool isDescending)
        {
            list = new List<T>(capacity);
            IsDescending = isDescending;
        }

        /// <summary>
        /// Public constructor
        /// </summary>
        /// <param name="collection">Base Collection. Needs to be Sorted</param>
        /// <param name="isDescending"></param>
        public PriorityQueue(IEnumerable<T> collection, bool isDescending)
            : this()
        {
            IsDescending = isDescending;
            foreach (T item in collection)
            {
                Enqueue(item);
            }
        }

        /// <summary>
        /// Number of items in the queue
        /// </summary>
        public int Count => list.Count;

        /// <summary>
        /// Enqueues an item into a queue
        /// </summary>
        /// <param name="x">The item to Enqueue</param>
        public void Enqueue(T x)
        {
            list.Add(x);
            int i = Count - 1; //Position of x

            while (i > 0)
            {
                int p = (i - 1) / 2; //Start at half of i
                if ((IsDescending ? -1 : 1) * list[p].CompareTo(x) <= 0)
                {
                    break; //
                }

                list[i] = list[p]; //Put P to position of i
                i = p; //I = (I-1)/2
            }

            if (Count > 0)
            {
                list[i] = x; //If while loop way executed at least once(X got replaced by some p), add it to the list
            }
        }

        /// <summary>
        /// Dequeues an item
        /// </summary>
        /// <returns></returns>
        public T Dequeue()
        {
            T target = Peek(); //Get first in list
            T root = list[Count - 1]; //Hold last of the list
            list.RemoveAt(Count - 1); //But remove it from the list

            int i = 0;
            while (i * 2 + 1 < Count)
            {
                int a = i * 2 + 1; //Every second entry starting by 1
                int b = i * 2 + 2; //Every second entries neighbour
                int c = b < Count && (IsDescending ? -1 : 1) * list[b].CompareTo(list[a]) < 0
                    ? b
                    : a; //Wether B(B is in range && B is smaller than A) or A

                if ((IsDescending ? -1 : 1) * list[c].CompareTo(root) >= 0)
                {
                    break; //
                }

                list[i] = list[c];
                i = c;
            }

            if (Count > 0)
            {
                list[i] = root;
            }

            return target;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>Item in the first position of the queue</returns>
        public T Peek()
        {
            if (Count == 0)
            {
                throw new InvalidOperationException("Queue is empty.");
            }

            return list[0];
        }

        /// <summary>
        /// Clears the complete queue
        /// </summary>
        public void Clear()
        {
            list.Clear();
        }

        /// <summary>
        /// Returns all items in the queue
        /// </summary>
        /// <returns>the data as a list of type T</returns>
        public List<T> GetData()
        {
            return list;
        }
    }
}