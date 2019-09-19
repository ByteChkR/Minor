using System;
using System.IO;
using CLHelperLibrary;
using Common;
using OpenCl.DotNetCore.Memory;
using Xunit;

namespace FilterLanguage.Tests
{
    public class InterpreterTests
    {
        [Fact]
        public void FLInterpreterTest()
        {
            DebugHelper.ListeningMask = DebugChannel.Log | DebugChannel.Error | DebugChannel.Internal_Error |
                                        DebugChannel.Warning;
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
    }
}
