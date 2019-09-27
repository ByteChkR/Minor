using BepuPhysics;
using GameEngine.engine.physics;
using MinorEngine.engine.components;
using OpenTK;

namespace MinorEngine.components
{
    public enum ColliderType
    {
        SPHERE = 0,
        BOX = 1
    }
    public class ColliderComponent : AbstractComponent
    {
        private BodyReference br;
        private bool _init;
        private ColliderType type;
        private float radius;
        public ColliderComponent(ColliderType type, float radius)
        {
            this.type = type;
            this.radius = radius;
        }
        public override void Update(float deltaTime)
        {
            if (!_init)
            {
                Vector3 pos = Owner.GetLocalPosition();
                if (type == ColliderType.SPHERE)
                {
                    br = SimpleSelfContainedDemo.AddSphereDynamic(1f, new System.Numerics.Vector3(pos.X, pos.Y, pos.Z), radius);
                }
                else
                {
                    br = SimpleSelfContainedDemo.AddBoxDynamic(1f, new System.Numerics.Vector3(pos.X, pos.Y, pos.Z), new System.Numerics.Vector3(radius));
                }

                _init = true;
            }
            Owner.SetLocalPosition(new Vector3(br.Pose.Position.X, br.Pose.Position.Y, br.Pose.Position.Z));
            Owner.SetRotation(br.Pose.Orientation);
        }
    }
}