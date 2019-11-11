using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.BuildTools.Builder.CLI
{
    class Program
    {
        static void Main(string[] args)
        {
            BuildTools.Builder.Builder.RunCommand(args);
        }
    }
}
