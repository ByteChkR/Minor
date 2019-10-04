using MinorEngine.BEPUphysics;
using MinorEngine.BEPUphysics.BroadPhaseEntries.Events;
using MinorEngine.BEPUphysics.Constraints.TwoEntity.Motors;
using MinorEngine.BEPUphysics.Entities;
using MinorEngine.BEPUphysics.Entities.Prefabs;
using MinorEngine.debug;
using MinorEngine.engine.components;
using MinorEngine.engine.physics;
using OpenTK;

namespace MinorEngine.components
{
    public class Collider : AbstractComponent
    {
        public Entity PhysicsCollider { get; }
        public RigidBodyConstraints ColliderConstraints { get; set; }

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

        private void enforceTranslationConstraints()
        {
            Vector3 veel = PhysicsCollider.LinearVelocity;
            if ((ColliderConstraints.PositionConstraints & FreezeConstraints.X) != 0)
            {
                veel.X = 0;
            }
            else if ((ColliderConstraints.PositionConstraints & FreezeConstraints.Y) != 0)
            {
                veel.Y = 0;
            }
            else if ((ColliderConstraints.PositionConstraints & FreezeConstraints.Z) != 0)
            {
                veel.Y = 0;
            }

            PhysicsCollider.LinearVelocity = veel;
        }

        private void enforceRotationConstraints()
        {
            Vector3 veel = PhysicsCollider.AngularVelocity;
            if ((ColliderConstraints.RotationConstraints & FreezeConstraints.X) != 0)
            {
                veel.X = 0;
            }
            else if ((ColliderConstraints.RotationConstraints & FreezeConstraints.Y) != 0)
            {
                veel.Y = 0;
            }
            else if ((ColliderConstraints.RotationConstraints & FreezeConstraints.Z) != 0)
            {
                veel.Y = 0;
            }

            PhysicsCollider.AngularVelocity = veel;
        }

        protected override void Update(float deltaTime)
        {
            if (PhysicsCollider.IsDynamic)
            {
                enforceRotationConstraints();
                enforceTranslationConstraints();
                //this.Log("Velocity: " + PhysicsCollider.LinearVelocity.Length(), DebugChannel.Log);
                Owner.SetLocalPosition(PhysicsCollider.Position);
                Owner.SetRotation(PhysicsCollider.Orientation);
            }
        }
    }
}