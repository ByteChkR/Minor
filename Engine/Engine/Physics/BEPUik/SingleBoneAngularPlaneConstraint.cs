using Engine.Physics.BEPUutilities;

namespace Engine.Physics.BEPUik
{
    public class SingleBoneAngularPlaneConstraint : SingleBoneConstraint
    {
        /// <summary>
        /// Axis to constrain to the plane in the bone's local space.
        /// </summary>
        public Vector3 BoneLocalAxis;

        /// <summary>
        /// Gets or sets normal of the plane which the bone's axis will be constrained to..
        /// </summary>
        public Vector3 PlaneNormal;

        protected internal override void UpdateJacobiansAndVelocityBias()
        {
            linearJacobian = new Matrix3x3();

            Vector3 boneAxis;
            Quaternion.Transform(ref BoneLocalAxis, ref TargetBone.Orientation, out boneAxis);

            Vector3 jacobian;
            Vector3.Cross(ref boneAxis, ref PlaneNormal, out jacobian);

            angularJacobian = new Matrix3x3
            {
                M11 = jacobian.X,
                M12 = jacobian.Y,
                M13 = jacobian.Z
            };


            Vector3.Dot(ref boneAxis, ref PlaneNormal, out velocityBias.X);
            velocityBias.X = -errorCorrectionFactor * velocityBias.X;
        }
    }
}