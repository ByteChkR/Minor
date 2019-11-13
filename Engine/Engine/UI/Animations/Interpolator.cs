using System;
using System.Collections.Generic;
using Engine.Core;
using Engine.Physics.BEPUphysics.EntityStateManagement;

namespace Engine.UI.Animations
{
    public abstract class Interpolator
    {
        private List<Interpolator> SubInterpolators = new List<Interpolator>();

        public delegate float Interpolate(float input);

        public Interpolator()
        {
        }

        protected Interpolator(List<Interpolator> subInterpolators)
        {
            SubInterpolators = subInterpolators;
        }

        public float GetValue(float input)
        {
            foreach (Interpolator subInterpolator in SubInterpolators)
            {
                input = subInterpolator.GetValue(input);
            }

            return _GetValue(input);
        }

        protected abstract float _GetValue(float input);
    }
}