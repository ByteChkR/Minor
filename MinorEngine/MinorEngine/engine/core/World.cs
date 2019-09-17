using OpenTK;

namespace GameEngine.engine.core
{
    public class World : GameObject
    {
        public Camera camera { get; private set; } = null;

        public World() : base(Vector3.Zero, "World", null)
        {
            world = this;
        }

        public void SetCamera(Camera c)
        {
            if (c != null) camera = c;
        }
    }
}