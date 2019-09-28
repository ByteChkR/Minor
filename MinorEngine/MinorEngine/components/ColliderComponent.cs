using System.Collections.Generic;
using BepuPhysics;
using GameEngine.engine.physics;
using GameEngine.engine.components;
using OpenTK;
using OpenTK.Graphics.ES11;

namespace GameEngine.components
{
    public enum ColliderType
    {
        SPHERE = 0,
        BOX = 1
    }
    public class ColliderComponent : AbstractComponent, IColliderComponent
    {
        public BodyReference BodyReference { get; set; }
        private readonly ColliderType type;
        private readonly float radius;
        private readonly ushort layer = ushort.MaxValue;
        private readonly ushort collidable = ushort.MaxValue;
        public ColliderComponent(ColliderType type, float radius, ushort collisionLayer, ushort collidableLayers)
        {
            layer = collisionLayer;
            collidable = collidableLayers;
            this.type = type;
            this.radius = radius;
        }


        protected override void Awake()
        {
            base.Awake();
            Vector3 pos = Owner.GetLocalPosition();
            if (type == ColliderType.SPHERE)
            {
                BodyReference = Physics.AddSphereDynamic(1f, new System.Numerics.Vector3(pos.X, pos.Y, pos.Z), radius);
            }
            else
            {
                BodyReference = Physics.AddBoxDynamic(1f, new System.Numerics.Vector3(pos.X, pos.Y, pos.Z), new System.Numerics.Vector3(radius));
            }

            ref Layer l = ref Physics.CollisionFilters.Allocate(BodyReference.Handle);
            l.CollidableSubgroups = collidable;
            l.SubgroupMembership = layer;
            l.GroupId = 0;
        }

        protected override void Update(float deltaTime)
        {
            
            Owner.SetLocalPosition(new Vector3(BodyReference.Pose.Position.X, BodyReference.Pose.Position.Y, BodyReference.Pose.Position.Z));
            Owner.SetRotation(BodyReference.Pose.Orientation);
        }
    }
}