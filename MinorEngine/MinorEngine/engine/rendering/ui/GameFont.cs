using System.Collections.Generic;
using MinorEngine.engine.rendering.ui;
using SharpFont;

namespace MinorEngine.engine.rendering.ui
{
    public class GameFont
    {
        public string Name => _fontFace.FullName;
        private readonly Dictionary<char, TextCharacter> _fontAtlas;
        private readonly FontFace _fontFace;
        private readonly int size;
        public FaceMetrics Metrics => _fontFace.GetFaceMetrics(size);

        public GameFont(FontFace ff, int size, Dictionary<char, TextCharacter> fontAtlas)
        {
            this.size = size;
            _fontFace = ff;
            _fontAtlas = fontAtlas;
        }

        public bool TryGetCharacter(char character, out TextCharacter charInfo)
        {
            return _fontAtlas.TryGetValue(character, out charInfo);
        }
    }
}