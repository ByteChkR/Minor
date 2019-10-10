using System;
using Engine.DataTypes;
using Engine.Rendering;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace Engine.UI
{
    /// <summary>
    /// Implements Image Rendering in Screen Space
    /// </summary>
    public class UIImageRenderContext : UIRenderContext
    {
        /// <summary>
        /// Screen space quad
        /// </summary>
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

        /// <summary>
        /// The image that will be drawn to the screen
        /// </summary>
        public Texture Texture { get; set; }


        /// <summary>
        /// Private flag if the Screen Space Quad has been loaded
        /// </summary>
        private bool _init;

        private int _vao;

        /// <summary>
        /// public constructor
        /// </summary>
        /// <param name="position">Position in screen space</param>
        /// <param name="scale">The Scale of the Image</param>
        /// <param name="modelMatrix">Model matrix</param>
        /// <param name="tex">The texture to draw</param>
        /// <param name="worldSpace">Is the Object in world space</param>
        /// <param name="alpha">Alpha value of the image</param>
        /// <param name="program">The shader to be used</param>
        /// <param name="renderQueue">The Render queue</param>
        public UIImageRenderContext(Vector2 position, Vector2 scale, Matrix4 modelMatrix, Texture tex,
            bool worldSpace, float alpha, ShaderProgram program, int renderQueue) : base(position, scale, modelMatrix,
            worldSpace, alpha, program, renderQueue)
        {
            Texture = tex;
        }

        /// <summary>
        /// Sets up the Screen Space Quad
        /// </summary>
        private void SetUpGLBuffers()
        {
            _init = true;
            _vao = GL.GenVertexArray();
            int _screenVBO = GL.GenBuffer();
            GL.BindVertexArray(_vao);
            GL.BindBuffer(BufferTarget.ArrayBuffer, _screenVBO);

            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr) (_screenQuadVertexData.Length * sizeof(float)),
                _screenQuadVertexData, BufferUsageHint.StaticDraw);
            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, 4 * sizeof(float), IntPtr.Zero);
            GL.EnableVertexAttribArray(1);
            GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, 4 * sizeof(float), 2 * sizeof(float));
        }

        /// <summary>
        /// renders the image on the screen
        /// </summary>
        /// <param name="viewMat"></param>
        /// <param name="projMat"></param>
        public override void Render(Matrix4 viewMat, Matrix4 projMat)
        {
            if (!_init)
            {
                SetUpGLBuffers();
            }

            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
            Program.Use();

            Matrix4 mat = Matrix4.Identity;

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