using Engine.Physics.BEPUphysics.Constraints;
using Engine.Physics.BEPUphysics.Entities;
using Engine.Physics.BEPUutilities;

namespace Engine.Physics.BEPUphysics.Vehicle
{
    /// <summary>
    /// Allows the connected wheel and vehicle to smoothly absorb bumps.
    /// </summary>
    public class WheelSuspension : ISpringSettings, ISolverSettings
    {
        internal float accumulatedImpulse;

        //float linearBX, linearBY, linearBZ;
        private float angularAX, angularAY, angularAZ;
        private float angularBX, angularBY, angularBZ;
        private float bias;

        internal bool isActive = true;
        private float linearAX, linearAY, linearAZ;
        private float allowedCompression = .01f;
        internal float currentLength;
        internal Vector3 localAttachmentPoint;
        internal Vector3 localDirection;
        private float maximumSpringCorrectionSpeed = float.MaxValue;
        private float maximumSpringForce = float.MaxValue;
        internal float restLength;
        internal SolverSettings solverSettings = new SolverSettings();
        internal Vector3 worldAttachmentPoint;
        internal Vector3 worldDirection;
        internal int numIterationsAtZeroImpulse;
        private Entity vehicleEntity, supportEntity;
        private float softness;

        //Inverse effective mass matrix
        private float velocityToImpulse;
        private bool supportIsDynamic;

        /// <summary>
        /// Constructs a new suspension for a wheel.
        /// </summary>
        /// <param name="stiffnessConstant">Strength of the spring.  Higher values resist compression more.</param>
        /// <param name="dampingConstant">Damping constant of the spring.  Higher values remove more momentum.</param>
        /// <param name="localDirection">Direction of the suspension in the vehicle's local space.  For a normal, straight down suspension, this would be (0, -1, 0).</param>
        /// <param name="restLength">Length of the suspension when uncompressed.</param>
        /// <param name="localAttachmentPoint">Place where the suspension hooks up to the body of the vehicle.</param>
        public WheelSuspension(float stiffnessConstant, float dampingConstant, Vector3 localDirection, float restLength,
            Vector3 localAttachmentPoint)
        {
            SpringSettings.Stiffness = stiffnessConstant;
            SpringSettings.Damping = dampingConstant;
            LocalDirection = localDirection;
            RestLength = restLength;
            LocalAttachmentPoint = localAttachmentPoint;
        }

        internal WheelSuspension(Wheel wheel)
        {
            Wheel = wheel;
        }

        /// <summary>
        /// Gets or sets the allowed compression of the suspension before suspension forces take effect.
        /// Usually a very small number.  Used to prevent 'jitter' where the wheel leaves the ground due to spring forces repeatedly.
        /// </summary>
        public float AllowedCompression
        {
            get => allowedCompression;
            set => allowedCompression = MathHelper.Max(0, value);
        }

        /// <summary>
        /// Gets the the current length of the suspension.
        /// This will be less than the RestLength if the suspension is compressed.
        /// </summary>
        public float CurrentLength => currentLength;

        /// <summary>
        /// Gets or sets the attachment point of the suspension to the vehicle body in the body's local space.
        /// </summary>
        public Vector3 LocalAttachmentPoint
        {
            get => localAttachmentPoint;
            set
            {
                localAttachmentPoint = value;
                if (Wheel != null && Wheel.vehicle != null)
                {
                    RigidTransform.Transform(ref localAttachmentPoint,
                        ref Wheel.vehicle.Body.CollisionInformation.worldTransform, out worldAttachmentPoint);
                }
                else
                {
                    worldAttachmentPoint = localAttachmentPoint;
                }
            }
        }


        /// <summary>
        /// Gets or sets the maximum speed at which the suspension will try to return the suspension to rest length.
        /// </summary>
        public float MaximumSpringCorrectionSpeed
        {
            get => maximumSpringCorrectionSpeed;
            set => maximumSpringCorrectionSpeed = MathHelper.Max(0, value);
        }

        /// <summary>
        /// Gets or sets the maximum force that can be applied by this suspension.
        /// </summary>
        public float MaximumSpringForce
        {
            get => maximumSpringForce;
            set => maximumSpringForce = MathHelper.Max(0, value);
        }

        /// <summary>
        /// Gets or sets the length of the uncompressed suspension.
        /// </summary>
        public float RestLength
        {
            get => restLength;
            set
            {
                restLength = value;
                if (Wheel != null)
                {
                    Wheel.shape.Initialize();
                }
            }
        }

        /// <summary>
        /// Gets the force that the suspension is applying to support the vehicle.
        /// </summary>
        public float TotalImpulse => -accumulatedImpulse;

        /// <summary>
        /// Gets the wheel that this suspension applies to.
        /// </summary>
        public Wheel Wheel { get; internal set; }

        /// <summary>
        /// Gets or sets the attachment point of the suspension to the vehicle body in world space.
        /// </summary>
        public Vector3 WorldAttachmentPoint
        {
            get => worldAttachmentPoint;
            set
            {
                worldAttachmentPoint = value;
                if (Wheel != null && Wheel.vehicle != null)
                {
                    RigidTransform.TransformByInverse(ref worldAttachmentPoint,
                        ref Wheel.vehicle.Body.CollisionInformation.worldTransform, out localAttachmentPoint);
                }
                else
                {
                    localAttachmentPoint = worldAttachmentPoint;
                }
            }
        }

