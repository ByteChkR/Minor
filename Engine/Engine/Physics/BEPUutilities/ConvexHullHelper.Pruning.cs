using System;
using System.Collections.Generic;
using Engine.Physics.BEPUutilities.DataStructures;
using Engine.Physics.BEPUutilities.ResourceManagement;

namespace Engine.Physics.BEPUutilities
{
    public static partial class ConvexHullHelper
    {
        /// <summary>
        /// Contains and manufactures cell sets used by the redundant point remover.  To minimize memory usage, this can be cleared
        /// after using the RemoveRedundantPoints if it isn't going to be used again.
        /// </summary>
        public static LockingResourcePool<HashSet<BlockedCell>> BlockedCellSets =
            new LockingResourcePool<HashSet<BlockedCell>>();

        /// <summary>
        /// Removes redundant points.  Two points are redundant if they occupy the same hash grid cell of size 0.001.
        /// </summary>
        /// <param name="points">List of points to prune.</param>
        public static void RemoveRedundantPoints(IList<Vector3> points)
        {
            RemoveRedundantPoints(points, .001);
        }

        /// <summary>
        /// Removes redundant points.  Two points are redundant if they occupy the same hash grid cell.
        /// </summary>
        /// <param name="points">List of points to prune.</param>
        /// <param name="cellSize">Size of cells to determine redundancy.</param>
        public static void RemoveRedundantPoints(IList<Vector3> points, double cellSize)
        {
            RawList<Vector3> rawPoints = CommonResources.GetVectorList();
            rawPoints.AddRange(points);
            RemoveRedundantPoints(rawPoints, cellSize);
            points.Clear();
            for (int i = 0; i < rawPoints.Count; ++i)
            {
                points.Add(rawPoints.Elements[i]);
            }

            CommonResources.GiveBack(rawPoints);
        }

        /// <summary>
        /// Removes redundant points.  Two points are redundant if they occupy the same hash grid cell of size 0.001.
        /// </summary>
        /// <param name="points">List of points to prune.</param>
        public static void RemoveRedundantPoints(RawList<Vector3> points)
        {
            RemoveRedundantPoints(points, .001);
        }

        /// <summary>
        /// Removes redundant points.  Two points are redundant if they occupy the same hash grid cell.
        /// </summary>
        /// <param name="points">List of points to prune.</param>
        /// <param name="cellSize">Size of cells to determine redundancy.</param>
        public static void RemoveRedundantPoints(RawList<Vector3> points, double cellSize)
        {
            HashSet<BlockedCell> set = BlockedCellSets.Take();
            for (int i = points.Count - 1; i >= 0; --i)
            {
                Vector3 element = points.Elements[i];
                BlockedCell cell = new BlockedCell
                {
                    X = (int) Math.Floor(element.X / cellSize),
                    Y = (int) Math.Floor(element.Y / cellSize),
                    Z = (int) Math.Floor(element.Z / cellSize)
                };
                if (set.Contains(cell))
                {
                    points.FastRemoveAt(i);
                }
                else
                {
                    set.Add(cell);
                }

                //TODO: Consider adding adjacent cells to guarantee that a point on the border between two cells will still detect the presence
                //of a point on the opposite side of that border.
            }

            set.Clear();
            BlockedCellSets.GiveBack(set);
        }

        /// <summary>
        /// Represents a cell in space which is already occupied by a point.  Any other points which resolve to the same cell are considered redundant.
        /// </summary>
        public struct BlockedCell : IEquatable<BlockedCell>
        {
            public int X;
            public int Y;
            public int Z;

            public override int GetHashCode()
            {
                const long p1 = 961748927L;
                const long p2 = 961748941L;
                const long p3 = 982451653L;
                return (int) (X * p1 + Y * p2 + Z * p3);
            }

            public override bool Equals(object obj)
            {
                return Equals((BlockedCell) obj);
            }

            public bool Equals(BlockedCell other)
            {
                return other.X == X && other.Y == Y && other.Z == Z;
            }
        }
    }
}