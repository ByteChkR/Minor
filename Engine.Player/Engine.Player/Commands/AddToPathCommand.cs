using System;
using System.IO;
using CommandRunner;

namespace Engine.Player.Commands
{
    public class AddToPathCommand : AbstractCommand
    {
        private static void AddToPathVariable()
        {
            const string name = "PATH";
            string pathvar = System.Environment.GetEnvironmentVariable(name, EnvironmentVariableTarget.Machine);
            string appPath = AppDomain.CurrentDomain.BaseDirectory;
            appPath = appPath.Remove(appPath.Length - 1, 1);
            Console.WriteLine("Adding Path: " + appPath);
            var value = pathvar + ";" + appPath;
            var target = EnvironmentVariableTarget.Machine;
            System.Environment.SetEnvironmentVariable(name, value, target);
        }


        private static void UpdatePathVariable()
        {
            const string name = "PATH";
            string pathvar = System.Environment.GetEnvironmentVariable(name, EnvironmentVariableTarget.Machine);
            Console.WriteLine(pathvar);
            string appPath = AppDomain.CurrentDomain.BaseDirectory;
            appPath = appPath.Remove(appPath.Length - 1, 1);
            string[] paths = pathvar.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            string val = "";
            for (int i = 0; i < paths.Length; i++)
            {
                if (File.Exists(paths[i] + "\\Engine.BuildTools.Builder.dll"))
                {
                    Console.WriteLine("Updating Path: " + paths[i] + " >> " + appPath);
                    paths[i] = appPath;
                }

                val += paths[i] + ";";
            }

            var target = EnvironmentVariableTarget.Machine;
            System.Environment.SetEnvironmentVariable(name, val, target);
        }


        private static bool IsInPathVariable()
        {
            const string name = "PATH";
            string pathvar = System.Environment.GetEnvironmentVariable(name, EnvironmentVariableTarget.Machine);
            string[] paths = pathvar.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < paths.Length; i++)
            {
                if (File.Exists(paths[i] + "\\Engine.BuildTools.Builder.dll"))
                {
                    return true;
                }
            }

            return false;
        }


        private static void AddToPath(StartupInfo info, string[] args)
        {
            try
            {
                if (IsInPathVariable()) UpdatePathVariable();
                else AddToPathVariable();
            }
            catch (Exception e)
            {
                Console.WriteLine("Could not access registry. Administrator rights are reequired once.");
                Console.WriteLine(e);
            }
        }

        public AddToPathCommand() : base(AddToPath, new[] { "--add-to-path" }, "Requires Admin Permissions", false)
        {

        }
    }
}