using MinorEngine.BEPUphysics.CollisionTests.Manifolds;

namespace MinorEngine.BEPUphysics.NarrowPhaseSystems.Pairs
{
    ///<summary>
    /// Handles a static mesh-convex collision pair.
    ///</summary>
    public class StaticMeshConvexPairHandler : StaticMeshPairHandler
    {
        private StaticMeshConvexContactManifold contactManifold = new StaticMeshConvexContactManifold();

        protected override StaticMeshContactManifold MeshManifold => contactManifold;
    }
}