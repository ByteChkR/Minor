using Engine.Physics.BEPUphysics.BroadPhaseEntries;

namespace Engine.Physics.BEPUphysics.BroadPhaseSystems.SortAndSweep
{
    internal class Grid2DEntry
    {
        internal BroadPhaseEntry item;
        internal Int2 previousMax;
        internal Int2 previousMin;

        internal void Initialize(BroadPhaseEntry entry)
        {
            item = entry;
            Grid2DSortAndSweep.ComputeCell(ref entry.boundingBox.Min, out previousMin);
            Grid2DSortAndSweep.ComputeCell(ref entry.boundingBox.Max, out previousMax);
        }
    }
}