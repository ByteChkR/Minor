using Engine.UI.Animations.Interpolators;
using OpenTK;

namespace Engine.UI.Animations.AnimationTypes
{
    public class LinearAnimation : Animation
    {
        public Vector2 EndPos;
        public Interpolator Interpolator = new LinearInterpolator();
        public float MaxAnimationTime = 1;
        public Vector2 StartPos;

        public override bool Animate(UIElement target, float timeSinceAnimationStart)
        {
            target.Position = Vector2.Lerp(StartPos, EndPos,
                Interpolator.GetValue(timeSinceAnimationStart / MaxAnimationTime));
            return MaxAnimationTime <= timeSinceAnimationStart;
        }
    }
}