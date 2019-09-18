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


        public TextureChanger(GameWindow window)
        {
            

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

        private bool _isDebuggingInterpreter;

        private FilterLanguage.Interpreter _fli;
        private readonly KernelDatabase _db = new KernelDatabase("kernel/");

        private void OnKeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 'r')
            {
                GL.DeleteTexture(Owner.Model.Meshes[0].Textures[0].TextureId);
                Owner.Model.Meshes[0].Textures[0] = GameTexture.Load("runicfloor.png");
                _isDebuggingInterpreter = false;
            }
            else if (e.KeyChar == 'f' || _isDebuggingInterpreter)
            {


                if (!_isDebuggingInterpreter)
                {
                    string filename = Console.ReadLine();
                    while (!File.Exists(filename))
                    {
                        filename = Console.ReadLine();
                    }
                    _fli = new FilterLanguage.Interpreter(filename, CL.CreateEmpty<byte>(512 * 512 * 4, MemoryFlag.CopyHostPointer | MemoryFlag.ReadWrite), 512, 512, 1, 4, _db);
                }

                FilterLanguage.Interpreter.InterpreterStepResult res = new FilterLanguage.Interpreter.InterpreterStepResult();

                if (_isDebuggingInterpreter)
                {

                    this.Log("Continuing Execution", DebugChannel.Log);
                    _isDebuggingInterpreter = false;
                }

                byte[] buf = null;

                do
                {
                    if (res.TriggeredDebug)
                    {
                        this.Log("Triggered Debug.", DebugChannel.Log);
                        _isDebuggingInterpreter = true;
                        buf = CL.ReadBuffer<byte>(res.DebugBuffer, (int)res.DebugBuffer.Size);
                        break;
                    }

                    res = _fli.Step();
                } while (!res.Terminated);

                if (!_isDebuggingInterpreter)
                {
                    buf = _fli.GetResult<byte>();
                }
                else if (buf == null)
                {
                    throw new InvalidOperationException("FUCK");
                }
                GameTexture.Update(Owner.Model.Meshes[0].Textures[0], buf, 512, 512);
            }
            else if (e.KeyChar == 't')
            {
                string[] filenames = Directory.GetFiles("filter/tests", "*.fl");
                this.Log("Running tests...", DebugChannel.Log);
                foreach (string filename in filenames)
                {
                    _fli = new FilterLanguage.Interpreter(filename, CL.CreateEmpty<byte>(512 * 512 * 4, MemoryFlag.CopyHostPointer | MemoryFlag.ReadWrite), 512, 512, 1, 4, _db);

                    this.Log("Running Test File: " + filename, DebugChannel.Log);


                    while (!_fli.Terminated)
                    {
                        _fli.Step();
                    }

                }
                GL.DeleteTexture(Owner.Model.Meshes[0].Textures[0].TextureId);
                Owner.Model.Meshes[0].Textures[0] = GameTexture.Load("textures/runicfloor.png");

                this.Log("Finished Running Tests", DebugChannel.Log);

            }
        }
    }
}