using System;
using Engine.Physics.BEPUphysics.BroadPhaseEntries;
using Engine.Physics.BEPUphysics.BroadPhaseEntries.MobileCollidables;
using Engine.Physics.BEPUphysics.DataStructures;
using Engine.Physics.BEPUutilities.DataStructures;

namespace Engine.Physics.BEPUphysics.NarrowPhaseSystems.Pairs
{
    ///<summary>
    /// Handles a compound-compound collision pair.
    ///</summary>
    public class CompoundPairHandler : CompoundGroupPairHandler
    {
        private CompoundCollidable compoundInfoB;

        //Some danger of unintuitive-to-address allocations here.  If these lists get huge, the user will see some RawList<<>> goofiness in the profiler.
        //They can still address it by clearing out the cached pair factories though.
        private RawList<TreeOverlapPair<CompoundChild, CompoundChild>> overlappedElements =
            new RawList<TreeOverlapPair<CompoundChild, CompoundChild>>();

        public override Collidable CollidableB => compoundInfoB;

        public override Entities.Entity EntityB => compoundInfoB.entity;


        ///<summary>
        /// Initializes the pair handler.
        ///</summary>
        ///<param name="entryA">First entry in the pair.</param>
        ///<param name="entryB">Second entry in the pair.</param>
        public override void Initialize(BroadPhaseEntry entryA, BroadPhaseEntry entryB)
        {
            compoundInfoB = entryB as CompoundCollidable;
            if (compoundInfoB == null)
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
            compoundInfoB = null;
        }

        protected override void UpdateContainedPairs()
        {
            compoundInfo.hierarchy.Tree.GetOverlaps(compoundInfoB.hierarchy.Tree, overlappedElements);
            for (int i = 0; i < overlappedElements.Count; i++)
            {
                TreeOverlapPair<CompoundChild, CompoundChild> element = overlappedElements.Elements[i];
                TryToAdd(element.OverlapA.CollisionInformation, element.OverlapB.CollisionInformation,
                    element.OverlapA.Material, element.OverlapB.Material);
            }

            overlappedElements.Clear();
        }
    }
}