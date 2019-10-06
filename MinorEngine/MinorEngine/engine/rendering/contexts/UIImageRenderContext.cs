using System;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace MinorEngine.engine.rendering.contexts
{
    public class UIImageRenderContext : UIRenderContext
    {
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

        public GameTexture Texture { get; set; }


        private bool _init;
        private int _vao;

        public UIImageRenderContext(Vector2 position, Vector2 scale, Matrix4 modelMatrix, GameTexture tex,
            bool worldSpace, float alpha, ShaderProgram program, int renderQueue) : base(position, scale, modelMatrix,
            worldSpace, alpha, program, renderQueue)
        {
            Texture = tex;
        }

        private void SetUpGLBuffers()
        {
            _init = true;
            _vao = GL.GenVertexArray();
            var _screenVBO = GL.GenBuffer();
            GL.BindVertexArray(_vao);
            GL.BindBuffer(BufferTarget.ArrayBuffer, _screenVBO);

            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr) (_screenQuadVertexData.Length * sizeof(float)),
                _screenQuadVertexData, BufferUsageHint.StaticDraw);
            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, 4 * sizeof(float), IntPtr.Zero);
            GL.EnableVertexAttribArray(1);
            GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, 4 * sizeof(float), 2 * sizeof(float));
        }

        public override void Render(Matrix4 viewMat, Matrix4 projMat)
        {
            if (!_init)
            {
                SetUpGLBuffers();
            }

            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
            Program.Use();

            var mat = Matrix4.Identity;

            if (WorldSpace)
            {
                mat = ModelMat * viewMat * projMat;
            }
            else
            {
                mat = ModelMat;
            }

            GL.UniformMatrix4(Program.GetUniformLocation("transform"), false, ref mat);
            GL.Uniform1(Program.GetUniformLocation("alpha"), Alpha);

            GL.Uniform1(Program.GetUniformLocation("uiTexture"), 0);
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindVertexArray(_vao);
            GL.BindTexture(TextureTarget.Texture2D, Texture.TextureId);

            GL.DrawArrays(PrimitiveType.Triangles, 0, 6);

            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindVertexArray(0);
            GL.BindTexture(TextureTarget.Texture2D, 0);

            GL.Disable(EnableCap.Blend);
        }
    }
}