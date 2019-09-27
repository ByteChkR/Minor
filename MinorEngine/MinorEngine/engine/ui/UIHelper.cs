using System;
using System.Collections.Generic;
using System.ComponentModel;
using MinorEngine.engine.rendering;
using OpenTK.Graphics.OpenGL;

namespace GameEngine.engine.ui
{
    public class UIHelper
    {
        private static UIHelper _instance = new UIHelper();
        public static UIHelper Instance => _instance ?? (_instance = new UIHelper());
        public int quadVAO { get; private set; }
        public ShaderProgram DefaultUIShader { get; private set; }
        private static float[] _screenQuadVertexData = new[]
        {
            // positions   // texCoords
            -1.0f,  1.0f,  0.0f, 1.0f,
            -1.0f, -1.0f,  0.0f, 0.0f,
            1.0f, -1.0f,  1.0f, 0.0f,

            -1.0f,  1.0f,  0.0f, 1.0f,
            1.0f, -1.0f,  1.0f, 0.0f,
            1.0f,  1.0f,  1.0f, 1.0f
        };

        private UIHelper()
        {
            Initialize();
        }
        private static void Initialize()
        {
            Instance.quadVAO = GL.GenVertexArray();
            int screenVbo = GL.GenBuffer();
            GL.BindVertexArray(Instance.quadVAO);
            GL.BindBuffer(BufferTarget.ArrayBuffer, screenVbo);

            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(_screenQuadVertexData.Length * sizeof(float)), _screenQuadVertexData, BufferUsageHint.StaticDraw);
            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, 4 * sizeof(float), IntPtr.Zero);
            GL.EnableVertexAttribArray(1);
            GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, 4 * sizeof(float), 2 * sizeof(float));
            if (!ShaderProgram.TryCreate(new Dictionary<ShaderType, string>
            {
                {ShaderType.FragmentShader, "shader/QuadRender.fs"},
                {ShaderType.VertexShader, "shader/UIRender.vs"}
            }, out ShaderProgram shader))
            {
                Console.ReadLine();
            }

            Instance.DefaultUIShader = shader;
        }
    }
}