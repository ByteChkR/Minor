using System;
using MinorEngine.engine.core;
using MinorEngine.engine.rendering.ui;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using SharpFont;

namespace MinorEngine.engine.rendering.contexts
{
    public class TextRenderContext : UIRenderContext
    {
        public string DisplayText { get; set; }
        public GameFont FontFace { get; set; }


        protected static int _vbo;
        protected static int _vao;
        protected static bool _init;

        private static float TabToSpaceCount = 0.1f;


        public TextRenderContext(ShaderProgram program, Vector2 position, Vector2 scale, Matrix4 modelMatrix,
            bool worldSpace, float alpha, GameFont fontFace, string displayText, int renderQueue) : base(position,
            scale, modelMatrix, worldSpace, alpha, program, renderQueue)
        {
            FontFace = fontFace;
            DisplayText = displayText;
        }

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