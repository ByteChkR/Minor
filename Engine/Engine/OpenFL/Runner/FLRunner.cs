using System;
using System.Collections;
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
        public Action OnFinishCallback;
        public Texture Input;
        public Dictionary<string, Texture> TexturesToUpdate;

        public FLExecutionContext(string filename, Texture input, Dictionary<string, Texture> texturesToUpdate,
            Action onFinishCallback)
        {
            Filename = filename;
            Input = input;
            TexturesToUpdate = texturesToUpdate;
            OnFinishCallback = onFinishCallback;
        }
    }
    public class FLRunner
    {
        protected Queue<FLExecutionContext> _processQueue;
        protected Action _onFinishQueueCallback;
        protected CLAPI _instance;
        protected KernelDatabase _db;
        public FLRunner(CLAPI instance, Action onFinishQueueCallback, TypeEnums.DataTypes dataTypes = TypeEnums.DataTypes.UCHAR1, string kernelFolder = "assets/kernel/")
        {
            _onFinishQueueCallback = onFinishQueueCallback;
            _instance = instance;
            _db = new KernelDatabase(instance, kernelFolder, dataTypes);
            _processQueue = new Queue<FLExecutionContext>();
        }

        public void Enqueue(FLExecutionContext context)
        {
            _processQueue.Enqueue(context);
        }

        public virtual void Process()
        {
            while (_processQueue.Count != 0)
            {
                Process(_processQueue.Dequeue());
            }
            _onFinishQueueCallback?.Invoke();
        }

        private void Process(FLExecutionContext context)
        {

            MemoryBuffer buf = TextureLoader.TextureToMemoryBuffer(_instance, context.Input);
            Interpreter ret = new Interpreter(_instance, context.Filename, buf, (int)context.Input.Width, (int)context.Input.Height, 1, 4, _db, true);

            do
            {
                ret.Step();
            } while (!ret.Terminated);


            byte[] buffer = ret.GetResult<byte>();
            TextureLoader.Update(context.Input, buffer, (int)context.Input.Width, (int)context.Input.Height);

            foreach (KeyValuePair<string, Texture> keyValuePair in context.TexturesToUpdate)
            {
                CLBufferInfo mbuf = ret.GetBuffer(keyValuePair.Key);
                byte[] spec = CLAPI.ReadBuffer<byte>(_instance, mbuf.Buffer, (int)mbuf.Buffer.Size);

                TextureLoader.Update(keyValuePair.Value, spec, (int)keyValuePair.Value.Width, (int)keyValuePair.Value.Height);
                mbuf.Buffer.Dispose();
            }

            context.OnFinishCallback?.Invoke();
        }
    }
}