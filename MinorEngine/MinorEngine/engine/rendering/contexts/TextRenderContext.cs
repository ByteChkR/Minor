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


        public TextRenderContext(ShaderProgram program, Vector2 position, Vector2 scale, Matrix4 modelMatrix, bool worldSpace, float alpha, GameFont fontFace, string displayText, int renderQueue) : base(position, scale, modelMatrix, worldSpace, alpha, program, renderQueue)
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
            if (!_init) SetUpTextResources();
            int scrW = GameEngine.Instance.Width;
            int scrH = GameEngine.Instance.Height;

            //GL.Disable(EnableCap.Blend);
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
            Program.Use();



            Matrix4 trmat = Matrix4.CreateTranslation(Position.X, Position.Y, 0);
            Matrix4 m = trmat;

            GL.UniformMatrix4(Program.GetUniformLocation("transform"), false, ref m);


            GL.Uniform3(Program.GetUniformLocation("textColor"), 1f, 0f, 0f);
            GL.Uniform1(Program.GetUniformLocation("sourceTexture"), 0);
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindVertexArray(_vao);
            float x = Position.X;
            float y = Position.Y;
            for (int i = 0; i < DisplayText.Length; i++)
            {
                if (DisplayText[i] == '\n')
                {
                    FaceMetrics fm = FontFace.Metrics;
                    x = Position.X;
                    y -= fm.LineHeight / scrH * Scale.Y;
                    continue;
                }


                if (DisplayText[i] == '\t')
                {
                    float len = x - Position.X;
                    float count = TabToSpaceCount - (len % TabToSpaceCount);
                    float val = count;
                    x += val;
                    continue;
                }
                //x-pos.x

                



                if (!FontFace.TryGetCharacter(DisplayText[i], out TextCharacter chr)) FontFace.TryGetCharacter('?', out chr);

                float xpos = x + chr.BearingX / scrW * Scale.X;
                float ypos = y - (chr.Height - chr.BearingY) / scrH * Scale.Y;

                float w = chr.Width / (float)scrW * Scale.X;
                float h = chr.Height / (float)scrH * Scale.Y;

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
                    GL.BufferSubData(BufferTarget.ArrayBuffer, IntPtr.Zero, (IntPtr)(sizeof(float) * verts.Length),
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