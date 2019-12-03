namespace Engine.UI.Animations.Interpolators
{
    /// <summary>
    /// Linear Interpolator implementation
    /// </summary>
    public class LinearInterpolator : Interpolator
    {
        protected override float _GetValue(float input)
        {
            return input;
        }
    }
}