using System.Numerics;
using MinorEngine.engine.physics;
using TKVector4 = OpenTK.Vector4;
using TKVector3 = OpenTK.Vector3;

namespace MinorEngine.components
{
    public enum ColliderType
    {
        DYNAMIC_SPHERE = 0,
        STATIC_SPHERE = 1,
        DYNAMIC_BOX = 2,
        STATIC_BOX = 3
    }


    public class BoxCollider : AbstractDynamicCollider
    {
        public Vector3 Bounds { get; }

        public BoxCollider(PhysicsMaterial physicsMaterial, Vector3 bounds, ushort collisionLayer,
            ushort collidableLayers) : base(ColliderType.DYNAMIC_BOX, physicsMaterial, collisionLayer, collidableLayers)
        {
            Bounds = bounds;
        }

        protected override void Awake()
        {
            Vector3 pos = new Vector3(Owner.GetLocalPosition().X, Owner.GetLocalPosition().Y,
                Owner.GetLocalPosition().Z);


            //BodyReference = Physics.AddBoxDynamic(1f, pos, Bounds);


            base.Awake();
        }
    }
}