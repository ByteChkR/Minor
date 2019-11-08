using System.Collections.Generic;
using Engine.Physics.BEPUphysics.BroadPhaseEntries;
using Engine.Physics.BEPUphysics.BroadPhaseEntries.MobileCollidables;
using Engine.Physics.BEPUphysics.Entities;
using Engine.Physics.BEPUutilities;
using Engine.Physics.BEPUutilities.DataStructures;

namespace Engine.Physics.BEPUphysics.UpdateableSystems.ForceFields
{
    /// <summary>
    /// Defines the area in which a force field works using an entity's shape.
    /// </summary>
    public class BoundingSphereForceFieldShape : ForceFieldShape
    {
        private readonly List<Entity> affectedEntities = new List<Entity>();
        private readonly RawList<BroadPhaseEntry> affectedEntries = new RawList<BroadPhaseEntry>();

        /// <summary>
        /// Constructs a new force field shape using a bounding sphere.
        /// </summary>
        /// <param name="sphere">Bounding sphere to use.</param>
        public BoundingSphereForceFieldShape(BoundingSphere sphere)
        {
            BoundingSphere = sphere;
        }

        /// <summary>
        /// Gets or sets the bounding box used by the shape.
        /// </summary>
        public BoundingSphere BoundingSphere { get; set; }

        /// <summary>
        /// Determines the possibly involved entities.
        /// </summary>
        /// <returns>Possibly involved entities.</returns>
        public override IList<Entity> GetPossiblyAffectedEntities()
        {
            affectedEntities.Clear();
            ForceField.QueryAccelerator.GetEntries(BoundingSphere, affectedEntries);
            for (int i = 0; i < affectedEntries.Count; i++)
            {
                EntityCollidable EntityCollidable = affectedEntries[i] as EntityCollidable;
                if (EntityCollidable != null)
                {
                    affectedEntities.Add(EntityCollidable.Entity);
                }
            }

            affectedEntries.Clear();
            return affectedEntities;
        }

        /// <summary>
        /// Determines if the entity is affected by the force field.
        /// </summary>
        /// <param name="testEntity">Entity to test.</param>
        /// <returns>Whether the entity is affected.</returns>
        public override bool IsEntityAffected(Entity testEntity)
        {
            return true;
        }
    }
}