using System.Drawing.Drawing2D;
using GameEngine.engine.core;
using OpenTK;

namespace GameEngine.engine.rendering
{
    public class UICamera : ICamera
    {

        public Matrix4 Projection { get; }
        public Matrix4 ViewMatrix => Matrix4.Invert(GetWorldTransform());

        public UICamera()
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