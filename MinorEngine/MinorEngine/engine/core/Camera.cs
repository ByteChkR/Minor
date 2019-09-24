using OpenTK;

namespace MinorEngine.engine.core
{
    public class Camera : GameObject
    {
        public Matrix4 Projection { get; set; }


        public Camera(Matrix4 projection, Vector3 position) : base(position, "Camera")
        {
            Projection = projection;
        }
    }
}