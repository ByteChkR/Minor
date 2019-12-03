using System;
using Engine.Core;
using Engine.UI;

namespace Engine.Demo.components
{
    public class TimeDisplay : AbstractComponent
    {
        private int fps;
        private int fpsFrames;
        private float frameTime;
        private float time;
        private UiTextRendererComponent tr;

        protected override void Awake()
        {
            base.Awake();
            tr = Owner.GetComponent<UiTextRendererComponent>();
        }

        protected override void Update(float deltaTime)
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

            tr.Text = "DT: " + MathF.Round(time, 3) +
                      "\nFPS: " + MathF.Round(fps);
        }
    }
}