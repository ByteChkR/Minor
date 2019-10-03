using BepuPhysics;
using MinorEngine.engine.components;
using MinorEngine.engine.physics;
using OpenTK;

namespace MinorEngine.components
{
    public class RigidBodyComponent : AbstractComponent
    {
        private AbstractDynamicCollider Collider { get; }
        private RigidBodyConstraints Constraints { get; }

        public RigidBodyComponent(AbstractDynamicCollider dynamicCollider)
        {
            Collider = dynamicCollider;
            Constraints = new RigidBodyConstraints { FixRotation = true };
        }

        protected override void Update(float deltaTime)
        {
            BodyReference bref = Collider.BodyReference;
            bref.SetLocalInertia(RigidBodyConstraints.ComputeRotationFreeze(Constraints, bref.LocalInertia));
            bref.Velocity = RigidBodyConstraints.ComputeTranslationFreeze(Constraints, bref.Velocity);

            Owner.SetLocalPosition(new Vector3(Collider.BodyReference.Pose.Position.X,
                Collider.BodyReference.Pose.Position.Y, Collider.BodyReference.Pose.Position.Z));
            Owner.SetRotation(bref.Pose.Orientation);
        }

        public void SetVelocityLinear(Vector3 newVelocity)
        {
            Collider.BodyReference.Velocity.Linear = new System.Numerics.Vector3(newVelocity.X, newVelocity.Y, newVelocity.Z);
        }

        public void SetVelocityAngular(Vector3 newVelocity)
        {
            Collider.BodyReference.Velocity.Angular = new System.Numerics.Vector3(newVelocity.X, newVelocity.Y,
                newVelocity.Z);
        }
    }
}