namespace Engine.UI.Animations.Interpolators
{
    /// <summary>
    /// Linear Interpolator implementation
    /// </summary>
    public class LinearInterpolator : Interpolator
    {
        /// <summary>
        /// Implements the Linear Interpolation
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        protected override float _GetValue(float input)
        {
            return input;
        }
    }
}