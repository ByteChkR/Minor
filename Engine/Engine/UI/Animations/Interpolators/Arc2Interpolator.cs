using Engine.Core;

namespace Engine.UI.Animations.Interpolators
{
    public class Arc2Interpolator : Interpolator
    {
        protected override float _GetValue(float input)
        {
            return Interpolations.Arch2(input);
        }
    }
}