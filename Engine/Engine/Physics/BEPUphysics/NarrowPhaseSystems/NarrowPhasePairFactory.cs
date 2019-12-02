using System;
using Engine.Physics.BEPUphysics.NarrowPhaseSystems.Pairs;
using Engine.Physics.BEPUutilities.ResourceManagement;

namespace Engine.Physics.BEPUphysics.NarrowPhaseSystems
{
    /// <summary>
    /// Superclass of the generic typed NarrowPhasePairFactory.  Offers interaction with the factory on a INarrowPhasePair level.
    /// </summary>
    public abstract class NarrowPhasePairFactory
    {
        protected bool allowOnDemandConstruction = true;

        /// <summary>
        /// Gets or sets the number of elements in the pair factory that are ready to take.
        /// If the factory runs out, it will construct new instances to give away (unless AllowOnDemandConstruction is set to false).
        /// </summary>
        public abstract int Count { get; set; }

        /// <summary>
        /// Gets or sets whether or not to allow the factory to create additional instances when it runs
        /// out of its initial set.  Defaults to true.
        /// </summary>
        public bool AllowOnDemandConstruction
        {
            get => allowOnDemandConstruction;
            set => allowOnDemandConstruction = value;
        }

        ///<summary>
        /// Manufactures and returns a narrow phase pair for the given overlap.
        ///</summary>
        ///<returns>Narrow phase pair.</returns>
        public abstract NarrowPhasePair GetNarrowPhasePair();

        /// <summary>
        /// Returns a pair to the factory for re-use.
        /// </summary>
        /// <param name="pair">Pair to return.</param>
        public abstract void GiveBack(NarrowPhasePair pair);

        /// <summary>
        /// Ensures that the factory has at least the given number of elements ready to take.
        /// </summary>
        /// <param name="minimumCount">Minimum number of elements to ensure in the factory.</param>
        public void EnsureCount(int minimumCount)
        {
            if (Count < minimumCount)
            {
                Count = minimumCount;
            }
        }

        /// <summary>
        /// Ensures that the factory has at most the given number of elements ready to take.
        /// </summary>
        /// <param name="maximumCount">Maximum number of elements to allow in the factory.</param>
        public void CapCount(int maximumCount)
        {
            if (Count > maximumCount)
            {
                Count = maximumCount;
            }
        }

        /// <summary>
        /// Removes all elements from the factory.
        /// </summary>
        public abstract void Clear();
    }

    ///<summary>
    /// Manufactures a given type of narrow phase pairs.
    ///</summary>
    /// <typeparam name="T">Type of the pair to manufacture.</typeparam>
    public class NarrowPhasePairFactory<T> : NarrowPhasePairFactory where T : NarrowPhasePair, new()
    {
        private LockingResourcePool<T> pool = new LockingResourcePool<T>();


        /// <summary>
        /// Gets or sets the number of elements in the pair factory that are ready to take.
        /// If the factory runs out, it will construct new instances to give away (unless AllowOnDemandConstruction is set to false).
        /// </summary>
        public override int Count
        {
            get => pool.Count;
            set => pool.Initialize(value);
        }

        /// <summary>
        /// Get a resource from the factory.
        /// </summary>
        /// <returns>A pair from the factory.</returns>
        public override NarrowPhasePair GetNarrowPhasePair()
        {
            if (!allowOnDemandConstruction && pool.Count == 0)
            {
                throw new InvalidOperationException(
                    "Cannot request additional resources from this factory; it is exhausted.  Consider specifying a greater number of initial resources or setting AllowOnDemandConstruction to true.");
            }

            T pair = pool.Take();
            pair.NeedsUpdate = true;
            return pair;
        }

        /// <summary>
        /// Give a resource back to the factory.
        /// </summary>
        /// <param name="pair">Pair to return.</param>
        public override void GiveBack(NarrowPhasePair pair)
        {
            pair.NarrowPhase = null;
            pool.GiveBack((T) pair);
        }


        /// <summary>
        /// Removes all elements from the factory.
        /// </summary>
        public override void Clear()
        {
            pool.Clear();
        }
    }
}