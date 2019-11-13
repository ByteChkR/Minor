using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.BuildTools.Builder.CLI
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Builder.RunCommand(args);
        }
    }
}