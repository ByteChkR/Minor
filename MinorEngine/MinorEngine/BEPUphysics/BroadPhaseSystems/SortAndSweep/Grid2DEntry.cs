using MinorEngine.BEPUphysics.BroadPhaseEntries;

namespace MinorEngine.BEPUphysics.BroadPhaseSystems.SortAndSweep
{
    internal class Grid2DEntry
    {
        internal void Initialize(BroadPhaseEntry entry)
        {
            item = entry;
            Grid2DSortAndSweep.ComputeCell(ref entry.boundingBox.Min, out previousMin);
            Grid2DSortAndSweep.ComputeCell(ref entry.boundingBox.Max, out previousMax);
        }


        internal BroadPhaseEntry item;
        internal Int2 previousMin;
        internal Int2 previousMax;
    }
}