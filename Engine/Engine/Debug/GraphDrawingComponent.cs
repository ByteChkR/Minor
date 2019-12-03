using System;
using Engine.Rendering;
using Engine.UI;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace Engine.Debug
{
    public class GraphLayout
    {
        public string Key;
        public Vector2 Maximum;
        public int MaxPoints;
        public Vector2 Minimum;
    }

    /// <summary>
    /// Graph Drawing Component
    /// </summary>
    public class GraphDrawingComponent : UiElement
    {
        /// <summary>
        /// A flag that indicates if we need to push the points to the gpu
        /// </summary>
        private bool bufferDirty = true;
        

        /// <summary>
        /// Flag that is used to initialize things on creation
        /// </summary>
        private bool init;

        /// <summary>
        /// Backing field of the graph data
        /// </summary>
        private Vector2[] points;

        /// <summary>
        /// The VBO of the screen quad
        /// </summary>
        private int vbo, vao;

        /// <summary>
        /// Public constructor
        /// </summary>
        /// <param name="shader">The shader to be used</param>
        /// <param name="worldSpace">flag if the graph is in world space</param>
        /// <param name="alpha">the alpha value of the graph</param>
        public GraphDrawingComponent(ShaderProgram shader, bool worldSpace, float alpha) :
            base(shader, worldSpace, alpha)
        {
            points = new Vector2[0];
        }


        /// <summary>
        /// the point data that will get drawn
        /// </summary>
        public Vector2[] Points
        {
            get => points;
            set
            {
                bufferDirty = true;
                if (Owner != null)
                {
                    points = ComputeUvPos(Position, Scale, value);
                }
            }
        }

        /// <summary>
        /// Flag that enables or disables the graph rendering
        /// </summary>
        public bool Enabled { get; set; } = true;

        /// <summary>
        /// Computes the UVs of the Quad that will contain the graph data
        /// </summary>
        /// <returns></returns>
        private static Vector2[] ComputeUvPos(Vector2 position, Vector2 scale, Vector2[] points)
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
        /// Initializer of the Screen quad
        /// </summary>
        private void Initialize()
        {
            init = true;
            vao = GL.GenVertexArray();
            vbo = GL.GenBuffer();
            GL.BindVertexArray(vao);
            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);


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
            GL.BindVertexArray(vao);
            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
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

            if (!init)
            {
                Initialize();
            }

            if (bufferDirty)
            {
                UpdateBuffer();
            }

            GL.Enable(EnableCap.Blend);
            GL.Disable(EnableCap.DepthTest);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

            float red = 1, green = 0, blue = 0;

            Program.Use();

            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);

            Matrix4 mat = Matrix4.Identity;

            if (WorldSpace)
            {
                mat = Owner.WorldTransformCache * viewMat * projMat;
            }
            else
            {
                mat = Owner.WorldTransformCache;
            }

            GL.UniformMatrix4(Program.GetUniformLocation("transform"), false, ref mat);
            GL.Uniform4(Program.GetUniformLocation("lineColor"), new Vector4(red, green, blue, 1));

            GL.BindVertexArray(vao);

            GL.DrawArrays(PrimitiveType.LineStrip, 0, Points.Length);

            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindVertexArray(0);
            GL.BindTexture(TextureTarget.Texture2D, 0);


            GL.Disable(EnableCap.Blend);
            GL.Enable(EnableCap.DepthTest);
        }
    }
}