using System;
using System.Collections.Generic;
using System.IO;
using BepuPhysics;
using CLHelperLibrary;
using CLHelperLibrary.CLStructs;
using Common;
using GameEngine.engine.physics;
using GameEngine.engine.audio;
using GameEngine.engine.audio.sources;
using GameEngine.engine.components;
using GameEngine.engine.core;
using GameEngine.engine.rendering;
using OpenCl.DotNetCore.Memory;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using Random = System.Random;

namespace GameEngine.components
{
    public class TextureChanger : AbstractComponent
    {
        private AbstractAudioSource source;
        private MeshRendererComponent renderer;
        private ShaderProgram _runicShader;
        private GameModel sphere, box;

        private int texWidth, texHeight, testNr;
        private string[] filenames;
        

        protected override void Awake()
        {
            base.Awake();
            ShaderProgram.TryCreate(new Dictionary<ShaderType, string>
            {
                {ShaderType.FragmentShader, "shader/texture.fs"},
                {ShaderType.VertexShader, "shader/texture.vs"},
            }, out _runicShader);


            sphere = new GameModel("models/sphere_smooth.obj");
            box = new GameModel("models/cube_flat.obj");
            box.Meshes[0].Textures = new[] { GameTexture.Load("textures/TEST.png") };
            sphere.Meshes[0].Textures = new[] { GameTexture.Load("textures/TEST.png") };
            testNr = -1;

            filenames = Directory.GetFiles("filter/tests", "*.fl");
            source = Owner.GetComponentIterative<AbstractAudioSource>();
            renderer = Owner.GetComponent<MeshRendererComponent>();
            GL.BindTexture(TextureTarget.Texture2D, renderer.Model.Meshes[0].Textures[0].TextureId);
            

            if (!AudioManager.TryLoad("sounds/test_mono_16.wav", out AudioClip clip))
            {
                Console.Read();
            }

            source.SetClip(clip);
            source.Looping = true;
        }


        private bool _isDebuggingInterpreter;

        private FilterLanguage.Interpreter _fli;
        private readonly KernelDatabase _db = new KernelDatabase("kernel/", DataTypes.UCHAR1);

        protected override void OnKeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 'r')
            {
                GL.DeleteTexture(renderer.Model.Meshes[0].Textures[0].TextureId);
                renderer.Model.Meshes[0].Textures[0] = GameTexture.Load("textures/runicfloor.png");
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

                    if (_fli == null)
                    {
                        _fli = new FilterLanguage.Interpreter(filename,
                            CL.CreateEmpty<byte>(texWidth * texHeight * 4, MemoryFlag.CopyHostPointer | MemoryFlag.ReadWrite), texWidth,
                            texHeight, 1, 4, _db);
                    }
                    else
                    {
                        _fli.Reset(filename,
                            CL.CreateEmpty<byte>(texWidth * texHeight * 4, MemoryFlag.CopyHostPointer | MemoryFlag.ReadWrite), texWidth,
                            texHeight, 1, 4, _db);
                    }
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
                GameTexture.Update(renderer.Model.Meshes[0].Textures[0], buf, texWidth, texHeight);
            }
            else if (e.KeyChar == 'l')
            {
                if (testNr == -1)
                {
                    testNr = 0;
                }
                else if (testNr == filenames.Length)
                {
                    testNr = -1;

                    this.Log("Finished Running Tests", DebugChannel.Log);
                }
                else
                {
                    testNr++;
                }
                this.Log("Running test...", DebugChannel.Log);

                _fli = new FilterLanguage.Interpreter(filenames[testNr],
                    CL.CreateEmpty<byte>(texWidth * texHeight * 4,
                        MemoryFlag.CopyHostPointer | MemoryFlag.ReadWrite), texWidth, texHeight, 1, 4, _db);

                this.Log("Running Test File: " + filenames[testNr], DebugChannel.Log);


                while (!_fli.Terminated)
                {
                    _fli.Step();
                }

                GameTexture.Update(renderer.Model.Meshes[0].Textures[0], _fli.GetResult<byte>(), texWidth, texHeight);



            }
            else if (e.KeyChar == 't')
            {
                string[] files = Directory.GetFiles("filter/tests", "*.fl");
                this.Log("Running tests...", DebugChannel.Log);
                foreach (string filename in files)
                {
                    _fli = new FilterLanguage.Interpreter(filename,
                        CL.CreateEmpty<byte>(texWidth * texHeight * 4,
                            MemoryFlag.CopyHostPointer | MemoryFlag.ReadWrite), texWidth, texHeight, 1, 4, _db);

                    this.Log("Running Test File: " + filename, DebugChannel.Log);


                    while (!_fli.Terminated)
                    {
                        _fli.Step();
                    }

                }
                GL.DeleteTexture(renderer.Model.Meshes[0].Textures[0].TextureId);
                renderer.Model.Meshes[0].Textures[0] = GameTexture.Load("textures/runicfloor.png");

                this.Log("Finished Running Tests", DebugChannel.Log);

            }
            else if (e.KeyChar == 'm' && !source.IsPlaying)
            {
                source.Play();
            }
            else if (e.KeyChar == 'n' && source.IsPlaying)
            {
                source.Stop();
            }
            else if (e.KeyChar == 'p' && source.IsPlaying)
            {
                source.Pause();
            }
            else if (e.KeyChar == 'i')
            {
                Physics.Gravity *= -1;
            }
            else if (e.KeyChar == 'g')
            {
                int nmbrs = int.Parse(Console.ReadLine());
                Random rnd = new Random();
                for (int i = 0; i < nmbrs; i++)
                {

                    Vector3 pos = new Vector3((float)rnd.NextDouble(), 3 + (float)rnd.NextDouble(), (float)rnd.NextDouble());
                    pos -= Vector3.One * 0.5f;
                    pos *= 50;

                    GameObject obj = new GameObject(pos, "Sphere");
                    float radius = 0.3f + (float)rnd.NextDouble();
                    obj.Scale(new Vector3(radius));
                    if (rnd.Next(0, 2) == 1)
                    {
                        obj.AddComponent(new MeshRendererComponent(_runicShader, sphere, 0));
                        obj.AddComponent(new ColliderComponent(ColliderType.SPHERE, radius, 1, 1));
                    }
                    else
                    {
                        obj.AddComponent(new ColliderComponent(ColliderType.BOX, radius, 1, 1));
                        obj.AddComponent(new MeshRendererComponent(_runicShader, box, 0));

                    }
                    Owner.World.Add(obj);
                }
            }
        }
    }
}