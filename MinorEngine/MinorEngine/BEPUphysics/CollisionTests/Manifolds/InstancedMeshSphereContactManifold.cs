using MinorEngine.BEPUphysics.CollisionTests.CollisionAlgorithms;
using MinorEngine.BEPUutilities.ResourceManagement;

namespace MinorEngine.BEPUphysics.CollisionTests.Manifolds
{
    ///<summary>
    /// Manages persistent contacts between a convex and an instanced mesh.
    ///</summary>
    public class InstancedMeshSphereContactManifold : InstancedMeshContactManifold
    {
        private static LockingResourcePool<TriangleSpherePairTester> testerPool =
            new LockingResourcePool<TriangleSpherePairTester>();

        protected override void GiveBackTester(TrianglePairTester tester)
        {
            testerPool.GiveBack((TriangleSpherePairTester) tester);
        }

        protected override TrianglePairTester GetTester()
        {
            return testerPool.Take();
        }
    }
}