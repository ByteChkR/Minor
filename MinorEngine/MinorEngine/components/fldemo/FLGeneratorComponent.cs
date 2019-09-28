using System.Collections.Generic;
using CLHelperLibrary;
using CLHelperLibrary.CLStructs;
using FilterLanguage;
using GameEngine.engine.components;
using GameEngine.engine.core;
using GameEngine.engine.rendering;
using OpenCl.DotNetCore.Memory;
using OpenTK.Input;

namespace GameEngine.components.fldemo
{
    public class FLGeneratorComponent : AbstractComponent
    {
        private List<MeshRendererComponent> _previews;
        private GameTexture _tex;
        private Interpreter _interpreter;
        private KernelDatabase _db;
        private bool _waitForInput = true;
        private FLGeneratorConsoleComponent _console;

        public FLGeneratorComponent(List<MeshRendererComponent> previews, GameTexture tex)
        {
            _previews = previews;
            _tex = tex;
        }

        protected override void Awake()
        {
            _console = Owner.GetComponent<FLGeneratorConsoleComponent>();
            _db = new KernelDatabase("kernel/", DataTypes.UCHAR1);
        }



        private MemoryBuffer GetRendererTextureBuffer()
        {
            return GameTexture.CreateFromTexture(_tex, MemoryFlag.CopyHostPointer | MemoryFlag.ReadWrite);

        }

        private void ReceiveInput(string name)
        {
            _waitForInput = true;
            RunOnObjImage(name);
        }

        protected override void OnKeyUp(object sender, KeyboardKeyEventArgs e)
        {
            if (_waitForInput)
            {
                if (e.Key == Key.C)
                {
                    _waitForInput = false;
                    _console.RequestInput(ReceiveInput);
                }
            }
        }

        public void RunOnObjImage(string filename)
        {
            MemoryBuffer buf = GetRendererTextureBuffer();

            _interpreter = new Interpreter(filename, buf, (int)_tex.Width, (int)_tex.Height, 1, 4, _db, false);


            do
            {
                _interpreter.Step();
            } while (!_interpreter.Terminated);

            GameTexture.Update(_tex, _interpreter.GetResult<byte>(), (int)_tex.Width, (int)_tex.Height);

            for (int i = 0; i < _previews.Count; i++)
            {
                _previews[i].Model.Meshes[0].Textures = new[] { _tex };
            }
        }
    }
}