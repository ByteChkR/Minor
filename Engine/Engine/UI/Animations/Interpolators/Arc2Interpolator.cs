﻿using System.Collections.Generic;
using Engine.Core;

namespace Engine.UI.Animations.Interpolators
{
    /// <summary>
    /// Arc2 Interpolator implementation
    /// </summary>
    public class Arc2Interpolator : Interpolator
    {
        public Arc2Interpolator()
        {
        }

        public Arc2Interpolator(List<Interpolator> subInterpolators) : base(subInterpolators)
        {
        }

        protected override float _GetValue(float input)
        {
            return Interpolations.Arch2(input);
        }
    }
}