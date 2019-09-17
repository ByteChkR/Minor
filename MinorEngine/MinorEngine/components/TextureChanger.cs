using System;
using System.Collections.Generic;
using System.IO;
using CLHelperLibrary;
using Common;
using GameEngine.engine.components;
using GameEngine.engine.rendering;
using OpenCl.DotNetCore.Memory;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace GameEngine.components
{
    public class TextureChanger : AbstractComponent
    {
        private readonly GameTexture overlay;


        public TextureChanger(GameWindow window)
        {
            overlay = GameTexture.Load("textures/overlay.png");

            window.KeyPress += OnKeyPress;
        }

        private bool init = false;
        public override void Update(float deltaTime)
        {
            if (!init && owner != null)
            {
                init = true;
            }
        }

        private bool isDebuggingInterpreter = false;

        private FilterLanguage.Interpreter fli = null;
        private KernelDatabase db = new KernelDatabase("kernel/");

        private void OnKeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 'r')
            {
                GL.DeleteTexture(owner.model.meshes[0].textures[0].textureID);
                owner.model.meshes[0].textures[0] = GameTexture.Load("runicfloor.png");
                isDebuggingInterpreter = false;
            }
            else if (e.KeyChar == 'f' || isDebuggingInterpreter)
            {


                if (!isDebuggingInterpreter)
                {
                    string filename = Console.ReadLine();
                    while (!File.Exists(filename))
                    {
                        filename = Console.ReadLine();
                    }
                    fli = new FilterLanguage.Interpreter(filename, CL.CreateEmpty<byte>(512 * 512 * 4, MemoryFlag.CopyHostPointer | MemoryFlag.ReadWrite), 512, 512, 1, 4, db);
                }

                FilterLanguage.Interpreter.InterpreterStepResult res = new FilterLanguage.Interpreter.InterpreterStepResult();

                if (isDebuggingInterpreter)
                {

                    this.Log("Continuing Execution", DebugChannel.Log);
                    isDebuggingInterpreter = false;
                }

                byte[] buf = null;

                do
                {
                    if (res.triggeredDebug)
                    {
                        this.Log("Triggered Debug.", DebugChannel.Log);
                        isDebuggingInterpreter = true;
                        buf = CL.ReadBuffer<byte>(res.debugBuffer, (int)res.debugBuffer.Size);
                        break;
                    }
                } while (!(res = fli.Step()).terminated);

                if (!isDebuggingInterpreter) buf = fli.GetResult<byte>();
                else if (buf == null) throw new InvalidOperationException("FUCK");
                GameTexture.Update(owner.model.meshes[0].textures[0], buf, 512, 512);
            }
            else if (e.KeyChar == 't')
            {
                string[] filenames = Directory.GetFiles("filter/tests", "*.fl");
                this.Log("Running tests...", DebugChannel.Log);
                foreach (string filename in filenames)
                {
                    fli = new FilterLanguage.Interpreter(filename, CL.CreateEmpty<byte>(512 * 512 * 4, MemoryFlag.CopyHostPointer | MemoryFlag.ReadWrite), 512, 512, 1, 4, db);

                    this.Log("Running Test File: " + filename, DebugChannel.Log);


                    while (!fli.Terminated)
                    {
                        fli.Step();
                    }

                }
                GL.DeleteTexture(owner.model.meshes[0].textures[0].textureID);
                owner.model.meshes[0].textures[0] = GameTexture.Load("textures/runicfloor.png");

                this.Log("Finished Running Tests", DebugChannel.Log);

            }
        }

        //private void CreateWorley()
        //{
        //    List<float> points = new List<float>();
        //    Random rnd = new Random();
        //    for (int i = 0; i < 240; i++)
        //    {
        //        points.Add((float)rnd.NextDouble());
        //    }
        //    GL.DeleteTexture(owner.model.meshes[0].textures[0].textureID);
        //    owner.model.meshes[0].textures[0] = CLFilterAPI.GetInstance().GetWorley(points, 512, 512);



        //}
    }
}