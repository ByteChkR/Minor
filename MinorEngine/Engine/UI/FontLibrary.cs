using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using Engine.DataTypes;
using Engine.Debug;
using Engine.IO;
using OpenTK.Graphics.OpenGL;
using SharpFont;
using PixelFormat = OpenTK.Graphics.OpenGL.PixelFormat;

namespace Engine.UI
{
    /// <summary>
    /// Font Library. Is Used to store the loaded game Fonts
    /// </summary>
    public class FontLibrary
    {
        /// <summary>
        /// Table of game fonts
        /// </summary>
        private readonly Dictionary<string, Tuple<string, GameFont>> _fonts;


        /// <summary>
        /// Public constructor that is loading every font in the specified folder
        /// </summary>
        /// <param name="folderPath">the specified folder</param>
        public FontLibrary(string folderPath)
        {
            _fonts = new Dictionary<string, Tuple<string, GameFont>>();
            string[] files = Directory.GetFiles(Path.GetFullPath(folderPath), "*.ttf");
            foreach (string file in files)
            {
                LoadFont(file);
            }
        }

        /// <summary>
        /// Manually loads a font by filename with default pixel size 32
        /// </summary>
        /// <param name="filename">the file name</param>
        public void LoadFont(string filename)
        {
            LoadFont(filename, 32);
        }

        public void LoadFont(string filename, int pixelSize)
        {
            if (File.Exists(filename))
            {
                LoadFont(File.OpenRead(filename), pixelSize, filename);
            }
            else
            {
                Logger.Log("Could not find Font: " + filename, DebugChannel.Error, 10);
            }
        }

        internal static GameFont LoadFontInternal(Stream fileStream, int pixelSize, out string fontName)
        {
            FontFace ff = new FontFace(fileStream);

            fontName = ff.FullName;

            Dictionary<char, TextCharacter> fontAtlas = new Dictionary<char, TextCharacter>();

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
                Texture glTex;
                if (g.RenderWidth != 0 && g.RenderHeight != 0)
                {
                    Bitmap bmp = new Bitmap(g.RenderWidth, g.RenderHeight);
                    BitmapData data = bmp.LockBits(new Rectangle(0, 0, g.RenderWidth, g.RenderHeight),
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

                    data = bmp.LockBits(new Rectangle(0, 0, g.RenderWidth, g.RenderHeight),
                        ImageLockMode.WriteOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

                    Texture tex = TextureLoader.ParameterToTexture(bmp.Width, bmp.Height);
                    glTex = tex;
                    GL.BindTexture(TextureTarget.Texture2D, tex.TextureId);

                    GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.R8, g.RenderWidth, g.RenderHeight,
                        0, PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);
                    GL.TextureParameter(tex.TextureId, TextureParameterName.TextureWrapS,
                        (int) TextureWrapMode.ClampToEdge);
                    GL.TextureParameter(tex.TextureId, TextureParameterName.TextureWrapT,
                        (int) TextureWrapMode.ClampToEdge);
                    GL.TextureParameter(tex.TextureId, TextureParameterName.TextureMinFilter,
                        (int) TextureMinFilter.Linear);
                    GL.TextureParameter(tex.TextureId, TextureParameterName.TextureMagFilter,
                        (int) TextureMagFilter.Linear);

                    bmp.UnlockBits(data);
                }
                else
                {
                    glTex = null;
                }

                TextCharacter c = new TextCharacter
                {
                    GlTexture = glTex,
                    Width = s.Width,
                    Height = s.Height,
                    Advance = g.HorizontalMetrics.Advance,
                    BearingX = g.HorizontalMetrics.Bearing.X,
                    BearingY = g.HorizontalMetrics.Bearing.Y
                };
                fontAtlas.Add((char) i, c);
            }

            GameFont font = new GameFont(ff, pixelSize, fontAtlas);
            return font;
        }

        /// <summary>
        /// Loads a font by filename and pixel size
        /// </summary>
        /// <param name="filename">the filename</param>
        /// <param name="pixelSize">The size of the font in pixels</param>
        private void LoadFont(Stream fileStream, int pixelSize, string path)
        {
            if (_fonts.ContainsKey(path))
            {
                return;
            }

            GameFont font = LoadFontInternal(fileStream, pixelSize, out string name);

            _fonts.Add(path, new Tuple<string, GameFont>(name, font));
        }

        /// <summary>
        /// returns a font by name
        /// </summary>
        /// <param name="name">The name of the font</param>
        /// <returns>The font with the specified name</returns>
        public GameFont GetFont(string name)
        {
            foreach (KeyValuePair<string, Tuple<string, GameFont>> font in _fonts)
            {
                if (name == font.Value.Item1)
                {
                    return font.Value.Item2;
                }
            }

            Logger.Log("Could not Find font with name: " + name, DebugChannel.Log);

            return GameFont.DefaultFont;
        }

        /// <summary>
        /// returns a font by name
        /// </summary>
        /// <param name="name">The name of the font</param>
        /// <returns>The font with the specified name</returns>
        public GameFont GetFontByPath(string path)
        {
            if (!_fonts.ContainsKey(path))
            {
                Logger.Log("Could not Find font with Path: " + path, DebugChannel.Log);
                return GameFont.DefaultFont;
            }

            return _fonts[path].Item2;
        }
    }
}