using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.CompilerServices;
using BepuPhysics;
using BepuPhysics.Collidables;
using BepuPhysics.CollisionDetection;
using BepuUtilities;
using BepuUtilities.Memory;
using GameEngine.components;
using GameEngine.engine.core;
using GameEngine.engine.physics.callbacks;
using NarrowPhase = GameEngine.engine.physics.callbacks.NarrowPhase;
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
        public static Vector3 Gravity { get; set; } = new Vector3(0, -10, 0);
        private static IThreadDispatcher ThreadDispatcher { get; set; }
        public static void Init()
        {
            //The buffer pool is a source of raw memory blobs for the engine to use.
            var bufferPool = new BufferPool();
            //Note that you can also control the order of internal stage execution using a different ITimestepper implementation.
            //For the purposes of this demo, we just use the default by passing in nothing (which happens to be PositionFirstTimestepper at the time of writing).
            _simulation = Simulation.Create(bufferPool, new NarrowPhase(), new PoseIntegrator());
            ThreadDispatcher = new ThreadDispatcher(SceneRunner.Instance.Settings.PhysicsThreadCount);
        }

        
        public static void SetLayerDetection(int bodyhandle, int otherBodyHandle)
        {
            ref var thisLayer = ref CollisionFilters.Allocate(bodyhandle);
            ref var otherLayer = ref CollisionFilters.Allocate(otherBodyHandle);
            Layer.DisableCollision(ref thisLayer, ref otherLayer);
        }

        public static BodyReference AddBoxDynamic(float mass, Vector3 position, Vector3 dimensions)
        {
            Box b = new Box(dimensions.X, dimensions.Y, dimensions.Z);
            b.ComputeInertia(1, out var boxIntertia);
            int handle = _simulation.Bodies.Add(
                BodyDescription.CreateDynamic(position, boxIntertia,
                    new CollidableDescription(_simulation.Shapes.Add(b), 0.1f),
                    new BodyActivityDescription(0.01f)));


            return _simulation.Bodies.GetBodyReference(handle);
        }

        public static BodyReference AddMeshDynamic(float mass, Vector3 position, Mesh mesh)
        {
            mesh.ComputeClosedInertia(mass, out var intertia);
            int handle = _simulation.Bodies.Add(
                BodyDescription.CreateDynamic(position, intertia,
                    new CollidableDescription(_simulation.Shapes.Add(mesh), 0.1f),
                    new BodyActivityDescription(0.01f)));
            return _simulation.Bodies.GetBodyReference(handle);
        }

        //TODO
        public static void AddBoxStatic(Vector3 position, Vector3 dimensions, ushort layer, ushort collidable)
        {
            _simulation.Statics.Add(new StaticDescription(position,
               new CollidableDescription(_simulation.Shapes.Add(new Box(dimensions.X, dimensions.Y, dimensions.Z)),
                   0.1f)));
        }

        public static void Update(float deltaTime)
        {
            _simulation.Timestep(deltaTime, ThreadDispatcher);
        }

        internal static BodyReference AddSphereDynamic(float mass, Vector3 position, float radius)
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