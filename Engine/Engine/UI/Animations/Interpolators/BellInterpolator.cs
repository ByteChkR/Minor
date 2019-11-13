using Engine.Core;

namespace Engine.UI.Animations.Interpolators
{
    public class BellInterpolator : Interpolator
    {
        public Interpolator Smoothness = new StaticInterpolator();

        protected override float _GetValue(float input)
        {
            return Interpolations.BellCurve(input, Smoothness.GetValue(input));
        }
    }
}