using Engine.Core;

namespace Engine.UI.Animations.Interpolators
{
    public class LinearInterpolator : Interpolator
    {
        protected override float _GetValue(float input)
        {
            return input;
        }
    }
}