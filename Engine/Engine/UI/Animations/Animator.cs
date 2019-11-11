using System.Collections.Generic;
using Engine.Core;
using Engine.UI.EventSystems;

namespace Engine.UI.Animations
{
    public class Animator : AbstractComponent
    {
        protected Button Target;
        List<Animation> animators = new List<Animation>();
        public Animator(Button target, List<Animation> animatorList)
        {
            animators = animatorList;
            AddTarget(target);
        }

        private void AddTarget(Button target)
        {
            Target = target;
            Target.AddToClickEvent(OnClick);
            Target.AddToEnterEvent(OnEnter);
            Target.AddToHoverEvent(OnHover);
            Target.AddToLeaveEvent(OnLeave);
        }

        private void RemoveTarget()
        {
            Target.RemoveFromClickEvent(OnClick);
            Target.RemoveFromEnterEvent(OnEnter);
            Target.RemoveFromHoverEvent(OnHover);
            Target.RemoveFromLeaveEvent(OnLeave);
            Target = null;
        }

        private void SetTarget(Button target)
        {
            RemoveTarget();
            AddTarget(target);
        }

        private void OnClick(Button target)
        {
            foreach (Animation ad in animators)
            {
                ad.CheckState(AnimationTrigger.OnClick);
            }
        }

        private void OnEnter(Button target)
        {
            foreach (Animation ad in animators)
            {
                ad.CheckState(AnimationTrigger.OnEnter);
            }
        }
        private void OnHover(Button target)
        {
            foreach (Animation ad in animators)
            {
                ad.CheckState(AnimationTrigger.OnHover);
            }
        }
        private void OnLeave(Button target)
        {
            foreach (Animation ad in animators)
            {
                ad.CheckState(AnimationTrigger.OnLeave);
            }
        }

        protected override void Update(float deltaTime)
        {
            base.Update(deltaTime);
            foreach (Animation animation in animators)
            {
                animation.Update(Target, deltaTime);
            }
        }
    }
}