        /// <summary>
        /// Gets or sets the direction of the wheel suspension in the local space of the vehicle body.
        /// A normal, straight suspension would be (0,-1,0).
        /// </summary>
        public Vector3 LocalDirection
        {
            get => localDirection;
            set
            {
                localDirection = Vector3.Normalize(value);
                if (Wheel != null)
                {
                    Wheel.shape.Initialize();
                }

                if (Wheel != null && Wheel.vehicle != null)
                {
                    Matrix3x3.Transform(ref localDirection, ref Wheel.vehicle.Body.orientationMatrix,
                        out worldDirection);
                }
                else
                {
                    worldDirection = localDirection;
                }
            }
        }

        /// <summary>
        /// Gets or sets the direction of the wheel suspension in the world space of the vehicle body.
        /// </summary>
        public Vector3 WorldDirection
        {
            get => worldDirection;
            set
            {
                worldDirection = Vector3.Normalize(value);
                if (Wheel != null)
                {
                    Wheel.shape.Initialize();
                }

                if (Wheel != null && Wheel.vehicle != null)
                {
                    Matrix3x3.TransformTranspose(ref worldDirection, ref Wheel.Vehicle.Body.orientationMatrix,
                        out localDirection);
                }
                else
                {
                    localDirection = worldDirection;
                }
            }
        }

        #region ISolverSettings Members

        /// <summary>
        /// Gets the solver settings used by this wheel constraint.
        /// </summary>
        public SolverSettings SolverSettings => solverSettings;

        #endregion

        #region ISpringSettings Members

        /// <summary>
        /// Gets the spring settings that define the behavior of the suspension.
        /// </summary>
        public SpringSettings SpringSettings { get; } = new SpringSettings();

        #endregion


        ///<summary>
        /// Gets the relative velocity along the support normal at the contact point.
        ///</summary>
        public float RelativeVelocity
        {
            get
            {
                var velocity = vehicleEntity.linearVelocity.X * linearAX + vehicleEntity.linearVelocity.Y * linearAY +
                               vehicleEntity.linearVelocity.Z * linearAZ +
                               vehicleEntity.angularVelocity.X * angularAX +
                               vehicleEntity.angularVelocity.Y * angularAY +
                               vehicleEntity.angularVelocity.Z * angularAZ;
                if (supportEntity != null)
                {
                    velocity += -supportEntity.linearVelocity.X * linearAX - supportEntity.linearVelocity.Y * linearAY -
                                supportEntity.linearVelocity.Z * linearAZ +
                                supportEntity.angularVelocity.X * angularBX +
                                supportEntity.angularVelocity.Y * angularBY +
                                supportEntity.angularVelocity.Z * angularBZ;
                }

                return velocity;
            }
        }

        internal float ApplyImpulse()
        {
            //Compute relative velocity
            var lambda = (RelativeVelocity
                          + bias //Add in position correction
                          + softness * accumulatedImpulse) //Add in squishiness
                         * velocityToImpulse; //convert to impulse


            //Clamp accumulated impulse
            var previousAccumulatedImpulse = accumulatedImpulse;
            accumulatedImpulse = MathHelper.Clamp(accumulatedImpulse + lambda, -maximumSpringForce, 0);
            lambda = accumulatedImpulse - previousAccumulatedImpulse;
            
            var linear = new Vector3();
            var angular = new Vector3();
            linear.X = lambda * linearAX;
            linear.Y = lambda * linearAY;
            linear.Z = lambda * linearAZ;
            if (vehicleEntity.isDynamic)
            {
                angular.X = lambda * angularAX;
                angular.Y = lambda * angularAY;
                angular.Z = lambda * angularAZ;
                vehicleEntity.ApplyLinearImpulse(ref linear);
                vehicleEntity.ApplyAngularImpulse(ref angular);
            }

            if (supportIsDynamic)
            {
                linear.X = -linear.X;
                linear.Y = -linear.Y;
                linear.Z = -linear.Z;
                angular.X = lambda * angularBX;
                angular.Y = lambda * angularBY;
                angular.Z = lambda * angularBZ;
                supportEntity.ApplyLinearImpulse(ref linear);
                supportEntity.ApplyAngularImpulse(ref angular);
            }

            return lambda;
        }

        internal void ComputeWorldSpaceData()
        {
            //Transform local space vectors to world space.
            RigidTransform.Transform(ref localAttachmentPoint,
                ref Wheel.vehicle.Body.CollisionInformation.worldTransform, out worldAttachmentPoint);
            Matrix3x3.Transform(ref localDirection, ref Wheel.vehicle.Body.orientationMatrix, out worldDirection);
        }

        internal void OnAdditionToVehicle()
        {
            //This looks weird, but it's just re-setting the world locations.
            //If the wheel doesn't belong to a vehicle (or this doesn't belong to a wheel)
            //then the world space location can't be set.
            LocalDirection = LocalDirection;
            LocalAttachmentPoint = LocalAttachmentPoint;
        }

