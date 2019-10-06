using MinorEngine.BEPUphysics.CollisionTests.Manifolds;

namespace MinorEngine.BEPUphysics.NarrowPhaseSystems.Pairs
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