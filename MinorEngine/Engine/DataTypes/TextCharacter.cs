using System;

namespace Engine.DataTypes
{

    /// <summary>
    /// A Data Type that is containing the information that is needed to render a Single Characer in OpenGL
    /// </summary>
    public class TextCharacter : IDisposable
    {

        /// <summary>
        /// The Texture containing the Pixel Data of the Character
        /// </summary>
        public Texture GlTexture { get; set; }
        
        /// <summary>
        /// Width in Pixels
        /// </summary>
        public int Width { get; set; }

        /// <summary>
        /// Height in Pixels
        /// </summary>
        public int Height { get; set; }

        /// <summary>
        /// Bearing X in Pixels
        /// https://learnopengl.com/img/in-practice/glyph.png
        /// </summary>
        public float BearingX { get; set; }

        /// <summary>
        /// Bearing Y Pixels
        /// https://learnopengl.com/img/in-practice/glyph.png
        /// </summary>
        public float BearingY { get; set; }

        /// <summary>
        /// The advance in Pixels
        /// </summary>
        public float Advance { get; set; }

        /// <summary>
        /// Disposable implementation to free the GL Texture representing the Character once it is no longer needed.
        /// </summary>
        public void Dispose()
        {
            GlTexture.Dispose();
        }
    }
}