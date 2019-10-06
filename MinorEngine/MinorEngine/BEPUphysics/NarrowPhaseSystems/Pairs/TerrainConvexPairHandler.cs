using MinorEngine.BEPUphysics.CollisionTests.Manifolds;

namespace MinorEngine.BEPUphysics.NarrowPhaseSystems.Pairs
{
    ///<summary>
    /// Handles a terrain-convex collision pair.
    ///</summary>
    public sealed class TerrainConvexPairHandler : TerrainPairHandler
    {
        private TerrainConvexContactManifold contactManifold = new TerrainConvexContactManifold();

        protected override TerrainContactManifold TerrainManifold => contactManifold;
    }
}