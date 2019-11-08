using System;
using Engine.Core;
using Engine.DataTypes;
using Engine.IO;
using Engine.OpenCL.DotNetCore.Kernels;
using Engine.OpenCL.DotNetCore.Memory;
using Engine.OpenFL;
using OpenTK;
using OpenTK.Graphics;

namespace Engine.OpenCL
{
    public class FLMultiThreadRunner
    {
        private Action<Interpreter> ac;
        private CLAPI _threadInstance;
        private GameWindow _threadGLContext;
        public void Run(string filename, Texture tex, Texture specTex, KernelDatabase db)
        {
            ThreadManager<Interpreter>.RunInThread(() => MultiThreadFunc(filename, tex, specTex, db), MultiThreadTestResult);
        }


        private Interpreter MultiThreadFunc(string filename, Texture tex, Texture specTex, KernelDatabase db)
        {
            _threadInstance = CLAPI.GetInstance();
            _threadGLContext = new GameWindow(256, 128, GraphicsMode.Default, "TEST");
            _threadGLContext.MakeCurrent();
            return RunInterpreter(filename, tex, specTex, db, _threadInstance);
        }
        public static Interpreter RunInterpreter(string filename, Texture tex, Texture specTex, KernelDatabase db, CLAPI instance)
        {
            MemoryBuffer buf = TextureLoader.TextureToMemoryBuffer(instance, tex);
            Interpreter ret = new Interpreter(instance, filename, buf, (int)tex.Width, (int)tex.Height, 1, 4, db, true);


            do
            {
                ret.Step();
            } while (!ret.Terminated);

            byte[] buffer = ret.GetResult<byte>();
            CLBufferInfo spe = ret.GetBuffer("specularOut");
            if (spe != null)
            {
                byte[] spec = CLAPI.ReadBuffer<byte>(instance, spe.Buffer, (int)spe.Buffer.Size);

                TextureLoader.Update(specTex, spec, (int)specTex.Width, (int)specTex.Height);
            }

            TextureLoader.Update(tex, buffer, (int)tex.Width, (int)tex.Height);

            return ret;
        }

        private void MultiThreadTestResult(Interpreter result)
        {
            _threadGLContext.Dispose();
            _threadInstance.Dispose();
            _threadInstance = null;
            _threadGLContext = null;
        }

    }
}