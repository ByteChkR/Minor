using System.Collections.Generic;
using Engine.Core;
using Engine.UI.EventSystems;

namespace Engine.UI.Animations
{
    /// <summary>
    /// Class used to play back animations once an animation is triggered
    /// </summary>
    public class Animator : AbstractComponent
    {
        private List<Animation> animators = new List<Animation>();
        /// <summary>
        /// A list of All Targets for this animation
        /// </summary>
        protected List<UiElement> Targets = new List<UiElement>();
        /// <summary>
        /// Public Constructor
        /// </summary>
        /// <param name="animatorList">The list of Animations</param>
        /// <param name="targets">The List of Targets for the Animations</param>
        public Animator(List<Animation> animatorList, params UiElement[] targets)
        {
            animators = animatorList;
            AddTargets(targets);
        }

        /// <summary>
        /// Adds Targets for the Animator to animate
        /// </summary>
        /// <param name="elements">elements to add</param>
        public void AddTargets(params UiElement[] elements)
        {
            for (int i = 0; i < elements.Length; i++)
            {
                AddTarget(elements[i]);
            }
        }
        /// <summary>
        /// Adds a Target for the Animator to animate
        /// </summary>
        /// <param name="target">target to add</param>
        public void AddTarget(UiElement target)
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
        /// <summary>
        /// Removes a Target from the Animator Target list
        /// </summary>
        /// <param name="target">target to remove</param>
        private void RemoveTarget(UiElement target)
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

        /// <summary>
        /// Triggers an Event Manually
        /// </summary>
        /// <param name="uiEvent"></param>
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

        /// <summary>
        /// Updates All Animations for All Targets
        /// </summary>
        /// <param name="deltaTime"></param>
        protected override void Update(float deltaTime)
        {
            base.Update(deltaTime);
            foreach (Animation animation in animators)
            {
                foreach (UiElement uiElement in Targets)
                {
                    animation.Update(uiElement, deltaTime);
                }
            }
        }
    }
}