using GameEngine.engine.core;

namespace GameEngine.engine.components
{
    public abstract class AbstractComponent
    {
        public GameObject owner;
        public virtual void Update(float deltaTime)
        {

        }
    }
}