using System;
using Engine.Core;
using Engine.DataTypes;
using Engine.Rendering;
using Engine.UI;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using SharpFont;

namespace Engine.UI
{
    /// <summary>
    /// Implements a text rendering context
    /// </summary>
    public class TextRenderContext : UIRenderContext
    {
        /// <summary>
        /// The Text to display
        /// </summary>
        public string DisplayText { get; set; }

        /// <summary>
        /// The font that is used
        /// </summary>
        public GameFont FontFace { get; set; }

        /// <summary>
        /// The VBO of the Quad used to draw each character
        /// </summary>
        protected static int _vbo;
        /// <summary>
        /// The VAO of the Quad used to draw each character
        /// </summary>
        protected static int _vao;

        /// <summary>
        /// Initialization flag
        /// </summary>
        protected static bool _init;


        /// <summary>
        /// The Length of a single \t in UV coordinates
        /// </summary>
        private static float TabToSpaceCount = 0.1f;

        /// <summary>
        /// Public Constructor
        /// </summary>
        /// <param name="program">The Program to be used</param>
        /// <param name="position">Position of the Text in Screen Space</param>
        /// <param name="scale">The Scale of the Text</param>
        /// <param name="modelMatrix">The Model Matrix</param>
        /// <param name="worldSpace">A flag if the Text is positioned in world space</param>
        /// <param name="alpha">Alpha value of the text</param>
        /// <param name="fontFace">The Font That is used</param>
        /// <param name="displayText">The Initial Display Text</param>
        /// <param name="renderQueue">The Render queue</param>
        public TextRenderContext(ShaderProgram program, Vector2 position, Vector2 scale, Matrix4 modelMatrix,
            bool worldSpace, float alpha, GameFont fontFace, string displayText, int renderQueue) : base(position,
            scale, modelMatrix, worldSpace, alpha, program, renderQueue)
        {
            FontFace = fontFace;
            DisplayText = displayText;
        }

        /// <summary>
        /// Initialization Method that sets up the Screen Quad buffers
        /// </summary>
        private static void SetUpTextResources()
        {
            _init = true;
            _vao = GL.GenVertexArray();
            _vbo = GL.GenBuffer();
            GL.BindVertexArray(_vao);
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, sizeof(float) * 6 * 4, IntPtr.Zero, BufferUsageHint.DynamicDraw);

            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, 4 * sizeof(float), IntPtr.Zero);
            GL.EnableVertexAttribArray(1);
            GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, 4 * sizeof(float), 2 * sizeof(float));

            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindVertexArray(0);
        }

        /// <summary>
        /// Renders the Text in Display Text to the screen
        /// </summary>
        /// <param name="viewMat">View matrix of the camera(unused)</param>
        /// <param name="projMat">View matrix of the camera(unused)</param>
        public override void Render(Matrix4 viewMat, Matrix4 projMat)
        {
            if (!_init)
            {
                SetUpTextResources();
            }

            var scrW = GameEngine.Instance.Width;
            var scrH = GameEngine.Instance.Height;

            //GL.Disable(EnableCap.Blend);
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
            Program.Use();


            var trmat = Matrix4.CreateTranslation(Position.X, Position.Y, 0);
            var m = trmat;

            GL.UniformMatrix4(Program.GetUniformLocation("transform"), false, ref m);


            GL.Uniform3(Program.GetUniformLocation("textColor"), 1f, 0f, 0f);
            GL.Uniform1(Program.GetUniformLocation("sourceTexture"), 0);
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindVertexArray(_vao);
            var x = Position.X;
            var y = Position.Y;
            for (var i = 0; i < DisplayText.Length; i++)
            {
                if (DisplayText[i] == '\n')
                {
                    var fm = FontFace.Metrics;
                    x = Position.X;
                    y -= fm.LineHeight / scrH * Scale.Y;
                    continue;
                }


                if (DisplayText[i] == '\t')
                {
                    var len = x - Position.X;
                    var count = TabToSpaceCount - len % TabToSpaceCount;
                    var val = count;
                    x += val;
                    continue;
                }
                //x-pos.x


                if (!FontFace.TryGetCharacter(DisplayText[i], out var chr))
                {
                    FontFace.TryGetCharacter('?', out chr);
                }

                var xpos = x + chr.BearingX / scrW * Scale.X;
                var ypos = y - (chr.Height - chr.BearingY) / scrH * Scale.Y;

                var w = chr.Width / (float) scrW * Scale.X;
                var h = chr.Height / (float) scrH * Scale.Y;

                //Remove Scale And initial position(start at (x,y) = 0)
                //Add Translation to Make text be centered at origin(-TotalTextWidth/2,-TotalTextHeight/2)
                //Multiply Verts by matrix or pass it in shader

                float[] verts =
                {
                    xpos, ypos + h, 0.0f, 1.0f,
                    xpos, ypos, 0.0f, 0.0f,
                    xpos + w, ypos, 1.0f, 0.0f,

                    xpos, ypos + h, 0.0f, 1.0f,
                    xpos + w, ypos, 1.0f, 0.0f,
                    xpos + w, ypos + h, 1.0f, 1.0f
                };

                if (chr.GlTexture != null)
                {
                    GL.BindTexture(TextureTarget.Texture2D, chr.GlTexture.TextureId);
                    GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
                    GL.BufferSubData(BufferTarget.ArrayBuffer, IntPtr.Zero, (IntPtr) (sizeof(float) * verts.Length),
                        verts);

                    GL.DrawArrays(PrimitiveType.Triangles, 0, 6);
                }

                x += chr.Advance / scrW * Scale.X;
            }

            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindVertexArray(0);
            GL.BindTexture(TextureTarget.Texture2D, 0);

            GL.Disable(EnableCap.Blend);
        }
    }
}