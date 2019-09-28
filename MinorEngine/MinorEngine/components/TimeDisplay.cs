using System;
using GameEngine.engine.ui;
using GameEngine.engine.components;

namespace GameEngine.components
{
    public class TimeDisplay : AbstractComponent
    {
        private UITextRendererComponent tr;
        private float time;
        private float frameTime = 0;
        private int fpsFrames = 0;
        private int fps = 0;
        protected override void Awake()
        {
            base.Awake();
            tr = Owner.GetComponent<UITextRendererComponent>();

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