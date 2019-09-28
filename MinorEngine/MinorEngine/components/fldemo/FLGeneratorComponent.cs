using System.Collections.Generic;
using CLHelperLibrary;
using CLHelperLibrary.CLStructs;
using FilterLanguage;
using GameEngine.engine.components;
using GameEngine.engine.core;
using GameEngine.engine.rendering;
using GameEngine.engine.ui.utils;
using OpenCl.DotNetCore.Memory;
using OpenTK.Input;

namespace GameEngine.components.fldemo
{
    public class FLGeneratorComponent : AbstractComponent
    {
        private readonly List<MeshRendererComponent> _previews;
        private readonly GameTexture _tex;
        private Interpreter _stepInterpreter;
        private KernelDatabase _db;
        private bool _isInStepMode;

        public FLGeneratorComponent(List<MeshRendererComponent> previews, GameTexture tex)
        {
            _previews = previews;
            _tex = tex;
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

        
        protected override void Awake()
        {
            DebugConsoleComponent console = AbstractGame.Instance.World.GetChildWithName("Console").GetComponent<DebugConsoleComponent>();
            console?.AddCommand("runfl", cmd_RunFL);
            console?.AddCommand("dbgfl", cmd_RunFLStepped);
            console?.AddCommand("step", cmd_FLStep);
            console?.AddCommand("dbgstop", cmd_FLStop);
            _db = new KernelDatabase("kernel/", DataTypes.UCHAR1);
        }



        private MemoryBuffer GetRendererTextureBuffer()
        {
            return GameTexture.CreateFromTexture(_tex, MemoryFlag.CopyHostPointer | MemoryFlag.ReadWrite);

        }

        public void RunOnObjImage(string filename)
        {
            MemoryBuffer buf = GetRendererTextureBuffer();

            Interpreter interpreter = new Interpreter(filename, buf, (int)_tex.Width, (int)_tex.Height, 1, 4, _db, true);


            do
            {
                interpreter.Step();
            } while (!interpreter.Terminated);

            GameTexture.Update(_tex, interpreter.GetResult<byte>(), (int)_tex.Width, (int)_tex.Height);

            for (int i = 0; i < _previews.Count; i++)
            {
                _previews[i].Model.Meshes[0].Textures = new[] { _tex };
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

            GameTexture.Update(_tex, CL.ReadBuffer<byte>(res, (int)res.Size), (int)_tex.Width, (int)_tex.Height);

            for (int i = 0; i < _previews.Count; i++)
            {
                _previews[i].Model.Meshes[0].Textures = new[] { _tex };
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
                _stepInterpreter = new Interpreter(args[0], buf, (int) _tex.Width, (int) _tex.Height, 1, 4, _db, false);
        
            return "Debugging Session Started.";

            
        }
    }
}