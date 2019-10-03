using System;
using GameEngine.engine.ui;
using GameEngine.engine.components;
using OpenTK;

namespace Demo.components
{
    public class UIMovingComponent : AbstractComponent
    {
        private UIRendererComponent comp;


        private float time;

        protected override void Awake()
        {
            base.Awake();

            comp = Owner.GetComponent<UIRendererComponent>();
        }

        protected override void Update(float deltaTime)
        {
            time += deltaTime;


            comp.Position = new Vector2(MathF.Sin(time * 2), MathF.Cos(time * 2));
            float x = MathF.Abs(MathF.Sin(time * 2)) * 0.3f + 0.1f;
            float y = MathF.Abs(MathF.Cos(time * 2)) * 0.3f + 0.1f;
            comp.Scale = new Vector2(x, y);



        }
    }
}