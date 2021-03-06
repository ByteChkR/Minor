using System.Collections.Generic;
using Engine.Physics.BEPUphysics.BroadPhaseEntries.MobileCollidables;
using Engine.Physics.BEPUphysics.CollisionShapes.ConvexShapes;
using Engine.Physics.BEPUphysics.EntityStateManagement;
using Engine.Physics.BEPUutilities;
using Engine.Physics.BEPUutilities.DataStructures;

namespace Engine.Physics.BEPUphysics.Entities.Prefabs
{
    /// <summary>
    /// Shape that can collide and move based on the convex 'outer layer' of a list of points.  After making an entity, add it to a Space so that the engine can manage it.
    /// </summary>
    public class ConvexHull : Entity<ConvexCollidable<ConvexHullShape>>
    {
        /// <summary>
        /// List of the points composing the surface of the convex hull in local space.
        /// </summary>
        public ReadOnlyList<Vector3> Vertices => CollisionInformation.Shape.Vertices;


        /// <summary>
        /// Constructs a nondynamic convex hull of points.
        /// </summary>
        /// <param name="points">List of points in the object.</param>
        public ConvexHull(IList<Vector3> points)
        {
            Vector3 center;
            ConvexHullShape shape = new ConvexHullShape(points, out center);
            Initialize(new ConvexCollidable<ConvexHullShape>(shape));
            Position = center;
        }


        /// <summary>
        /// Constructs a physically simulated convex hull of points.
        /// </summary>
        /// <param name="points">List of points in the object.</param>
        /// <param name="mass">Mass of the object.</param>
        public ConvexHull(IList<Vector3> points, float mass)
        {
            Vector3 center;
            ConvexHullShape shape = new ConvexHullShape(points, out center);
            Initialize(new ConvexCollidable<ConvexHullShape>(shape), mass);
            Position = center;
        }

        /// <summary>
        /// Constructs a physically simulated convex hull of points.
        /// </summary>
        /// <param name="position">Position to place the convex hull.</param>
        /// <param name="points">List of points in the object.</param>
        /// <param name="mass">Mass of the object.</param>
        public ConvexHull(Vector3 position, IList<Vector3> points, float mass)
            : this(points, mass)
        {
            Position = position;
        }


        /// <summary>
        /// Constructs a nondynamic convex hull of points.
        /// </summary>
        /// <param name="position">Position to place the convex hull.</param>
        /// <param name="points">List of points in the object.</param>
        public ConvexHull(Vector3 position, IList<Vector3> points)
            : this(points)
        {
            Position = position;
        }

        /// <summary>
        /// Constructs a physically simulated convex hull of points.
        /// </summary>
        /// <param name="motionState">Motion state specifying the entity's initial state.</param>
        /// <param name="points">List of points in the object.</param>
        /// <param name="mass">Mass of the object.</param>
        public ConvexHull(MotionState motionState, IList<Vector3> points, float mass)
            : this(points, mass)
        {
            MotionState = motionState;
        }


        /// <summary>
        /// Constructs a nondynamic convex hull of points.
        /// </summary>
        /// <param name="motionState">Motion state specifying the entity's initial state.</param>
        /// <param name="points">List of points in the object.</param>
        public ConvexHull(MotionState motionState, IList<Vector3> points)
            : this(points)
        {
            MotionState = motionState;
        }
    }
}