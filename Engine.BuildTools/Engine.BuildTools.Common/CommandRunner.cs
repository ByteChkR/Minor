using System.Collections.Generic;
using System.Linq;
using Engine.BuildTools.Builder;

namespace Engine.BuildTools.Common
{
    public static class CommandRunner
    {
        private static List<Command> _commands = new List<Command>();
        private static Command _default;
        public static int CommandCount => _commands.Count;

        public static void AddCommand(Command cmd)
        {
            _commands.Add(cmd);
        }

        public static void SetDefaultCommand(Command cmd)
        {
            _default = cmd;
        }


        public static Command GetCommandAt(int index)
        {
            return _commands[index];
        }

        public static void RunCommands(string[] args)
        {
            for (int i = 0; i < args.Length; i++)
            {
                for (int j = 0; j < _commands.Count; j++)
                {
                    if (_commands[j].CommandKeys.Contains(args[i]))
                    {
                        args[i] = _commands[j].CommandKeys[0]; //Make sure its the first command key.
                    }
                }
            }

            StartupInfo info = new StartupInfo(args);

            for (int i = 0; i < _commands.Count; i++)
            {
                if (info.HasFlag(_commands[i].CommandKeys[0]))
                {
                    _commands[i].CommandAction?.Invoke(info, info.GetValues(_commands[i].CommandKeys[0]).ToArray());
                }
            }

            if (info.HasFlag("noflag"))
            {
                _default?.CommandAction?.Invoke(info, info.GetValues("noflag").ToArray());
            }
        }
    }
}