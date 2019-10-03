﻿using System.Collections.Generic;
using SharpFont;

namespace MinorEngine.engine.ui
{
    public class GameFont
    {
        public string Name => _fontFace.FullName;
        private readonly Dictionary<char, Character> _fontAtlas;
        private readonly FontFace _fontFace;
        private readonly int size;
        public FaceMetrics Metrics => _fontFace.GetFaceMetrics(size);

        public GameFont(FontFace ff, int size, Dictionary<char, Character> fontAtlas)
        {
            this.size = size;
            _fontFace = ff;
            _fontAtlas = fontAtlas;
        }

        public bool TryGetCharacter(char character, out Character charInfo)
        {
            return _fontAtlas.TryGetValue(character, out charInfo);
        }
    }
}