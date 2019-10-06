using System;
using System.Collections.Generic;
using OpenTK.Graphics.OpenGL;

namespace MinorEngine.engine.rendering.ui
{
    public class UIHelper
    {
        private static UIHelper _instance;
        public static UIHelper Instance => _instance ?? (_instance = new UIHelper());

        public FontLibrary FontLibrary { get; }
        public int quadVAO { get; private set; }
        public ShaderProgram DefaultUIShader { get; private set; }

        private static float[] _screenQuadVertexData =
        {
            // positions   // texCoords
            -1.0f, 1.0f, 0.0f, 1.0f,
            -1.0f, -1.0f, 0.0f, 0.0f,
            1.0f, -1.0f, 1.0f, 0.0f,

            -1.0f, 1.0f, 0.0f, 1.0f,
            1.0f, -1.0f, 1.0f, 0.0f,
            1.0f, 1.0f, 1.0f, 1.0f
        };

        private UIHelper()
        {
            Initialize();
            FontLibrary = new FontLibrary("fonts/");
        }

        public static void InitializeUI()
        {
            _instance = new UIHelper();
        }

        private void Initialize()
        {
            quadVAO = GL.GenVertexArray();
            var screenVbo = GL.GenBuffer();
            GL.BindVertexArray(quadVAO);
            GL.BindBuffer(BufferTarget.ArrayBuffer, screenVbo);

            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr) (_screenQuadVertexData.Length * sizeof(float)),
                _screenQuadVertexData, BufferUsageHint.StaticDraw);
            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, 4 * sizeof(float), IntPtr.Zero);
            GL.EnableVertexAttribArray(1);
            GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, 4 * sizeof(float), 2 * sizeof(float));
            if (!ShaderProgram.TryCreate(new Dictionary<ShaderType, string>
            {
                {ShaderType.FragmentShader, "shader/ScreenRenderer.fs"},
                {ShaderType.VertexShader, "shader/UIRender.vs"}
            }, out var shader))
            {
                Console.ReadLine();
            }

            DefaultUIShader = shader;
        }
    }
}