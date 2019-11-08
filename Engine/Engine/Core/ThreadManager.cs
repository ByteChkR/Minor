using System;
using System.Threading;

namespace Engine.Core
{
    public static class ThreadManager<T>
    {
        public delegate void OnThreadFinish(T result);
        public delegate T Func();


        public static void RunInThread(Func func, OnThreadFinish onFinish)
        {
            Thread t = new Thread(() => ThreadFunction(func, onFinish));
            t.Start();
        }

        private static void ThreadFunction(Func action, OnThreadFinish onFinish)
        {
            T item = action.Invoke();
            onFinish?.Invoke(item);
        }



    }
}