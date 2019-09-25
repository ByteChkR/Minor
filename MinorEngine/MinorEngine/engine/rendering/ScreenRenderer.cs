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
        private static RenderTarget _screenTarget = new RenderTarget(new UICamera(), int.MaxValue, OpenTK.Color.Black);
        private static ShaderProgram _mergeShader;
        private static ShaderProgram _screenShader;
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
            GL.EnableVertexAttribArray(1);
            GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, 4 * sizeof(float), 2 * sizeof(float));
            if (!ShaderProgram.TryCreate(new Dictionary<ShaderType, string>
            {
                {ShaderType.FragmentShader, "shader/OverlayRenderer.fs"},
                {ShaderType.VertexShader, "shader/OverlayRenderer.vs"}
            }, out _mergeShader))
            {
                Console.ReadLine();
            }

            if (!ShaderProgram.TryCreate(new Dictionary<ShaderType, string>
            {
                {ShaderType.FragmentShader, "shader/QuadRender.fs"},
                {ShaderType.VertexShader, "shader/OverlayRenderer.vs"}
            }, out _screenShader))
            {
                Console.ReadLine();
            }


        }
        public static void MergeAndDisplayTargets(List<RenderTarget> targets)
        {
            if (!_init) Init();



            int divideCount = targets.Count;
            _mergeShader.Use();

            GL.BindFramebuffer(FramebufferTarget.Framebuffer, _screenTarget.FrameBuffer);
            
            GL.Disable(EnableCap.DepthTest);
            GL.Clear(ClearBufferMask.ColorBufferBit);
            foreach (var renderTarget in targets)
            {
                GL.Uniform1(_mergeShader.GetUniformLocation("divWeight"), 1/(float)divideCount);

                GL.ActiveTexture(TextureUnit.Texture0);
                GL.Uniform1(_mergeShader.GetUniformLocation("destinationTexture"), 0);
                GL.BindTexture(TextureTarget.Texture2D, _screenTarget.RenderedTexture);

                GL.ActiveTexture(TextureUnit.Texture1);
                GL.Uniform1(_mergeShader.GetUniformLocation("otherTexture"), 1);
                GL.BindTexture(TextureTarget.Texture2D, renderTarget.RenderedTexture);


                GL.BindVertexArray(_screenVAO);
                GL.DrawArrays(PrimitiveType.Triangles, 0, 6);


                GL.BindVertexArray(0);
                GL.ActiveTexture(TextureUnit.Texture0);
            }


            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
            GL.Disable(EnableCap.DepthTest);
            GL.Clear(ClearBufferMask.ColorBufferBit);

            GL.ActiveTexture(TextureUnit.Texture0);
            GL.Uniform1(_screenShader.GetUniformLocation("sourceTexture"), 0);
            GL.BindTexture(TextureTarget.Texture2D, _screenTarget.RenderedTexture);
            

            GL.BindVertexArray(_screenVAO);
            GL.DrawArrays(PrimitiveType.Triangles, 0, 6);


            GL.BindVertexArray(0);
            GL.ActiveTexture(TextureUnit.Texture0);

            




        }
    }
}