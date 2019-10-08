using System.Collections.Generic;
using System.Resources;
using Engine.Core;
using Engine.DataTypes;
using Engine.Debug;
using Engine.IO;
using Engine.OpenCL;
using Engine.Rendering;
using OpenCl.DotNetCore.Memory;

namespace Engine.OpenFL
{
    public class FLGeneratorComponent : AbstractComponent
    {
        private readonly List<MeshRendererComponent> _previews;
        private Texture Tex { get; set; }
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
            Tex = TextureLoader.ParameterToTexture(width, height);
            return "Texture Reset.";
        }


        protected override void Awake()
        {
            Tex = TextureLoader.ParameterToTexture(width, height);

            for (var i = 0; i < _previews.Count; i++)
            {
                _previews[i].Texture = Tex;
            }


            var console =
                Owner.Scene.GetChildWithName("Console").GetComponent<DebugConsoleComponent>();
            console?.AddCommand("runfl", cmd_RunFL);
            console?.AddCommand("dbgfl", cmd_RunFLStepped);
            console?.AddCommand("step", cmd_FLStep);
            console?.AddCommand("r", cmd_FLReset);
            console?.AddCommand("dbgstop", cmd_FLStop);
            _db = new KernelDatabase("kernel/", OpenCL.TypeEnums.DataTypes.UCHAR1);
        }

        protected override void OnDestroy()
        {
            Tex = null;
            _previews.Clear();
        }

        private CLBuffer GetRendererTextureBuffer()
        {
            return TextureLoader.TextureToMemoryBuffer(Tex);
        }

        public void RunOnObjImage(string filename)
        {
            var buf = GetRendererTextureBuffer();

            var interpreter =
                new Interpreter(filename, buf, (int)Tex.Width, (int)Tex.Height, 1, 4, _db, true);


            do
            {
                interpreter.Step();
            } while (!interpreter.Terminated);

            TextureLoader.Update(Tex, interpreter.GetResult<byte>(), (int)Tex.Width, (int)Tex.Height);
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

            TextureLoader.Update(Tex, CLAPI.ReadBuffer<byte>(res, (int)res.Size), (int)Tex.Width,
                (int)Tex.Height);

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
            _stepInterpreter = new Interpreter(args[0], buf, (int)Tex.Width, (int)Tex.Height, 1, 4, _db, false);

            return "Debugging Session Started.";
        }
    }
}