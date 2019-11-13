namespace Engine.UI.Animations.Interpolators
{
    public class StaticInterpolator : Interpolator
    {
        public float Value = 0;

        protected override float _GetValue(float input)
        {
            return Value;
        }
    }
}