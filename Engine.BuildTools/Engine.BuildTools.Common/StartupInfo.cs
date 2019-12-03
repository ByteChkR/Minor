using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Engine.BuildTools.Builder
{
    public class StartupInfo
    {
        private Dictionary<string, List<string>> _values = new Dictionary<string, List<string>>();

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

                    if (i == 0 && !args[0].StartsWith("--"))
                    {
                        argValues.Add(args[0]);
                        _values.Add("noflag", argValues);
                    }
                    else
                    {
                        _values.Add(args[i], argValues);
                    }
                }
            }
        }

        public List<string> GetValues(string flag)
        {
            return _values[flag];
        }

        public bool HasFlag(string flag)
        {
            return _values.ContainsKey(flag);
        }

        public bool HasValueFlag(string flag)
        {
            return HasFlag(flag) && _values[flag].Count != 0;
        }

        public static List<string> ResolveFileReferences(string arg)
        {
            if (arg.StartsWith("@"))
            {
                return File.ReadAllLines(arg.Remove(0, 1)).ToList();
            }

            return new List<string> {arg};
        }
    }
}