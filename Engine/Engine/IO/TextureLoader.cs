using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using Assimp;
using Engine.DataTypes;
using Engine.Debug;
using Engine.Exceptions;
using Engine.OpenCL;
using Engine.OpenCL.DotNetCore.Memory;
using Engine.OpenCL.TypeEnums;
using OpenTK.Graphics.OpenGL;
using PixelFormat = OpenTK.Graphics.OpenGL.PixelFormat;
using TextureType = Engine.DataTypes.TextureType;
using TextureWrapMode = OpenTK.Graphics.OpenGL.TextureWrapMode;

namespace Engine.IO
{
    public static class TextureLoader
    {
        /// <summary>
        /// Creates a 4 Channel Texture from a pointer and width/height
        /// </summary>
        /// <param name="ptr">IntPtr of the array to copy</param>
        /// <param name="width">width of the array</param>
        /// <param name="height">height of the array</param>
        /// <returns>The texture</returns>
        public static Texture BytesToTexture(IntPtr ptr, int width, int height)
        {
            int texID = GL.GenTexture();

            GL.BindTexture(TextureTarget.Texture2D, texID);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, width, height, 0, PixelFormat.Bgra,
                PixelType.UnsignedByte, ptr);


            DefaultTexParameter();
            GL.BindTexture(TextureTarget.Texture2D, 0);
            long bytes = width * height * 4;
            EngineStatisticsManager.GLObjectCreated(bytes);
            return new Texture(texID, bytes);
            ;
        }

        /// <summary>
        /// Creates a 4 Channel Texture from a byte array and width/height
        /// </summary>
        /// <param name="buffer">IntPtr of the array to copy</param>
        /// <param name="width">width of the array</param>
        /// <param name="height">height of the array</param>
        /// <returns>The texture</returns>
        public static Texture BytesToTexture(byte[] buffer, int width, int height)
        {
            GCHandle handle = GCHandle.Alloc(buffer, GCHandleType.Pinned);
            Texture ret = BytesToTexture(handle.AddrOfPinnedObject(), width, height);

            handle.Free();
            return ret;
        }


        /// <summary>
        /// Creates a 4 Channel Texture from width and height
        /// </summary>
        /// <param name="width">width of the array</param>
        /// <param name="height">height of the array</param>
        /// <returns>The texture</returns>
        public static Texture ParameterToTexture(int width, int height)
        {
            return BytesToTexture(IntPtr.Zero, width, height);
        }


        /// <summary>
        /// Reads Texture Data from GPU into CPU Memory
        /// </summary>
        /// <param name="tex"></param>
        /// <returns></returns>
        private static byte[] TextureToByteArray(Texture tex)
        {
            byte[] buffer = new byte[(int) (tex.Width * tex.Height * 4)];
            GCHandle handle = GCHandle.Alloc(buffer, GCHandleType.Pinned);
            GL.BindTexture(TextureTarget.Texture2D, tex.TextureId);

            GL.GetTextureSubImage(tex.TextureId, 0, 0, 0, 0, (int) tex.Width, (int) tex.Height, 1, PixelFormat.Bgra,
                PixelType.UnsignedByte, buffer.Length, handle.AddrOfPinnedObject());

            handle.Free();
            return buffer;
        }


