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

        public override RenderContext Context
        {
            get
            {
                if (_context == null)
                {
                    _context = new TextRenderContext(Shader, Position, Scale, Owner._worldTransformCache, WorldSpace,
                        Alpha, font, _cachedText, RenderQueue);
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
        private static StringBuilder sb = new StringBuilder();

        public string Text
        {
            get => _text;
            set
            {
                if (!string.Equals(_text, value))
                {
                    _cachedText = value;
                    ////_cachedText = _text.Replace("\t", "   "); //Replace Tab character with spaces
                    //int charcount = 0;
                    //for (int i = 0; i < value.Length; i++)
                    //{
                    //    char c = value[i];
                    //    if (c == '\n')
                    //    {
                    //        sb.Append('\n');
                    //        charcount = 0;
                    //        continue;
                    //    }

                    //}

                    //_cachedText = sb.ToString();
                    //sb.Clear();
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