using MinorEngine.BEPUphysics;
using MinorEngine.BEPUphysics.Constraints.TwoEntity.Motors;
using MinorEngine.BEPUphysics.Entities;
using MinorEngine.BEPUphysics.Entities.Prefabs;
using MinorEngine.debug;
using MinorEngine.engine.components;
using MinorEngine.engine.physics;

namespace MinorEngine.components
{
    public class Collider : AbstractComponent
    {
        public Entity PhysicsCollider { get; set; }

        public Collider(Entity shape)
        {
            PhysicsCollider = shape;
        }

        protected override void Awake()
        {
            PhysicsCollider.Position = Owner.GetLocalPosition();
            PhysicsCollider.orientation = Owner.GetOrientation();
            Physics.AddEntity(PhysicsCollider);
        }

        protected override void OnDestroy()
        {
            Physics.RemoveEntity(PhysicsCollider);
        }

        protected override void Update(float deltaTime)
        {
            if(PhysicsCollider.IsDynamic)
            {
                //this.Log("Velocity: " + PhysicsCollider.LinearVelocity.Length(), DebugChannel.Log);
                Owner.SetLocalPosition(PhysicsCollider.Position);
                Owner.SetRotation(PhysicsCollider.Orientation);
            }
        }
    }
}