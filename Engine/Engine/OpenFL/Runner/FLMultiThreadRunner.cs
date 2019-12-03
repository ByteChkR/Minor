using System;
using System.Collections.Generic;
using Engine.Core;
using Engine.DataTypes;
using Engine.IO;
using OpenTK;
using OpenTK.Graphics;

namespace Engine.OpenCL.Runner
{
    public class FlMultiThreadRunner : FlRunner
    {
        private GameWindow window;

        public FlMultiThreadRunner(Action onFinishQueueCallback,
            TypeEnums.DataTypes dataTypes = TypeEnums.DataTypes.Uchar1, string kernelFolder = "assets/kernel/") : base(
            Clapi.GetInstance(), dataTypes, kernelFolder)
        {
        }

        private void FlFinished(Dictionary<Texture, byte[]> result)
        {
            foreach (KeyValuePair<Texture, byte[]> keyValuePair in result)
            {
                TextureLoader.Update(keyValuePair.Key, keyValuePair.Value, (int) keyValuePair.Key.Width,
                    (int) keyValuePair.Key.Height);
            }
        }

        public override void Enqueue(FlExecutionContext context)
        {
            if (context.OnFinishCallback == null)
            {
                context.OnFinishCallback = FlFinished;
            }
            else
            {
                context.OnFinishCallback += FlFinished;
            }

            base.Enqueue(context);
        }

        public override void Process(Action onFinish = null)
        {
            ThreadManager.RunTask(() => _proc(onFinish), x =>
            {
                foreach (KeyValuePair<FlExecutionContext, Dictionary<Texture, byte[]>> textureUpdate in x)
                {
                    foreach (KeyValuePair<Texture, byte[]> bytese in textureUpdate.Value)
                    {
                        TextureLoader.Update(bytese.Key, bytese.Value, (int) bytese.Key.Width, (int) bytese.Key.Height);
                    }
                }
            });
        }

        private Dictionary<FlExecutionContext, Dictionary<Texture, byte[]>> _proc(Action onFinish = null)
        {
            window = new GameWindow(100, 100, GraphicsMode.Default, "FLRunner");
            window.MakeCurrent();
            Dictionary<FlExecutionContext, Dictionary<Texture, byte[]>> ret =
                new Dictionary<FlExecutionContext, Dictionary<Texture, byte[]>>();
            while (ProcessQueue.Count != 0)
            {
                FlExecutionContext fle = ProcessQueue.Dequeue();
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