using Engine.Core;

namespace Engine.UI.Animations.Interpolators
{
    /// <summary>
    /// Smooth Interpolator implementation
    /// </summary>
    public class SmoothInterpolator : Interpolator
    {
        public Interpolator SmoothnessStart { get; set; } = new StaticInterpolator();
        public Interpolator SmoothnessStop { get; set; } = new StaticInterpolator {Value = 1};

        protected override float _GetValue(float input)
        {
            return Interpolations.SmoothStep(input, SmoothnessStart.GetValue(input), SmoothnessStop.GetValue(input));
        }
    }
}