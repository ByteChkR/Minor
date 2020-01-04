using System;
using System.Reflection;
using CommandRunner;
using Microsoft.Win32;

namespace Engine.Player.Commands
{
    public class SetDefaultProgramCommand : AbstractCommand
    {


        private static void RegisterExtensions()
        {
            RegisterExtension(".game");
            RegisterExtension(".engine");
        }
        private static void RegisterExtension(string ext)
        {
            RegistryKey key = Registry.ClassesRoot.CreateSubKey(ext);
            key.SetValue("", "DotNetPlayer");
            key.Close();

            key = Registry.ClassesRoot.CreateSubKey(ext + "\\Shell\\Open\\command");
            //key = key.CreateSubKey("command");

            string loc = Assembly.GetExecutingAssembly().Location;
            key.SetValue("", "\"" + loc + "\" \"%L\"");
            key.Close();

            key = Registry.ClassesRoot.CreateSubKey(ext + "\\DefaultIcon");
            key.SetValue("", loc);
            key.Close();
        }

        private static void SetDefaultProgram(StartupInfo info, string[] args)
        {
            try
            {
                RegisterExtensions();
            }
            catch (Exception e)
            {
                Console.WriteLine("Could not access registry. Administrator rights are reequired once.");
                Console.WriteLine(e);
            }
        }

        public SetDefaultProgramCommand() : base(SetDefaultProgram, new[] { "--set-default-program", "-sD" }, "Requires Admin Permissions", false)
        {

        }
    }
}