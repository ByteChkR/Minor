using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using Assimp;
using Engine.DataTypes;
using Engine.OpenCL;
using OpenCl.DotNetCore.Memory;
using OpenTK.Graphics.OpenGL;
using PixelFormat = OpenTK.Graphics.OpenGL.PixelFormat;
using TextureWrapMode = OpenTK.Graphics.OpenGL.TextureWrapMode;

namespace Engine.IO
{
    public class TextureLoader
    {
        public static Texture BytesToTexture(IntPtr ptr, int width, int height)
        {
            int texID = GL.GenTexture();

            GL.BindTexture(TextureTarget.Texture2D, texID);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, width, height, 0, PixelFormat.Bgra,
                PixelType.UnsignedByte, ptr);


            DefaultTexParameter();
            GL.BindTexture(TextureTarget.Texture2D, 0);
            return new Texture(texID); ;
        }

        public static Texture BytesToTexture(byte[] buffer, int width, int height)
        {
            var handle = GCHandle.Alloc(buffer, GCHandleType.Pinned);
            var ret = BytesToTexture(handle.AddrOfPinnedObject(), width, height);

            handle.Free();
            return ret;
        }

        public static Texture ParameterToTexture(int width, int height)
        {
            return BytesToTexture(IntPtr.Zero, width, height);
        }

        private static byte[] TextureToByteArray(Texture tex)
        {
            var buffer = new byte[(int)(tex.Width * tex.Height * 4)];
            var handle = GCHandle.Alloc(buffer, GCHandleType.Pinned);
            GL.BindTexture(TextureTarget.Texture2D, tex.TextureId);

            GL.GetTextureSubImage(tex.TextureId, 0, 0, 0, 0, (int)tex.Width, (int)tex.Height, 1, PixelFormat.Bgra,
                PixelType.UnsignedByte, buffer.Length, handle.AddrOfPinnedObject());

            handle.Free();
            return buffer;
        }

        public static CLBuffer TextureToMemoryBuffer(Texture tex)
        {
            var buffer = TextureToByteArray(tex);
            return CLAPI.CreateBuffer(buffer, MemoryFlag.CopyHostPointer | MemoryFlag.ReadWrite);
        }

        public static void Update(Texture tex, byte[] data, int width, int height)
        {
            GL.BindTexture(TextureTarget.Texture2D, tex.TextureId);
            GL.TexSubImage2D(TextureTarget.Texture2D, 0, 0, 0, width, height, PixelFormat.Bgra,
                PixelType.UnsignedByte, data);
            GL.BindTexture(TextureTarget.Texture2D, 0);
        }

        public static Texture BitmapToTexture(Bitmap bmp)
        {
            var data = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadOnly,
                System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            var tex = BytesToTexture(data.Scan0, bmp.Width, bmp.Height);

            bmp.UnlockBits(data);
            return tex;
        }


        private static Texture[] AssimpSceneToTextures(Scene scene)
        {
            if (!scene.HasTextures)
            {
                return new Texture[0];
            }

            var list = new List<Texture>();
            foreach (var x in scene.Textures)
            {
                list.Add(AssimpEmbeddedToTexture(x));
            }

            return list.ToArray();
        }

        private static byte[] flattenImageData(Texel[] imageData)
        {
            var ret = new byte[imageData.Length * 4];
            for (var i = 0; i < imageData.Length; i++)
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

        //Maybe create key for textures loaded with assimp and then use them as cache name
        public static Texture AssimpEmbeddedToTexture(EmbeddedTexture tex)
        {
            return BytesToTexture(flattenImageData(tex.NonCompressedData), tex.Width, tex.Height);
        }

        public static Texture FileToTexture(string file)
        {
            //if (IsContained(file)) return KeyToTexture(file);
            return BitmapToTexture(new Bitmap(file));
        }

        private static void DefaultTexParameter()
        {
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter,
                (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter,
                (int)TextureMagFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS,
                (int)TextureWrapMode.Repeat);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT,
                (int)TextureWrapMode.Repeat);
        }

        private static Texture Copy(Texture other)
        {
            return BytesToTexture(TextureToByteArray(other), (int)other.Width, (int)other.Height);
        }
    }
}