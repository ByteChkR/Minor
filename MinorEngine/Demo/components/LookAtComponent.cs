using Engine.Core;
namespace Demo.components
{
    public class LookAtComponent : AbstractComponent
    {
        private GameObject target;
        public bool IsLooking => target != null;

        protected override void Awake()
        {
        }

        public void SetTarget(GameObject target)
        {
            this.target = target;
        }

        protected override void Update(float deltaTime)
        {
            if (target != null)
            {
                Owner.LookAt(target);
            }
        }
    }
}