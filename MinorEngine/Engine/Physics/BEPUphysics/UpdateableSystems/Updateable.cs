using System.Collections.Generic;

namespace Engine.Physics.BEPUphysics.UpdateableSystems
{
    ///<summary>
    /// Convenience superclass of Updateables.
    /// Updateables are updated by the Space at various
    /// points during the execution of the engine
    /// to support easy extensions.
    ///</summary>
    public abstract class Updateable : ISpaceUpdateable
    {
        protected Updateable()
        {
            IsUpdating = true;
        }

        #region ISpaceUpdateable Members

        private List<UpdateableManager> managers = new List<UpdateableManager>();

        List<UpdateableManager> ISpaceUpdateable.Managers => managers;

        private bool isUpdatedSequentially = true;

        /// <summary>
        /// Gets and sets whether or not the updateable should be updated sequentially even in a multithreaded space.
        /// If this is true, the updateable can make use of the space's ParallelLooper for internal multithreading.
        /// </summary>
        public bool IsUpdatedSequentially
        {
            get => isUpdatedSequentially;
            set
            {
                var oldValue = isUpdatedSequentially;
                isUpdatedSequentially = value;
                if (value != oldValue)
                {
                    for (var i = 0; i < managers.Count; i++)
                    {
                        managers[i].SequentialUpdatingStateChanged(this);
                    }
                }
            }
        }


        /// <summary>
        /// Gets and sets whether or not the updateable should be updated by its manager.
        /// </summary>
        public bool IsUpdating { get; set; }


        /// <summary>
        /// Called after the object is added to a space.
        /// </summary>
        /// <param name="newSpace">Space to which the object was added.</param>
        public virtual void OnAdditionToSpace(Space newSpace)
        {
        }


        /// <summary>
        /// Called before an object is removed from its space.
        /// </summary>
        /// <param name="oldSpace">Space from which the object was removed.</param>
        public virtual void OnRemovalFromSpace(Space oldSpace)
        {
        }

        Space ISpaceObject.Space
        {
            get => Space;
            set => Space = value;
        }

        ///<summary>
        /// Space that owns the updateable.
        ///</summary>
        public Space Space { get; private set; }

        /// <summary>
        /// Gets or sets the user data associated with this object.
        /// </summary>
        public object Tag { get; set; }

        #endregion
    }
}