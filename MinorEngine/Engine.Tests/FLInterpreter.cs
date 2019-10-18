using System;
using System.IO;
using Common;
using Engine.Exceptions;
using Engine.OpenCL;
using Engine.OpenCL.DotNetCore.DataTypes;
using Engine.OpenCL.DotNetCore.Memory;
using Engine.OpenFL;
using Xunit;

namespace Engine.Tests
{
    public class FLInterpreter
    {

        public FLInterpreter()
        {
            TestSetup.ApplyDebugSettings();
        }

        [Fact]
        public void FLDefines()
        {
            DebugHelper.ThrowOnAllExceptions = true;
            
            string file = Path.GetFullPath("resources/filter/defines/test.fl");
            Interpreter P = new Interpreter(file,
                CLAPI.CreateEmpty<byte>(128 * 128 * 4, MemoryFlag.CopyHostPointer | MemoryFlag.ReadWrite), 128, 128,
                1,
                4, TestSetup.KernelDB);

            InterpreterStepResult ret = P.Step();


            Assert.True(ret.DefinedBuffers.Count == 5);
            Assert.True(ret.DefinedBuffers[0] == "in_unmanaged");
            Assert.True(ret.DefinedBuffers[1] == "textureD_internal");
            Assert.True(ret.DefinedBuffers[2] == "textureC_internal");
            Assert.True(ret.DefinedBuffers[3] == "textureB_internal");
            Assert.True(ret.DefinedBuffers[4] == "textureA_internal");


        }

        [Fact]
        public void FLWFCDefines_Wrong()
        {
            DebugHelper.ThrowOnAllExceptions = true;

            string[] files = Directory.GetFiles("resources/filter/defines/", "test_wrong_define_wfc_*.fl");
            

            for (int i = 0; i < 2; i++)
            {

                DebugHelper.ThrowOnAllExceptions = i == 0;
                foreach (string file in files)
                {
                    try
                    {
                        Interpreter P = new Interpreter(file,
                            CLAPI.CreateEmpty<byte>(128 * 128 * 4, MemoryFlag.CopyHostPointer | MemoryFlag.ReadWrite), 128,
                            128,
                            1,
                            4, TestSetup.KernelDB);
                        Assert.True(!DebugHelper.ThrowOnAllExceptions);
                    }
                    catch (Exception e)
                    {
                        Assert.True(DebugHelper.ThrowOnAllExceptions);
                        if (!(e is FLInvalidFunctionUseException))
                        {
                            Assert.True(false);
                            continue;
                        }

                        //We passed
                    }

                }
            }
        }

        [Fact]
        public void FLDefineFile_Wrong()
        {
            DebugHelper.ThrowOnAllExceptions = true;

            string file = "resources/filter/defines/test_wrong_define_invalid_file.fl";
            

            for (int i = 0; i < 2; i++)
            {

                DebugHelper.ThrowOnAllExceptions = i == 0;
                try
                {
                    Interpreter P = new Interpreter(file,
                        CLAPI.CreateEmpty<byte>(128 * 128 * 4, MemoryFlag.CopyHostPointer | MemoryFlag.ReadWrite), 128,
                        128,
                        1,
                        4, TestSetup.KernelDB);
                    Assert.True(!DebugHelper.ThrowOnAllExceptions);
                }
                catch (Exception e)
                {
                    Assert.True(DebugHelper.ThrowOnAllExceptions);
                    if (!(e is FLInvalidFunctionUseException))
                    {
                        Assert.True(false);
                    }

                    //We passed
                }
            }

        }

        [Fact]
        public void FLDefineScriptFile_Wrong()
        {
            DebugHelper.ThrowOnAllExceptions = true;

            string file = "resources/filter/defines/test_wrong_script_invalid_file.fl";
            
            for (int i = 0; i < 2; i++)
            {

                try
                {
                    Interpreter P = new Interpreter(file,
                        CLAPI.CreateEmpty<byte>(128 * 128 * 4, MemoryFlag.CopyHostPointer | MemoryFlag.ReadWrite), 128,
                        128,
                        1,
                        4, TestSetup.KernelDB);
                    Assert.True(!DebugHelper.ThrowOnAllExceptions);
                }
                catch (Exception e)
                {
                    Assert.True(DebugHelper.ThrowOnAllExceptions);
                    if (!(e is FLInvalidFunctionUseException))
                    {
                        Assert.True(false);
                    }

                    //We passed
                }
            }

        }


        [Fact]
        public void FLDefineScriptNoFile_Wrong()
        {
            DebugHelper.ThrowOnAllExceptions = true;

            string file = "resources/filter/defines/test_wrong_script_.fl";
            

            for (int i = 0; i < 2; i++)
            {

                try
                {
                    Interpreter P = new Interpreter(file,
                        CLAPI.CreateEmpty<byte>(128 * 128 * 4, MemoryFlag.CopyHostPointer | MemoryFlag.ReadWrite), 128,
                        128,
                        1,
                        4, TestSetup.KernelDB);
                    Assert.True(!DebugHelper.ThrowOnAllExceptions);
                }
                catch (Exception e)
                {
                    Assert.True(DebugHelper.ThrowOnAllExceptions);
                    if (!(e is FLInvalidFunctionUseException))
                    {
                        Assert.True(false);
                    }

                    //We passed
                }
            }

        }

        [Fact]
        public void FLComments()
        {
            DebugHelper.ThrowOnAllExceptions = true;
            string file = Path.GetFullPath("resources/filter/comments/test.fl");
            Interpreter P = new Interpreter(file,
                CLAPI.CreateEmpty<byte>(128 * 128 * 4, MemoryFlag.CopyHostPointer | MemoryFlag.ReadWrite), 128, 128,
                1,
                4, new KernelDatabase("resources/kernel", OpenCL.TypeEnums.DataTypes.UCHAR1)); //We need to Create a "fresh" database since xunit is making the cl context invalid when changing the test
            while (!P.Terminated)
            {
                P.Step();
            }
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
                    4, db); //We need to Create a "fresh" database since xunit is making the cl context invalid when changing the test
                while (!P.Terminated)
                {
                    P.Step();
                }
            }
        }

        [Fact]
        public void TypeConversion()
        {
            float f = float.MaxValue / 2;
            byte b = (byte)CLTypeConverter.Convert(typeof(byte), f);
            float4 f4 = new float4(f);
            uchar4 i4 = (uchar4)CLTypeConverter.Convert(typeof(uchar4), f4);
            Assert.True(b == 128);

            for (int i = 0; i < 4; i++)
            {
                byte s = i4[i];
                Assert.True(s == 128);

            }
        }
    }
}