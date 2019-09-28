using System;
using System.Threading;
using BepuUtilities;
using BepuUtilities.Memory;

namespace GameEngine.engine.physics
{
    public class ThreadDispatcher : IThreadDispatcher, IDisposable
    {
        private readonly int _threadCount;
        public int ThreadCount => _threadCount;
        struct Worker
        {
            public Thread Thread;
            public AutoResetEvent Signal;
        }

        private readonly Worker[] _workers;
        private readonly AutoResetEvent _finished;
        private readonly BufferPool[] _bufferPools;

        volatile Action<int> _workerBody;
        private int _workerIndex;
        private int _completedWorkerCounter;

        public ThreadDispatcher(int threadCount)
        {
            this._threadCount = threadCount;
            _workers = new Worker[threadCount - 1];
            for (int i = 0; i < _workers.Length; ++i)
            {
                _workers[i] = new Worker { Thread = new Thread(WorkerLoop), Signal = new AutoResetEvent(false) };
                _workers[i].Thread.IsBackground = true;
                _workers[i].Thread.Start(_workers[i].Signal);
            }
            _finished = new AutoResetEvent(false);
            _bufferPools = new BufferPool[threadCount];
            for (int i = 0; i < _bufferPools.Length; ++i)
            {
                _bufferPools[i] = new BufferPool();
            }
        }

        void DispatchThread(int workerIndex)
        {
            _workerBody(workerIndex);

            if (Interlocked.Increment(ref _completedWorkerCounter) == _threadCount)
            {
                _finished.Set();
            }
        }


        void WorkerLoop(object untypedSignal)
        {
            var signal = (AutoResetEvent)untypedSignal;
            while (true)
            {
                signal.WaitOne();
                if (disposed)
                {
                    return;
                }
                DispatchThread(Interlocked.Increment(ref _workerIndex) - 1);
            }
        }

        void SignalThreads()
        {
            for (int i = 0; i < _workers.Length; ++i)
            {
                _workers[i].Signal.Set();
            }
        }

        public void DispatchWorkers(Action<int> workerBody)
        {
            _workerIndex = 1; //Just make the inline thread worker 0. While the other threads might start executing first, the user should never rely on the dispatch order.
            _completedWorkerCounter = 0;
            this._workerBody = workerBody;
            SignalThreads();
            //Calling thread does work. No reason to spin up another worker and block this one!
            DispatchThread(0);
            _finished.WaitOne();
            this._workerBody = null;
        }

        volatile bool disposed;
        public void Dispose()
        {
            if (!disposed)
            {
                disposed = true;
                SignalThreads();
                for (int i = 0; i < _bufferPools.Length; ++i)
                {
                    _bufferPools[i].Clear();
                }
                foreach (var worker in _workers)
                {
                    worker.Thread.Join();
                    worker.Signal.Dispose();
                }
            }
        }

        public BufferPool GetThreadMemoryPool(int workerIndex)
        {
            return _bufferPools[workerIndex];
        }

    }
}