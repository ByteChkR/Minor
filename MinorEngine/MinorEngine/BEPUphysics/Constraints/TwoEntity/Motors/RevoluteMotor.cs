﻿using System;
using MinorEngine.BEPUphysics.Entities;
using MinorEngine.BEPUutilities;

namespace MinorEngine.BEPUphysics.Constraints.TwoEntity.Motors
{
    /// <summary>
    /// Tries to rotate two entities so that they reach a specified relative orientation or speed around an axis.
    /// </summary>
    public class RevoluteMotor : Motor, I1DImpulseConstraintWithError, I1DJacobianConstraint
    {
        protected float biasVelocity;
        private Vector3 jacobianA, jacobianB;

        private Vector3 localTestAxis;


        private Vector3 worldTestAxis;
        private float velocityToImpulse;

        /// <summary>
        /// Constructs a new constraint tries to rotate two entities so that they reach a specified relative orientation around an axis.
        /// To finish the initialization, specify the connections (ConnectionA and ConnectionB) 
        /// as well as the Basis and TestAxis.
        /// This constructor sets the constraint's IsActive property to false by default.
        /// </summary>
        public RevoluteMotor()
        {
            Settings = new MotorSettings1D(this);
            IsActive = false;
        }

        /// <summary>
        /// Constructs a new constraint tries to rotate two entities so that they reach a specified relative orientation around an axis.
        /// </summary>
        /// <param name="connectionA">First connection of the pair.</param>
        /// <param name="connectionB">Second connection of the pair.</param>
        /// <param name="motorizedAxis">Rotation axis to control in world space.</param>
        public RevoluteMotor(Entity connectionA, Entity connectionB, Vector3 motorizedAxis)
        {
            ConnectionA = connectionA;
            ConnectionB = connectionB;
            SetupJointTransforms(motorizedAxis);

            Settings = new MotorSettings1D(this);
        }

        /// <summary>
        /// Gets the basis attached to entity A.
        /// The primary axis represents the motorized axis of rotation.  The 'measurement plane' which the test axis is tested against is based on this primary axis.
        /// The x axis defines the 'base' direction on the measurement plane corresponding to 0 degrees of relative rotation.
        /// </summary>
        public JointBasis2D Basis { get; } = new JointBasis2D();

        /// <summary>
        /// Gets or sets the axis attached to entity B in its local space.
        /// This axis is projected onto the x and y axes of transformA to determine the hinge angle.
        /// </summary>
        public Vector3 LocalTestAxis
        {
            get => localTestAxis;
            set
            {
                localTestAxis = Vector3.Normalize(value);
                Matrix3x3.Transform(ref localTestAxis, ref connectionB.orientationMatrix, out worldTestAxis);
            }
        }

        /// <summary>
        /// Gets the motor's velocity and servo settings.
        /// </summary>
        public MotorSettings1D Settings { get; }

        /// <summary>
        /// Gets or sets the axis attached to entity B in world space.
        /// This axis is projected onto the x and y axes of the Basis attached to entity A to determine the hinge angle.
        /// </summary>
        public Vector3 TestAxis
        {
            get => worldTestAxis;
            set
            {
                worldTestAxis = Vector3.Normalize(value);
                Matrix3x3.TransformTranspose(ref worldTestAxis, ref connectionB.orientationMatrix, out localTestAxis);
            }
        }

        #region I1DImpulseConstraintWithError Members

        /// <summary>
        /// Gets the current relative velocity between the connected entities with respect to the constraint.
        /// </summary>
        public float RelativeVelocity
        {
            get
            {
                float velocityA, velocityB;
                Vector3.Dot(ref connectionA.angularVelocity, ref jacobianA, out velocityA);
                Vector3.Dot(ref connectionB.angularVelocity, ref jacobianB, out velocityB);
                return velocityA + velocityB;
            }
        }

        /// <summary>
        /// Gets the total impulse applied by this constraint.
        /// </summary>
        public float TotalImpulse { get; private set; }

        /// <summary>
        /// Gets the current constraint error.
        /// If the motor is in velocity only mode, the error is zero.
        /// </summary>
        public float Error { get; private set; }

        #endregion

        #region I1DJacobianConstraint Members

