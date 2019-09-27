
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection.Metadata;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using Common;
using MinorEngine.engine.core;
using MinorEngine.engine.rendering;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using SharpFont;
using Bitmap = System.Drawing.Bitmap;
using PixelFormat = OpenTK.Graphics.OpenGL.PixelFormat;

namespace GameEngine.engine.ui
{

    public struct Character
    {
        public int GlTexture { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public float BearingX { get; set; }
        public float BearingY;
        public float Advance { get; set; }
    }

    public class UITextRendererComponent : UIRendererComponent
    {
        private FontFace ff;
        private Dictionary<char, Character> _fontAtlas = new Dictionary<char, Character>();
        private int vbo, vao;
        private int scrW, scrH;
        public string Text { get; set; } = "HELLO";

        public UITextRendererComponent(string fontPath, int fontSize,  ShaderProgram shader) : base(null, shader)
        {
            ff = new FontFace(File.OpenRead(fontPath));
            int glTex;
            scrW = AbstractGame.Instance.Settings.Width;
            scrH = AbstractGame.Instance.Settings.Height;
            for (int i = 0; i < ushort.MaxValue; i++)
            {
                Glyph g = ff.GetGlyph(new CodePoint(i), fontSize);
                if (g == null) continue;
                this.Log("Creating GL Texture from Character: " + (char)i, DebugChannel.Log);

                byte[] buf = new byte[g.RenderWidth * g.RenderHeight];
                GCHandle handle = GCHandle.Alloc(buf, GCHandleType.Pinned);
                Surface s = new Surface
                {
                    Bits = handle.AddrOfPinnedObject(),
                    Width = g.RenderWidth,
                    Height = g.RenderHeight,
                    Pitch = g.RenderWidth
                };

                g.RenderTo(s);
                if (g.RenderWidth != 0 && g.RenderHeight != 0)
                {
                    Bitmap bmp = new Bitmap(g.RenderWidth, g.RenderHeight);
                    BitmapData data = bmp.LockBits(new System.Drawing.Rectangle(0, 0, g.RenderWidth, g.RenderHeight),
                        ImageLockMode.WriteOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                    byte[] iimgBuf = new byte[buf.Length * 4];
                    for (int j = 0; j < buf.Length; j++)
                    {

                        iimgBuf[j * 4 + 3] = 255;
                        iimgBuf[j * 4 + 1] = buf[j];
                        iimgBuf[j * 4 + 2] = buf[j];
                        iimgBuf[j * 4] = buf[j];
                    }

                    Marshal.Copy(iimgBuf, 0, data.Scan0, iimgBuf.Length);

                    bmp.UnlockBits(data);

                    bmp.RotateFlip(RotateFlipType.RotateNoneFlipY); //Rotating hack

                    data = bmp.LockBits(new System.Drawing.Rectangle(0, 0, g.RenderWidth, g.RenderHeight),
                        ImageLockMode.WriteOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

                    int tex = GL.GenTexture();
                    glTex = tex;
                    GL.BindTexture(TextureTarget.Texture2D, tex);

                    GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, g.RenderWidth, g.RenderHeight,
                        0, PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);
                    GL.TextureParameter(tex, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
                    GL.TextureParameter(tex, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);
                    GL.TextureParameter(tex, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
                    GL.TextureParameter(tex, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

                    bmp.UnlockBits(data);

                    bmp.Save("fontExport/" + i + "_font.png");
                }
                else
                {
                    glTex = -1;
                }
                Character c = new Character
                {
                    GlTexture = glTex,
                    Width = s.Width,
                    Height = s.Height,
                    Advance = g.HorizontalMetrics.Advance,
                    BearingX = g.HorizontalMetrics.Bearing.X,
                    BearingY = g.HorizontalMetrics.Bearing.Y
                };
                _fontAtlas.Add((char)i, c);


            }

            vao = GL.GenVertexArray();
            vbo = GL.GenBuffer();
            GL.BindVertexArray(vao);
            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, sizeof(float) * 6 * 4, IntPtr.Zero, BufferUsageHint.DynamicDraw);

            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, 4 * sizeof(float), IntPtr.Zero);
            GL.EnableVertexAttribArray(1);
            GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, 4 * sizeof(float), 2 * sizeof(float));

            //GL.EnableVertexAttribArray(0);
            //GL.VertexAttribPointer(0, 4, VertexAttribPointerType.Float, false, 4 * sizeof(float), 0);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindVertexArray(0);

        }

        public override void Render(Matrix4 modelMat, Matrix4 viewMat, Matrix4 projMat)
        {
            //base.Render(modelMat, viewMat, projMat);
            //return;
            //base.Render(modelMat, viewMat, projMat);
            if (Shader != null)
            {
                GL.Enable(EnableCap.Blend);
                GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
                Shader.Use();

                Matrix4 trmat = Matrix4.CreateTranslation(Position.X, Position.Y, 0);
                Matrix4 m = trmat;
                
                GL.UniformMatrix4(Shader.GetUniformLocation("transform"), false, ref m);


                GL.Uniform3(Shader.GetUniformLocation("textColor"), 1f, 0f, 0f);
                GL.Uniform1(Shader.GetUniformLocation("sourceTexture"), 0);
                GL.ActiveTexture(TextureUnit.Texture0);
                GL.BindVertexArray(vao);
                float x = Position.X;
                for (int i = 0; i < Text.Length; i++)
                {
                    Character chr = _fontAtlas[Text[i]];

                    float xpos = x + chr.BearingX / scrW * Scale.X;
                    float ypos = Position.Y - (chr.Height - chr.BearingY) / scrH * Scale.Y;

                    float w = chr.Width / (float)scrW * Scale.X;
                    float h = chr.Height / (float)scrH * Scale.Y;
                    float[] verts = {
                         xpos,     ypos + h,    0.0f, 1.0f ,
                         xpos,     ypos,        0.0f, 0.0f,
                         xpos + w, ypos,        1.0f, 0.0f ,

                         xpos,     ypos + h,    0.0f, 1.0f ,
                         xpos + w, ypos,        1.0f, 0.0f,
                         xpos + w, ypos + h,    1.0f, 1.0f
                    };

                    if (chr.GlTexture != -1)
                    {
                        GL.BindTexture(TextureTarget.Texture2D, chr.GlTexture);
                        GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
                        GL.BufferSubData(BufferTarget.ArrayBuffer, IntPtr.Zero, (IntPtr)(sizeof(float) * verts.Length),
                            verts);

                        GL.DrawArrays(PrimitiveType.Triangles, 0, 6);
                        ErrorCode ec = GL.GetError();
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