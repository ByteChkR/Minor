using System;
using MinorEngine.engine.components;
using OpenTK;

namespace MinorEngine.components
{
    public class RotateAroundComponent : AbstractComponent
    {
        public override void Update(float deltaTime)
        {
            Vector4 pos = new Vector4(Owner.GetLocalPosition());
            pos *= Matrix4.CreateFromAxisAngle(new Vector3(0, 1, 0), (MathF.PI / 4) * deltaTime);
            Owner.SetLocalPosition(new Vector3(pos));
            


        }
    }
}