using System;
using Engine.Physics.BEPUphysics.BroadPhaseEntries.Events;
using Engine.Physics.BEPUphysics.CollisionRuleManagement;
using Engine.Physics.BEPUphysics.CollisionShapes;
using Engine.Physics.BEPUphysics.NarrowPhaseSystems.Pairs;
using Engine.Physics.BEPUutilities.DataStructures;

namespace Engine.Physics.BEPUphysics.BroadPhaseEntries
{
    ///<summary>
    /// Superclass of objects living in the collision detection pipeline
    /// that can result in contacts.
    ///</summary>
    public abstract class Collidable : BroadPhaseEntry
    {
        internal RawList<CollidablePairHandler> pairs = new RawList<CollidablePairHandler>();


        protected internal CollisionShape
            shape; //Having this non-private allows for some very special-casey stuff; see TriangleShape initialization.

        private Action<CollisionShape> shapeChangedDelegate;

        private bool shapeChangedHooked = true;

        protected Collidable()
        {
            shapeChangedDelegate = OnShapeChanged;
        }

        ///<summary>
        /// Gets the shape used by the collidable.
        ///</summary>
        public CollisionShape Shape
        {
            get => shape;
            protected set
            {
                if (shape != null && shapeChangedHooked)
                {
                    shape.ShapeChanged -= shapeChangedDelegate;
                }

                shape = value;
                if (shape != null && shapeChangedHooked)
                {
                    shape.ShapeChanged += shapeChangedDelegate;
                }

                OnShapeChanged(shape);
            }
        }

        /// <summary>
        /// Gets or sets whether the shape changed event is hooked. Setting this modifies the event delegate list on the associated shape, if any shape exists.
        /// If no shape exists, getting this property returns whether the event would be hooked if a shape was present.
        /// </summary>
        /// <remarks>Yes, this is a hack.</remarks>
        public bool ShapeChangedHooked
        {
            get => shapeChangedHooked;
            set
            {
                if (shape != null)
                {
                    if (shapeChangedHooked && !value)
                    {
                        shape.ShapeChanged -= shapeChangedDelegate;
                    }
                    else if (!shapeChangedHooked && value)
                    {
                        shape.ShapeChanged += shapeChangedDelegate;
                    }
                }

                shapeChangedHooked = value;
            }
        }

        protected internal abstract IContactEventTriggerer EventTriggerer { get; }


        /// <summary>
        /// Gets or sets whether or not to ignore shape changes.  When true, changing the collision shape will not force the collidable to perform any updates.
        /// Does not modify the shape changed event delegate list.
        /// </summary>
        public bool IgnoreShapeChanges { get; set; }

        ///<summary>
        /// Gets the list of pairs associated with the collidable.
        /// These pairs are found by the broad phase and are managed by the narrow phase;
        /// they can contain other collidables, entities, and contacts.
        ///</summary>
        public ReadOnlyList<CollidablePairHandler> Pairs => new ReadOnlyList<CollidablePairHandler>(pairs);

        ///<summary>
        /// Gets a list of all other collidables that this collidable overlaps.
        ///</summary>
        public CollidableCollection OverlappedCollidables => new CollidableCollection(this);

        protected virtual void OnShapeChanged(CollisionShape collisionShape)
        {
        }

        protected override void CollisionRulesUpdated()
        {
            for (int i = 0; i < pairs.Count; i++)
            {
                pairs[i].CollisionRule = CollisionRules.CollisionRuleCalculator(pairs[i].BroadPhaseOverlap.entryA,
                    pairs[i].BroadPhaseOverlap.entryB);
            }
        }


        internal void AddPair(CollidablePairHandler pair, ref int index)
        {
            index = pairs.Count;
            pairs.Add(pair);
        }

        internal void RemovePair(CollidablePairHandler pair, ref int index)
        {
            if (pairs.Count > index)
            {
                pairs.FastRemoveAt(index);
                if (pairs.Count > index)
                {
                    CollidablePairHandler endPair = pairs.Elements[index];
                    if (endPair.CollidableA == this)
                    {
                        endPair.listIndexA = index;
                    }
                    else
                    {
                        endPair.listIndexB = index;
                    }
                }
            }

            index = -1;
        }
    }
}