using System;
using Assimp;
using MinorEngine.debug;
using OpenTK.Graphics.OpenGL;
using TKPixelFormat = OpenTK.Graphics.OpenGL.PixelFormat;
using SYSPixelFormat = System.Drawing.Imaging.PixelFormat;

namespace MinorEngine.engine.rendering
{
    public class GameTexture : IDisposable
    {
        public int TextureId { get; }
        private bool _disposed;
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