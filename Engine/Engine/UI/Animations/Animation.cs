using System;
using System.Collections.Generic;
using Engine.Core;
using Engine.UI.EventSystems;
using OpenTK;

namespace Engine.UI.Animations
{

    public abstract class Animation
    {
        public AnimationTrigger Trigger { get; set; }
        public string Type { get; }
        protected float TimeSinceAnimationStart { get; private set; }
        public bool IsAnimating { get; private set; }
        private bool isLoaded = false;
        private int frameCount = 0;
        protected Button Target;

        protected Animation(Button target)
        {
            Target = target;
        }
        public abstract bool Animate(Button target);

        public void CheckState(AnimationTrigger trigger)
        {
            if (Trigger == trigger)
            {
                IsAnimating = true;
                TimeSinceAnimationStart = 0;
            }
        }

        public void Update(Button target, float deltaTime)
        {
            if (!isLoaded && frameCount > 1) //To Prevent loading lag we will call OnLoad event after 2 frames. To ensure that delta time is stable
            {
                isLoaded = true;
                CheckState(AnimationTrigger.OnLoad);
            }
            else if (!isLoaded)
                frameCount++;

            if (IsAnimating)
            {
                TimeSinceAnimationStart += deltaTime;
                if (Animate(target))
                {
                    IsAnimating = false;
                    TimeSinceAnimationStart = 0;
                }
            }
        }

    }
}