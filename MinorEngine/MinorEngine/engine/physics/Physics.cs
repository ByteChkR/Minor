using MinorEngine.BEPUphysics;
using MinorEngine.BEPUphysics.Entities;
using MinorEngine.BEPUphysics.Entities.Prefabs;
using MinorEngine.BEPUphysics.OtherSpaceStages;
using MinorEngine.components;
using MinorEngine.debug;
using OpenTK;
using Vector3 = MinorEngine.BEPUutilities.Vector3;

namespace MinorEngine.engine.physics
{
    public static class Physics
    {
        private static Space _space;

        public static void Initialize()
        {
            _space = new Space();
            ForceUpdater fu = new ForceUpdater(new TimeStepSettings());
            fu.Gravity = Vector3.Down * 10;
            _space.ForceUpdater = fu;
        }

        public static void AddEntity(Entity physicsCollider)
        {
            _space.Add(physicsCollider);
        }

        public static void RemoveEntity(Entity physicsCollider)
        {
            physicsCollider.Log("Removing Physics Collider.", DebugChannel.Log);
            _space.Remove(physicsCollider);
        }


        internal static void Update(float deltaTime)
        {
            _space.Update(deltaTime);
        }
    }
}