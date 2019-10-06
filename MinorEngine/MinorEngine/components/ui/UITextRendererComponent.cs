﻿using MinorEngine.components.ui;
using MinorEngine.debug;
using MinorEngine.engine.rendering;
using MinorEngine.engine.rendering.contexts;
using MinorEngine.engine.rendering.ui;

namespace MinorEngine.engine.components.ui
{


    public class UITextRendererComponent : UIElement
    {

        private TextRenderContext _context;
        public override RenderContext Context
        {
            get
            {
                if (_context == null)
                {
                    _context = new TextRenderContext(Shader, Position, Scale, Owner._worldTransformCache, WorldSpace, Alpha, font, _cachedText, RenderQueue);
                }
                else
                {
                    _context.ModelMat = Owner._worldTransformCache;
                    _context.Alpha = Alpha;
                    _context.Position = Position;
                    _context.Scale = Scale;
                    _context.DisplayText = _cachedText;
                    _context.WorldSpace = WorldSpace;
                    
                }

                return _context;
            }
        }
        private readonly GameFont font;
        private string _text = "HELLO";
        private string _cachedText;

        public string Text
        {
            get => _text;
            set
            {
                if (!string.Equals(_text, value))
                {
                    _text = value;
                    _cachedText = _text.Replace("\t", "   "); //Replace Tab character with spaces
                }

            }
        }

        public UITextRendererComponent(string fontName, bool worldSpace, float alpha, ShaderProgram shader) : base(shader, worldSpace, alpha)
        {
            font = UIHelper.Instance.FontLibrary.GetFont("Arial");
            Logger.Log("Reading Character Glyphs from " + fontName, DebugChannel.Log);


        }
    }
}