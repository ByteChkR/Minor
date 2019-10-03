using MinorEngine.BEPUphysics.CollisionTests.CollisionAlgorithms;
using MinorEngine.BEPUutilities.ResourceManagement;

namespace MinorEngine.BEPUphysics.CollisionTests.Manifolds
{
    public class TerrainSphereContactManifold : TerrainContactManifold
    {
        static LockingResourcePool<TriangleSpherePairTester> testerPool = new LockingResourcePool<TriangleSpherePairTester>();
        protected override TrianglePairTester GetTester()
        {
            return testerPool.Take();
        }

        protected override void GiveBackTester(TrianglePairTester tester)
        {
            testerPool.GiveBack((TriangleSpherePairTester)tester);
        }

    }
}
