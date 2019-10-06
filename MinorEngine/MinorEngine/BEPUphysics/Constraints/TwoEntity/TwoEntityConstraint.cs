using MinorEngine.BEPUphysics.Entities;
using MinorEngine.BEPUphysics.Entities.Prefabs;
using MinorEngine.BEPUutilities;
using MinorEngine.BEPUutilities.DataStructures;

namespace MinorEngine.BEPUphysics.Constraints.TwoEntity
{
    /// <summary>
    /// Abstract superclass of constraints involving two bodies.
    /// </summary>
    public abstract class TwoEntityConstraint : SolverUpdateable
    {
        /// <summary>
        /// Entity that constraints connect to when they are given a null connection.
        /// </summary>
        public static readonly Entity WorldEntity = new Sphere(Vector3.Zero, 0);

        /// <summary>
        /// First connection to the constraint.
        /// </summary>
        protected internal Entity connectionA;


        /// <summary>
        /// Second connection to the constraint.
        /// </summary>
        protected internal Entity connectionB;


        /// <summary>
        /// Gets or sets the first connection to the constraint.
        /// </summary>
        public Entity ConnectionA
        {
            get => connectionA;
            set
            {
                connectionA = value ?? WorldEntity;
                OnInvolvedEntitiesChanged();
            }
        }

        /// <summary>
        /// Gets or sets the second connection to the constraint.
        /// </summary>
        public Entity ConnectionB
        {
            get => connectionB;
            set
            {
                connectionB = value ?? WorldEntity;
                OnInvolvedEntitiesChanged();
            }
        }


        /// <summary>
        /// Adds entities associated with the solver item to the involved entities list.
        /// Ensure that sortInvolvedEntities() is called at the end of the function.
        /// This allows the non-batched multithreading system to lock properly.
        /// </summary>
        protected internal override void CollectInvolvedEntities(RawList<Entity> outputInvolvedEntities)
        {
            if (connectionA != null && connectionA != WorldEntity)
            {
                outputInvolvedEntities.Add(connectionA);
            }

            if (connectionB != null && connectionB != WorldEntity)
            {
                outputInvolvedEntities.Add(connectionB);
            }
        }
    }
}