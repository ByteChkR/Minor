using System.Collections.Generic;
using Engine.Core;

namespace Engine.UI.Animations.Interpolators
{
    /// <summary>
    /// Arc2 Interpolator implementation
    /// </summary>
    public class Arc2Interpolator : Interpolator
    {
        /// <summary>
        /// Default Constructor
        /// </summary>
        public Arc2Interpolator()
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="subInterpolators">A list of Sub Interpolator</param>
        public Arc2Interpolator(List<Interpolator> subInterpolators) : base(subInterpolators)
        {
        }

        /// <summary>
        /// Implements the Arch2 Interpolation
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        protected override float _GetValue(float input)
        {
            return Interpolations.Arch2(input);
        }
    }
}