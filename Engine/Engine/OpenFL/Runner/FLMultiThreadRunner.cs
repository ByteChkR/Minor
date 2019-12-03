using System;
using System.Collections.Generic;
using Engine.Core;
using Engine.DataTypes;
using Engine.IO;
using OpenTK;
using OpenTK.Graphics;

namespace Engine.OpenCL.Runner
{
    public class FLMultiThreadRunner : FLRunner
    {
        private GameWindow window;

        public FLMultiThreadRunner(Action onFinishQueueCallback,
            TypeEnums.DataTypes dataTypes = TypeEnums.DataTypes.UCHAR1, string kernelFolder = "assets/kernel/") : base(
            CLAPI.GetInstance(), dataTypes, kernelFolder)
        {
        }

        private void FLFinished(Dictionary<Texture, byte[]> result)
        {
            foreach (KeyValuePair<Texture, byte[]> keyValuePair in result)
            {
                TextureLoader.Update(keyValuePair.Key, keyValuePair.Value, (int) keyValuePair.Key.Width,
                    (int) keyValuePair.Key.Height);
            }
        }

        public override void Enqueue(FLExecutionContext context)
        {
            if (context.OnFinishCallback == null)
            {
                context.OnFinishCallback = FLFinished;
            }
            else
            {
                context.OnFinishCallback += FLFinished;
            }

            base.Enqueue(context);
        }

        public override void Process(Action onFinish = null)
        {
            ThreadManager.RunTask(() => _proc(onFinish), x =>
            {
                foreach (KeyValuePair<FLExecutionContext, Dictionary<Texture, byte[]>> textureUpdate in x)
                {
                    foreach (KeyValuePair<Texture, byte[]> bytese in textureUpdate.Value)
                    {
                        TextureLoader.Update(bytese.Key, bytese.Value, (int) bytese.Key.Width, (int) bytese.Key.Height);
                    }
                }
            });
        }

        private Dictionary<FLExecutionContext, Dictionary<Texture, byte[]>> _proc(Action onFinish = null)
        {
            window = new GameWindow(100, 100, GraphicsMode.Default, "FLRunner");
            window.MakeCurrent();
            Dictionary<FLExecutionContext, Dictionary<Texture, byte[]>> ret =
                new Dictionary<FLExecutionContext, Dictionary<Texture, byte[]>>();
            while (_processQueue.Count != 0)
            {
                FLExecutionContext fle = _processQueue.Dequeue();
                Dictionary<string, byte[]> texUpdate = Process(fle);
                Dictionary<Texture, byte[]> texMap = new Dictionary<Texture, byte[]>();
                foreach (KeyValuePair<string, byte[]> bytese in texUpdate)
                {
                    if (fle.TextureMap.ContainsKey(bytese.Key))
                    {
                        texMap.Add(fle.TextureMap[bytese.Key], bytese.Value);
                    }
                }

                ret.Add(fle, texMap);
            }

            window.Dispose();
            onFinish?.Invoke();
            return ret;
        }
    }
}