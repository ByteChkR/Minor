using Engine.Core;

namespace Demo.components
{
    public class DestroyTimer : AbstractComponent
    {
        private float _destroyTime;
        private float _time;

        public DestroyTimer(float destroyTime)
        {
            _destroyTime = destroyTime;
        }


        protected override void Update(float deltaTime)
        {
            _time += deltaTime;
            if (_time >= _destroyTime)
            {
                Owner.Destroy();
            }
        }
    }
}