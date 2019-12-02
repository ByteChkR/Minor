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
    public class StaticGroupCompoundPairHandler : StaticGroupPairHandler
    {
        private CompoundCollidable compoundInfo;

        //Some danger of unintuitive-to-address allocations here.  If these lists get huge, the user will see some RawList<<>> goofiness in the profiler.
        //They can still address it by clearing out the cached pair factories though.
        private RawList<TreeOverlapPair<Collidable, CompoundChild>> overlappedElements =
            new RawList<TreeOverlapPair<Collidable, CompoundChild>>();

        public override Collidable CollidableB => compoundInfo;

        public override Entities.Entity EntityB => compoundInfo.entity;


        ///<summary>
        /// Initializes the pair handler.
        ///</summary>
        ///<param name="entryA">First entry in the pair.</param>
        ///<param name="entryB">Second entry in the pair.</param>
        public override void Initialize(BroadPhaseEntry entryA, BroadPhaseEntry entryB)
        {
            compoundInfo = entryA as CompoundCollidable;
            if (compoundInfo == null)
            {
                compoundInfo = entryB as CompoundCollidable;
                if (compoundInfo == null)
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
            compoundInfo = null;
        }

        protected override void UpdateContainedPairs()
        {
            staticGroup.Shape.CollidableTree.GetOverlaps(compoundInfo.hierarchy.Tree, overlappedElements);
            for (int i = 0; i < overlappedElements.Count; i++)
            {
                TreeOverlapPair<Collidable, CompoundChild> element = overlappedElements.Elements[i];
                StaticCollidable staticCollidable = element.OverlapA as StaticCollidable;
                TryToAdd(element.OverlapA, element.OverlapB.CollisionInformation,
                    staticCollidable != null ? staticCollidable.Material : staticGroup.Material,
                    element.OverlapB.Material);
            }

            overlappedElements.Clear();
        }
    }
}