        internal void PreStep(float dt)
        {
            vehicleEntity = Wheel.vehicle.Body;
            supportEntity = Wheel.supportingEntity;
            supportIsDynamic = supportEntity != null && supportEntity.isDynamic;

            //The next line is commented out because the world direction is computed by the wheelshape.  Weird, but necessary.
            //Vector3.TransformNormal(ref myLocalDirection, ref parentA.myInternalOrientationMatrix, out myWorldDirection);

            //Set up the jacobians.
            linearAX = -Wheel.normal.X; //myWorldDirection.X;
            linearAY = -Wheel.normal.Y; //myWorldDirection.Y;
            linearAZ = -Wheel.normal.Z; // myWorldDirection.Z;
            //linearBX = -linearAX;
            //linearBY = -linearAY;
            //linearBZ = -linearAZ;

            //angular A = Ra x N
            angularAX = Wheel.ra.Y * linearAZ - Wheel.ra.Z * linearAY;
            angularAY = Wheel.ra.Z * linearAX - Wheel.ra.X * linearAZ;
            angularAZ = Wheel.ra.X * linearAY - Wheel.ra.Y * linearAX;

            //Angular B = N x Rb
            angularBX = linearAY * Wheel.rb.Z - linearAZ * Wheel.rb.Y;
            angularBY = linearAZ * Wheel.rb.X - linearAX * Wheel.rb.Z;
            angularBZ = linearAX * Wheel.rb.Y - linearAY * Wheel.rb.X;

            //Compute inverse effective mass matrix
            float entryA, entryB;

            //these are the transformed coordinates
            float tX, tY, tZ;
            if (vehicleEntity.isDynamic)
            {
                tX = angularAX * vehicleEntity.inertiaTensorInverse.M11 +
                     angularAY * vehicleEntity.inertiaTensorInverse.M21 +
                     angularAZ * vehicleEntity.inertiaTensorInverse.M31;
                tY = angularAX * vehicleEntity.inertiaTensorInverse.M12 +
                     angularAY * vehicleEntity.inertiaTensorInverse.M22 +
                     angularAZ * vehicleEntity.inertiaTensorInverse.M32;
                tZ = angularAX * vehicleEntity.inertiaTensorInverse.M13 +
                     angularAY * vehicleEntity.inertiaTensorInverse.M23 +
                     angularAZ * vehicleEntity.inertiaTensorInverse.M33;
                entryA = tX * angularAX + tY * angularAY + tZ * angularAZ + vehicleEntity.inverseMass;
            }
            else
            {
                entryA = 0;
            }

            if (supportIsDynamic)
            {
                tX = angularBX * supportEntity.inertiaTensorInverse.M11 +
                     angularBY * supportEntity.inertiaTensorInverse.M21 +
                     angularBZ * supportEntity.inertiaTensorInverse.M31;
                tY = angularBX * supportEntity.inertiaTensorInverse.M12 +
                     angularBY * supportEntity.inertiaTensorInverse.M22 +
                     angularBZ * supportEntity.inertiaTensorInverse.M32;
                tZ = angularBX * supportEntity.inertiaTensorInverse.M13 +
                     angularBY * supportEntity.inertiaTensorInverse.M23 +
                     angularBZ * supportEntity.inertiaTensorInverse.M33;
                entryB = tX * angularBX + tY * angularBY + tZ * angularBZ + supportEntity.inverseMass;
            }
            else
            {
                entryB = 0;
            }

            //Convert spring constant and damping constant into ERP and CFM.
            float biasFactor;
            SpringSettings.ComputeErrorReductionAndSoftness(dt, 1 / dt, out biasFactor, out softness);

            velocityToImpulse = -1 / (entryA + entryB + softness);

            //Correction velocity
            bias = MathHelper.Min(MathHelper.Max(0, restLength - currentLength - allowedCompression) * biasFactor,
                maximumSpringCorrectionSpeed);
        }

        internal void ExclusiveUpdate()
        {
            //Warm starting
            var linear = new Vector3();
            var angular = new Vector3();
            linear.X = accumulatedImpulse * linearAX;
            linear.Y = accumulatedImpulse * linearAY;
            linear.Z = accumulatedImpulse * linearAZ;
            if (vehicleEntity.isDynamic)
            {
                angular.X = accumulatedImpulse * angularAX;
                angular.Y = accumulatedImpulse * angularAY;
                angular.Z = accumulatedImpulse * angularAZ;
                vehicleEntity.ApplyLinearImpulse(ref linear);
                vehicleEntity.ApplyAngularImpulse(ref angular);
            }

            if (supportIsDynamic)
            {
                linear.X = -linear.X;
                linear.Y = -linear.Y;
                linear.Z = -linear.Z;
                angular.X = accumulatedImpulse * angularBX;
                angular.Y = accumulatedImpulse * angularBY;
                angular.Z = accumulatedImpulse * angularBZ;
                supportEntity.ApplyLinearImpulse(ref linear);
                supportEntity.ApplyAngularImpulse(ref angular);
            }
        }
    }
}