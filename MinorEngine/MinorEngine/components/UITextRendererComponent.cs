using System;
using Common;
using MinorEngine.debug;
using MinorEngine.engine.core;
using MinorEngine.engine.rendering;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using SharpFont;

namespace MinorEngine.engine.ui
{
    public class Character
    {
        public GameTexture GlTexture { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public float BearingX { get; set; }
        public float BearingY { get; set; }
        public float Advance { get; set; }
    }

    public class UITextRendererComponent : UIRendererComponent
    {
        public override Renderer.RenderContext Context
        {
            get
            {
                return new Renderer.TextRenderContext(Shader, Position, Scale, font, Text);
            }
        }

        private readonly GameFont font;
        private readonly int _vbo;
        private readonly int _vao;
        public string Text { get; set; } = "HELLO";

        public UITextRendererComponent(string fontName, ShaderProgram shader) : base(null, shader)
        {
            font = UIHelper.Instance.FontLibrary.GetFont("Arial");

            Logger.Log("Reading Character Glyphs from " + fontName, DebugChannel.Log);

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

        public override void Render(Matrix4 modelMat, Matrix4 viewMat, Matrix4 projMat)
        {
            if (Shader != null)
            {
                int scrW = GameEngine.Instance.Width;
                int scrH = GameEngine.Instance.Height;

                GL.Enable(EnableCap.Blend);
                GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
                Shader.Use();

                Matrix4 trmat = Matrix4.CreateTranslation(Position.X, Position.Y, 0);
                Matrix4 m = trmat;

                GL.UniformMatrix4(Shader.GetUniformLocation("transform"), false, ref m);


                GL.Uniform3(Shader.GetUniformLocation("textColor"), 1f, 0f, 0f);
                GL.Uniform1(Shader.GetUniformLocation("sourceTexture"), 0);
                GL.ActiveTexture(TextureUnit.Texture0);
                GL.BindVertexArray(_vao);
                float x = Position.X;
                float y = Position.Y;
                for (int i = 0; i < Text.Length; i++)
                {
                    if (Text[i] == '\n')
                    {
                        FaceMetrics fm = font.Metrics;
                        x = Position.X;
                        y -= fm.LineHeight / scrH * Scale.Y;
                        continue;
                    }

                    if (!font.TryGetCharacter(Text[i], out Character chr)) font.TryGetCharacter('?', out chr);

                    float xpos = x + chr.BearingX / scrW * Scale.X;
                    float ypos = y - (chr.Height - chr.BearingY) / scrH * Scale.Y;

                    float w = chr.Width / (float) scrW * Scale.X;
                    float h = chr.Height / (float) scrH * Scale.Y;
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
}