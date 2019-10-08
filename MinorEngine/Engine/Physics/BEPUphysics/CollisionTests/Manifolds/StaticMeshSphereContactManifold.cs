using Engine.Physics.BEPUphysics.CollisionTests.CollisionAlgorithms;
using Engine.Physics.BEPUutilities.ResourceManagement;

namespace Engine.Physics.BEPUphysics.CollisionTests.Manifolds
{
    ///<summary>
    /// Manages persistent contacts between a static mesh and a convex.
    ///</summary>
    public class StaticMeshSphereContactManifold : StaticMeshContactManifold
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