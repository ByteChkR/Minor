using System;
using System.Collections.Generic;
using GameEngine.engine.rendering;
using OpenTK.Graphics.OpenGL;

namespace MinorEngine.engine.rendering
{
    public class ScreenRenderer
    {
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

        private static bool _init = false;
        private static int _screenVAO;
        private static int _screenVBO;
        private static ShaderProgram _mergeShader;
        private static void Init()
        {


            _init = true;
            _screenVAO = GL.GenVertexArray();
            _screenVBO = GL.GenBuffer();
            GL.BindVertexArray(_screenVAO);
            GL.BindBuffer(BufferTarget.ArrayBuffer, _screenVBO);

            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(_screenQuadVertexData.Length * sizeof(float)), _screenQuadVertexData, BufferUsageHint.StaticDraw);
            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, 4 * sizeof(float), IntPtr.Zero);
            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, 4 * sizeof(float), 2 * sizeof(float));
            if (!ShaderProgram.TryCreate(new Dictionary<ShaderType, string>
            {
                {ShaderType.FragmentShader, "shader/OverlayRenderer.fs"},
                {ShaderType.VertexShader, "shader/OverlayRenderer.vs"}
            }, out _mergeShader))
            {
                Console.ReadLine();
            }


        }
        public static void MergeAndDisplayTargets(List<RenderTarget> targets)
        {
            if (!_init) Init();

            int divideCount = targets.Count;
            ErrorCode erc = GL.GetError();
            _mergeShader.Use();
            erc = GL.GetError();
            GL.BindVertexArray(_screenVAO);
            foreach (var renderTarget in targets)
            {
                 ErrorCode ec = GL.GetError();
                if (ec != ErrorCode.NoError)
                {
                    Console.Read();
                }
                
                GL.BindTexture(TextureTarget.Texture2D, renderTarget.RenderedTexture);
                GL.Uniform1(_mergeShader.GetUniformLocation("screenTexture"), renderTarget.RenderedTexture);

                ec = GL.GetError();
                if (ec != ErrorCode.NoError)
                {
                    Console.Read();
                }
                GL.DrawArrays(PrimitiveType.Triangles, 0, 6);
                ec = GL.GetError();
                if (ec != ErrorCode.NoError)
                {
                    Console.Read();
                }
            }




        }
    }
}