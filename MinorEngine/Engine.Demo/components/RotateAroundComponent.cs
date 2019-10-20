using System;
using Engine.Core;
using OpenTK;

namespace Engine.Demo.components
{
    public class RotateAroundComponent : AbstractComponent
    {
        public float Slow = 1f;
        protected override void Update(float deltaTime)
        {
            Vector4 pos = new Vector4(Owner.GetLocalPosition());
            pos *= Matrix4.CreateFromAxisAngle(new Vector3(0, 1, 0), MathF.PI / 4 * deltaTime* Slow);
            //Owner.Rotate(new Vector3(0, 1, 0), (MathF.PI / 4) * deltaTime);
            Owner.SetLocalPosition(new Vector3(pos));
        }
    }
}