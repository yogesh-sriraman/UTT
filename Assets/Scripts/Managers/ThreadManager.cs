using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;


namespace com.medcare360.utt
{
    public class ThreadManager : SingletonManager<ThreadManager>
    {
        private static readonly Queue<Action> _mainThread = new Queue<Action>();

        public Dictionary<string, Thread> activeThreads;

        public void Initialize()
        {
            activeThreads = new Dictionary<string, Thread>();
        }

        #region THREAD OPERATIONS

        public void AddToMainThread(Action action)
        {
            lock (_mainThread)
            {
                _mainThread.Enqueue(action);
            }
        }

        public void AddToThread(Action action, string type)
        {
            ThreadStart start = new ThreadStart(action);
            Thread thread = new Thread(start);
            thread.IsBackground = true;
            thread.Start();

            activeThreads.Add(type, thread);
        }

        public void AbortThread(string threadName)
        {
            Thread activeThread = activeThreads[threadName];
            if (activeThread != null && activeThread.IsAlive)
            {
                activeThread.Abort();
            }
            activeThreads.Remove(threadName);
        }

        #endregion

        #region INVOKE MAIN THREAD

        private void Update()
        {
            if (Application.targetFrameRate != ProcessManager.Instance.targetFps)
            {
                Application.targetFrameRate = ProcessManager.Instance.targetFps;
            }
            InvokeMainThreadActions();
        }

        private void InvokeMainThreadActions()
        {
            lock (_mainThread)
            {
                while(_mainThread.Count > 0)
                {
                    _mainThread.Dequeue().Invoke();
                }
            }
        }
        #endregion
    }

}