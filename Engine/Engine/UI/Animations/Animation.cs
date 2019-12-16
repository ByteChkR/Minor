namespace Engine.UI.Animations
{
    /// <summary>
    /// The Abstract Animation Class that is used for Animating UI Elements
    /// </summary>
    public abstract class Animation
    {
        private int frameCount;
        private bool isLoaded;
        /// <summary>
        /// Trigger of the Animation
        /// </summary>
        public AnimationTrigger Trigger { get; set; }
        /// <summary>
        /// Delay before the animation starts
        /// </summary>
        public float AnimationDelay { get; set; }
        private float TimeSinceAnimationStart { get; set; }

        /// <summary>
        /// Flag that indicates if the animation is currently playing
        /// </summary>
        public bool IsAnimating { get; private set; }

        /// <summary>
        /// Abstract animation implementation
        /// </summary>
        /// <param name="target">target of the animation</param>
        /// <param name="animationStart">the animation start in seconds</param>
        /// <returns></returns>
        public abstract bool Animate(UiElement target, float animationStart);

        /// <summary>
        /// Checks the state of the object
        /// Starts animating when triggered
        /// </summary>
        /// <param name="trigger">trigger to check against</param>
        public void CheckState(AnimationTrigger trigger)
        {
            if (Trigger == trigger)
            {
                IsAnimating = true;
                TimeSinceAnimationStart = 0;
            }
        }
        /// <summary>
        /// Updates a UI Element
        /// </summary>
        /// <param name="target">The UI Element to Update</param>
        /// <param name="deltaTime">Delta Time in seconds</param>
        public void Update(UiElement target, float deltaTime)
        {
            if (!isLoaded && frameCount > 1
            ) //To Prevent loading lag we will call OnLoad event after 2 frames. To ensure that delta time is stable
            {
                isLoaded = true;
                CheckState(AnimationTrigger.OnLoad);
            }
            else if (!isLoaded)
            {
                frameCount++;
            }

            if (IsAnimating)
            {
                TimeSinceAnimationStart += deltaTime;
                float time = TimeSinceAnimationStart - AnimationDelay;
                if (time >= 0 && Animate(target, time))
                {
                    IsAnimating = false;
                    TimeSinceAnimationStart = 0;
                }
            }
        }
    }
}