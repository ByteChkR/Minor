using Engine.Physics.BEPUphysics.CollisionTests.Manifolds;

namespace Engine.Physics.BEPUphysics.NarrowPhaseSystems.Pairs
{
    ///<summary>
    /// Handles a mobile mesh-convex collision pair.
    ///</summary>
    public class MobileMeshConvexPairHandler : MobileMeshPairHandler
    {
        private MobileMeshConvexContactManifold contactManifold = new MobileMeshConvexContactManifold();

        protected internal override MobileMeshContactManifold MeshManifold => contactManifold;
    }
}