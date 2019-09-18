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
            if (!init && Owner != null)
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
                GL.DeleteTexture(Owner.Model.meshes[0].Textures[0].TextureId);
                Owner.Model.meshes[0].Textures[0] = GameTexture.Load("runicfloor.png");
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
                    if (res.TriggeredDebug)
                    {
                        this.Log("Triggered Debug.", DebugChannel.Log);
                        isDebuggingInterpreter = true;
                        buf = CL.ReadBuffer<byte>(res.DebugBuffer, (int)res.DebugBuffer.Size);
                        break;
                    }

                    res = fli.Step();
                } while (!res.Terminated);

                if (!isDebuggingInterpreter)
                {
                    buf = fli.GetResult<byte>();
                }
                else if (buf == null)
                {
                    throw new InvalidOperationException("FUCK");
                }
                GameTexture.Update(Owner.Model.meshes[0].Textures[0], buf, 512, 512);
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
                GL.DeleteTexture(Owner.Model.meshes[0].Textures[0].TextureId);
                Owner.Model.meshes[0].Textures[0] = GameTexture.Load("textures/runicfloor.png");

                this.Log("Finished Running Tests", DebugChannel.Log);

            }
        }
    }
}