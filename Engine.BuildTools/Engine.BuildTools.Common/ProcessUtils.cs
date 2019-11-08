using System;
using System.Diagnostics;

namespace Engine.BuildTools.Common
{
    public static class ProcessUtils
    {
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