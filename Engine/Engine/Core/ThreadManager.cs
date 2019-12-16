using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Engine.Core
{

    /// <summary>
    /// Helper Interface for the generic ThreadManager implementation.
    /// 
    /// </summary>
    internal interface IThreadManager
    {
        /// <summary>
        /// Type of the Thread Manager 
        /// </summary>
        Type Type { get; }

        /// <summary>
        /// Checks the States of the currently running tasks
        /// </summary>
        /// <returns>True if all tasks finished</returns>
        bool CheckStates();
    }


    /// <summary>
    /// A reference to a specific task item
    /// </summary>
    /// <typeparam name="T">Type of endresult</typeparam>
    public class TaskReference<T>
    {
        /// <summary>
        /// Delegate used by the TaskReference implementation
        /// </summary>
        /// <returns></returns>
        public delegate T DelTask();

        private T ret;
        private Action<T> onFinish;
        private Thread t;

        /// <summary>
        /// Internal Constructor
        /// </summary>
        /// <param name="task">Task to complete</param>
        /// <param name="onFinish">On Finish Event</param>
        internal TaskReference(DelTask task, Action<T> onFinish)
        {
            this.onFinish = onFinish;
            t = new Thread(() => ThreadRun(task));
        }

        /// <summary>
        /// A flag that is true if the task has been finished
        /// </summary>
        public bool IsDone => !t.IsAlive;


        /// <summary>
        /// Internal Starts the Thread for the Task
        /// </summary>
        internal void RunTask()
        {
            t.Start();
        }

        private void ThreadRun(DelTask task)
        {
            ret = task.Invoke();
        }

        internal bool CheckState()
        {
            if (IsDone)
            {
                onFinish?.Invoke(ret);
            }

            return IsDone;
        }
    }

    internal class ThreadManager<T> : IThreadManager
    {
        /// <summary>
        /// A List of all Running Tasks
        /// </summary>
        public List<TaskReference<T>> RunningTasks = new List<TaskReference<T>>();
        /// <summary>
        /// The Type of the TheadManager
        /// to comply with the IThreadManager Interface
        /// </summary>
        public Type Type => typeof(T);


        /// <summary>
        /// Checks the States of the currently running tasks
        /// </summary>
        /// <returns>True if all tasks finished</returns>
        public bool CheckStates()
        {
            for (int i = RunningTasks.Count - 1; i >= 0; i--)
            {
                if (RunningTasks[i].CheckState())
                {
                    RunningTasks.RemoveAt(i);
                }
            }

            return RunningTasks.Count == 0;
        }

        /// <summary>
        /// Enqueues the Task and runs it
        /// </summary>
        /// <param name="task"></param>
        public void RunTask(TaskReference<T> task)
        {
            RunningTasks.Add(task);
            task.RunTask();
        }
    }

    /// <summary>
    /// Static Implementation of the Thread Manager
    /// </summary>
    public static class ThreadManager
    {
        private static List<IThreadManager> _managers = new List<IThreadManager>();

        /// <summary>
        /// Runs a Task on a different thread
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="task"></param>
        /// <param name="onFinish"></param>
        public static void RunTask<T>(TaskReference<T>.DelTask task, Action<T> onFinish)
        {
            ThreadManager<T> manager = GetManager<T>();
            manager.RunTask(CreateTask(task, onFinish));
        }
        /// <summary>
        /// Runs a Task on a different thread
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="task"></param>
        public static void RunTask<T>(TaskReference<T> task)
        {
            ThreadManager<T> manager = GetManager<T>();
            manager.RunTask(task);
        }

        /// <summary>
        /// Checks all ThreadManager Completion States and removes them when they have finished
        /// </summary>
        internal static void CheckManagerStates()
        {
            for (int i = _managers.Count - 1; i >= 0; i--)
            {
                if (_managers[i].CheckStates())
                {
                    _managers.RemoveAt(i);
                }
            }
        }

        /// <summary>
        /// Returns a ThreadManager of Type T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        private static ThreadManager<T> GetManager<T>()
        {
            List<IThreadManager> mgrs = _managers.Where(x => x.Type == typeof(T)).ToList();
            ThreadManager<T> manager;
            if (mgrs.Count == 0)
            {
                manager = new ThreadManager<T>();
                _managers.Add(manager);
            }
            else
            {
                manager = mgrs[0] as ThreadManager<T>;
            }

            return manager;
        }

        /// <summary>
        /// Removes a manager from the manager list
        /// </summary>
        /// <param name="manager"></param>
        internal static void RemoveManager(IThreadManager manager)
        {
            _managers.Remove(manager);
        }

        /// <summary>
        /// Creates a Task of a specific type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="task"></param>
        /// <param name="onFinish"></param>
        /// <returns></returns>
        public static TaskReference<T> CreateTask<T>(TaskReference<T>.DelTask task, Action<T> onFinish)
        {
            return new TaskReference<T>(task, onFinish);
        }
    }
}