using System;
using System.Collections.Generic;
using SharpFont;

namespace Engine.DataTypes
{
    /// <summary>
    /// A Data Type that is containing the information that is needed to store a Font in OpenGL
    /// </summary>
    public class GameFont :IDisposable
    {
        /// <summary>
        /// The Name of the Font. This is the key you can load Fonts by.
        /// </summary>
        public string Name => _fontFace.FullName;

        /// <summary>
        /// The internal font atlas that is used to map Text Characters to the OpenGL Abstraction (TextCharacter)
        /// </summary>
        private readonly Dictionary<char, TextCharacter> _fontAtlas;

        /// <summary>
        /// Private field for the font this GameFont has been loaded from. Not needed perse, but it is convenient
        /// </summary>
        private readonly FontFace _fontFace;

        /// <summary>
        /// The Pixel size that has been used to Load the font into the GPU Memory
        /// </summary>
        public int Size { get; }

        /// <summary>
        /// The Font Metrics Associated with the Game Font
        /// Is used for getting the LineHeight when rendering line breaks and other characters.
        /// </summary>
        public FaceMetrics Metrics => _fontFace.GetFaceMetrics(Size);


        /// <summary>
        /// Internal Constructor to create a Game Font Data object.
        /// </summary>
        internal GameFont(FontFace ff, int size, Dictionary<char, TextCharacter> fontAtlas)
        {
            Size = size;
            _fontFace = ff;
            _fontAtlas = fontAtlas;
        }

        /// <summary>
        /// A function to get a Text Character from a System Char.
        /// </summary>
        /// <param name="character">the input character</param>
        /// <param name="charInfo">the GL Character</param>
        /// <returns>True if the Character has been found</returns>
        public bool TryGetCharacter(char character, out TextCharacter charInfo)
        {
            return _fontAtlas.TryGetValue(character, out charInfo);
        }

        /// <summary>
        /// Disposable implementation to free the Texture Atlas once it is no longer needed.
        /// </summary>
        public void Dispose()
        {
            foreach (var textCharacter in _fontAtlas)
            {
                textCharacter.Value.Dispose();
            }
            _fontAtlas.Clear();
        }
    }
}