using BepuPhysics;
using BepuPhysics.Constraints;
using Common;
using MinorEngine.debug;
using MinorEngine.engine.components;
using MinorEngine.engine.physics;
using OpenTK;

namespace MinorEngine.components
{
    public struct PhysicsMaterial
    {
        public SpringSettings SpringSettings => new SpringSettings(SpringFreq, DampRatio);
        public float SpringFreq;
        public float DampRatio;
        public float FrictionCoeff;
        public float MaxRecoverVelocity;

        public PhysicsMaterial(float friction)
        {
            SpringFreq = 30;
            DampRatio = 1;
            FrictionCoeff = friction;
            MaxRecoverVelocity = 2;
        }
    }

    public abstract class AbstractDynamicCollider : AbstractComponent, IColliderComponent
    {
        public BodyReference BodyReference { get; set; }
        public PhysicsMaterial PhysicsMaterial { get; }

        protected ColliderType Type { get; }
        protected ushort Layer { get; } = ushort.MaxValue;
        protected ushort Collidable { get; } = ushort.MaxValue;

        protected AbstractDynamicCollider(ColliderType type, PhysicsMaterial phyMaterial, ushort collisionLayer,
            ushort collidableLayers)
        {
            Type = type;
            Layer = collisionLayer;
            Collidable = collidableLayers;
            PhysicsMaterial = phyMaterial;
        }

        protected override void OnDestroy()
        {
            this.Log("Removing Physics Body with handle: " + BodyReference.Handle, DebugChannel.Log);
            Physics.RemoveBody(BodyReference);
        }

        protected override void Awake()
        {
            base.Awake();


            Vector3 pos = Owner.GetLocalPosition();


            ref PhysicsMaterial phyMat = ref Physics.PhysicsMaterials.Allocate(BodyReference.Handle);
            phyMat = PhysicsMaterial;


            ref Layer l = ref Physics.CollisionFilters.Allocate(BodyReference.Handle);
            l.CollidableSubgroups = Collidable;
            l.SubgroupMembership = Layer;
            l.GroupId = 0;
        }
    }
}