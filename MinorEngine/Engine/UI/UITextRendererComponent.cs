using System.Text;
using Engine.DataTypes;
using Engine.Debug;
using Engine.Rendering;
using Engine.Rendering.Contexts;

namespace Engine.UI
{
    /// <summary>
    /// Implements A Text Renderer Component
    /// </summary>
    public class UITextRendererComponent : UIElement
    {
        /// <summary>
        /// The backing field of the Context
        /// </summary>
        private TextRenderContext _context;

        /// <summary>
        /// The Context used to draw
        /// </summary>
        public override RenderContext Context
        {
            get
            {
                if (_context == null)
                {
                    ContextInvalid = false;
                    _context = new TextRenderContext(Shader, Position, Scale, Owner._worldTransformCache, WorldSpace,
                        Alpha, font, _text, RenderQueue);
                }
                else if (ContextInvalid)
                {
                    ContextInvalid = false;
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

        /// <summary>
        /// the Font that is used to draw
        /// </summary>
        private readonly GameFont font;

        /// <summary>
        /// the backing field for Text
        /// </summary>
        private string _text = "HELLO";

        /// <summary>
        /// The text that is drawn
        /// </summary>
        public string Text
        {
            get => _text;
            set
            {
                if (!string.Equals(_text, value))
                {
                    ContextInvalid = true;
                    _text = value;
                }
            }
        }

        /// <summary>
        /// Public Constructor
        /// </summary>
        /// <param name="fontName">The name of the Font</param>
        /// <param name="worldSpace">Is the Object in world space</param>
        /// <param name="alpha">Alpha value of the image</param>
        /// <param name="shader">The shader to be used</param>
        public UITextRendererComponent(string fontName, bool worldSpace, float alpha, ShaderProgram shader) : base(
            shader, worldSpace, alpha)
        {
            font = UIHelper.Instance.FontLibrary.GetFont("Arial");
            Logger.Log("Reading Character Glyphs from " + fontName, DebugChannel.Log);
        }
    }
}