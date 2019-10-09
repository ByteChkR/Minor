using System;
using OpenTK.Graphics.OpenGL;

namespace Engine.DataTypes
{
    public class Texture :IDisposable
    {

        public TextureType TexType { get; set; }
        public int TextureId { get; }
        private bool _disposed;

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


        internal Texture(int textureId)
        {
            TextureId = textureId;
        }


        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }

            _disposed = true;
            GL.DeleteTexture(TextureId);
        }
    }
}