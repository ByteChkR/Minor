using BepuPhysics;

namespace MinorEngine.engine.physics
{
    public struct RigidBodyConstraints
    {
        public bool FixRotation;
        public FreezeConstraints PositionConstraints;


        public static BodyInertia ComputeRotationFreeze(RigidBodyConstraints constraints, BodyInertia current)
        {
            if (constraints.FixRotation) current.InverseInertiaTensor = new Symmetric3x3();
            return current;
        }

        public static BodyVelocity ComputeTranslationFreeze(RigidBodyConstraints constraints, BodyVelocity bin)
        {
            System.Numerics.Vector3 dir = bin.Linear;
            if ((constraints.PositionConstraints & FreezeConstraints.X) != 0)
                dir.X = 0;
            else if ((constraints.PositionConstraints & FreezeConstraints.Y) != 0)
                dir.Y = 0;
            else if ((constraints.PositionConstraints & FreezeConstraints.Z) != 0) dir.Z = 0;

            bin.Linear = dir;

            return bin;
        }
    }
}