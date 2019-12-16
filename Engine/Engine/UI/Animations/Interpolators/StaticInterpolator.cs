namespace Engine.UI.Animations.Interpolators
{

    /// <summary>
    /// Static Interpolator implementation
    /// </summary>
    public class StaticInterpolator : Interpolator
    {
        /// <summary>
        /// The value to be returned
        /// </summary>
        public float Value { get; set; } = 0;

        /// <summary>
        /// Implements the Static Interpolation
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        protected override float _GetValue(float input)
        {
            return Value;
        }
    }
}