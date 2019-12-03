using System.Collections.Generic;

namespace Engine.UI.Animations
{
    /// <summary>
    /// Class used to interpolate between two values
    /// </summary>
    public abstract class Interpolator
    {
        public delegate float Interpolate(float input);

        private List<Interpolator> subInterpolators = new List<Interpolator>();

        public Interpolator()
        {
        }

        protected Interpolator(List<Interpolator> subInterpolators)
        {
            this.subInterpolators = subInterpolators;
        }

        public float GetValue(float input)
        {
            foreach (Interpolator subInterpolator in subInterpolators)
            {
                input = subInterpolator.GetValue(input);
            }

            return _GetValue(input);
        }

        protected abstract float _GetValue(float input);
    }
}