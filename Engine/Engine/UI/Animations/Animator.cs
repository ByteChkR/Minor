using System.Collections.Generic;
using Engine.Core;
using Engine.UI.EventSystems;

namespace Engine.UI.Animations
{
    public class Animator : AbstractComponent
    {
        protected List<UIElement> Targets = new List<UIElement>();
        private List<Animation> animators = new List<Animation>();

        public Animator(List<Animation> animatorList, params UIElement[] targets)
        {
            animators = animatorList;
            AddTargets(targets);
        }

        public void AddTargets(params UIElement[] elements)
        {
            for (int i = 0; i < elements.Length; i++)
            {
                AddTarget(elements[i]);
            }
        }

        public void AddTarget(UIElement target)
        {
            Targets.Add(target);
            if (target is Button btn)
            {
                btn.AddToClickEvent(OnClick);
                btn.AddToEnterEvent(OnEnter);
                btn.AddToHoverEvent(OnHover);
                btn.AddToLeaveEvent(OnLeave);
            }
        }

        private void RemoveTarget(UIElement target)
        {
            if (Targets.Contains(target))
            {
                if (target is Button btn)
                {
                    btn.RemoveFromClickEvent(OnClick);
                    btn.RemoveFromEnterEvent(OnEnter);
                    btn.RemoveFromHoverEvent(OnHover);
                    btn.RemoveFromLeaveEvent(OnLeave);
                }

                Targets.Remove(target);
            }
        }

        public void TriggerEvent(AnimationTrigger uiEvent)
        {
            foreach (Animation ad in animators)
            {
                ad.CheckState(uiEvent);
            }
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
                foreach (UIElement uiElement in Targets)
                {
                    animation.Update(uiElement, deltaTime);
                }
            }
        }
    }
}