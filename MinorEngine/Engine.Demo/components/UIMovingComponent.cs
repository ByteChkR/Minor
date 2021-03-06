﻿using System;
using Engine.DataTypes;
using Engine.Rendering;
using Engine.UI;
using OpenTK;

namespace Engine.Demo.components
{
    public class UIMovingComponent : UIImageRendererComponent
    {
        public UIMovingComponent(Texture texture, bool worldSpace, float alpha, ShaderProgram shader) : base(
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
            float x = MathF.Abs(MathF.Sin(time * 2)) * 0.3f + 0.1f;
            float y = MathF.Abs(MathF.Cos(time * 2)) * 0.3f + 0.1f;
            Scale = new Vector2(x, y);
        }
    }
}