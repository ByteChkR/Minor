using Engine.Core;

namespace Engine.UI.Animations.Interpolators
{
    /// <summary>
    /// Smooth Interpolator implementation
    /// </summary>
    public class SmoothInterpolator : Interpolator
    {
        /// <summary>
        /// Interpolator for The Smooth Start
        /// </summary>
        public Interpolator SmoothnessStart { get; set; } = new StaticInterpolator();
        /// <summary>
        /// Interpolator for The Smooth Stop
        /// </summary>
        public Interpolator SmoothnessStop { get; set; } = new StaticInterpolator {Value = 1};

        /// <summary>
        /// Implements the Smooth Interpolation
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        protected override float _GetValue(float input)
        {
            return Interpolations.SmoothStep(input, SmoothnessStart.GetValue(input), SmoothnessStop.GetValue(input));
        }
    }
}