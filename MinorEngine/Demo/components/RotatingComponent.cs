using System;
using Engine.Core;
using OpenTK;

namespace Demo.components
{
    public class RotatingComponent : AbstractComponent
    {
        protected override void Update(float deltaTime)
        {
            Owner.Rotate(new Vector3(1, 1, 0), MathF.PI / 4 * deltaTime);
        }
    }
}