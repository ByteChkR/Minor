using System;
using System.Diagnostics;

namespace Engine.BuildTools.Common
{
    public static class ProcessUtils
    {

        public static int RunProcess(string file, string args, Action waitAction)
        {
            ProcessStartInfo psi = new ProcessStartInfo(file, args)
            {
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true,
                UseShellExecute = false
            };


            Process p = new Process { StartInfo = psi };

            p.Start();

            ConsoleRedirector redir;
            redir = ConsoleRedirector.CreateRedirector(p.StandardOutput, p.StandardError, p, Console.WriteLine);

            redir.StartThreads();

            while (!p.HasExited)
            {
                waitAction?.Invoke();
            }


            redir.StopThreads();
            Console.WriteLine(redir.GetRemainingLogs());

            return p.ExitCode;
        }

    }
}
