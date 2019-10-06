using MinorEngine.BEPUphysics.CollisionTests.Manifolds;

namespace MinorEngine.BEPUphysics.NarrowPhaseSystems.Pairs
{
    ///<summary>
    /// Handles a mobile mesh-sphere collision pair.
    ///</summary>
    public class MobileMeshSpherePairHandler : MobileMeshPairHandler
    {
        private MobileMeshSphereContactManifold contactManifold = new MobileMeshSphereContactManifold();

        protected internal override MobileMeshContactManifold MeshManifold => contactManifold;
    }
}