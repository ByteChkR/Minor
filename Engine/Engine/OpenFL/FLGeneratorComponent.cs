using System.Collections.Generic;
using Engine.Core;
using Engine.DataTypes;
using Engine.Debug;
using Engine.IO;
using Engine.OpenCL;
using Engine.OpenCL.DotNetCore.Memory;
using Engine.OpenCL.Runner;
using Engine.Rendering;

namespace Engine.OpenFL
{
    /// <summary>
    /// FLGeneratorComponent that implements a Demo usecase of OpenFL
    /// </summary>
    public class FlGeneratorComponent : AbstractComponent
    {
        /// <summary>
        /// List of previews
        /// </summary>
        private readonly List<LitMeshRendererComponent> previews;

        private FlRunner flRunner;


        /// <summary>
        /// Flag to indicate that an active debugging session is running.
        /// </summary>
        private bool isInStepMode;

        /// <summary>
        /// The FL Interpreter
        /// </summary>
        private Interpreter stepInterpreter;

        /// <summary>
        /// The height of the output texture
        /// </summary>
        private int height = 512;

        private bool multiThread;

        /// <summary>
        /// The width of the output texture
        /// </summary>
        private int width = 512;


        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="previews">List of previews</param>
        /// <param name="width">Width/height of the output texture</param>
        /// <param name="height"></param>
        public FlGeneratorComponent(List<LitMeshRendererComponent> previews, int width, int height,
            bool multiThread = false)
        {
            this.multiThread = multiThread;

            this.width = width;
            this.height = height;
            this.previews = previews;
        }

        /// <summary>
        /// the texture that is beeing used to update the previews
        /// </summary>
        private Texture Tex { get; set; }

        /// <summary>
        /// the texture that is beeing used to update the previews
        /// </summary>
        private Texture SpecularTex { get; set; }

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
            Tex.Dispose();
            SpecularTex.Dispose();
            Tex = TextureLoader.ParameterToTexture(width, height);
            SpecularTex = TextureLoader.ParameterToTexture(width, height);
            SpecularTex.TexType = TextureType.Specular;
            for (int i = 0; i < previews.Count; i++)
            {
                previews[i].Textures[0] = Tex;
            }

            stepInterpreter.ReleaseResources();

            return "Texture Reset.";
        }

        /// <summary>
        /// Overridden Awake method for setting up the Interpreter and add the commands to the console
        /// </summary>
        protected override void Awake()
        {
            Tex = TextureLoader.ParameterToTexture(width, height);
            SpecularTex = TextureLoader.ParameterToTexture(width, height);
            SpecularTex.TexType = TextureType.Specular;
            for (int i = 0; i < previews.Count; i++)
            {
                previews[i].Textures[0] = Tex;
                previews[i].Textures[1] = SpecularTex;
            }


            if (multiThread)
            {
                flRunner = new FlMultiThreadRunner(null);
            }
            else
            {
                flRunner = new FlRunner(Clapi.MainThread);
            }


            DebugConsoleComponent console =
                Owner.Scene.GetChildWithName("Console").GetComponent<DebugConsoleComponent>();
            console?.AddCommand("runfl", cmd_RunFL);
            console?.AddCommand("dbgfl", cmd_RunFLStepped);
            console?.AddCommand("step", cmd_FLStep);
            console?.AddCommand("r", cmd_FLReset);
            console?.AddCommand("dbgstop", cmd_FLStop);
        }

        /// <summary>
        /// OnDestroy Function. Gets called when the Component or the GameObject got removed from the game
        /// This function is called AFTER the engines update function. So it can happen that before the object is destroyed it can still collide and do other things until its removed at the end of the frame.
        /// </summary>
        protected override void OnDestroy()
        {
            Tex = null;
            SpecularTex = null;
            previews.Clear();
        }


        /// <summary>
        /// Runs a FL Script
        /// </summary>
        /// <param name="filename">Path to the FL Script</param>
        public void RunOnObjImage(string filename)
        {
            if (isInStepMode)
            {
                return;
            }

            Dictionary<string, Texture> otherTex = new Dictionary<string, Texture>();
            otherTex.Add("result", Tex);
            otherTex.Add("specularOut", SpecularTex);
            MemoryBuffer b = TextureLoader.TextureToMemoryBuffer(Clapi.MainThread, Tex);

            byte[] buf = Clapi.ReadBuffer<byte>(Clapi.MainThread, b, (int) b.Size);
            FlExecutionContext exec = new FlExecutionContext(filename, Tex, otherTex, null);

            flRunner.Enqueue(exec);
            flRunner.Process();
        }

        /// <summary>
        /// Aborts a running debugging session
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        private string cmd_FLStop(string[] args)
        {
            if (!isInStepMode)
            {
                return "Not in an active Debugging Session";
            }

            stepInterpreter = null;
            isInStepMode = false;

            return "Session Aborted.";
        }

        /// <summary>
        /// Proceeds to the next operation when in an active debugging session
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        private string cmd_FLStep(string[] args)
        {
            if (!isInStepMode)
            {
                return "Not in an active Debugging Session";
            }

            InterpreterStepResult stepResult = stepInterpreter.Step();
            MemoryBuffer res = stepInterpreter.GetActiveBufferInternal().Buffer;
            if (stepInterpreter.Terminated)
            {
                isInStepMode = false;
                res = stepInterpreter.GetActiveBufferInternal().Buffer;
            }

            TextureLoader.Update(Tex, Clapi.ReadBuffer<byte>(Clapi.MainThread, res, (int) res.Size), (int) Tex.Width,
                (int) Tex.Height);
            ClBufferInfo spe = stepInterpreter.GetBuffer("specularOut");
            if (spe != null)
            {
                byte[] spec = Clapi.ReadBuffer<byte>(Clapi.MainThread, spe.Buffer, (int) spe.Buffer.Size);

                TextureLoader.Update(SpecularTex, spec, (int) SpecularTex.Width, (int) SpecularTex.Height);
            }


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

            MemoryBuffer buf = TextureLoader.TextureToMemoryBuffer(Clapi.MainThread, Tex);


            isInStepMode = true;
            stepInterpreter?.ReleaseResources();
            stepInterpreter = new Interpreter(Clapi.MainThread, args[0], OpenCL.TypeEnums.DataTypes.Uchar1, buf,
                (int) Tex.Width, (int) Tex.Height, 1, 4, "assets/kernel/", false);

            return "Debugging Session Started.";
        }
    }
}