using System;
using System.Data.SqlTypes;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace Engine.Player.Common
{
    public class ConsoleRedirector
    {
        private Thread _thread = null;
        private Thread _errThread = null;
        private TextReader _cOut;
        private TextReader _cEOut;
        private Process _proc;
        private Action<string> _del;
        private object lockObj = new object();
        private bool quitFlag = false;
        public ConsoleRedirector()
        {
            //SHIT
        }


        public static ConsoleRedirector CreateRedirector(StreamReader consoleOut, StreamReader errorConsoleOut,
            Process proc, Action<string> del = null)
        {
            ConsoleRedirector gcr = new ConsoleRedirector
            {
                _proc = proc,
                _cEOut = errorConsoleOut,
                _cOut = consoleOut,
                _del = del
            };
            return gcr;
        }

        public void StartThreads()
        {
            quitFlag = false;
            if (_thread == null)
            {
                _thread = new Thread(() => Start(_cOut));
            }
            else
            {
                _thread.Abort();
            }

            if (_errThread == null)
            {
                _errThread = new Thread(() => Start(_cEOut));
            }
            else
            {
                _errThread.Abort();
            }

            _errThread.Start();
            _thread.Start();
        }

        public void StopThreads()
        {
            lock (lockObj)
                quitFlag = true;
        }

        public string GetRemainingLogs()
        {
            string ret = "";
            if (!(_cOut as StreamReader).EndOfStream)
            {
                ret += _cOut.ReadToEnd();
            }

            if (!(_cEOut as StreamReader).EndOfStream)
            {
                ret += _cEOut.ReadToEnd();
            }

            return ret;
        }

        public void Start(TextReader cout)
        {
            string txt = "";
            while (!_proc.HasExited)
            {
                lock (lockObj)
                {

                    if (quitFlag)
                    {
                        Console.WriteLine("Quitting From Loop");
                        return;
                    }
                }
                txt = cout.ReadLine();
                if (txt != "")
                {
                    _del?.Invoke(txt);
                }
            }
        }
    }
}