using System.Collections.Generic;
using BepuPhysics;
using GameEngine.engine.physics;
using MinorEngine.engine.components;
using OpenTK;
using OpenTK.Graphics.ES11;

namespace MinorEngine.components
{
    public enum ColliderType
    {
        SPHERE = 0,
        BOX = 1
    }
    public class ColliderComponent : AbstractComponent
    {
        private BodyReference br;
        private bool _init;
        private ColliderType type;
        private float radius;
        private ushort layer = ushort.MaxValue;
        private ushort collidable = ushort.MaxValue;
        public ColliderComponent(ColliderType type, float radius, ushort collisionLayer, ushort collidableLayers)
        {
            layer = collisionLayer;
            collidable = collidableLayers;
            this.type = type;
            this.radius = radius;
        }



        public override void Update(float deltaTime)
        {
            if (!_init)
            {
                Vector3 pos = Owner.GetLocalPosition();
                if (type == ColliderType.SPHERE)
                {
                    br = Physics.AddSphereDynamic(1f, new System.Numerics.Vector3(pos.X, pos.Y, pos.Z), radius, this);
                }
                else
                {
                    br = Physics.AddBoxDynamic(1f, new System.Numerics.Vector3(pos.X, pos.Y, pos.Z), new System.Numerics.Vector3(radius), this);
                }

                ref Layer l = ref Physics.CollisionFilters.Allocate(br.Handle);
                l.CollidableSubgroups = collidable;
                l.SubgroupMembership = layer;
                l.GroupId = 0;
                _init = true;
            }
            Owner.SetLocalPosition(new Vector3(br.Pose.Position.X, br.Pose.Position.Y, br.Pose.Position.Z));
            Owner.SetRotation(br.Pose.Orientation);
        }
    }
}