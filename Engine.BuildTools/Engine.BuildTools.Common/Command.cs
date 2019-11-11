using System;
using System.Text;
using Engine.BuildTools.Builder;

namespace Engine.BuildTools.Common
{
    public class Command
    {
        public string[] CommandKeys;
        public Action<StartupInfo, string[]> CommandAction;

        private Command(Action<StartupInfo, string[]> action, string[] keys)
        {
            CommandAction = action;
            CommandKeys = keys;
        }
        public static Command CreateCommand(Action<StartupInfo, string[]> action, params string[] keys)
        {
            if(keys.Length == 0)return  new Command(action, new []{""});
            return new Command(action, keys);
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder(CommandKeys[0]);
            for (int i = 1; i < CommandKeys.Length; i++)
            {
                sb.Append(" | " + CommandKeys[i]);
            }

            return sb.ToString();
        }
    }
}