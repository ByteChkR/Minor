using System;
using Engine.DataTypes;
using Engine.IO;
using Engine.Rendering;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace Engine.UI
{
    /// <summary>
    /// A Component that is rendering an image in camera space
    /// </summary>
    public class UIImageRendererComponent : UIElement
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
        /// Private flag if the Screen Space Quad has been loaded
        /// </summary>
        private bool _init;

        private int _vao;

        /// <summary>
        /// public contstructor
        /// </summary>
        /// <param name="width">Width of the image</param>
        /// <param name="height">Height of the image</param>
        /// <param name="worldSpace">Is the image in world space</param>
        /// <param name="alpha">The alpha value of the image</param>
        /// <param name="shader">The shader that is used to draw</param>
        public UIImageRendererComponent(int width, int height, bool worldSpace, float alpha, ShaderProgram shader) :
            this(
                TextureLoader.ParameterToTexture(width, height), worldSpace,
                alpha, shader)
        {
        }

        /// <summary>
        /// public constructor
        /// </summary>
        /// <param name="texture">The texture that is used to draw</param>
        /// <param name="worldSpace">Is the image in world space</param>
        /// <param name="alpha">The alpha value of the image</param>
        /// <param name="shader">The shader that is used to draw</param>
        public UIImageRendererComponent(Texture texture, bool worldSpace, float alpha, ShaderProgram shader) : base(
            shader, worldSpace, alpha)
        {
            Texture = texture;
        }

        /// <summary>
        /// The image that will be drawn to the screen
        /// </summary>
        public virtual Texture Texture { get; set; }

        /// <summary>
        /// Disposes th Texture used by the context
        /// </summary>
        protected override void OnDestroy()
        {
            Texture?.Dispose();
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


            Program.AddUniformCache("transform");
            Program.AddUniformCache("alpha");
            Program.AddUniformCache("uiTexture");
            Program.AddUniformCache("tiling");
            Program.AddUniformCache("offset");
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
            GL.Disable(EnableCap.DepthTest);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
            Program.Use();

            Matrix4 mat = Matrix4.Identity;

            if (WorldSpace)
            {
                mat = Owner._worldTransformCache * viewMat * projMat;
            }
            else
            {
                mat = Owner._worldTransformCache;
            }

            GL.UniformMatrix4(Program.GetUniformLocation("transform"), false, ref mat);
            GL.Uniform1(Program.GetUniformLocation("alpha"), Alpha);

            GL.Uniform2(Program.GetUniformLocation("tiling"), Tiling);
            GL.Uniform2(Program.GetUniformLocation("offset"), Offset);

            GL.Uniform1(Program.GetUniformLocation("uiTexture"), 0);
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindVertexArray(_vao);
            GL.BindTexture(TextureTarget.Texture2D, Texture.TextureId);

            GL.DrawArrays(PrimitiveType.Triangles, 0, 6);

            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindVertexArray(0);
            GL.BindTexture(TextureTarget.Texture2D, 0);
            GL.Enable(EnableCap.DepthTest);

            GL.Disable(EnableCap.Blend);
        }
    }
}