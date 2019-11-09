using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Engine.BuildTools.Builder
{
    public class StartupInfo
    {
        private Dictionary<string, List<string>> values = new Dictionary<string, List<string>>();

        public StartupInfo(string[] args)
        {
            for (int i = 0; i < args.Length; i++)
            {
                if (args[i].StartsWith("--") || i == 0)
                {
                    List<string> argValues = new List<string>();
                    for (int j = i + 1; j < args.Length; j++)
                    {
                        if (args[j].StartsWith("--"))
                        {
                            break;
                        }

                        argValues.Add(args[j]);
                    }

                    if (i == 0 && !args[i].StartsWith("--"))
                    {
                        values.Add("noflag", argValues);
                    }
                    else
                    {
                        values.Add(args[i], argValues);
                    }
                }
            }
        }

        public List<string> GetValues(string flag)
        {
            return values[flag];
        }

        public bool HasFlag(string flag)
        {
            return values.ContainsKey(flag);
        }

        public bool HasValueFlag(string flag)
        {
            return HasFlag(flag) && values[flag].Count != 0;
        }

        public static List<string> ResolveFileReferences(string arg)
        {
            if (arg.StartsWith("@"))
            {
                return File.ReadAllLines(arg.Remove(0, 1)).ToList();
            }

            return new List<string>() { arg };
        }
    }
}