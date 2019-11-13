using Engine.Core;

namespace Engine.UI.Animations.Interpolators
{
    public class SphericalInterpolator : Interpolator
    {
        protected override float _GetValue(float input)
        {
            return Interpolations.Slerp(input);
        }
    }
}