        /// <summary>
        /// Gets the linear jacobian entry for the first connected entity.
        /// </summary>
        /// <param name="jacobian">Linear jacobian entry for the first connected entity.</param>
        public void GetLinearJacobianA(out Vector3 jacobian)
        {
            jacobian = Toolbox.ZeroVector;
        }

        /// <summary>
        /// Gets the linear jacobian entry for the second connected entity.
        /// </summary>
        /// <param name="jacobian">Linear jacobian entry for the second connected entity.</param>
        public void GetLinearJacobianB(out Vector3 jacobian)
        {
            jacobian = Toolbox.ZeroVector;
        }

        /// <summary>
        /// Gets the angular jacobian entry for the first connected entity.
        /// </summary>
        /// <param name="jacobian">Angular jacobian entry for the first connected entity.</param>
        public void GetAngularJacobianA(out Vector3 jacobian)
        {
            jacobian = jacobianA;
        }

        /// <summary>
        /// Gets the angular jacobian entry for the second connected entity.
        /// </summary>
        /// <param name="jacobian">Angular jacobian entry for the second connected entity.</param>
        public void GetAngularJacobianB(out Vector3 jacobian)
        {
            jacobian = jacobianB;
        }

        /// <summary>
        /// Gets the mass matrix of the constraint.
        /// </summary>
        /// <param name="outputMassMatrix">Constraint's mass matrix.</param>
        public void GetMassMatrix(out float outputMassMatrix)
        {
            outputMassMatrix = velocityToImpulse;
        }

        #endregion

        /// <summary>
        /// Sets up the joint transforms by automatically creating perpendicular vectors to complete the bases.
        /// </summary>
        /// <param name="motorizedAxis">Axis around which the motor acts.</param>
        public void SetupJointTransforms(Vector3 motorizedAxis)
        {
            //Compute a vector which is perpendicular to the axis.  It'll be added in local space to both connections.
            Vector3 xAxis;
            Vector3.Cross(ref motorizedAxis, ref Toolbox.UpVector, out xAxis);
            var length = xAxis.LengthSquared();
            if (length < Toolbox.Epsilon)
            {
                Vector3.Cross(ref motorizedAxis, ref Toolbox.RightVector, out xAxis);
            }

            //Put the axes into the joint transform of A.
            Basis.rotationMatrix = connectionA.orientationMatrix;
            Basis.SetWorldAxes(motorizedAxis, xAxis);


            //Put the axes into the 'joint transform' of B too.
            TestAxis = Basis.xAxis;
        }

        ///<summary>
        /// Performs the frame's configuration step.
        ///</summary>
        ///<param name="dt">Timestep duration.</param>
        public override void Update(float dt)
        {
            //Transform the axes into world space.
            Basis.rotationMatrix = connectionA.orientationMatrix;
            Basis.ComputeWorldSpaceAxes();
            Matrix3x3.Transform(ref localTestAxis, ref connectionB.orientationMatrix, out worldTestAxis);

            var updateRate = 1 / dt;
            if (Settings.mode == MotorMode.Servomechanism)
            {
                float y, x;
                Vector3 yAxis;
                Vector3.Cross(ref Basis.primaryAxis, ref Basis.xAxis, out yAxis);
                Vector3.Dot(ref worldTestAxis, ref yAxis, out y);
                Vector3.Dot(ref worldTestAxis, ref Basis.xAxis, out x);
                var angle = (float) Math.Atan2(y, x);

                //****** VELOCITY BIAS ******//
                //Compute the correction velocity.
                Error = GetDistanceFromGoal(angle);


                var absErrorOverDt = Math.Abs(Error * updateRate);
                float errorReduction;
                Settings.servo.springSettings.ComputeErrorReductionAndSoftness(dt, updateRate, out errorReduction,
                    out usedSoftness);
                biasVelocity = Math.Sign(Error) * MathHelper.Min(Settings.servo.baseCorrectiveSpeed, absErrorOverDt) +
                               Error * errorReduction;

                biasVelocity = MathHelper.Clamp(biasVelocity, -Settings.servo.maxCorrectiveVelocity,
                    Settings.servo.maxCorrectiveVelocity);
            }
            else
            {
                biasVelocity = Settings.velocityMotor.goalVelocity;
                usedSoftness = Settings.velocityMotor.softness * updateRate;
                Error = 0;
            }


            //Compute the jacobians
            jacobianA = Basis.primaryAxis;
            jacobianB.X = -jacobianA.X;
            jacobianB.Y = -jacobianA.Y;
            jacobianB.Z = -jacobianA.Z;


            //****** EFFECTIVE MASS MATRIX ******//
            //Connection A's contribution to the mass matrix
            float entryA;
            Vector3 transformedAxis;
            if (connectionA.isDynamic)
            {
                Matrix3x3.Transform(ref jacobianA, ref connectionA.inertiaTensorInverse, out transformedAxis);
                Vector3.Dot(ref transformedAxis, ref jacobianA, out entryA);
            }
            else
            {
                entryA = 0;
            }

            //Connection B's contribution to the mass matrix
            float entryB;
            if (connectionB.isDynamic)
            {
                Matrix3x3.Transform(ref jacobianB, ref connectionB.inertiaTensorInverse, out transformedAxis);
                Vector3.Dot(ref transformedAxis, ref jacobianB, out entryB);
            }
            else
            {
                entryB = 0;
            }

            //Compute the inverse mass matrix
            velocityToImpulse = 1 / (usedSoftness + entryA + entryB);


            //Update the maximum force
            ComputeMaxForces(Settings.maximumForce, dt);
        }

