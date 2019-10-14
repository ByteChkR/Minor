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
    /// <summary>
    /// Static class that serves as a wrapper for the physics engine, providing all much used functions and the initialization code to start a physics simulation
    /// </summary>
    public static class PhysicsEngine
    {
        /// <summary>
        /// The Space the simulation takes place
        /// </summary>
        private static Space _space;

        /// <summary>
        /// The Force updater that is used to apply gravity to the colliders
        /// </summary>
        private static ForceUpdater _fu;

        /// <summary>
        /// The Gravity that is applied to the colliders
        /// </summary>
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

        /// <summary>
        /// Initialization Code for the Physics Engine
        /// </summary>
        public static void Initialize()
        {
            _space = new Space();
            _fu = new ForceUpdater(new TimeStepSettings());
            _fu.Gravity = TKVector3.UnitY * -10;
            _space.ForceUpdater = _fu;
        }

        /// <summary>
        /// Constructs a Ray from the current mouse posiition
        /// </summary>
        /// <param name="origin"></param>
        /// <returns>A ray from origin to mouse position in world coordinates</returns>
        public static Ray ConstructRayFromMousePosition(TKVector3 origin)
        {
            TKVector2 mpos = GameEngine.Instance.MousePosition;
            TKVector3 mousepos = GameEngine.Instance.ConvertScreenToWorldCoords((int)mpos.X, (int)mpos.Y);
            return new Ray(origin, (mousepos - origin).Normalized());
        }

        /// <summary>
        /// Raycasts all colliders
        /// </summary>
        /// <param name="ray">The Ray to cast</param>
        /// <param name="maxLength">Maximum Length of the Ray</param>
        /// <param name="layerInfo">The Layer Name to cast on</param>
        /// <param name="collisions">The Collisions that were detected</param>
        /// <returns>If any colliders have been hit</returns>
        public static bool RayCastAll(Ray ray, float maxLength, string layerInfo,
            out KeyValuePair<Collider, RayHit>[] collisions)
        {
            return RayCastAll(ray, maxLength, LayerManager.NameToLayer(layerInfo), out collisions);
        }

        /// <summary>
        /// Raycasts all colliders
        /// </summary>
        /// <param name="ray">The Ray to cast</param>
        /// <param name="maxLength">Maximum Length of the Ray</param>
        /// <param name="layerInfo">The Layer ID to cast on</param>
        /// <param name="collisions">The Collisions that were detected</param>
        /// <returns>If any colliders have been hit</returns>
        public static bool RayCastAll(Ray ray, float maxLength, int layerInfo,
            out KeyValuePair<Collider, RayHit>[] collisions)
        {
            IList<RayCastResult> results = new List<RayCastResult>();
            bool ret = _space.RayCast(ray, maxLength, entry => FilterFunc(entry, layerInfo), results);
            collisions = new KeyValuePair<Collider, RayHit>[results.Count];
            for (int i = 0; i < results.Count; i++)
            {
                collisions[i] =
                    new KeyValuePair<Collider, RayHit>((Collider)results[i].HitObject.Tag, results[i].HitData);
            }

            return ret;
        }

        /// <summary>
        /// Raycasts until the first collider has been found
        /// </summary>
        /// <param name="ray">The Ray to cast</param>
        /// <param name="maxLength">Maximum Length of the Ray</param>
        /// <param name="layerInfo">The Layer Name to cast on</param>
        /// <param name="collision">The Collision that was detected</param>
        /// <returns>If any colliders have been hit</returns>
        public static bool RayCastFirst(Ray ray, float maxLength, string layerInfo,
            out KeyValuePair<Collider, RayHit> collision)
        {
            return RayCastFirst(ray, maxLength, LayerManager.NameToLayer(layerInfo), out collision);
        }


        /// <summary>
        /// Raycasts until the first collider has been found
        /// </summary>
        /// <param name="ray">The Ray to cast</param>
        /// <param name="maxLength">Maximum Length of the Ray</param>
        /// <param name="layerInfo">The Layer ID to cast on</param>
        /// <param name="collision">The Collision that was detected</param>
        /// <returns>If any colliders have been hit</returns>
        public static bool RayCastFirst(Ray ray, float maxLength, int layerInfo,
            out KeyValuePair<Collider, RayHit> collision)
        {
            bool ret = _space.RayCast(ray, maxLength, entry => FilterFunc(entry, layerInfo), out RayCastResult res);
            if (!ret)
            {
                collision = new KeyValuePair<Collider, RayHit>(null, new RayHit());
            }
            else
            {
                collision = new KeyValuePair<Collider, RayHit>((Collider)res.HitObject.Tag, res.HitData);
            }

            return ret;
        }

        /// <summary>
        /// Private Function that is used to filter Collisions by tags
        /// </summary>
        /// <param name="entry"></param>
        /// <param name="layerInfo"></param>
        /// <returns>True if the objects are allowed to collide; False if they are not</returns>
        private static bool FilterFunc(BroadPhaseEntry entry, int layerInfo)
        {
            if (entry.Tag == null)
            {
                return false;
            }

            Collider obj = (Collider)entry.Tag;
            return LayerManager.AllowCollision(obj.CollisionLayer, layerInfo);
        }

        /// <summary>
        /// Adds an physics entity to the physics engine
        /// </summary>
        /// <param name="physicsCollider">the entity to add</param>
        public static void AddEntity(Entity physicsCollider)
        {
            _space.Add(physicsCollider);
        }

        /// <summary>
        /// Removes an physics entity from the physics engine
        /// </summary>
        /// <param name="physicsCollider">the entity to remove</param>
        public static void RemoveEntity(Entity physicsCollider)
        {
            if (physicsCollider.Space == _space)
                _space.Remove(physicsCollider);
        }

        /// <summary>
        /// Function to update the Physics Engine
        /// </summary>
        /// <param name="deltaTime">The amount of time that has passed since the last update</param>
        internal static void Update(float deltaTime)
        {
            _space.Update(deltaTime);
        }
    }
}