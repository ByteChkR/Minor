using System;
using MinorEngine.engine.components.ui;
using MinorEngine.engine.rendering;
using OpenTK;

namespace Demo.components
{
    public class UIMovingComponent : UIImageRendererComponent
    {
        public UIMovingComponent(GameTexture texture, bool worldSpace, float alpha, ShaderProgram shader) : base(
            texture, worldSpace, alpha, shader)
        {
        }

        private float time;

        protected override void Awake()
        {
            base.Awake();
        }

        protected override void Update(float deltaTime)
        {
            time += deltaTime;


            Position = new Vector2(MathF.Sin(time * 2), MathF.Cos(time * 2));
            var x = MathF.Abs(MathF.Sin(time * 2)) * 0.3f + 0.1f;
            var y = MathF.Abs(MathF.Cos(time * 2)) * 0.3f + 0.1f;
            Scale = new Vector2(x, y);
        }
    }
}