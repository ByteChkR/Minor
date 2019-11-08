using System;
using Engine.Physics.BEPUphysics.BroadPhaseEntries;
using Engine.Physics.BEPUphysics.BroadPhaseEntries.MobileCollidables;
using Engine.Physics.BEPUutilities.DataStructures;

namespace Engine.Physics.BEPUphysics.NarrowPhaseSystems.Pairs
{
    ///<summary>
    /// Handles a compound and convex collision pair.
    ///</summary>
    public class StaticGroupConvexPairHandler : StaticGroupPairHandler
    {
        private ConvexCollidable convexInfo;


        public override Collidable CollidableB => convexInfo;

        public override Entities.Entity EntityB => convexInfo.entity;


        ///<summary>
        /// Initializes the pair handler.
        ///</summary>
        ///<param name="entryA">First entry in the pair.</param>
        ///<param name="entryB">Second entry in the pair.</param>
        public override void Initialize(BroadPhaseEntry entryA, BroadPhaseEntry entryB)
        {
            convexInfo = entryA as ConvexCollidable;
            if (convexInfo == null)
            {
                convexInfo = entryB as ConvexCollidable;
                if (convexInfo == null)
                {
                    throw new ArgumentException("Inappropriate types used to initialize pair.");
                }
            }

            base.Initialize(entryA, entryB);
        }


        ///<summary>
        /// Cleans up the pair handler.
        ///</summary>
        public override void CleanUp()
        {
            base.CleanUp();
            convexInfo = null;
        }


        protected override void UpdateContainedPairs()
        {
            RawList<Collidable> overlappedElements = PhysicsResources.GetCollidableList();
            staticGroup.Shape.CollidableTree.GetOverlaps(convexInfo.boundingBox, overlappedElements);
            for (int i = 0; i < overlappedElements.Count; i++)
            {
                StaticCollidable staticCollidable = overlappedElements.Elements[i] as StaticCollidable;
                TryToAdd(overlappedElements.Elements[i], CollidableB,
                    staticCollidable != null ? staticCollidable.Material : staticGroup.Material);
            }

            PhysicsResources.GiveBack(overlappedElements);
        }
    }
}