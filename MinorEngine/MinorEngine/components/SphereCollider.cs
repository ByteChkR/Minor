using System.Numerics;
using MinorEngine.engine.physics;

namespace MinorEngine.components
{
    public class SphereCollider : AbstractDynamicCollider
    {
        public float Radius { get; }

        public SphereCollider(PhysicsMaterial physicsMaterial, ushort layer, ushort collidableLayer, float sphereRadius)
            : base(ColliderType.DYNAMIC_SPHERE, physicsMaterial, layer, collidableLayer)
        {
            Radius = sphereRadius;
        }

        protected override void Awake()
        {
            Vector3 pos = new Vector3(Owner.GetLocalPosition().X, Owner.GetLocalPosition().Y,
                Owner.GetLocalPosition().Z);
            //BodyReference = Physics.AddSphereDynamic(1f, pos, Radius);


            base.Awake();
        }
    }
}