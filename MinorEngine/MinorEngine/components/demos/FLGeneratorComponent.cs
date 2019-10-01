using System.Collections.Generic;
using CLHelperLibrary;
using CLHelperLibrary.CLStructs;
using FilterLanguage;
using GameEngine.engine.components;
using GameEngine.engine.core;
using GameEngine.engine.rendering;
using GameEngine.engine.ui.utils;
using OpenCl.DotNetCore.Memory;
using OpenTK.Graphics.OpenGL;

namespace GameEngine.components.fldemo
{
    public class FLGeneratorComponent : AbstractComponent
    {
        private readonly List<MeshRendererComponent> _previews;
        private GameTexture Tex { get; set; }
        private Interpreter _stepInterpreter;
        private KernelDatabase _db;
        private bool _isInStepMode;
        private int width = 512, height = 512;

        public FLGeneratorComponent(List<MeshRendererComponent> previews, int width, int height)
        {
            _previews = previews;
            Tex = GameTexture.Create(width, height, true);
        }

        private string cmd_RunFL(string[] args)
        {
            if (args.Length != 1)
            {
                return "Only One Filepath.";
            }

            RunOnObjImage(args[0]);

            return "Command Finished";
        }

        private string cmd_FLReset(string[] args)
        {

            TextureProvider.GiveBack(Tex);
            Tex = GameTexture.Create(width, height, true);
            return "Texture Reset.";
        }


        protected override void Awake()
        {
            DebugConsoleComponent console = Owner.World.GetChildWithName("Console").GetComponent<DebugConsoleComponent>();
            console?.AddCommand("runfl", cmd_RunFL);
            console?.AddCommand("dbgfl", cmd_RunFLStepped);
            console?.AddCommand("step", cmd_FLStep);
            console?.AddCommand("r", cmd_FLReset);
            console?.AddCommand("dbgstop", cmd_FLReset);
            _db = new KernelDatabase("kernel/", DataTypes.UCHAR1);
        }

        protected override void OnDestroy()
        {
            TextureProvider.GiveBack(Tex);
        }

        private MemoryBuffer GetRendererTextureBuffer()
        {
            return GameTexture.CreateFromTexture(Tex, MemoryFlag.CopyHostPointer | MemoryFlag.ReadWrite);

        }

        public void RunOnObjImage(string filename)
        {
            MemoryBuffer buf = GetRendererTextureBuffer();

            Interpreter interpreter = new Interpreter(filename, buf, (int)Tex.Width, (int)Tex.Height, 1, 4, _db, true);


            do
            {
                interpreter.Step();
            } while (!interpreter.Terminated);

            GameTexture.Update(Tex, interpreter.GetResult<byte>(), (int)Tex.Width, (int)Tex.Height);

            for (int i = 0; i < _previews.Count; i++)
            {
                _previews[i].Model.Meshes[0].Textures = new[] { Tex };
            }
        }

        private string cmd_FLStop(string[] args)
        {
            if (!_isInStepMode)
            {
                return "Not in an active Debugging Session";
            }

            _stepInterpreter = null;
            _isInStepMode = false;

            return "Session Aborted.";
        }
        private string cmd_FLStep(string[] args)
        {
            if (!_isInStepMode)
            {
                return "Not in an active Debugging Session";
            }

            Interpreter.InterpreterStepResult stepResult = _stepInterpreter.Step();
            MemoryBuffer res = stepResult.DebugBuffer;
            if (_stepInterpreter.Terminated)
            {
                _isInStepMode = false;
                res = _stepInterpreter.GetResultBuffer();
            }

            GameTexture.Update(Tex, CL.ReadBuffer<byte>(res, (int)res.Size), (int)Tex.Width, (int)Tex.Height);

            for (int i = 0; i < _previews.Count; i++)
            {
                _previews[i].Model.Meshes[0].Textures = new[] { Tex };
            }

            return stepResult.ToString();
        }
        private string cmd_RunFLStepped(string[] args)
        {
            if (args.Length == 0)
            {
                return "No file specified.";
            }
            MemoryBuffer buf = GetRendererTextureBuffer();


            _isInStepMode = true;
            _stepInterpreter = new Interpreter(args[0], buf, (int)Tex.Width, (int)Tex.Height, 1, 4, _db, false);

            return "Debugging Session Started.";


        }
    }
}