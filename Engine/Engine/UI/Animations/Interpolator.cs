using System.Collections.Generic;

namespace Engine.UI.Animations
{
    /// <summary>
    /// Class used to interpolate between two values
    /// </summary>
    public abstract class Interpolator
    {
        /// <summary>
        /// Delegate that is used by all Smoothing functions
        /// </summary>
        /// <param name="input"></param>
        /// <returns>interpolated result</returns>
        public delegate float Interpolate(float input);

        private List<Interpolator> subInterpolators = new List<Interpolator>();

        /// <summary>
        /// Default Constructor
        /// </summary>
        protected Interpolator()
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="subInterpolators">List of Subinterpolators that will be run before this interpolator</param>
        protected Interpolator(List<Interpolator> subInterpolators)
        {
            this.subInterpolators = subInterpolators;
        }

        /// <summary>
        /// Returns the interpolated value from the interpolator
        /// </summary>
        /// <param name="input">input value</param>
        /// <returns>interpolated value</returns>
        public float GetValue(float input)
        {
            foreach (Interpolator subInterpolator in subInterpolators)
            {
                input = subInterpolator.GetValue(input);
            }

            return _GetValue(input);
        }

        /// <summary>
        /// Abstract Interpolator Implementation
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        protected abstract float _GetValue(float input);
    }
}