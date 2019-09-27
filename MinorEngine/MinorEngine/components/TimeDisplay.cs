using System;
using GameEngine.engine.ui;
using MinorEngine.engine.components;

namespace MinorEngine.components
{
    public class TimeDisplay : AbstractComponent
    {
        private UITextRendererComponent tr;
        private float time;
        private float frameTime = 0;
        private int fpsFrames = 0;
        private int fps = 0;
        public override void Update(float deltaTime)
        {
            time += deltaTime;
            frameTime += deltaTime;
            fpsFrames++;
            if (frameTime > 1f)
            {
                fps = fpsFrames;
                fpsFrames = 0;
                frameTime = 0;
            }
            if (tr != null)
            {
                tr.Text = "DT: " + MathF.Round(time, 3) +
                    "\nFPS: " + MathF.Round(fps);
            }
            else
            {
                tr = Owner.GetComponent<UITextRendererComponent>();
            }
        }
    }
}