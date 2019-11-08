using Engine.Physics.BEPUphysics.CollisionTests.Manifolds;

namespace Engine.Physics.BEPUphysics.NarrowPhaseSystems.Pairs
{
    ///<summary>
    /// Handles a terrain-sphere collision pair.
    ///</summary>
    public sealed class TerrainSpherePairHandler : TerrainPairHandler
    {
        private TerrainSphereContactManifold contactManifold = new TerrainSphereContactManifold();

        protected override TerrainContactManifold TerrainManifold => contactManifold;
    }
}