using Engine.Core;
using Engine.Debug;
using Engine.Exceptions;
using Engine.Physics.BEPUphysics.CollisionRuleManagement;
using Engine.Physics.BEPUphysics.Entities;
using Engine.Physics.BEPUutilities;
using Vector3 = OpenTK.Vector3;

namespace Engine.Physics
{
    /// <summary>
    /// Collider Component that is used to Connect the physics engine to the game systems
    /// </summary>
    public class Collider : AbstractComponent
    {
        /// <summary>
        /// The Physics System collider
        /// </summary>
        public Entity PhysicsCollider { get; }

        /// <summary>
        /// The constraints of the collider
        /// </summary>
        public ColliderConstraints ColliderConstraints { get; set; }

        /// <summary>
        /// The Collision layer
        /// </summary>
        public int CollisionLayer { get; set; }

        /// <summary>
        /// Private flag if the collider has been removed from the physics engine
        /// </summary>
        private bool _colliderRemoved;

        /// <summary>
        /// Property to get/set the collider state
        /// IsTrigger = true -> No Collision solving.
        /// IsTrigger = false -> Collision Solving
        /// </summary>
        public bool IsTrigger
        {
            get => PhysicsCollider.CollisionInformation.CollisionRules.Personal == CollisionRule.NoSolver;
            set => PhysicsCollider.CollisionInformation.CollisionRules.Personal =
                value ? CollisionRule.NoSolver : CollisionRule.Normal;
        }

        /// <summary>
        /// Constructor for creating a Collider Component
        /// </summary>
        /// <param name="shape">The Physics Engine Shape</param>
        /// <param name="layerName">The layer of the collider</param>
        public Collider(Entity shape, string layerName) : this(shape, LayerManager.NameToLayer(layerName))
        {
        }

        /// <summary>
        /// Constructor for creating a Collider Component
        /// </summary>
        /// <param name="shape">The Physics Engine Shape</param>
        /// <param name="layerID">The layer of the collider</param>
        public Collider(Entity shape, int layerID)
        {
            PhysicsCollider = shape;

            PhysicsCollider.CollisionInformation.Tag = this;
            CollisionLayer = layerID;
        }


        /// <summary>
        /// Destructor that will log a crash when the collider was not removed when the object is being garbage collected
        /// </summary>
        ~Collider()
        {
            if (!_colliderRemoved)
            {
                Logger.Crash(new PossibleMemoryLeakException("Collider Component"), true);
            }
        }

        /// <summary>
        /// Awake override that initially sets the physicsshape to the position of the gameobject
        /// </summary>
        protected override void Awake()
        {
            PhysicsCollider.Position = Owner.GetLocalPosition();
            PhysicsCollider.orientation = Owner.GetOrientation();
            PhysicsEngine.AddEntity(PhysicsCollider);
        }

        /// <summary>
        /// Removes the Physics Collider from the Physics Engine
        /// </summary>
        protected override void OnDestroy()
        {
            PhysicsCollider.CollisionInformation.Tag = null;
            PhysicsEngine.RemoveEntity(PhysicsCollider);
            _colliderRemoved = true;
        }

        /// <summary>
        /// Sets the Linear velocity to a specified amount
        /// </summary>
        /// <param name="vel">The velocity of the body</param>
        public void SetVelocityLinear(Vector3 vel)
        {
            PhysicsCollider.LinearVelocity = vel;
        }

        /// <summary>
        /// Sets the Angular velocity to a specified amount
        /// </summary>
        /// <param name="vel">The velocity of the body</param>
        public void SetVelocityAngular(Vector3 vel)
        {
            PhysicsCollider.AngularVelocity = vel;
        }

        /// <summary>
        /// Function that will modify the linear velocity to satisfy the constraints
        /// </summary>
        private void enforceTranslationConstraints()
        {
            Vector3 veel = PhysicsCollider.LinearVelocity;
            if ((ColliderConstraints.PositionConstraints & FreezeConstraints.X) != 0)
            {
                veel.X = 0;
            }

            if ((ColliderConstraints.PositionConstraints & FreezeConstraints.Y) != 0)
            {
                veel.Y = 0;
            }

            if ((ColliderConstraints.PositionConstraints & FreezeConstraints.Z) != 0)
            {
                veel.Y = 0;
            }

            PhysicsCollider.LinearVelocity = veel;
        }

        /// <summary>
        /// Function that will modify the angular velocity(indirectly) to satisfy the constraints
        /// by setting the InertiaTensorInverse to 0 on the specified axes(effectively having infinite rotational friction)
        /// </summary>
        private void enforceRotationConstraints()
        {
            Matrix3x3 veel = PhysicsCollider.LocalInertiaTensorInverse;

            if ((ColliderConstraints.RotationConstraints & FreezeConstraints.X) != 0)
            {
                veel.Left = new BEPUutilities.Vector3(0, 0, 0);
            }

            if ((ColliderConstraints.RotationConstraints & FreezeConstraints.Y) != 0)
            {
                veel.Up = new BEPUutilities.Vector3(0, 0, 0);
            }

            if ((ColliderConstraints.RotationConstraints & FreezeConstraints.Z) != 0)
            {
                veel.Backward = new BEPUutilities.Vector3(0, 0, 0);
            }

            PhysicsCollider.LocalInertiaTensorInverse = veel;
        }


        /// <summary>
        /// Update function that will keep the Collider in the physics engine in sync with the Game World
        /// </summary>
        /// <param name="deltaTime"></param>
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