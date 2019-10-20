using System;
using System.Drawing;
using System.IO;
using System.Reflection;
using Engine.Debug;
using Engine.Exceptions;
using Engine.IO;
using Engine.UI;
using OpenTK.Graphics.OpenGL;

namespace Engine.DataTypes
{
    /// <summary>
    /// A Data Type that is containing the information that is needed to store a Texture in OpenGL
    /// </summary>
    public class Texture : IDisposable
    {
        /// <summary>
        /// Backing field for the default texture
        /// </summary>
        private static Texture _defaultTexture;

        public static Texture DefaultTexture => _defaultTexture ?? (_defaultTexture = GetDefaultTexture());

        /// <summary>
        /// Creates the default Texture from embedded program resources
        /// </summary>
        /// <returns>The Default Texture</returns>
        private static Texture GetDefaultTexture()
        {
            Assembly asm = Assembly.GetExecutingAssembly();
            string[] paths = asm.GetManifestResourceNames();
            string path = asm.GetName().Name + "._DefaultResources.DefaultTexture.bmp";
            using (Stream resourceStream = asm.GetManifestResourceStream(path))
            {
                if (resourceStream == null)
                {
                    Logger.Crash(new EngineException("Could not load default Texture"), false);
                    return null;
                }

                Texture f = TextureLoader.BitmapToTexture(new Bitmap(resourceStream));
                GL.BindTexture(TextureTarget.Texture2D, f.TextureId);
                GL.TextureParameter(f.TextureId, TextureParameterName.TextureMinFilter, (int) TextureMinFilter.Nearest);
                GL.TextureParameter(f.TextureId, TextureParameterName.TextureMagFilter, (int) TextureMagFilter.Nearest);
                GL.BindTexture(TextureTarget.Texture2D, 0);

                resourceStream.Close();
                return f;
            }
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
        /// Private flag to keep from disposing the texture twice
        /// </summary>
        private bool _disposed;

        private bool _dontDispose;

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
        /// Returns a Copy of this Mesh(It Does not copy the buffers on the GPU)
        /// It is save to Dispose the copied object.
        /// BUT NEVER DISPOSE THE SOURCE OBJECT BEFORE ALL COPIES HAVE BEEN DISPOSED, otherwise they become unusable and will probably crash the engine once interacted with
        /// </summary>
        /// <returns>A Copy of this Mesh Object</returns>
        public Texture Copy()
        {
            return new Texture(TextureId)
            {
                _dontDispose = true
            };
        }

        /// <summary>
        /// Disposed Implementation to free the texture memory when it is no longer in use.
        /// </summary>
        public void Dispose()
        {
            if (_disposed || _dontDispose)
            {
                return;
            }

            _disposed = true;
            GL.DeleteTexture(TextureId);
        }
    }
}