        /// <summary>
        /// Reads Texture Data from CL into CPU Memory and passes it into the CL implementation
        /// </summary>
        /// <param name="tex">Input Texture</param>
        /// <returns>CL Buffer Object containing the image data</returns>
        public static MemoryBuffer TextureToMemoryBuffer(CLAPI instance, Texture tex)
        {
            byte[] buffer = TextureToByteArray(tex);
            return CLAPI.CreateBuffer(instance, buffer, MemoryFlag.CopyHostPointer | MemoryFlag.ReadWrite);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tex"></param>
        /// <param name="data">Array With the new data</param>
        /// <param name="width">width of the array</param>
        /// <param name="height">height of the array</param>
        public static void Update(Texture tex, byte[] data, int width, int height)
        {
            GL.BindTexture(TextureTarget.Texture2D, tex.TextureId);
            GL.TexSubImage2D(TextureTarget.Texture2D, 0, 0, 0, width, height, PixelFormat.Bgra,
                PixelType.UnsignedByte, data);
            GL.BindTexture(TextureTarget.Texture2D, 0);
        }

        /// <summary>
        /// Turns a bitmap into a GL Texture
        /// </summary>
        /// <param name="bmp">Bitmap to Load</param>
        /// <returns>The GL Texture</returns>
        public static byte[] BitmapToBytes(Bitmap bmp)
        {
            BitmapData data = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadOnly,
                System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            byte[] buf = new byte[bmp.Width * bmp.Height * 4];

            Marshal.Copy(data.Scan0, buf, 0, buf.Length);

            bmp.UnlockBits(data);
            return buf;
        }

        /// <summary>
        /// Turns a bitmap into a GL Texture
        /// </summary>
        /// <param name="bmp">Bitmap to Load</param>
        /// <returns>The GL Texture</returns>
        public static Texture BitmapToTexture(Bitmap bmp)
        {
            bmp.RotateFlip(RotateFlipType.RotateNoneFlipY);
            BitmapData data = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadOnly,
                System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            Texture tex = BytesToTexture(data.Scan0, bmp.Width, bmp.Height);

            bmp.UnlockBits(data);
            bmp.RotateFlip(RotateFlipType.RotateNoneFlipY);
            return tex;
        }


        /// <summary>
        /// Loads all textures from a specified Assimp Scene.
        /// </summary>
        /// <param name="scene">The Assimp Scene</param>
        /// <returns>An array of all textures contained in the scene</returns>
        private static Texture[] AssimpSceneToTextures(Scene scene)
        {
            if (!scene.HasTextures)
            {
                return new Texture[0];
            }

            List<Texture> list = new List<Texture>();
            foreach (EmbeddedTexture x in scene.Textures)
            {
                list.Add(AssimpEmbeddedToTexture(x));
            }

            return list.ToArray();
        }

        /// <summary>
        /// Creates a flat byte array out of a list of Texels from assimp
        /// </summary>
        /// <param name="imageData">The texels to convert</param>
        /// <returns>Flat byte array containing the data</returns>
        private static byte[] flattenImageData(Texel[] imageData)
        {
            byte[] ret = new byte[imageData.Length * 4];
            for (int i = 0; i < imageData.Length; i++)
            {
                TexelToByteSequence(i * 4, ret, imageData[i]);
            }

            return ret;
        }

        /// <summary>
        /// Creates a flat byte array sequence out of a texel
        /// </summary>
        /// <param name="txl">The texel to convert</param>
        /// <param name="arr">the output array</param>
        /// <param name="startidx">The index to start inserting the texel data</param>
        private static void TexelToByteSequence(int startidx, byte[] arr, Texel txl)
        {
            arr[startidx] = txl.R;
            arr[startidx + 1] = txl.G;
            arr[startidx + 2] = txl.B;
            arr[startidx + 3] = txl.A;
        }

        //Maybe create key for textures loaded with assimp and then use them as cache name
        /// <summary>
        /// Converts an Assimp Texture into a Engine/OpenGL Texture
        /// </summary>
        /// <param name="tex">Assimp texture</param>
        /// <returns>GL Texure</returns>
        public static Texture AssimpEmbeddedToTexture(EmbeddedTexture tex)
        {
            return BytesToTexture(flattenImageData(tex.NonCompressedData), tex.Width, tex.Height);
        }

        public static Texture ColorToTexture(Color c)
        {
            Bitmap bmp = new Bitmap(1, 1);
            bmp.SetPixel(0, 0, c);
            return BitmapToTexture(bmp);
        }

        /// <summary>
        /// Texture a file from disk
        /// </summary>
        /// <param name="file">The file to load</param>
        /// <returns>The GL Texture</returns>
        public static Texture FileToTexture(string file)
        {
            if (!IOManager.Exists(file))
            {
                Logger.Crash(new InvalidFilePathException(file), true);
                return DefaultFilepaths.DefaultTexture;
            }

            Bitmap bmp = new Bitmap(IOManager.GetStream(file));
            Texture t = BitmapToTexture(bmp);
            bmp.Dispose();
            return t;
        }

        /// <summary>
        /// Applies the Default texParameters to the loaded objects
        /// </summary>
        private static void DefaultTexParameter()
        {
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter,
                (int) TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter,
                (int) TextureMagFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS,
                (int) TextureWrapMode.Repeat);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT,
                (int) TextureWrapMode.Repeat);
        }

        /// <summary>
        /// Copies a Texture by copying the byte arrays
        /// </summary>
        /// <param name="other">The texture to copy</param>
        /// <returns>A copy of Other</returns>
        private static Texture Copy(Texture other)
        {
            return BytesToTexture(TextureToByteArray(other), (int) other.Width, (int) other.Height);
        }

        /// <summary>
        /// Loads Textures from AssimpMaterials
        /// </summary>
        /// <param name="m">Assimp Material</param>
        /// <param name="texType">Type of texture</param>
        /// <param name="dir">The directory of the file that references this material</param>
        /// <returns>A list of textures that were attached to the material</returns>
        internal static List<Texture> LoadMaterialTextures(Material m, TextureType texType, string dir)
        {
            List<Texture> ret = new List<Texture>();

            Logger.Log("Loading Baked Material Textures of type: " + Enum.GetName(typeof(TextureType), texType),
                DebugChannel.Log | DebugChannel.IO, 1);
            for (int i = 0; i < m.GetMaterialTextureCount((Assimp.TextureType) texType); i++)
            {
                TextureSlot s;
                m.GetMaterialTexture((Assimp.TextureType) texType, i, out s);
                Texture tx = FileToTexture(dir + s.FilePath);
                tx.TexType = texType;
                ret.Add(tx);
            }

            return ret;
        }
    }
}