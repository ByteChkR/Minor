using Engine.Core;
using OpenTK;

namespace Engine.Rendering
{
    public class BasicCamera : GameObject, ICamera
    {
        public Matrix4 Projection { get; set; }
        public Matrix4 ViewMatrix => Matrix4.Invert(GetWorldTransform());

        public BasicCamera(Matrix4 projection, Vector3 localPosition) : base(localPosition, "Camera")
        {
            Projection = projection;
        }
    }
}