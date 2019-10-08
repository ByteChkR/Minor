using Engine.Physics.BEPUphysics.CollisionTests.Manifolds;

namespace Engine.Physics.BEPUphysics.NarrowPhaseSystems.Pairs
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