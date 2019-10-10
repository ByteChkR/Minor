using System;
using OpenTK.Graphics.OpenGL;

namespace Engine.DataTypes
{
    /// <summary>
    /// A Data Type that is containing the information that is needed to store a Texture in OpenGL
    /// </summary>
    public class Texture : IDisposable
    {
        /// <summary>
        /// The Texture type used to automatically bind textures to the right Uniforms in the shader
        /// None -> Diffuse
        /// </summary>
        public TextureType TexType { get; set; }

        /// <summary>
        /// The OpenGL Handle to the texture
        /// </summary>
        public int TextureId { get; }

        /// <summary>
        /// Private flag to keep from disposing the texture twice
        /// </summary>
        private bool _disposed;

        /// <summary>
        /// Convenient Wrapper to query the texture width
        /// </summary>
        public float Width
        {
            get
            {
                GL.BindTexture(TextureTarget.Texture2D, TextureId);
                GL.GetTexLevelParameter(TextureTarget.Texture2D, 0, GetTextureParameter.TextureWidth, out float width);
                GL.BindTexture(TextureTarget.Texture2D, 0);
                return width;
            }
        }

        /// <summary>
        /// Convenient Wrapper to query the texture height
        /// </summary>
        public float Height
        {
            get
            {
                GL.BindTexture(TextureTarget.Texture2D, TextureId);
                GL.GetTexLevelParameter(TextureTarget.Texture2D, 0, GetTextureParameter.TextureHeight,
                    out float height);
                GL.BindTexture(TextureTarget.Texture2D, 0);
                return height;
            }
        }

        /// <summary>
        /// Internal Constructor to Create a Texture Object from a GL Texture Handle
        /// </summary>
        /// <param name="textureId"></param>
        internal Texture(int textureId)
        {
            TextureId = textureId;
        }

        /// <summary>
        /// Disposed Implementation to free the texture memory when it is no longer in use.
        /// </summary>
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