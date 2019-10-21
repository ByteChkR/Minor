using System;
using System.Collections.Generic;

namespace Engine.AI
{
    public class PriorityQueue<T> where T : IComparable<T>
    {
        //The underlying structure.
        private List<T> list;
        public int Count { get { return list.Count; } }
        public readonly bool IsDescending;

        public PriorityQueue()
        {
            list = new List<T>();
        }

        public PriorityQueue(bool isdesc)
            : this(0, isdesc)
        {
        }

        public PriorityQueue(int capacity)
            : this(capacity, false)
        { }

        public PriorityQueue(IEnumerable<T> collection)
            : this(collection, false)
        { }

        public PriorityQueue(int capacity, bool isdesc)
        {
            list = new List<T>(capacity);
            IsDescending = isdesc;
        }

        public PriorityQueue(IEnumerable<T> collection, bool isdesc)
            : this()
        {
            IsDescending = isdesc;
            foreach (var item in collection)
                Enqueue(item);
        }


        public void Enqueue(T x)
        {

            list.Add(x);
            int i = Count - 1; //Position of x

            while (i > 0)
            {
                int p = (i - 1) / 2; //Start at half of i
                if ((IsDescending ? -1 : 1) * list[p].CompareTo(x) <= 0) break; //

                list[i] = list[p];//Put P to position of i
                i = p;//I = (I-1)/2
            }

            if (Count > 0) list[i] = x; //If while loop way executed at least once(X got replaced by some p), add it to the list
        }

        public T Dequeue()
        {

            T target = Peek(); //Get first in list
            T root = list[Count - 1]; //Hold last of the list
            list.RemoveAt(Count - 1); //But remove it from the list

            int i = 0;
            while (i * 2 + 1 < Count)
            {
                int a = i * 2 + 1;//Every second entry starting by 1
                int b = i * 2 + 2;//Every second entries neighbour
                int c = b < Count && (IsDescending ? -1 : 1) * list[b].CompareTo(list[a]) < 0 ? b : a; //Wether B(B is in range && B is smaller than A) or A

                if ((IsDescending ? -1 : 1) * list[c].CompareTo(root) >= 0) break; //
                list[i] = list[c];
                i = c;
            }

            if (Count > 0) list[i] = root;
            return target;
        }

        public T Peek()
        {
            if (Count == 0) throw new InvalidOperationException("Queue is empty.");
            return list[0];
        }

        public void Clear()
        {
            list.Clear();
        }

        public List<T> GetData()
        {
            return list;
        }
    }
}