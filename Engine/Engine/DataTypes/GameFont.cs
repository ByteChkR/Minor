using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Net.Mime;
using System.Reflection;
using Engine.Core;
using Engine.Debug;
using Engine.Exceptions;
using Engine.Physics.BEPUutilities;
using Engine.UI;
using OpenTK.Input;
using SharpFont;

namespace Engine.DataTypes
{
    /// <summary>
    /// A Data Type that is containing the information that is needed to store a Font in OpenGL
    /// </summary>
    public class GameFont : IDisposable
    {
        /// <summary>
        /// The default font
        /// </summary>
        public static GameFont DefaultFont => _defaultFont ?? (_defaultFont = GetDefaultFont());

        /// <summary>
        /// Creates the default font from embedded program resources
        /// </summary>
        /// <returns>The Default font</returns>
        private static GameFont GetDefaultFont()
        {
            Assembly asm = Assembly.GetExecutingAssembly();
            string[] paths = asm.GetManifestResourceNames();
            string path = asm.GetName().Name + "._DefaultResources.DefaultFont.ttf";
            using (Stream resourceStream = asm.GetManifestResourceStream(path))
            {
                if (resourceStream == null)
                {
                    Logger.Crash(new EngineException("Could not load default font"), false);
                    return null;
                }

                GameFont f = FontLibrary.LoadFontInternal(resourceStream, 32, out string name);
                resourceStream.Close();
                return f;
            }
        }

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
        /// Backing field for the default font
        /// </summary>
        private static GameFont _defaultFont;

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
                    float count = UITextRendererComponent.TabToSpaceCount -
                                  len % UITextRendererComponent.TabToSpaceCount;
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
            return _fontAtlas.TryGetValue(character, out charInfo);
        }

        /// <summary>
        /// Disposable implementation to free the Texture Atlas once it is no longer needed.
        /// </summary>
        public void Dispose()
        {
            foreach (KeyValuePair<char, TextCharacter> textCharacter in _fontAtlas)
            {
                textCharacter.Value.Dispose();
            }

            _fontAtlas.Clear();
        }
    }
}