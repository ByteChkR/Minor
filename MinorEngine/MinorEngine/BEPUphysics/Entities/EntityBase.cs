﻿using System;
using MinorEngine.BEPUphysics.BroadPhaseEntries;
using MinorEngine.BEPUphysics.BroadPhaseEntries.MobileCollidables;
using MinorEngine.BEPUphysics.BroadPhaseSystems;
using MinorEngine.BEPUphysics.CollisionRuleManagement;
using MinorEngine.BEPUphysics.CollisionShapes;
using MinorEngine.BEPUphysics.CollisionShapes.ConvexShapes;
using MinorEngine.BEPUphysics.DeactivationManagement;
using MinorEngine.BEPUphysics.EntityStateManagement;
using MinorEngine.BEPUphysics.Materials;
using MinorEngine.BEPUphysics.OtherSpaceStages;
using MinorEngine.BEPUphysics.PositionUpdating;
using MinorEngine.BEPUphysics.Settings;
using MinorEngine.BEPUutilities;

namespace MinorEngine.BEPUphysics.Entities
{
    ///<summary>
    /// Superclass of movable rigid bodies.  Contains information for
    /// both dynamic and kinematic simulation.
    ///</summary>
    public class Entity :
        IBroadPhaseEntryOwner,
        IDeferredEventCreatorOwner,
        ISimulationIslandMemberOwner,
        ICCDPositionUpdateable,
        IForceUpdateable,
        ISpaceObject,
        IMaterialOwner,
        ICollisionRulesOwner,
        IEquatable<Entity>
    {
        internal Vector3 position;
        internal Quaternion orientation = Quaternion.Identity;
        internal Matrix3x3 orientationMatrix = Matrix3x3.Identity;
        internal Vector3 linearVelocity;
        internal Vector3 angularVelocity;
#if CONSERVE
        internal Vector3 angularMomentum;
#endif
        internal bool isDynamic;


        ///<summary>
        /// Gets or sets the position of the Entity.  This Position acts
        /// as the center of mass for dynamic entities.
        ///</summary>
        public Vector3 Position
        {
            get => position;
            set
            {
                position = value;
                activityInformation.Activate();

                position.Validate();
            }
        }

        ///<summary>
        /// Gets or sets the orientation quaternion of the entity.
        ///</summary>
        public Quaternion Orientation
        {
            get => orientation;
            set
            {
                Quaternion.Normalize(ref value, out orientation);
                Matrix3x3.CreateFromQuaternion(ref orientation, out orientationMatrix);
                //Update inertia tensors for consistency.
                Matrix3x3 multiplied;
                Matrix3x3.MultiplyTransposed(ref orientationMatrix, ref localInertiaTensorInverse, out multiplied);
                Matrix3x3.Multiply(ref multiplied, ref orientationMatrix, out inertiaTensorInverse);
                Matrix3x3.MultiplyTransposed(ref orientationMatrix, ref localInertiaTensor, out multiplied);
                Matrix3x3.Multiply(ref multiplied, ref orientationMatrix, out inertiaTensor);
                activityInformation.Activate();

                orientation.Validate();
            }
        }

        /// <summary>
        /// Gets or sets the orientation matrix of the entity.
        /// </summary>
        public Matrix3x3 OrientationMatrix
        {
            get => orientationMatrix;
            set
            {
                Quaternion.CreateFromRotationMatrix(ref value, out orientation);
                Orientation = orientation; //normalizes and sets.
            }
        }

        ///<summary>
        /// Gets or sets the world transform of the entity.
        /// The upper left 3x3 part is the Orientation, and the translation is the Position.
        /// When setting this property, ensure that the rotation matrix component does not include
        /// any scaling or shearing.
        ///</summary>
        public Matrix WorldTransform
        {
            get
            {
                Matrix worldTransform;
                Matrix3x3.ToMatrix4X4(ref orientationMatrix, out worldTransform);
                worldTransform.Translation = position;
                return worldTransform;
            }
            set
            {
                Quaternion.CreateFromRotationMatrix(ref value, out orientation);
                Orientation = orientation; //normalizes and sets.
                position = value.Translation;
                activityInformation.Activate();

                position.Validate();
            }
        }

        /// <summary>
        /// Gets or sets the angular velocity of the entity.
        /// </summary>
        public Vector3 AngularVelocity
        {
            get { return angularVelocity; }
            set
            {
                angularVelocity = value;
                angularVelocity.Validate();
#if CONSERVE
                Matrix3x3.Transform(ref value, ref inertiaTensor, out angularMomentum);
                MathChecker.Validate(angularMomentum);
#endif
                activityInformation.Activate();
            }
        }

        /// <summary>
        /// Gets or sets the angular momentum of the entity.
        /// </summary>
        public Vector3 AngularMomentum
        {
            get
            {
#if CONSERVE
                return angularMomentum;
#else
                Vector3 v;
                Matrix3x3.Transform(ref angularVelocity, ref inertiaTensor, out v);
                return v;
#endif
            }
            set
            {
#if CONSERVE
                angularMomentum = value;
                MathChecker.Validate(angularMomentum);
#else
                Matrix3x3.Transform(ref value, ref inertiaTensorInverse, out angularVelocity);
                activityInformation.Activate();
#endif
                angularVelocity.Validate();
            }
        }

        /// <summary>
        /// Gets or sets the linear velocity of the entity.
        /// </summary>
        public Vector3 LinearVelocity
        {
            get => linearVelocity;
            set
            {
                linearVelocity = value;
                activityInformation.Activate();

                linearVelocity.Validate();
            }
        }

        /// <summary>
        /// Gets or sets the linear momentum of the entity.
        /// </summary>
        public Vector3 LinearMomentum
        {
            get
            {
                Vector3 momentum;
                Vector3.Multiply(ref linearVelocity, mass, out momentum);
                return momentum;
            }
            set
            {
                Vector3.Multiply(ref value, inverseMass, out linearVelocity);
                activityInformation.Activate();

                linearVelocity.Validate();
            }
        }

        /// <summary>
        /// Gets or sets the position, orientation, linear velocity, and angular velocity of the entity.
        /// </summary>
        public MotionState MotionState
        {
            get
            {
                MotionState toReturn;
                toReturn.Position = position;
                toReturn.Orientation = orientation;
                toReturn.LinearVelocity = linearVelocity;
                toReturn.AngularVelocity = angularVelocity;
                return toReturn;
            }
            set
            {
                Position = value.Position;
                Orientation = value.Orientation;
                LinearVelocity = value.LinearVelocity;
                AngularVelocity = value.AngularVelocity;
            }
        }

        /// <summary>
        /// Gets whether or not the entity is dynamic.
        /// Dynamic entities have finite mass and respond
        /// to collisions.  Kinematic (non-dynamic) entities
        /// have infinite mass and inertia and will plow through anything.
        /// </summary>
        public bool IsDynamic => isDynamic;


        private bool hasPersonalGravity;
        private Vector3 personalGravity;

        ///<summary>
        /// Gets or sets the entity's personal gravity. If null, the ForceUpdater's gravity is used instead. Defaults to null.
        ///</summary>
        public Vector3? Gravity
        {
            get
            {
                if (hasPersonalGravity)
                {
                    return personalGravity;
                }

                return null;
            }
            set
            {
                if (value != null)
                {
                    if (!hasPersonalGravity || personalGravity != value.Value)
                        //Personal gravity either turned on or changed; wake up to handle it.
                    {
                        activityInformation.Activate();
                    }

                    hasPersonalGravity = true;
                    personalGravity = value.Value;
                }
                else
                {
                    if (hasPersonalGravity)
                        //Personal gravity turned off; wake up to handle it.
                    {
                        activityInformation.Activate();
                    }

                    hasPersonalGravity = false;
                    personalGravity = new Vector3();
                }
            }
        }

        ///<summary>
        /// Gets the buffered states of the entity.  If the Space.BufferedStates manager is enabled,
        /// this property provides access to the buffered and interpolated states of the entity.
        /// Buffered states are the most recent completed update values, while interpolated states are the previous values blended
        /// with the current frame's values.  Interpolated states are helpful when updating the engine with internal time stepping, 
        /// giving entity motion a smooth appearance even when updates aren't occurring consistently every frame.  
        /// Both are buffered for asynchronous access.
        ///</summary>
        public EntityBufferedStates BufferedStates { get; }

        internal Matrix3x3 inertiaTensorInverse;

        ///<summary>
        /// Gets the world space inertia tensor inverse of the entity.
        ///</summary>
        public Matrix3x3 InertiaTensorInverse => inertiaTensorInverse;

        internal Matrix3x3 inertiaTensor;

        ///<summary>
        /// Gets the world space inertia tensor of the entity.
        ///</summary>
        public Matrix3x3 InertiaTensor => inertiaTensor;

        internal Matrix3x3 localInertiaTensor;

        ///<summary>
        /// Gets or sets the local inertia tensor of the entity.
        ///</summary>
        public Matrix3x3 LocalInertiaTensor
        {
            get => localInertiaTensor;
            set
            {
                localInertiaTensor = value;
                Matrix3x3.AdaptiveInvert(ref localInertiaTensor, out localInertiaTensorInverse);
                Matrix3x3 multiplied;
                Matrix3x3.MultiplyTransposed(ref orientationMatrix, ref localInertiaTensorInverse, out multiplied);
                Matrix3x3.Multiply(ref multiplied, ref orientationMatrix, out inertiaTensorInverse);
                Matrix3x3.MultiplyTransposed(ref orientationMatrix, ref localInertiaTensor, out multiplied);
                Matrix3x3.Multiply(ref multiplied, ref orientationMatrix, out inertiaTensor);

                localInertiaTensor.Validate();
                localInertiaTensorInverse.Validate();
            }
        }

        internal Matrix3x3 localInertiaTensorInverse;

        /// <summary>
        /// Gets or sets the local inertia tensor inverse of the entity.
        /// </summary>
        public Matrix3x3 LocalInertiaTensorInverse
        {
            get => localInertiaTensorInverse;
            set
            {
                localInertiaTensorInverse = value;
                Matrix3x3.AdaptiveInvert(ref localInertiaTensorInverse, out localInertiaTensor);
                //Update the world space versions.
                Matrix3x3 multiplied;
                Matrix3x3.MultiplyTransposed(ref orientationMatrix, ref localInertiaTensorInverse, out multiplied);
                Matrix3x3.Multiply(ref multiplied, ref orientationMatrix, out inertiaTensorInverse);
                Matrix3x3.MultiplyTransposed(ref orientationMatrix, ref localInertiaTensor, out multiplied);
                Matrix3x3.Multiply(ref multiplied, ref orientationMatrix, out inertiaTensor);

                localInertiaTensor.Validate();
                localInertiaTensorInverse.Validate();
            }
        }

        internal float mass;

        ///<summary>
        /// Gets or sets the mass of the entity.  Setting this to an invalid value, such as a non-positive number, NaN, or infinity, makes the entity kinematic.
        /// Setting it to a valid positive number will also scale the inertia tensor if it was already dynamic, or force the calculation of a new inertia tensor
        /// if it was previously kinematic.
        ///</summary>
        public float Mass
        {
            get => mass;
            set
            {
                if (value <= 0 || float.IsNaN(value) || float.IsInfinity(value))
                {
                    BecomeKinematic();
                }
                else
                {
                    if (isDynamic)
                    {
                        //If it's already dynamic, then we don't need to recompute the inertia tensor.
                        //Instead, scale the one we have already.
                        Matrix3x3 newInertia;
                        Matrix3x3.Multiply(ref localInertiaTensor, value * inverseMass, out newInertia);
                        BecomeDynamic(value, newInertia);
                    }
                    else
                    {
                        BecomeDynamic(value);
                    }
                }
            }
        }

        internal float inverseMass;

        /// <summary>
        /// Gets or sets the inverse mass of the entity.
        /// </summary>
        public float InverseMass
        {
            get => inverseMass;
            set
            {
                if (value > 0)
                {
                    Mass = 1 / value;
                }
                else
                {
                    Mass = 0;
                }
            }
        }


        ///<summary>
        /// Fires when the entity's position and orientation is updated.
        ///</summary>
        public event Action<Entity> PositionUpdated;


        protected EntityCollidable collisionInformation;

        ///<summary>
        /// Gets the collidable used by the entity.
        ///</summary>
        public EntityCollidable CollisionInformation
        {
            get => collisionInformation;
            protected set
            {
                if (collisionInformation != null)
                {
                    collisionInformation.Shape.ShapeChanged -= shapeChangedDelegate;
                }

                collisionInformation = value;
                if (collisionInformation != null)
                {
                    collisionInformation.Shape.ShapeChanged += shapeChangedDelegate;
                }

                //Entity constructors do their own initialization when the collision information changes.
                //Might be able to condense it up here, but don't really need it right now.
                //ShapeChangedHandler(collisionInformation.shape);
            }
        }

        //protected internal object locker = new object();
        /////<summary>
        ///// Gets the synchronization object used by systems that need
        ///// exclusive access to the entity's properties.
        /////</summary>
        //public object Locker
        //{
        //    get
        //    {
        //        return locker;
        //    }
        //}

        protected internal SpinLock locker = new SpinLock();

        ///<summary>
        /// Gets the synchronization object used by systems that need
        /// exclusive access to the entity's properties.
        ///</summary>
        public SpinLock Locker => locker;

        internal Material material;

        //NOT thread safe due to material change pair update.
        ///<summary>
        /// Gets or sets the material used by the entity.
        ///</summary>
        public Material Material
        {
            get => material;
            set
            {
                if (material != null)
                {
                    material.MaterialChanged -= materialChangedDelegate;
                }

                material = value;
                if (material != null)
                {
                    material.MaterialChanged += materialChangedDelegate;
                }

                OnMaterialChanged(material);
            }
        }

        private Action<Material> materialChangedDelegate;

        private void OnMaterialChanged(Material newMaterial)
        {
            for (var i = 0; i < collisionInformation.pairs.Count; i++)
            {
                collisionInformation.pairs[i].UpdateMaterialProperties();
            }
        }


        ///<summary>
        /// Gets all the EntitySolverUpdateables associated with this entity.
        ///</summary>
        public EntitySolverUpdateableCollection SolverUpdateables =>
            new EntitySolverUpdateableCollection(activityInformation.connections);

        ///<summary>
        /// Gets the two-entity constraints associated with this entity (a subset of the solver updateables).
        ///</summary>
        public EntityConstraintCollection Constraints =>
            new EntityConstraintCollection(activityInformation.connections);

        #region Construction

        protected Entity()
        {
            InitializeId();

            BufferedStates = new EntityBufferedStates(this);

            material = new Material();
            materialChangedDelegate = OnMaterialChanged;
            material.MaterialChanged += materialChangedDelegate;

            shapeChangedDelegate = OnShapeChanged;

            activityInformation = new SimulationIslandMember(this);
        }

        ///<summary>
        /// Constructs a new kinematic entity.
        ///</summary>
        ///<param name="collisionInformation">Collidable to use with the entity.</param>
        public Entity(EntityCollidable collisionInformation)
            : this()
        {
            Initialize(collisionInformation);
        }

        ///<summary>
        /// Constructs a new entity.
        ///</summary>
        ///<param name="collisionInformation">Collidable to use with the entity.</param>
        ///<param name="mass">Mass of the entity. If positive, the entity will be dynamic. Otherwise, it will be kinematic.</param>
        public Entity(EntityCollidable collisionInformation, float mass)
            : this()
        {
            Initialize(collisionInformation, mass);
        }

        ///<summary>
        /// Constructs a new entity.
        ///</summary>
        ///<param name="collisionInformation">Collidable to use with the entity.</param>
        ///<param name="mass">Mass of the entity. If positive, the entity will be dynamic. Otherwise, it will be kinematic.</param>
        /// <param name="inertiaTensor">Inertia tensor of the entity. Only used for a dynamic entity.</param>
        public Entity(EntityCollidable collisionInformation, float mass, Matrix3x3 inertiaTensor)
            : this()
        {
            Initialize(collisionInformation, mass, inertiaTensor);
        }

        ///<summary>
        /// Constructs a new kinematic entity.
        ///</summary>
        ///<param name="shape">Shape to use with the entity.</param>
        public Entity(EntityShape shape)
            : this()
        {
            Initialize(shape.GetCollidableInstance());
        }

        ///<summary>
        /// Constructs a new entity.
        ///</summary>
        ///<param name="shape">Shape to use with the entity.</param>
        ///<param name="mass">Mass of the entity. If positive, the entity will be dynamic. Otherwise, it will be kinematic.</param>
        public Entity(EntityShape shape, float mass)
            : this()
        {
            Initialize(shape.GetCollidableInstance(), mass);
        }

        ///<summary>
        /// Constructs a new entity.
        ///</summary>
        ///<param name="shape">Shape to use with the entity.</param>
        ///<param name="mass">Mass of the entity. If positive, the entity will be dynamic. Otherwise, it will be kinematic.</param>
        /// <param name="inertiaTensor">Inertia tensor of the entity. Only used for a dynamic entity.</param>
        public Entity(EntityShape shape, float mass, Matrix3x3 inertiaTensor)
            : this()
        {
            Initialize(shape.GetCollidableInstance(), mass, inertiaTensor);
        }


        //These initialize methods make it easier to construct some Entity prefab types.
        protected internal void Initialize(EntityCollidable collisionInformation)
        {
            CollisionInformation = collisionInformation;
            BecomeKinematic();
            collisionInformation.Entity = this;
        }

        protected internal void Initialize(EntityCollidable collisionInformation, float mass)
        {
            CollisionInformation = collisionInformation;

            if (mass > 0)
            {
                BecomeDynamic(mass,
                    collisionInformation.Shape.VolumeDistribution * (mass * InertiaHelper.InertiaTensorScale));
            }
            else
            {
                BecomeKinematic();
            }

            collisionInformation.Entity = this;
        }

        protected internal void Initialize(EntityCollidable collisionInformation, float mass, Matrix3x3 inertiaTensor)
        {
            CollisionInformation = collisionInformation;

            if (mass > 0)
            {
                BecomeDynamic(mass, inertiaTensor);
            }
            else
            {
                BecomeKinematic();
            }

            collisionInformation.Entity = this;
        }

        #endregion

        #region IDeferredEventCreatorOwner Members

        IDeferredEventCreator IDeferredEventCreatorOwner.EventCreator => CollisionInformation.Events;

        #endregion

        internal SimulationIslandMember activityInformation;

        public SimulationIslandMember ActivityInformation => activityInformation;

        bool IForceUpdateable.IsActive => activityInformation.IsActive;

        bool IPositionUpdateable.IsActive => activityInformation.IsActive;


        ///<summary>
        /// Applies an impulse to the entity.
        ///</summary>
        ///<param name="location">Location to apply the impulse.</param>
        ///<param name="impulse">Impulse to apply.</param>
        public void ApplyImpulse(Vector3 location, Vector3 impulse)
        {
            ApplyImpulse(ref location, ref impulse);
        }

        ///<summary>
        /// Applies an impulse to the entity.
        ///</summary>
        ///<param name="location">Location to apply the impulse.</param>
        ///<param name="impulse">Impulse to apply.</param>
        public void ApplyImpulse(ref Vector3 location, ref Vector3 impulse)
        {
            if (isDynamic)
            {
                ApplyImpulseWithoutActivating(ref location, ref impulse);
                activityInformation.Activate();
            }
        }

        ///<summary>
        /// Applies an impulse to the entity without activating the entity.
        ///</summary>
        ///<param name="location">Location to apply the impulse.</param>
        ///<param name="impulse">Impulse to apply.</param>
        public void ApplyImpulseWithoutActivating(ref Vector3 location, ref Vector3 impulse)
        {
            ApplyLinearImpulse(ref impulse);
#if WINDOWS
            Vector3 positionDifference;
#else
            var positionDifference = new Vector3();
#endif
            positionDifference.X = location.X - position.X;
            positionDifference.Y = location.Y - position.Y;
            positionDifference.Z = location.Z - position.Z;

            Vector3 cross;
            Vector3.Cross(ref positionDifference, ref impulse, out cross);
            ApplyAngularImpulse(ref cross);
        }


        //These methods are very direct and quick.  They don't activate the object or anything.
        /// <summary>
        /// Applies a linear velocity change to the entity using the given impulse.
        /// This method does not wake up the object or perform any other nonessential operation;
        /// it is meant to be used for performance-sensitive constraint solving.
        /// Consider equivalently adding to the LinearMomentum property for convenience instead.
        /// </summary>
        /// <param name="impulse">Impulse to apply.</param>
        public void ApplyLinearImpulse(ref Vector3 impulse)
        {
            linearVelocity.X += impulse.X * inverseMass;
            linearVelocity.Y += impulse.Y * inverseMass;
            linearVelocity.Z += impulse.Z * inverseMass;
            linearVelocity.Validate();
        }

        /// <summary>
        /// Applies an angular velocity change to the entity using the given impulse.
        /// This method does not wake up the object or perform any other nonessential operation;
        /// it is meant to be used for performance-sensitive constraint solving.
        /// Consider equivalently adding to the AngularMomentum property for convenience instead.
        /// </summary>
        /// <param name="impulse">Impulse to apply.</param>
        public void ApplyAngularImpulse(ref Vector3 impulse)
        {
            //There's some room here for SIMD-friendliness.  However, since the phone doesn't accelerate non-XNA types, the matrix3x3 operations don't gain much.
#if CONSERVE
            angularMomentum.X += impulse.X;
            angularMomentum.Y += impulse.Y;
            angularMomentum.Z += impulse.Z;
            angularVelocity.X =
 angularMomentum.X * inertiaTensorInverse.M11 + angularMomentum.Y * inertiaTensorInverse.M21 + angularMomentum.Z * inertiaTensorInverse.M31;
            angularVelocity.Y =
 angularMomentum.X * inertiaTensorInverse.M12 + angularMomentum.Y * inertiaTensorInverse.M22 + angularMomentum.Z * inertiaTensorInverse.M32;
            angularVelocity.Z =
 angularMomentum.X * inertiaTensorInverse.M13 + angularMomentum.Y * inertiaTensorInverse.M23 + angularMomentum.Z * inertiaTensorInverse.M33;
            
            MathChecker.Validate(angularMomentum);
#else
            angularVelocity.X += impulse.X * inertiaTensorInverse.M11 + impulse.Y * inertiaTensorInverse.M21 +
                                 impulse.Z * inertiaTensorInverse.M31;
            angularVelocity.Y += impulse.X * inertiaTensorInverse.M12 + impulse.Y * inertiaTensorInverse.M22 +
                                 impulse.Z * inertiaTensorInverse.M32;
            angularVelocity.Z += impulse.X * inertiaTensorInverse.M13 + impulse.Y * inertiaTensorInverse.M23 +
                                 impulse.Z * inertiaTensorInverse.M33;
#endif

            angularVelocity.Validate();
        }

        /// <summary>
        /// Gets or sets whether or not to ignore shape changes.  When true, changing the entity's collision shape will not update the volume, density, or inertia tensor. 
        /// </summary>
        public bool IgnoreShapeChanges { get; set; }

        private Action<CollisionShape> shapeChangedDelegate;

        protected void OnShapeChanged(CollisionShape shape)
        {
            if (!IgnoreShapeChanges)
            {
                //When the shape changes, force the entity awake so that it performs any necessary updates.
                activityInformation.Activate();
                if (isDynamic)
                {
                    LocalInertiaTensor = collisionInformation.Shape.VolumeDistribution *
                                         (InertiaHelper.InertiaTensorScale * mass);
                }
                else
                {
                    LocalInertiaTensorInverse = new Matrix3x3();
                }
            }
        }


        //TODO: Include warnings about multithreading.  These modify things outside of the entity and use single-thread-only helpers.
        ///<summary>
        /// Forces the entity to become kinematic.  Kinematic entities have infinite mass and inertia.
        ///</summary>
        public void BecomeKinematic()
        {
            var previousState = isDynamic;
            isDynamic = false;
            LocalInertiaTensorInverse = new Matrix3x3();
            mass = 0;
            inverseMass = 0;

            //Notify simulation island of the change.
            if (previousState)
            {
                if (activityInformation.DeactivationManager != null)
                {
                    activityInformation.DeactivationManager.RemoveSimulationIslandFromMember(activityInformation);
                }

                if (((IForceUpdateable) this).ForceUpdater != null)
                {
                    ((IForceUpdateable) this).ForceUpdater.ForceUpdateableBecomingKinematic(this);
                }
            }

            //Change the collision group if it was using the default.
            if (collisionInformation.CollisionRules.Group == CollisionRules.DefaultDynamicCollisionGroup ||
                collisionInformation.CollisionRules.Group == null)
            {
                collisionInformation.CollisionRules.Group = CollisionRules.DefaultKinematicCollisionGroup;
            }

            activityInformation.Activate();

            //Preserve velocity and reinitialize momentum for new state.
            LinearVelocity = linearVelocity;
            AngularVelocity = angularVelocity;
        }


        ///<summary>
        /// Forces the entity to become dynamic.  Dynamic entities respond to collisions and have finite mass and inertia.
        ///</summary>
        ///<param name="mass">Mass to use for the entity.</param>
        public void BecomeDynamic(float mass)
        {
            BecomeDynamic(mass,
                collisionInformation.Shape.VolumeDistribution * (mass * InertiaHelper.InertiaTensorScale));
        }

        ///<summary>
        /// Forces the entity to become dynamic.  Dynamic entities respond to collisions and have finite mass and inertia.
        ///</summary>
        ///<param name="mass">Mass to use for the entity.</param>
        /// <param name="localInertiaTensor">Inertia tensor to use for the entity.</param>
        public void BecomeDynamic(float mass, Matrix3x3 localInertiaTensor)
        {
            if (mass <= 0 || float.IsInfinity(mass) || float.IsNaN(mass))
            {
                throw new InvalidOperationException("Cannot use a mass of " + mass +
                                                    " for a dynamic entity.  Consider using a kinematic entity instead.");
            }

            var previousState = isDynamic;
            isDynamic = true;
            LocalInertiaTensor = localInertiaTensor;
            this.mass = mass;
            inverseMass = 1 / mass;

            //Notify simulation island system of the change.
            if (!previousState)
            {
                if (activityInformation.DeactivationManager != null)
                {
                    activityInformation.DeactivationManager.AddSimulationIslandToMember(activityInformation);
                }

                if (((IForceUpdateable) this).ForceUpdater != null)
                {
                    ((IForceUpdateable) this).ForceUpdater.ForceUpdateableBecomingDynamic(this);
                }
            }

            //Change the group if it was using the defaults.
            if (collisionInformation.CollisionRules.Group == CollisionRules.DefaultKinematicCollisionGroup ||
                collisionInformation.CollisionRules.Group == null)
            {
                collisionInformation.CollisionRules.Group = CollisionRules.DefaultDynamicCollisionGroup;
            }

            activityInformation.Activate();


            //Preserve velocity and reinitialize momentum for new state.
            LinearVelocity = linearVelocity;
            AngularVelocity = angularVelocity;
        }


        void IForceUpdateable.UpdateForForces(float dt)
        {
            //Apply gravity.
            if (hasPersonalGravity)
            {
                Vector3 gravityDt;
                Vector3.Multiply(ref personalGravity, dt, out gravityDt);
                Vector3.Add(ref gravityDt, ref linearVelocity, out linearVelocity);
            }
            else
            {
                Vector3.Add(ref forceUpdater.gravityDt, ref linearVelocity, out linearVelocity);
            }

            //Boost damping at very low velocities.  This is a strong stabilizer; removes a ton of energy from the system.
            if (activityInformation.DeactivationManager.useStabilization && activityInformation.allowStabilization &&
                (activityInformation.isSlowing || activityInformation.velocityTimeBelowLimit >
                 activityInformation.DeactivationManager.lowVelocityTimeMinimum))
            {
                var energy = linearVelocity.LengthSquared() + angularVelocity.LengthSquared();
                if (energy < activityInformation.DeactivationManager.velocityLowerLimitSquared)
                {
                    var boost = 1 - (float) (Math.Sqrt(energy) /
                                             (2f * activityInformation.DeactivationManager.velocityLowerLimit));
                    ModifyAngularDamping(boost);
                    ModifyLinearDamping(boost);
                }
            }

            //Damping
            var linear = LinearDamping + linearDampingBoost;
            if (linear > 0)
            {
                Vector3.Multiply(ref linearVelocity, (float) Math.Pow(MathHelper.Clamp(1 - linear, 0, 1), dt),
                    out linearVelocity);
            }

            //When applying angular damping, the momentum or velocity is damped depending on the conservation setting.
            var angular = AngularDamping + angularDampingBoost;
            if (angular > 0)
            {
#if CONSERVE
                Vector3.Multiply(ref angularMomentum, (float)Math.Pow(MathHelper.Clamp(1 - angular, 0, 1), dt), out angularMomentum);
#else
                Vector3.Multiply(ref angularVelocity, (float) Math.Pow(MathHelper.Clamp(1 - angular, 0, 1), dt),
                    out angularVelocity);
#endif
            }

            linearDampingBoost = 0;
            angularDampingBoost = 0;

            //Update world inertia tensors.
            Matrix3x3 multiplied;
            Matrix3x3.MultiplyTransposed(ref orientationMatrix, ref localInertiaTensorInverse, out multiplied);
            Matrix3x3.Multiply(ref multiplied, ref orientationMatrix, out inertiaTensorInverse);
            Matrix3x3.MultiplyTransposed(ref orientationMatrix, ref localInertiaTensor, out multiplied);
            Matrix3x3.Multiply(ref multiplied, ref orientationMatrix, out inertiaTensor);

#if CONSERVE
            //Update angular velocity.
            //Note that this doesn't play nice with singular inertia tensors.
            //Locked tensors result in zero angular velocity.
            Matrix3x3.Transform(ref angularMomentum, ref inertiaTensorInverse, out angularVelocity);
            MathChecker.Validate(angularMomentum);
#endif
            linearVelocity.Validate();
            angularVelocity.Validate();
        }

        private ForceUpdater forceUpdater;

        ForceUpdater IForceUpdateable.ForceUpdater
        {
            get => forceUpdater;
            set => forceUpdater = value;
        }

        #region ISpaceObject

        Space ISpaceObject.Space
        {
            get => Space;
            set => Space = value;
        }

        ///<summary>
        /// Gets the space that owns the entity.
        ///</summary>
        public Space Space { get; private set; }


        void ISpaceObject.OnAdditionToSpace(Space newSpace)
        {
            OnAdditionToSpace(newSpace);
        }

        protected virtual void OnAdditionToSpace(Space newSpace)
        {
        }

        void ISpaceObject.OnRemovalFromSpace(Space oldSpace)
        {
            OnRemovalFromSpace(oldSpace);
        }

        protected virtual void OnRemovalFromSpace(Space oldSpace)
        {
        }

        #endregion


        #region ICCDPositionUpdateable

        PositionUpdater IPositionUpdateable.PositionUpdater { get; set; }

        private PositionUpdateMode positionUpdateMode = MotionSettings.DefaultPositionUpdateMode;

        ///<summary>
        /// Gets the position update mode of the entity.
        ///</summary>
        public PositionUpdateMode PositionUpdateMode
        {
            get => positionUpdateMode;
            set
            {
                var previous = positionUpdateMode;
                positionUpdateMode = value;
                //Notify our owner of the change, if needed.
                if (positionUpdateMode != previous &&
                    ((IPositionUpdateable) this).PositionUpdater != null &&
                    ((IPositionUpdateable) this).PositionUpdater as ContinuousPositionUpdater != null)
                {
                    (((IPositionUpdateable) this).PositionUpdater as ContinuousPositionUpdater).UpdateableModeChanged(
                        this, previous);
                }
            }
        }

        void ICCDPositionUpdateable.UpdateTimesOfImpact(float dt)
        {
            //I am a continuous object.  If I am in a pair with another object, even if I am inactive,
            //I must order the pairs to compute a time of impact.

            //The pair method works in such a way that, when this method is run asynchronously, there will be no race conditions.
            for (var i = 0; i < collisionInformation.pairs.Count; i++)
                //Only perform CCD if we're either supposed to test against no solver pairs or if this isn't a no solver pair.
            {
                if (MotionSettings.CCDFilter(collisionInformation.pairs.Elements[i]))
                {
                    collisionInformation.pairs.Elements[i].UpdateTimeOfImpact(collisionInformation, dt);
                }
            }
        }

        void ICCDPositionUpdateable.ResetTimesOfImpact()
        {
            //Reset all of the times of impact to 1, allowing the entity to move all the way through its velocity-defined motion.
            for (var i = 0; i < collisionInformation.pairs.Count; i++)
            {
                collisionInformation.pairs.Elements[i].timeOfImpact = 1;
            }
        }

        void ICCDPositionUpdateable.UpdatePositionContinuously(float dt)
        {
            float minimumToi = 1;
            for (var i = 0; i < collisionInformation.pairs.Count; i++)
            {
                if (collisionInformation.pairs.Elements[i].timeOfImpact < minimumToi)
                {
                    minimumToi = collisionInformation.pairs.Elements[i].timeOfImpact;
                }
            }

            //The orientation was already updated by the PreUpdatePosition.
            //However, to be here, this object is not a discretely updated object.
            //That means we still need to update the linear motion.

            Vector3 increment;
            Vector3.Multiply(ref linearVelocity, dt * minimumToi, out increment);
            Vector3.Add(ref position, ref increment, out position);

            collisionInformation.UpdateWorldTransform(ref position, ref orientation);

            if (PositionUpdated != null)
            {
                PositionUpdated(this);
            }

            linearVelocity.Validate();
            angularVelocity.Validate();
            position.Validate();
            orientation.Validate();
#if CONSERVE
            MathChecker.Validate(angularMomentum);
#endif
        }

        void IPositionUpdateable.PreUpdatePosition(float dt)
        {
            Vector3 increment;

            Vector3.Multiply(ref angularVelocity, dt * .5f, out increment);
            var multiplier = new Quaternion(increment.X, increment.Y, increment.Z, 0);
            Quaternion.Multiply(ref multiplier, ref orientation, out multiplier);
            Quaternion.Add(ref orientation, ref multiplier, out orientation);
            orientation.Normalize();

            Matrix3x3.CreateFromQuaternion(ref orientation, out orientationMatrix);

            //Only do the linear motion if this object doesn't obey CCD.
            if (PositionUpdateMode == PositionUpdateMode.Discrete)
            {
                Vector3.Multiply(ref linearVelocity, dt, out increment);
                Vector3.Add(ref position, ref increment, out position);

                collisionInformation.UpdateWorldTransform(ref position, ref orientation);
                //The position update is complete if this is a discretely updated object.
                if (PositionUpdated != null)
                {
                    PositionUpdated(this);
                }
            }

            linearVelocity.Validate();
            angularVelocity.Validate();
            position.Validate();
            orientation.Validate();
#if CONSERVE
            MathChecker.Validate(angularMomentum);
#endif
        }

        #endregion


        private float linearDampingBoost, angularDampingBoost;
        private float angularDamping = .15f;
        private float linearDamping = .03f;

        ///<summary>
        /// Gets or sets the angular damping of the entity.
        /// Values range from 0 to 1, corresponding to a fraction of angular momentum removed
        /// from the entity over a unit of time.
        ///</summary>
        public float AngularDamping
        {
            get => angularDamping;
            set => angularDamping = MathHelper.Clamp(value, 0, 1);
        }

        ///<summary>
        /// Gets or sets the linear damping of the entity.
        /// Values range from 0 to 1, corresponding to a fraction of linear momentum removed
        /// from the entity over a unit of time.
        ///</summary>
        public float LinearDamping
        {
            get => linearDamping;

            set => linearDamping = MathHelper.Clamp(value, 0, 1);
        }

        /// <summary>
        /// Temporarily adjusts the linear damping by an amount.  After the value is used, the
        /// damping returns to the base value.
        /// </summary>
        /// <param name="damping">Damping to add.</param>
        public void ModifyLinearDamping(float damping)
        {
            var totalDamping = LinearDamping + linearDampingBoost;
            var remainder = 1 - totalDamping;
            linearDampingBoost += damping * remainder;
        }

        /// <summary>
        /// Temporarily adjusts the angular damping by an amount.  After the value is used, the
        /// damping returns to the base value.
        /// </summary>
        /// <param name="damping">Damping to add.</param>
        public void ModifyAngularDamping(float damping)
        {
            var totalDamping = AngularDamping + angularDampingBoost;
            var remainder = 1 - totalDamping;
            angularDampingBoost += damping * remainder;
        }

        /// <summary>
        /// Gets or sets the user data associated with the entity.
        /// This is separate from the entity's collidable's tag.
        /// If a tag needs to be accessed from within the collision
        /// detection pipeline, consider using the entity.CollisionInformation.Tag.
        /// </summary>
        public object Tag { get; set; }


        CollisionRules ICollisionRulesOwner.CollisionRules
        {
            get => collisionInformation.collisionRules;
            set => collisionInformation.CollisionRules = value;
        }

        BroadPhaseEntry IBroadPhaseEntryOwner.Entry => collisionInformation;

        public override string ToString()
        {
            if (Tag == null)
            {
                return base.ToString();
            }

            return base.ToString() + ", " + Tag;
        }


#if WINDOWS_PHONE
        static int idCounter;
        /// <summary>
        /// Gets the entity's unique instance id.
        /// </summary>
        public int InstanceId { get; private set; }
#else
        private static long idCounter;
        /// <summary>
        /// Gets the entity's unique instance id.
        /// </summary>
        public long InstanceId { get; private set; }
#endif
        private void InitializeId()
        {
            InstanceId = System.Threading.Interlocked.Increment(ref idCounter);

            hashCode = (int) ((ulong) InstanceId * 4294967311UL % 4294967296UL);
        }


        private int hashCode;

        public override int GetHashCode()
        {
            return hashCode;
        }


        public bool Equals(Entity other)
        {
            return other == this;
        }

        public override bool Equals(object obj)
        {
            return Equals((Entity) obj);
        }
    }
}