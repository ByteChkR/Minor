using System;
using Engine.Debug;
using OpenTK.Graphics.OpenGL;

namespace Engine.DataTypes
{
    /// <summary>
    /// A Data Type that is containing the information that is needed to store a Texture in OpenGL
    /// </summary>
    public class Texture : IDisposable
    {
        /// <summary>
        /// Private flag to keep from disposing the texture twice
        /// </summary>
        private bool disposed;

        private bool dontDispose;
        private readonly long bytes;

        /// <summary>
        /// Internal Constructor to Create a Texture Object from a GL Texture Handle
        /// </summary>
        /// <param name="textureId"></param>
        internal Texture(int textureId, long bytes)
        {
            this.bytes = bytes;
            TextureId = textureId;
        }

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
        /// Disposed Implementation to free the texture memory when it is no longer in use.
        /// </summary>
        public void Dispose()
        {
            if (disposed || dontDispose)
            {
                return;
            }

            EngineStatisticsManager.GlObjectDestroyed(bytes);
            disposed = true;
            GL.DeleteTexture(TextureId);
        }

        /// <summary>
        /// Returns a Copy of this Mesh(It Does not copy the buffers on the GPU)
        /// It is save to Dispose the copied object.
        /// BUT NEVER DISPOSE THE SOURCE OBJECT BEFORE ALL COPIES HAVE BEEN DISPOSED, otherwise they become unusable and will probably crash the engine once interacted with
        /// </summary>
        /// <returns>A Copy of this Mesh Object</returns>
        public Texture Copy()
        {
            return new Texture(TextureId, bytes)
            {
                dontDispose = true
            };
        }
    }
}