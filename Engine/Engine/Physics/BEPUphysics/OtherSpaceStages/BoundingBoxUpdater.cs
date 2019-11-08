﻿using System;
using Engine.Physics.BEPUphysics.BroadPhaseEntries.MobileCollidables;
using Engine.Physics.BEPUutilities;
using Engine.Physics.BEPUutilities.DataStructures;
using Engine.Physics.BEPUutilities.Threading;

namespace Engine.Physics.BEPUphysics.OtherSpaceStages
{
    ///<summary>
    /// Updates the bounding box of managed objects.
    ///</summary>
    public class BoundingBoxUpdater : MultithreadedProcessingStage
    {
        //TODO: should the Entries field be publicly accessible since there's not any custom add/remove logic?
        private RawList<MobileCollidable> entries = new RawList<MobileCollidable>();
        private TimeStepSettings timeStepSettings;

        ///<summary>
        /// Gets or sets the time step settings used by the updater.
        ///</summary>
        public TimeStepSettings TimeStepSettings { get; set; }

        ///<summary>
        /// Constructs the bounding box updater.
        ///</summary>
        ///<param name="timeStepSettings">Time step setttings to be used by the updater.</param>
        public BoundingBoxUpdater(TimeStepSettings timeStepSettings)
        {
            multithreadedLoopBodyDelegate = LoopBody;
            Enabled = true;
            this.timeStepSettings = timeStepSettings;
        }

        ///<summary>
        /// Constructs the bounding box updater.
        ///</summary>
        ///<param name="timeStepSettings">Time step setttings to be used by the updater.</param>
        /// <param name="parallelLooper">Parallel loop provider to be used by the updater.</param>
        public BoundingBoxUpdater(TimeStepSettings timeStepSettings, IParallelLooper parallelLooper)
            : this(timeStepSettings)
        {
            ParallelLooper = parallelLooper;
            AllowMultithreading = true;
        }

        private Action<int> multithreadedLoopBodyDelegate;

        private void LoopBody(int i)
        {
            MobileCollidable entry = entries.Elements[i];
            if (entry.IsActive)
            {
                entry.UpdateBoundingBox(timeStepSettings.TimeStepDuration);
                entry.boundingBox.Validate();
            }
        }

        ///<summary>
        /// Adds an entry to the updater.
        ///</summary>
        ///<param name="entry">Entry to add.</param>
        public void Add(MobileCollidable entry)
        {
            //TODO: Contains check?
            entries.Add(entry);
        }

        ///<summary>
        /// Removes an entry from the updater.
        ///</summary>
        ///<param name="entry">Entry to remove.</param>
        public void Remove(MobileCollidable entry)
        {
            entries.Remove(entry);
        }

        protected override void UpdateMultithreaded()
        {
            ParallelLooper.ForLoop(0, entries.Count, multithreadedLoopBodyDelegate);
        }

        protected override void UpdateSingleThreaded()
        {
            for (int i = 0; i < entries.Count; ++i)
            {
                LoopBody(i);
            }
        }
    }
}