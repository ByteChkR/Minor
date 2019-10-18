using System;
using Engine.Rendering;
using Engine.UI;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace Engine.Debug
{
    /// <summary>
    /// The Graph Drawing Context that uses GL.Lines
    /// </summary>
    public class GraphDrawingContext : UIRenderContext
    {
        /// <summary>
        /// Backing field of the point data
        /// </summary>
        private Vector2[] _points;

        /// <summary>
        /// the point data that will get drawn
        /// </summary>
        public Vector2[] Points
        {
            get => _points;
            set
            {
                _bufferDirty = true;
                _points = value;
            }
        }

        /// <summary>
        /// The VBO of the screen quad
        /// </summary>
        private int _vbo, _vao;

        /// <summary>
        /// A flag that indicates if we need to push the points to the gpu
        /// </summary>
        private bool _bufferDirty = true;

        /// <summary>
        /// Flag that is used to initialize things on creation
        /// </summary>
        private bool _init = false;

        /// <summary>
        /// Flag that enables or disables the graph rendering
        /// </summary>
        public bool Enabled { get; set; } = true;

        /// <summary>
        /// Public Constructor
        /// </summary>
        /// <param name="points">the points to be drawn to the screen</param>
        /// <param name="position">Position in screen space</param>
        /// <param name="scale">The Scale of the Image</param>
        /// <param name="modelMatrix">Model matrix</param>
        /// <param name="worldSpace">Is the Object in world space</param>
        /// <param name="alpha">Alpha value of the image</param>
        /// <param name="program">The shader to be used</param>
        /// <param name="renderQueue">The Render queue</param>
        public GraphDrawingContext(Vector2[] points, Vector2 position, Vector2 scale, Matrix4 modelMatrix,
            bool worldSpace, float alpha,
            ShaderProgram program, int renderQueue) : base(position, scale, modelMatrix, worldSpace, alpha, program,
            renderQueue)
        {
            Points = points;
        }

        /// <summary>
        /// Initializer of the Screen quad
        /// </summary>
        private void Initialize()
        {
            _init = true;
            _vao = GL.GenVertexArray();
            _vbo = GL.GenBuffer();
            GL.BindVertexArray(_vao);
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);


            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, 2 * sizeof(float), IntPtr.Zero);

            GL.BindVertexArray(0);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        }

        /// <summary>
        /// Copies the point data to the gpu
        /// </summary>
        private void UpdateBuffer()
        {
            GL.BindVertexArray(_vao);
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr) (Points.Length * sizeof(float) * 2),
                Points, BufferUsageHint.StaticDraw);
            GL.BindVertexArray(0);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        }

        /// <summary>
        /// Renders the Graph
        /// </summary>
        /// <param name="viewMat">View matrix</param>
        /// <param name="projMat">Project Matrix</param>
        public override void Render(Matrix4 viewMat, Matrix4 projMat)
        {
            if (!Enabled)
            {
                return;
            }

            if (!_init)
            {
                Initialize();
            }

            if (_bufferDirty)
            {
                UpdateBuffer();
            }

            GL.Enable(EnableCap.Blend);
            GL.Disable(EnableCap.DepthTest);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

            float red = 1, green = 0, blue = 0;

            Program.Use();

            GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);

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
            GL.Uniform4(Program.GetUniformLocation("lineColor"), new Vector4(red, green, blue, 1));

            GL.BindVertexArray(_vao);

            GL.DrawArrays(PrimitiveType.LineStrip, 0, Points.Length);

            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindVertexArray(0);
            GL.BindTexture(TextureTarget.Texture2D, 0);


            GL.Disable(EnableCap.Blend);
            GL.Enable(EnableCap.DepthTest);
        }
    }
}