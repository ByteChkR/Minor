using MinorEngine.engine.core;

namespace MinorEngine.engine.components
{
    public abstract class AbstractComponent
    {
        public GameObject Owner { get; set; }

        public virtual void Update(float deltaTime)
        {

        }
    }
}