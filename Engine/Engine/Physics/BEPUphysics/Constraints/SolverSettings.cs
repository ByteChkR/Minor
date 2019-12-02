using System;

namespace Engine.Physics.BEPUphysics.Constraints
{
    /// <summary>
    /// Contains information about how a wheel solves its constraints.
    /// </summary>
    public class SolverSettings
    {
        /// <summary>
        /// The value to assign to new constraints' SolverSettings.MinimumImpulse.
        /// Impulses with magnitudes below this value will count as effectively zero in determining iteration early outs unless changed in the constraint's solver settings.
        /// High values quicken the short circuit but can cause instability, while low values will often prevent short circuiting, possibly increasing accuracy but harming performance.
        /// Defaults to .001f.
        /// </summary>
        public static float DefaultMinimumImpulse = .001f;

        /// <summary>
        /// The value to assign to new constraints' SolverSettings.MinimumIterations.
        /// Constraints are able to skip extra calculations if deemed appropriate after they complete the minimum iterations.
        /// Higher values force the system to wait longer before trying to early out, possibly improving behavior.
        /// Defaults to 1.
        /// </summary>
        public static int DefaultMinimumIterationCount = 1;

        /// <summary>
        /// Used to count how many iterations have taken place so far.
        /// </summary>
        internal int currentIterations;

        internal int iterationsAtZeroImpulse;

        internal int maximumIterationCount = int.MaxValue;

        internal float minimumImpulse = DefaultMinimumImpulse;

        internal int minimumIterationCount = DefaultMinimumIterationCount;

        /// <summary>
        /// Gets or sets the maximum iterations that the wheel constraint can undergo.
        /// If the space's iteration count is lower than this, the solver will only attempt
        /// as many iterations as the space iteration count.
        /// Lower iteration counts are less accurate, but can improve performance.
        /// </summary>
        public int MaximumIterationCount
        {
            get => maximumIterationCount;
            set => maximumIterationCount = Math.Max(value, 0);
        }

        /// <summary>
        /// Gets or sets the minimum number of iterations that will be applied.
        /// If an impulse of magnitude smaller than the MinimumImpulse is applied, a 'tiny impulses' counter increases.  Once it exceeds the MinimumIterations,
        /// the system can decide to stop solving to save time if appropriate.
        /// </summary>
        public int MinimumIterationCount
        {
            get => minimumIterationCount;
            set => minimumIterationCount = Math.Max(value, 0);
        }

        /// <summary>
        /// Gets or sets the lower limit for impulses.  Impulses applied with magnitudes less than this will increment the 'tiny impulse' counter, which is checked
        /// against the MinimumIterations property.  If there's been too many tiny impulses in a row, then the system will stop trying to solve to save time.
        /// Higher values will allow the system to give up earlier, but can harm accuracy.
        /// </summary>
        public float MinimumImpulse
        {
            get => minimumImpulse;
            set => minimumImpulse = Math.Max(value, 0);
        }
    }
}