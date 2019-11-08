using Engine.Physics.BEPUphysics.CollisionTests.Manifolds;

namespace Engine.Physics.BEPUphysics.NarrowPhaseSystems.Pairs
{
    ///<summary>
    /// Handles a static mesh-sphere collision pair.
    ///</summary>
    public class StaticMeshSpherePairHandler : StaticMeshPairHandler
    {
        private StaticMeshSphereContactManifold contactManifold = new StaticMeshSphereContactManifold();

        protected override StaticMeshContactManifold MeshManifold => contactManifold;
    }
}