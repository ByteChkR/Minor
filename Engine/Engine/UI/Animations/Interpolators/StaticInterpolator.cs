namespace Engine.UI.Animations.Interpolators
{

    /// <summary>
    /// Static Interpolator implementation
    /// </summary>
    public class StaticInterpolator : Interpolator
    {
        public float Value { get; set; } = 0;

        protected override float _GetValue(float input)
        {
            return Value;
        }
    }
}