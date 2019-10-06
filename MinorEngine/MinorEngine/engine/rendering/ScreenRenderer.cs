using System;
using System.Collections.Generic;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace MinorEngine.engine.rendering
{
    public static class ScreenRenderer
    {
        public enum MergeType
        {
            None,
            Additive,
            Multiplikative
        }

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

        private static bool _init;
        private static int _screenVAO;
        private static RenderTarget _screenTarget0 = new RenderTarget(new UICamera(), int.MaxValue, new Color(0, 0, 0, 0));
        private static RenderTarget _screenTarget1 = new RenderTarget(new UICamera(), int.MaxValue, new Color(0, 0, 0, 0));
        private static ShaderProgram _screenShader;
        private static Dictionary<MergeType, ShaderProgram> _mergeTypes = new Dictionary<MergeType, ShaderProgram>();

        private static void Init()
        {
            _init = true;
            _screenVAO = GL.GenVertexArray();
            int _screenVBO = GL.GenBuffer();
            GL.BindVertexArray(_screenVAO);
            GL.BindBuffer(BufferTarget.ArrayBuffer, _screenVBO);

            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(_screenQuadVertexData.Length * sizeof(float)),
                _screenQuadVertexData, BufferUsageHint.StaticDraw);
            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, 4 * sizeof(float), IntPtr.Zero);
            GL.EnableVertexAttribArray(1);
            GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, 4 * sizeof(float), 2 * sizeof(float));
            if (!ShaderProgram.TryCreate(new Dictionary<ShaderType, string>
            {
                {ShaderType.FragmentShader, "shader/MergeRenderer_Add.fs"},
                {ShaderType.VertexShader, "shader/MergeRenderer.vs"}
            }, out ShaderProgram _mergeAddShader))
                Console.ReadLine();
            _mergeTypes.Add(MergeType.Additive, _mergeAddShader);

            if (!ShaderProgram.TryCreate(new Dictionary<ShaderType, string>
            {
                {ShaderType.FragmentShader, "shader/MergeRenderer_Mul.fs"},
                {ShaderType.VertexShader, "shader/MergeRenderer.vs"}
            }, out ShaderProgram _mergeMulShader))
                Console.ReadLine();
            _mergeTypes.Add(MergeType.Multiplikative, _mergeMulShader);


            if (!ShaderProgram.TryCreate(new Dictionary<ShaderType, string>
            {
                {ShaderType.FragmentShader, "shader/ScreenRenderer.fs"},
                {ShaderType.VertexShader, "shader/ScreenRenderer.vs"}
            }, out _screenShader))
                Console.ReadLine();
        }

        private static bool _isOne;

        private static void Ping()
        {
            _isOne = !_isOne;
        }

        private static RenderTarget GetTarget()
        {
            return _isOne ? _screenTarget1 : _screenTarget0;
        }

        private static RenderTarget GetSource()
        {
            return _isOne ? _screenTarget0 : _screenTarget1;
        }

        public static void MergeAndDisplayTargets(List<RenderTarget> targets)
        {
            if (!_init) Init();


            int divideCount = targets.Count;

            //GL.Enable(EnableCap.ScissorTest);

            //GL.Enable(EnableCap.ScissorTest);
            foreach (var renderTarget in targets)
            {
                RenderTarget dst = GetTarget();
                RenderTarget src = GetSource();

                _mergeTypes[renderTarget.MergeType].Use();

                GL.BindFramebuffer(FramebufferTarget.Framebuffer, dst.FrameBuffer);

                //GL.Scissor(renderTarget.ViewPort.X, renderTarget.ViewPort.Y, renderTarget.ViewPort.Width, renderTarget.ViewPort.Height);
                //GL.Viewport(renderTarget.ViewPort.X, renderTarget.ViewPort.Y, renderTarget.ViewPort.Width, renderTarget.ViewPort.Height);
                GL.ClearColor(dst.ClearColor);
                GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

                //GL.Uniform1(_mergeShader.GetUniformLocation("divWeight"), 1 / (float)divideCount);

                GL.ActiveTexture(TextureUnit.Texture0);
                GL.Uniform1(_mergeTypes[renderTarget.MergeType].GetUniformLocation("destinationTexture"), 0);
                GL.BindTexture(TextureTarget.Texture2D, src.RenderedTexture);

                GL.ActiveTexture(TextureUnit.Texture1);
                GL.Uniform1(_mergeTypes[renderTarget.MergeType].GetUniformLocation("otherTexture"), 1);
                GL.BindTexture(TextureTarget.Texture2D, renderTarget.RenderedTexture);

                GL.BindVertexArray(_screenVAO);
                GL.DrawArrays(PrimitiveType.Triangles, 0, 6);
                //GL.Scissor(0, 0, GameEngine.Instance.Width, GameEngine.Instance.Height);
                //GL.Viewport(0, 0, GameEngine.Instance.Width, GameEngine.Instance.Height);


                Ping();


                //GL.DrawArrays(PrimitiveType.Triangles, 0, 6);
            }

            Ping();
            _screenShader.Use();

            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
            //GL.Disable(EnableCap.DepthTest);

            //GL.Disable(EnableCap.ScissorTest);

            GL.ClearColor(new Color(168, 143, 50, 255));
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);


            GL.ActiveTexture(TextureUnit.Texture0);
            GL.Uniform1(_screenShader.GetUniformLocation("sourceTexture"), 0);
            GL.BindTexture(TextureTarget.Texture2D, GetTarget().RenderedTexture);

            //GL.Scissor(0, 0, GameEngine.Instance.Width, GameEngine.Instance.Height);


            //GL.ActiveTexture(TextureUnit.Texture1);
            //GL.Uniform1(_mergeShader.GetUniformLocation("otherTexture"), 1);
            //GL.BindTexture(TextureTarget.Texture2D, renderTarget.RenderedTexture);

            GL.BindVertexArray(_screenVAO);

            //GL.Viewport(0, 0, GameEngine.Instance.Width, GameEngine.Instance.Height);
            //GL.Scissor(0, 0, GameEngine.Instance.Width, GameEngine.Instance.Height);
            GL.DrawArrays(PrimitiveType.Triangles, 0, 6);

            GL.BindVertexArray(0);
            GL.ActiveTexture(TextureUnit.Texture0);


            //GL.BindVertexArray(0);
            //GL.ActiveTexture(TextureUnit.Texture0);


            //Clear the ping pong buffers after rendering them to the screen
            //For whatever reason GL.Clear is not acting on the active framebuffer
            GL.ClearTexImage(_screenTarget1.RenderedTexture, 0, PixelFormat.Bgra, PixelType.UnsignedByte, IntPtr.Zero);
            GL.ClearTexImage(_screenTarget0.RenderedTexture, 0, PixelFormat.Bgra, PixelType.UnsignedByte, IntPtr.Zero);
        }
    }
}