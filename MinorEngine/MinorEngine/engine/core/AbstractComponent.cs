using GameEngine.engine.core;

namespace GameEngine.engine.components
{
    public abstract class AbstractComponent
    {
        public GameObject Owner { get; set; }

        public virtual void Update(float deltaTime)
        {

        }
    }
}