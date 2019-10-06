using System;
using MinorEngine.BEPUphysics.DataStructures;

namespace MinorEngine.BEPUphysics.BroadPhaseEntries.MobileCollidables
{
    ///<summary>
    /// Hierarchy of children used to accelerate queries and tests for compound collidables.
    ///</summary>
    public class CompoundHierarchy
    {
        ///<summary>
        /// Gets the bounding box tree of the hierarchy.
        ///</summary>
        public BoundingBoxTree<CompoundChild> Tree { get; }

        ///<summary>
        /// Gets the CompoundCollidable that owns this hierarchy.
        ///</summary>
        public CompoundCollidable Owner { get; }

        ///<summary>
        /// Constructs a new compound hierarchy.
        ///</summary>
        ///<param name="owner">Owner of the hierarchy.</param>
        public CompoundHierarchy(CompoundCollidable owner)
        {
            Owner = owner;
            var children = new CompoundChild[owner.children.Count];
            Array.Copy(owner.children.Elements, children, owner.children.Count);
            //In order to initialize a good tree, the local space bounding boxes should first be computed.
            //Otherwise, the tree would try to create a hierarchy based on a bunch of zeroed out bounding boxes!
            for (var i = 0; i < children.Length; i++)
            {
                children[i].CollisionInformation.worldTransform = owner.Shape.shapes.Elements[i].LocalTransform;
                children[i].CollisionInformation.UpdateBoundingBoxInternal(0);
            }

            Tree = new BoundingBoxTree<CompoundChild>(children);
        }
    }
}