using System;
using System.Collections.Generic;
using System.Text;

namespace MinorEngine.engine.rendering.ui
{
    public class TextCharacter
    {
        public GameTexture GlTexture { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public float BearingX { get; set; }
        public float BearingY { get; set; }
        public float Advance { get; set; }
    }
}
