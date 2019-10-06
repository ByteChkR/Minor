using MinorEngine.components.ui;
using MinorEngine.debug;
using MinorEngine.engine.rendering;
using MinorEngine.engine.rendering.contexts;
using MinorEngine.engine.rendering.ui;

namespace MinorEngine.engine.components.ui
{
    

    public class UITextRendererComponent : UIElement
    {
        public override RenderContext Context => new TextRenderContext(Shader, Position, Scale, Owner._worldTransformCache, WorldSpace, Alpha, font, Text, RenderQueue);

        private readonly GameFont font;
        public string Text { get; set; } = "HELLO";

        public UITextRendererComponent(string fontName, bool worldSpace, float alpha, ShaderProgram shader) : base(shader, worldSpace, alpha)
        {
            font = UIHelper.Instance.FontLibrary.GetFont("Arial");
            Logger.Log("Reading Character Glyphs from " + fontName, DebugChannel.Log);

           
        }
    }
}