        /// <summary>
        /// Performs any pre-solve iteration work that needs exclusive
        /// access to the members of the solver updateable.
        /// Usually, this is used for applying warmstarting impulses.
        /// </summary>
        public override void ExclusiveUpdate()
        {
            //****** WARM STARTING ******//
            //Apply accumulated impulse
            Vector3 impulse;
            if (connectionA.isDynamic)
            {
                Vector3.Multiply(ref jacobianA, TotalImpulse, out impulse);
                connectionA.ApplyAngularImpulse(ref impulse);
            }

            if (connectionB.isDynamic)
            {
                Vector3.Multiply(ref jacobianB, TotalImpulse, out impulse);
                connectionB.ApplyAngularImpulse(ref impulse);
            }
        }

        /// <summary>
        /// Computes one iteration of the constraint to meet the solver updateable's goal.
        /// </summary>
        /// <returns>The rough applied impulse magnitude.</returns>
        public override float SolveIteration()
        {
            float velocityA, velocityB;
            //Find the velocity contribution from each connection
            Vector3.Dot(ref connectionA.angularVelocity, ref jacobianA, out velocityA);
            Vector3.Dot(ref connectionB.angularVelocity, ref jacobianB, out velocityB);
            //Add in the constraint space bias velocity
            var lambda = -(velocityA + velocityB) - biasVelocity - usedSoftness * TotalImpulse;

            //Transform to an impulse
            lambda *= velocityToImpulse;

            //Accumulate the impulse
            var previousAccumulatedImpulse = TotalImpulse;
            TotalImpulse = MathHelper.Clamp(TotalImpulse + lambda, -maxForceDt, maxForceDt);
            lambda = TotalImpulse - previousAccumulatedImpulse;

            //Apply the impulse
            Vector3 impulse;
            if (connectionA.isDynamic)
            {
                Vector3.Multiply(ref jacobianA, lambda, out impulse);
                connectionA.ApplyAngularImpulse(ref impulse);
            }

            if (connectionB.isDynamic)
            {
                Vector3.Multiply(ref jacobianB, lambda, out impulse);
                connectionB.ApplyAngularImpulse(ref impulse);
            }

            return Math.Abs(lambda);
        }


        private float GetDistanceFromGoal(float angle)
        {
            float forwardDistance;
            var goalAngle = MathHelper.WrapAngle(Settings.servo.goal);
            if (goalAngle > 0)
            {
                if (angle > goalAngle)
                {
                    forwardDistance = angle - goalAngle;
                }
                else if (angle > 0)
                {
                    forwardDistance = MathHelper.TwoPi - goalAngle + angle;
                }
                else //if (angle <= 0)
                {
                    forwardDistance = MathHelper.TwoPi - goalAngle + angle;
                }
            }
            else
            {
                if (angle < goalAngle)
                {
                    forwardDistance = MathHelper.TwoPi - goalAngle + angle;
                }
                else //if (angle < 0)
                {
                    forwardDistance = angle - goalAngle;
                }

                //else //if (currentAngle >= 0)
                //    return angle - myMinimumAngle;
            }

            return forwardDistance > MathHelper.Pi ? MathHelper.TwoPi - forwardDistance : -forwardDistance;
        }
    }
}