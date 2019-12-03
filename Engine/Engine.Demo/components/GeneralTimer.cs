using System;
using Engine.Core;

namespace Engine.Demo.components
{
    public class GeneralTimer : AbstractComponent
    {
        private float destroyTime;
        private Action onTrigger;
        private float time;

        public GeneralTimer(float destroyTime, Action onTrigger)
        {
            this.destroyTime = destroyTime;
            this.onTrigger = onTrigger;
        }


        protected override void Update(float deltaTime)
        {
            time += deltaTime;
            if (time >= destroyTime)
            {
                onTrigger?.Invoke();
            }
        }
    }
}