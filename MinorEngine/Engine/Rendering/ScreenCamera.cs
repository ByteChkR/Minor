using Engine.Rendering;
using OpenTK;

namespace Engine.Rendering
{
    public class ScreenCamera : ICamera
    {
        public Matrix4 Projection { get; }
        public Matrix4 ViewMatrix => Matrix4.Invert(GetWorldTransform());

        public ScreenCamera()
        {
            Projection = Matrix4.CreateOrthographic(3,
                3, -10, 100);
        }

        public Matrix4 GetWorldTransform()
        {
            return Matrix4.Identity;
        }
    }
}