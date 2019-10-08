using System.Text;
using Engine.DataTypes;
using Engine.Debug;
using Engine.Rendering;
using Engine.Rendering.Contexts;

namespace Engine.UI
{
    public class UITextRendererComponent : UIElement
    {
        private TextRenderContext _context;
        private bool _contextInvalid = true;
        public override RenderContext Context
        {
            get
            {
                if (_context == null||_contextInvalid)
                {
                    _context = new TextRenderContext(Shader, Position, Scale, Owner._worldTransformCache, WorldSpace,
                        Alpha, font, _text, RenderQueue);
                }
                else
                {
                    _context.ModelMat = Owner._worldTransformCache;
                    _context.Alpha = Alpha;
                    _context.Position = Position;
                    _context.Scale = Scale;
                    _context.DisplayText = _text;
                    _context.WorldSpace = WorldSpace;
                }

                return _context;
            }
        }

        private readonly GameFont font;
        private string _text = "HELLO";
        private static StringBuilder sb = new StringBuilder();

        public string Text
        {
            get => _text;
            set
            {
                if (!string.Equals(_text, value))
                {
                    _contextInvalid = true;
                    _text = value;
                }
            }
        }

        public UITextRendererComponent(string fontName, bool worldSpace, float alpha, ShaderProgram shader) : base(
            shader, worldSpace, alpha)
        {
            font = UIHelper.Instance.FontLibrary.GetFont("Arial");
            Logger.Log("Reading Character Glyphs from " + fontName, DebugChannel.Log);
        }
    }
}