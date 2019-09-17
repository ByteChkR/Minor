using System;
using System.IO;
using CLHelperLibrary;
using OpenCl.DotNetCore.Memory;
using Xunit;

namespace FilterLanguage.Tests
{
    public class InterpreterTests
    {
#if NO_CL
#else
        [Fact]
        public void FLInterpreterTest()
        {

            string path = Path.GetFullPath("../../../resources");
            string[] files = Directory.GetFiles(path + "/filter/tests", "*.fl");

            Directory.SetCurrentDirectory(path);
            KernelDatabase db = new KernelDatabase(path + "/kernel");

            foreach (string file in files)
            {

                Interpreter P = new Interpreter(file, CL.CreateEmpty<byte>(512 * 512 * 4, MemoryFlag.CopyHostPointer | MemoryFlag.ReadWrite), 512, 512, 1, 4, db);
                while (!P.Terminated)
                {
                    P.Step();
                }
            }
        }
#endif
    }
}
