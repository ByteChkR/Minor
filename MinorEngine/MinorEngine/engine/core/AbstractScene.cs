namespace MinorEngine.engine.core
{
    public abstract class AbstractScene
    {
        internal void _initializeScene(World world)
        {
            InitializeScene();
        }

        public void Destroy()
        {
            OnDestroy();
        }


        protected abstract void InitializeScene();

        public virtual void OnDestroy()
        {
        }

        public virtual void Update(float deltaTime)
        {
        }
    }
}