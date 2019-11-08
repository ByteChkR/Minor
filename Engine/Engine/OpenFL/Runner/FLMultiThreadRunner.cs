using System;
using System.Threading;
using Engine.Core;
using Engine.DataTypes;
using Engine.IO;
using Engine.OpenCL.DotNetCore.Kernels;
using Engine.OpenCL.DotNetCore.Memory;
using Engine.OpenFL;
using OpenTK;
using OpenTK.Graphics;

namespace Engine.OpenCL.Runner
{
    public class FLMultiThreadRunner :FLRunner
    {
        private GameWindow window;

        public FLMultiThreadRunner(Action onFinishQueueCallback,
            TypeEnums.DataTypes dataTypes = TypeEnums.DataTypes.UCHAR1, string kernelFolder = "assets/kernel/") : base(
            CLAPI.GetInstance(), onFinishQueueCallback, dataTypes, kernelFolder)
        {



        }
        
        public override void Process()
        {
            Thread t = new Thread(ThreadProcess);
            t.Start();
        }

        

        private void ThreadProcess()
        {
            window = new GameWindow(1,1,GraphicsMode.Default, "FLRunner");
            window.MakeCurrent();
            base.Process();
            window.Dispose();
        }
    }
}