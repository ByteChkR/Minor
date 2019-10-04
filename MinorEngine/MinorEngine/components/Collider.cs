using MinorEngine.BEPUphysics;
using MinorEngine.BEPUphysics.BroadPhaseEntries.Events;
using MinorEngine.BEPUphysics.Constraints.TwoEntity.Motors;
using MinorEngine.BEPUphysics.Entities;
using MinorEngine.BEPUphysics.Entities.Prefabs;
using MinorEngine.BEPUutilities;
using MinorEngine.debug;
using MinorEngine.engine.components;
using MinorEngine.engine.physics;
using Vector3 = OpenTK.Vector3;

namespace MinorEngine.components
{
    public class Collider : AbstractComponent
    {
        public Entity PhysicsCollider { get; }
        public RigidBodyConstraints ColliderConstraints { get; set; }
        public int CollisionLayer { get; set; }
        private bool _colliderRemoved = false;

        public Collider(Entity shape, string layerName):this(shape, LayerManager.NameToLayer(layerName))
        {
            
        }

        public Collider(Entity shape, int layerID)
        {
            PhysicsCollider = shape;

            PhysicsCollider.CollisionInformation.Tag = this;
            CollisionLayer = layerID;
        }

        ~Collider()
        {

            if (!_colliderRemoved)
            {
                this.Log("FUCK", DebugChannel.Log);
            }
        }


        protected override void Awake()
        {
            PhysicsCollider.Position = Owner.GetLocalPosition();
            PhysicsCollider.orientation = Owner.GetOrientation();
            Physics.AddEntity(PhysicsCollider);
        }

        protected override void OnDestroy()
        {
            PhysicsCollider.CollisionInformation.Tag = null;
            Physics.RemoveEntity(PhysicsCollider);
            _colliderRemoved = true;
        }

        public void SetVelocityLinear(Vector3 vel)
        {
            PhysicsCollider.LinearVelocity = vel;
        }
        public void SetVelocityAngular(Vector3 vel)
        {
            PhysicsCollider.AngularVelocity = vel;
        }

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