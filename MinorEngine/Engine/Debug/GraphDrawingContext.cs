using System;
using Engine.Rendering;
using Engine.UI;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace Engine.Debug
{
    public class GraphDrawingContext : UIRenderContext
    {
        private Vector2[] _points;
        public Vector2[] Points
        {
            get => _points;
            set
            {
                _bufferDirty = true;
                _points = value;
            }
        }

        private int _vbo, _vao;
        private bool _bufferDirty = true;
        private bool _init = false;
        public bool Enabled { get; set; } = true;

        public GraphDrawingContext(Vector2[] points, Vector2 position, Vector2 scale, Matrix4 modelMatrix, bool worldSpace, float alpha,
            ShaderProgram program, int renderQueue) : base(position, scale, modelMatrix, worldSpace, alpha, program,
            renderQueue)
        {
            Points = points;
        }

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

        private void UpdateBuffer()
        {
            GL.BindVertexArray(_vao);
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(Points.Length * sizeof(float) * 2),
                Points, BufferUsageHint.StaticDraw);
            GL.BindVertexArray(0);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        }

        public override void Render(Matrix4 viewMat, Matrix4 projMat)
        {
            if (!Enabled) return;
            if (!_init)
            {
                Initialize();
            }
            if (_bufferDirty)
            {
                UpdateBuffer();
            }

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




        }
    }
}