using System.Collections.Generic;
using Engine.UI.Animations.Interpolators;
using Engine.UI.EventSystems;
using OpenTK;

namespace Engine.UI.Animations.AnimationTypes
{
    public class LinearAnimation : Animation
    {
        public float MaxAnimationTime = 1;
        public Vector2 StartPos;
        public Vector2 EndPos;
        public Interpolator Interpolator = new LinearInterpolator();

        public override bool Animate(UIElement target, float timeSinceAnimationStart)
        {
            target.Position = Vector2.Lerp(StartPos, EndPos,
                Interpolator.GetValue(timeSinceAnimationStart / MaxAnimationTime));
            return MaxAnimationTime <= timeSinceAnimationStart;
        }
    }
}