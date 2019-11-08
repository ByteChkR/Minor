using Engine.Physics.BEPUphysics.CollisionTests.Manifolds;

namespace Engine.Physics.BEPUphysics.NarrowPhaseSystems.Pairs
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