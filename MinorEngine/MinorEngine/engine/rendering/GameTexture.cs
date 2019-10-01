using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using Assimp;
using CLHelperLibrary;
using Common;
using GameEngine.engine.core;
using OpenCl.DotNetCore.Memory;
using OpenTK.Graphics.OpenGL;
using TKPixelFormat = OpenTK.Graphics.OpenGL.PixelFormat;
using SYSPixelFormat = System.Drawing.Imaging.PixelFormat;
using TextureWrapMode = OpenTK.Graphics.OpenGL.TextureWrapMode;

namespace GameEngine.engine.rendering
{
    public class GameTexture: IDestroyable
    {
        public int TextureId { get; }
        public TextureType TexType { get; set; }
        public string Path { get; set; }

        public float Width
        {
            get
            {
                GL.BindTexture(TextureTarget.Texture2D, TextureId);
                GL.GetTexLevelParameter(TextureTarget.Texture2D, 0, GetTextureParameter.TextureWidth, out float width);
                return width;
            }
        }
        public float Height
        {
            get
            {
                GL.BindTexture(TextureTarget.Texture2D, TextureId);
                GL.GetTexLevelParameter(TextureTarget.Texture2D, 0, GetTextureParameter.TextureHeight, out float height);
                return height;
            }
        }
        

        private GameTexture()
        {
            TexType = TextureType.Diffuse;
            int id;
            GL.GenTextures(1, out id);
            TextureId = id;
        }

        ~GameTexture()
        {
            
        }

        public void Destroy()
        {
            GL.DeleteTexture(TextureId);
        }

        private static int texCreateCount = 0;
        public static GameTexture Create(int width, int height, bool cache = true)
        {
            GameTexture ret = new GameTexture();

            
            
            GL.BindTexture(TextureTarget.Texture2D, ret.TextureId);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba,
                width, height, 0, TKPixelFormat.Bgra,
                PixelType.UnsignedByte, IntPtr.Zero);
            GL.BindTexture(TextureTarget.Texture2D, 0);

            if (cache)
            {
                string n = "CreatedTexture_" + texCreateCount + "_" + width + "_" + height;
                texCreateCount++;
                TextureProvider.Add(ret, n);

            }
            return ret;
        }

        public static GameTexture[] LoadTextures(Scene scene)
        {
            if (!scene.HasTextures)
            {
                return new GameTexture[0];
            }
            List<GameTexture> list = new List<GameTexture>();
            foreach (var x in scene.Textures)
            {
                list.Add(ConvertToTexture(x));
            }

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

        private static int texAssimpCount = 0;
        public static GameTexture ConvertToTexture(EmbeddedTexture tex)
        {
            string n = "AssimpEmbeddedTexture" + texAssimpCount + "_" + tex.Width + "_" + tex.Height;
            texAssimpCount++;
            tex.Log($"Loading Texture... Width: {tex.Width} Height: {tex.Height}", DebugChannel.Log);
            GameTexture ret = new GameTexture();

            GL.BindTexture(TextureTarget.Texture2D, ret.TextureId);

            byte[] flatBytes = flattenImageData(tex.NonCompressedData);

            GCHandle handle = GCHandle.Alloc(flatBytes, GCHandleType.Pinned);
            IntPtr ptr = handle.AddrOfPinnedObject();


            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, tex.Width, tex.Height, 0, OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, ptr);
            handle.Free();
            DefaultTexParameter();
            TextureProvider.Add(ret,n);
            return ret;

        }

        /// <summary>
        /// Creates a buffer with the content of an image and the specified Memory Flags
        /// </summary>
        /// <param name="bmp">The image that holds the data</param>
        /// <param name="flags">The memory flags for the buffer creation</param>
        /// <returns></returns>
        public static MemoryBuffer CreateFromTexture(GameTexture tex, MemoryFlag flags)
        {

            byte[] buffer = new byte[(int)(tex.Width * tex.Height * 4)];
            GCHandle handle = GCHandle.Alloc(buffer, GCHandleType.Pinned);
            GL.BindTexture(TextureTarget.Texture2D, tex.TextureId);

            GL.GetTextureSubImage(tex.TextureId, 0, 0, 0,0,  (int)tex.Width, (int)tex.Height,1, TKPixelFormat.Bgra, PixelType.UnsignedByte, buffer.Length, handle.AddrOfPinnedObject());

            handle.Free();
            
#if NO_CL
            bmp.Log("Creating CL Buffer from Image", DebugChannel.Warning);
            return null;
#else
            MemoryBuffer mb = CL.CreateBuffer(buffer, flags);
            return mb;
#endif
        }


        private static void DefaultTexParameter()
        {
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);

        }

        public static GameTexture Load(string filename)
        {
            filename.Log($"Loading Texture: " + filename, DebugChannel.Log);
            Bitmap bmp = new Bitmap(filename);

            bmp.RotateFlip(RotateFlipType.RotateNoneFlipY);

            BitmapData data = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadOnly,
                SYSPixelFormat.Format32bppArgb);

            GameTexture ret = new GameTexture();
            GL.BindTexture(TextureTarget.Texture2D, ret.TextureId);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, data.Width, data.Height, 0, OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);
            bmp.UnlockBits(data);



            DefaultTexParameter();

            

            return ret;

        }

        public static void Update(GameTexture tex, byte[] buffer, int width, int height)
        {
            GL.BindTexture(TextureTarget.Texture2D, tex.TextureId);
            GL.TexSubImage2D(TextureTarget.Texture2D, 0, 0, 0, width, height, TKPixelFormat.Bgra, PixelType.UnsignedByte, buffer);

        }

        private static int texRawCount = 0;

        public static GameTexture Load(byte[] buffer, int width, int height)
        {
            string n = "RawTexture" + texRawCount + "_" + width + "_" + height;
            texRawCount++;
            GameTexture ret = new GameTexture();
            ret.Log($"Loading Texture... Width: {width} Height: {height}", DebugChannel.Log);

            GL.BindTexture(TextureTarget.Texture2D, ret.TextureId);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, width, height, 0, TKPixelFormat.Bgra, PixelType.UnsignedByte, buffer);



            DefaultTexParameter();
            TextureProvider.Add(ret, n);
            return ret;
        }
    }
}