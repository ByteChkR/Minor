using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using Assimp;
using Common;
using OpenTK.Graphics.OpenGL;
using TKPixelFormat = OpenTK.Graphics.OpenGL.PixelFormat;
using SYSPixelFormat = System.Drawing.Imaging.PixelFormat;
using TextureWrapMode = OpenTK.Graphics.OpenGL.TextureWrapMode;

namespace GameEngine.engine.rendering
{
    public class GameTexture
    {

        public int TextureId;
        public TextureType TexType;
        public string Path;
        private GameTexture()
        {
            TexType = TextureType.Diffuse;
            GL.GenTextures(1, out TextureId);
        }



        public static GameTexture[] LoadTextures(Scene scene)
        {
            if (!scene.HasTextures) return new GameTexture[0];
            List<GameTexture> list = new List<GameTexture>();
            foreach (var x in scene.Textures) list.Add(ConvertToTexture(x));

            return list.ToArray();
        }

        private static byte[] flattenImageData(Texel[] imageData)
        {
            byte[] ret = new byte[imageData.Length * 4];
            for (int i = 0; i < imageData.Length; i++)
            {
                TexelToByteSequence(i * 4, ret, imageData[i]);
            }

            return ret;
        }

        private static void TexelToByteSequence(int startidx, byte[] arr, Texel txl)
        {
            arr[startidx] = txl.R;
            arr[startidx + 1] = txl.G;
            arr[startidx + 2] = txl.B;
            arr[startidx + 3] = txl.A;

        }

        public static GameTexture ConvertToTexture(EmbeddedTexture tex)
        {

            tex.Log($"Loading Texture... Width: {tex.Width} Height: {tex.Height}", DebugChannel.Log);
            GameTexture ret = new GameTexture();

            GL.BindTexture(TextureTarget.Texture2D, ret.TextureId);

            byte[] flatBytes = flattenImageData(tex.NonCompressedData);

            GCHandle handle = GCHandle.Alloc(flatBytes, GCHandleType.Pinned);
            IntPtr ptr = handle.AddrOfPinnedObject();


            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, tex.Width, tex.Height, 0, OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, ptr);

            handle.Free();

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);

            return ret;

        }


        public static GameTexture Load(string filename)
        {
            Bitmap bmp = new Bitmap(filename);

            bmp.Log($"Loading Texture: " + filename, DebugChannel.Log);
            bmp.RotateFlip(RotateFlipType.RotateNoneFlipY);

            BitmapData data = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadOnly,
                SYSPixelFormat.Format32bppArgb);

            GameTexture ret = new GameTexture();
            GL.BindTexture(TextureTarget.Texture2D, ret.TextureId);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, data.Width, data.Height, 0, OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);
            bmp.UnlockBits(data);


            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);

            return ret;

        }

        public static void Update(GameTexture tex, byte[] buffer, int width, int height)
        {
            GL.BindTexture(TextureTarget.Texture2D, tex.TextureId);
            GL.TexSubImage2D(TextureTarget.Texture2D, 0, 0, 0, (int)width, (int)height, OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, buffer);

        }

        public static GameTexture Load(byte[] buffer, int width, int height)
        {
            GameTexture ret = new GameTexture();
            ret.Log($"Loading Texture... Width: {width} Height: {height}", DebugChannel.Log);

            GL.BindTexture(TextureTarget.Texture2D, ret.TextureId);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, width, height, 0, TKPixelFormat.Bgra, PixelType.UnsignedByte, buffer);


            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);

            return ret;
        }
    }
}