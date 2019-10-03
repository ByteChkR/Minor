using System;
using BepuPhysics;
using BepuPhysics.CollisionDetection;
using BepuPhysics.Constraints;
using BepuUtilities;
using Common;
using MinorEngine.engine.components;
using MinorEngine.engine.physics;
using OpenTK;
using Quaternion = BepuUtilities.Quaternion;

namespace MinorEngine.components
{
    public class RigidBodyComponent : AbstractComponent
    {
        private AbstractDynamicCollider Collider { get; }
        private RigidBodyConstraints Constraints { get; }

        public RigidBodyComponent(AbstractDynamicCollider dynamicCollider)
        {
            Collider = dynamicCollider;
            Constraints = new RigidBodyConstraints() { FixRotation = true};
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
    }
}