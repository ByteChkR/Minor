using System.Collections.Generic;
using Engine.Physics.BEPUphysics.Constraints;

namespace Engine.Physics.BEPUphysics.UpdateableSystems
{
    ///<summary>
    /// A class which is both a space updateable and a Solver Updateable.
    ///</summary>
    public abstract class CombinedUpdateable : SolverUpdateable, ISpaceUpdateable
    {
        private bool isSequentiallyUpdated = true;

        protected CombinedUpdateable()
        {
            IsUpdating = true;
        }

        #region ISpaceUpdateable Members

        private List<UpdateableManager> managers = new List<UpdateableManager>();

        List<UpdateableManager> ISpaceUpdateable.Managers => managers;

        /// <summary>
        /// Gets and sets whether or not the updateable should be updated sequentially even in a multithreaded space.
        /// If this is true, the updateable can make use of the space's ParallelLooper for internal multithreading.
        /// </summary>
        public bool IsUpdatedSequentially
        {
            get => isSequentiallyUpdated;
            set
            {
                var oldValue = isSequentiallyUpdated;
                isSequentiallyUpdated = value;
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
        /// Gets and sets whether or not the updateable should be updated by the space.
        /// </summary>
        public bool IsUpdating { get; set; }

        #endregion
    }
}