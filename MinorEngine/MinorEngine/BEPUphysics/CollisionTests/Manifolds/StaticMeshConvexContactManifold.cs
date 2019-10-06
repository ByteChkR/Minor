using MinorEngine.BEPUphysics.CollisionTests.CollisionAlgorithms;
using MinorEngine.BEPUutilities.ResourceManagement;

namespace MinorEngine.BEPUphysics.CollisionTests.Manifolds
{
    ///<summary>
    /// Manages persistent contacts between a static mesh and a convex.
    ///</summary>
    public class StaticMeshConvexContactManifold : StaticMeshContactManifold
    {
        private static LockingResourcePool<TriangleConvexPairTester> testerPool =
            new LockingResourcePool<TriangleConvexPairTester>();

        protected override void GiveBackTester(TrianglePairTester tester)
        {
            testerPool.GiveBack((TriangleConvexPairTester) tester);
        }

        protected override TrianglePairTester GetTester()
        {
            return testerPool.Take();
        }
    }
}