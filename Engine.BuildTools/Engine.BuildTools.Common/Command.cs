using System;
using System.Text;

namespace Engine.BuildTools.Common
{

    /// <summary>
    /// Contains Abstraction for Commands that will be executed by the Command Runner
    /// </summary>
    public class Command
    {
        public Action<StartupInfo, string[]> CommandAction { get; }
        public string[] CommandKeys { get; }
        public string HelpText;

        private Command(Action<StartupInfo, string[]> action, string[] keys, string helpText ="No Help Text Available")
        {
            CommandAction = action;
            CommandKeys = keys;
            HelpText = helpText;
        }

        public static Command CreateCommand(Action<StartupInfo, string[]> action, string HelpText, params string[] keys)
        {
            if (keys.Length == 0)
            {
                return new Command(action, new[] {""}, HelpText);
            }

            return new Command(action, keys, HelpText);
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder(CommandKeys[0]);
            for (int i = 1; i < CommandKeys.Length; i++)
            {
                sb.Append(" | " + CommandKeys[i]);
            }

            sb.AppendLine("");
            string[] helpText = HelpText.Split(new []{'\n'});
            for (int i = 0; i < helpText.Length; i++)
            {
                sb.AppendLine($"\t{helpText[i]}");
            }

            return sb.ToString();
        }
    }
}