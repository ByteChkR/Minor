using MinorEngine.BEPUphysics.CollisionTests.Manifolds;

namespace MinorEngine.BEPUphysics.NarrowPhaseSystems.Pairs
{
    ///<summary>
    /// Handles a instanced mesh-convex collision pair.
    ///</summary>
    public class InstancedMeshConvexPairHandler : InstancedMeshPairHandler
    {
        private InstancedMeshConvexContactManifold contactManifold = new InstancedMeshConvexContactManifold();

        protected override InstancedMeshContactManifold MeshManifold => contactManifold;
    }
}