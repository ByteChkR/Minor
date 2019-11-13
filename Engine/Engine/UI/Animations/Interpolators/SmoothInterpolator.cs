using Engine.Core;

namespace Engine.UI.Animations.Interpolators
{
    public class SmoothInterpolator : Interpolator
    {
        public Interpolator SmoothnessStart = new StaticInterpolator();
        public Interpolator SmoothnessStop = new StaticInterpolator() {Value = 1};

        protected override float _GetValue(float input)
        {
            return Interpolations.SmoothStep(input, SmoothnessStart.GetValue(input), SmoothnessStop.GetValue(input));
        }
    }
}