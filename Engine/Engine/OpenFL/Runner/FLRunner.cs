using System;
using System.Collections.Generic;
using Engine.DataTypes;
using Engine.IO;
using Engine.OpenCL;
using Engine.OpenCL.DotNetCore.Memory;
using Engine.OpenFL;

namespace Engine.OpenFL.Runner
{
    /// <summary>
    /// Contains the Context in which the FL Runner is executing the enqueued items
    /// </summary>
    public struct FlExecutionContext
    {
        public string Filename;
        public Action<Dictionary<Texture, byte[]>> OnFinishCallback;
        public byte[] Input;
        public int Width;
        public int Height;
        public Dictionary<string, Texture> TextureMap;

        public FlExecutionContext(string filename, Texture tex, Dictionary<string, Texture> textureMap,
            Action<Dictionary<Texture, byte[]>> onFinishCallback)
        {
            Width = (int) tex.Width;
            Height = (int) tex.Height;
            Filename = filename;
            MemoryBuffer buf = TextureLoader.TextureToMemoryBuffer(Clapi.MainThread, tex);
            Input = Clapi.ReadBuffer<byte>(Clapi.MainThread, buf, (int) buf.Size);
            buf.Dispose();
            TextureMap = textureMap;
            OnFinishCallback = onFinishCallback;
        }

        public FlExecutionContext(string filename, byte[] input, int width, int height,
            Dictionary<string, Texture> textureMap,
            Action<Dictionary<Texture, byte[]>> onFinishCallback)
        {
            Width = width;
            Height = height;
            Filename = filename;
            Input = input;
            TextureMap = textureMap;
            OnFinishCallback = onFinishCallback;
        }
    }

    /// <summary>
    /// Single Threaded FL Runner Implementation
    /// </summary>
    public class FlRunner
    {
        protected KernelDatabase Db;
        
        protected Clapi Instance;
        protected Queue<FlExecutionContext> ProcessQueue;
        public int ItemsInQueue => ProcessQueue.Count;
        public FlRunner(
            Clapi instance, OpenCL.TypeEnums.DataTypes dataTypes = OpenCL.TypeEnums.DataTypes.Uchar1, string kernelFolder = "assets/kernel/")
        {
            Instance = instance;
            Db = new KernelDatabase(instance, kernelFolder, dataTypes);
            ProcessQueue = new Queue<FlExecutionContext>();
        }

        public virtual void Enqueue(FlExecutionContext context)
        {
            ProcessQueue.Enqueue(context);
        }

        public virtual void Process(Action onFinish = null)
        {
            while (ProcessQueue.Count != 0)
            {
                FlExecutionContext fle = ProcessQueue.Dequeue();
                Dictionary<string, byte[]> ret = Process(fle);
                Dictionary<Texture, byte[]> texMap = new Dictionary<Texture, byte[]>();
                foreach (KeyValuePair<string, byte[]> bytese in ret)
                {
                    if (fle.TextureMap.ContainsKey(bytese.Key))
                    {
                        texMap.Add(fle.TextureMap[bytese.Key], bytese.Value);
                    }
                }

                foreach (KeyValuePair<Texture, byte[]> textureUpdate in texMap)
                {
                    TextureLoader.Update(textureUpdate.Key, textureUpdate.Value, (int) textureUpdate.Key.Width,
                        (int) textureUpdate.Key.Height);
                }

                fle.OnFinishCallback?.Invoke(texMap);
            }

            onFinish?.Invoke();
        }

        protected Dictionary<string, byte[]> Process(FlExecutionContext context)
        {
            MemoryBuffer buf = Clapi.CreateBuffer(Instance, context.Input, MemoryFlag.ReadWrite);
            Interpreter ret = new Interpreter(Instance, context.Filename, buf, context.Width,
                context.Height, 1, 4, Db, true);

            do
            {
                ret.Step();
            } while (!ret.Terminated);


            byte[] buffer = ret.GetResult<byte>();
            Dictionary<string, byte[]> result = new Dictionary<string, byte[]>();
            result.Add("result", buffer);

            foreach (KeyValuePair<string, Texture> keyValuePair in context.TextureMap)
            {
                ClBufferInfo mbuf = ret.GetBuffer(keyValuePair.Key);
                if (mbuf == null)
                {
                    continue;
                }

                byte[] spec = Clapi.ReadBuffer<byte>(Instance, mbuf.Buffer, (int) mbuf.Buffer.Size);
                result.Add(keyValuePair.Key, spec);
                mbuf.Buffer.Dispose();
            }

            return result;
        }
    }
}