using System.Collections.Generic;
using MinorEngine.CLHelperLibrary;
using MinorEngine.CLHelperLibrary.cltypes;
using MinorEngine.engine.components;
using MinorEngine.engine.core;
using MinorEngine.engine.rendering;
using MinorEngine.FilterLanguage;
using OpenCl.DotNetCore.Memory;

namespace MinorEngine.components
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
            Tex = ResourceManager.TextureIO.ParameterToTexture(width, height, "FLTexture");
            return "Texture Reset.";
        }


        protected override void Awake()
        {
            Tex = ResourceManager.TextureIO.ParameterToTexture(width, height, "FLTexture");

            for (var i = 0; i < _previews.Count; i++)
            {
                _previews[i].Model.SetTextureBuffer(new[] {Tex});
            }


            var console =
                Owner.World.GetChildWithName("Console").GetComponent<DebugConsoleComponent>();
            console?.AddCommand("runfl", cmd_RunFL);
            console?.AddCommand("dbgfl", cmd_RunFLStepped);
            console?.AddCommand("step", cmd_FLStep);
            console?.AddCommand("r", cmd_FLReset);
            console?.AddCommand("dbgstop", cmd_FLStop);
            _db = new KernelDatabase("kernel/", DataTypes.UCHAR1);
        }

        protected override void OnDestroy()
        {
            Tex = null;
            _previews.Clear();
        }

        private MemoryBuffer GetRendererTextureBuffer()
        {
            return ResourceManager.TextureIO.TextureToMemoryBuffer(Tex);
        }

        public void RunOnObjImage(string filename)
        {
            var buf = GetRendererTextureBuffer();

            var interpreter =
                new Interpreter(filename, buf, (int) Tex.Width, (int) Tex.Height, 1, 4, _db, true);


            do
            {
                interpreter.Step();
            } while (!interpreter.Terminated);

            ResourceManager.TextureIO.Update(Tex, interpreter.GetResult<byte>(), (int) Tex.Width, (int) Tex.Height);
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

            var stepResult = _stepInterpreter.Step();
            var res = stepResult.DebugBuffer;
            if (_stepInterpreter.Terminated)
            {
                _isInStepMode = false;
                res = _stepInterpreter.GetResultBuffer();
            }

            ResourceManager.TextureIO.Update(Tex, CL.ReadBuffer<byte>(res, (int) res.Size), (int) Tex.Width,
                (int) Tex.Height);

            return stepResult.ToString();
        }

        private string cmd_RunFLStepped(string[] args)
        {
            if (args.Length == 0)
            {
                return "No file specified.";
            }

            var buf = GetRendererTextureBuffer();


            _isInStepMode = true;
            _stepInterpreter = new Interpreter(args[0], buf, (int) Tex.Width, (int) Tex.Height, 1, 4, _db, false);

            return "Debugging Session Started.";
        }
    }
}