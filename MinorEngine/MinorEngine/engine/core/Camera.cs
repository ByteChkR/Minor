using GameEngine.engine.rendering;
using OpenTK;

namespace GameEngine.engine.core
{
    public class Camera : GameObject, ICamera
    {
        public Matrix4 Projection { get; set; }
        public Matrix4 ViewMatrix => Matrix4.Invert(GetWorldTransform());

        public Camera(Matrix4 projection, Vector3 position) : base(position, "Camera")
        {
            Projection = projection;
        }
    }
}