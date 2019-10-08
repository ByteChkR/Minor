using System.Collections.Generic;
using Engine.Core;
using Engine.Physics.BEPUphysics;
using Engine.Physics.BEPUphysics.BroadPhaseEntries;
using Engine.Physics.BEPUphysics.Entities;
using Engine.Physics.BEPUphysics.OtherSpaceStages;
using Engine.Physics.BEPUutilities;
using TKVector3 = OpenTK.Vector3;
using TKVector2 = OpenTK.Vector2;

namespace Engine.Physics
{
    public static class PhysicsEngine
    {
        private static Space _space;
        private static ForceUpdater _fu;

        public static TKVector3 Gravity
        {
            get => _fu?.Gravity ?? TKVector3.Zero;
            set
            {
                if (_fu != null)
                {
                    _fu.Gravity = value;
                }
            }
        }

        public static void Initialize()
        {
            _space = new Space();
            _fu = new ForceUpdater(new TimeStepSettings());
            _fu.Gravity = TKVector3.UnitY * -10;
            _space.ForceUpdater = _fu;
        }

        public static Ray ConstructRayFromMousePosition(TKVector3 origin)
        {
            var mpos = GameEngine.Instance.MousePosition;
            var mousepos = GameEngine.Instance.ConvertScreenToWorldCoords((int) mpos.X, (int) mpos.Y);
            return new Ray(origin, (mousepos - origin).Normalized());
        }

        public static bool RayCastAll(Ray ray, float maxLength, string layerInfo,
            out KeyValuePair<Collider, RayHit>[] collisions)
        {
            return RayCastAll(ray, maxLength, LayerManager.NameToLayer(layerInfo), out collisions);
        }

        public static bool RayCastAll(Ray ray, float maxLength, int layerInfo,
            out KeyValuePair<Collider, RayHit>[] collisions)
        {
            IList<RayCastResult> results = new List<RayCastResult>();
            var ret = _space.RayCast(ray, maxLength, entry => FilterFunc(entry, layerInfo), results);
            collisions = new KeyValuePair<Collider, RayHit>[results.Count];
            for (var i = 0; i < results.Count; i++)
            {
                collisions[i] =
                    new KeyValuePair<Collider, RayHit>((Collider) results[i].HitObject.Tag, results[i].HitData);
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
            var ret = _space.RayCast(ray, maxLength, entry => FilterFunc(entry, layerInfo), out var res);
            if (!ret)
            {
                collision = new KeyValuePair<Collider, RayHit>(null, new RayHit());
            }
            else
            {
                collision = new KeyValuePair<Collider, RayHit>((Collider) res.HitObject.Tag, res.HitData);
            }

            return ret;
        }

        private static bool FilterFunc(BroadPhaseEntry entry, int layerInfo)
        {
            if (entry.Tag == null)
            {
                return false;
            }

            var obj = (Collider) entry.Tag;
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