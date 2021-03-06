﻿using System;
using Engine.Physics.BEPUphysics.Constraints.TwoEntity.Joints;
using Engine.Physics.BEPUutilities;

namespace Engine.Physics.BEPUphysics.Constraints.TwoEntity.JointLimits
{
    /// <summary>
    /// Superclass of constraints which have a limited area of free movement.
    /// </summary>
    public abstract class JointLimit : Joint
    {
        /// <summary>
        /// Minimum velocity necessary for a bounce to occur at a joint limit.
        /// </summary>
        protected float bounceVelocityThreshold = 1;

        /// <summary>
        /// Bounciness of this joint limit.  0 is completely inelastic; 1 is completely elastic.
        /// </summary>
        protected float bounciness;

        protected bool isLimitActive;

        /// <summary>
        /// Small area that the constraint can be violated without applying position correction.  Helps avoid jitter.
        /// </summary>
        protected float margin = 0.005f;

        /// <summary>
        /// Gets or sets the minimum velocity necessary for a bounce to occur at a joint limit.
        /// </summary>
        public float BounceVelocityThreshold
        {
            get => bounceVelocityThreshold;
            set => bounceVelocityThreshold = Math.Max(0, value);
        }

        /// <summary>
        /// Gets or sets the bounciness of this joint limit.  0 is completely inelastic; 1 is completely elastic.
        /// </summary>
        public float Bounciness
        {
            get => bounciness;
            set => bounciness = MathHelper.Clamp(value, 0, 1);
        }

        /// <summary>
        /// Gets whether or not the limit is currently exceeded.  While violated, the constraint will apply impulses in an attempt to stop further violation and to correct any current error.
        /// This is true whenever the limit is touched.
        /// </summary>
        public bool IsLimitExceeded => isLimitActive;

        /// <summary>
        /// Gets or sets the small area that the constraint can be violated without applying position correction.  Helps avoid jitter.
        /// </summary>
        public float Margin
        {
            get => margin;
            set => margin = MathHelper.Max(value, 0);
        }

        /// <summary>
        /// Computes the bounce velocity for this limit.
        /// </summary>
        /// <param name="impactVelocity">Velocity of the impact on the limit.</param>
        /// <returns>The resulting bounce velocity of the impact.</returns>
        protected float ComputeBounceVelocity(float impactVelocity)
        {
            float lowThreshold = bounceVelocityThreshold * 0.3f;
            float velocityFraction =
                MathHelper.Clamp(
                    (impactVelocity - lowThreshold) / (bounceVelocityThreshold - lowThreshold + Toolbox.Epsilon), 0, 1);
            return velocityFraction * impactVelocity * Bounciness;
        }
    }
}