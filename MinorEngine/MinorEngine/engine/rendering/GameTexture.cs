using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Resources;
using System.Runtime.InteropServices;
using Assimp;
using MinorEngine.CLHelperLibrary;
using Common;
using MinorEngine.debug;
using MinorEngine.engine.core;
using OpenCl.DotNetCore.Memory;
using OpenTK.Graphics.OpenGL;
using ResourceManager = MinorEngine.engine.core.ResourceManager;
using TKPixelFormat = OpenTK.Graphics.OpenGL.PixelFormat;
using SYSPixelFormat = System.Drawing.Imaging.PixelFormat;
using TextureWrapMode = OpenTK.Graphics.OpenGL.TextureWrapMode;

namespace MinorEngine.engine.rendering
{
    public class GameTexture : IDisposable
    {
        public int TextureId { get; }
        private bool _disposed = false;
        public string TextureDescriptor { get; }
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
                GL.GetTexLevelParameter(TextureTarget.Texture2D, 0, GetTextureParameter.TextureHeight,
                    out float height);
                return height;
            }
        }


        internal GameTexture(int texID, string textureDescriptor)
        {
            TexType = TextureType.Diffuse;
            TextureId = texID;
            TextureDescriptor = textureDescriptor;
        }

        internal GameTexture(string textureDescriptor)
        {
            TextureDescriptor = textureDescriptor;
            TexType = TextureType.Diffuse;
            GL.GenTextures(1, out int id);
            TextureId = id;
        }


        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }
            _disposed = true;
            Logger.Log("Deleting Texture: " + TextureDescriptor + "(ID: " + TextureId + ")..", DebugChannel.Log);
            GL.DeleteTexture(TextureId);

        }
    }
}