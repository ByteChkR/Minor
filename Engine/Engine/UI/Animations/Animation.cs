namespace Engine.UI.Animations
{
    public abstract class Animation
    {
        private int frameCount;
        private bool isLoaded;
        public AnimationTrigger Trigger { get; set; }
        public string Type { get; set; }
        public float AnimationDelay { get; set; }
        private float TimeSinceAnimationStart { get; set; }
        public bool IsAnimating { get; private set; }

        public abstract bool Animate(UIElement target, float animationStart);

        public void CheckState(AnimationTrigger trigger)
        {
            if (Trigger == trigger)
            {
                IsAnimating = true;
                TimeSinceAnimationStart = 0;
            }
        }

        public void Update(UIElement target, float deltaTime)
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