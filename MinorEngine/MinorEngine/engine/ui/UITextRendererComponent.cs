
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection.Metadata;
using System.Runtime.InteropServices;
using MinorEngine.engine.rendering;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using SharpFont;

namespace GameEngine.engine.ui
{

    public struct Character
    {
        public int glTexture;
        public int width;
        public int height;
        public float bearingX;
        public float bearingY;
        public float advance;
    }

    public class UITextRendererComponent : UIRendererComponent
    {
        private FontFace ff;
        private Dictionary<char, Character> _fontAtlas = new Dictionary<char, Character>();
        private int vbo, vao;
        private string text = "HELLO";
        public UITextRendererComponent(string fontPath, int width, int height, ShaderProgram shader) : base(width, height, shader)
        {
            ff = new FontFace(File.OpenRead(fontPath));

            for (int i = 0; i < 128; i++)
            {
                Glyph g = ff.GetGlyph(new CodePoint(i), 32);
                if (g == null) continue;
                int tex = GL.GenTexture();
                GL.BindTexture(TextureTarget.Texture2D, tex);
                byte[] buf = new byte[g.RenderWidth*g.RenderHeight];
                GCHandle handle = GCHandle.Alloc(buf, GCHandleType.Pinned);
                Surface s = new Surface
                {
                    Bits = handle.AddrOfPinnedObject(),
                    Width = g.RenderWidth,
                    Height = g.RenderHeight,
                    Pitch = g.RenderWidth
                };

                g.RenderTo(s);

                GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.CompressedRed, g.RenderWidth, g.RenderHeight, 0, PixelFormat.Red, PixelType.Byte, s.Bits);
                GL.TextureParameter(tex, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
                GL.TextureParameter(tex, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);
                GL.TextureParameter(tex, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
                GL.TextureParameter(tex, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

                Character c = new Character
                {
                    glTexture = tex,
                    width = s.Width,
                    height = s.Height,
                    advance = g.HorizontalMetrics.Advance,
                    bearingX = g.HorizontalMetrics.Bearing.X,
                    bearingY = g.HorizontalMetrics.Bearing.Y
                };
                _fontAtlas.Add((char)i, c);


            }

            vao = GL.GenVertexArray();
            vbo = GL.GenBuffer();
            GL.BindVertexArray(vao);
            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, sizeof(float) * 6 * 4, IntPtr.Zero, BufferUsageHint.DynamicDraw);
            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(0, 4, VertexAttribPointerType.Float, false, 4 * sizeof(float), 0);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindVertexArray(0);

        }

        public override void Render(Matrix4 modelMat, Matrix4 viewMat, Matrix4 projMat)
        {
            //base.Render(modelMat, viewMat, projMat);
            //return;
            
            if (Shader != null)
            {
                GL.Enable(EnableCap.Blend);
                GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
                Shader.Use();

                Matrix4 trmat = Matrix4.CreateTranslation(Position.X, Position.Y, 0);
                Matrix4 scmat = Matrix4.CreateScale(Scale.X, Scale.Y, 1);
                Matrix4 m = trmat * scmat;

                GL.UniformMatrix4(Shader.GetUniformLocation("transform"), false, ref m);


                GL.Uniform3(Shader.GetUniformLocation("textColor"), 1f, 0f, 0f);
                GL.Uniform1(Shader.GetUniformLocation("sourceTexture"), 0);
                GL.ActiveTexture(TextureUnit.Texture0);
                GL.BindVertexArray(vao);

                for (int i = 0; i < text.Length; i++)
                {
                    Character chr = _fontAtlas[text[i]];

                    float xpos = Position.X + chr.bearingX * Scale.X;
                    float ypos = Position.Y - (chr.width - chr.bearingY) * Scale.Y;

                    float w = chr.width * Scale.X;
                    float h = chr.width * Scale.Y;
                    float[] verts = {
                         xpos,     ypos + h,   0.0f, 0.0f ,
                         xpos,     ypos,       0.0f, 1.0f ,
                         xpos + w, ypos,       1.0f, 1.0f ,

                         xpos,     ypos + h,   0.0f, 0.0f ,
                         xpos + w, ypos,       1.0f, 1.0f,
                         xpos + w, ypos + h,   1.0f, 0.0f
                    };

                    GL.BindTexture(TextureTarget.Texture2D, chr.glTexture);
                    GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
                    GL.BufferSubData(BufferTarget.ArrayBuffer, IntPtr.Zero, (IntPtr)(sizeof(float) * verts.Length), verts);
                    
                    GL.DrawArrays(PrimitiveType.Triangles, 0, 6);



                }
                GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
                GL.BindVertexArray(0);
                GL.BindTexture(TextureTarget.Texture2D, 0);

                GL.Disable(EnableCap.Blend);

            }
        }
    }
}