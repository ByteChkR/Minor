using System;
using System.Collections.Generic;
using Engine.Core;
using Engine.Physics.BEPUutilities;
using Engine.UI;
using SharpFont;

namespace Engine.DataTypes
{
    /// <summary>
    /// A Data Type that is containing the information that is needed to store a Font in OpenGL
    /// </summary>
    public class GameFont : IDisposable
    {
        /// <summary>
        /// The internal font atlas that is used to map Text Characters to the OpenGL Abstraction (TextCharacter)
        /// </summary>
        private readonly Dictionary<char, TextCharacter> fontAtlas;

        /// <summary>
        /// Private field for the font this GameFont has been loaded from. Not needed perse, but it is convenient
        /// </summary>
        private readonly FontFace fontFace;


        /// <summary>
        /// Internal Constructor to create a Game Font Data object.
        /// </summary>
        internal GameFont(FontFace ff, int size, Dictionary<char, TextCharacter> fontAtlas)
        {
            Size = size;
            fontFace = ff;
            this.fontAtlas = fontAtlas;
        }

        /// <summary>
        /// The Name of the Font. This is the key you can load Fonts by.
        /// </summary>
        public string Name => fontFace.FullName;

        /// <summary>
        /// The Pixel size that has been used to Load the font into the GPU Memory
        /// </summary>
        public int Size { get; }

        /// <summary>
        /// The Font Metrics Associated with the Game Font
        /// Is used for getting the LineHeight when rendering line breaks and other characters.
        /// </summary>
        public FaceMetrics Metrics => fontFace.GetFaceMetrics(Size);

        /// <summary>
        /// Disposable implementation to free the Texture Atlas once it is no longer needed.
        /// </summary>
        public void Dispose()
        {
            foreach (KeyValuePair<char, TextCharacter> textCharacter in fontAtlas)
            {
                textCharacter.Value.Dispose();
            }

            fontAtlas.Clear();
        }

        /// <summary>
        /// Returns the 2D Bounding box of the text
        /// </summary>
        /// <param name="stringValue"></param>
        /// <returns></returns>
        public Vector2 GetRenderBounds(string stringValue)
        {
            int scrW = GameEngine.Instance.Width;
            int scrH = GameEngine.Instance.Height;
            Vector2 pos = Vector2.Zero; //Hacked
            float x = pos.X;
            float y = pos.Y;
            for (int i = 0; i < stringValue.Length; i++)
            {
                if (stringValue[i] == '\n')
                {
                    FaceMetrics fm = Metrics;
                    x = pos.X;
                    y -= fm.LineHeight / scrH;
                    continue;
                }


                if (stringValue[i] == '\t')
                {
                    float len = x - pos.X;
                    float count = UiTextRendererComponent.TabToSpaceCount -
                                  len % UiTextRendererComponent.TabToSpaceCount;
                    float val = count;
                    x += val;
                    continue;
                }
                //x-pos.x


                if (!TryGetCharacter(stringValue[i], out TextCharacter chr))
                {
                    TryGetCharacter('?', out chr);
                }

                x += chr.Advance / scrW;
            }

            return new Vector2(x, y + Metrics.LineHeight / scrH / 2);
        }


        /// <summary>
        /// A function to get a Text Character from a System Char.
        /// </summary>
        /// <param name="character">the input character</param>
        /// <param name="charInfo">the GL Character</param>
        /// <returns>True if the Character has been found</returns>
        public bool TryGetCharacter(char character, out TextCharacter charInfo)
        {
            return fontAtlas.TryGetValue(character, out charInfo);
        }
    }
}