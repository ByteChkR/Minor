using System;
using System.Collections.Generic;
using Engine.DataTypes;
using Engine.IO;
using Engine.OpenCL.DotNetCore.Memory;
using Engine.OpenFL;

namespace Engine.OpenCL.Runner
{
    public struct FLExecutionContext
    {
        public string Filename;
        public Action<Dictionary<Texture, byte[]>> OnFinishCallback;
        public byte[] Input;
        public int Width;
        public int Height;
        public Dictionary<string, Texture> TextureMap;

        public FLExecutionContext(string filename, Texture tex, Dictionary<string, Texture> textureMap,
            Action<Dictionary<Texture, byte[]>> onFinishCallback)
        {
            Width = (int) tex.Width;
            Height = (int) tex.Height;
            Filename = filename;
            MemoryBuffer buf = TextureLoader.TextureToMemoryBuffer(CLAPI.MainThread, tex);
            Input = CLAPI.ReadBuffer<byte>(CLAPI.MainThread, buf, (int) buf.Size);
            buf.Dispose();
            TextureMap = textureMap;
            OnFinishCallback = onFinishCallback;
        }

        public FLExecutionContext(string filename, byte[] input, int width, int height,
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

    public class FLRunner
    {
        protected KernelDatabase _db;
        
        protected CLAPI _instance;
        protected Queue<FLExecutionContext> _processQueue;

        public FLRunner(
            CLAPI instance, TypeEnums.DataTypes dataTypes = TypeEnums.DataTypes.UCHAR1, string kernelFolder = "assets/kernel/")
        {
            _instance = instance;
            _db = new KernelDatabase(instance, kernelFolder, dataTypes);
            _processQueue = new Queue<FLExecutionContext>();
        }

        public virtual void Enqueue(FLExecutionContext context)
        {
            _processQueue.Enqueue(context);
        }

        public virtual void Process(Action onFinish = null)
        {
            while (_processQueue.Count != 0)
            {
                FLExecutionContext fle = _processQueue.Dequeue();
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

        protected Dictionary<string, byte[]> Process(FLExecutionContext context)
        {
            MemoryBuffer buf = CLAPI.CreateBuffer(_instance, context.Input, MemoryFlag.ReadWrite);
            Interpreter ret = new Interpreter(_instance, context.Filename, buf, context.Width,
                context.Height, 1, 4, _db, true);

            do
            {
                ret.Step();
            } while (!ret.Terminated);


            byte[] buffer = ret.GetResult<byte>();
            Dictionary<string, byte[]> result = new Dictionary<string, byte[]>();
            result.Add("result", buffer);

            foreach (KeyValuePair<string, Texture> keyValuePair in context.TextureMap)
            {
                CLBufferInfo mbuf = ret.GetBuffer(keyValuePair.Key);
                if (mbuf == null)
                {
                    continue;
                }

                byte[] spec = CLAPI.ReadBuffer<byte>(_instance, mbuf.Buffer, (int) mbuf.Buffer.Size);
                result.Add(keyValuePair.Key, spec);
                mbuf.Buffer.Dispose();
            }

            return result;
        }
    }
}