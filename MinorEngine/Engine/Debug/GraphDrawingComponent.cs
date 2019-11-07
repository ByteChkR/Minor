using System;
using Engine.Core;
using Engine.Rendering;
using Engine.UI;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace Engine.Debug
{
    /// <summary>
    /// Graph Drawing Component
    /// </summary>
    public class GraphDrawingComponent : UIElement
    {
        /// <summary>
        /// Backing field of the graph data
        /// </summary>
        private Vector2[] _points;

        /// <summary>
        /// Computes the UVs of the Quad that will contain the graph data
        /// </summary>
        /// <returns></returns>
        private static Vector2[] ComputeUVPos(Vector2 position, Vector2 scale, Vector2[] points)
        {
            Vector2[] ret = new Vector2[points.Length];
            float max = 16f;
            for (int i = 0; i < points.Length; i++)
            {
                max = MathF.Max(points[i].Y, max);
            }

            Vector2 scal = new Vector2(scale.X, scale.Y / max);
            for (int i = 0; i < points.Length; i++)
            {
                ret[i] = (position + points[i] - Vector2.One * 0.5f) * 2 * scal;
            }

            return ret;
        }


        /// <summary>
        /// the point data that will get drawn
        /// </summary>
        public Vector2[] Points
        {
            get => _points;
            set
            {
                _bufferDirty = true;
                if (Owner != null)
                    _points = ComputeUVPos(Position, Scale, value);
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
        /// Public constructor
        /// </summary>
        /// <param name="shader">The shader to be used</param>
        /// <param name="worldSpace">flag if the graph is in world space</param>
        /// <param name="alpha">the alpha value of the graph</param>
        public GraphDrawingComponent(ShaderProgram shader, bool worldSpace, float alpha) :
            base(shader, worldSpace, alpha)
        {
            _points = new Vector2[0];
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


            Program.AddUniformCache("transform");
            Program.AddUniformCache("lineColor");
        }

        /// <summary>
        /// Copies the point data to the gpu
        /// </summary>
        private void UpdateBuffer()
        {
            GL.BindVertexArray(_vao);
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(Points.Length * sizeof(float) * 2),
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
                mat = Owner._worldTransformCache * viewMat * projMat;
            }
            else
            {
                mat = Owner._worldTransformCache;
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