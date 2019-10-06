using System;
using MinorEngine.BEPUphysics.BroadPhaseEntries;
using MinorEngine.BEPUphysics.BroadPhaseEntries.MobileCollidables;
using MinorEngine.BEPUphysics.CollisionShapes.ConvexShapes;
using MinorEngine.BEPUphysics.CollisionTests.Manifolds;

namespace MinorEngine.BEPUphysics.NarrowPhaseSystems.Pairs
{
    ///<summary>
    /// Pair handler that manages a pair of two boxes.
    ///</summary>
    public class BoxPairHandler : ConvexConstraintPairHandler
    {
        private ConvexCollidable<BoxShape> boxA;
        private ConvexCollidable<BoxShape> boxB;

        private BoxContactManifold contactManifold = new BoxContactManifold();

        public override Collidable CollidableA => boxA;

        public override Collidable CollidableB => boxB;

        public override Entities.Entity EntityA => boxA.entity;

        public override Entities.Entity EntityB => boxB.entity;

        /// <summary>
        /// Gets the contact manifold used by the pair handler.
        /// </summary>
        public override ContactManifold ContactManifold => contactManifold;


        ///<summary>
        /// Initializes the pair handler.
        ///</summary>
        ///<param name="entryA">First entry in the pair.</param>
        ///<param name="entryB">Second entry in the pair.</param>
        public override void Initialize(BroadPhaseEntry entryA, BroadPhaseEntry entryB)
        {
            boxA = entryA as ConvexCollidable<BoxShape>;
            boxB = entryB as ConvexCollidable<BoxShape>;

            if (boxA == null || boxB == null)
            {
                throw new ArgumentException("Inappropriate types used to initialize pair.");
            }

            base.Initialize(entryA, entryB);
        }


        ///<summary>
        /// Cleans up the pair handler.
        ///</summary>
        public override void CleanUp()
        {
            base.CleanUp();

            boxA = null;
            boxB = null;
        }
    }
}