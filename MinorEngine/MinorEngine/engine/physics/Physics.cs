using System.Collections.Generic;
using MinorEngine.BEPUphysics;
using MinorEngine.BEPUphysics.BroadPhaseEntries;
using MinorEngine.BEPUphysics.BroadPhaseEntries.Events;
using MinorEngine.BEPUphysics.Entities;
using MinorEngine.BEPUphysics.Entities.Prefabs;
using MinorEngine.BEPUphysics.OtherSpaceStages;
using MinorEngine.BEPUutilities;
using MinorEngine.components;
using MinorEngine.debug;
using MinorEngine.engine.core;
using OpenTK;
using TKVector3 = OpenTK.Vector3;
using TKVector2 = OpenTK.Vector2;
using Vector3 = MinorEngine.BEPUutilities.Vector3;

namespace MinorEngine.engine.physics
{
    public static class Physics
    {
        private static Space _space;
        private static ForceUpdater _fu;

        public static Vector3 Gravity
        {
            get => _fu?.Gravity ?? Vector3.Zero;
            set
            {
                if (_fu != null) _fu.Gravity = value;
            }
        }

        public static void Initialize()
        {
            _space = new Space();
            _fu = new ForceUpdater(new TimeStepSettings());
            _fu.Gravity = Vector3.Down * 10;
            _space.ForceUpdater = _fu;
        }

        public static Ray ConstructRayFromMousePosition(TKVector3 origin)
        {
            TKVector2 mpos = GameEngine.Instance.MousePosition;
            TKVector3 mousepos = GameEngine.Instance.ConvertScreenToWorldCoords((int)mpos.X, (int)mpos.Y);
            return new Ray(origin, (mousepos - origin).Normalized());
        }

        public static bool RayCastAll(Ray ray, float maxLength, string layerInfo,
            out KeyValuePair<Collider, RayHit>[] collisions)
        {
            return RayCastAll(ray, maxLength, LayerManager.NameToLayer(layerInfo), out collisions);
        }

        public static bool RayCastAll(Ray ray, float maxLength, int layerInfo, out KeyValuePair<Collider, RayHit>[] collisions)
        {
            IList<RayCastResult> results = new List<RayCastResult>();
            bool ret = _space.RayCast(ray, maxLength, (entry) => FilterFunc(entry, layerInfo), results);
            collisions = new KeyValuePair<Collider, RayHit>[results.Count];
            for (int i = 0; i < results.Count; i++)
            {
                collisions[i] = new KeyValuePair<Collider, RayHit>((Collider)results[i].HitObject.Tag, results[i].HitData);
            }
            return ret;
        }

        public static bool RayCastFirst(Ray ray, float maxLength, string layerInfo,
            out KeyValuePair<Collider, RayHit> collision)
        {
            return RayCastFirst(ray, maxLength, LayerManager.NameToLayer(layerInfo), out collision);
        }

        public static bool RayCastFirst(Ray ray, float maxLength, int layerInfo,
            out KeyValuePair<Collider, RayHit> collision)
        {
            bool ret = _space.RayCast(ray, maxLength, (entry) => FilterFunc(entry, layerInfo), out RayCastResult res);
            if (!ret) collision = new KeyValuePair<Collider, RayHit>(null, default);
            else collision= new KeyValuePair<Collider, RayHit>((Collider)res.HitObject.Tag, res.HitData);
            return ret;
        }

        private static bool FilterFunc(BroadPhaseEntry entry, int layerInfo)
        {
            if (entry.Tag == null) return false;
            Collider obj = (Collider)entry.Tag;
            return LayerManager.AllowCollision(obj.CollisionLayer, layerInfo);

        }

        public static void AddEntity(Entity physicsCollider)
        {
            _space.Add(physicsCollider);
        }

        public static void RemoveEntity(Entity physicsCollider)
        {
            _space.Remove(physicsCollider);
        }


        internal static void Update(float deltaTime)
        {
            _space.Update(deltaTime);
        }
    }
}