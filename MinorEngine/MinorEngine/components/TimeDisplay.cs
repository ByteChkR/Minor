using System;
using GameEngine.engine.ui;
using MinorEngine.engine.components;

namespace MinorEngine.components
{
    public class TimeDisplay : AbstractComponent
    {
        private UITextRendererComponent tr;
        private float time = 0;
        public override void Update(float deltaTime)
        {
            time += deltaTime;
            if (tr != null)
            {
                tr.Text = "DT: " + MathF.Round(time, 3);
            }
            else
            {
                tr = Owner.GetComponent<UITextRendererComponent>();
            }
        }
    }
}