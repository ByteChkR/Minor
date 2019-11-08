using System;
using Engine.Core;

namespace Engine.Demo.components
{
    public class GeneralTimer : AbstractComponent
    {
        private float _destroyTime;
        private float _time;
        private Action _onTrigger;

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