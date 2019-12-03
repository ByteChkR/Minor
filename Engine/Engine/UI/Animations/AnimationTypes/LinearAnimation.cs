using Engine.UI.Animations.Interpolators;
using OpenTK;

namespace Engine.UI.Animations.AnimationTypes
{
    /// <summary>
    /// Implementation for a linear animation
    /// </summary>
    public class LinearAnimation : Animation
    {
        /// <summary>
        /// The End Position of the Animation
        /// </summary>
        public Vector2 EndPos;

        /// <summary>
        /// The Interpolator used to Animate
        /// </summary>
        public Interpolator Interpolator = new LinearInterpolator();

        /// <summary>
        /// The Maximum Animation Time
        /// </summary>
        public float MaxAnimationTime = 1;
        /// <summary>
        /// The Start Position of the Animation
        /// </summary>
        public Vector2 StartPos;

        /// <summary>
        /// Animates a Target
        /// </summary>
        /// <param name="target">Target to animate</param>
        /// <param name="timeSinceAnimationStart"></param>
        /// <returns></returns>
        public override bool Animate(UiElement target, float timeSinceAnimationStart)
        {
            target.Position = Vector2.Lerp(StartPos, EndPos,
                Interpolator.GetValue(timeSinceAnimationStart / MaxAnimationTime));
            return MaxAnimationTime <= timeSinceAnimationStart;
        }
    }
}