using System;
using Engine.Core;

namespace Engine.Demo.components
{
    public class GeneralTimer : AbstractComponent
    {
        private float _destroyTime;
        private Action _onTrigger;
        private float _time;

        public GeneralTimer(float destroyTime, Action onTrigger)
        {
            _destroyTime = destroyTime;
            _onTrigger = onTrigger;
        }


        protected override void Update(float deltaTime)
        {
            _time += deltaTime;
            if (_time >= _destroyTime)
            {
                _onTrigger?.Invoke();
            }
        }
    }
}