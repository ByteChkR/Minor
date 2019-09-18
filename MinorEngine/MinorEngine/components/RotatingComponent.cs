using System;
using GameEngine.engine.components;
using OpenTK;

namespace GameEngine.components
{
    public class RotatingComponent : AbstractComponent
    {
        public override void Update(float deltaTime)
        {

            Owner.Rotate(new Vector3(1, 1, 0), (MathF.PI / 4) * deltaTime);

        }
    }
}