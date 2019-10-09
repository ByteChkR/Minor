using Engine.Rendering;

namespace Engine.Core
{
    public abstract class AbstractScene : GameObject
    {

        public BasicCamera Camera { get; private set; }
        public void SetCamera(BasicCamera c)
        {
            if (c != null)
            {
                Camera = c;
            }
        }

        protected AbstractScene(string sceneName = "Scene") : base(sceneName)
        {
            Scene = this;
        }
        internal void _initializeScene()
        {
            InitializeScene();
        }

        public void DestroyScene()
        {
            OnDestroy();
        }


        protected abstract void InitializeScene();

        public virtual void OnDestroy()
        {
        }
    }
}