using System;
using GameEngine.engine.ui;
using MinorEngine.engine.components;
using OpenTK;

namespace MinorEngine.components
{
    public class UIMovingComponent : AbstractComponent
    {
        private UIRendererComponent comp;
       

        private float time;
        public override void Update(float deltaTime)
        {
            time += deltaTime;
            if (comp != null)
            {

                comp.Position = new Vector2(MathF.Sin(time * 2), MathF.Cos(time * 2));
                float x = MathF.Abs(MathF.Sin(time * 2)) * 0.3f + 0.1f;
                float y = MathF.Abs(MathF.Cos(time * 2)) * 0.3f + 0.1f;
                comp.Scale = new Vector2(x, y);


            }
            else
            {
                comp = Owner.GetComponent<UIRendererComponent>();
            }


        }
    }
}