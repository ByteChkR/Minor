using System;
using System.Diagnostics;
using System.Threading;

namespace Engine.BuildTools.Common
{
    public static class ProcessUtils
    {
        public static void RunActionAsCommand(Action<string[]> action, string args, Action<Exception> onEnd)
        {
            string[] a = args.Split(new[] {' ', '\n'});
            RunActionAsCommand(action, a, onEnd);
        }
        public static void RunActionAsCommand(Action<string[]> action, string[] args, Action<Exception> onEnd)
        {
            Thread t = new Thread(() => RunThread(action, args, onEnd));
            t.Start();
        }
        

        private static void RunThread(Action<string[]> action, string[] args, Action<Exception> onEnd)
        {
            try
            {
                action(args);
                onEnd?.Invoke(null);
            }
            catch (Exception e)
            {
                onEnd?.Invoke(e);
            }


        }

        public static int RunProcess(string file, string args, Action waitAction, Action<string> writeLine = null)
        {
            if (writeLine == null) writeLine = Console.WriteLine;
            ProcessStartInfo psi = new ProcessStartInfo(file, args)
            {
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true,
                UseShellExecute = false
            };

            writeLine(file + " " + args);

            Process p = new Process { StartInfo = psi };

            p.Start();

            ConsoleRedirector redir;
            redir = ConsoleRedirector.CreateRedirector(p.StandardOutput, p.StandardError, p, writeLine);

            redir.StartThreads();

            while (!p.HasExited)
            {
                waitAction?.Invoke();
            }


            redir.StopThreads();
            writeLine?.Invoke(redir.GetRemainingLogs());

            return p.ExitCode;
        }
    }
}