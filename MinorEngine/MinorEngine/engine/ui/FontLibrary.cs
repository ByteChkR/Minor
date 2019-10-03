using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using MinorEngine.engine.rendering;
using OpenTK.Graphics.OpenGL;
using SharpFont;
using PixelFormat = OpenTK.Graphics.OpenGL.PixelFormat;

namespace MinorEngine.engine.ui
{
    public class FontLibrary
    {
        private readonly Dictionary<string, GameFont> _fonts;


        public FontLibrary(string folderPath)
        {
            _fonts = new Dictionary<string, GameFont>();
            string[] files = Directory.GetFiles(Path.GetFullPath(folderPath), "*.ttf");
            foreach (var file in files)
            {
                LoadFont(file);
            }





        }

        public void LoadFont(string filename)
        {
            LoadFont(filename, 32);
        }
        public void LoadFont(string filename, int pixelSize)
        {

            FontFace ff = new FontFace(File.OpenRead(filename));

            if (_fonts.ContainsKey(ff.FullName))
            {
                return;
            }

            Dictionary<char, Character> fontAtlas = new Dictionary<char, Character>();

            for (int i = 0; i < ushort.MaxValue; i++)
            {
                Glyph g = ff.GetGlyph(new CodePoint(i), pixelSize);
                if (g == null)
                {
                    continue;
                }

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
                GameTexture glTex;
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

                    GameTexture tex = GameTexture.Create(bmp.Width, bmp.Height, false);
                    glTex = tex;
                    GL.BindTexture(TextureTarget.Texture2D, tex.TextureId);

                    GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.R8, g.RenderWidth, g.RenderHeight,
                        0, PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);
                    GL.TextureParameter(tex.TextureId, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
                    GL.TextureParameter(tex.TextureId, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);
                    GL.TextureParameter(tex.TextureId, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
                    GL.TextureParameter(tex.TextureId, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

                    bmp.UnlockBits(data);

                }
                else
                {
                    glTex = null;
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
                fontAtlas.Add((char)i, c);
            }

            GameFont font = new GameFont(ff, pixelSize, fontAtlas);
            _fonts.Add(ff.FullName, font);
        }


        public GameFont GetFont(string name)
        {
            return _fonts[name];
        }
    }
}