using System;
using System.Collections.Generic;
using Engine.Debug;
using Engine.Rendering;
using OpenTK.Graphics.OpenGL;

namespace Engine.UI
{
    /// <summary>
    /// Class that is providing some helper functions to the ui rendering
    /// </summary>
    public class UIHelper
    {
        /// <summary>
        /// Backing field of Instance
        /// </summary>
        private static UIHelper _instance;

        /// <summary>
        /// The instance
        /// </summary>
        public static UIHelper Instance => _instance ?? (_instance = new UIHelper());

        /// <summary>
        /// The Font library that is storing all the fonts loaded into the system
        /// </summary>
        public FontLibrary FontLibrary { get; }

        /// <summary>
        /// The Default Shader for UI Rendering
        /// </summary>
        public ShaderProgram DefaultUIShader { get; private set; }


        /// <summary>
        /// Private Constructor
        /// </summary>
        private UIHelper()
        {
            FontLibrary = new FontLibrary("assets/fonts/");
            Initialize();
        }

        /// <summary>
        /// Initializes the Default Shader
        /// </summary>
        private void Initialize()
        {
            //quadVAO = GL.GenVertexArray();
            //var screenVbo = GL.GenBuffer();
            //GL.BindVertexArray(quadVAO);
            //GL.BindBuffer(BufferTarget.ArrayBuffer, screenVbo);

            //GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr) (_screenQuadVertexData.Length * sizeof(float)),
            //    _screenQuadVertexData, BufferUsageHint.StaticDraw);
            //GL.EnableVertexAttribArray(0);
            //GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, 4 * sizeof(float), IntPtr.Zero);
            //GL.EnableVertexAttribArray(1);
            //GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, 4 * sizeof(float), 2 * sizeof(float));

            if (!ShaderProgram.TryCreate(new Dictionary<ShaderType, string>
            {
                {ShaderType.FragmentShader, "assets/shader/ScreenRenderer.fs"},
                {ShaderType.VertexShader, "assets/shader/UIRender.vs"}
            }, out ShaderProgram shader))
            {
                Console.ReadLine();
            }

            DefaultUIShader = shader;
        }
    }
}