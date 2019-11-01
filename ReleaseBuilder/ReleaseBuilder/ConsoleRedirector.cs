using System.Diagnostics;
using System.IO;
using System.Security.Policy;
using System.Threading;

namespace ReleaseBuilder
{
    public class ConsoleRedirector
    {
        Thread _thread = null;
        Thread _errThread = null;
        TextReader _cOut;
        TextReader _cEOut;
        Process _proc;
        private WriteLine _del;

        public ConsoleRedirector()
        {
            //SHIT
        }

        public delegate void WriteLine(string line);
        public static ConsoleRedirector CreateRedirector(StreamReader consoleOut, StreamReader errorConsoleOut, Process proc, WriteLine del = null)
        {
            ConsoleRedirector gcr = new ConsoleRedirector();
            gcr._proc = proc;
            gcr._cEOut = errorConsoleOut;
            gcr._cOut = consoleOut;
            gcr._del = del;
            return gcr;
        }

        public void StartThreads()
        {
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
            if (_thread == null && _errThread == null) return;
            if (_thread != null) _thread.Abort();
            if (_errThread != null) _errThread.Abort();
            
            

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
                txt = cout.ReadLine();
                if (txt != "") _del?.Invoke(txt);
            }
        }

    }
}