﻿using System.Collections.Generic;
using Engine.Core;
using Engine.DataTypes;
using Engine.Debug;
using Engine.IO;
using Engine.OpenCL;
using Engine.OpenCL.DotNetCore.Memory;
using Engine.Rendering;

namespace Engine.OpenFL
{

    /// <summary>
    /// FLGeneratorComponent that implements a Demo usecase of OpenFL
    /// </summary>
    public class FLGeneratorComponent : AbstractComponent
    {
        /// <summary>
        /// List of previews
        /// </summary>
        private readonly List<MeshRendererComponent> _previews;

        /// <summary>
        /// the texture that is beeing used to update the previews
        /// </summary>
        private Texture Tex { get; set; }

        /// <summary>
        /// The FL Interpreter
        /// </summary>
        private Interpreter _stepInterpreter;
        /// <summary>
        /// The Kernel Database that is used to provide the kernels that the interpreter uses
        /// </summary>
        private KernelDatabase _db;

        /// <summary>
        /// Flag to indicate that an active debugging session is running.
        /// </summary>
        private bool _isInStepMode;

        /// <summary>
        /// The width of the output texture
        /// </summary>
        private int width = 512;

        /// <summary>
        /// The height of the output texture
        /// </summary>
        private int height = 512;


        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="previews">List of previews</param>
        /// <param name="width">Width/height of the output texture</param>
        /// <param name="height"></param>
        public FLGeneratorComponent(List<MeshRendererComponent> previews, int width, int height)
        {
            this.width = width;
            this.height = height;
            _previews = previews;
        }

        /// <summary>
        /// Command to run a FL script
        /// </summary>
        /// <param name="args">The filename to the script</param>
        /// <returns>Function Result</returns>
        private string cmd_RunFL(string[] args)
        {
            if (args.Length != 1)
            {
                return "Only One Filepath.";
            }

            RunOnObjImage(args[0]);

            return "Command Finished";
        }


        /// <summary>
        /// Command to reset the Output Texture
        /// </summary>
        /// <param name="args">None</param>
        /// <returns>Function Result</returns>
        private string cmd_FLReset(string[] args)
        {
            Tex = TextureLoader.ParameterToTexture(width, height);
            return "Texture Reset.";
        }

        /// <summary>
        /// Overridden Awake method for setting up the Interpreter and add the commands to the console
        /// </summary>
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

        /// <summary>
        /// OnDestroy Function. Gets called when the Component or the GameObject got removed from the game
        /// This function is called AFTER the engines update function. So it can happen that before the object is destroyed it can still collide and do other things until its removed at the end of the frame.
        /// </summary>
        protected override void OnDestroy()
        {
            Tex=null;
            _previews.Clear();
        }


        /// <summary>
        /// Converts the preview texture into an Memory Buffer
        /// </summary>
        /// <returns>The CL buffer with the contents of the Preview Texture</returns>
        private MemoryBuffer GetRendererTextureBuffer()
        {
            return TextureLoader.TextureToMemoryBuffer(Tex);
        }

        /// <summary>
        /// Runs a FL Script
        /// </summary>
        /// <param name="filename">Path to the FL Script</param>
        public void RunOnObjImage(string filename)
        {
            var buf = GetRendererTextureBuffer();

            var interpreter =
                new Interpreter(filename, buf, (int)Tex.Width, (int)Tex.Height, 1, 4, _db, true);


            do
            {
                interpreter.Step();
            } while (!interpreter.Terminated);

            byte[] retbuf = interpreter.GetResult<byte>();
            TextureLoader.Update(Tex, retbuf, (int)Tex.Width, (int)Tex.Height);
        }

        /// <summary>
        /// Aborts a running debugging session
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Proceeds to the next operation when in an active debugging session
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
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


        /// <summary>
        /// Runs a FL Script in stepped mode
        /// </summary>
        /// <param name="filename">Path to the FL Script</param>
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