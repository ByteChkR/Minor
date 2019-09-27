using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.CompilerServices;
using BepuPhysics;
using BepuPhysics.Collidables;
using BepuPhysics.CollisionDetection;
using BepuUtilities.Memory;
using MinorEngine.components;
using MinorEngine.engine.physics.callbacks;
using NarrowPhase = MinorEngine.engine.physics.callbacks.NarrowPhase;
using Quaternion = BepuUtilities.Quaternion;

namespace GameEngine.engine.physics
{
    /// <summary>
    /// Shows a completely isolated usage of the engine without using any of the other demo types.
    /// </summary>
    public static class Physics
    {
        private static Simulation _simulation;
        public static BodyProperty<Layer> CollisionFilters { get; set; }
        public static List<Layer> LayerList { get; } = new List<Layer>();

        public static void Init()
        {
            //The buffer pool is a source of raw memory blobs for the engine to use.
            var bufferPool = new BufferPool();
            //Note that you can also control the order of internal stage execution using a different ITimestepper implementation.
            //For the purposes of this demo, we just use the default by passing in nothing (which happens to be PositionFirstTimestepper at the time of writing).
            _simulation = Simulation.Create(bufferPool, new NarrowPhase(), new PoseIntegrator(new Vector3(0, -10, 0)));

        }

        /// <summary>
        ///     Returns true if the specified flag is also set in the mask
        /// </summary>
        /// <param name="mask">the mask</param>
        /// <param name="flag">the flag</param>
        /// <param name="matchType">if false, it will return true if ANY flag is set on both sides.</param>
        /// <returns></returns>
        public static bool IsContainedInMask(int mask, int flag, bool matchType)
        {
            if (mask == 0 || flag == 0)
            {
                return false; //Anti-Wildcard
            }

            if (matchType) //If true it compares the whole mask with the whole flag(if constructed from different flags)
            {
                return (mask & flag) == flag;
            }
            var a = GetUniqueMasksSet(flag);
            foreach (var f in a)
            {
                if ((mask & f) == f)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        ///     Splits up parameter mask into Unique Flags(power of 2 numbers)
        /// </summary>
        /// <param name="mask">the mask you want to split</param>
        /// <returns></returns>
        public static List<int> GetUniqueMasksSet(int mask)
        {
            if (IsUniqueMask(mask))
            {
                return new List<int> {mask};
            }
            var ret = new List<int>();
            for (var i = 0; i < sizeof(int) * sizeof(byte); i++)
            {
                var f = 1 << i;
                if (IsContainedInMask(mask, f, true))
                {
                    ret.Add(f);
                }
            }

            return ret;
        }

        /// <summary>
        ///     Checks if the specified mask is unique(e.g. a power of 2 number)
        /// </summary>
        /// <param name="mask">mask to test</param>
        /// <returns></returns>
        public static bool IsUniqueMask(int mask)
        {
            return mask != 0 && (mask & (mask - 1)) == 0;
        }

        public static void SetLayerDetection(int bodyhandle, int otherBodyHandle)
        {
            ref var thisLayer = ref CollisionFilters.Allocate(bodyhandle);
            ref var otherLayer = ref CollisionFilters.Allocate(otherBodyHandle);
            Layer.DisableCollision(ref thisLayer, ref otherLayer);
        }

        internal static BodyReference AddBoxDynamic(float mass, Vector3 position, Vector3 dimensions, ColliderComponent comp)
        {
            Box b = new Box(dimensions.X, dimensions.Y, dimensions.Z);
            b.ComputeInertia(1, out var boxIntertia);
            int handle = _simulation.Bodies.Add(
                BodyDescription.CreateDynamic(position, boxIntertia,
                    new CollidableDescription(_simulation.Shapes.Add(b), 0.1f),
                    new BodyActivityDescription(0.01f)));


            return _simulation.Bodies.GetBodyReference(handle);
        }

        internal static void AddBoxStatic(Vector3 position, Vector3 dimensions, ushort layer, ushort collidable)
        {
            _simulation.Statics.Add(new StaticDescription(position,
               new CollidableDescription(_simulation.Shapes.Add(new Box(dimensions.X, dimensions.Y, dimensions.Z)),
                   0.1f)));
        }

        public static void Update(float deltaTime)
        {
            _simulation.Timestep(deltaTime);
        }

        internal static BodyReference AddSphereDynamic(float mass, Vector3 position, float radius, ColliderComponent comp)
        {
            //Drop a ball on a big static box.
            var sphere = new Sphere(radius);
            sphere.ComputeInertia(mass, out var sphereInertia);
            int handle = _simulation.Bodies.Add(
                BodyDescription.CreateDynamic(position, sphereInertia,
                    new CollidableDescription(_simulation.Shapes.Add(sphere), 0.1f),
                    new BodyActivityDescription(0.01f)));
            return _simulation.Bodies.GetBodyReference(handle);

        }

        internal static void AddBoxStatic(Vector3 position, float radius, ushort layer, ushort collidable)
        {
            _simulation.Statics.Add(new StaticDescription(position, new CollidableDescription(_simulation.Shapes.Add(new Sphere(radius)), 0.1f)));

        }
    }
}