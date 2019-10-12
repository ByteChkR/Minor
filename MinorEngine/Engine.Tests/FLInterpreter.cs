using System.IO;
using Common;
using Engine.Debug;
using Engine.OpenCL;
using Engine.OpenCL.DotNetCore.Memory;
using Engine.OpenFL;
using Xunit;

namespace Engine.Tests
{
    public class FLInterpreter
    {

        [Fact]
        public void FLDefines()
        {
            DebugHelper.ThrowOnAllExceptions = true;

            KernelDatabase db = new KernelDatabase("resources/kernel", OpenCL.TypeEnums.DataTypes.UCHAR1);
            string file = Path.GetFullPath("resources/filter/defines/test.fl");
            Interpreter P = new Interpreter(file,
                CLAPI.CreateEmpty<byte>(128 * 128 * 4, MemoryFlag.CopyHostPointer | MemoryFlag.ReadWrite), 128, 128,
                1,
                4, db);

            Interpreter.InterpreterStepResult ret = P.Step();


            System.Diagnostics.Debug.Assert(ret.DefinedBuffers.Count == 4);
            System.Diagnostics.Debug.Assert(ret.DefinedBuffers[0] == "in_unmanaged");
            System.Diagnostics.Debug.Assert(ret.DefinedBuffers[1] == "textureC_internal");
            System.Diagnostics.Debug.Assert(ret.DefinedBuffers[2] == "textureB_internal");
            System.Diagnostics.Debug.Assert(ret.DefinedBuffers[3] == "textureA_internal");
            

        }

        [Fact]
        public void FLComments()
        {
            DebugHelper.ThrowOnAllExceptions = true;
            KernelDatabase db = new KernelDatabase("resources/kernel", OpenCL.TypeEnums.DataTypes.UCHAR1);
            string file = Path.GetFullPath("resources/filter/comments/test.fl");
            Interpreter P = new Interpreter(file,
                CLAPI.CreateEmpty<byte>(128 * 128 * 4, MemoryFlag.CopyHostPointer | MemoryFlag.ReadWrite), 128, 128,
                1,
                4, db);

            Interpreter.InterpreterStepResult ret = P.Step();
        }

        [Fact]
        public void FLKernels()
        {
            DebugHelper.ThrowOnAllExceptions = true;
            DebugHelper.SeverityFilter = 10;
            string path = "resources/filter/tests";
            string[] files = Directory.GetFiles(path, "*.fl");

            KernelDatabase db = new KernelDatabase("resources/kernel", OpenCL.TypeEnums.DataTypes.UCHAR1);

            foreach (string file in files)
            {
                Interpreter P = new Interpreter(file,
                    CLAPI.CreateEmpty<byte>(128 * 128 * 4, MemoryFlag.CopyHostPointer | MemoryFlag.ReadWrite), 128, 128,
                    1,
                    4, db);
                while (!P.Terminated)
                {
                    P.Step();
                }
            }
        }
    }
}