using Engine.Physics.BEPUphysics.CollisionTests.Manifolds;

namespace Engine.Physics.BEPUphysics.NarrowPhaseSystems.Pairs
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